using System.Collections.Generic;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Enemies
{
    /// <summary>
    /// Handles spawning enemies in rooms based on RoomData configuration.
    /// Manages enemy lifecycle and respawning behavior.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        public static EnemySpawner Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private Transform enemyContainer;
        [SerializeField] private bool respawnOnReentry = true;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnDelay = 0.5f;
        [SerializeField] private int maxEnemiesPerRoom = 10;

        // Current room's enemies
        private List<Enemy> _currentRoomEnemies = new();
        private string _currentRoomId;

        // Track killed enemies (don't respawn in same session)
        private HashSet<string> _killedEnemyIds = new();

        // Events
        public event System.Action<Enemy> OnEnemySpawned;
        public event System.Action<Enemy> OnEnemyKilled;

        // Properties
        public int ActiveEnemyCount => _currentRoomEnemies.Count;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Subscribe to room changes
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadStarted += OnRoomUnloading;
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomLoaded;
            }

            // Ensure container exists
            if (enemyContainer == null)
            {
                var containerObj = GameObject.Find("EnemyContainer");
                if (containerObj == null)
                {
                    containerObj = new GameObject("EnemyContainer");
                }
                enemyContainer = containerObj.transform;
            }
        }

        private void OnDestroy()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadStarted -= OnRoomUnloading;
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomLoaded;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnRoomUnloading(string roomId)
        {
            ClearCurrentRoomEnemies();
        }

        private void OnRoomLoaded(RoomData roomData)
        {
            if (roomData == null) return;

            _currentRoomId = roomData.roomId;
            StartCoroutine(SpawnRoomEnemiesDelayed(roomData));
        }

        private System.Collections.IEnumerator SpawnRoomEnemiesDelayed(RoomData roomData)
        {
            yield return new WaitForSeconds(spawnDelay);
            SpawnRoomEnemies(roomData);
        }

        /// <summary>
        /// Spawns all enemies defined in the room data.
        /// </summary>
        public void SpawnRoomEnemies(RoomData roomData)
        {
            if (roomData.enemySpawns == null) return;

            int spawnedCount = 0;

            foreach (var enemySpawn in roomData.enemySpawns)
            {
                if (spawnedCount >= maxEnemiesPerRoom) break;

                if (TrySpawnEnemy(enemySpawn, roomData.roomId))
                {
                    spawnedCount++;
                }
            }

            Debug.Log($"[EnemySpawner] Spawned {spawnedCount} enemies in {roomData.roomId}");
        }

        private bool TrySpawnEnemy(EnemySpawn spawn, string roomId)
        {
            if (spawn.enemyData == null)
            {
                // Try to get from database
                return false;
            }

            // Generate unique ID for tracking
            string uniqueId = $"{roomId}_{spawn.enemyData.enemyId}_{spawn.position.x}_{spawn.position.y}";

            // Check if this enemy was killed (and shouldn't respawn)
            if (!spawn.respawnsOnReentry && _killedEnemyIds.Contains(uniqueId))
            {
                return false;
            }

            // Check spawn chance
            if (Random.value > spawn.spawnChance)
            {
                return false;
            }

            // Check max per room
            int currentCount = CountEnemiesOfType(spawn.enemyData.enemyType);
            if (currentCount >= spawn.enemyData.maxPerRoom)
            {
                return false;
            }

            // Spawn the enemy
            var enemy = SpawnEnemy(spawn.enemyData, spawn.position);
            if (enemy != null)
            {
                enemy.name = $"Enemy_{uniqueId}";

                // Track for respawn behavior
                if (!spawn.respawnsOnReentry)
                {
                    enemy.OnEnemyDestroyed += (e) => _killedEnemyIds.Add(uniqueId);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Spawns a single enemy at a position.
        /// </summary>
        public Enemy SpawnEnemy(EnemyData enemyData, Vector2 position)
        {
            var enemyObj = new GameObject($"Enemy_{enemyData.displayName}");
            enemyObj.transform.SetParent(enemyContainer);
            enemyObj.transform.position = position;

            // Add required components
            var rb = enemyObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;

            var col = enemyObj.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;

            var sr = enemyObj.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Enemies";

            // Add enemy component
            var enemy = enemyObj.AddComponent<Enemy>();
            enemy.Initialize(enemyData, position);

            // Subscribe to death
            enemy.OnEnemyDestroyed += OnEnemyDestroyed;

            _currentRoomEnemies.Add(enemy);
            OnEnemySpawned?.Invoke(enemy);

            return enemy;
        }

        /// <summary>
        /// Spawns an enemy by type ID at a position.
        /// </summary>
        public Enemy SpawnEnemyByType(EnemyType type, Vector2 position)
        {
            var enemyData = EnemyDatabase.GetEnemy(type);
            if (enemyData == null)
            {
                Debug.LogWarning($"[EnemySpawner] Unknown enemy type: {type}");
                return null;
            }

            return SpawnEnemy(enemyData, position);
        }

        private void OnEnemyDestroyed(Enemy enemy)
        {
            _currentRoomEnemies.Remove(enemy);
            OnEnemyKilled?.Invoke(enemy);
        }

        /// <summary>
        /// Clears all current room enemies.
        /// </summary>
        public void ClearCurrentRoomEnemies()
        {
            foreach (var enemy in _currentRoomEnemies)
            {
                if (enemy != null)
                {
                    enemy.OnEnemyDestroyed -= OnEnemyDestroyed;
                    Destroy(enemy.gameObject);
                }
            }
            _currentRoomEnemies.Clear();
        }

        /// <summary>
        /// Counts enemies of a specific type in the current room.
        /// </summary>
        public int CountEnemiesOfType(EnemyType type)
        {
            int count = 0;
            foreach (var enemy in _currentRoomEnemies)
            {
                if (enemy != null && enemy.Data?.enemyType == type)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Gets all enemies in range of a position.
        /// </summary>
        public List<Enemy> GetEnemiesInRange(Vector2 position, float range)
        {
            var result = new List<Enemy>();
            foreach (var enemy in _currentRoomEnemies)
            {
                if (enemy != null && enemy.IsAlive)
                {
                    float dist = Vector2.Distance(position, enemy.transform.position);
                    if (dist <= range)
                    {
                        result.Add(enemy);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Resets killed enemy tracking (for new game).
        /// </summary>
        public void ResetKilledEnemies()
        {
            _killedEnemyIds.Clear();
        }
    }
}
