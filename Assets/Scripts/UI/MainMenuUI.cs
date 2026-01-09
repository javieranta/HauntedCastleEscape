using UnityEngine;
using UnityEngine.UI;
using HauntedCastle.Services;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Main menu screen with game title and navigation options.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Text titleText;
        [SerializeField] private Text subtitleText;
        [SerializeField] private Text[] menuOptions;
        [SerializeField] private Text instructionsText;

        [Header("Selection")]
        [SerializeField] private int selectedIndex = 0;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color selectedColor = Color.yellow;

        private string[] _menuLabels = { "START GAME", "CHARACTER SELECT", "QUIT" };

        private void Awake()
        {
            SetupCanvas();
        }

        private void Start()
        {
            UpdateMenuDisplay();
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
            canvas.sortingOrder = 100;

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
            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform, false);
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.05f, 0.02f, 0.1f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Title
            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(transform, false);
            titleText = titleObj.AddComponent<Text>();
            titleText.text = "HAUNTED CASTLE";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 20;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = new Color(0.8f, 0.2f, 0.2f);
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.7f);
            titleRect.anchorMax = new Vector2(1, 0.9f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            // Subtitle
            var subObj = new GameObject("Subtitle");
            subObj.transform.SetParent(transform, false);
            subtitleText = subObj.AddComponent<Text>();
            subtitleText.text = "ESCAPE";
            subtitleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            subtitleText.fontSize = 14;
            subtitleText.alignment = TextAnchor.MiddleCenter;
            subtitleText.color = new Color(0.9f, 0.7f, 0.2f);
            var subRect = subObj.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0, 0.6f);
            subRect.anchorMax = new Vector2(1, 0.7f);
            subRect.offsetMin = Vector2.zero;
            subRect.offsetMax = Vector2.zero;

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

            // Credits
            var creditsObj = new GameObject("Credits");
            creditsObj.transform.SetParent(transform, false);
            var creditsText = creditsObj.AddComponent<Text>();
            creditsText.text = "Inspired by Atic Atac (1983)";
            creditsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            creditsText.fontSize = 5;
            creditsText.alignment = TextAnchor.MiddleCenter;
            creditsText.color = new Color(0.4f, 0.4f, 0.4f);
            var creditsRect = creditsObj.GetComponent<RectTransform>();
            creditsRect.anchorMin = new Vector2(0, 0.05f);
            creditsRect.anchorMax = new Vector2(1, 0.1f);
            creditsRect.offsetMin = Vector2.zero;
            creditsRect.offsetMax = Vector2.zero;
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
                case 0: // Start Game
                    StartGame();
                    break;
                case 1: // Character Select
                    GoToCharacterSelect();
                    break;
                case 2: // Quit
                    QuitGame();
                    break;
            }
        }

        private void StartGame()
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

        private void GoToCharacterSelect()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToCharacterSelect();
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelect");
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
    }
}
