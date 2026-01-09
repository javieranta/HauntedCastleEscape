using UnityEngine;
using HauntedCastle.Core.GameState;
using HauntedCastle.Player;
using HauntedCastle.Data;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Handles player spawning in the game scene.
    /// Works with RoomManager to position player at correct spawn points.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        public static PlayerSpawner Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private Vector3 defaultSpawnPosition = Vector3.zero;

        [Header("References")]
        [SerializeField] private GameObject playerPrefab;

        private GameObject _currentPlayer;
        private PlayerController _playerController;
        private PlayerHealth _playerHealth;
        private PlayerSetup _playerSetup;

        public GameObject CurrentPlayer => _currentPlayer;
        public PlayerController PlayerController => _playerController;
        public PlayerHealth PlayerHealth => _playerHealth;

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
            // Auto-spawn if no player exists
            if (_currentPlayer == null)
            {
                SpawnPlayer();
            }
        }

        /// <summary>
        /// Spawns the player at the default or specified position.
        /// </summary>
        public void SpawnPlayer(Vector3? position = null)
        {
            // Destroy existing player
            if (_currentPlayer != null)
            {
                Destroy(_currentPlayer);
            }

            Vector3 spawnPos = position ?? defaultSpawnPosition;

            // Get character type from GameSession
            CharacterType charType = GameSession.SelectedCharacter;

            // Create player using prefab or factory method
            if (playerPrefab != null)
            {
                _currentPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                _playerSetup = _currentPlayer.GetComponent<PlayerSetup>();
                if (_playerSetup != null)
                {
                    _playerSetup.Initialize(charType);
                }
            }
            else
            {
                _currentPlayer = PlayerSetup.CreatePlayer(charType, spawnPos);
                _playerSetup = _currentPlayer.GetComponent<PlayerSetup>();
            }

            // Cache references
            _playerController = _currentPlayer.GetComponent<PlayerController>();
            _playerHealth = _currentPlayer.GetComponent<PlayerHealth>();

            // Subscribe to player events
            if (_playerHealth != null)
            {
                _playerHealth.OnDeath += OnPlayerDeath;
                _playerHealth.OnGameOver += OnPlayerGameOver;
            }

            Debug.Log($"[PlayerSpawner] Spawned player at {spawnPos} as {charType}");
        }

        /// <summary>
        /// Repositions the player to a specific location.
        /// </summary>
        public void RepositionPlayer(Vector3 position)
        {
            if (_playerController != null)
            {
                _playerController.SetPosition(position);
            }
            else if (_currentPlayer != null)
            {
                _currentPlayer.transform.position = position;
            }
        }

        /// <summary>
        /// Spawns the player at a named spawn point in the current room.
        /// </summary>
        public void SpawnAtPoint(string spawnPointId)
        {
            if (RoomManager.Instance != null)
            {
                Vector2 spawnPos = RoomManager.Instance.GetSpawnPosition(spawnPointId);
                RepositionPlayer(spawnPos);
            }
        }

        private void OnPlayerDeath()
        {
            Debug.Log("[PlayerSpawner] Player died");
            // Death handling is managed by PlayerHealth
        }

        private void OnPlayerGameOver()
        {
            Debug.Log("[PlayerSpawner] Game Over");
            // Notify GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }

        /// <summary>
        /// Sets player control enabled/disabled.
        /// </summary>
        public void SetPlayerControlEnabled(bool enabled)
        {
            if (_playerController != null)
            {
                _playerController.CanMove = enabled;
            }
        }

        /// <summary>
        /// Gets the player's current position.
        /// </summary>
        public Vector3 GetPlayerPosition()
        {
            return _currentPlayer != null ? _currentPlayer.transform.position : defaultSpawnPosition;
        }

        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnDeath -= OnPlayerDeath;
                _playerHealth.OnGameOver -= OnPlayerGameOver;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
