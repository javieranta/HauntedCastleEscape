using System.Collections.Generic;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Door component that handles player interaction and room transitions.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Door : MonoBehaviour
    {
        [Header("Door Data")]
        [SerializeField] private DoorConnection doorData;
        [SerializeField] private DoorDirection direction;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite openSprite;
        [SerializeField] private Sprite lockedSprite;
        [SerializeField] private Sprite hiddenSprite;

        [Header("State")]
        [SerializeField] private bool isUnlocked = false;

        // Properties
        public DoorDirection Direction => direction;
        public DoorType DoorType => doorData?.doorType ?? DoorType.Open;
        public KeyColor RequiredKey => doorData?.requiredKeyColor ?? KeyColor.None;
        public string DestinationRoomId => doorData?.destinationRoomId ?? "";
        public bool IsUnlocked => isUnlocked || DoorType == DoorType.Open;

        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Initialize(DoorConnection data, DoorDirection dir)
        {
            doorData = data;
            direction = dir;

            // Set initial state
            isUnlocked = data.doorType == DoorType.Open;

            // Update visual
            UpdateVisual();

            // Set tag
            gameObject.tag = "Door";
            gameObject.layer = LayerMask.NameToLayer("Doors");
        }

        /// <summary>
        /// Checks if the player can pass through this door.
        /// </summary>
        public bool CanPass(List<string> playerInventory)
        {
            if (isUnlocked || DoorType == DoorType.Open)
            {
                return true;
            }

            if (DoorType == DoorType.Locked && RequiredKey != KeyColor.None)
            {
                // Check if player has the required key
                string keyId = $"key_{RequiredKey.ToString().ToLower()}";
                return playerInventory != null && playerInventory.Contains(keyId);
            }

            if (DoorType == DoorType.Hidden)
            {
                // Hidden doors need to be revealed first
                return isUnlocked;
            }

            return false;
        }

        /// <summary>
        /// Attempts to unlock the door with a key.
        /// </summary>
        public bool TryUnlock(KeyColor keyColor)
        {
            if (DoorType != DoorType.Locked)
            {
                return false;
            }

            if (keyColor == RequiredKey)
            {
                isUnlocked = true;
                UpdateVisual();

                // Play unlock sound
                Debug.Log($"[Door] Unlocked {direction} door with {keyColor} key");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reveals a hidden door.
        /// </summary>
        public void Reveal()
        {
            if (DoorType == DoorType.Hidden && !isUnlocked)
            {
                isUnlocked = true;
                UpdateVisual();
                Debug.Log($"[Door] Revealed hidden {direction} door");
            }
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null) return;

            if (DoorType == DoorType.Hidden && !isUnlocked)
            {
                spriteRenderer.sprite = hiddenSprite;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.3f); // Semi-transparent
            }
            else if (DoorType == DoorType.Locked && !isUnlocked)
            {
                spriteRenderer.sprite = lockedSprite;
                spriteRenderer.color = GetKeyColor(RequiredKey);
            }
            else
            {
                spriteRenderer.sprite = openSprite;
                spriteRenderer.color = Color.white;
            }
        }

        private Color GetKeyColor(KeyColor keyColor)
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            // Get player inventory (will be properly implemented in Milestone 3)
            var playerInventory = new List<string>(); // Placeholder

            if (CanPass(playerInventory))
            {
                // Trigger room transition
                if (RoomManager.Instance != null)
                {
                    RoomManager.Instance.TransitionThroughDoor(direction);
                }
            }
            else if (DoorType == DoorType.Locked)
            {
                // Try to auto-unlock with player's key
                // This will be properly implemented with inventory system
                Debug.Log($"[Door] Need {RequiredKey} key to unlock");
            }
        }

        private void OnDrawGizmos()
        {
            // Draw door direction indicator in editor
            Gizmos.color = IsUnlocked ? Color.green : Color.red;

            Vector3 arrowDir = direction switch
            {
                DoorDirection.North => Vector3.up,
                DoorDirection.South => Vector3.down,
                DoorDirection.East => Vector3.right,
                DoorDirection.West => Vector3.left,
                _ => Vector3.zero
            };

            Gizmos.DrawLine(transform.position, transform.position + arrowDir * 0.5f);
        }
    }
}
