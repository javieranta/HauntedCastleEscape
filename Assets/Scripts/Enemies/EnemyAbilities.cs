using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Player;

namespace HauntedCastle.Enemies
{
    /// <summary>
    /// Provides special abilities for different enemy types.
    /// Attached to Enemy to give them unique attacks and behaviors.
    /// </summary>
    public class EnemyAbilities : MonoBehaviour
    {
        [Header("Ability Settings")]
        [SerializeField] private float abilityCooldown = 3f;
        [SerializeField] private float abilityRange = 5f;

        private Enemy _enemy;
        private EnemyType _enemyType;
        private float _cooldownTimer;
        private Transform _playerTransform;
        private bool _initialized;

        // Special state for certain enemies
        private bool _isInvisible;
        private bool _isTeleporting;
        private float _teleportTimer;
        private Vector3 _teleportTarget;

        private void Update()
        {
            try
            {
                if (!_initialized) return;

                _cooldownTimer -= Time.deltaTime;

                // Check if player is in range for ability use
                if (_cooldownTimer <= 0 && _playerTransform != null)
                {
                    float dist = Vector2.Distance(transform.position, _playerTransform.position);
                    if (dist <= abilityRange)
                    {
                        UseAbility();
                    }
                }

                // Handle ongoing ability effects - DISABLED as it may cause issues
                // UpdateAbilityEffects();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[EnemyAbilities] Update error: {e.Message}");
            }
        }

        /// <summary>
        /// Initializes abilities for the enemy type.
        /// </summary>
        public void Initialize(Enemy enemy, EnemyType type)
        {
            _enemy = enemy;
            _enemyType = type;
            _initialized = true;

            // Find player
            var player = PlayerController.Instance;
            if (player != null)
            {
                _playerTransform = player.transform;
            }

            // Set ability parameters based on enemy type
            ConfigureForType();
        }

        private void ConfigureForType()
        {
            switch (_enemyType)
            {
                case EnemyType.Ghost:
                    abilityCooldown = 4f;
                    abilityRange = 6f;
                    break;
                case EnemyType.Demon:
                    abilityCooldown = 2f;
                    abilityRange = 7f;
                    break;
                case EnemyType.Witch:
                    abilityCooldown = 2.5f;
                    abilityRange = 8f;
                    break;
                case EnemyType.Skeleton:
                    abilityCooldown = 3f;
                    abilityRange = 5f;
                    break;
                case EnemyType.Spider:
                    abilityCooldown = 4f;
                    abilityRange = 4f;
                    break;
                case EnemyType.Bat:
                    abilityCooldown = 5f;
                    abilityRange = 6f;
                    break;
                case EnemyType.Vampire:
                    abilityCooldown = 3f;
                    abilityRange = 5f;
                    break;
                case EnemyType.Mummy:
                    abilityCooldown = 4f;
                    abilityRange = 3f;
                    break;
                default:
                    abilityCooldown = 3f;
                    abilityRange = 5f;
                    break;
            }
        }

        private void UseAbility()
        {
            _cooldownTimer = abilityCooldown;

            try
            {
                // SIMPLIFIED: All ranged enemies just fire a simple projectile
                // This avoids complex ability code that may cause freezes
                if (_playerTransform == null) return;

                Vector2 direction = (_playerTransform.position - transform.position).normalized;

                // Validate direction
                if (float.IsNaN(direction.x) || float.IsNaN(direction.y) || direction.sqrMagnitude < 0.1f)
                {
                    direction = Vector2.right;
                }

                Vector3 spawnPos = transform.position + (Vector3)(direction * 0.5f);

                // Get color based on enemy type
                Color projColor = _enemyType switch
                {
                    EnemyType.Demon => new Color(1f, 0.3f, 0f),
                    EnemyType.Witch => new Color(0.5f, 0f, 0.8f),
                    EnemyType.Spider => new Color(0.9f, 0.9f, 0.9f),
                    EnemyType.Bat => new Color(0.8f, 0.8f, 1f),
                    EnemyType.Skeleton => Color.white,
                    _ => Color.red
                };

                // Get projectile type
                EnemyProjectile.ProjectileType projType = _enemyType switch
                {
                    EnemyType.Demon => EnemyProjectile.ProjectileType.Fireball,
                    EnemyType.Witch => EnemyProjectile.ProjectileType.ShadowBolt,
                    EnemyType.Spider => EnemyProjectile.ProjectileType.WebShot,
                    EnemyType.Bat => EnemyProjectile.ProjectileType.SonicWave,
                    EnemyType.Skeleton => EnemyProjectile.ProjectileType.BoneSpear,
                    _ => EnemyProjectile.ProjectileType.Fireball
                };

                // Only these enemy types fire projectiles
                switch (_enemyType)
                {
                    case EnemyType.Demon:
                    case EnemyType.Witch:
                    case EnemyType.Skeleton:
                    case EnemyType.Spider:
                    case EnemyType.Bat:
                        EnemyProjectile.SpawnProjectile(spawnPos, direction, 1, projType, projColor);
                        Debug.Log($"[EnemyAbilities] {_enemyType} fired {projType}");
                        break;

                    // Non-projectile abilities - simplified or disabled
                    case EnemyType.Ghost:
                    case EnemyType.Vampire:
                    case EnemyType.Mummy:
                    case EnemyType.Werewolf:
                    case EnemyType.Reaper:
                        // These abilities are disabled for now to prevent freezes
                        Debug.Log($"[EnemyAbilities] {_enemyType} ability skipped (disabled)");
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[EnemyAbilities] UseAbility error for {_enemyType}: {e.Message}\n{e.StackTrace}");
            }
        }

        #region Ghost Abilities

        private void GhostPhaseAbility()
        {
            // Ghost becomes invisible and phases through walls briefly
            if (!_isInvisible)
            {
                StartCoroutine(PhaseSequence());
            }
        }

        private System.Collections.IEnumerator PhaseSequence()
        {
            _isInvisible = true;
            var sr = GetComponent<SpriteRenderer>();
            var col = GetComponent<Collider2D>();

            // Fade out
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0.3f;
                sr.color = c;
            }

            // Disable collision briefly
            if (col != null) col.enabled = false;

            // CRITICAL: Use WaitForSecondsRealtime to work when timeScale = 0
            yield return new WaitForSecondsRealtime(1.5f);

            // Reappear near player
            if (_playerTransform != null)
            {
                Vector2 offset = Random.insideUnitCircle.normalized * 2f;
                transform.position = _playerTransform.position + (Vector3)offset;
            }

            // Fade back in
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 1f;
                sr.color = c;
            }

            if (col != null) col.enabled = true;
            _isInvisible = false;

            Debug.Log("[EnemyAbilities] Ghost phased and reappeared!");
        }

        #endregion

        #region Demon Abilities

        private void DemonFireballAbility()
        {
            if (_playerTransform == null) return;

            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            Vector3 spawnPos = transform.position + (Vector3)(direction * 0.5f);

            EnemyProjectile.SpawnProjectile(
                spawnPos,
                direction,
                2,
                EnemyProjectile.ProjectileType.Fireball,
                new Color(1f, 0.3f, 0f) // Orange-red fireball
            );

            // Visual effect at spawn
            if (Effects.VisualEffectsManager.Instance != null)
            {
                Effects.VisualEffectsManager.Instance.SpawnParticleBurst(spawnPos, Color.red, 5);
            }

            Debug.Log("[EnemyAbilities] Demon fired fireball!");
        }

        #endregion

        #region Witch Abilities

        private void WitchMagicBoltAbility()
        {
            if (_playerTransform == null) return;

            // Witch fires 3 spread bolts
            Vector2 baseDir = (_playerTransform.position - transform.position).normalized;
            float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

            for (int i = -1; i <= 1; i++)
            {
                float angle = baseAngle + (i * 15f);
                Vector2 dir = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );

                Vector3 spawnPos = transform.position + (Vector3)(dir * 0.5f);

                EnemyProjectile.SpawnProjectile(
                    spawnPos,
                    dir,
                    1,
                    EnemyProjectile.ProjectileType.ShadowBolt,
                    new Color(0.5f, 0f, 0.8f) // Purple magic bolt
                );
            }

            Debug.Log("[EnemyAbilities] Witch fired magic bolts!");
        }

        #endregion

        #region Skeleton Abilities

        private void SkeletonBoneThrowAbility()
        {
            if (_playerTransform == null) return;

            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            Vector3 spawnPos = transform.position + (Vector3)(direction * 0.5f);

            EnemyProjectile.SpawnProjectile(
                spawnPos,
                direction,
                1,
                EnemyProjectile.ProjectileType.BoneSpear,
                Color.white
            );

            Debug.Log("[EnemyAbilities] Skeleton threw bone!");
        }

        #endregion

        #region Spider Abilities

        private void SpiderWebShotAbility()
        {
            if (_playerTransform == null) return;

            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            Vector3 spawnPos = transform.position + (Vector3)(direction * 0.5f);

            EnemyProjectile.SpawnProjectile(
                spawnPos,
                direction,
                1,
                EnemyProjectile.ProjectileType.WebShot,
                new Color(0.9f, 0.9f, 0.9f, 0.8f) // White web
            );

            Debug.Log("[EnemyAbilities] Spider shot web!");
        }

        #endregion

        #region Bat Abilities

        private void BatSonicScreamAbility()
        {
            if (_playerTransform == null) return;

            // Bat fires a wide sonic wave
            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            Vector3 spawnPos = transform.position + (Vector3)(direction * 0.3f);

            EnemyProjectile.SpawnProjectile(
                spawnPos,
                direction,
                1,
                EnemyProjectile.ProjectileType.SonicWave,
                new Color(0.8f, 0.8f, 1f, 0.6f) // Light blue wave
            );

            // Small screen shake for dramatic effect
            if (Effects.VisualEffectsManager.Instance != null)
            {
                Effects.VisualEffectsManager.Instance.ShakeScreen(0.05f, 0.1f);
            }

            Debug.Log("[EnemyAbilities] Bat used sonic scream!");
        }

        #endregion

        #region Vampire Abilities

        private void VampireTeleportAbility()
        {
            if (_playerTransform == null || _isTeleporting) return;

            StartCoroutine(TeleportSequence());
        }

        private System.Collections.IEnumerator TeleportSequence()
        {
            _isTeleporting = true;
            var sr = GetComponent<SpriteRenderer>();

            // Fade out with bat particles
            if (Effects.VisualEffectsManager.Instance != null)
            {
                Effects.VisualEffectsManager.Instance.SpawnParticleBurst(transform.position, Color.black, 10);
            }

            if (sr != null)
            {
                float fadeTime = 0.3f;
                float elapsed = 0f;
                Color startColor = sr.color;

                while (elapsed < fadeTime)
                {
                    // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                    elapsed += Time.unscaledDeltaTime;
                    Color c = startColor;
                    c.a = 1f - (elapsed / fadeTime);
                    sr.color = c;
                    yield return null;
                }
            }

            // Teleport behind player
            if (_playerTransform != null)
            {
                Vector2 playerForward = _playerTransform.right; // Assuming facing direction
                Vector2 behindPlayer = (Vector2)_playerTransform.position - playerForward * 1.5f;
                transform.position = behindPlayer;
            }

            // CRITICAL: Use WaitForSecondsRealtime to work when timeScale = 0
            yield return new WaitForSecondsRealtime(0.1f);

            // Fade in
            if (Effects.VisualEffectsManager.Instance != null)
            {
                Effects.VisualEffectsManager.Instance.SpawnParticleBurst(transform.position, new Color(0.5f, 0f, 0f), 10);
            }

            if (sr != null)
            {
                float fadeTime = 0.2f;
                float elapsed = 0f;
                Color startColor = sr.color;
                startColor.a = 0f;
                sr.color = startColor;

                while (elapsed < fadeTime)
                {
                    // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                    elapsed += Time.unscaledDeltaTime;
                    Color c = startColor;
                    c.a = elapsed / fadeTime;
                    sr.color = c;
                    yield return null;
                }

                Color final = sr.color;
                final.a = 1f;
                sr.color = final;
            }

            _isTeleporting = false;
            Debug.Log("[EnemyAbilities] Vampire teleported behind player!");
        }

        #endregion

        #region Mummy Abilities

        private void MummyCurseAbility()
        {
            // Mummy releases a curse aura that damages nearby player
            if (_playerTransform == null) return;

            float dist = Vector2.Distance(transform.position, _playerTransform.position);
            if (dist <= 3f)
            {
                var playerHealth = _playerTransform.GetComponent<PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsInvulnerable)
                {
                    playerHealth.TakeDamage(1, Vector2.zero);
                    Debug.Log("[EnemyAbilities] Mummy curse damaged player!");
                }

                // Visual curse effect
                if (Effects.VisualEffectsManager.Instance != null)
                {
                    Effects.VisualEffectsManager.Instance.ShowTextPopup(
                        _playerTransform.position + Vector3.up,
                        "CURSED!",
                        new Color(0.4f, 0.3f, 0f)
                    );
                    Effects.VisualEffectsManager.Instance.SpawnParticleBurst(
                        transform.position,
                        new Color(0.5f, 0.4f, 0.1f),
                        12
                    );
                }
            }
        }

        #endregion

        #region Werewolf Abilities

        private void WerewolfLungeAbility()
        {
            if (_playerTransform == null) return;

            StartCoroutine(LungeSequence());
        }

        private System.Collections.IEnumerator LungeSequence()
        {
            Vector2 lungeDir = (_playerTransform.position - transform.position).normalized;
            float lungeSpeed = 15f;
            float lungeDuration = 0.3f;
            float elapsed = 0f;

            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                while (elapsed < lungeDuration)
                {
                    // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                    elapsed += Time.unscaledDeltaTime;
                    rb.velocity = lungeDir * lungeSpeed;
                    yield return null;
                }
                rb.velocity = Vector2.zero;
            }

            Debug.Log("[EnemyAbilities] Werewolf lunged at player!");
        }

        #endregion

        #region Reaper Abilities

        private void ReaperDeathTouchAbility()
        {
            // Reaper creates a death zone around itself
            if (Effects.VisualEffectsManager.Instance != null)
            {
                // Spawn dark particles in a circle
                for (int i = 0; i < 8; i++)
                {
                    float angle = (i / 8f) * Mathf.PI * 2f;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * 1.5f;
                    Effects.VisualEffectsManager.Instance.SpawnParticleBurst(
                        transform.position + offset,
                        Color.black,
                        3
                    );
                }
            }

            // Check if player is in death zone
            if (_playerTransform != null)
            {
                float dist = Vector2.Distance(transform.position, _playerTransform.position);
                if (dist <= 2f)
                {
                    var playerHealth = _playerTransform.GetComponent<PlayerHealth>();
                    if (playerHealth != null && !playerHealth.IsInvulnerable)
                    {
                        playerHealth.TakeDamage(3, Vector2.zero); // Heavy damage
                        Debug.Log("[EnemyAbilities] Reaper's death touch hit player!");
                    }
                }
            }
        }

        #endregion

        private void UpdateAbilityEffects()
        {
            // Update any ongoing visual effects
            if (_isInvisible)
            {
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    // Ghostly flicker effect
                    float flicker = 0.2f + Mathf.Sin(Time.time * 10f) * 0.1f;
                    Color c = sr.color;
                    c.a = flicker;
                    sr.color = c;
                }
            }
        }
    }
}
