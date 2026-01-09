using UnityEngine;
using UnityEngine.UI;
using HauntedCastle.Services;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Game Over screen displayed when player loses all lives.
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Text gameOverText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text[] menuOptions;
        [SerializeField] private Text instructionsText;

        [Header("Selection")]
        [SerializeField] private int selectedIndex = 0;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        private string[] _menuLabels = { "TRY AGAIN", "MAIN MENU", "QUIT" };

        private void Awake()
        {
            SetupCanvas();
        }

        private void Start()
        {
            UpdateMenuDisplay();
            Time.timeScale = 1f; // Ensure time is running
        }

        private void SetupCanvas()
        {
            if (canvas == null)
            {
                canvas = GetComponent<Canvas>();
                if (canvas == null)
                {
                    canvas = gameObject.AddComponent<Canvas>();
                }
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            if (GetComponent<CanvasScaler>() == null)
            {
                var scaler = gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(256, 192);
            }

            if (GetComponent<GraphicRaycaster>() == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }

            CreateUIElements();
        }

        private void CreateUIElements()
        {
            // Dark overlay background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform, false);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0f, 0f, 0.95f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Game Over Text
            var goObj = new GameObject("GameOverText");
            goObj.transform.SetParent(transform, false);
            gameOverText = goObj.AddComponent<Text>();
            gameOverText.text = "GAME OVER";
            gameOverText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            gameOverText.fontSize = 24;
            gameOverText.alignment = TextAnchor.MiddleCenter;
            gameOverText.color = Color.red;
            var goRect = goObj.GetComponent<RectTransform>();
            goRect.anchorMin = new Vector2(0, 0.7f);
            goRect.anchorMax = new Vector2(1, 0.9f);
            goRect.offsetMin = Vector2.zero;
            goRect.offsetMax = Vector2.zero;

            // Score Text
            var scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(transform, false);
            scoreText = scoreObj.AddComponent<Text>();
            scoreText.text = "FINAL SCORE: 0";
            scoreText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            scoreText.fontSize = 10;
            scoreText.alignment = TextAnchor.MiddleCenter;
            scoreText.color = new Color(0.9f, 0.9f, 0.5f);
            var scoreRect = scoreObj.GetComponent<RectTransform>();
            scoreRect.anchorMin = new Vector2(0, 0.55f);
            scoreRect.anchorMax = new Vector2(1, 0.65f);
            scoreRect.offsetMin = Vector2.zero;
            scoreRect.offsetMax = Vector2.zero;

            // Menu Options
            menuOptions = new Text[_menuLabels.Length];
            float startY = 0.45f;
            float spacing = 0.1f;

            for (int i = 0; i < _menuLabels.Length; i++)
            {
                var optObj = new GameObject($"Option_{i}");
                optObj.transform.SetParent(transform, false);
                menuOptions[i] = optObj.AddComponent<Text>();
                menuOptions[i].text = _menuLabels[i];
                menuOptions[i].font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                menuOptions[i].fontSize = 10;
                menuOptions[i].alignment = TextAnchor.MiddleCenter;
                menuOptions[i].color = normalColor;

                var optRect = optObj.GetComponent<RectTransform>();
                float yMin = startY - (i * spacing) - spacing;
                float yMax = startY - (i * spacing);
                optRect.anchorMin = new Vector2(0.2f, yMin);
                optRect.anchorMax = new Vector2(0.8f, yMax);
                optRect.offsetMin = Vector2.zero;
                optRect.offsetMax = Vector2.zero;
            }

            // Instructions
            var instrObj = new GameObject("Instructions");
            instrObj.transform.SetParent(transform, false);
            instructionsText = instrObj.AddComponent<Text>();
            instructionsText.text = "W/S: SELECT    ENTER: CONFIRM";
            instructionsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            instructionsText.fontSize = 6;
            instructionsText.alignment = TextAnchor.MiddleCenter;
            instructionsText.color = new Color(0.5f, 0.5f, 0.5f);
            var instrRect = instrObj.GetComponent<RectTransform>();
            instrRect.anchorMin = new Vector2(0, 0);
            instrRect.anchorMax = new Vector2(1, 0.1f);
            instrRect.offsetMin = Vector2.zero;
            instrRect.offsetMax = Vector2.zero;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // Navigate up/down
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = _menuLabels.Length - 1;
                UpdateMenuDisplay();
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedIndex++;
                if (selectedIndex >= _menuLabels.Length) selectedIndex = 0;
                UpdateMenuDisplay();
            }

            // Confirm selection
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                SelectOption();
            }
        }

        private void UpdateMenuDisplay()
        {
            for (int i = 0; i < menuOptions.Length; i++)
            {
                if (menuOptions[i] != null)
                {
                    menuOptions[i].color = (i == selectedIndex) ? selectedColor : normalColor;
                    menuOptions[i].text = (i == selectedIndex) ? $"> {_menuLabels[i]} <" : _menuLabels[i];
                }
            }
        }

        private void SelectOption()
        {
            switch (selectedIndex)
            {
                case 0: // Try Again
                    TryAgain();
                    break;
                case 1: // Main Menu
                    GoToMainMenu();
                    break;
                case 2: // Quit
                    QuitGame();
                    break;
            }
        }

        private void TryAgain()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            }
        }

        private void GoToMainMenu()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToMainMenu();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            }
        }

        private void QuitGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
            else
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        /// <summary>
        /// Sets the displayed score.
        /// </summary>
        public void SetScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"FINAL SCORE: {score}";
            }
        }
    }
}
