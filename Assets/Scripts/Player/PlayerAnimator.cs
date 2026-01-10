using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Visuals;

namespace HauntedCastle.Player
{
    /// <summary>
    /// Handles player sprite animation.
    /// Supports 4-directional movement and attack animations.
    /// Integrates procedural animation for smooth motion effects.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite idleDown;
        [SerializeField] private Sprite idleUp;
        [SerializeField] private Sprite idleLeft;
        [SerializeField] private Sprite idleRight;

        [SerializeField] private Sprite[] walkDown;
        [SerializeField] private Sprite[] walkUp;
        [SerializeField] private Sprite[] walkLeft;
        [SerializeField] private Sprite[] walkRight;

        [SerializeField] private Sprite attackSprite;

        [Header("Animation Settings")]
        [SerializeField] private float walkFrameRate = 0.15f;
        [SerializeField] private float attackDuration = 0.2f;

        [Header("Sprite Sizing")]
        [SerializeField] private float targetSpriteWorldSize = 1f; // Target size in world units
        [SerializeField] private bool autoScaleSprite = true;

        [Header("Procedural Animation")]
        // Scale-based animation (safe for physics - doesn't modify position)
        [SerializeField] private bool useScaleAnimation = true;
        [SerializeField] private float idleBreathAmount = 0.05f;
        [SerializeField] private float idleBreathSpeed = 2.5f;
        [SerializeField] private float walkSquashAmount = 0.15f;
        [SerializeField] private float walkSquashSpeed = 16f;

        [Header("Character Data")]
        [SerializeField] private CharacterData characterData;

        private SpriteRenderer _spriteRenderer;
        private ProceduralSpriteAnimator _proceduralAnimator;
        private bool _isMoving;
        private Vector2 _facingDirection = Vector2.down;
        private float _walkTimer;
        private int _currentWalkFrame;
        private bool _isAttacking;
        private float _attackTimer;
        private float _animationTime;

        // Direction tracking
        private enum Direction { Down, Up, Left, Right }
        private Direction _currentDirection = Direction.Down;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _proceduralAnimator = GetComponent<ProceduralSpriteAnimator>();

            // Try to load sprite from Resources based on character type
            TryLoadSpriteFromResources();
        }

        private void Start()
        {
            // Capture base scale after Awake has done any auto-scaling
            _baseScale = transform.localScale;
            UpdateIdleSprite();

            Debug.Log($"[PlayerAnimator] Base scale set to: {_baseScale}");
        }

        private Vector3 _baseScale = Vector3.one;

        /// <summary>
        /// Attempts to load character sprite from Resources folder.
        /// </summary>
        private void TryLoadSpriteFromResources()
        {
            // Try loading knight sprite (default for now)
            string[] characterPaths = new string[]
            {
                "Sprites/Characters/Knight/knight_idle",
                "Sprites/Characters/Wizard/wizard_idle",
                "Sprites/Characters/Serf/serf_idle"
            };

            foreach (string path in characterPaths)
            {
                Sprite loadedSprite = Resources.Load<Sprite>(path);
                if (loadedSprite != null)
                {
                    Debug.Log($"[PlayerAnimator] Loaded sprite from: {path}");
                    idleDown = loadedSprite;
                    idleUp = loadedSprite;
                    idleLeft = loadedSprite;
                    idleRight = loadedSprite;

                    // Apply immediately
                    if (_spriteRenderer != null)
                    {
                        _spriteRenderer.sprite = loadedSprite;

                        // Fix filter mode for smooth rendering (not pixelated)
                        if (loadedSprite.texture != null)
                        {
                            loadedSprite.texture.filterMode = FilterMode.Bilinear;
                            Debug.Log($"[PlayerAnimator] Set texture filter to Bilinear for smooth rendering");
                        }

                        // Auto-scale large sprites to fit target world size
                        if (autoScaleSprite)
                        {
                            ScaleSpriteToTargetSize(loadedSprite);
                        }
                    }
                    return;
                }
            }

            Debug.Log("[PlayerAnimator] No custom sprites found, using defaults");
        }

        /// <summary>
        /// Scales the transform so the sprite appears at the target world size.
        /// </summary>
        private void ScaleSpriteToTargetSize(Sprite sprite)
        {
            if (sprite == null) return;

            // Calculate the sprite's current world size
            float spriteWorldHeight = sprite.bounds.size.y;
            float spriteWorldWidth = sprite.bounds.size.x;
            float maxDimension = Mathf.Max(spriteWorldWidth, spriteWorldHeight);

            if (maxDimension <= 0) return;

            // Calculate scale factor to reach target size
            float scaleFactor = targetSpriteWorldSize / maxDimension;

            // Apply uniform scale
            transform.localScale = Vector3.one * scaleFactor;

            Debug.Log($"[PlayerAnimator] Scaled sprite from {maxDimension:F2} to {targetSpriteWorldSize:F2} (scale: {scaleFactor:F3})");
        }

        private void Update()
        {
            _animationTime += Time.deltaTime;

            // Handle attack animation
            if (_isAttacking)
            {
                _attackTimer -= Time.deltaTime;
                if (_attackTimer <= 0)
                {
                    _isAttacking = false;
                    UpdateSprite();
                }
                return;
            }

            // Handle walk animation
            if (_isMoving)
            {
                _walkTimer += Time.deltaTime;
                if (_walkTimer >= walkFrameRate)
                {
                    _walkTimer = 0f;
                    AdvanceWalkFrame();
                }
            }

            // Apply scale-based procedural animation (safe for physics)
            if (useScaleAnimation)
            {
                ApplyScaleAnimation();
            }
        }

        private void ApplyScaleAnimation()
        {
            if (_isAttacking)
            {
                // Quick squash during attack
                float attackProgress = _attackTimer / attackDuration;
                float squash = 1f + Mathf.Sin(attackProgress * Mathf.PI) * 0.15f;
                transform.localScale = new Vector3(_baseScale.x * squash, _baseScale.y / squash, _baseScale.z);
                return;
            }

            if (_isMoving)
            {
                // Bouncy squash/stretch while walking
                float bounce = Mathf.Sin(_animationTime * walkSquashSpeed);
                float scaleX = _baseScale.x * (1f + bounce * walkSquashAmount * 0.5f);
                float scaleY = _baseScale.y * (1f - bounce * walkSquashAmount);
                transform.localScale = new Vector3(scaleX, scaleY, _baseScale.z);
            }
            else
            {
                // Gentle breathing when idle
                float breath = Mathf.Sin(_animationTime * idleBreathSpeed);
                float scaleX = _baseScale.x * (1f + breath * idleBreathAmount * 0.3f);
                float scaleY = _baseScale.y * (1f + breath * idleBreathAmount);
                transform.localScale = new Vector3(scaleX, scaleY, _baseScale.z);
            }
        }

        /// <summary>
        /// Sets the character data to load sprites from.
        /// </summary>
        public void SetCharacterData(CharacterData data)
        {
            characterData = data;

            if (data != null)
            {
                // Load sprites from character data
                if (data.idleSprite != null)
                {
                    idleDown = data.idleSprite;
                    idleUp = data.idleSprite;
                    idleLeft = data.idleSprite;
                    idleRight = data.idleSprite;
                }

                if (data.walkSprites != null && data.walkSprites.Length > 0)
                {
                    walkDown = data.walkSprites;
                    walkUp = data.walkSprites;
                    walkLeft = data.walkSprites;
                    walkRight = data.walkSprites;
                }

                if (data.attackSprite != null)
                {
                    attackSprite = data.attackSprite;
                }
            }

            UpdateIdleSprite();
        }

        /// <summary>
        /// Updates movement state and facing direction.
        /// </summary>
        public void SetMoving(bool isMoving, Vector2 direction)
        {
            bool wasMoving = _isMoving;
            _isMoving = isMoving;
            _facingDirection = direction;

            // Update direction enum
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                _currentDirection = direction.x > 0 ? Direction.Right : Direction.Left;
            }
            else
            {
                _currentDirection = direction.y > 0 ? Direction.Up : Direction.Down;
            }

            // Reset walk animation when starting to move
            if (isMoving && !wasMoving)
            {
                _walkTimer = 0f;
                _currentWalkFrame = 0;
            }

            // Update sprite if not attacking
            if (!_isAttacking)
            {
                UpdateSprite();
            }
        }

        /// <summary>
        /// Triggers the attack animation.
        /// </summary>
        public void TriggerAttack()
        {
            _isAttacking = true;
            _attackTimer = attackDuration;

            if (attackSprite != null)
            {
                _spriteRenderer.sprite = attackSprite;
            }

            // Trigger procedural attack animation
            if (_proceduralAnimator != null)
            {
                _proceduralAnimator.PlayAttack(_facingDirection);
            }
        }

        /// <summary>
        /// Triggers the damage flash effect.
        /// </summary>
        public void TriggerDamage()
        {
            // Flash red briefly with procedural animation
            if (_proceduralAnimator != null)
            {
                _proceduralAnimator.PlayDamage();
            }
            else
            {
                // Simple flash effect
                StartCoroutine(DamageFlash());
            }
        }

        private System.Collections.IEnumerator DamageFlash()
        {
            Color originalColor = _spriteRenderer.color;
            float flashDuration = 0.15f;
            float elapsed = 0f;

            while (elapsed < flashDuration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / flashDuration;
                float flash = Mathf.Sin(t * Mathf.PI * 4f) * 0.5f + 0.5f;
                _spriteRenderer.color = Color.Lerp(originalColor, Color.red, flash);
                yield return null;
            }

            _spriteRenderer.color = originalColor;
        }

        private void UpdateSprite()
        {
            if (_isMoving)
            {
                UpdateWalkSprite();
            }
            else
            {
                UpdateIdleSprite();
            }
        }

        private void UpdateIdleSprite()
        {
            Sprite sprite = _currentDirection switch
            {
                Direction.Up => idleUp,
                Direction.Down => idleDown,
                Direction.Left => idleLeft,
                Direction.Right => idleRight,
                _ => idleDown
            };

            if (sprite != null)
            {
                _spriteRenderer.sprite = sprite;
            }

            // Flip sprite for left/right if using same sprite
            _spriteRenderer.flipX = _currentDirection == Direction.Left;
        }

        private void UpdateWalkSprite()
        {
            Sprite[] walkSprites = _currentDirection switch
            {
                Direction.Up => walkUp,
                Direction.Down => walkDown,
                Direction.Left => walkLeft,
                Direction.Right => walkRight,
                _ => walkDown
            };

            if (walkSprites != null && walkSprites.Length > 0)
            {
                _spriteRenderer.sprite = walkSprites[_currentWalkFrame % walkSprites.Length];
            }

            // Flip sprite for left/right if using same sprite
            _spriteRenderer.flipX = _currentDirection == Direction.Left;
        }

        private void AdvanceWalkFrame()
        {
            Sprite[] walkSprites = _currentDirection switch
            {
                Direction.Up => walkUp,
                Direction.Down => walkDown,
                Direction.Left => walkLeft,
                Direction.Right => walkRight,
                _ => walkDown
            };

            if (walkSprites != null && walkSprites.Length > 0)
            {
                _currentWalkFrame = (_currentWalkFrame + 1) % walkSprites.Length;
                _spriteRenderer.sprite = walkSprites[_currentWalkFrame];
            }
        }

        /// <summary>
        /// Sets sprite color (used for damage flash, power-ups, etc.).
        /// </summary>
        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }

        /// <summary>
        /// Resets sprite color to white.
        /// </summary>
        public void ResetColor()
        {
            _spriteRenderer.color = Color.white;
        }

        /// <summary>
        /// Sets sprite visibility.
        /// </summary>
        public void SetVisible(bool visible)
        {
            _spriteRenderer.enabled = visible;
        }
    }
}
