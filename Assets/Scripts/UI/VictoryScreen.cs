using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HauntedCastle.Services;
using HauntedCastle.Audio;
using HauntedCastle.Core;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Victory screen showing congratulations, final stats, and celebration effects.
    /// </summary>
    public class VictoryScreen : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI promptText;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 1f;
        [SerializeField] private float textDelayBetweenLines = 0.4f;

        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private bool _canInput;
        private float _celebrationTimer;

        private void Start()
        {
            CreateUI();
            StartCoroutine(ShowVictory());
        }

        private void Update()
        {
            // Celebration particles
            _celebrationTimer += Time.unscaledDeltaTime;
            if (_celebrationTimer >= 0.5f)
            {
                _celebrationTimer = 0f;
                SpawnCelebrationParticles();
            }

            if (!_canInput) return;

            // Any key to continue
            if (Input.anyKeyDown)
            {
                AudioManager.Instance?.PlaySFX(SoundEffect.MenuConfirm);
                GameManager.Instance?.GoToMainMenu();
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

            // Background with gradient effect
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(transform, false);

            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0f, 0.1f, 0.05f, 0.95f); // Dark green/gold tint

            // Title
            var titleObj = new GameObject("Title");
            titleObj.transform.SetParent(transform, false);

            var titleRect = titleObj.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.78f);
            titleRect.anchorMax = new Vector2(0.5f, 0.9f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(500, 80);

            titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "VICTORY!";
            titleText.fontSize = 72;
            titleText.color = new Color(1f, 0.85f, 0.2f); // Gold
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.fontStyle = FontStyles.Bold;

            // Subtitle
            var subObj = new GameObject("Subtitle");
            subObj.transform.SetParent(transform, false);

            var subRect = subObj.AddComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0.5f, 0.68f);
            subRect.anchorMax = new Vector2(0.5f, 0.78f);
            subRect.anchoredPosition = Vector2.zero;
            subRect.sizeDelta = new Vector2(500, 50);

            subtitleText = subObj.AddComponent<TextMeshProUGUI>();
            subtitleText.text = "";
            subtitleText.fontSize = 28;
            subtitleText.color = Color.white;
            subtitleText.alignment = TextAlignmentOptions.Center;

            // Stats
            var statsObj = new GameObject("Stats");
            statsObj.transform.SetParent(transform, false);

            var statsRect = statsObj.AddComponent<RectTransform>();
            statsRect.anchorMin = new Vector2(0.2f, 0.25f);
            statsRect.anchorMax = new Vector2(0.8f, 0.62f);
            statsRect.offsetMin = Vector2.zero;
            statsRect.offsetMax = Vector2.zero;

            statsText = statsObj.AddComponent<TextMeshProUGUI>();
            statsText.text = "";
            statsText.fontSize = 26;
            statsText.color = Color.white;
            statsText.alignment = TextAlignmentOptions.Center;

            // Prompt
            var promptObj = new GameObject("Prompt");
            promptObj.transform.SetParent(transform, false);

            var promptRect = promptObj.AddComponent<RectTransform>();
            promptRect.anchorMin = new Vector2(0.3f, 0.08f);
            promptRect.anchorMax = new Vector2(0.7f, 0.18f);
            promptRect.offsetMin = Vector2.zero;
            promptRect.offsetMax = Vector2.zero;

            promptText = promptObj.AddComponent<TextMeshProUGUI>();
            promptText.text = "";
            promptText.fontSize = 22;
            promptText.color = new Color(0.8f, 0.8f, 0.8f);
            promptText.alignment = TextAlignmentOptions.Center;
        }

        private System.Collections.IEnumerator ShowVictory()
        {
            // Play victory fanfare
            AudioManager.Instance?.PlaySFX(SoundEffect.GreatKey);

            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _canvasGroup.alpha = elapsed / fadeInDuration;

                // Animate title scale
                float scale = 1f + Mathf.Sin(elapsed * 5f) * 0.1f;
                titleText.transform.localScale = Vector3.one * scale;

                yield return null;
            }
            _canvasGroup.alpha = 1f;

            yield return new WaitForSecondsRealtime(0.3f);

            // Show subtitle
            subtitleText.text = "You have escaped the Haunted Castle!";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines);

            // Show stats one by one
            int score = ScoreManager.Instance?.CurrentScore ?? 0;
            int enemies = ScoreManager.Instance?.EnemiesDefeated ?? 0;
            int rooms = ScoreManager.Instance?.RoomsDiscovered ?? 0;
            int secrets = ScoreManager.Instance?.SecretsFound ?? 0;

            statsText.text = $"<color=#FFD700>FINAL SCORE: {score:N0}</color>";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines);

            statsText.text += $"\n\nEnemies Vanquished: {enemies}";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines);

            statsText.text += $"\nRooms Explored: {rooms}";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines);

            statsText.text += $"\nSecrets Discovered: {secrets}";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines);

            // Calculate rank
            string rank = CalculateRank(score, enemies, rooms, secrets);
            statsText.text += $"\n\n<size=32><color=#FFD700>RANK: {rank}</color></size>";
            yield return new WaitForSecondsRealtime(textDelayBetweenLines * 2);

            // Show prompt
            promptText.text = "Press any key to continue...";
            StartCoroutine(BlinkPrompt());
            _canInput = true;
        }

        private string CalculateRank(int score, int enemies, int rooms, int secrets)
        {
            int totalPoints = score + enemies * 50 + rooms * 100 + secrets * 200;

            if (totalPoints >= 10000) return "S - LEGENDARY HERO";
            if (totalPoints >= 7500) return "A - CASTLE MASTER";
            if (totalPoints >= 5000) return "B - BRAVE ADVENTURER";
            if (totalPoints >= 2500) return "C - WORTHY EXPLORER";
            if (totalPoints >= 1000) return "D - SURVIVOR";
            return "E - LUCKY ESCAPE";
        }

        private System.Collections.IEnumerator BlinkPrompt()
        {
            while (true)
            {
                promptText.alpha = 1f;
                yield return new WaitForSecondsRealtime(0.7f);
                promptText.alpha = 0.3f;
                yield return new WaitForSecondsRealtime(0.3f);
            }
        }

        private void SpawnCelebrationParticles()
        {
            // Create confetti-like particles
            for (int i = 0; i < 5; i++)
            {
                var particle = new GameObject("Confetti");
                particle.transform.SetParent(transform, false);

                var rect = particle.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(Random.value, 1f);
                rect.anchorMax = rect.anchorMin;
                rect.anchoredPosition = Vector2.zero;
                rect.sizeDelta = new Vector2(10, 10);

                var image = particle.AddComponent<Image>();
                image.color = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);

                StartCoroutine(AnimateConfetti(rect, image));
            }
        }

        private System.Collections.IEnumerator AnimateConfetti(RectTransform rect, Image image)
        {
            float duration = 2f;
            float elapsed = 0f;
            Vector2 startPos = rect.anchoredPosition;
            float fallSpeed = Random.Range(200f, 400f);
            float swaySpeed = Random.Range(3f, 6f);
            float swayAmount = Random.Range(30f, 60f);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Fall and sway
                float xOffset = Mathf.Sin(elapsed * swaySpeed) * swayAmount;
                float yOffset = -elapsed * fallSpeed;
                rect.anchoredPosition = startPos + new Vector2(xOffset, yOffset);

                // Fade out
                Color c = image.color;
                c.a = 1f - t;
                image.color = c;

                // Rotate
                rect.Rotate(0, 0, 180f * Time.unscaledDeltaTime);

                yield return null;
            }

            Destroy(rect.gameObject);
        }

        /// <summary>
        /// Creates the victory screen.
        /// </summary>
        public static VictoryScreen Create()
        {
            var obj = new GameObject("VictoryScreen");
            return obj.AddComponent<VictoryScreen>();
        }
    }
}
