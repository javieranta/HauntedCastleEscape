using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Player;

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
            // Use a valid layer index - default to TRIGGERS_LAYER (14) if Hazards layer doesn't exist
            int hazardsLayer = LayerMask.NameToLayer("Hazards");
            if (hazardsLayer >= 0 && hazardsLayer <= 31)
            {
                gameObject.layer = hazardsLayer;
            }
            else
            {
                // Fallback to a standard layer (Default = 0, or use Triggers layer index 14)
                gameObject.layer = 0; // Default layer
                Debug.LogWarning($"[Hazard] 'Hazards' layer not found, using Default layer. Add 'Hazards' layer in Unity's Layer settings.");
            }
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
            // Use string comparison instead of CompareTag (CompareTag fails silently if tag doesn't exist)
            if (other.tag == "Player" || other.gameObject.name.ToLower().Contains("player"))
            {
                _playerInHazard = true;
                Debug.Log($"[Hazard] Player entered {hazardType} hazard");
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Use string comparison instead of CompareTag
            if (other.tag == "Player" || other.gameObject.name.ToLower().Contains("player"))
            {
                _playerInHazard = false;
            }
        }

        private void ApplyDamageToPlayer()
        {
            float damage = damagePerSecond * Time.deltaTime;

            // Get player health and apply damage
            var playerHealth = Player.PlayerController.Instance?.GetComponent<Player.PlayerHealth>();
            if (playerHealth != null && !playerHealth.IsInvulnerable)
            {
                playerHealth.DrainEnergy(damage);
            }
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
