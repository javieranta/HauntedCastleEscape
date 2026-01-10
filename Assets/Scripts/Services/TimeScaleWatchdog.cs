using UnityEngine;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Watchdog that prevents Time.timeScale from getting stuck at 0.
    /// Runs independently using unscaledDeltaTime.
    /// </summary>
    public class TimeScaleWatchdog : MonoBehaviour
    {
        public static TimeScaleWatchdog Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float maxZeroTimeScaleDuration = 0.5f; // Reduced from 2f for faster recovery
        [SerializeField] private bool enableWatchdog = true;

        private float _zeroTimeScaleTime;
        private bool _wasZero;
        private int _framesSinceLastCheck;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!enableWatchdog) return;

            _framesSinceLastCheck++;

            // Check if timeScale is at or near zero
            if (Time.timeScale < 0.01f)
            {
                if (!_wasZero)
                {
                    _zeroTimeScaleTime = Time.unscaledTime;
                    _wasZero = true;
                    Debug.Log($"[TimeScaleWatchdog] TimeScale dropped to {Time.timeScale}");
                }

                // If it's been zero for too long, force reset
                float duration = Time.unscaledTime - _zeroTimeScaleTime;
                if (duration > maxZeroTimeScaleDuration)
                {
                    Debug.LogWarning($"[TimeScaleWatchdog] TimeScale was stuck at {Time.timeScale} for {duration:F1}s. Force resetting to 1.0!");
                    ForceResetTimeScale();
                }
            }
            else
            {
                _wasZero = false;
            }

            // Additional check: if no physics has happened for too long, something is wrong
            // This catches freezes that aren't timeScale related
            if (_framesSinceLastCheck > 60) // Check every ~60 frames
            {
                _framesSinceLastCheck = 0;
                ValidatePhysicsHealth();
            }
        }

        /// <summary>
        /// Validates physics system health by checking for NaN in common areas.
        /// </summary>
        private void ValidatePhysicsHealth()
        {
            // Find all enemies and check for NaN velocities
            var enemies = FindObjectsByType<Enemies.Enemy>(FindObjectsSortMode.None);
            foreach (var enemy in enemies)
            {
                if (enemy == null) continue;

                var rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    if (float.IsNaN(rb.velocity.x) || float.IsNaN(rb.velocity.y) ||
                        float.IsInfinity(rb.velocity.x) || float.IsInfinity(rb.velocity.y))
                    {
                        Debug.LogWarning($"[TimeScaleWatchdog] Found NaN/Infinity velocity in {enemy.name}, resetting");
                        rb.velocity = Vector2.zero;
                    }
                }
            }

            // Also check player
            var player = Player.PlayerController.Instance;
            if (player != null)
            {
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null && (float.IsNaN(rb.velocity.x) || float.IsNaN(rb.velocity.y) ||
                    float.IsInfinity(rb.velocity.x) || float.IsInfinity(rb.velocity.y)))
                {
                    Debug.LogWarning("[TimeScaleWatchdog] Found NaN/Infinity velocity in player, resetting");
                    rb.velocity = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// Forces Time.timeScale back to 1.0 and resets related state.
        /// </summary>
        public void ForceResetTimeScale()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f; // Default physics timestep

            // Also try to reset any game state that might have caused the freeze
            if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Paused)
            {
                Debug.Log("[TimeScaleWatchdog] Unpausing game state");
                GameManager.Instance.ChangeState(GameState.Playing);
            }

            Debug.Log("[TimeScaleWatchdog] TimeScale force reset complete");
        }

        /// <summary>
        /// Creates the TimeScaleWatchdog if it doesn't exist.
        /// </summary>
        public static TimeScaleWatchdog EnsureExists()
        {
            if (Instance == null)
            {
                var obj = new GameObject("TimeScaleWatchdog");
                Instance = obj.AddComponent<TimeScaleWatchdog>();
            }
            return Instance;
        }
    }
}
