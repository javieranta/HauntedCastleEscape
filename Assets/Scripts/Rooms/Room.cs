using System.Collections.Generic;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Represents an instantiated room in the game world.
    /// Manages room-specific state and child objects.
    /// </summary>
    public class Room : MonoBehaviour
    {
        [Header("Room Info")]
        [SerializeField] private RoomData roomData;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer backgroundRenderer;
        [SerializeField] private SpriteRenderer[] wallRenderers;

        // Doors
        private Dictionary<DoorDirection, Door> _doors = new();

        // Spawned entities
        private List<GameObject> _spawnedEnemies = new();
        private List<GameObject> _spawnedItems = new();

        // State
        public RoomData Data => roomData;
        public string RoomId => roomData?.roomId ?? "";
        public int FloorNumber => roomData?.floorNumber ?? 0;
        public bool IsStartRoom => roomData?.isStartRoom ?? false;
        public bool IsExitRoom => roomData?.isExitRoom ?? false;

        // Room dimensions
        private const float ROOM_WIDTH = 14f;
        private const float ROOM_HEIGHT = 10f;
        private const float WALL_THICKNESS = 1f;
        private const float DOOR_WIDTH = 2f;

        public void Initialize(RoomData data)
        {
            roomData = data;
            name = $"Room_{data.roomId}";

            SetupBackground();
            SetupAmbience();
            SetupWallColliders();
        }

        /// <summary>
        /// Creates invisible wall colliders around the room perimeter with gaps for doors.
        /// </summary>
        private void SetupWallColliders()
        {
            var wallsContainer = new GameObject("WallColliders");
            wallsContainer.transform.SetParent(transform);
            wallsContainer.transform.localPosition = Vector3.zero;

            float halfWidth = ROOM_WIDTH / 2f;
            float halfHeight = ROOM_HEIGHT / 2f;

            // North wall (top)
            bool hasNorthDoor = roomData.northDoor?.exists == true;
            if (hasNorthDoor)
            {
                // Left segment
                CreateWallCollider(wallsContainer, "NorthWall_Left",
                    new Vector2(-halfWidth / 2f - DOOR_WIDTH / 4f, halfHeight + WALL_THICKNESS / 2f),
                    new Vector2(halfWidth - DOOR_WIDTH / 2f, WALL_THICKNESS));
                // Right segment
                CreateWallCollider(wallsContainer, "NorthWall_Right",
                    new Vector2(halfWidth / 2f + DOOR_WIDTH / 4f, halfHeight + WALL_THICKNESS / 2f),
                    new Vector2(halfWidth - DOOR_WIDTH / 2f, WALL_THICKNESS));
            }
            else
            {
                CreateWallCollider(wallsContainer, "NorthWall",
                    new Vector2(0, halfHeight + WALL_THICKNESS / 2f),
                    new Vector2(ROOM_WIDTH + WALL_THICKNESS * 2, WALL_THICKNESS));
            }

            // South wall (bottom)
            bool hasSouthDoor = roomData.southDoor?.exists == true;
            if (hasSouthDoor)
            {
                CreateWallCollider(wallsContainer, "SouthWall_Left",
                    new Vector2(-halfWidth / 2f - DOOR_WIDTH / 4f, -halfHeight - WALL_THICKNESS / 2f),
                    new Vector2(halfWidth - DOOR_WIDTH / 2f, WALL_THICKNESS));
                CreateWallCollider(wallsContainer, "SouthWall_Right",
                    new Vector2(halfWidth / 2f + DOOR_WIDTH / 4f, -halfHeight - WALL_THICKNESS / 2f),
                    new Vector2(halfWidth - DOOR_WIDTH / 2f, WALL_THICKNESS));
            }
            else
            {
                CreateWallCollider(wallsContainer, "SouthWall",
                    new Vector2(0, -halfHeight - WALL_THICKNESS / 2f),
                    new Vector2(ROOM_WIDTH + WALL_THICKNESS * 2, WALL_THICKNESS));
            }

            // West wall (left)
            bool hasWestDoor = roomData.westDoor?.exists == true;
            if (hasWestDoor)
            {
                CreateWallCollider(wallsContainer, "WestWall_Top",
                    new Vector2(-halfWidth - WALL_THICKNESS / 2f, halfHeight / 2f + DOOR_WIDTH / 4f),
                    new Vector2(WALL_THICKNESS, halfHeight - DOOR_WIDTH / 2f));
                CreateWallCollider(wallsContainer, "WestWall_Bottom",
                    new Vector2(-halfWidth - WALL_THICKNESS / 2f, -halfHeight / 2f - DOOR_WIDTH / 4f),
                    new Vector2(WALL_THICKNESS, halfHeight - DOOR_WIDTH / 2f));
            }
            else
            {
                CreateWallCollider(wallsContainer, "WestWall",
                    new Vector2(-halfWidth - WALL_THICKNESS / 2f, 0),
                    new Vector2(WALL_THICKNESS, ROOM_HEIGHT + WALL_THICKNESS * 2));
            }

            // East wall (right)
            bool hasEastDoor = roomData.eastDoor?.exists == true;
            if (hasEastDoor)
            {
                CreateWallCollider(wallsContainer, "EastWall_Top",
                    new Vector2(halfWidth + WALL_THICKNESS / 2f, halfHeight / 2f + DOOR_WIDTH / 4f),
                    new Vector2(WALL_THICKNESS, halfHeight - DOOR_WIDTH / 2f));
                CreateWallCollider(wallsContainer, "EastWall_Bottom",
                    new Vector2(halfWidth + WALL_THICKNESS / 2f, -halfHeight / 2f - DOOR_WIDTH / 4f),
                    new Vector2(WALL_THICKNESS, halfHeight - DOOR_WIDTH / 2f));
            }
            else
            {
                CreateWallCollider(wallsContainer, "EastWall",
                    new Vector2(halfWidth + WALL_THICKNESS / 2f, 0),
                    new Vector2(WALL_THICKNESS, ROOM_HEIGHT + WALL_THICKNESS * 2));
            }
        }

        private void CreateWallCollider(GameObject parent, string name, Vector2 position, Vector2 size)
        {
            var wall = new GameObject(name);
            wall.transform.SetParent(parent.transform);
            wall.transform.localPosition = position;

            var collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
            collider.isTrigger = false; // Solid collider, not trigger

            // Set to Walls layer if it exists
            int wallLayer = LayerMask.NameToLayer("Walls");
            if (wallLayer >= 0)
            {
                wall.layer = wallLayer;
            }
        }

        private void SetupBackground()
        {
            if (roomData.backgroundSprite != null)
            {
                if (backgroundRenderer == null)
                {
                    var bgObj = new GameObject("Background");
                    bgObj.transform.SetParent(transform);
                    bgObj.transform.localPosition = Vector3.zero;
                    backgroundRenderer = bgObj.AddComponent<SpriteRenderer>();
                    backgroundRenderer.sortingLayerName = "Background";
                    backgroundRenderer.sortingOrder = 0;
                }

                backgroundRenderer.sprite = roomData.backgroundSprite;
            }

            // Apply ambient color
            if (backgroundRenderer != null)
            {
                backgroundRenderer.color = roomData.ambientColor;
            }
        }

        private void SetupAmbience()
        {
            // Ambient sound would be handled by AudioManager
            // For now, just log
            if (roomData.ambientSound != null)
            {
                Debug.Log($"[Room] Would play ambient sound: {roomData.ambientSound.name}");
            }
        }

        /// <summary>
        /// Registers a door with this room.
        /// </summary>
        public void RegisterDoor(DoorDirection direction, Door door)
        {
            _doors[direction] = door;
        }

        /// <summary>
        /// Gets a door by direction.
        /// </summary>
        public Door GetDoor(DoorDirection direction)
        {
            return _doors.TryGetValue(direction, out var door) ? door : null;
        }

        /// <summary>
        /// Checks if a door exists and is passable.
        /// </summary>
        public bool CanPassDoor(DoorDirection direction, List<string> playerInventory)
        {
            var door = GetDoor(direction);
            if (door == null) return false;
            return door.CanPass(playerInventory);
        }

        /// <summary>
        /// Spawns enemies for this room based on room data.
        /// </summary>
        public void SpawnEnemies()
        {
            // Clear existing enemies
            ClearEnemies();

            foreach (var enemySpawn in roomData.enemySpawns)
            {
                if (enemySpawn.enemyData == null) continue;

                // Check spawn chance
                if (Random.value > enemySpawn.spawnChance) continue;

                // Create enemy (placeholder - actual enemy spawning in Milestone 4)
                var enemyObj = new GameObject($"Enemy_{enemySpawn.enemyData.enemyId}");
                enemyObj.transform.SetParent(transform);
                enemyObj.transform.localPosition = enemySpawn.position;

                // Add visual placeholder
                var sr = enemyObj.AddComponent<SpriteRenderer>();
                sr.sprite = enemySpawn.enemyData.sprite;
                sr.color = enemySpawn.enemyData.tintColor;
                sr.sortingLayerName = "Enemies";

                _spawnedEnemies.Add(enemyObj);
            }
        }

        /// <summary>
        /// Spawns items for this room based on room data.
        /// </summary>
        public void SpawnItems(HashSet<string> collectedItemIds = null)
        {
            // Clear existing items
            ClearItems();

            foreach (var itemSpawn in roomData.itemSpawns)
            {
                if (itemSpawn.itemData == null) continue;

                // Check if already collected (for persistent items)
                string itemKey = $"{roomData.roomId}_{itemSpawn.itemData.itemId}";
                if (collectedItemIds != null && collectedItemIds.Contains(itemKey))
                {
                    continue;
                }

                // Create item (placeholder - actual item spawning in Milestone 3)
                var itemObj = new GameObject($"Item_{itemSpawn.itemData.itemId}");
                itemObj.transform.SetParent(transform);

                Vector2 position = itemSpawn.position;
                if (itemSpawn.randomizePosition)
                {
                    position += Random.insideUnitCircle * 2f;
                }
                itemObj.transform.localPosition = position;

                // Add visual placeholder
                var sr = itemObj.AddComponent<SpriteRenderer>();
                sr.sprite = itemSpawn.itemData.worldSprite ?? itemSpawn.itemData.icon;
                sr.color = itemSpawn.itemData.tintColor;
                sr.sortingLayerName = "Items";

                _spawnedItems.Add(itemObj);
            }
        }

        /// <summary>
        /// Clears all spawned enemies.
        /// </summary>
        public void ClearEnemies()
        {
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
            _spawnedEnemies.Clear();
        }

        /// <summary>
        /// Clears all spawned items.
        /// </summary>
        public void ClearItems()
        {
            foreach (var item in _spawnedItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            _spawnedItems.Clear();
        }

        /// <summary>
        /// Gets all spawn points in the room.
        /// </summary>
        public List<SpawnPoint> GetSpawnPoints()
        {
            return roomData?.playerSpawnPoints ?? new List<SpawnPoint>();
        }

        /// <summary>
        /// Gets the spawn position for a given spawn ID.
        /// </summary>
        public Vector2 GetSpawnPosition(string spawnId)
        {
            if (roomData == null) return Vector2.zero;

            foreach (var spawn in roomData.playerSpawnPoints)
            {
                if (spawn.spawnId == spawnId)
                {
                    return spawn.position;
                }
            }

            return Vector2.zero;
        }

        private void OnDestroy()
        {
            ClearEnemies();
            ClearItems();
        }
    }
}
