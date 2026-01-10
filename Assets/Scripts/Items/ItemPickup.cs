using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Player;
using HauntedCastle.Inventory;
using HauntedCastle.Utils;
using HauntedCastle.Visuals;
using HauntedCastle.Audio;

namespace HauntedCastle.Items
{
    /// <summary>
    /// World item that can be picked up by the player.
    /// Handles visual representation and pickup interaction.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class ItemPickup : MonoBehaviour
    {
        [Header("Item Data")]
        [SerializeField] private ItemData itemData;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float bobAmplitude = 0.1f;
        [SerializeField] private float bobFrequency = 2f;
        [SerializeField] private bool enableBobbing = true;

        [Header("Pickup Settings")]
        [SerializeField] private float pickupRadius = 0.5f;
        [SerializeField] private bool autoPickup = false;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        private Collider2D _collider;
        private Vector3 _startPosition;
        private bool _isPickedUp;

        // Properties
        public ItemData Data => itemData;
        public bool IsPickedUp => _isPickedUp;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }

            // Configure collider
            if (_collider is CircleCollider2D circleCol)
            {
                circleCol.radius = pickupRadius;
            }
            else if (_collider is BoxCollider2D boxCol)
            {
                boxCol.size = Vector2.one * pickupRadius * 2;
            }

            _startPosition = transform.position;
        }

        private void Start()
        {
            if (itemData != null)
            {
                UpdateVisual();
            }

            // Set layer if it exists
            int itemLayer = LayerMask.NameToLayer("Items");
            if (itemLayer >= 0)
            {
                gameObject.layer = itemLayer;
            }

            // Set tag (create if needed)
            try
            {
                gameObject.tag = "Item";
            }
            catch
            {
                // Tag might not exist, ignore
            }
        }

        private void Update()
        {
            if (enableBobbing && !_isPickedUp)
            {
                // Simple bobbing animation
                float yOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
                transform.position = _startPosition + Vector3.up * yOffset;
            }
        }

        /// <summary>
        /// Initializes the pickup with item data.
        /// </summary>
        public void Initialize(ItemData data)
        {
            itemData = data;
            autoPickup = data.autoPickup;
            UpdateVisual();
            _startPosition = transform.position;
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null || itemData == null) return;

            // Use world sprite or icon
            spriteRenderer.sprite = itemData.worldSprite ?? itemData.icon;
            spriteRenderer.color = itemData.tintColor;

            // If no sprite, use pixel art sprite provider for better visuals
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = PixelArtSpriteProvider.GetItemSprite(
                    itemData.itemType,
                    itemData.keyColor,
                    itemData.subType
                );
            }

            // Configure rendering
            spriteRenderer.sortingLayerName = "Items";
            spriteRenderer.sortingOrder = 0;
        }

        /// <summary>
        /// Attempts to pick up the item.
        /// </summary>
        public bool TryPickup(PlayerController player)
        {
            if (_isPickedUp || itemData == null) return false;

            // Try to add to inventory
            if (PlayerInventory.Instance != null)
            {
                bool added = PlayerInventory.Instance.TryAddItem(itemData);

                if (added)
                {
                    OnPickedUp();
                    return true;
                }
                else
                {
                    // Inventory full or item rejected
                    Debug.Log($"[ItemPickup] Cannot pick up {itemData.displayName}");
                    return false;
                }
            }

            return false;
        }

        private void OnPickedUp()
        {
            _isPickedUp = true;

            // Play pickup sound - use AudioManager with procedural fallback
            SoundEffect pickupSound = itemData.itemType switch
            {
                ItemType.Key => SoundEffect.KeyUnlock,
                ItemType.KeyPiece => SoundEffect.KeyPiece,
                ItemType.Food => SoundEffect.Pickup,
                ItemType.Treasure => SoundEffect.Pickup,
                ItemType.Special => SoundEffect.Pickup,
                ItemType.GreatKey => SoundEffect.GreatKey,
                _ => SoundEffect.Pickup
            };
            AudioManager.Instance?.PlaySFX(pickupSound);

            // Legacy audio source support
            if (audioSource != null && itemData.pickupSound != null)
            {
                audioSource.PlayOneShot(itemData.pickupSound);
            }

            // Visual effects - text popup and particles
            if (Effects.VisualEffectsManager.Instance != null)
            {
                string pickupText = itemData.itemType switch
                {
                    ItemType.Key => $"+{itemData.displayName}",
                    ItemType.KeyPiece => "KEY PIECE!",
                    ItemType.Food => $"+{itemData.energyRestore} Energy",
                    ItemType.Treasure => $"+{itemData.scoreValue} Points",
                    _ => itemData.displayName
                };

                Color pickupColor = itemData.itemType switch
                {
                    ItemType.Key => GetKeyTypeColor(itemData.keyColor),
                    ItemType.KeyPiece => Color.yellow,
                    ItemType.Food => Color.green,
                    ItemType.Treasure => new Color(1f, 0.8f, 0.2f),
                    _ => Color.white
                };

                Effects.VisualEffectsManager.Instance.ShowTextPopup(transform.position, pickupText, pickupColor);
                Effects.VisualEffectsManager.Instance.SpawnParticleBurst(transform.position, pickupColor, 8);
            }

            // Visual feedback
            StartCoroutine(PickupAnimation());
        }

        private Color GetKeyTypeColor(KeyColor keyColor)
        {
            return keyColor switch
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

        private System.Collections.IEnumerator PickupAnimation()
        {
            float duration = 0.2f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Scale up and fade out
                transform.localScale = startScale * (1f + t * 0.5f);

                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = 1f - t;
                    spriteRenderer.color = c;
                }

                yield return null;
            }

            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (_isPickedUp) return;

            // Auto-pickup items
            if (autoPickup || (itemData != null && itemData.autoPickup))
            {
                var player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    TryPickup(player);
                }
            }
        }

        private void OnDrawGizmos()
        {
            // Draw pickup radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickupRadius);
        }
    }
}
