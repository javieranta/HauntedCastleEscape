using System;
using UnityEngine;
using HauntedCastle.Services;
using HauntedCastle.Data;

namespace HauntedCastle.Core
{
    /// <summary>
    /// Manages player score, multipliers, and combos.
    /// Tracks points from defeating enemies, collecting items, and discovering secrets.
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [Header("Score Settings")]
        [SerializeField] private int startingScore = 0;
        [SerializeField] private float comboTimeWindow = 2f;
        [SerializeField] private int maxComboMultiplier = 5;

        [Header("Point Values")]
        [SerializeField] private int enemyKillBase = 100;
        [SerializeField] private int roomDiscoveryBonus = 50;
        [SerializeField] private int secretFoundBonus = 200;
        [SerializeField] private int floorClearedBonus = 500;
        [SerializeField] private int noDamageRoomBonus = 100;

        // State
        private int _currentScore;
        private int _comboCount;
        private float _comboTimer;
        private int _roomsDiscovered;
        private int _secretsFound;
        private int _enemiesDefeated;
        private bool _tookDamageInRoom;

        // Events
        public event Action<int> OnScoreChanged;
        public event Action<int, int> OnComboChanged; // current combo, multiplier
        public event Action<int, string> OnPointsAwarded; // points, reason

        // Properties
        public int CurrentScore => _currentScore;
        public int ComboCount => _comboCount;
        public int ComboMultiplier => Mathf.Min(_comboCount + 1, maxComboMultiplier);
        public int RoomsDiscovered => _roomsDiscovered;
        public int SecretsFound => _secretsFound;
        public int EnemiesDefeated => _enemiesDefeated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            ResetScore();
        }

        private void Start()
        {
            // Subscribe to game events
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomEntered;
            }
        }

        private void OnDestroy()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomEntered;
            }
        }

        private void Update()
        {
            // Update combo timer
            if (_comboCount > 0)
            {
                _comboTimer -= Time.deltaTime;
                if (_comboTimer <= 0)
                {
                    ResetCombo();
                }
            }
        }

        /// <summary>
        /// Resets the score to starting value.
        /// </summary>
        public void ResetScore()
        {
            _currentScore = startingScore;
            _comboCount = 0;
            _comboTimer = 0;
            _roomsDiscovered = 0;
            _secretsFound = 0;
            _enemiesDefeated = 0;
            _tookDamageInRoom = false;

            OnScoreChanged?.Invoke(_currentScore);
        }

        /// <summary>
        /// Adds points to the score.
        /// </summary>
        public void AddPoints(int basePoints, string reason = "")
        {
            int multiplier = ComboMultiplier;
            int totalPoints = basePoints * multiplier;

            _currentScore += totalPoints;

            OnScoreChanged?.Invoke(_currentScore);
            OnPointsAwarded?.Invoke(totalPoints, reason);

            // Show visual feedback
            ShowPointsPopup(totalPoints, reason);

            Debug.Log($"[ScoreManager] +{totalPoints} points ({basePoints} x{multiplier}) - {reason}. Total: {_currentScore}");
        }

        /// <summary>
        /// Awards points for defeating an enemy.
        /// </summary>
        public void AwardEnemyKill(EnemyType enemyType)
        {
            _enemiesDefeated++;

            // Different enemies have different point values
            int points = enemyType switch
            {
                EnemyType.Bat => enemyKillBase / 2,
                EnemyType.Spider => enemyKillBase / 2,
                EnemyType.Ghost => enemyKillBase,
                EnemyType.Skeleton => enemyKillBase,
                EnemyType.Mummy => (int)(enemyKillBase * 1.5f),
                EnemyType.Demon => enemyKillBase * 2,
                EnemyType.Witch => enemyKillBase * 2,
                EnemyType.Vampire => (int)(enemyKillBase * 2.5f),
                EnemyType.Werewolf => (int)(enemyKillBase * 2.5f),
                EnemyType.Reaper => enemyKillBase * 3,
                _ => enemyKillBase
            };

            // Extend combo
            ExtendCombo();

            AddPoints(points, $"Defeated {enemyType}");
        }

        /// <summary>
        /// Awards points for discovering a new room.
        /// </summary>
        public void AwardRoomDiscovery(string roomId)
        {
            _roomsDiscovered++;
            AddPoints(roomDiscoveryBonus, "Room Discovered");
        }

        /// <summary>
        /// Awards points for finding a secret.
        /// </summary>
        public void AwardSecretFound(string secretType)
        {
            _secretsFound++;
            AddPoints(secretFoundBonus, $"Secret Found: {secretType}");
        }

        /// <summary>
        /// Awards bonus for clearing a floor.
        /// </summary>
        public void AwardFloorCleared(int floorNumber)
        {
            string floorName = floorNumber switch
            {
                0 => "Basement",
                1 => "Castle",
                2 => "Tower",
                _ => "Floor"
            };
            AddPoints(floorClearedBonus, $"{floorName} Cleared!");
        }

        /// <summary>
        /// Extends the combo timer.
        /// </summary>
        public void ExtendCombo()
        {
            _comboCount++;
            _comboTimer = comboTimeWindow;

            OnComboChanged?.Invoke(_comboCount, ComboMultiplier);

            Debug.Log($"[ScoreManager] Combo: {_comboCount}x (Multiplier: {ComboMultiplier}x)");
        }

        /// <summary>
        /// Resets the combo counter.
        /// </summary>
        public void ResetCombo()
        {
            if (_comboCount > 0)
            {
                Debug.Log($"[ScoreManager] Combo ended at {_comboCount}x");
            }
            _comboCount = 0;
            _comboTimer = 0;

            OnComboChanged?.Invoke(0, 1);
        }

        /// <summary>
        /// Called when player takes damage - breaks combo.
        /// </summary>
        public void OnPlayerDamaged()
        {
            _tookDamageInRoom = true;
            ResetCombo();
        }

        private void OnRoomEntered(RoomData roomData)
        {
            // Award no-damage bonus for previous room
            if (!_tookDamageInRoom && _roomsDiscovered > 0)
            {
                AddPoints(noDamageRoomBonus, "Perfect Room!");
            }

            // Reset for new room
            _tookDamageInRoom = false;
        }

        private void ShowPointsPopup(int points, string reason)
        {
            if (Effects.VisualEffectsManager.Instance != null)
            {
                // Find player position for popup
                var player = FindFirstObjectByType<Player.PlayerController>();
                if (player != null)
                {
                    Color popupColor = _comboCount > 0
                        ? Color.Lerp(Color.white, Color.yellow, _comboCount / (float)maxComboMultiplier)
                        : Color.white;

                    string text = _comboCount > 1
                        ? $"+{points} x{ComboMultiplier}!"
                        : $"+{points}";

                    Effects.VisualEffectsManager.Instance.ShowTextPopup(
                        player.transform.position + Vector3.up * 1.5f,
                        text,
                        popupColor
                    );
                }
            }
        }

        /// <summary>
        /// Gets the final score with bonuses.
        /// </summary>
        public int GetFinalScore()
        {
            int bonus = 0;

            // Time bonus (would be implemented with timer)
            // Lives bonus
            // Completion bonus

            return _currentScore + bonus;
        }

        /// <summary>
        /// Creates the ScoreManager if it doesn't exist.
        /// </summary>
        public static ScoreManager EnsureExists()
        {
            if (Instance == null)
            {
                var obj = new GameObject("ScoreManager");
                Instance = obj.AddComponent<ScoreManager>();
            }
            return Instance;
        }
    }
}
