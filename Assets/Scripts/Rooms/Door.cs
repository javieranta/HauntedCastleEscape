using System.Collections.Generic;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;
using HauntedCastle.Inventory;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Door component that handles player interaction and room transitions.
    /// Integrates with PlayerInventory for key-based unlocking.
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

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip unlockSound;
        [SerializeField] private AudioClip lockedSound;

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
        /// Checks if the player can pass through this door using PlayerInventory.
        /// </summary>
        public bool CanPass()
        {
            if (isUnlocked || DoorType == DoorType.Open)
            {
                return true;
            }

            if (DoorType == DoorType.Locked && RequiredKey != KeyColor.None)
            {
                // Check PlayerInventory for the required key
                if (PlayerInventory.Instance != null)
                {
                    return PlayerInventory.Instance.HasKey(RequiredKey);
                }
            }

            if (DoorType == DoorType.Hidden)
            {
                return isUnlocked;
            }

            return false;
        }

        /// <summary>
        /// Legacy method for compatibility - checks if the player can pass through this door.
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
                return isUnlocked;
            }

            return false;
        }

        /// <summary>
        /// Attempts to unlock the door using the player's inventory.
        /// Returns true if the door was unlocked.
        /// </summary>
        public bool TryUnlockWithInventory()
        {
            if (isUnlocked || DoorType != DoorType.Locked)
            {
                return isUnlocked;
            }

            if (PlayerInventory.Instance != null && RequiredKey != KeyColor.None)
            {
                if (PlayerInventory.Instance.TryUseKey(RequiredKey))
                {
                    isUnlocked = true;
                    UpdateVisual();
                    PlayUnlockSound();
                    Debug.Log($"[Door] Unlocked {direction} door with {RequiredKey} key");
                    return true;
                }
            }

            PlayLockedSound();
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
                PlayUnlockSound();
                Debug.Log($"[Door] Unlocked {direction} door with {keyColor} key");
                return true;
            }

            return false;
        }

        private void PlayUnlockSound()
        {
            if (audioSource != null && unlockSound != null)
            {
                audioSource.PlayOneShot(unlockSound);
            }
        }

        private void PlayLockedSound()
        {
            if (audioSource != null && lockedSound != null)
            {
                audioSource.PlayOneShot(lockedSound);
            }
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

            // Check if player can pass using inventory
            if (CanPass())
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
                if (!TryUnlockWithInventory())
                {
                    Debug.Log($"[Door] Need {RequiredKey} key to unlock");
                }
                else
                {
                    // Door was unlocked, now pass through
                    if (RoomManager.Instance != null)
                    {
                        RoomManager.Instance.TransitionThroughDoor(direction);
                    }
                }
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
