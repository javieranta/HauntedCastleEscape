using UnityEngine;
using UnityEngine.SceneManagement;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Handles initial game boot sequence.
    /// Initializes core systems and transitions to main menu.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        [Header("Boot Settings")]
        [SerializeField] private float bootDelay = 1.0f;
        [SerializeField] private bool skipToGame = false; // Debug option

        private float _bootTimer;
        private bool _initialized;

        private void Start()
        {
            InitializeCoreSystems();
        }

        private void Update()
        {
            if (!_initialized) return;

            _bootTimer += Time.deltaTime;

            if (_bootTimer >= bootDelay)
            {
                TransitionToNextScene();
            }
        }

        private void InitializeCoreSystems()
        {
            // Ensure GameManager exists
            if (GameManager.Instance == null)
            {
                var gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent<GameManager>();
            }

            // Initialize audio manager (will be added later)
            // InitializeAudioManager();

            // Initialize input system (will be added later)
            // InitializeInputSystem();

            Debug.Log("[BootLoader] Core systems initialized");
            _initialized = true;
        }

        private void TransitionToNextScene()
        {
            if (skipToGame)
            {
                GameManager.Instance.StartNewGame();
            }
            else
            {
                GameManager.Instance.GoToMainMenu();
            }
        }
    }
}
