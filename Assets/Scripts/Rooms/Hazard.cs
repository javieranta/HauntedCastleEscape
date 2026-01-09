using UnityEngine;
using HauntedCastle.Data;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Stationary hazard that damages the player on contact.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hazard : MonoBehaviour
    {
        [Header("Hazard Data")]
        [SerializeField] private HazardType hazardType;
        [SerializeField] private float damagePerSecond = 10f;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] animationSprites;
        [SerializeField] private float animationSpeed = 0.1f;

        [Header("Audio")]
        [SerializeField] private AudioClip hazardSound;
        [SerializeField] private bool loopSound = true;

        // Properties
        public HazardType Type => hazardType;
        public float DamagePerSecond => damagePerSecond;

        private Collider2D _collider;
        private int _currentFrame = 0;
        private float _animationTimer = 0f;
        private bool _playerInHazard = false;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        public void Initialize(HazardSpawn spawnData)
        {
            hazardType = spawnData.hazardType;
            damagePerSecond = spawnData.damagePerSecond;

            // Set collider size
            if (_collider is BoxCollider2D boxCollider)
            {
                boxCollider.size = spawnData.size;
            }

            UpdateVisual();

            gameObject.tag = "Hazard";
            gameObject.layer = LayerMask.NameToLayer("Hazards");
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null) return;

            spriteRenderer.color = GetHazardColor();
            spriteRenderer.sortingLayerName = "Items"; // Below player
        }

        private Color GetHazardColor()
        {
            return hazardType switch
            {
                HazardType.Spikes => new Color(0.5f, 0.5f, 0.5f),
                HazardType.Fire => new Color(1f, 0.5f, 0f),
                HazardType.Poison => new Color(0.2f, 0.8f, 0.2f),
                HazardType.Acid => new Color(0.8f, 1f, 0f),
                HazardType.Electricity => new Color(0.5f, 0.8f, 1f),
                _ => Color.red
            };
        }

        private void Update()
        {
            // Animate hazard
            if (animationSprites != null && animationSprites.Length > 1)
            {
                _animationTimer += Time.deltaTime;
                if (_animationTimer >= animationSpeed)
                {
                    _animationTimer = 0f;
                    _currentFrame = (_currentFrame + 1) % animationSprites.Length;
                    spriteRenderer.sprite = animationSprites[_currentFrame];
                }
            }

            // Apply damage to player while in hazard
            if (_playerInHazard)
            {
                ApplyDamageToPlayer();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInHazard = true;
                Debug.Log($"[Hazard] Player entered {hazardType} hazard");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInHazard = false;
            }
        }

        private void ApplyDamageToPlayer()
        {
            // This will be properly connected to the player health system in Milestone 2
            // For now, just log
            float damage = damagePerSecond * Time.deltaTime;

            // TODO: Get player reference and apply damage
            // PlayerController.Instance?.TakeDamage(damage);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = GetHazardColor();

            if (_collider is BoxCollider2D boxCollider)
            {
                Gizmos.DrawWireCube(transform.position, boxCollider.size);
            }
            else
            {
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}
