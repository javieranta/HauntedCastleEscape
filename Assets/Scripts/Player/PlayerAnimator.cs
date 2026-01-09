using UnityEngine;
using HauntedCastle.Data;

namespace HauntedCastle.Player
{
    /// <summary>
    /// Handles player sprite animation.
    /// Supports 4-directional movement and attack animations.
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

        [Header("Character Data")]
        [SerializeField] private CharacterData characterData;

        private SpriteRenderer _spriteRenderer;
        private bool _isMoving;
        private Vector2 _facingDirection = Vector2.down;
        private float _walkTimer;
        private int _currentWalkFrame;
        private bool _isAttacking;
        private float _attackTimer;

        // Direction tracking
        private enum Direction { Down, Up, Left, Right }
        private Direction _currentDirection = Direction.Down;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            UpdateIdleSprite();
        }

        private void Update()
        {
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
        }

        /// <summary>
        /// Triggers the damage flash effect.
        /// </summary>
        public void TriggerDamage()
        {
            // Flash red briefly - handled by PlayerHealth
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
