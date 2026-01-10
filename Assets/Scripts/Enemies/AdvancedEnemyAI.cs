using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Player;

namespace HauntedCastle.Enemies
{
    /// <summary>
    /// Advanced AI behaviors for enemies.
    /// Provides more sophisticated movement patterns and decision-making.
    /// </summary>
    public class AdvancedEnemyAI : MonoBehaviour
    {
        [Header("Advanced AI Settings")]
        [SerializeField] private bool enableAdvancedAI = true;
        [SerializeField] private float reactionTime = 0.2f;
        [SerializeField] private float predictionStrength = 0.5f;

        [Header("Flocking Behavior")]
        [SerializeField] private bool enableFlocking = false;
        [SerializeField] private float flockRadius = 3f;
        [SerializeField] private float separationWeight = 1f;
        [SerializeField] private float cohesionWeight = 0.5f;

        [Header("Evasion")]
        [SerializeField] private bool canEvade = false;
        [SerializeField] private float evasionChance = 0.3f;
        [SerializeField] private float evasionDistance = 2f;

        [Header("Patrol Patterns")]
        [SerializeField] private PatrolPattern patrolPattern = PatrolPattern.Linear;

        private Enemy _enemy;
        private EnemyData _enemyData;
        private Transform _playerTransform;
        private Rigidbody2D _rb;
        private Vector2 _lastPlayerPosition;
        private Vector2 _playerVelocity;
        private float _reactionTimer;
        private bool _initialized;

        // Patrol state
        private Vector2[] _patrolPoints;
        private int _currentPatrolIndex;
        private float _patrolTimer;
        private Vector2 _circleCenter;
        private float _circleAngle;

        public enum PatrolPattern
        {
            Linear,         // Back and forth between points
            Circular,       // Circle around a point
            Figure8,        // Figure 8 pattern
            Random,         // Random wandering
            Sentry          // Stand, look around, move to next point
        }

        private void Update()
        {
            if (!_initialized || !enableAdvancedAI) return;

            UpdatePlayerTracking();
            _reactionTimer -= Time.deltaTime;
        }

        /// <summary>
        /// Initializes the advanced AI for this enemy.
        /// </summary>
        public void Initialize(Enemy enemy, EnemyData data)
        {
            _enemy = enemy;
            _enemyData = data;
            _rb = GetComponent<Rigidbody2D>();
            _initialized = true;

            // Find player
            var player = PlayerController.Instance;
            if (player != null)
            {
                _playerTransform = player.transform;
            }

            // Configure based on enemy type
            ConfigureForEnemyType();

            // Initialize patrol points
            InitializePatrolPoints();
        }

        private void ConfigureForEnemyType()
        {
            if (_enemyData == null) return;

            switch (_enemyData.enemyType)
            {
                case EnemyType.Ghost:
                    canEvade = true;
                    evasionChance = 0.5f;
                    predictionStrength = 0.3f;
                    patrolPattern = PatrolPattern.Circular;
                    break;

                case EnemyType.Skeleton:
                    enableFlocking = true;
                    patrolPattern = PatrolPattern.Sentry;
                    break;

                case EnemyType.Spider:
                    canEvade = true;
                    evasionChance = 0.4f;
                    patrolPattern = PatrolPattern.Random;
                    break;

                case EnemyType.Bat:
                    enableFlocking = true;
                    patrolPattern = PatrolPattern.Figure8;
                    predictionStrength = 0.7f;
                    break;

                case EnemyType.Demon:
                    predictionStrength = 0.8f;
                    patrolPattern = PatrolPattern.Linear;
                    break;

                case EnemyType.Witch:
                    canEvade = true;
                    evasionChance = 0.6f;
                    patrolPattern = PatrolPattern.Circular;
                    break;

                case EnemyType.Vampire:
                    predictionStrength = 1f;
                    canEvade = true;
                    evasionChance = 0.7f;
                    patrolPattern = PatrolPattern.Random;
                    break;

                case EnemyType.Werewolf:
                    predictionStrength = 0.9f;
                    patrolPattern = PatrolPattern.Linear;
                    break;

                case EnemyType.Mummy:
                    predictionStrength = 0.2f;
                    patrolPattern = PatrolPattern.Sentry;
                    break;

                case EnemyType.Reaper:
                    predictionStrength = 1f;
                    canEvade = true;
                    evasionChance = 0.8f;
                    patrolPattern = PatrolPattern.Random;
                    break;
            }
        }

        private void InitializePatrolPoints()
        {
            Vector2 startPos = transform.position;
            float patrolRange = _enemyData?.wanderRadius ?? 2f;
            _circleCenter = startPos;

            switch (patrolPattern)
            {
                case PatrolPattern.Linear:
                    _patrolPoints = new Vector2[]
                    {
                        startPos + Vector2.left * patrolRange,
                        startPos + Vector2.right * patrolRange
                    };
                    break;

                case PatrolPattern.Figure8:
                    _patrolPoints = new Vector2[]
                    {
                        startPos + new Vector2(-patrolRange, patrolRange * 0.5f),
                        startPos + new Vector2(0, 0),
                        startPos + new Vector2(patrolRange, -patrolRange * 0.5f),
                        startPos + new Vector2(0, 0)
                    };
                    break;

                case PatrolPattern.Sentry:
                    _patrolPoints = new Vector2[]
                    {
                        startPos,
                        startPos + Vector2.left * patrolRange,
                        startPos + Vector2.up * patrolRange,
                        startPos + Vector2.right * patrolRange,
                        startPos + Vector2.down * patrolRange
                    };
                    break;

                default:
                    _patrolPoints = new Vector2[] { startPos };
                    break;
            }
        }

        private void UpdatePlayerTracking()
        {
            if (_playerTransform == null) return;

            // Track player velocity for prediction
            Vector2 currentPlayerPos = _playerTransform.position;

            // CRITICAL FIX: Guard against divide by zero when Time.timeScale = 0
            // Division by zero produces Infinity/NaN which corrupts the physics engine
            if (Time.deltaTime > 0.0001f)
            {
                _playerVelocity = (currentPlayerPos - _lastPlayerPosition) / Time.deltaTime;

                // Sanity check: clamp velocity to prevent physics issues
                float maxVelocity = 50f;
                if (_playerVelocity.sqrMagnitude > maxVelocity * maxVelocity)
                {
                    _playerVelocity = _playerVelocity.normalized * maxVelocity;
                }

                // Check for NaN/Infinity and reset if found
                if (float.IsNaN(_playerVelocity.x) || float.IsNaN(_playerVelocity.y) ||
                    float.IsInfinity(_playerVelocity.x) || float.IsInfinity(_playerVelocity.y))
                {
                    _playerVelocity = Vector2.zero;
                }
            }
            // When timeScale is 0, don't update velocity (keep previous value or zero)

            _lastPlayerPosition = currentPlayerPos;
        }

        /// <summary>
        /// Gets the predicted position where player will be.
        /// </summary>
        public Vector2 GetPredictedPlayerPosition(float lookAheadTime = 0.5f)
        {
            if (_playerTransform == null) return transform.position;

            Vector2 currentPos = _playerTransform.position;
            Vector2 predictedPos = currentPos + _playerVelocity * lookAheadTime * predictionStrength;

            return predictedPos;
        }

        /// <summary>
        /// Gets movement direction with advanced AI applied.
        /// </summary>
        public Vector2 GetAdvancedMoveDirection(Vector2 baseDirection, bool isChasing)
        {
            if (!enableAdvancedAI) return baseDirection;

            Vector2 finalDirection = baseDirection;

            if (isChasing && _playerTransform != null)
            {
                // Apply prediction to intercept player
                Vector2 predictedPos = GetPredictedPlayerPosition();
                Vector2 toPlayer = (predictedPos - (Vector2)transform.position).normalized;
                finalDirection = Vector2.Lerp(baseDirection, toPlayer, predictionStrength);

                // Check for evasion
                if (canEvade && ShouldEvade())
                {
                    finalDirection = GetEvasionDirection(finalDirection);
                }
            }

            // Apply flocking behavior if enabled
            if (enableFlocking)
            {
                Vector2 flockingForce = CalculateFlockingForce();
                finalDirection = (finalDirection + flockingForce).normalized;
            }

            return finalDirection;
        }

        /// <summary>
        /// Gets the next patrol position based on pattern.
        /// </summary>
        public Vector2 GetPatrolPosition()
        {
            _patrolTimer += Time.deltaTime;

            switch (patrolPattern)
            {
                case PatrolPattern.Circular:
                    _circleAngle += Time.deltaTime * 1f;
                    float radius = _enemyData?.wanderRadius ?? 2f;
                    return _circleCenter + new Vector2(
                        Mathf.Cos(_circleAngle) * radius,
                        Mathf.Sin(_circleAngle) * radius
                    );

                case PatrolPattern.Random:
                    if (_patrolTimer >= 2f)
                    {
                        _patrolTimer = 0f;
                        float wanderRadius = _enemyData?.wanderRadius ?? 2f;
                        return _circleCenter + Random.insideUnitCircle * wanderRadius;
                    }
                    return _patrolPoints.Length > 0 ? _patrolPoints[0] : transform.position;

                case PatrolPattern.Sentry:
                    if (_patrolTimer >= 3f)
                    {
                        _patrolTimer = 0f;
                        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
                    }
                    return _patrolPoints[_currentPatrolIndex];

                default:
                    // Linear and Figure8
                    if (_patrolPoints.Length == 0) return transform.position;

                    Vector2 target = _patrolPoints[_currentPatrolIndex];
                    float dist = Vector2.Distance(transform.position, target);

                    if (dist < 0.3f)
                    {
                        _currentPatrolIndex = (_currentPatrolIndex + 1) % _patrolPoints.Length;
                    }

                    return _patrolPoints[_currentPatrolIndex];
            }
        }

        private bool ShouldEvade()
        {
            if (_playerTransform == null) return false;

            // Check if player is attacking (would need to hook into player attack system)
            // For now, evade randomly based on distance
            float dist = Vector2.Distance(transform.position, _playerTransform.position);
            if (dist < evasionDistance)
            {
                return Random.value < evasionChance * Time.deltaTime * 2f;
            }

            return false;
        }

        private Vector2 GetEvasionDirection(Vector2 currentDirection)
        {
            // Dodge perpendicular to current direction
            Vector2 perpendicular = new Vector2(-currentDirection.y, currentDirection.x);
            if (Random.value > 0.5f) perpendicular = -perpendicular;

            return (currentDirection + perpendicular).normalized;
        }

        private Vector2 CalculateFlockingForce()
        {
            Vector2 separation = Vector2.zero;
            Vector2 cohesion = Vector2.zero;
            int neighborCount = 0;

            // Find nearby enemies
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, flockRadius);

            foreach (var col in colliders)
            {
                if (col.gameObject == gameObject) continue;

                var otherEnemy = col.GetComponent<Enemy>();
                if (otherEnemy != null && otherEnemy.IsAlive)
                {
                    Vector2 toOther = col.transform.position - transform.position;
                    float dist = toOther.magnitude;

                    if (dist < flockRadius)
                    {
                        // Separation: move away from very close neighbors
                        if (dist < flockRadius * 0.5f && dist > 0.01f)
                        {
                            separation -= toOther.normalized / dist;
                        }

                        // Cohesion: move towards center of group
                        cohesion += (Vector2)col.transform.position;
                        neighborCount++;
                    }
                }
            }

            Vector2 result = Vector2.zero;

            if (neighborCount > 0)
            {
                cohesion /= neighborCount;
                Vector2 toCohesion = (cohesion - (Vector2)transform.position).normalized;

                result = separation * separationWeight + toCohesion * cohesionWeight;
                result = result.normalized * 0.3f; // Limit influence
            }

            return result;
        }

        private void OnDrawGizmosSelected()
        {
            if (!enableAdvancedAI) return;

            // Draw patrol path
            if (_patrolPoints != null && _patrolPoints.Length > 1)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < _patrolPoints.Length; i++)
                {
                    int next = (i + 1) % _patrolPoints.Length;
                    Gizmos.DrawLine(_patrolPoints[i], _patrolPoints[next]);
                    Gizmos.DrawSphere(_patrolPoints[i], 0.2f);
                }
            }

            // Draw prediction line
            if (_playerTransform != null && Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Vector2 predicted = GetPredictedPlayerPosition();
                Gizmos.DrawLine(transform.position, predicted);
                Gizmos.DrawWireSphere(predicted, 0.3f);
            }

            // Draw flock radius
            if (enableFlocking)
            {
                Gizmos.color = new Color(0, 1, 1, 0.3f);
                Gizmos.DrawWireSphere(transform.position, flockRadius);
            }
        }
    }
}
