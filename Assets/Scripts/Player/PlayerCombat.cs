using System;
using UnityEngine;
using HauntedCastle.Data;

namespace HauntedCastle.Player
{
    /// <summary>
    /// Handles player combat - attacks, projectiles, and melee.
    /// Attack type varies by character (Wizard=projectile, Knight=melee, Serf=thrown).
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Character Data")]
        [SerializeField] private CharacterData characterData;

        [Header("Projectile Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;

        [Header("Melee Settings")]
        [SerializeField] private Transform meleeHitbox;
        [SerializeField] private float meleeRadius = 0.8f;
        [SerializeField] private LayerMask enemyLayer;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;

        // State
        private float _attackCooldownTimer;
        private bool _canAttack = true;

        // Events
        public event Action OnAttack;
        public event Action<int> OnEnemyHit; // damage dealt

        // Properties
        public bool CanAttack => _canAttack && _attackCooldownTimer <= 0;
        public AttackType CurrentAttackType => characterData?.attackType ?? AttackType.Projectile;

        private void Update()
        {
            // Update cooldown
            if (_attackCooldownTimer > 0)
            {
                _attackCooldownTimer -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Sets the character data (determines attack type).
        /// </summary>
        public void SetCharacterData(CharacterData data)
        {
            characterData = data;
        }

        /// <summary>
        /// Attempts to perform an attack in the given direction.
        /// </summary>
        public bool TryAttack(Vector2 direction)
        {
            if (!CanAttack || characterData == null) return false;

            // Start cooldown
            _attackCooldownTimer = characterData.attackCooldown;

            // Perform attack based on type
            switch (characterData.attackType)
            {
                case AttackType.Projectile:
                    FireProjectile(direction);
                    break;

                case AttackType.Melee:
                    PerformMeleeAttack(direction);
                    break;

                case AttackType.Thrown:
                    ThrowWeapon(direction);
                    break;
            }

            // Play attack sound
            PlayAttackSound();

            OnAttack?.Invoke();
            return true;
        }

        private void FireProjectile(Vector2 direction)
        {
            Vector3 spawnPos = projectileSpawnPoint != null
                ? projectileSpawnPoint.position
                : transform.position + (Vector3)direction * 0.5f;

            GameObject projObj;

            if (projectilePrefab != null)
            {
                projObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                // Create default projectile
                projObj = CreateDefaultProjectile(spawnPos);
            }

            var projectile = projObj.GetComponent<Projectile>();
            if (projectile == null)
            {
                projectile = projObj.AddComponent<Projectile>();
            }

            projectile.Initialize(
                direction,
                characterData.projectileSpeed,
                characterData.attackDamage,
                characterData.attackRange,
                true // isPlayerProjectile
            );

            Debug.Log($"[PlayerCombat] Fired projectile in direction {direction}");
        }

        private void PerformMeleeAttack(Vector2 direction)
        {
            // Position hitbox in attack direction
            Vector2 hitboxPos = (Vector2)transform.position + direction * 0.8f;

            // Find all enemies in range
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitboxPos, meleeRadius, enemyLayer);

            int enemiesHit = 0;
            foreach (var hit in hits)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(characterData.attackDamage, direction);
                    enemiesHit++;
                }
            }

            if (enemiesHit > 0)
            {
                OnEnemyHit?.Invoke(characterData.attackDamage * enemiesHit);
                Debug.Log($"[PlayerCombat] Melee hit {enemiesHit} enemies");
            }

            // Visual feedback - could spawn a slash effect here
            SpawnMeleeEffect(hitboxPos, direction);
        }

        private void ThrowWeapon(Vector2 direction)
        {
            // Thrown weapons are similar to projectiles but may return or have different behavior
            Vector3 spawnPos = transform.position + (Vector3)direction * 0.5f;

            GameObject projObj = CreateDefaultProjectile(spawnPos);

            var projectile = projObj.GetComponent<Projectile>();
            if (projectile == null)
            {
                projectile = projObj.AddComponent<Projectile>();
            }

            // Thrown weapons are slightly slower but may have other properties
            projectile.Initialize(
                direction,
                characterData.projectileSpeed * 0.8f,
                characterData.attackDamage,
                characterData.attackRange * 0.7f,
                true
            );

            // Different color for thrown weapon
            var sr = projObj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = new Color(0.8f, 0.6f, 0.3f); // Brown for thrown object
            }

            Debug.Log($"[PlayerCombat] Threw weapon in direction {direction}");
        }

        private GameObject CreateDefaultProjectile(Vector3 position)
        {
            var projObj = new GameObject("Projectile");
            projObj.transform.position = position;
            projObj.layer = LayerMask.NameToLayer("Projectiles");
            projObj.tag = "Projectile";

            // Add sprite
            var sr = projObj.AddComponent<SpriteRenderer>();
            sr.color = GetProjectileColor();
            sr.sortingLayerName = "Projectiles";
            sr.sortingOrder = 0;

            // Add collider
            var col = projObj.AddComponent<CircleCollider2D>();
            col.radius = 0.2f;
            col.isTrigger = true;

            // Add rigidbody
            var rb = projObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            return projObj;
        }

        private Color GetProjectileColor()
        {
            if (characterData == null) return Color.white;

            return characterData.attackType switch
            {
                AttackType.Projectile => new Color(0.5f, 0.8f, 1f), // Blue for magic
                AttackType.Melee => Color.white,
                AttackType.Thrown => new Color(0.8f, 0.6f, 0.3f),   // Brown for thrown
                _ => Color.white
            };
        }

        private void SpawnMeleeEffect(Vector2 position, Vector2 direction)
        {
            // Create temporary visual effect for melee attack
            var effectObj = new GameObject("MeleeEffect");
            effectObj.transform.position = position;

            var sr = effectObj.AddComponent<SpriteRenderer>();
            sr.color = new Color(1f, 1f, 1f, 0.7f);
            sr.sortingLayerName = "Projectiles";

            // Rotate based on direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            effectObj.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Destroy after short duration
            Destroy(effectObj, 0.15f);
        }

        private void PlayAttackSound()
        {
            if (audioSource != null && characterData?.attackSound != null)
            {
                audioSource.PlayOneShot(characterData.attackSound);
            }
        }

        /// <summary>
        /// Enables or disables attacking.
        /// </summary>
        public void SetCanAttack(bool canAttack)
        {
            _canAttack = canAttack;
        }

        /// <summary>
        /// Resets the attack cooldown (e.g., for power-ups).
        /// </summary>
        public void ResetCooldown()
        {
            _attackCooldownTimer = 0f;
        }

        private void OnDrawGizmosSelected()
        {
            if (characterData?.attackType == AttackType.Melee)
            {
                // Draw melee range
                Gizmos.color = Color.red;
                Vector2 dir = Application.isPlaying && PlayerController.Instance != null
                    ? PlayerController.Instance.FacingDirection
                    : Vector2.right;

                Vector3 hitboxPos = transform.position + (Vector3)dir * 0.8f;
                Gizmos.DrawWireSphere(hitboxPos, meleeRadius);
            }
        }
    }

    /// <summary>
    /// Interface for anything that can take damage.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int damage, Vector2 knockbackDirection);
        bool IsAlive { get; }
    }
}
