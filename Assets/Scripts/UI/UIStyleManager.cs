using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Manages consistent UI styling across the game.
    /// Provides retro-style pixel art UI appearance.
    /// </summary>
    public static class UIStyleManager
    {
        // Color palette
        public static readonly Color PrimaryColor = new Color(0.8f, 0.6f, 0.2f);      // Gold
        public static readonly Color SecondaryColor = new Color(0.4f, 0.1f, 0.1f);     // Dark red
        public static readonly Color BackgroundColor = new Color(0.1f, 0.1f, 0.15f);   // Dark blue-gray
        public static readonly Color TextColor = new Color(0.95f, 0.95f, 0.9f);        // Off-white
        public static readonly Color AccentColor = new Color(0.3f, 0.7f, 0.3f);        // Green
        public static readonly Color DangerColor = new Color(0.8f, 0.2f, 0.2f);        // Red
        public static readonly Color DisabledColor = new Color(0.5f, 0.5f, 0.5f);      // Gray

        // Font settings
        public const int TitleFontSize = 48;
        public const int HeaderFontSize = 32;
        public const int BodyFontSize = 24;
        public const int SmallFontSize = 18;

        /// <summary>
        /// Styles a button with retro appearance.
        /// </summary>
        public static void StyleButton(Button button, ButtonStyle style = ButtonStyle.Primary)
        {
            if (button == null) return;

            var colors = button.colors;

            switch (style)
            {
                case ButtonStyle.Primary:
                    colors.normalColor = PrimaryColor;
                    colors.highlightedColor = PrimaryColor * 1.2f;
                    colors.pressedColor = PrimaryColor * 0.8f;
                    colors.selectedColor = PrimaryColor * 1.1f;
                    break;
                case ButtonStyle.Secondary:
                    colors.normalColor = SecondaryColor;
                    colors.highlightedColor = SecondaryColor * 1.3f;
                    colors.pressedColor = SecondaryColor * 0.7f;
                    colors.selectedColor = SecondaryColor * 1.2f;
                    break;
                case ButtonStyle.Danger:
                    colors.normalColor = DangerColor;
                    colors.highlightedColor = DangerColor * 1.2f;
                    colors.pressedColor = DangerColor * 0.7f;
                    colors.selectedColor = DangerColor * 1.1f;
                    break;
            }

            colors.disabledColor = DisabledColor;
            colors.fadeDuration = 0.1f;
            button.colors = colors;

            // Style the text if it exists
            var text = button.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.color = TextColor;
                text.fontSize = BodyFontSize;
            }
        }

        /// <summary>
        /// Styles a panel with retro appearance.
        /// </summary>
        public static void StylePanel(Image panel, float alpha = 0.9f)
        {
            if (panel == null) return;

            Color bgColor = BackgroundColor;
            bgColor.a = alpha;
            panel.color = bgColor;

            // Add border effect if possible
            var outline = panel.GetComponent<Outline>();
            if (outline == null)
                outline = panel.gameObject.AddComponent<Outline>();

            outline.effectColor = PrimaryColor;
            outline.effectDistance = new Vector2(2, -2);
        }

        /// <summary>
        /// Styles text with retro font appearance.
        /// </summary>
        public static void StyleText(TextMeshProUGUI text, TextStyle style = TextStyle.Body)
        {
            if (text == null) return;

            text.color = TextColor;

            switch (style)
            {
                case TextStyle.Title:
                    text.fontSize = TitleFontSize;
                    text.fontStyle = FontStyles.Bold;
                    break;
                case TextStyle.Header:
                    text.fontSize = HeaderFontSize;
                    text.fontStyle = FontStyles.Bold;
                    break;
                case TextStyle.Body:
                    text.fontSize = BodyFontSize;
                    text.fontStyle = FontStyles.Normal;
                    break;
                case TextStyle.Small:
                    text.fontSize = SmallFontSize;
                    text.fontStyle = FontStyles.Normal;
                    break;
            }

            // Pixel-perfect rendering
            text.enableWordWrapping = true;
            text.overflowMode = TextOverflowModes.Truncate;
        }

        /// <summary>
        /// Styles a slider with retro appearance.
        /// </summary>
        public static void StyleSlider(Slider slider)
        {
            if (slider == null) return;

            var background = slider.transform.Find("Background")?.GetComponent<Image>();
            var fill = slider.fillRect?.GetComponent<Image>();
            var handle = slider.handleRect?.GetComponent<Image>();

            if (background != null)
                background.color = BackgroundColor;

            if (fill != null)
                fill.color = AccentColor;

            if (handle != null)
                handle.color = PrimaryColor;
        }

        /// <summary>
        /// Creates a styled health bar.
        /// </summary>
        public static GameObject CreateHealthBar(Transform parent, Vector2 size)
        {
            var healthBar = new GameObject("HealthBar");
            healthBar.transform.SetParent(parent, false);

            var rectTransform = healthBar.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;

            // Background
            var bgImage = healthBar.AddComponent<Image>();
            bgImage.color = BackgroundColor;

            // Fill container
            var fillContainer = new GameObject("Fill");
            fillContainer.transform.SetParent(healthBar.transform, false);
            var fillRect = fillContainer.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2, 2);
            fillRect.offsetMax = new Vector2(-2, -2);

            // Fill
            var fill = new GameObject("FillImage");
            fill.transform.SetParent(fillContainer.transform, false);
            var fillRectTransform = fill.AddComponent<RectTransform>();
            fillRectTransform.anchorMin = Vector2.zero;
            fillRectTransform.anchorMax = new Vector2(1, 1);
            fillRectTransform.offsetMin = Vector2.zero;
            fillRectTransform.offsetMax = Vector2.zero;
            fillRectTransform.pivot = new Vector2(0, 0.5f);

            var fillImage = fill.AddComponent<Image>();
            fillImage.color = DangerColor;

            return healthBar;
        }

        /// <summary>
        /// Creates a styled inventory slot.
        /// </summary>
        public static GameObject CreateInventorySlot(Transform parent, Vector2 size)
        {
            var slot = new GameObject("InventorySlot");
            slot.transform.SetParent(parent, false);

            var rectTransform = slot.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;

            var image = slot.AddComponent<Image>();
            image.color = new Color(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, 0.8f);

            var outline = slot.AddComponent<Outline>();
            outline.effectColor = PrimaryColor;
            outline.effectDistance = new Vector2(1, -1);

            // Icon placeholder
            var icon = new GameObject("Icon");
            icon.transform.SetParent(slot.transform, false);
            var iconRect = icon.AddComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0.1f, 0.1f);
            iconRect.anchorMax = new Vector2(0.9f, 0.9f);
            iconRect.offsetMin = Vector2.zero;
            iconRect.offsetMax = Vector2.zero;

            var iconImage = icon.AddComponent<Image>();
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;

            return slot;
        }

        /// <summary>
        /// Applies retro pixel style to an image.
        /// </summary>
        public static void ApplyPixelStyle(Image image)
        {
            if (image == null || image.sprite == null) return;

            // Ensure smooth rendering for modern look
            if (image.sprite.texture != null)
            {
                image.sprite.texture.filterMode = FilterMode.Bilinear;
            }
        }

        public enum ButtonStyle
        {
            Primary,
            Secondary,
            Danger
        }

        public enum TextStyle
        {
            Title,
            Header,
            Body,
            Small
        }
    }
}
