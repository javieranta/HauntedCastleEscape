using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HauntedCastle.Services;
using HauntedCastle.Audio;
using HauntedCastle.Core;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Game Over screen showing final stats and options to retry or quit.
    /// </summary>
    public class GameOverScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI promptText;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float textDelayBetweenLines = 0.3f;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private bool _canInput;
        private int _selectedOption;

        private void Start()
        {
            CreateUI();
            StartCoroutine(ShowGameOver());
        }

        private void Update()
        {
            if (!_canInput) return;

            // Navigation
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _selectedOption = (_selectedOption - 1 + 2) % 2;
                UpdatePromptHighlight();
                AudioManager.Instance?.PlaySFX(SoundEffect.MenuSelect);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                _selectedOption = (_selectedOption + 1) % 2;
                UpdatePromptHighlight();
                AudioManager.Instance?.PlaySFX(SoundEffect.MenuSelect);
            }

            // Selection
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                AudioManager.Instance?.PlaySFX(SoundEffect.MenuConfirm);

                if (_selectedOption == 0)
                {
                    // Retry
                    GameManager.Instance?.StartNewGame();
                }
                else
                {
                    // Main Menu
                    GameManager.Instance?.GoToMainMenu();
                }
            }
        }

        private void CreateUI()
        {
            // Create canvas
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 200;

            gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();

            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;

            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform, false);

            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.1f, 0f, 0f, 0.95f); // Dark red tint

            // Title
            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(transform, false);

            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.75f);
            titleRect.anchorMax = new Vector2(0.5f, 0.85f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(400, 60);

            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "GAME OVER";
            titleText.fontSize = 64;
            titleText.color = Color.red;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;

            // Stats
            var statsObj = new GameObject("Stats");
            statsObj.transform.SetParent(transform, false);

            var statsRect = statsObj.AddComponent<RectTransform>();
            statsRect.anchorMin = new Vector2(0.2f, 0.35f);
            statsRect.anchorMax = new Vector2(0.8f, 0.65f);
            statsRect.offsetMin = Vector2.zero;
            statsRect.offsetMax = Vector2.zero;

            statsText = statsObj.AddComponent<TextMeshProUGUI>();
            statsText.text = "";
            statsText.fontSize = 24;
            statsText.color = Color.white;
            statsText.alignment = TextAlignmentOptions.Center;

            // Prompt
            var promptObj = new GameObject("Prompt");
            promptObj.transform.SetParent(transform, false);

            var promptRect = promptObj.AddComponent<RectTransform>();
            promptRect.anchorMin = new Vector2(0.3f, 0.15f);
            promptRect.anchorMax = new Vector2(0.7f, 0.3f);
            promptRect.offsetMin = Vector2.zero;
            promptRect.offsetMax = Vector2.zero;

            promptText = promptObj.AddComponent<TextMeshProUGUI>();
            promptText.text = "";
            promptText.fontSize = 28;
            promptText.color = Color.white;
            promptText.alignment = TextAlignmentOptions.Center;
        }

        private System.Collections.IEnumerator ShowGameOver()
        {
            // Play death sound
            AudioManager.Instance?.PlaySFX(SoundEffect.PlayerDeath);

            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = elapsed / fadeInDuration;
                yield return null;
            }
            _canvasGroup.alpha = 1f;

            yield return new WaitForSecondsRealtime(0.5f);

            // Show stats one by one
            int score = ScoreManager.Instance?.CurrentScore ?? 0;
            int enemies = ScoreManager.Instance?.EnemiesDefeated ?? 0;
            int rooms = ScoreManager.Instance?.RoomsDiscovered ?? 0;
            int secrets = ScoreManager.Instance?.SecretsFound ?? 0;

            statsText.text = $"Final Score: {score:N0}";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines);

            statsText.text += $"\n\nEnemies Defeated: {enemies}";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines);

            statsText.text += $"\nRooms Explored: {rooms}";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines);

            statsText.text += $"\nSecrets Found: {secrets}";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines * 2);

            // Show options
            UpdatePromptHighlight();
            _canInput = true;
        }

        private void UpdatePromptHighlight()
        {
            string retry = _selectedOption == 0 ? "<color=yellow>> TRY AGAIN <</color>" : "TRY AGAIN";
            string menu = _selectedOption == 1 ? "<color=yellow>> MAIN MENU <</color>" : "MAIN MENU";
            promptText.text = $"{retry}\n{menu}";
        }

        /// <summary>
        /// Creates the game over screen.
        /// </summary>
        public static GameOverScreen Create()
        {
            var obj = new GameObject("GameOverScreen");
            return obj.AddComponent<GameOverScreen>();
        }
    }
}
