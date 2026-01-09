using System;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Player;
using HauntedCastle.Inventory;

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
        [SerializeField] private EnemyHealth health;

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

        private void Start()
        {
            _startPosition = transform.position;
            FindPlayer();

            if (enemyData != null)
            {
                _currentHealth = enemyData.health;
                UpdateVisual();
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
            if (health == null) health = GetComponent<EnemyHealth>();

            // Configure rigidbody
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Configure collider
            col.isTrigger = false;

            // Set tag and layer
            gameObject.tag = "Enemy";
            gameObject.layer = LayerMask.NameToLayer("Enemies");
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

            // Create placeholder if no sprite
            if (spriteRenderer.sprite == null)
            {
                spriteRenderer.sprite = CreatePlaceholderSprite();
            }
        }

        private Sprite CreatePlaceholderSprite()
        {
            int size = 14;
            Texture2D texture = new Texture2D(size, size);
            texture.filterMode = FilterMode.Point;

            Color enemyColor = enemyData?.tintColor ?? Color.red;
            Color[] pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Create diamond shape for enemies
                    int centerX = size / 2;
                    int centerY = size / 2;
                    int dist = Mathf.Abs(x - centerX) + Mathf.Abs(y - centerY);

                    if (dist <= size / 2 - 1)
                    {
                        pixels[y * size + x] = enemyColor;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 14f);
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

            switch (currentState)
            {
                case EnemyState.Patrol:
                    Vector2 patrolTarget = patrolToB ? patrolPointB : patrolPointA;
                    moveDirection = (patrolTarget - (Vector2)transform.position).normalized;
                    break;

                case EnemyState.Chase:
                    if (_playerTransform != null)
                    {
                        moveDirection = ((Vector2)_playerTransform.position - (Vector2)transform.position).normalized;
                    }
                    break;

                case EnemyState.Wander:
                    moveDirection = (_wanderTarget - (Vector2)transform.position).normalized;
                    speed *= 0.5f; // Wander slower
                    break;

                case EnemyState.Stunned:
                    moveDirection = Vector2.zero;
                    break;
            }

            rb.linearVelocity = moveDirection * speed;

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
            rb.linearVelocity = Vector2.zero;

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
            rb.linearVelocity = knockbackDirection.normalized * 3f;

            // Flash effect
            StartCoroutine(DamageFlash());

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
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            rb.linearVelocity = Vector2.zero;

            OnEnemyDeath?.Invoke();
            OnEnemyDestroyed?.Invoke(this);

            Debug.Log($"[Enemy] {enemyData?.displayName ?? "Enemy"} died!");

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
                elapsed += Time.deltaTime;
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
            if (enemyData?.animationSprites == null || enemyData.animationSprites.Length <= 1)
                return;

            _animTimer += Time.deltaTime;
            if (_animTimer >= 0.15f)
            {
                _animTimer = 0f;
                _animFrame = (_animFrame + 1) % enemyData.animationSprites.Length;
                spriteRenderer.sprite = enemyData.animationSprites[_animFrame];
            }
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
