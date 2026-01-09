using UnityEngine;
using HauntedCastle.Inventory;
using HauntedCastle.Data;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Monitors win conditions and triggers victory when player escapes with Great Key.
    /// Attaches to exit doors to check for victory conditions.
    /// </summary>
    public class WinConditionChecker : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool requireGreatKey = true;
        [SerializeField] private bool isExitDoor = false;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer exitIndicator;
        [SerializeField] private Color lockedColor = new Color(0.5f, 0.2f, 0.2f);
        [SerializeField] private Color unlockedColor = new Color(0.2f, 0.8f, 0.2f);

        private bool _playerInTrigger = false;
        private Collider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            if (_collider != null)
            {
                _collider.isTrigger = true;
            }
        }

        private void Start()
        {
            UpdateVisual();

            // Subscribe to Great Key formation event
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.OnGreatKeyFormed += OnGreatKeyFormed;
            }
        }

        private void OnDestroy()
        {
            if (PlayerInventory.Instance != null)
            {
                PlayerInventory.Instance.OnGreatKeyFormed -= OnGreatKeyFormed;
            }
        }

        private void OnGreatKeyFormed()
        {
            UpdateVisual();
            Debug.Log("[WinConditionChecker] Great Key formed! Exit is now accessible.");
        }

        private void UpdateVisual()
        {
            if (exitIndicator == null) return;

            bool canExit = !requireGreatKey || (PlayerInventory.Instance?.HasGreatKey ?? false);
            exitIndicator.color = canExit ? unlockedColor : lockedColor;
        }

        private void Update()
        {
            if (_playerInTrigger && Input.GetKeyDown(KeyCode.E))
            {
                TryExit();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInTrigger = true;

                if (CanExit())
                {
                    Debug.Log("[WinConditionChecker] Press E to escape the castle!");
                }
                else
                {
                    Debug.Log("[WinConditionChecker] You need the Great Key to escape!");
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInTrigger = false;
            }
        }

        private bool CanExit()
        {
            if (!requireGreatKey) return true;

            return PlayerInventory.Instance != null && PlayerInventory.Instance.HasGreatKey;
        }

        private void TryExit()
        {
            if (!isExitDoor) return;

            if (CanExit())
            {
                TriggerVictory();
            }
            else
            {
                // Show message that Great Key is needed
                Debug.Log("[WinConditionChecker] Cannot exit without the Great Key!");

                // Could trigger UI notification here
            }
        }

        private void TriggerVictory()
        {
            Debug.Log("[WinConditionChecker] VICTORY! Player escaped the castle!");

            // Stop player movement
            if (Player.PlayerController.Instance != null)
            {
                Player.PlayerController.Instance.CanMove = false;
            }

            // Trigger victory in GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerVictory();
            }
        }

        /// <summary>
        /// Sets whether this is an exit door.
        /// </summary>
        public void SetAsExitDoor(bool isExit)
        {
            isExitDoor = isExit;
        }
    }
}
