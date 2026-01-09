using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Handles floor transitions via stairs and trapdoors.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FloorTransitionTrigger : MonoBehaviour
    {
        [Header("Transition Data")]
        [SerializeField] private FloorTransition transitionData;
        [SerializeField] private FloorTransitionType transitionType;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite stairsUpSprite;
        [SerializeField] private Sprite stairsDownSprite;
        [SerializeField] private Sprite trapdoorSprite;

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
            if (spriteRenderer == null) return;

            spriteRenderer.sprite = transitionType switch
            {
                FloorTransitionType.StairsUp => stairsUpSprite,
                FloorTransitionType.StairsDown => stairsDownSprite,
                FloorTransitionType.Trapdoor => trapdoorSprite,
                _ => null
            };

            // Color coding for debug
            spriteRenderer.color = transitionType switch
            {
                FloorTransitionType.StairsUp => new Color(0.5f, 1f, 0.5f),
                FloorTransitionType.StairsDown => new Color(0.5f, 0.5f, 1f),
                FloorTransitionType.Trapdoor => new Color(1f, 0.5f, 1f),
                _ => Color.white
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
            if (!other.CompareTag("Player")) return;

            _playerInTrigger = true;

            if (!requiresInteraction)
            {
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
            if (other.CompareTag("Player"))
            {
                _playerInTrigger = false;
            }
        }

        private void TriggerTransition()
        {
            if (RoomManager.Instance != null && !RoomManager.Instance.IsTransitioning)
            {
                RoomManager.Instance.TransitionThroughFloor(transitionType);
            }
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
