using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;
using HauntedCastle.Utils;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Handles floor transitions via stairs and trapdoors.
    /// Uses enhanced sprites for visual representation.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FloorTransitionTrigger : MonoBehaviour
    {
        [Header("Transition Data")]
        [SerializeField] private FloorTransition transitionData;
        [SerializeField] private FloorTransitionType transitionType;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer arrowIndicator;

        [Header("Interaction")]
        [SerializeField] private bool requiresInteraction = false;
        [SerializeField] private KeyCode interactionKey = KeyCode.E;

        // Properties
        public FloorTransitionType TransitionType => transitionType;
        public string DestinationRoomId => transitionData?.destinationRoomId ?? "";

        private Collider2D _collider;
        private bool _playerInTrigger = false;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Initialize(FloorTransition data, FloorTransitionType type)
        {
            transitionData = data;
            transitionType = type;

            UpdateVisual();

            // Set appropriate tag
            gameObject.tag = type == FloorTransitionType.Trapdoor ? "Hazard" : "Untagged";
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }

            // Use enhanced sprites from PlaceholderSpriteGenerator
            spriteRenderer.sprite = transitionType switch
            {
                FloorTransitionType.StairsUp => PlaceholderSpriteGenerator.GetStaircaseSprite(true),
                FloorTransitionType.StairsDown => PlaceholderSpriteGenerator.GetStaircaseSprite(false),
                FloorTransitionType.Trapdoor => PlaceholderSpriteGenerator.GetDoorSprite(false, ""),
                _ => null
            };

            // Tint based on transition type
            spriteRenderer.color = transitionType switch
            {
                FloorTransitionType.StairsUp => new Color(0.9f, 0.95f, 1f),
                FloorTransitionType.StairsDown => new Color(0.8f, 0.75f, 0.7f),
                FloorTransitionType.Trapdoor => new Color(0.6f, 0.5f, 0.4f),
                _ => Color.white
            };

            spriteRenderer.sortingLayerName = "Items";
            spriteRenderer.sortingOrder = -1;

            // Create direction indicator arrow
            CreateArrowIndicator();
        }

        private void CreateArrowIndicator()
        {
            if (arrowIndicator != null) return;

            var arrowObj = new GameObject("ArrowIndicator");
            arrowObj.transform.SetParent(transform);
            arrowObj.transform.localPosition = transitionType == FloorTransitionType.StairsUp
                ? new Vector3(0, 1.2f, 0)
                : new Vector3(0, -1.2f, 0);

            arrowIndicator = arrowObj.AddComponent<SpriteRenderer>();

            // Create LARGE, clearly visible arrow texture (64x64 for visibility)
            int size = 64;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Bilinear;

            // Bright, vibrant colors for maximum visibility
            Color arrowColor = transitionType == FloorTransitionType.StairsUp
                ? new Color(0.2f, 1f, 0.2f, 1f)    // Bright green for UP
                : new Color(1f, 0.4f, 0.2f, 1f);   // Bright orange for DOWN

            Color glowColor = transitionType == FloorTransitionType.StairsUp
                ? new Color(0.4f, 1f, 0.4f, 0.5f)  // Green glow
                : new Color(1f, 0.6f, 0.4f, 0.5f); // Orange glow

            // Clear texture
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    tex.SetPixel(x, y, Color.clear);

            // Draw LARGE arrow with glow effect
            bool pointUp = transitionType == FloorTransitionType.StairsUp;
            int center = size / 2;

            // Draw glow first (larger arrow behind)
            DrawLargeArrow(tex, size, center, pointUp, glowColor, 4);

            // Draw main arrow
            DrawLargeArrow(tex, size, center, pointUp, arrowColor, 0);

            tex.Apply();
            arrowIndicator.sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            arrowIndicator.sortingLayerName = "UI";
            arrowIndicator.sortingOrder = 100;
            arrowIndicator.transform.localScale = Vector3.one * 1.5f;
            arrowIndicator.enabled = false; // Hidden until player approaches

            // Create text label for even more clarity
            CreateTextLabel();
        }

        private void DrawLargeArrow(Texture2D tex, int size, int center, bool pointUp, Color color, int expand)
        {
            // Arrow head (large triangle)
            int headHeight = size / 2;
            for (int row = 0; row < headHeight; row++)
            {
                int width = (row + 1) * 2 + expand * 2;
                int startX = center - width / 2;
                int y = pointUp ? (size - 1 - row) : row;

                for (int x = startX; x < startX + width && x < size; x++)
                {
                    if (x >= 0)
                        tex.SetPixel(x, y, color);
                }
            }

            // Arrow shaft (thick line)
            int shaftWidth = size / 4 + expand;
            int shaftHeight = size / 2;
            int shaftStartX = center - shaftWidth / 2;

            for (int y = 0; y < shaftHeight; y++)
            {
                int actualY = pointUp ? y : (size - 1 - y);
                for (int x = shaftStartX; x < shaftStartX + shaftWidth && x < size; x++)
                {
                    if (x >= 0)
                        tex.SetPixel(x, actualY, color);
                }
            }
        }

        private void CreateTextLabel()
        {
            // Create a text indicator using a texture with "UP" or "DOWN" text
            var labelObj = new GameObject("StairLabel");
            labelObj.transform.SetParent(transform);
            labelObj.transform.localPosition = transitionType == FloorTransitionType.StairsUp
                ? new Vector3(0, -0.8f, 0)
                : new Vector3(0, 0.8f, 0);

            var labelRenderer = labelObj.AddComponent<SpriteRenderer>();

            // Create text texture (48x16 for UP or 64x16 for DOWN)
            string text = transitionType == FloorTransitionType.StairsUp ? "UP" : "DOWN";
            int width = text.Length * 16;
            int height = 24;
            var tex = new Texture2D(width, height);
            tex.filterMode = FilterMode.Bilinear;

            Color textColor = transitionType == FloorTransitionType.StairsUp
                ? new Color(0.2f, 1f, 0.2f, 1f)
                : new Color(1f, 0.4f, 0.2f, 1f);
            Color bgColor = new Color(0, 0, 0, 0.7f);

            // Fill background
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    tex.SetPixel(x, y, bgColor);

            // Draw simple pixel letters
            DrawPixelText(tex, text, 2, 4, textColor);

            tex.Apply();
            labelRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 16);
            labelRenderer.sortingLayerName = "UI";
            labelRenderer.sortingOrder = 101;
        }

        private void DrawPixelText(Texture2D tex, string text, int startX, int startY, Color color)
        {
            int charWidth = 12;
            int charHeight = 16;

            for (int i = 0; i < text.Length; i++)
            {
                int x = startX + i * charWidth;
                DrawPixelChar(tex, text[i], x, startY, charWidth, charHeight, color);
            }
        }

        private void DrawPixelChar(Texture2D tex, char c, int startX, int startY, int width, int height, Color color)
        {
            // Simple 5x7 pixel font patterns (scaled up)
            bool[,] pattern = GetCharPattern(c);

            for (int py = 0; py < 7; py++)
            {
                for (int px = 0; px < 5; px++)
                {
                    if (pattern[py, px])
                    {
                        // Scale up 2x
                        for (int sy = 0; sy < 2; sy++)
                        {
                            for (int sx = 0; sx < 2; sx++)
                            {
                                int tx = startX + px * 2 + sx;
                                int ty = startY + (6 - py) * 2 + sy;
                                if (tx >= 0 && tx < tex.width && ty >= 0 && ty < tex.height)
                                    tex.SetPixel(tx, ty, color);
                            }
                        }
                    }
                }
            }
        }

        private bool[,] GetCharPattern(char c)
        {
            return c switch
            {
                'U' => new bool[,] {
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {false, true, true, true, false}
                },
                'P' => new bool[,] {
                    {true, true, true, true, false},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, true, true, true, false},
                    {true, false, false, false, false},
                    {true, false, false, false, false},
                    {true, false, false, false, false}
                },
                'D' => new bool[,] {
                    {true, true, true, true, false},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, true, true, true, false}
                },
                'O' => new bool[,] {
                    {false, true, true, true, false},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {false, true, true, true, false}
                },
                'W' => new bool[,] {
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true},
                    {true, false, true, false, true},
                    {true, false, true, false, true},
                    {true, true, false, true, true},
                    {true, false, false, false, true}
                },
                'N' => new bool[,] {
                    {true, false, false, false, true},
                    {true, true, false, false, true},
                    {true, false, true, false, true},
                    {true, false, true, false, true},
                    {true, false, false, true, true},
                    {true, false, false, false, true},
                    {true, false, false, false, true}
                },
                _ => new bool[5, 5]
            };
        }

        private void Update()
        {
            if (_playerInTrigger && requiresInteraction)
            {
                if (Input.GetKeyDown(interactionKey))
                {
                    TriggerTransition();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // DEBUG: Log EVERY collision with this trigger
            Debug.LogWarning($"[STAIRS DEBUG] OnTriggerEnter2D called! Collider: {other.gameObject.name}, Tag: '{other.tag}'");

            // Use string comparison instead of CompareTag (CompareTag fails silently if tag doesn't exist)
            bool isPlayer = other.tag == "Player" || other.gameObject.name.ToLower().Contains("player");

            Debug.LogWarning($"[STAIRS DEBUG] Is this the player? {isPlayer}");

            if (!isPlayer)
            {
                Debug.LogWarning($"[STAIRS DEBUG] Not the player - ignoring collision");
                return;
            }

            Debug.LogWarning($"[STAIRS DEBUG] *** PLAYER ENTERED {transitionType} TRIGGER! ***");
            Debug.LogWarning($"[STAIRS DEBUG] Destination: {transitionData?.destinationRoomId ?? "NULL"}");
            Debug.LogWarning($"[STAIRS DEBUG] RequiresInteraction: {requiresInteraction}");

            _playerInTrigger = true;

            // Show arrow indicator
            if (arrowIndicator != null)
            {
                arrowIndicator.enabled = true;
            }

            if (!requiresInteraction)
            {
                Debug.LogWarning($"[STAIRS DEBUG] Auto-triggering transition (no interaction required)...");
                TriggerTransition();
            }
            else
            {
                // Show interaction prompt (will be implemented with UI)
                Debug.Log($"[FloorTransition] Press {interactionKey} to use {transitionType}");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Use string comparison instead of CompareTag
            bool isPlayer = other.tag == "Player" || other.gameObject.name.ToLower().Contains("player");
            if (isPlayer)
            {
                Debug.LogWarning($"[STAIRS DEBUG] Player exited {transitionType} trigger");
                _playerInTrigger = false;

                // Hide arrow indicator
                if (arrowIndicator != null)
                {
                    arrowIndicator.enabled = false;
                }
            }
        }

        private void TriggerTransition()
        {
            Debug.LogWarning($"[STAIRS DEBUG] TriggerTransition called for {transitionType}");
            Debug.LogWarning($"[STAIRS DEBUG] RoomManager.Instance: {(RoomManager.Instance != null ? "EXISTS" : "NULL")}");

            if (RoomManager.Instance == null)
            {
                Debug.LogError($"[STAIRS DEBUG] FAILED: RoomManager.Instance is NULL!");
                return;
            }

            Debug.LogWarning($"[STAIRS DEBUG] RoomManager.IsTransitioning: {RoomManager.Instance.IsTransitioning}");

            if (RoomManager.Instance.IsTransitioning)
            {
                Debug.LogWarning($"[STAIRS DEBUG] BLOCKED: Already transitioning, ignoring");
                return;
            }

            Debug.LogWarning($"[STAIRS DEBUG] *** CALLING RoomManager.TransitionThroughFloor({transitionType}) ***");
            RoomManager.Instance.TransitionThroughFloor(transitionType);
            Debug.LogWarning($"[STAIRS DEBUG] TransitionThroughFloor call completed");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = transitionType switch
            {
                FloorTransitionType.StairsUp => Color.green,
                FloorTransitionType.StairsDown => Color.blue,
                FloorTransitionType.Trapdoor => Color.magenta,
                _ => Color.white
            };

            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);

            // Draw direction arrow
            Vector3 dir = transitionType switch
            {
                FloorTransitionType.StairsUp => Vector3.up,
                FloorTransitionType.StairsDown => Vector3.down,
                FloorTransitionType.Trapdoor => Vector3.down,
                _ => Vector3.zero
            };

            Gizmos.DrawLine(transform.position, transform.position + dir * 0.5f);
        }
    }
}
