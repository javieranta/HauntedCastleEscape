using System;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Player;
using HauntedCastle.Inventory;
using HauntedCastle.Utils;
using HauntedCastle.Audio;
using HauntedCastle.Visuals;

namespace HauntedCastle.Enemies
{
    /// <summary>
    /// Base enemy controller handling AI, movement, and combat.
    /// Implements IDamageable for receiving damage from player attacks.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("Enemy Data")]
        [SerializeField] private EnemyData enemyData;

        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("AI State")]
        [SerializeField] private EnemyState currentState = EnemyState.Idle;
        [SerializeField] private float stateTimer;

        [Header("Movement")]
        [SerializeField] private Vector2 patrolPointA;
        [SerializeField] private Vector2 patrolPointB;
        [SerializeField] private bool patrolToB = true;

        // References
        private Transform _playerTransform;
        private Vector2 _startPosition;
        private Vector2 _wanderTarget;
        private float _wanderTimer;
        private int _currentHealth;
        private bool _isDead;
        private float _animTimer;
        private int _animFrame;

        // Procedural animation
        private float _proceduralAnimTime;
        private Vector3 _baseScale;
        private Vector3 _basePosition;
        private bool _isFlashing;

        // Events
        public event Action OnEnemyDeath;
        public event Action<Enemy> OnEnemyDestroyed;

        // Properties
        public EnemyData Data => enemyData;
        public EnemyState State => currentState;
        public bool IsAlive => !_isDead && _currentHealth > 0;
        public bool IsSpecialEnemy => enemyData?.isSpecialEnemy ?? false;
        public string CounteredByItem => enemyData?.counteredByItem ?? "";

        private void Awake()
        {
            InitializeComponents();
        }

        // Advanced AI components
        private EnemyAbilities _abilities;
        private AdvancedEnemyAI _advancedAI;

        private void Start()
        {
            _startPosition = transform.position;
            _basePosition = transform.localPosition;
            _baseScale = transform.localScale;
            FindPlayer();

            if (enemyData != null)
            {
                _currentHealth = enemyData.health;
                UpdateVisual();

                // Add advanced AI components
                InitializeAdvancedSystems();
            }
        }

        private void InitializeAdvancedSystems()
        {
            // Add abilities for enemies that have special attacks
            if (ShouldHaveAbilities())
            {
                _abilities = gameObject.AddComponent<EnemyAbilities>();
                _abilities.Initialize(this, enemyData.enemyType);
            }

            // Add advanced AI for smarter movement
            _advancedAI = gameObject.AddComponent<AdvancedEnemyAI>();
            _advancedAI.Initialize(this, enemyData);
        }

        private bool ShouldHaveAbilities()
        {
            // Most enemies get abilities except very basic ones
            switch (enemyData.enemyType)
            {
                case EnemyType.Ghost:
                case EnemyType.Demon:
                case EnemyType.Witch:
                case EnemyType.Skeleton:
                case EnemyType.Spider:
                case EnemyType.Bat:
                case EnemyType.Vampire:
                case EnemyType.Mummy:
                case EnemyType.Werewolf:
                case EnemyType.Reaper:
                    return true;
                default:
                    return false;
            }
        }

        private void Update()
        {
            if (_isDead) return;

            UpdateAI();
            UpdateAnimation();
        }

        private void FixedUpdate()
        {
            if (_isDead) return;

            ExecuteMovement();
        }

        private void InitializeComponents()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (col == null) col = GetComponent<Collider2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            // Configure rigidbody
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Configure collider - NOT a trigger so player can bump into enemies
            col.isTrigger = false;

            // Set tag and layer using the LayerSetup constants
            gameObject.tag = "Enemy";

            // Use LayerSetup constant, fallback to default if layer doesn't exist
            int enemyLayer = Core.LayerSetup.ENEMIES_LAYER;
            if (enemyLayer >= 0 && enemyLayer < 32)
            {
                gameObject.layer = enemyLayer;
            }

            Debug.Log($"[Enemy] Initialized with tag=Enemy, layer={gameObject.layer}");
        }

        /// <summary>
        /// Initializes the enemy with data.
        /// </summary>
        public void Initialize(EnemyData data, Vector2 position)
        {
            enemyData = data;
            transform.position = position;
            _startPosition = position;
            _currentHealth = data.health;

            // Set patrol points relative to spawn
            patrolPointA = position + Vector2.left * 2f;
            patrolPointB = position + Vector2.right * 2f;

            UpdateVisual();
            ChangeState(EnemyState.Idle);

            Debug.Log($"[Enemy] Spawned {data.displayName} at {position}");
        }

        private void FindPlayer()
        {
            var player = PlayerController.Instance;
            if (player != null)
            {
                _playerTransform = player.transform;
            }
        }

        private void UpdateVisual()
        {
            if (spriteRenderer == null || enemyData == null) return;

            spriteRenderer.sprite = enemyData.sprite;
            spriteRenderer.color = enemyData.tintColor;
            spriteRenderer.sortingLayerName = "Enemies";

            // Create placeholder if no sprite - use type-specific shapes
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = PlaceholderSpriteGenerator.GetEnemySprite(enemyData.enemyType);
            }
        }

        #region AI State Machine

        private void UpdateAI()
        {
            if (_playerTransform == null)
            {
                FindPlayer();
            }

            stateTimer -= Time.deltaTime;

            switch (currentState)
            {
                case EnemyState.Idle:
                    UpdateIdleState();
                    break;
                case EnemyState.Patrol:
                    UpdatePatrolState();
                    break;
                case EnemyState.Chase:
                    UpdateChaseState();
                    break;
                case EnemyState.Attack:
                    UpdateAttackState();
                    break;
                case EnemyState.Wander:
                    UpdateWanderState();
                    break;
                case EnemyState.Stunned:
                    UpdateStunnedState();
                    break;
            }
        }

        private void ChangeState(EnemyState newState)
        {
            currentState = newState;
            stateTimer = 0f;
        }

        private void UpdateIdleState()
        {
            // Check for player detection
            if (CanSeePlayer())
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            // Start behavior based on type
            if (stateTimer <= 0)
            {
                switch (enemyData?.behavior ?? EnemyBehavior.Wanderer)
                {
                    case EnemyBehavior.Patrol:
                        ChangeState(EnemyState.Patrol);
                        break;
                    case EnemyBehavior.Wanderer:
                        ChangeState(EnemyState.Wander);
                        break;
                    case EnemyBehavior.Chaser:
                        ChangeState(EnemyState.Chase);
                        break;
                    case EnemyBehavior.Ambush:
                        // Stay idle until player is very close
                        if (GetDistanceToPlayer() < (enemyData?.detectionRange ?? 3f) * 0.5f)
                        {
                            ChangeState(EnemyState.Chase);
                        }
                        break;
                    default:
                        stateTimer = 1f;
                        break;
                }
            }
        }

        private void UpdatePatrolState()
        {
            if (CanSeePlayer())
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            // Check if reached patrol point
            Vector2 target = patrolToB ? patrolPointB : patrolPointA;
            float dist = Vector2.Distance(transform.position, target);

            if (dist < 0.2f)
            {
                patrolToB = !patrolToB;
                stateTimer = 0.5f; // Pause at waypoint
            }
        }

        private void UpdateChaseState()
        {
            if (_playerTransform == null)
            {
                ChangeState(EnemyState.Idle);
                return;
            }

            float dist = GetDistanceToPlayer();

            // Lost player
            if (dist > (enemyData?.detectionRange ?? 5f) * 1.5f)
            {
                ChangeState(EnemyState.Idle);
                return;
            }

            // Close enough to attack
            if (dist < 0.8f)
            {
                ChangeState(EnemyState.Attack);
            }
        }

        private void UpdateAttackState()
        {
            stateTimer += Time.deltaTime;

            // Attack duration
            if (stateTimer > 0.3f)
            {
                ChangeState(EnemyState.Chase);
            }
        }

        private void UpdateWanderState()
        {
            if (CanSeePlayer())
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            _wanderTimer -= Time.deltaTime;

            if (_wanderTimer <= 0)
            {
                // Pick new wander target
                float radius = enemyData?.wanderRadius ?? 2f;
                _wanderTarget = _startPosition + UnityEngine.Random.insideUnitCircle * radius;
                _wanderTimer = UnityEngine.Random.Range(2f, 4f);
            }

            // Check if reached wander target
            if (Vector2.Distance(transform.position, _wanderTarget) < 0.3f)
            {
                _wanderTimer = 0f;
            }
        }

        private void UpdateStunnedState()
        {
            if (stateTimer <= 0)
            {
                ChangeState(EnemyState.Idle);
            }
        }

        #endregion

        #region Movement

        private void ExecuteMovement()
        {
            Vector2 moveDirection = Vector2.zero;
            float speed = enemyData?.moveSpeed ?? 2f;
            bool isChasing = false;

            switch (currentState)
            {
                case EnemyState.Patrol:
                    // Use advanced AI patrol patterns if available
                    if (_advancedAI != null)
                    {
                        Vector2 advancedPatrolTarget = _advancedAI.GetPatrolPosition();
                        moveDirection = (advancedPatrolTarget - (Vector2)transform.position).normalized;
                    }
                    else
                    {
                        Vector2 patrolTarget = patrolToB ? patrolPointB : patrolPointA;
                        moveDirection = (patrolTarget - (Vector2)transform.position).normalized;
                    }
                    break;

                case EnemyState.Chase:
                    isChasing = true;
                    if (_playerTransform != null)
                    {
                        // Use prediction from advanced AI if available
                        if (_advancedAI != null)
                        {
                            Vector2 predictedPos = _advancedAI.GetPredictedPlayerPosition();
                            moveDirection = (predictedPos - (Vector2)transform.position).normalized;
                        }
                        else
                        {
                            moveDirection = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;
                        }
                    }
                    break;

                case EnemyState.Wander:
                    // Use advanced AI patrol for wander too
                    if (_advancedAI != null)
                    {
                        Vector2 wanderPos = _advancedAI.GetPatrolPosition();
                        moveDirection = (wanderPos - (Vector2)transform.position).normalized;
                    }
                    else
                    {
                        moveDirection = (_wanderTarget - (Vector2)transform.position).normalized;
                    }
                    speed *= 0.5f; // Wander slower
                    break;

                case EnemyState.Stunned:
                    moveDirection = Vector2.zero;
                    break;
            }

            // Apply advanced movement modifications (flocking, evasion)
            if (_advancedAI != null && moveDirection != Vector2.zero)
            {
                moveDirection = _advancedAI.GetAdvancedMoveDirection(moveDirection, isChasing);
            }

            // CRITICAL: Validate velocity before applying to prevent physics corruption
            Vector2 newVelocity = moveDirection * speed;

            // Check for NaN/Infinity which can corrupt the physics engine
            if (float.IsNaN(newVelocity.x) || float.IsNaN(newVelocity.y) ||
                float.IsInfinity(newVelocity.x) || float.IsInfinity(newVelocity.y))
            {
                Debug.LogWarning($"[Enemy] Invalid velocity detected: {newVelocity}, resetting to zero");
                newVelocity = Vector2.zero;
            }

            // Clamp to reasonable max speed
            float maxSpeed = 20f;
            if (newVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            {
                newVelocity = newVelocity.normalized * maxSpeed;
            }

            rb.velocity = newVelocity;

            // Flip sprite based on direction
            if (spriteRenderer != null && moveDirection.x != 0)
            {
                spriteRenderer.flipX = moveDirection.x < 0;
            }
        }

        private bool CanSeePlayer()
        {
            if (_playerTransform == null) return false;

            float dist = GetDistanceToPlayer();
            return dist <= (enemyData?.detectionRange ?? 5f);
        }

        private float GetDistanceToPlayer()
        {
            if (_playerTransform == null) return float.MaxValue;
            return Vector2.Distance(transform.position, _playerTransform.position);
        }

        #endregion

        #region Combat

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_isDead) return;

            if (collision.gameObject.CompareTag("Player"))
            {
                AttackPlayer(collision.gameObject);
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_isDead) return;

            // Continuous damage while touching
            if (collision.gameObject.CompareTag("Player") && currentState != EnemyState.Stunned)
            {
                AttackPlayer(collision.gameObject);
            }
        }

        private void AttackPlayer(GameObject playerObj)
        {
            // Check if player has counter item for special enemies
            if (IsSpecialEnemy && !string.IsNullOrEmpty(CounteredByItem))
            {
                if (PlayerInventory.Instance != null && PlayerInventory.Instance.HasItem(CounteredByItem))
                {
                    // Player has counter item - enemy is repelled
                    Debug.Log($"[Enemy] {enemyData.displayName} repelled by {CounteredByItem}!");
                    Stun(2f);
                    return;
                }
                else
                {
                    // Special enemy instant kills without counter item
                    var playerHealth = playerObj.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.InstantKill();
                        Debug.Log($"[Enemy] {enemyData.displayName} INSTANT KILL - no counter item!");
                        return;
                    }
                }
            }

            // Normal damage
            var health = playerObj.GetComponent<PlayerHealth>();
            if (health != null && !health.IsInvulnerable)
            {
                Vector2 knockbackDir = (playerObj.transform.position - transform.position).normalized;
                health.TakeDamage(enemyData?.damage ?? 1, knockbackDir);
            }

            // Destroy on contact if configured
            if (enemyData != null && !enemyData.persistsAfterContact)
            {
                Die();
            }
        }

        /// <summary>
        /// Stuns the enemy for a duration.
        /// </summary>
        public void Stun(float duration)
        {
            ChangeState(EnemyState.Stunned);
            stateTimer = duration;
            rb.velocity = Vector2.zero;

            // Visual feedback
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.gray;
            }
        }

        #endregion

        #region IDamageable Implementation

        public void TakeDamage(int damage, Vector2 knockbackDirection)
        {
            if (_isDead) return;

            _currentHealth -= damage;

            // Knockback
            rb.velocity = knockbackDirection.normalized * 3f;

            // Play hit sound
            AudioManager.Instance?.PlaySFX(SoundEffect.EnemyHit);

            // Flash effect
            StartCoroutine(DamageFlash());

            // Visual effects - damage number and screen shake
            if (Effects.VisualEffectsManager.Instance != null)
            {
                bool isCritical = damage >= 5;
                Effects.VisualEffectsManager.Instance.ShowDamageNumber(transform.position, damage, isCritical);
                if (isCritical)
                {
                    Effects.VisualEffectsManager.Instance.ShakeScreen(0.08f, 0.1f);
                }
            }

            // Stun briefly
            Stun(0.3f);

            Debug.Log($"[Enemy] {enemyData?.displayName ?? "Enemy"} took {damage} damage. HP: {_currentHealth}");

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        private System.Collections.IEnumerator DamageFlash()
        {
            if (spriteRenderer == null) yield break;

            Color originalColor = enemyData?.tintColor ?? Color.white;

            spriteRenderer.color = Color.white;
            // CRITICAL: Use WaitForSecondsRealtime to work when timeScale = 0
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.color = originalColor;
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            rb.velocity = Vector2.zero;

            OnEnemyDeath?.Invoke();
            OnEnemyDestroyed?.Invoke(this);

            Debug.Log($"[Enemy] {enemyData?.displayName ?? "Enemy"} died!");

            // Play death sound
            AudioManager.Instance?.PlaySFX(SoundEffect.EnemyDeath);

            // Visual effects - death particles and screen shake
            if (Effects.VisualEffectsManager.Instance != null)
            {
                Color deathColor = enemyData?.tintColor ?? Color.white;
                Effects.VisualEffectsManager.Instance.SpawnParticleBurst(transform.position, deathColor, 15);
                Effects.VisualEffectsManager.Instance.ShakeScreen(0.12f, 0.15f);
            }

            // Award score for defeating enemy
            if (Core.ScoreManager.Instance != null && enemyData != null)
            {
                Core.ScoreManager.Instance.AwardEnemyKill(enemyData.enemyType);
            }

            // Death animation
            StartCoroutine(DeathAnimation());
        }

        private System.Collections.IEnumerator DeathAnimation()
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                transform.localScale = startScale * (1f - t);

                if (spriteRenderer != null)
                {
                    Color c = spriteRenderer.color;
                    c.a = 1f - t;
                    spriteRenderer.color = c;
                }

                yield return null;
            }

            Destroy(gameObject);
        }

        #endregion

        #region Animation

        private void UpdateAnimation()
        {
            _proceduralAnimTime += Time.deltaTime;

            // Frame-based animation
            if (enemyData?.animationSprites != null && enemyData.animationSprites.Length > 1)
            {
                _animTimer += Time.deltaTime;
                if (_animTimer >= 0.15f)
                {
                    _animTimer = 0f;
                    _animFrame = (_animFrame + 1) % enemyData.animationSprites.Length;
                    spriteRenderer.sprite = enemyData.animationSprites[_animFrame];
                }
            }

            // Procedural animation effects
            ApplyProceduralAnimation();
        }

        private void ApplyProceduralAnimation()
        {
            if (_isDead || _isFlashing) return;

            bool isMoving = rb.velocity.sqrMagnitude > 0.1f;
            float animSpeed = GetAnimationSpeed();

            if (isMoving)
            {
                // Walking bounce
                float bounce = Mathf.Abs(Mathf.Sin(_proceduralAnimTime * 8f * animSpeed)) * 0.04f;
                transform.localPosition = _basePosition + Vector3.up * bounce;

                // Slight lean in movement direction
                float lean = Mathf.Sin(_proceduralAnimTime * 8f * animSpeed) * 2f * Mathf.Sign(rb.velocity.x);
                transform.localRotation = Quaternion.Euler(0, 0, lean);

                // Flip based on movement direction
                if (Mathf.Abs(rb.velocity.x) > 0.1f)
                {
                    spriteRenderer.flipX = rb.velocity.x < 0;
                }
            }
            else
            {
                // Idle bobbing (type-specific)
                float bobAmount = GetIdleBobAmount();
                float bob = Mathf.Sin(_proceduralAnimTime * 2.5f * animSpeed) * bobAmount;
                transform.localPosition = _basePosition + Vector3.up * bob;
                transform.localRotation = Quaternion.identity;
            }

            // Enemy-type specific effects
            ApplyTypeSpecificAnimation();
        }

        private float GetAnimationSpeed()
        {
            return enemyData?.enemyType switch
            {
                EnemyType.Ghost => 0.6f,   // Slow, floaty
                EnemyType.Bat => 2f,       // Fast, frantic
                EnemyType.Spider => 1.5f,  // Quick scurrying
                EnemyType.Skeleton => 0.8f,
                EnemyType.Mummy => 0.5f,   // Slow, shambling
                EnemyType.Demon => 1.2f,
                EnemyType.Vampire => 1f,
                EnemyType.Witch => 0.9f,
                EnemyType.Werewolf => 1.3f,
                EnemyType.Reaper => 0.7f,
                _ => 1f
            };
        }

        private float GetIdleBobAmount()
        {
            return enemyData?.enemyType switch
            {
                EnemyType.Ghost => 0.08f,  // Floaty hovering
                EnemyType.Bat => 0.05f,    // Slight flutter
                EnemyType.Spider => 0.02f, // Minimal
                EnemyType.Skeleton => 0.025f,
                EnemyType.Mummy => 0.015f, // Almost none
                EnemyType.Demon => 0.04f,
                EnemyType.Vampire => 0.03f,
                EnemyType.Witch => 0.05f,  // Hovering
                EnemyType.Werewolf => 0.02f,
                EnemyType.Reaper => 0.06f, // Ethereal floating
                _ => 0.03f
            };
        }

        private void ApplyTypeSpecificAnimation()
        {
            switch (enemyData?.enemyType)
            {
                case EnemyType.Ghost:
                    // Ethereal fade effect
                    float fade = 0.6f + Mathf.Sin(_proceduralAnimTime * 1.5f) * 0.2f;
                    spriteRenderer.color = new Color(
                        enemyData.tintColor.r,
                        enemyData.tintColor.g,
                        enemyData.tintColor.b,
                        fade
                    );
                    break;

                case EnemyType.Bat:
                    // Wing flutter effect (scale pulse)
                    float flutter = 1f + Mathf.Abs(Mathf.Sin(_proceduralAnimTime * 15f)) * 0.1f;
                    transform.localScale = new Vector3(_baseScale.x * flutter, _baseScale.y, _baseScale.z);
                    break;

                case EnemyType.Witch:
                case EnemyType.Reaper:
                    // Hover effect
                    float hover = Mathf.Sin(_proceduralAnimTime * 2f) * 0.06f;
                    transform.localPosition = _basePosition + Vector3.up * hover;
                    break;

                default:
                    // Subtle breathing
                    float breathe = 1f + Mathf.Sin(_proceduralAnimTime * 2f) * 0.02f;
                    transform.localScale = new Vector3(_baseScale.x, _baseScale.y * breathe, _baseScale.z);
                    break;
            }
        }

        /// <summary>
        /// Triggers damage flash effect.
        /// </summary>
        public void PlayDamageAnimation()
        {
            if (!_isFlashing)
            {
                StartCoroutine(DamageFlashRoutine());
            }
        }

        private System.Collections.IEnumerator DamageFlashRoutine()
        {
            _isFlashing = true;
            Color originalColor = spriteRenderer.color;
            float duration = 0.15f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Flash and shake
                float flash = Mathf.Sin(t * Mathf.PI * 4f) * 0.5f + 0.5f;
                spriteRenderer.color = Color.Lerp(originalColor, Color.red, flash);

                float shake = (1f - t) * 0.1f;
                transform.localPosition = _basePosition + new Vector3(
                    UnityEngine.Random.Range(-shake, shake),
                    UnityEngine.Random.Range(-shake, shake),
                    0
                );

                yield return null;
            }

            spriteRenderer.color = originalColor;
            transform.localPosition = _basePosition;
            _isFlashing = false;
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, enemyData?.detectionRange ?? 5f);

            // Wander radius
            Gizmos.color = Color.cyan;
            Vector3 startPos = Application.isPlaying ? (Vector3)_startPosition : transform.position;
            Gizmos.DrawWireSphere(startPos, enemyData?.wanderRadius ?? 2f);

            // Patrol path
            if (enemyData?.behavior == EnemyBehavior.Patrol)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(patrolPointA, patrolPointB);
                Gizmos.DrawSphere(patrolPointA, 0.2f);
                Gizmos.DrawSphere(patrolPointB, 0.2f);
            }
        }
    }

    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Wander,
        Stunned,
        Dead
    }
}
