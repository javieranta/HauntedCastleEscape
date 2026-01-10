using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HauntedCastle.Services;
using HauntedCastle.Data;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Displays the current floor and room name in the UI.
    /// Updates automatically when the player changes rooms.
    /// </summary>
    public class FloorIndicatorUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI floorText;
        [SerializeField] private TextMeshProUGUI roomNameText;
        [SerializeField] private Image floorIcon;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float displayDuration = 3f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        [Header("Floor Colors")]
        [SerializeField] private Color basementColor = new Color(0.5f, 0.3f, 0.5f);
        [SerializeField] private Color castleColor = new Color(0.8f, 0.7f, 0.4f);
        [SerializeField] private Color towerColor = new Color(0.4f, 0.6f, 0.9f);

        private float _timer;
        private bool _isShowing;
        private int _lastFloor = -1;

        private void Awake()
        {
            // Create UI elements if not assigned
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f;
        }

        private void Start()
        {
            // Subscribe to room load events
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomLoaded;
            }
        }

        private void OnDestroy()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomLoaded;
            }
        }

        private void Update()
        {
            if (_isShowing)
            {
                _timer -= Time.deltaTime;

                if (_timer <= 0)
                {
                    // Start fade out
                    _isShowing = false;
                    StartCoroutine(FadeOut());
                }
            }
        }

        private void OnRoomLoaded(RoomData roomData)
        {
            if (roomData == null) return;

            // Always show room name
            UpdateDisplay(roomData);

            // Only show full announcement if floor changed
            if (roomData.floorNumber != _lastFloor)
            {
                ShowFloorChange(roomData);
                _lastFloor = roomData.floorNumber;
            }
            else
            {
                // Brief room name display
                ShowBrief(roomData);
            }
        }

        private void UpdateDisplay(RoomData roomData)
        {
            string floorName = RoomDatabase.GetFloorName(roomData.floorNumber);
            Color floorColor = GetFloorColor(roomData.floorNumber);

            if (floorText != null)
            {
                floorText.text = floorName;
                floorText.color = floorColor;
            }

            if (roomNameText != null)
            {
                roomNameText.text = roomData.displayName;
            }

            if (floorIcon != null)
            {
                floorIcon.color = floorColor;
            }
        }

        private void ShowFloorChange(RoomData roomData)
        {
            StopAllCoroutines();
            StartCoroutine(ShowFloorChangeSequence(roomData));
        }

        private void ShowBrief(RoomData roomData)
        {
            StopAllCoroutines();
            StartCoroutine(ShowBriefSequence());
        }

        private System.Collections.IEnumerator ShowFloorChangeSequence(RoomData roomData)
        {
            // Fade in
            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }
            canvasGroup.alpha = 1f;

            // Wait
            _isShowing = true;
            _timer = displayDuration;
            // CRITICAL: Use WaitForSecondsRealtime to work when timeScale = 0
            yield return new WaitForSecondsRealtime(displayDuration);
        }

        private System.Collections.IEnumerator ShowBriefSequence()
        {
            // Quick fade in
            canvasGroup.alpha = 1f;

            // Brief display
            _isShowing = true;
            _timer = 1.5f;
            // CRITICAL: Use WaitForSecondsRealtime to work when timeScale = 0
            yield return new WaitForSecondsRealtime(1.5f);
        }

        private System.Collections.IEnumerator FadeOut()
        {
            float elapsed = 0f;
            float startAlpha = canvasGroup.alpha;

            while (elapsed < fadeOutDuration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
                yield return null;
            }
            canvasGroup.alpha = 0f;
        }

        private Color GetFloorColor(int floor)
        {
            return floor switch
            {
                0 => basementColor,
                1 => castleColor,
                2 => towerColor,
                _ => Color.white
            };
        }

        /// <summary>
        /// Creates the floor indicator UI if it doesn't exist.
        /// </summary>
        public static FloorIndicatorUI CreateUI()
        {
            // Find or create canvas
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                var canvasObj = new GameObject("FloorIndicatorCanvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Create floor indicator panel
            var indicatorObj = new GameObject("FloorIndicator");
            indicatorObj.transform.SetParent(canvas.transform, false);

            var rectTransform = indicatorObj.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 1f);
            rectTransform.anchoredPosition = new Vector2(0, -20);
            rectTransform.sizeDelta = new Vector2(400, 80);

            // Background panel
            var bg = indicatorObj.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.7f);

            // Add the component
            var indicator = indicatorObj.AddComponent<FloorIndicatorUI>();

            // Create floor text
            var floorTextObj = new GameObject("FloorText");
            floorTextObj.transform.SetParent(indicatorObj.transform, false);
            var floorTextRect = floorTextObj.AddComponent<RectTransform>();
            floorTextRect.anchorMin = Vector2.zero;
            floorTextRect.anchorMax = new Vector2(1, 0.6f);
            floorTextRect.offsetMin = new Vector2(10, 0);
            floorTextRect.offsetMax = new Vector2(-10, 0);

            var floorTmp = floorTextObj.AddComponent<TextMeshProUGUI>();
            floorTmp.text = "Floor";
            floorTmp.fontSize = 20;
            floorTmp.alignment = TextAlignmentOptions.Center;
            floorTmp.color = Color.white;
            indicator.floorText = floorTmp;

            // Create room name text
            var roomTextObj = new GameObject("RoomText");
            roomTextObj.transform.SetParent(indicatorObj.transform, false);
            var roomTextRect = roomTextObj.AddComponent<RectTransform>();
            roomTextRect.anchorMin = new Vector2(0, 0.6f);
            roomTextRect.anchorMax = Vector2.one;
            roomTextRect.offsetMin = new Vector2(10, 0);
            roomTextRect.offsetMax = new Vector2(-10, -5);

            var roomTmp = roomTextObj.AddComponent<TextMeshProUGUI>();
            roomTmp.text = "Room Name";
            roomTmp.fontSize = 28;
            roomTmp.fontStyle = FontStyles.Bold;
            roomTmp.alignment = TextAlignmentOptions.Center;
            roomTmp.color = Color.white;
            indicator.roomNameText = roomTmp;

            indicator.canvasGroup = indicatorObj.AddComponent<CanvasGroup>();
            indicator.canvasGroup.alpha = 0f;

            return indicator;
        }
    }
}
