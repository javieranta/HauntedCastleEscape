using UnityEngine;
using UnityEngine.UI;
using HauntedCastle.Services;
using HauntedCastle.Data;
using HauntedCastle.Core.GameState;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Character selection screen UI.
    /// Allows player to choose between Wizard, Knight, or Serf.
    /// </summary>
    public class CharacterSelectUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Canvas canvas;
        [SerializeField] private Text titleText;
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text characterDescText;
        [SerializeField] private Text characterStatsText;
        [SerializeField] private Image characterPreview;
        [SerializeField] private Text instructionsText;

        [Header("Selection")]
        [SerializeField] private int selectedIndex = 1; // Start with Knight

        private CharacterData[] _characters;
        private Color[] _characterColors;

        private void Awake()
        {
            SetupCanvas();
            LoadCharacters();
        }

        private void Start()
        {
            UpdateDisplay();
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
            bgImage.color = new Color(0.1f, 0.05f, 0.15f);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;

            // Title
            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(transform, false);
            titleText = titleObj.AddComponent<Text>();
            titleText.text = "SELECT YOUR HERO";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 16;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = Color.yellow;
            var titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 0.95f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            // Character Preview Area
            var previewObj = new GameObject("CharacterPreview");
            previewObj.transform.SetParent(transform, false);
            characterPreview = previewObj.AddComponent<Image>();
            characterPreview.color = Color.white;
            var previewRect = previewObj.GetComponent<RectTransform>();
            previewRect.anchorMin = new Vector2(0.35f, 0.45f);
            previewRect.anchorMax = new Vector2(0.65f, 0.8f);
            previewRect.offsetMin = Vector2.zero;
            previewRect.offsetMax = Vector2.zero;

            // Character Name
            var nameObj = new GameObject("CharacterName");
            nameObj.transform.SetParent(transform, false);
            characterNameText = nameObj.AddComponent<Text>();
            characterNameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            characterNameText.fontSize = 12;
            characterNameText.alignment = TextAnchor.MiddleCenter;
            characterNameText.color = Color.white;
            var nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.35f);
            nameRect.anchorMax = new Vector2(1, 0.45f);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;

            // Character Description
            var descObj = new GameObject("CharacterDesc");
            descObj.transform.SetParent(transform, false);
            characterDescText = descObj.AddComponent<Text>();
            characterDescText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            characterDescText.fontSize = 8;
            characterDescText.alignment = TextAnchor.MiddleCenter;
            characterDescText.color = new Color(0.8f, 0.8f, 0.8f);
            var descRect = descObj.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0.1f, 0.2f);
            descRect.anchorMax = new Vector2(0.9f, 0.35f);
            descRect.offsetMin = Vector2.zero;
            descRect.offsetMax = Vector2.zero;

            // Character Stats
            var statsObj = new GameObject("CharacterStats");
            statsObj.transform.SetParent(transform, false);
            characterStatsText = statsObj.AddComponent<Text>();
            characterStatsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            characterStatsText.fontSize = 7;
            characterStatsText.alignment = TextAnchor.MiddleCenter;
            characterStatsText.color = new Color(0.7f, 0.9f, 0.7f);
            var statsRect = statsObj.GetComponent<RectTransform>();
            statsRect.anchorMin = new Vector2(0.1f, 0.1f);
            statsRect.anchorMax = new Vector2(0.9f, 0.2f);
            statsRect.offsetMin = Vector2.zero;
            statsRect.offsetMax = Vector2.zero;

            // Instructions
            var instrObj = new GameObject("Instructions");
            instrObj.transform.SetParent(transform, false);
            instructionsText = instrObj.AddComponent<Text>();
            instructionsText.text = "<< A/D >>  SELECT    ENTER: START    ESC: BACK";
            instructionsText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            instructionsText.fontSize = 6;
            instructionsText.alignment = TextAnchor.MiddleCenter;
            instructionsText.color = new Color(0.6f, 0.6f, 0.6f);
            var instrRect = instrObj.GetComponent<RectTransform>();
            instrRect.anchorMin = new Vector2(0, 0);
            instrRect.anchorMax = new Vector2(1, 0.1f);
            instrRect.offsetMin = Vector2.zero;
            instrRect.offsetMax = Vector2.zero;
        }

        private void LoadCharacters()
        {
            _characters = CharacterDatabase.GetAllCharacters();
            _characterColors = new Color[_characters.Length];

            for (int i = 0; i < _characters.Length; i++)
            {
                _characterColors[i] = _characters[i].characterColor;
            }
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            // Navigate left/right
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                selectedIndex--;
                if (selectedIndex < 0) selectedIndex = _characters.Length - 1;
                UpdateDisplay();
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                selectedIndex++;
                if (selectedIndex >= _characters.Length) selectedIndex = 0;
                UpdateDisplay();
            }

            // Confirm selection
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                ConfirmSelection();
            }

            // Back to menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoBack();
            }
        }

        private void UpdateDisplay()
        {
            if (_characters == null || _characters.Length == 0) return;

            var character = _characters[selectedIndex];

            characterNameText.text = character.displayName ?? character.characterName;
            characterDescText.text = character.description;

            // Format stats
            string attackTypeStr = character.attackType switch
            {
                AttackType.Projectile => "Magic (Ranged)",
                AttackType.Melee => "Sword (Melee)",
                AttackType.Thrown => "Thrown (Medium)",
                _ => "Unknown"
            };

            string passageStr = character.accessiblePassageType switch
            {
                SecretPassageType.Bookcase => "Bookcases",
                SecretPassageType.Clock => "Grandfather Clocks",
                SecretPassageType.Barrel => "Barrels",
                _ => "None"
            };

            characterStatsText.text = $"Speed: {character.moveSpeed:F1}  |  Attack: {attackTypeStr}  |  Passages: {passageStr}";

            // Update preview color
            if (characterPreview != null)
            {
                characterPreview.color = character.characterColor;
            }
        }

        private void ConfirmSelection()
        {
            if (_characters == null || _characters.Length == 0) return;

            var character = _characters[selectedIndex];

            // Store selection
            GameSession.SelectedCharacter = character.characterType;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.SelectedCharacter = character.characterType;
            }

            Debug.Log($"[CharacterSelectUI] Selected: {character.characterName}");

            // Start the game
            StartGame();
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

        private void GoBack()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GoToMainMenu();
            }
        }
    }
}
