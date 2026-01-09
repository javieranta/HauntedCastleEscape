using UnityEngine;
using UnityEngine.UI;
using HauntedCastle.Services;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Pause menu overlay that can be toggled during gameplay.
    /// </summary>
    public class PauseMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private Text pauseText;
        [SerializeField] private Text[] menuOptions;
        [SerializeField] private Text instructionsText;

        [Header("Selection")]
        [SerializeField] private int selectedIndex = 0;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        [Header("State")]
        [SerializeField] private bool isPaused = false;

        private string[] _menuLabels = { "RESUME", "MAIN MENU", "QUIT" };

        public bool IsPaused => isPaused;

        private void Awake()
        {
            SetupCanvas();
        }

        private void Start()
        {
            HideMenu();
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
            canvas.sortingOrder = 150;

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
            // Menu Panel (container for all pause menu elements)
            menuPanel = new GameObject("MenuPanel");
            menuPanel.transform.SetParent(transform, false);
            var panelRect = menuPanel.AddComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;

            // Dark overlay background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(menuPanel.transform, false);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.85f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Pause Text
            var pauseObj = new GameObject("PauseText");
            pauseObj.transform.SetParent(menuPanel.transform, false);
            pauseText = pauseObj.AddComponent<Text>();
            pauseText.text = "PAUSED";
            pauseText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            pauseText.fontSize = 20;
            pauseText.alignment = TextAnchor.MiddleCenter;
            pauseText.color = Color.white;
            var pauseRect = pauseObj.GetComponent<RectTransform>();
            pauseRect.anchorMin = new Vector2(0, 0.7f);
            pauseRect.anchorMax = new Vector2(1, 0.85f);
            pauseRect.offsetMin = Vector2.zero;
            pauseRect.offsetMax = Vector2.zero;

            // Menu Options
            menuOptions = new Text[_menuLabels.Length];
            float startY = 0.55f;
            float spacing = 0.12f;

            for (int i = 0; i < _menuLabels.Length; i++)
            {
                var optObj = new GameObject($"Option_{i}");
                optObj.transform.SetParent(menuPanel.transform, false);
                menuOptions[i] = optObj.AddComponent<Text>();
                menuOptions[i].text = _menuLabels[i];
                menuOptions[i].font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                menuOptions[i].fontSize = 12;
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
            instrObj.transform.SetParent(menuPanel.transform, false);
            instructionsText = instrObj.AddComponent<Text>();
            instructionsText.text = "W/S: SELECT    ENTER: CONFIRM    ESC: RESUME";
            instructionsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            instructionsText.fontSize = 6;
            instructionsText.alignment = TextAnchor.MiddleCenter;
            instructionsText.color = new Color(0.5f, 0.5f, 0.5f);
            var instrRect = instrObj.GetComponent<RectTransform>();
            instrRect.anchorMin = new Vector2(0, 0.05f);
            instrRect.anchorMax = new Vector2(1, 0.15f);
            instrRect.offsetMin = Vector2.zero;
            instrRect.offsetMax = Vector2.zero;
        }

        private void Update()
        {
            // Toggle pause with Escape
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }

            // Handle input only when paused
            if (isPaused)
            {
                HandleMenuInput();
            }
        }

        private void HandleMenuInput()
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
                case 0: // Resume
                    ResumeGame();
                    break;
                case 1: // Main Menu
                    GoToMainMenu();
                    break;
                case 2: // Quit
                    QuitGame();
                    break;
            }
        }

        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
            ShowMenu();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.Paused);
            }
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            HideMenu();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }
        }

        private void ShowMenu()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
            }
            selectedIndex = 0;
            UpdateMenuDisplay();
        }

        private void HideMenu()
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(false);
            }
        }

        private void GoToMainMenu()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToMainMenu();
            }
        }

        private void QuitGame()
        {
            Time.timeScale = 1f;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.QuitGame();
            }
        }
    }
}
