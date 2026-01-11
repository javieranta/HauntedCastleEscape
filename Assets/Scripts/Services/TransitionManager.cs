using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Manages screen transitions (fade in/out) for room changes.
    /// </summary>
    public class TransitionManager : MonoBehaviour
    {
        public static TransitionManager Instance { get; private set; }

        [Header("Fade Settings")]
        [SerializeField] private Color fadeColor = Color.black;
        [SerializeField] private float defaultFadeDuration = 0.5f;

        [Header("References")]
        [SerializeField] private Canvas fadeCanvas;
        [SerializeField] private Image fadeImage;

        private bool _isFading = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupFadeCanvas();
        }

        private void SetupFadeCanvas()
        {
            if (fadeCanvas == null)
            {
                // Create fade canvas
                GameObject canvasObj = new GameObject("FadeCanvas");
                canvasObj.transform.SetParent(transform);

                fadeCanvas = canvasObj.AddComponent<Canvas>();
                fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                fadeCanvas.sortingOrder = 9999; // Always on top

                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(256, 192);

                canvasObj.AddComponent<GraphicRaycaster>();
            }

            if (fadeImage == null)
            {
                // Create fade image
                GameObject imageObj = new GameObject("FadeImage");
                imageObj.transform.SetParent(fadeCanvas.transform);

                fadeImage = imageObj.AddComponent<Image>();
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
                fadeImage.raycastTarget = false;

                // Stretch to fill
                RectTransform rect = fadeImage.rectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }

            // Start transparent
            SetFadeAlpha(0f);
        }

        /// <summary>
        /// Fades the screen to black (or fade color).
        /// </summary>
        public Coroutine FadeOut(float duration = -1f)
        {
            if (duration < 0f) duration = defaultFadeDuration;
            return StartCoroutine(FadeCoroutine(0f, 1f, duration));
        }

        /// <summary>
        /// Fades the screen from black back to clear.
        /// </summary>
        public Coroutine FadeIn(float duration = -1f)
        {
            if (duration < 0f) duration = defaultFadeDuration;
            return StartCoroutine(FadeCoroutine(1f, 0f, duration));
        }

        /// <summary>
        /// Performs a full fade out then fade in with an action in between.
        /// </summary>
        public Coroutine FadeOutIn(System.Action midFadeAction, float duration = -1f)
        {
            if (duration < 0f) duration = defaultFadeDuration;
            return StartCoroutine(FadeOutInCoroutine(midFadeAction, duration));
        }

        private IEnumerator FadeCoroutine(float startAlpha, float endAlpha, float duration)
        {
            _isFading = true;

            float elapsed = 0f;
            SetFadeAlpha(startAlpha);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
                SetFadeAlpha(alpha);
                yield return null;
            }

            SetFadeAlpha(endAlpha);
            _isFading = false;
        }

        private IEnumerator FadeOutInCoroutine(System.Action midFadeAction, float duration)
        {
            yield return FadeCoroutine(0f, 1f, duration);

            midFadeAction?.Invoke();

            yield return FadeCoroutine(1f, 0f, duration);
        }

        private void SetFadeAlpha(float alpha)
        {
            if (fadeImage != null)
            {
                fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha);
            }
        }

        /// <summary>
        /// Immediately sets the screen to fully faded (black).
        /// </summary>
        public void SetFaded()
        {
            SetFadeAlpha(1f);
        }

        /// <summary>
        /// Immediately sets the screen to fully clear.
        /// </summary>
        public void SetClear()
        {
            SetFadeAlpha(0f);
        }

        /// <summary>
        /// Performs a quick flash effect.
        /// </summary>
        public Coroutine Flash(Color flashColor, float duration = 0.1f)
        {
            return StartCoroutine(FlashCoroutine(flashColor, duration));
        }

        private IEnumerator FlashCoroutine(Color flashColor, float duration)
        {
            Color originalColor = fadeColor;
            fadeColor = flashColor;

            SetFadeAlpha(1f);
            yield return new WaitForSecondsRealtime(duration);
            SetFadeAlpha(0f);

            fadeColor = originalColor;
        }

        public bool IsFading => _isFading;

        /// <summary>
        /// Gets the current fade alpha for diagnostics.
        /// </summary>
        public float CurrentFadeAlpha => fadeImage != null ? fadeImage.color.a : 0f;

        /// <summary>
        /// Forces the fade to be fully transparent. Use for debugging stuck fades.
        /// </summary>
        public void ForceTransparent()
        {
            SetFadeAlpha(0f);
            _isFading = false;
            Debug.LogWarning("[TransitionManager] FORCE cleared fade to transparent!");
        }
    }
}
