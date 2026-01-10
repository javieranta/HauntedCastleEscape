using UnityEngine;
using HauntedCastle.Player;

namespace HauntedCastle.Enemies
{
    /// <summary>
    /// Projectile fired by ranged enemies (Witch, Demon, etc.)
    /// Handles movement, collision, and visual effects.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class EnemyProjectile : MonoBehaviour
    {
        [Header("Projectile Settings")]
        [SerializeField] private float speed = 8f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float lifetime = 5f;
        [SerializeField] private bool destroyOnWall = true;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private Color projectileColor = Color.red;
        [SerializeField] private ProjectileType projectileType = ProjectileType.Fireball;

        private Rigidbody2D _rb;
        private Collider2D _col;
        private Vector2 _direction;
        private float _timer;
        private bool _destroyed;

        // CRITICAL: Cache projectile sprite to avoid creating new textures for each projectile
        private static Sprite _cachedProjectileSprite;

        public enum ProjectileType
        {
            Fireball,       // Demon/Witch fire attack
            ShadowBolt,     // Ghost dark magic
            BoneSpear,      // Skeleton ranged attack
            WebShot,        // Spider web (slows player)
            SonicWave       // Bat screech attack
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<Collider2D>();

            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _col.isTrigger = true;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer == null)
                {
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                }
            }

            CreateVisual();
        }

        /// <summary>
        /// Initializes and fires the projectile.
        /// </summary>
        public void Fire(Vector2 direction, int damageAmount, ProjectileType type, Color color)
        {
            _direction = direction.normalized;

            // CRITICAL: Validate direction to prevent NaN/Infinity
            if (float.IsNaN(_direction.x) || float.IsNaN(_direction.y) ||
                float.IsInfinity(_direction.x) || float.IsInfinity(_direction.y) ||
                _direction.sqrMagnitude < 0.01f)
            {
                Debug.LogWarning("[EnemyProjectile] Invalid direction, using default");
                _direction = Vector2.right;
            }

            damage = damageAmount;
            projectileType = type;
            projectileColor = color;

            UpdateVisualForType();

            // Set velocity with validation
            Vector2 newVelocity = _direction * speed;
            if (float.IsNaN(newVelocity.x) || float.IsNaN(newVelocity.y))
            {
                newVelocity = Vector2.right * speed;
            }
            _rb.velocity = newVelocity;

            // Rotate to face direction
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Set layer to avoid hitting other enemies - SAFELY
            int projLayer = LayerMask.NameToLayer("EnemyProjectiles");
            if (projLayer < 0 || projLayer > 31)
            {
                // Layer doesn't exist, use default layer
                projLayer = LayerMask.NameToLayer("Default");
                if (projLayer < 0) projLayer = 0;
            }
            gameObject.layer = projLayer;
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= lifetime)
            {
                DestroyProjectile();
            }

            // Add some visual effects based on type
            UpdateVisualEffects();
        }

        private void CreateVisual()
        {
            try
            {
                // CRITICAL FIX: Use cached sprite to avoid creating new textures for each projectile
                if (_cachedProjectileSprite == null)
                {
                    Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
                    Color[] pixels = new Color[16 * 16];

                    for (int y = 0; y < 16; y++)
                    {
                        for (int x = 0; x < 16; x++)
                        {
                            float dist = Vector2.Distance(new Vector2(x, y), new Vector2(7.5f, 7.5f));
                            if (dist < 6)
                            {
                                // Use white color - we'll tint with spriteRenderer.color
                                float alpha = 1f - (dist / 6f) * 0.5f;
                                pixels[y * 16 + x] = new Color(1f, 1f, 1f, alpha);
                            }
                            else
                            {
                                pixels[y * 16 + x] = Color.clear;
                            }
                        }
                    }

                    tex.SetPixels(pixels);
                    tex.Apply(false, false);
                    tex.filterMode = FilterMode.Bilinear;

                    _cachedProjectileSprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16f);
                }

                spriteRenderer.sprite = _cachedProjectileSprite;
                spriteRenderer.color = projectileColor; // Tint with the projectile's color
                spriteRenderer.sortingLayerName = "Projectiles";
                spriteRenderer.sortingOrder = 10;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[EnemyProjectile] CreateVisual failed: {e.Message}");
                spriteRenderer.color = projectileColor;
            }
        }

        private void UpdateVisualForType()
        {
            // Adjust speed and appearance based on type
            switch (projectileType)
            {
                case ProjectileType.Fireball:
                    speed = 8f;
                    transform.localScale = Vector3.one * 0.8f;
                    break;
                case ProjectileType.ShadowBolt:
                    speed = 10f;
                    transform.localScale = Vector3.one * 0.6f;
                    break;
                case ProjectileType.BoneSpear:
                    speed = 12f;
                    transform.localScale = new Vector3(1.2f, 0.4f, 1f);
                    break;
                case ProjectileType.WebShot:
                    speed = 6f;
                    transform.localScale = Vector3.one * 1f;
                    break;
                case ProjectileType.SonicWave:
                    speed = 15f;
                    transform.localScale = new Vector3(0.8f, 0.3f, 1f);
                    break;
            }

            spriteRenderer.color = projectileColor;
            _rb.velocity = _direction * speed;
        }

        private void UpdateVisualEffects()
        {
            // Pulsing effect for magical projectiles
            if (projectileType == ProjectileType.Fireball || projectileType == ProjectileType.ShadowBolt)
            {
                float pulse = 1f + Mathf.Sin(Time.time * 10f) * 0.2f;
                transform.localScale = Vector3.one * 0.8f * pulse;
            }

            // Rotation for bone spear
            if (projectileType == ProjectileType.BoneSpear)
            {
                transform.Rotate(0, 0, 360f * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_destroyed) return;

            // Hit player
            if (other.CompareTag("Player"))
            {
                var playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsInvulnerable)
                {
                    Vector2 knockback = _direction;
                    playerHealth.TakeDamage(damage, knockback);

                    // Special effects for web shot - would slow player
                    if (projectileType == ProjectileType.WebShot)
                    {
                        Debug.Log("[EnemyProjectile] Web shot hit - player slowed!");
                    }
                }
                DestroyProjectile();
            }
            // Hit wall - use safe layer checking
            else if (destroyOnWall)
            {
                // Check by tag instead of layer to avoid LayerMask.NameToLayer issues
                if (other.CompareTag("Wall") || other.CompareTag("Ground") || other.CompareTag("Obstacle"))
                {
                    DestroyProjectile();
                }
                // Also check by layer name safely
                else
                {
                    int wallLayer = LayerMask.NameToLayer("Walls");
                    int groundLayer = LayerMask.NameToLayer("Ground");
                    int otherLayer = other.gameObject.layer;

                    if ((wallLayer >= 0 && otherLayer == wallLayer) ||
                        (groundLayer >= 0 && otherLayer == groundLayer))
                    {
                        DestroyProjectile();
                    }
                }
            }
        }

        private void DestroyProjectile()
        {
            if (_destroyed) return;
            _destroyed = true;

            // DISABLED: Particle effects were causing freezes
            // if (Effects.VisualEffectsManager.Instance != null)
            // {
            //     Effects.VisualEffectsManager.Instance.SpawnParticleBurst(transform.position, projectileColor, 5);
            // }

            Destroy(gameObject);
        }

        // Static cached sprite for simple projectiles
        private static Sprite _simpleProjectileSprite;

        private static Sprite GetSimpleSprite()
        {
            if (_simpleProjectileSprite == null)
            {
                // Create a simple 8x8 white circle texture ONCE
                var tex = new Texture2D(8, 8, TextureFormat.RGBA32, false);
                var pixels = new Color[64];
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        float dist = Vector2.Distance(new Vector2(x, y), new Vector2(3.5f, 3.5f));
                        pixels[y * 8 + x] = dist < 3.5f ? Color.white : Color.clear;
                    }
                }
                tex.SetPixels(pixels);
                tex.Apply();
                tex.filterMode = FilterMode.Bilinear;
                _simpleProjectileSprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8f);
            }
            return _simpleProjectileSprite;
        }

        /// <summary>
        /// Creates a projectile dynamically.
        /// FIXED: Uses string == instead of CompareTag to avoid freeze.
        /// </summary>
        public static EnemyProjectile SpawnProjectile(Vector3 position, Vector2 direction, int damage, ProjectileType type, Color color)
        {
            var projObj = new GameObject("Projectile");
            projObj.transform.position = position;
            projObj.transform.localScale = Vector3.one * 0.4f;

            // Sprite (uses cached sprite)
            var sr = projObj.AddComponent<SpriteRenderer>();
            sr.sprite = GetSimpleSprite();
            sr.color = color;
            sr.sortingOrder = 100;

            // Physics
            var rb = projObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.velocity = direction.normalized * 8f;

            // Collider
            var col = projObj.AddComponent<CircleCollider2D>();
            col.radius = 0.25f;
            col.isTrigger = true;

            // Behavior
            var behavior = projObj.AddComponent<UltraSimpleProjectile>();
            behavior.damage = damage;
            behavior.direction = direction.normalized;

            Object.Destroy(projObj, 5f);
            return null;
        }
    }
}
