using UnityEngine;
using UnityEngine.SceneManagement;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Core game manager singleton that persists across scenes.
    /// Handles game state, scene transitions, and global settings.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private int targetFrameRate = 60;
        [SerializeField] private bool vSyncEnabled = true;

        [Header("Internal Resolution")]
        [SerializeField] private int internalWidth = 256;
        [SerializeField] private int internalHeight = 192;

        // Game state
        public GameState CurrentState { get; private set; } = GameState.Boot;
        public CharacterType SelectedCharacter { get; set; } = CharacterType.Knight;

        // Scene names
        public const string SCENE_BOOT = "Boot";
        public const string SCENE_MAIN_MENU = "MainMenu";
        public const string SCENE_CHARACTER_SELECT = "CharacterSelect";
        public const string SCENE_GAME = "Game";
        public const string SCENE_GAME_OVER = "GameOver";
        public const string SCENE_VICTORY = "Victory";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSettings();
        }

        private void InitializeSettings()
        {
            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = vSyncEnabled ? 1 : 0;
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;

            // Ensure time scale is correct for each state
            if (newState == GameState.Playing)
            {
                Time.timeScale = 1f;
            }
            else if (newState == GameState.Paused)
            {
                Time.timeScale = 0f;
            }

            Debug.Log($"[GameManager] State changed to: {newState}, TimeScale={Time.timeScale}");
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void StartNewGame()
        {
            // Reset time scale in case it was paused
            Time.timeScale = 1f;

            // Ensure all managers exist
            Effects.VisualEffectsManager.EnsureExists();
            Effects.AtmosphereManager.EnsureExists();
            Effects.CombatFeedbackManager.EnsureExists();
            Core.ScoreManager.EnsureExists();

            ChangeState(GameState.Playing);
            LoadScene(SCENE_GAME);
        }

        public void GoToMainMenu()
        {
            // Reset time scale in case game was paused
            Time.timeScale = 1f;
            ChangeState(GameState.MainMenu);
            LoadScene(SCENE_MAIN_MENU);
        }

        public void GoToCharacterSelect()
        {
            ChangeState(GameState.CharacterSelect);
            LoadScene(SCENE_CHARACTER_SELECT);
        }

        public void TriggerGameOver()
        {
            // Reset time scale
            Time.timeScale = 1f;
            ChangeState(GameState.GameOver);
            LoadScene(SCENE_GAME_OVER);
        }

        public void TriggerVictory()
        {
            ChangeState(GameState.Victory);
            LoadScene(SCENE_VICTORY);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public Vector2Int GetInternalResolution()
        {
            return new Vector2Int(internalWidth, internalHeight);
        }
    }

    public enum GameState
    {
        Boot,
        MainMenu,
        CharacterSelect,
        Playing,
        Paused,
        GameOver,
        Victory
    }

    public enum CharacterType
    {
        Wizard,
        Knight,
        Serf
    }
}
