using UnityEngine;
using HauntedCastle.Core;

namespace HauntedCastle.Player
{
    /// <summary>
    /// Projectile component for ranged attacks.
    /// Moves in a direction and damages enemies on contact.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private int damage = 1;
        [SerializeField] private float maxRange = 10f;
        [SerializeField] private bool isPlayerProjectile = true;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrailRenderer trailRenderer;

        [Header("Behavior")]
        [SerializeField] private bool destroyOnHit = true;
        [SerializeField] private bool piercing = false;
        [SerializeField] private int maxPierceCount = 3;

        private Rigidbody2D _rb;
        private Vector2 _direction;
        private Vector3 _startPosition;
        private int _pierceCount;
        private bool _initialized;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
            {
                _rb = gameObject.AddComponent<Rigidbody2D>();
            }

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            // Configure rigidbody
            _rb.gravityScale = 0f;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            Debug.Log($"[Projectile] Awake - rb={((_rb != null) ? "exists" : "NULL")}");
        }

        /// <summary>
        /// Initializes the projectile with movement parameters.
        /// </summary>
        public void Initialize(Vector2 direction, float projectileSpeed, int projectileDamage, float range, bool playerOwned)
        {
            _direction = direction.normalized;
            speed = projectileSpeed;
            damage = projectileDamage;
            maxRange = range;
            isPlayerProjectile = playerOwned;

            _startPosition = transform.position;
            _pierceCount = 0;
            _initialized = true;

            // Set velocity
            _rb.velocity = _direction * speed;

            // Rotate sprite to face direction
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Set layer based on owner (safely)
            int projLayer = LayerMask.NameToLayer("Projectiles");
            if (projLayer >= 0)
            {
                gameObject.layer = projLayer;
            }

            // Add projectile trail effect
            if (Effects.CombatFeedbackManager.Instance != null && spriteRenderer != null)
            {
                Color trailColor = spriteRenderer.color;
                Effects.CombatFeedbackManager.Instance.CreateProjectileTrail(gameObject, trailColor);
            }

            Debug.Log($"[Projectile] Initialized - direction={_direction}, speed={speed}, damage={damage}");
        }

        private void Update()
        {
            if (!_initialized) return;

            // Check max range
            float distance = Vector3.Distance(_startPosition, transform.position);
            if (distance >= maxRange)
            {
                DestroyProjectile();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollision(other);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Also handle non-trigger collisions (enemies have non-trigger colliders)
            HandleCollision(collision.collider);
        }

        private void HandleCollision(Collider2D other)
        {
            // Ignore same-team projectiles
            if (other.CompareTag("Projectile")) return;

            // Player projectiles hit enemies
            if (isPlayerProjectile)
            {
                // Check by tag OR by layer OR by component
                bool isEnemy = other.CompareTag("Enemy") ||
                               other.gameObject.layer == Core.LayerSetup.ENEMIES_LAYER ||
                               other.GetComponent<IDamageable>() != null;

                if (isEnemy)
                {
                    HitEnemy(other);
                }
                else if (other.gameObject.layer == Core.LayerSetup.WALLS_LAYER ||
                         other.CompareTag("Wall"))
                {
                    // Hit wall
                    DestroyProjectile();
                }
            }
            // Enemy projectiles hit player
            else
            {
                if (other.CompareTag("Player") || other.gameObject.layer == Core.LayerSetup.PLAYER_LAYER)
                {
                    HitPlayer(other);
                }
                else if (other.gameObject.layer == Core.LayerSetup.WALLS_LAYER ||
                         other.CompareTag("Wall"))
                {
                    DestroyProjectile();
                }
            }
        }

        private void HitEnemy(Collider2D enemy)
        {
            // Try to damage enemy
            var damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, _direction);
            }

            // Handle piercing
            if (piercing)
            {
                _pierceCount++;
                if (_pierceCount >= maxPierceCount)
                {
                    DestroyProjectile();
                }
            }
            else if (destroyOnHit)
            {
                DestroyProjectile();
            }

            // Spawn hit effect
            SpawnHitEffect();
        }

        private void HitPlayer(Collider2D player)
        {
            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage, _direction);
            }

            if (destroyOnHit)
            {
                DestroyProjectile();
            }

            SpawnHitEffect();
        }

        private void SpawnHitEffect()
        {
            // Create simple hit particle effect
            var effectObj = new GameObject("HitEffect");
            effectObj.transform.position = transform.position;

            var sr = effectObj.AddComponent<SpriteRenderer>();
            sr.color = spriteRenderer != null ? spriteRenderer.color : Color.white;
            sr.sortingLayerName = "Projectiles";
            sr.sortingOrder = 1;

            // Scale up then destroy
            StartCoroutine(HitEffectRoutine(effectObj));
        }

        private System.Collections.IEnumerator HitEffectRoutine(GameObject effect)
        {
            float duration = 0.2f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                effect.transform.localScale = Vector3.one * (1f + t);

                var sr = effect.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f - t);
                }

                yield return null;
            }

            Destroy(effect);
        }

        private void DestroyProjectile()
        {
            // Could add destruction effect here
            Destroy(gameObject);
        }

        /// <summary>
        /// Reflects the projectile (e.g., hitting a shield).
        /// </summary>
        public void Reflect()
        {
            _direction = -_direction;
            _rb.velocity = _direction * speed;
            isPlayerProjectile = !isPlayerProjectile;

            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// Boosts projectile speed (e.g., power-up).
        /// </summary>
        public void BoostSpeed(float multiplier)
        {
            speed *= multiplier;
            _rb.velocity = _direction * speed;
        }
    }
}
