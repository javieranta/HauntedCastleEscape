using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using HauntedCastle.Player;
using HauntedCastle.Services;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Displays contextual tutorial hints based on player actions and game state.
    /// Hints fade in/out and are shown only once per session.
    /// </summary>
    public class TutorialHintUI : MonoBehaviour
    {
        public static TutorialHintUI Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float hintDisplayDuration = 4f;
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private bool enableTutorial = true;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI hintText;
        [SerializeField] private CanvasGroup canvasGroup;

        // Track shown hints
        private HashSet<string> _shownHints = new HashSet<string>();
        private Queue<string> _hintQueue = new Queue<string>();
        private float _displayTimer;
        private bool _isShowingHint;
        private Canvas _canvas;

        // Hint definitions
        private static readonly Dictionary<string, string> HintMessages = new Dictionary<string, string>
        {
            { "movement", "Use WASD or Arrow Keys to move" },
            { "attack", "Press SPACE or Left Click to attack" },
            { "pickup", "Walk over items to pick them up" },
            { "door", "Approach doors to enter new rooms" },
            { "locked_door", "Find the matching KEY to unlock this door" },
            { "stairs", "Use stairs to move between floors" },
            { "enemy", "Defeat enemies or avoid them!" },
            { "special_enemy", "Some enemies require SPECIAL ITEMS to defeat" },
            { "health", "Find FOOD to restore health" },
            { "key_piece", "Collect all KEY PIECES to form the Great Key" },
            { "combo", "Chain attacks for COMBO multipliers!" },
            { "secret", "Look for hidden passages in the walls" },
            { "pause", "Press ESC to pause the game" },
            { "low_health", "Your health is low! Find food!" },
            { "boss", "A powerful enemy blocks your path!" }
        };

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            CreateUI();

            // Show initial movement hint
            if (enableTutorial)
            {
                ShowHint("movement");
            }
        }

        private void Update()
        {
            if (!enableTutorial) return;

            // Process hint display
            if (_isShowingHint)
            {
                _displayTimer -= Time.deltaTime;
                if (_displayTimer <= 0)
                {
                    StartCoroutine(FadeOut());
                }
            }
            else if (_hintQueue.Count > 0)
            {
                ShowNextHint();
            }

            // Context-based hints
            CheckContextualHints();
        }

        private void CreateUI()
        {
            // Find or create canvas
            _canvas = FindFirstObjectByType<Canvas>();
            if (_canvas == null)
            {
                var canvasObj = new GameObject("TutorialCanvas");
                _canvas = canvasObj.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = 90;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Create hint container
            var hintContainer = new GameObject("TutorialHint");
            hintContainer.transform.SetParent(_canvas.transform, false);

            var rect = hintContainer.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.15f);
            rect.anchorMax = new Vector2(0.5f, 0.25f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(500, 60);

            canvasGroup = hintContainer.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(hintContainer.transform, false);

            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var bgImage = bgObj.AddComponent<UnityEngine.UI.Image>();
            bgImage.color = new Color(0, 0, 0, 0.7f);

            // Hint text
            var textObj = new GameObject("HintText");
            textObj.transform.SetParent(hintContainer.transform, false);

            var textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 5);
            textRect.offsetMax = new Vector2(-10, -5);

            hintText = textObj.AddComponent<TextMeshProUGUI>();
            hintText.text = "";
            hintText.fontSize = 24;
            hintText.color = Color.white;
            hintText.alignment = TextAlignmentOptions.Center;
        }

        /// <summary>
        /// Shows a tutorial hint by key.
        /// </summary>
        public void ShowHint(string hintKey)
        {
            if (!enableTutorial) return;
            if (_shownHints.Contains(hintKey)) return;
            if (!HintMessages.ContainsKey(hintKey)) return;

            _shownHints.Add(hintKey);
            _hintQueue.Enqueue(HintMessages[hintKey]);

            Debug.Log($"[TutorialHintUI] Queued hint: {hintKey}");
        }

        /// <summary>
        /// Shows a custom hint message.
        /// </summary>
        public void ShowCustomHint(string message, string key = null)
        {
            if (!enableTutorial) return;

            if (!string.IsNullOrEmpty(key))
            {
                if (_shownHints.Contains(key)) return;
                _shownHints.Add(key);
            }

            _hintQueue.Enqueue(message);
        }

        private void ShowNextHint()
        {
            if (_hintQueue.Count == 0) return;

            string message = _hintQueue.Dequeue();
            hintText.text = message;
            _displayTimer = hintDisplayDuration;
            _isShowingHint = true;

            StartCoroutine(FadeIn());
        }

        private System.Collections.IEnumerator FadeIn()
        {
            while (canvasGroup.alpha < 1f)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        private System.Collections.IEnumerator FadeOut()
        {
            while (canvasGroup.alpha > 0f)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                canvasGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
                yield return null;
            }
            canvasGroup.alpha = 0f;
            _isShowingHint = false;
        }

        private void CheckContextualHints()
        {
            // Check player health
            if (PlayerHealth.Instance != null)
            {
                float healthPercent = PlayerHealth.Instance.CurrentHealth / (float)PlayerHealth.Instance.MaxHealth;
                if (healthPercent <= 0.25f)
                {
                    ShowHint("low_health");
                }
            }
        }

        /// <summary>
        /// Enables or disables tutorial hints.
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            enableTutorial = enabled;
            if (!enabled && canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// Resets shown hints (for new game).
        /// </summary>
        public void ResetHints()
        {
            _shownHints.Clear();
            _hintQueue.Clear();
            _isShowingHint = false;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        /// <summary>
        /// Creates the tutorial hint system if it doesn't exist.
        /// </summary>
        public static TutorialHintUI EnsureExists()
        {
            if (Instance == null)
            {
                var obj = new GameObject("TutorialHintUI");
                Instance = obj.AddComponent<TutorialHintUI>();
            }
            return Instance;
        }
    }
}
