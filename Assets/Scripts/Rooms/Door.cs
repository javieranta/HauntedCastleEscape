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

            // Set tag (create if needed)
            try
            {
                gameObject.tag = "Door";
            }
            catch
            {
                // Tag might not exist, ignore
            }

            // Set layer if it exists
            int doorLayer = LayerMask.NameToLayer("Doors");
            if (doorLayer >= 0)
            {
                gameObject.layer = doorLayer;
            }
            else
            {
                // Fallback to Default layer
                gameObject.layer = 0;
            }
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

            // When entering a locked door, try to unlock it
            if (DoorType == DoorType.Locked && !isUnlocked)
            {
                if (!TryUnlockWithInventory())
                {
                    Debug.Log($"[Door] Need {RequiredKey} key to unlock");
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            // Check if player exited through the door (in the direction of the destination)
            Vector2 playerPos = other.transform.position;
            Vector2 doorPos = transform.position;

            // Much more forgiving alignment tolerance
            const float DOOR_ALIGNMENT_TOLERANCE = 2.5f;

            bool exitedThroughDoor = false;
            bool properlyAligned = false;

            switch (direction)
            {
                case DoorDirection.North:
                    exitedThroughDoor = playerPos.y > doorPos.y + 0.3f;
                    properlyAligned = Mathf.Abs(playerPos.x - doorPos.x) < DOOR_ALIGNMENT_TOLERANCE;
                    break;
                case DoorDirection.South:
                    exitedThroughDoor = playerPos.y < doorPos.y - 0.3f;
                    properlyAligned = Mathf.Abs(playerPos.x - doorPos.x) < DOOR_ALIGNMENT_TOLERANCE;
                    break;
                case DoorDirection.East:
                    exitedThroughDoor = playerPos.x > doorPos.x + 0.3f;
                    properlyAligned = Mathf.Abs(playerPos.y - doorPos.y) < DOOR_ALIGNMENT_TOLERANCE;
                    break;
                case DoorDirection.West:
                    exitedThroughDoor = playerPos.x < doorPos.x - 0.3f;
                    properlyAligned = Mathf.Abs(playerPos.y - doorPos.y) < DOOR_ALIGNMENT_TOLERANCE;
                    break;
            }

            Debug.Log($"[Door] OnTriggerExit2D - dir={direction}, exited={exitedThroughDoor}, aligned={properlyAligned}, canPass={CanPass()}, dest={DestinationRoomId}");

            // Only transition if player exited in the correct direction, is aligned, and can pass
            if (exitedThroughDoor && properlyAligned && CanPass())
            {
                if (RoomManager.Instance != null && !RoomManager.Instance.IsTransitioning)
                {
                    Debug.Log($"[Door] Transitioning through {direction} door to {DestinationRoomId}");
                    RoomManager.Instance.TransitionThroughDoor(direction);
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
