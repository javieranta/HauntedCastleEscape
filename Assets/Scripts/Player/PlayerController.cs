using UnityEngine;
using UnityEngine.InputSystem;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Player
{
    /// <summary>
    /// Main player controller handling movement, input, and character-specific behavior.
    /// Uses Unity's new Input System for flexible control schemes.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Character Configuration")]
        [SerializeField] private CharacterData characterData;

        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerCombat playerCombat;
        [SerializeField] private PlayerAnimator playerAnimator;

        [Header("Movement Override")]
        [SerializeField] private float moveSpeedOverride = 0f;
        [SerializeField] private float accelerationOverride = 0f;

        // Input state
        private Vector2 _moveInput;
        private bool _attackPressed;
        private bool _interactPressed;

        // Movement state
        private Vector2 _velocity;
        private Vector2 _facingDirection = Vector2.down;
        private bool _canMove = true;

        // Properties
        public CharacterData CharacterData => characterData;
        public Vector2 FacingDirection => _facingDirection;
        public Vector2 Velocity => _velocity;
        public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;
        public bool CanMove
        {
            get => _canMove;
            set => _canMove = value;
        }

        // Computed stats
        private float MoveSpeed => moveSpeedOverride > 0 ? moveSpeedOverride : (characterData?.moveSpeed ?? 5f);
        private float Acceleration => accelerationOverride > 0 ? accelerationOverride : (characterData?.acceleration ?? 20f);
        private float Friction => characterData?.friction ?? 0.9f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeComponents();
        }

        private void Start()
        {
            // Ensure CanMove is true on start
            _canMove = true;

            // Load character data from GameManager selection
            if (GameManager.Instance != null && characterData == null)
            {
                // Character data would be loaded from resources based on selected type
                Debug.Log($"[PlayerController] Using character: {GameManager.Instance.SelectedCharacter}");
            }

            // Subscribe to room load events for repositioning
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomLoaded;
            }

            Debug.Log($"[PlayerController] Started. CanMove={_canMove}, Position={transform.position}");
        }

        private void OnDestroy()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomLoaded;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void InitializeComponents()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (col == null) col = GetComponent<Collider2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
            if (playerCombat == null) playerCombat = GetComponent<PlayerCombat>();
            if (playerAnimator == null) playerAnimator = GetComponent<PlayerAnimator>();

            // Configure rigidbody for top-down movement
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            if (_canMove)
            {
                ApplyMovement();
            }
            else
            {
                ApplyFriction();
            }
        }

        private void HandleInput()
        {
            // Attack input (handled in Update for responsiveness)
            if (_attackPressed && playerCombat != null)
            {
                playerCombat.TryAttack(_facingDirection);
                _attackPressed = false;
            }

            // Interact input
            if (_interactPressed)
            {
                TryInteract();
                _interactPressed = false;
            }
        }

        private void ApplyMovement()
        {
            if (_moveInput.sqrMagnitude > 0.01f)
            {
                // Update facing direction
                if (Mathf.Abs(_moveInput.x) > Mathf.Abs(_moveInput.y))
                {
                    _facingDirection = _moveInput.x > 0 ? Vector2.right : Vector2.left;
                }
                else
                {
                    _facingDirection = _moveInput.y > 0 ? Vector2.up : Vector2.down;
                }

                // Accelerate towards target velocity
                Vector2 targetVelocity = _moveInput.normalized * MoveSpeed;
                _velocity = Vector2.MoveTowards(_velocity, targetVelocity, Acceleration * Time.fixedDeltaTime);

                // Drain energy while moving (if enabled)
                if (playerHealth != null)
                {
                    playerHealth.DrainEnergyFromMovement(Time.fixedDeltaTime);
                }
            }
            else
            {
                ApplyFriction();
            }

            rb.velocity = _velocity;

            // Update animator
            if (playerAnimator != null)
            {
                playerAnimator.SetMoving(IsMoving, _facingDirection);
            }
        }

        private void ApplyFriction()
        {
            _velocity *= Friction;
            if (_velocity.sqrMagnitude < 0.01f)
            {
                _velocity = Vector2.zero;
            }
            rb.velocity = _velocity;
        }

        private void TryInteract()
        {
            // Cast a short ray in facing direction to find interactables
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                _facingDirection,
                1.5f,
                LayerMask.GetMask("Items", "Doors")
            );

            if (hit.collider != null)
            {
                // Try to interact with item
                var item = hit.collider.GetComponent<Items.ItemPickup>();
                if (item != null)
                {
                    item.TryPickup(this);
                    return;
                }

                // Other interactables can be added here
            }
        }

        /// <summary>
        /// Called when a new room is loaded to reposition the player.
        /// </summary>
        private void OnRoomLoaded(RoomData roomData)
        {
            if (RoomManager.Instance != null)
            {
                Vector2 spawnPos = RoomManager.Instance.GetPendingSpawnPosition();
                transform.position = spawnPos;
                _velocity = Vector2.zero;
                rb.velocity = Vector2.zero;
            }
        }

        /// <summary>
        /// Teleports the player to a position.
        /// </summary>
        public void SetPosition(Vector2 position)
        {
            transform.position = position;
            _velocity = Vector2.zero;
            rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Applies knockback force to the player.
        /// </summary>
        public void ApplyKnockback(Vector2 direction, float force)
        {
            _velocity = direction.normalized * force;
            rb.velocity = _velocity;
        }

        /// <summary>
        /// Checks if this character can use a specific secret passage type.
        /// </summary>
        public bool CanUseSecretPassage(SecretPassageType passageType)
        {
            if (characterData == null) return false;
            return characterData.accessiblePassageType == passageType;
        }

        /// <summary>
        /// Sets the character data (used when selecting character).
        /// </summary>
        public void SetCharacterData(CharacterData data)
        {
            characterData = data;

            if (playerCombat != null)
            {
                playerCombat.SetCharacterData(data);
            }

            if (playerAnimator != null)
            {
                playerAnimator.SetCharacterData(data);
            }

            if (spriteRenderer != null && data.idleSprite != null)
            {
                spriteRenderer.sprite = data.idleSprite;
            }
        }

        #region Input System Callbacks

        /// <summary>
        /// Called by Input System for movement input.
        /// </summary>
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();

            // Debug log to verify input is being received
            if (_moveInput.sqrMagnitude > 0.01f)
            {
                Debug.Log($"[PlayerController] Move input: {_moveInput}, CanMove={_canMove}");
            }
        }

        /// <summary>
        /// Called by Input System for attack input.
        /// </summary>
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _attackPressed = true;
            }
        }

        /// <summary>
        /// Called by Input System for interact input.
        /// </summary>
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _interactPressed = true;
            }
        }

        /// <summary>
        /// Called by Input System for pause input.
        /// </summary>
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                // Toggle pause
                if (GameManager.Instance != null)
                {
                    if (GameManager.Instance.CurrentState == GameState.Playing)
                    {
                        GameManager.Instance.ChangeState(GameState.Paused);
                        Time.timeScale = 0f;
                    }
                    else if (GameManager.Instance.CurrentState == GameState.Paused)
                    {
                        GameManager.Instance.ChangeState(GameState.Playing);
                        Time.timeScale = 1f;
                    }
                }
            }
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            // Draw facing direction
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)_facingDirection);

            // Draw interaction range
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + (Vector3)_facingDirection * 0.75f, 0.5f);
        }
    }
}
