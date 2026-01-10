using UnityEngine;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Provides smooth procedural animations for sprites.
    /// Complements frame-based SpriteAnimator with smooth motion effects.
    /// Handles idle bobbing, walking bounce, attack animations, and special effects.
    /// </summary>
    public class ProceduralSpriteAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private AnimationMode currentMode = AnimationMode.Idle;
        [SerializeField] private float animationSpeed = 1f;

        [Header("Idle Animation")]
        [SerializeField] private float idleBobAmount = 0.05f;
        [SerializeField] private float idleBobSpeed = 2f;
        [SerializeField] private float idleSquashStretch = 0.02f;

        [Header("Walk Animation")]
        [SerializeField] private float walkBounceAmount = 0.08f;
        [SerializeField] private float walkBounceSpeed = 8f;
        [SerializeField] private float walkLeanAmount = 5f;

        [Header("Attack Animation")]
        [SerializeField] private float attackWindupTime = 0.1f;
        [SerializeField] private float attackSwingTime = 0.15f;
        [SerializeField] private float attackRecoverTime = 0.2f;

        [Header("Damage Animation")]
        [SerializeField] private float damageFlashDuration = 0.15f;
        [SerializeField] private Color damageFlashColor = new Color(1f, 0.3f, 0.3f);
        [SerializeField] private float damageShakeAmount = 0.1f;

        // Components
        private SpriteRenderer _spriteRenderer;
        private Transform _spriteTransform;

        // State
        private Vector3 _originalPosition;
        private Vector3 _originalScale;
        private Quaternion _originalRotation;
        private Color _originalColor;
        private float _animationTime;
        private float _modeStartTime;
        private Vector2 _moveDirection;
        private bool _isAnimating;

        public AnimationMode CurrentMode => currentMode;
        public bool IsAnimating => _isAnimating;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            _spriteTransform = _spriteRenderer != null ? _spriteRenderer.transform : transform;
        }

        private void Start()
        {
            CacheOriginalValues();
        }

        private void CacheOriginalValues()
        {
            _originalPosition = _spriteTransform.localPosition;
            _originalScale = _spriteTransform.localScale;
            _originalRotation = _spriteTransform.localRotation;
            _originalColor = _spriteRenderer != null ? _spriteRenderer.color : Color.white;
        }

        private void Update()
        {
            _animationTime += Time.deltaTime * animationSpeed;
            float modeTime = Time.time - _modeStartTime;

            switch (currentMode)
            {
                case AnimationMode.Idle:
                    UpdateIdleAnimation();
                    break;
                case AnimationMode.Walk:
                    UpdateWalkAnimation();
                    break;
                case AnimationMode.Attack:
                    UpdateAttackAnimation(modeTime);
                    break;
                case AnimationMode.Damage:
                    UpdateDamageAnimation(modeTime);
                    break;
                case AnimationMode.Death:
                    UpdateDeathAnimation(modeTime);
                    break;
            }
        }

        private void UpdateIdleAnimation()
        {
            // Gentle bobbing motion
            float bob = Mathf.Sin(_animationTime * idleBobSpeed) * idleBobAmount;
            _spriteTransform.localPosition = _originalPosition + Vector3.up * bob;

            // Subtle breathing (squash/stretch)
            float breathe = Mathf.Sin(_animationTime * idleBobSpeed * 0.5f) * idleSquashStretch;
            _spriteTransform.localScale = new Vector3(
                _originalScale.x * (1f - breathe * 0.5f),
                _originalScale.y * (1f + breathe),
                _originalScale.z
            );

            // Reset rotation
            _spriteTransform.localRotation = _originalRotation;
        }

        private void UpdateWalkAnimation()
        {
            // Bouncy walk cycle
            float bounce = Mathf.Abs(Mathf.Sin(_animationTime * walkBounceSpeed)) * walkBounceAmount;
            _spriteTransform.localPosition = _originalPosition + Vector3.up * bounce;

            // Lean into movement direction
            float lean = Mathf.Sin(_animationTime * walkBounceSpeed) * walkLeanAmount * _moveDirection.x;
            _spriteTransform.localRotation = Quaternion.Euler(0, 0, lean);

            // Squash on ground, stretch in air
            float cycle = Mathf.Sin(_animationTime * walkBounceSpeed);
            float squash = 1f + cycle * 0.08f;
            float stretch = 1f - cycle * 0.04f;
            _spriteTransform.localScale = new Vector3(
                _originalScale.x * stretch,
                _originalScale.y * squash,
                _originalScale.z
            );
        }

        private void UpdateAttackAnimation(float modeTime)
        {
            float totalTime = attackWindupTime + attackSwingTime + attackRecoverTime;

            if (modeTime < attackWindupTime)
            {
                // Windup - pull back
                float t = modeTime / attackWindupTime;
                float windupAmount = Mathf.Sin(t * Mathf.PI * 0.5f) * 0.15f;
                _spriteTransform.localPosition = _originalPosition - (Vector3)_moveDirection * windupAmount;
                _spriteTransform.localScale = _originalScale * (1f + t * 0.12f);
            }
            else if (modeTime < attackWindupTime + attackSwingTime)
            {
                // Swing - lunge forward
                float t = (modeTime - attackWindupTime) / attackSwingTime;
                float swingAmount = Mathf.Sin(t * Mathf.PI) * 0.35f;
                _spriteTransform.localPosition = _originalPosition + (Vector3)_moveDirection * swingAmount;

                // Stretch in attack direction
                float angle = Mathf.Atan2(_moveDirection.y, _moveDirection.x) * Mathf.Rad2Deg;
                _spriteTransform.localRotation = Quaternion.Euler(0, 0, angle * 0.15f * Mathf.Sin(t * Mathf.PI));
                _spriteTransform.localScale = _originalScale * (1f + (1f - Mathf.Abs(t - 0.5f) * 2f) * 0.08f);
            }
            else if (modeTime < totalTime)
            {
                // Recovery - return to normal
                float t = (modeTime - attackWindupTime - attackSwingTime) / attackRecoverTime;
                _spriteTransform.localPosition = Vector3.Lerp(_spriteTransform.localPosition, _originalPosition, t);
                _spriteTransform.localScale = Vector3.Lerp(_spriteTransform.localScale, _originalScale, t);
                _spriteTransform.localRotation = Quaternion.Lerp(_spriteTransform.localRotation, _originalRotation, t);
            }
            else
            {
                // Animation complete - return to idle
                SetMode(AnimationMode.Idle);
            }
        }

        private void UpdateDamageAnimation(float modeTime)
        {
            if (modeTime < damageFlashDuration)
            {
                // Flash red and shake
                float t = modeTime / damageFlashDuration;
                float flash = Mathf.Sin(t * Mathf.PI * 4f) * 0.5f + 0.5f;

                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = Color.Lerp(_originalColor, damageFlashColor, flash);
                }

                // Shake
                float shake = (1f - t) * damageShakeAmount;
                Vector3 shakeOffset = new Vector3(
                    Random.Range(-shake, shake),
                    Random.Range(-shake, shake),
                    0
                );
                _spriteTransform.localPosition = _originalPosition + shakeOffset;
            }
            else
            {
                // Reset
                if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = _originalColor;
                }
                _spriteTransform.localPosition = _originalPosition;
                SetMode(AnimationMode.Idle);
            }
        }

        private void UpdateDeathAnimation(float modeTime)
        {
            float deathDuration = 0.6f;

            if (modeTime < deathDuration)
            {
                float t = modeTime / deathDuration;

                // Fall and fade
                _spriteTransform.localPosition = _originalPosition + Vector3.down * t * 0.5f;
                _spriteTransform.localRotation = Quaternion.Euler(0, 0, t * 90f);
                _spriteTransform.localScale = _originalScale * (1f - t * 0.3f);

                if (_spriteRenderer != null)
                {
                    Color c = _originalColor;
                    c.a = 1f - t;
                    _spriteRenderer.color = c;
                }
            }
            else
            {
                _isAnimating = false;
            }
        }

        /// <summary>
        /// Sets the current animation mode.
        /// </summary>
        public void SetMode(AnimationMode mode)
        {
            if (currentMode == mode && mode != AnimationMode.Attack && mode != AnimationMode.Damage)
                return;

            currentMode = mode;
            _modeStartTime = Time.time;
            _isAnimating = true;

            // Reset to original values when changing to idle
            if (mode == AnimationMode.Idle)
            {
                ResetToOriginal();
            }
        }

        /// <summary>
        /// Sets movement direction for walk animation.
        /// </summary>
        public void SetMoveDirection(Vector2 direction)
        {
            _moveDirection = direction.normalized;

            // Flip sprite based on direction
            if (_spriteRenderer != null && Mathf.Abs(direction.x) > 0.1f)
            {
                _spriteRenderer.flipX = direction.x < 0;
            }

            // Switch to walk if moving, idle if stopped
            if (direction.sqrMagnitude > 0.01f)
            {
                if (currentMode == AnimationMode.Idle)
                {
                    SetMode(AnimationMode.Walk);
                }
            }
            else
            {
                if (currentMode == AnimationMode.Walk)
                {
                    SetMode(AnimationMode.Idle);
                }
            }
        }

        /// <summary>
        /// Triggers attack animation in the given direction.
        /// </summary>
        public void PlayAttack(Vector2 direction)
        {
            _moveDirection = direction.normalized;
            SetMode(AnimationMode.Attack);
        }

        /// <summary>
        /// Triggers damage animation.
        /// </summary>
        public void PlayDamage()
        {
            SetMode(AnimationMode.Damage);
        }

        /// <summary>
        /// Triggers death animation.
        /// </summary>
        public void PlayDeath()
        {
            SetMode(AnimationMode.Death);
        }

        /// <summary>
        /// Resets sprite to original transform values.
        /// </summary>
        public void ResetToOriginal()
        {
            _spriteTransform.localPosition = _originalPosition;
            _spriteTransform.localScale = _originalScale;
            _spriteTransform.localRotation = _originalRotation;

            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = _originalColor;
            }
        }

        /// <summary>
        /// Updates cached original values (call after scale changes).
        /// </summary>
        public void RefreshOriginalValues()
        {
            CacheOriginalValues();
        }
    }

    public enum AnimationMode
    {
        Idle,
        Walk,
        Attack,
        Damage,
        Death
    }
}
