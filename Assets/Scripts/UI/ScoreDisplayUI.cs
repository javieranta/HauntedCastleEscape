using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HauntedCastle.Core;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Displays the current score, combo, and multiplier.
    /// </summary>
    public class ScoreDisplayUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private CanvasGroup comboGroup;

        [Header("Animation")]
        [SerializeField] private float scorePunchScale = 1.2f;
        [SerializeField] private float scorePunchDuration = 0.15f;
        [SerializeField] private float comboFadeDuration = 0.3f;

        private int _displayedScore;
        private int _targetScore;
        private float _scoreAnimTimer;
        private Vector3 _originalScoreScale;
        private bool _isComboVisible;

        private void Start()
        {
            CreateUI();

            // Subscribe to score events
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += OnScoreChanged;
                ScoreManager.Instance.OnComboChanged += OnComboChanged;
                ScoreManager.Instance.OnPointsAwarded += OnPointsAwarded;
            }

            UpdateScoreDisplay();
        }

        private void OnDestroy()
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;
                ScoreManager.Instance.OnComboChanged -= OnComboChanged;
                ScoreManager.Instance.OnPointsAwarded -= OnPointsAwarded;
            }
        }

        private void Update()
        {
            // Animate score counting up
            if (_displayedScore != _targetScore)
            {
                _displayedScore = (int)Mathf.MoveTowards(_displayedScore, _targetScore, Time.deltaTime * 1000);
                UpdateScoreDisplay();
            }

            // Animate score scale punch
            if (_scoreAnimTimer > 0)
            {
                _scoreAnimTimer -= Time.deltaTime;
                float t = 1f - (_scoreAnimTimer / scorePunchDuration);
                float scale = Mathf.Lerp(scorePunchScale, 1f, t);
                if (scoreText != null)
                {
                    scoreText.transform.localScale = _originalScoreScale * scale;
                }
            }
        }

        private void CreateUI()
        {
            // Find or create canvas
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                canvas = FindFirstObjectByType<Canvas>();
                if (canvas == null)
                {
                    var canvasObj = new GameObject("ScoreCanvas");
                    canvas = canvasObj.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvas.sortingOrder = 100;
                    canvasObj.AddComponent<CanvasScaler>();
                    canvasObj.AddComponent<GraphicRaycaster>();
                }
            }

            // Create score container
            var scoreContainer = new GameObject("ScoreDisplay");
            scoreContainer.transform.SetParent(canvas.transform, false);

            var rect = scoreContainer.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1, 1);
            rect.anchorMax = new Vector2(1, 1);
            rect.pivot = new Vector2(1, 1);
            rect.anchoredPosition = new Vector2(-10, -10);
            rect.sizeDelta = new Vector2(200, 50);

            // Score text
            var scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(scoreContainer.transform, false);
            var scoreRect = scoreObj.AddComponent<RectTransform>();
            scoreRect.anchorMin = Vector2.zero;
            scoreRect.anchorMax = Vector2.one;
            scoreRect.offsetMin = Vector2.zero;
            scoreRect.offsetMax = Vector2.zero;

            scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
            scoreText.text = "0";
            scoreText.fontSize = 32;
            scoreText.fontStyle = FontStyles.Bold;
            scoreText.alignment = TextAlignmentOptions.Right;
            scoreText.color = Color.white;
            _originalScoreScale = scoreText.transform.localScale;

            // Combo text (below score)
            var comboObj = new GameObject("ComboText");
            comboObj.transform.SetParent(scoreContainer.transform, false);
            var comboRect = comboObj.AddComponent<RectTransform>();
            comboRect.anchorMin = new Vector2(0, 0);
            comboRect.anchorMax = new Vector2(1, 0.4f);
            comboRect.offsetMin = Vector2.zero;
            comboRect.offsetMax = Vector2.zero;

            comboText = comboObj.AddComponent<TextMeshProUGUI>();
            comboText.text = "";
            comboText.fontSize = 18;
            comboText.alignment = TextAlignmentOptions.Right;
            comboText.color = Color.yellow;

            comboGroup = comboObj.AddComponent<CanvasGroup>();
            comboGroup.alpha = 0f;
        }

        private void OnScoreChanged(int newScore)
        {
            _targetScore = newScore;
            PunchScale();
        }

        private void OnComboChanged(int combo, int multiplier)
        {
            if (combo > 0)
            {
                if (comboText != null)
                {
                    comboText.text = $"COMBO x{multiplier}!";
                }
                ShowCombo();
            }
            else
            {
                HideCombo();
            }
        }

        private void OnPointsAwarded(int points, string reason)
        {
            PunchScale();
        }

        private void UpdateScoreDisplay()
        {
            if (scoreText != null)
            {
                scoreText.text = _displayedScore.ToString("N0");
            }
        }

        private void PunchScale()
        {
            _scoreAnimTimer = scorePunchDuration;
        }

        private void ShowCombo()
        {
            if (!_isComboVisible && comboGroup != null)
            {
                _isComboVisible = true;
                StartCoroutine(FadeCombo(1f));
            }
        }

        private void HideCombo()
        {
            if (_isComboVisible && comboGroup != null)
            {
                _isComboVisible = false;
                StartCoroutine(FadeCombo(0f));
            }
        }

        private System.Collections.IEnumerator FadeCombo(float targetAlpha)
        {
            float startAlpha = comboGroup.alpha;
            float elapsed = 0f;

            while (elapsed < comboFadeDuration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                comboGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / comboFadeDuration);
                yield return null;
            }

            comboGroup.alpha = targetAlpha;
        }

        /// <summary>
        /// Creates the ScoreDisplayUI if it doesn't exist.
        /// </summary>
        public static ScoreDisplayUI CreateScoreDisplay()
        {
            var obj = new GameObject("ScoreDisplayUI");
            return obj.AddComponent<ScoreDisplayUI>();
        }
    }
}
