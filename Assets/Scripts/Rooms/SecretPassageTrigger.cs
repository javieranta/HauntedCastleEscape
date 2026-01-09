using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Handles secret passage interactions.
    /// Only certain character types can use specific passage types.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SecretPassageTrigger : MonoBehaviour
    {
        [Header("Passage Data")]
        [SerializeField] private SecretPassage passageData;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private SpriteRenderer highlightRenderer;

        [Header("Interaction")]
        [SerializeField] private KeyCode interactionKey = KeyCode.E;
        [SerializeField] private bool showHighlightOnApproach = true;

        // Properties
        public SecretPassageType PassageType => passageData?.passageType ?? SecretPassageType.None;
        public string DestinationRoomId => passageData?.destinationRoomId ?? "";

        private Collider2D _collider;
        private bool _playerInTrigger = false;
        private bool _canPlayerUse = false;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            SetupHighlight();
        }

        public void Initialize(SecretPassage data)
        {
            passageData = data;

            if (spriteRenderer != null && data.disguiseSprite != null)
            {
                spriteRenderer.sprite = data.disguiseSprite;
            }

            UpdateVisual();

            gameObject.tag = "SecretPassage";
        }

        private void SetupHighlight()
        {
            if (highlightRenderer != null) return;

            var highlightObj = new GameObject("Highlight");
            highlightObj.transform.SetParent(transform);
            highlightObj.transform.localPosition = Vector3.zero;

            highlightRenderer = highlightObj.AddComponent<SpriteRenderer>();
            highlightRenderer.sortingOrder = spriteRenderer != null ? spriteRenderer.sortingOrder + 1 : 1;
            highlightRenderer.color = new Color(1f, 1f, 0f, 0f); // Transparent yellow
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null) return;

            // Secret passages look like normal objects (bookcase, clock, barrel)
            spriteRenderer.color = GetPassageColor(PassageType);
        }

        private Color GetPassageColor(SecretPassageType type)
        {
            return type switch
            {
                SecretPassageType.Bookcase => new Color(0.4f, 0.25f, 0.1f),
                SecretPassageType.Clock => new Color(0.7f, 0.6f, 0.3f),
                SecretPassageType.Barrel => new Color(0.5f, 0.35f, 0.15f),
                _ => Color.gray
            };
        }

        private void Update()
        {
            if (_playerInTrigger && _canPlayerUse)
            {
                if (Input.GetKeyDown(interactionKey))
                {
                    UsePassage();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            _playerInTrigger = true;

            // Check if player can use this passage type
            _canPlayerUse = CheckPlayerCanUse();

            if (_canPlayerUse && showHighlightOnApproach)
            {
                ShowHighlight();
            }

            if (_canPlayerUse)
            {
                Debug.Log($"[SecretPassage] Press {interactionKey} to use {PassageType}");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInTrigger = false;
                _canPlayerUse = false;
                HideHighlight();
            }
        }

        private bool CheckPlayerCanUse()
        {
            // Get player's character type from GameManager
            if (GameManager.Instance == null) return false;

            CharacterType playerCharacter = GameManager.Instance.SelectedCharacter;

            // Check if character can access this passage type
            return PassageType switch
            {
                SecretPassageType.Bookcase => playerCharacter == CharacterType.Wizard,
                SecretPassageType.Clock => playerCharacter == CharacterType.Knight,
                SecretPassageType.Barrel => playerCharacter == CharacterType.Serf,
                _ => false
            };
        }

        private void UsePassage()
        {
            if (RoomManager.Instance != null && !RoomManager.Instance.IsTransitioning)
            {
                Debug.Log($"[SecretPassage] Using {PassageType} passage to {DestinationRoomId}");
                RoomManager.Instance.TransitionThroughSecretPassage(PassageType, DestinationRoomId);
            }
        }

        private void ShowHighlight()
        {
            if (highlightRenderer != null)
            {
                highlightRenderer.color = new Color(1f, 1f, 0f, 0.3f);
            }
        }

        private void HideHighlight()
        {
            if (highlightRenderer != null)
            {
                highlightRenderer.color = new Color(1f, 1f, 0f, 0f);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = PassageType switch
            {
                SecretPassageType.Bookcase => new Color(0.5f, 0f, 1f, 0.5f), // Purple for Wizard
                SecretPassageType.Clock => new Color(1f, 0.8f, 0f, 0.5f),    // Gold for Knight
                SecretPassageType.Barrel => new Color(0.6f, 0.4f, 0.2f, 0.5f), // Brown for Serf
                _ => Color.gray
            };

            Gizmos.DrawCube(transform.position, new Vector3(1f, 1.5f, 0.1f));
        }
    }
}
