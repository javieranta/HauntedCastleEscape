using System;
using UnityEngine;
using HauntedCastle.Core.GameState;
using HauntedCastle.Services;

namespace HauntedCastle.Player
{
    /// <summary>
    /// Manages player health, energy, lives, and damage handling.
    /// Energy drains over time and with movement; reaching zero costs a life.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float maxEnergy = 100f;
        [SerializeField] private int startingLives = 3;

        [Header("Energy Drain")]
        [SerializeField] private float passiveDrainRate = 0.5f;      // Energy per second (idle) - 200 seconds to drain
        [SerializeField] private float movementDrainRate = 0.5f;     // Additional drain while moving
        [SerializeField] private bool drainWhileMoving = true;

        [Header("Damage")]
        [SerializeField] private float invulnerabilityDuration = 1.5f;
        [SerializeField] private float knockbackForce = 5f;
        [SerializeField] private int flashCount = 5;

        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color damageFlashColor = Color.red;

        // State
        private float _currentEnergy;
        private int _currentLives;
        private bool _isInvulnerable;
        private float _invulnerabilityTimer;
        private bool _isDead;

        // Events
        public event Action<float, float> OnEnergyChanged;          // current, max
        public event Action<int> OnLivesChanged;                     // current lives
        public event Action<float> OnDamageTaken;                    // damage amount
        public event Action OnDeath;
        public event Action OnGameOver;

        // Properties
        public float CurrentEnergy => _currentEnergy;
        public float MaxEnergy => maxEnergy;
        public float EnergyPercent => _currentEnergy / maxEnergy;
        public int CurrentLives => _currentLives;
        public bool IsInvulnerable => _isInvulnerable;
        public bool IsDead => _isDead;

        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void Start()
        {
            InitializeFromSession();
        }

        private void Update()
        {
            if (_isDead) return;

            // Passive energy drain
            DrainEnergy(passiveDrainRate * Time.deltaTime);

            // Update invulnerability
            if (_isInvulnerable)
            {
                _invulnerabilityTimer -= Time.deltaTime;
                if (_invulnerabilityTimer <= 0)
                {
                    EndInvulnerability();
                }
            }
        }

        private void InitializeFromSession()
        {
            // Try to get state from GameSession if available
            // For now, use defaults
            _currentEnergy = maxEnergy;
            _currentLives = startingLives;
            _isDead = false;
            _isInvulnerable = false;

            // Ensure player can move on initialization
            if (_playerController != null)
            {
                _playerController.CanMove = true;
            }

            OnEnergyChanged?.Invoke(_currentEnergy, maxEnergy);
            OnLivesChanged?.Invoke(_currentLives);

            Debug.Log($"[PlayerHealth] Initialized: Energy={_currentEnergy}, Lives={_currentLives}, CanMove=true");
        }

        /// <summary>
        /// Drains energy by the specified amount.
        /// </summary>
        public void DrainEnergy(float amount)
        {
            if (_isDead || amount <= 0) return;

            _currentEnergy = Mathf.Max(0, _currentEnergy - amount);
            OnEnergyChanged?.Invoke(_currentEnergy, maxEnergy);

            if (_currentEnergy <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Called by PlayerController when moving.
        /// </summary>
        public void DrainEnergyFromMovement(float deltaTime)
        {
            if (drainWhileMoving)
            {
                DrainEnergy(movementDrainRate * deltaTime);
            }
        }

        /// <summary>
        /// Restores energy by the specified amount.
        /// </summary>
        public void RestoreEnergy(float amount)
        {
            if (_isDead || amount <= 0) return;

            _currentEnergy = Mathf.Min(maxEnergy, _currentEnergy + amount);
            OnEnergyChanged?.Invoke(_currentEnergy, maxEnergy);

            Debug.Log($"[PlayerHealth] Restored {amount} energy. Current: {_currentEnergy}/{maxEnergy}");
        }

        /// <summary>
        /// Takes damage from an enemy or hazard.
        /// </summary>
        public void TakeDamage(float damage, Vector2 knockbackDirection = default)
        {
            if (_isDead || _isInvulnerable || damage <= 0) return;

            // Apply damage to energy
            DrainEnergy(damage);

            OnDamageTaken?.Invoke(damage);

            // Start invulnerability
            StartInvulnerability();

            // Apply knockback
            if (knockbackDirection != default && _playerController != null)
            {
                _playerController.ApplyKnockback(knockbackDirection, knockbackForce);
            }

            // Visual feedback
            StartCoroutine(DamageFlashRoutine());

            Debug.Log($"[PlayerHealth] Took {damage} damage. Energy: {_currentEnergy}/{maxEnergy}");
        }

        /// <summary>
        /// Instantly kills the player (e.g., special enemy without counter item).
        /// </summary>
        public void InstantKill()
        {
            if (_isDead) return;

            _currentEnergy = 0;
            OnEnergyChanged?.Invoke(0, maxEnergy);
            Die();
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            _currentLives--;

            OnDeath?.Invoke();
            OnLivesChanged?.Invoke(_currentLives);

            Debug.Log($"[PlayerHealth] Player died! Lives remaining: {_currentLives}");

            // Place death marker
            PlaceDeathMarker();

            if (_currentLives <= 0)
            {
                // Game over
                OnGameOver?.Invoke();
                HandleGameOver();
            }
            else
            {
                // Respawn
                StartCoroutine(RespawnRoutine());
            }
        }

        private void PlaceDeathMarker()
        {
            // Create death marker at current position
            if (RoomManager.Instance != null && RoomManager.Instance.CurrentRoomData != null)
            {
                string roomId = RoomManager.Instance.CurrentRoomData.roomId;
                Vector2 pos = transform.position;

                // Create visual marker
                var markerObj = new GameObject("DeathMarker");
                markerObj.transform.position = pos;

                var sr = markerObj.AddComponent<SpriteRenderer>();
                sr.color = new Color(1f, 0f, 0f, 0.5f);
                sr.sortingLayerName = "Items";
                sr.sortingOrder = -1;

                // Create simple X shape using line renderers or just a colored square
                // For now, use the sprite renderer with a tint

                // The marker persists for the run
                DontDestroyOnLoad(markerObj);

                Debug.Log($"[PlayerHealth] Death marker placed in {roomId} at {pos}");
            }
        }

        private System.Collections.IEnumerator RespawnRoutine()
        {
            // Disable player control
            if (_playerController != null)
            {
                _playerController.CanMove = false;
            }

            // Wait for death animation/effect
            yield return new WaitForSeconds(1f);

            // Restore energy
            _currentEnergy = maxEnergy;
            _isDead = false;

            OnEnergyChanged?.Invoke(_currentEnergy, maxEnergy);

            // Respawn at room start or last checkpoint
            if (_playerController != null)
            {
                _playerController.SetPosition(Vector2.zero); // Room center
                _playerController.CanMove = true;
            }

            // Brief invulnerability after respawn
            StartInvulnerability();

            Debug.Log("[PlayerHealth] Player respawned");
        }

        private void HandleGameOver()
        {
            Debug.Log("[PlayerHealth] Game Over!");

            // Disable player
            if (_playerController != null)
            {
                _playerController.CanMove = false;
            }

            // Transition to game over screen
            StartCoroutine(GameOverRoutine());
        }

        private System.Collections.IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(2f);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }

        private void StartInvulnerability()
        {
            _isInvulnerable = true;
            _invulnerabilityTimer = invulnerabilityDuration;
        }

        private void EndInvulnerability()
        {
            _isInvulnerable = false;
            _invulnerabilityTimer = 0;

            // Ensure sprite is visible
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.white;
            }
        }

        private System.Collections.IEnumerator DamageFlashRoutine()
        {
            if (spriteRenderer == null) yield break;

            float flashDuration = invulnerabilityDuration / (flashCount * 2);

            for (int i = 0; i < flashCount && _isInvulnerable; i++)
            {
                spriteRenderer.color = damageFlashColor;
                yield return new WaitForSeconds(flashDuration);

                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(flashDuration);
            }

            spriteRenderer.color = Color.white;
        }

        /// <summary>
        /// Adds extra lives.
        /// </summary>
        public void AddLife(int count = 1)
        {
            _currentLives += count;
            OnLivesChanged?.Invoke(_currentLives);
        }

        /// <summary>
        /// Fully restores energy to maximum.
        /// </summary>
        public void FullRestore()
        {
            _currentEnergy = maxEnergy;
            OnEnergyChanged?.Invoke(_currentEnergy, maxEnergy);
        }

        /// <summary>
        /// Sets drain rates (for difficulty adjustment).
        /// </summary>
        public void SetDrainRates(float passive, float movement)
        {
            passiveDrainRate = passive;
            movementDrainRate = movement;
        }
    }
}
