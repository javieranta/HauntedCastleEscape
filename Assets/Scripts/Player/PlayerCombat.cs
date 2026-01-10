using System;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Utils;
using HauntedCastle.Core;

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
            Debug.Log($"[PlayerCombat] TryAttack called. CanAttack={CanAttack}, _canAttack={_canAttack}, cooldown={_attackCooldownTimer}");

            if (!CanAttack)
            {
                Debug.Log("[PlayerCombat] Cannot attack - still on cooldown or disabled");
                return false;
            }

            // Normalize direction, default to right if zero
            if (direction.sqrMagnitude < 0.01f)
            {
                direction = Vector2.right;
            }

            // If no character data, use fallback attack
            if (characterData == null)
            {
                Debug.LogWarning("[PlayerCombat] No characterData, using fallback projectile attack");
                _attackCooldownTimer = 0.4f;
                FireFallbackProjectile(direction);
                OnAttack?.Invoke();
                return true;
            }

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
            Debug.Log($"[PlayerCombat] Attack performed: {characterData.attackType} in direction {direction}");
            return true;
        }

        private void FireFallbackProjectile(Vector2 direction)
        {
            Vector3 spawnPos = transform.position + (Vector3)direction * 0.5f;
            GameObject projObj = CreateDefaultProjectile(spawnPos);

            var projectile = projObj.GetComponent<Projectile>();
            if (projectile == null)
            {
                projectile = projObj.AddComponent<Projectile>();
            }

            projectile.Initialize(direction, 8f, 1, 6f, true);
            Debug.Log($"[PlayerCombat] Fired fallback projectile in direction {direction}");
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

            // Create melee swing trail
            if (Effects.CombatFeedbackManager.Instance != null)
            {
                Effects.CombatFeedbackManager.Instance.CreateMeleeSwingTrail(transform.position, direction, meleeRadius + 0.3f);
            }

            // Find all enemies in range - use LayerSetup mask if enemyLayer not set
            LayerMask effectiveLayer = enemyLayer.value != 0 ? enemyLayer : (LayerMask)(1 << LayerSetup.ENEMIES_LAYER);

            // Also check all colliders if layer mask fails
            Collider2D[] hits = Physics2D.OverlapCircleAll(hitboxPos, meleeRadius, effectiveLayer);

            // If no hits with layer mask, try finding any damageable objects
            if (hits.Length == 0)
            {
                hits = Physics2D.OverlapCircleAll(hitboxPos, meleeRadius);
            }

            Debug.Log($"[PlayerCombat] Melee attack at {hitboxPos}, radius={meleeRadius}, found {hits.Length} colliders");

            int enemiesHit = 0;
            foreach (var hit in hits)
            {
                var damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(characterData.attackDamage, direction);
                    enemiesHit++;

                    // Combat feedback for each hit
                    if (Effects.CombatFeedbackManager.Instance != null)
                    {
                        bool isKill = !damageable.IsAlive;
                        Effects.CombatFeedbackManager.Instance.FullImpactEffect(
                            hit.transform.position,
                            direction,
                            characterData.attackDamage,
                            isKill
                        );
                    }
                }
            }

            if (enemiesHit > 0)
            {
                OnEnemyHit?.Invoke(characterData.attackDamage * enemiesHit);
                Debug.Log($"[PlayerCombat] Melee hit {enemiesHit} enemies");

                // Show combo counter
                if (Effects.CombatFeedbackManager.Instance != null && Core.ScoreManager.Instance != null)
                {
                    int combo = Core.ScoreManager.Instance.ComboCount;
                    if (combo > 1)
                    {
                        Effects.CombatFeedbackManager.Instance.ShowComboCounter(hitboxPos, combo);
                        Effects.CombatFeedbackManager.Instance.CreateComboStreak(hitboxPos, combo);
                    }
                }
            }

            // Visual feedback - spawn a slash effect
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

            // Thrown weapon sprite already set in CreateDefaultProjectile

            Debug.Log($"[PlayerCombat] Threw weapon in direction {direction}");
        }

        private GameObject CreateDefaultProjectile(Vector3 position)
        {
            var projObj = new GameObject("Projectile");
            projObj.transform.position = position;

            // Set layer safely
            int projLayer = LayerMask.NameToLayer("Projectiles");
            if (projLayer >= 0)
            {
                projObj.layer = projLayer;
            }
            else
            {
                projObj.layer = 0; // Default layer
            }

            // Set tag safely
            try { projObj.tag = "Projectile"; } catch { /* Tag doesn't exist */ }

            // Add rigidbody FIRST (required by Projectile component)
            var rb = projObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Add collider (required by Projectile component)
            var col = projObj.AddComponent<CircleCollider2D>();
            col.radius = 0.25f;
            col.isTrigger = true;

            // Add sprite with placeholder
            var sr = projObj.AddComponent<SpriteRenderer>();
            sr.sprite = GetProjectileSprite();

            // Set sorting layer safely
            if (SortingLayer.NameToID("Projectiles") != 0)
            {
                sr.sortingLayerName = "Projectiles";
            }
            else
            {
                sr.sortingLayerName = "Default";
            }
            sr.sortingOrder = 100; // High order to be visible

            Debug.Log($"[PlayerCombat] Created projectile at {position}");

            return projObj;
        }

        private Sprite GetProjectileSprite()
        {
            if (characterData == null)
                return PlaceholderSpriteGenerator.GetCircleSprite("DefaultProjectile", Color.white, 12);

            return characterData.attackType switch
            {
                AttackType.Projectile => PlaceholderSpriteGenerator.GetMagicProjectileSprite(),
                AttackType.Melee => PlaceholderSpriteGenerator.GetSwordSwingSprite(),
                AttackType.Thrown => PlaceholderSpriteGenerator.GetThrownProjectileSprite(),
                _ => PlaceholderSpriteGenerator.GetCircleSprite("DefaultProjectile", Color.white, 12)
            };
        }

        private void SpawnMeleeEffect(Vector2 position, Vector2 direction)
        {
            // Create temporary visual effect for melee attack
            var effectObj = new GameObject("MeleeEffect");
            effectObj.transform.position = position;

            var sr = effectObj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetSwordSwingSprite();
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
