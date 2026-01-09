using UnityEngine;
using UnityEngine.UI;
using HauntedCastle.Services;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Victory screen displayed when player escapes the castle with the Great Key.
    /// </summary>
    public class VictoryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Text victoryText;
        [SerializeField] private Text congratsText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text timeText;
        [SerializeField] private Text[] menuOptions;
        [SerializeField] private Text instructionsText;

        [Header("Selection")]
        [SerializeField] private int selectedIndex = 0;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        private string[] _menuLabels = { "PLAY AGAIN", "MAIN MENU", "QUIT" };

        private void Awake()
        {
            SetupCanvas();
        }

        private void Start()
        {
            UpdateMenuDisplay();
            Time.timeScale = 1f;
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
            // Golden background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform, false);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0.08f, 0.02f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Victory Text
            var vicObj = new GameObject("VictoryText");
            vicObj.transform.SetParent(transform, false);
            victoryText = vicObj.AddComponent<Text>();
            victoryText.text = "VICTORY!";
            victoryText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            victoryText.fontSize = 24;
            victoryText.alignment = TextAnchor.MiddleCenter;
            victoryText.color = new Color(1f, 0.85f, 0.2f);
            var vicRect = vicObj.GetComponent<RectTransform>();
            vicRect.anchorMin = new Vector2(0, 0.8f);
            vicRect.anchorMax = new Vector2(1, 0.95f);
            vicRect.offsetMin = Vector2.zero;
            vicRect.offsetMax = Vector2.zero;

            // Congrats Text
            var congObj = new GameObject("CongratsText");
            congObj.transform.SetParent(transform, false);
            congratsText = congObj.AddComponent<Text>();
            congratsText.text = "You escaped the Haunted Castle!";
            congratsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            congratsText.fontSize = 10;
            congratsText.alignment = TextAnchor.MiddleCenter;
            congratsText.color = Color.white;
            var congRect = congObj.GetComponent<RectTransform>();
            congRect.anchorMin = new Vector2(0, 0.7f);
            congRect.anchorMax = new Vector2(1, 0.8f);
            congRect.offsetMin = Vector2.zero;
            congRect.offsetMax = Vector2.zero;

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
            scoreRect.anchorMin = new Vector2(0, 0.6f);
            scoreRect.anchorMax = new Vector2(1, 0.7f);
            scoreRect.offsetMin = Vector2.zero;
            scoreRect.offsetMax = Vector2.zero;

            // Time Text
            var timeObj = new GameObject("TimeText");
            timeObj.transform.SetParent(transform, false);
            timeText = timeObj.AddComponent<Text>();
            timeText.text = "TIME: 00:00";
            timeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            timeText.fontSize = 8;
            timeText.alignment = TextAnchor.MiddleCenter;
            timeText.color = new Color(0.7f, 0.9f, 0.7f);
            var timeRect = timeObj.GetComponent<RectTransform>();
            timeRect.anchorMin = new Vector2(0, 0.52f);
            timeRect.anchorMax = new Vector2(1, 0.6f);
            timeRect.offsetMin = Vector2.zero;
            timeRect.offsetMax = Vector2.zero;

            // Menu Options
            menuOptions = new Text[_menuLabels.Length];
            float startY = 0.42f;
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
            instrRect.anchorMax = new Vector2(1, 0.08f);
            instrRect.offsetMin = Vector2.zero;
            instrRect.offsetMax = Vector2.zero;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
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
                case 0: // Play Again
                    PlayAgain();
                    break;
                case 1: // Main Menu
                    GoToMainMenu();
                    break;
                case 2: // Quit
                    QuitGame();
                    break;
            }
        }

        private void PlayAgain()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
            }
        }

        private void GoToMainMenu()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToMainMenu();
            }
        }

        private void QuitGame()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }

        public void SetScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"FINAL SCORE: {score}";
            }
        }

        public void SetTime(float seconds)
        {
            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(seconds / 60);
                int secs = Mathf.FloorToInt(seconds % 60);
                timeText.text = $"TIME: {minutes:00}:{secs:00}";
            }
        }
    }
}
