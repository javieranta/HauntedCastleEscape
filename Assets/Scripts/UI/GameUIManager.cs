using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HauntedCastle.Core;
using HauntedCastle.Services;
using HauntedCastle.Data;
using HauntedCastle.Inventory;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Manages all game UI elements including health, inventory, and floor indicator.
    /// Automatically creates UI elements if they don't exist.
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        public static GameUIManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private FloorIndicatorUI floorIndicator;
        [SerializeField] private HealthBarUI healthBar;
        [SerializeField] private InventoryDisplayUI inventoryDisplay;

        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI keyIndicatorText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeUI();
        }

        private void InitializeUI()
        {
            // Create main canvas if needed
            if (mainCanvas == null)
            {
                var canvasObj = new GameObject("GameCanvas");
                canvasObj.transform.SetParent(transform);
                mainCanvas = canvasObj.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                mainCanvas.sortingOrder = 100;

                var scaler = canvasObj.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);

                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Create floor indicator
            if (floorIndicator == null)
            {
                floorIndicator = FloorIndicatorUI.CreateUI();
            }

            // Create health bar
            if (healthBar == null)
            {
                healthBar = CreateHealthBar();
            }

            // Create key indicator
            CreateKeyIndicator();
        }

        private HealthBarUI CreateHealthBar()
        {
            var healthObj = new GameObject("HealthBar");
            healthObj.transform.SetParent(mainCanvas.transform, false);

            var rect = healthObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(20, -20);
            rect.sizeDelta = new Vector2(200, 30);

            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(healthObj.transform, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0, 0, 0.8f);

            // Fill
            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(healthObj.transform, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2, 2);
            fillRect.offsetMax = new Vector2(-2, -2);
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = new Color(0.8f, 0.2f, 0.2f, 1f);

            // Label
            var labelObj = new GameObject("Label");
            labelObj.transform.SetParent(healthObj.transform, false);
            var labelRect = labelObj.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var labelText = labelObj.AddComponent<TextMeshProUGUI>();
            labelText.text = "HEALTH";
            labelText.fontSize = 14;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.color = Color.white;

            var healthUI = healthObj.AddComponent<HealthBarUI>();
            healthUI.Initialize(fillRect);

            return healthUI;
        }

        private void CreateKeyIndicator()
        {
            var keyObj = new GameObject("KeyIndicator");
            keyObj.transform.SetParent(mainCanvas.transform, false);

            var rect = keyObj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(20, -60);
            rect.sizeDelta = new Vector2(200, 25);

            keyIndicatorText = keyObj.AddComponent<TextMeshProUGUI>();
            keyIndicatorText.text = "Keys: None";
            keyIndicatorText.fontSize = 16;
            keyIndicatorText.alignment = TextAlignmentOptions.Left;
            keyIndicatorText.color = Color.white;
        }

        private void Update()
        {
            UpdateKeyIndicator();
        }

        private void UpdateKeyIndicator()
        {
            if (keyIndicatorText == null) return;

            if (PlayerInventory.Instance != null)
            {
                var keys = PlayerInventory.Instance.GetOwnedKeys();
                if (keys.Count > 0)
                {
                    string keyStr = "Keys: ";
                    foreach (var key in keys)
                    {
                        keyStr += GetKeyColorSymbol(key) + " ";
                    }
                    keyIndicatorText.text = keyStr;
                }
                else
                {
                    keyIndicatorText.text = "Keys: None";
                }
            }
        }

        private string GetKeyColorSymbol(KeyColor color)
        {
            return color switch
            {
                KeyColor.Red => "<color=#FF4444>R</color>",
                KeyColor.Blue => "<color=#4444FF>B</color>",
                KeyColor.Green => "<color=#44FF44>G</color>",
                KeyColor.Yellow => "<color=#FFFF44>Y</color>",
                KeyColor.Cyan => "<color=#44FFFF>C</color>",
                KeyColor.Magenta => "<color=#FF44FF>M</color>",
                _ => "?"
            };
        }
    }

    /// <summary>
    /// Simple health bar UI component.
    /// </summary>
    public class HealthBarUI : MonoBehaviour
    {
        private RectTransform _fillRect;
        private float _maxWidth;

        public void Initialize(RectTransform fillRect)
        {
            _fillRect = fillRect;
            _maxWidth = _fillRect.rect.width;
        }

        public void SetHealth(float current, float max)
        {
            if (_fillRect == null) return;

            float ratio = Mathf.Clamp01(current / max);
            var sizeDelta = _fillRect.sizeDelta;
            sizeDelta.x = _maxWidth * ratio;
            _fillRect.sizeDelta = sizeDelta;
        }
    }

    /// <summary>
    /// Displays collected inventory items.
    /// </summary>
    public class InventoryDisplayUI : MonoBehaviour
    {
        // Placeholder for inventory display
    }
}
