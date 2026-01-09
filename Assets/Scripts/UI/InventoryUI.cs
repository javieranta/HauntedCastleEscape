using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Inventory;

namespace HauntedCastle.UI
{
    /// <summary>
    /// UI component for displaying the 3-slot inventory.
    /// Uses OnGUI for development - will be replaced with proper UI later.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool showInventory = true;
        [SerializeField] private float slotSize = 50f;
        [SerializeField] private float slotSpacing = 5f;
        [SerializeField] private float margin = 10f;

        [Header("Position")]
        [SerializeField] private AnchorPosition anchor = AnchorPosition.BottomCenter;

        [Header("Colors")]
        [SerializeField] private Color slotBackgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        [SerializeField] private Color slotBorderColor = new Color(0.5f, 0.5f, 0.5f);
        [SerializeField] private Color selectedSlotColor = new Color(1f, 0.8f, 0f);
        [SerializeField] private Color keyPieceActiveColor = new Color(1f, 0.84f, 0f);
        [SerializeField] private Color keyPieceInactiveColor = new Color(0.3f, 0.3f, 0.3f);

        [Header("Key Display")]
        [SerializeField] private float keyPieceSize = 20f;

        private Texture2D _whiteTexture;
        private GUIStyle _slotStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _keyBindStyle;
        private int _selectedSlot = -1;

        private enum AnchorPosition
        {
            BottomLeft,
            BottomCenter,
            BottomRight,
            TopLeft,
            TopCenter,
            TopRight
        }

        private void Start()
        {
            // Create white texture for drawing
            _whiteTexture = new Texture2D(1, 1);
            _whiteTexture.SetPixel(0, 0, Color.white);
            _whiteTexture.Apply();

            // Subscribe to inventory events
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.OnInventoryChanged += OnInventoryChanged;
            }
        }

        private void OnDestroy()
        {
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.OnInventoryChanged -= OnInventoryChanged;
            }
        }

        private void Update()
        {
            // Handle slot selection via keyboard (1, 2, 3)
            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);

            // Use selected item
            if (_selectedSlot >= 0 && Input.GetKeyDown(KeyCode.U))
            {
                UseSelectedItem();
            }

            // Drop selected item
            if (_selectedSlot >= 0 && Input.GetKeyDown(KeyCode.G))
            {
                DropSelectedItem();
            }
        }

        private void SelectSlot(int slot)
        {
            if (PlayerInventory.Instance == null) return;

            if (slot == _selectedSlot)
            {
                _selectedSlot = -1; // Deselect
            }
            else if (slot < PlayerInventory.Instance.ItemCount)
            {
                _selectedSlot = slot;
            }
        }

        private void UseSelectedItem()
        {
            if (_selectedSlot < 0) return;

            PlayerInventory.Instance?.UseItem(_selectedSlot);
            _selectedSlot = -1;
        }

        private void DropSelectedItem()
        {
            if (_selectedSlot < 0) return;

            var player = Player.PlayerController.Instance;
            if (player != null && PlayerInventory.Instance != null)
            {
                Vector2 dropPos = (Vector2)player.transform.position + player.FacingDirection * 1f;
                PlayerInventory.Instance.DropItem(_selectedSlot, dropPos);
                _selectedSlot = -1;
            }
        }

        private void OnInventoryChanged()
        {
            // Validate selected slot
            if (PlayerInventory.Instance != null && _selectedSlot >= PlayerInventory.Instance.ItemCount)
            {
                _selectedSlot = -1;
            }
        }

        private void OnGUI()
        {
            if (!showInventory) return;

            InitStyles();

            // Calculate position based on anchor
            float totalWidth = (slotSize * 3) + (slotSpacing * 2);
            float totalHeight = slotSize + keyPieceSize + slotSpacing + 25f; // +25 for labels

            Rect containerRect = GetAnchoredRect(totalWidth, totalHeight);

            // Draw inventory container
            DrawInventorySlots(containerRect);

            // Draw key pieces indicator
            DrawKeyPiecesIndicator(containerRect);

            // Draw controls hint
            DrawControlsHint(containerRect);
        }

        private void InitStyles()
        {
            if (_slotStyle == null)
            {
                _slotStyle = new GUIStyle(GUI.skin.box);
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.fontSize = 10;
                _labelStyle.alignment = TextAnchor.MiddleCenter;
                _labelStyle.normal.textColor = Color.white;
            }

            if (_keyBindStyle == null)
            {
                _keyBindStyle = new GUIStyle(GUI.skin.label);
                _keyBindStyle.fontSize = 8;
                _keyBindStyle.alignment = TextAnchor.UpperLeft;
                _keyBindStyle.normal.textColor = Color.gray;
            }
        }

        private Rect GetAnchoredRect(float width, float height)
        {
            float x = 0, y = 0;

            switch (anchor)
            {
                case AnchorPosition.BottomLeft:
                    x = margin;
                    y = Screen.height - height - margin;
                    break;
                case AnchorPosition.BottomCenter:
                    x = (Screen.width - width) / 2;
                    y = Screen.height - height - margin;
                    break;
                case AnchorPosition.BottomRight:
                    x = Screen.width - width - margin;
                    y = Screen.height - height - margin;
                    break;
                case AnchorPosition.TopLeft:
                    x = margin;
                    y = margin + 90; // Below player HUD
                    break;
                case AnchorPosition.TopCenter:
                    x = (Screen.width - width) / 2;
                    y = margin;
                    break;
                case AnchorPosition.TopRight:
                    x = Screen.width - width - margin;
                    y = margin + 90;
                    break;
            }

            return new Rect(x, y, width, height);
        }

        private void DrawInventorySlots(Rect container)
        {
            int maxSlots = PlayerInventory.Instance?.MaxSlots ?? 3;

            for (int i = 0; i < maxSlots; i++)
            {
                float slotX = container.x + i * (slotSize + slotSpacing);
                float slotY = container.y;
                Rect slotRect = new Rect(slotX, slotY, slotSize, slotSize);

                // Draw slot background
                GUI.color = slotBackgroundColor;
                GUI.DrawTexture(slotRect, _whiteTexture);

                // Draw border (highlight if selected)
                GUI.color = i == _selectedSlot ? selectedSlotColor : slotBorderColor;
                DrawBorder(slotRect, 2);

                // Draw item if present
                ItemData item = PlayerInventory.Instance?.GetItem(i);
                if (item != null)
                {
                    DrawItemInSlot(slotRect, item);
                }

                // Draw slot number
                GUI.color = Color.white;
                Rect keyRect = new Rect(slotX + 2, slotY + 2, 12, 12);
                GUI.Label(keyRect, (i + 1).ToString(), _keyBindStyle);
            }

            GUI.color = Color.white;
        }

        private void DrawItemInSlot(Rect slotRect, ItemData item)
        {
            // Draw item color square
            Color itemColor = GetItemColor(item);
            float iconPadding = 6f;
            Rect iconRect = new Rect(
                slotRect.x + iconPadding,
                slotRect.y + iconPadding,
                slotRect.width - iconPadding * 2,
                slotRect.height - iconPadding * 2
            );

            GUI.color = itemColor;
            GUI.DrawTexture(iconRect, _whiteTexture);

            // Draw item type indicator
            GUI.color = Color.white;
            string typeChar = item.itemType switch
            {
                ItemType.Food => "F",
                ItemType.Key => "K",
                ItemType.Treasure => "T",
                ItemType.Special => "S",
                _ => "?"
            };

            GUIStyle centerStyle = new GUIStyle(_labelStyle);
            centerStyle.fontSize = 14;
            centerStyle.fontStyle = FontStyle.Bold;
            GUI.Label(iconRect, typeChar, centerStyle);
        }

        private Color GetItemColor(ItemData item)
        {
            if (item.itemType == ItemType.Key)
            {
                return item.keyColor switch
                {
                    KeyColor.Red => Color.red,
                    KeyColor.Blue => Color.blue,
                    KeyColor.Green => Color.green,
                    KeyColor.Yellow => Color.yellow,
                    KeyColor.Cyan => Color.cyan,
                    KeyColor.Magenta => Color.magenta,
                    _ => Color.white
                };
            }

            return item.itemType switch
            {
                ItemType.Food => new Color(0.4f, 0.8f, 0.4f),
                ItemType.Treasure => new Color(1f, 0.6f, 0.2f),
                ItemType.Special => new Color(0.8f, 0.2f, 0.8f),
                _ => Color.white
            };
        }

        private void DrawBorder(Rect rect, int thickness)
        {
            // Top
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), _whiteTexture);
            // Bottom
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), _whiteTexture);
            // Left
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), _whiteTexture);
            // Right
            GUI.DrawTexture(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), _whiteTexture);
        }

        private void DrawKeyPiecesIndicator(Rect container)
        {
            float y = container.y + slotSize + slotSpacing;
            float totalKeyWidth = keyPieceSize * 3 + slotSpacing * 2;
            float startX = container.x + (container.width - totalKeyWidth) / 2;

            // Label
            Rect labelRect = new Rect(startX, y, totalKeyWidth, 15);
            GUI.color = Color.white;
            GUI.Label(labelRect, "Great Key:", _labelStyle);

            y += 15;

            // Draw three key piece indicators
            for (int i = 0; i < 3; i++)
            {
                float pieceX = startX + i * (keyPieceSize + slotSpacing);
                Rect pieceRect = new Rect(pieceX, y, keyPieceSize, keyPieceSize);

                bool haspiece = PlayerInventory.Instance?.GetKeyPieceCount() > i;

                // Actually check if specific piece is collected
                // For now, approximate
                GUI.color = haspiece ? keyPieceActiveColor : keyPieceInactiveColor;
                GUI.DrawTexture(pieceRect, _whiteTexture);

                // Draw piece number
                GUI.color = haspiece ? Color.black : Color.gray;
                GUIStyle numStyle = new GUIStyle(_labelStyle);
                numStyle.fontSize = 12;
                numStyle.fontStyle = FontStyle.Bold;
                GUI.Label(pieceRect, (i + 1).ToString(), numStyle);
            }

            // Great key formed indicator
            if (PlayerInventory.Instance?.HasGreatKey == true)
            {
                GUI.color = keyPieceActiveColor;
                Rect formedRect = new Rect(container.x, y + keyPieceSize + 5, container.width, 15);
                GUI.Label(formedRect, "★ GREAT KEY FORMED ★", _labelStyle);
            }

            GUI.color = Color.white;
        }

        private void DrawControlsHint(Rect container)
        {
            if (_selectedSlot < 0) return;

            float y = container.y + container.height + 5;
            Rect hintRect = new Rect(container.x, y, container.width, 15);

            GUIStyle hintStyle = new GUIStyle(_labelStyle);
            hintStyle.fontSize = 9;

            GUI.color = new Color(1f, 1f, 1f, 0.7f);
            GUI.Label(hintRect, "[U] Use  [G] Drop", hintStyle);
            GUI.color = Color.white;
        }

        /// <summary>
        /// Toggles inventory visibility.
        /// </summary>
        public void ToggleInventory()
        {
            showInventory = !showInventory;
        }
    }
}
