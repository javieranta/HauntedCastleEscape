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

            // Try to get combat component again in Start (in case it was added after Awake)
            if (playerCombat == null)
            {
                playerCombat = GetComponent<PlayerCombat>();
                if (playerCombat == null)
                {
                    Debug.LogWarning("[PlayerController] PlayerCombat still null in Start! Adding component...");
                    playerCombat = gameObject.AddComponent<PlayerCombat>();
                }
            }

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

            Debug.Log($"[PlayerController] Started. CanMove={_canMove}, Position={transform.position}, HasCombat={playerCombat != null}");
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

            // Configure rigidbody for top-down movement - CRITICAL: bodyType MUST be Dynamic for velocity to work
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

            Debug.Log($"[PlayerController] Rigidbody2D: bodyType={rb.bodyType}, isKinematic={rb.isKinematic}");
        }

        // Safety tracking
        private float _lastMoveAttemptTime;
        private int _stuckFrameCount;

        // Track time for aggressive timeScale recovery
        private float _lastTimeScaleResetTime;

        private void Update()
        {
            // CRITICAL SAFETY: Aggressive Time.timeScale reset
            // Uses unscaledDeltaTime to work even when timeScale is 0
            if (Time.timeScale < 0.01f)
            {
                // Track how long we've been at zero timeScale
                if (_lastTimeScaleResetTime == 0)
                {
                    _lastTimeScaleResetTime = Time.unscaledTime;
                }
                else if (Time.unscaledTime - _lastTimeScaleResetTime > 1.5f)
                {
                    // Been frozen for over 1.5 seconds - force reset
                    Debug.LogWarning($"[PlayerController] CRITICAL: Time.timeScale stuck at {Time.timeScale} - forcing reset!");
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                    _lastTimeScaleResetTime = 0;

                    // Also check if game state is paused when it shouldn't be
                    if (GameManager.Instance != null && GameManager.Instance.CurrentState == GameState.Paused)
                    {
                        Debug.LogWarning("[PlayerController] Game was stuck in Paused state - unpausing!");
                        GameManager.Instance.ChangeState(GameState.Playing);
                    }
                }
            }
            else
            {
                _lastTimeScaleResetTime = 0;
            }

            // SAFETY: Detect if CanMove is stuck as false when it shouldn't be
            if (!_canMove && playerHealth != null && !playerHealth.IsDead)
            {
                _stuckFrameCount++;
                // If we've been stuck for 3 seconds (180 frames at 60fps), auto-recover
                if (_stuckFrameCount > 180)
                {
                    Debug.LogWarning("[PlayerController] CanMove was stuck at false, auto-recovering");
                    _canMove = true;
                    _stuckFrameCount = 0;
                }
            }
            else
            {
                _stuckFrameCount = 0;
            }

            // Direct keyboard input fallback (in case Input System callbacks aren't working)
            ReadKeyboardInput();
            HandleInput();

            // DEBUG: Log movement status when pressing movement keys (every second)
            if (_moveInput.sqrMagnitude > 0.01f && Time.frameCount % 60 == 0)
            {
                Debug.Log($"[PlayerController] Update: canMove={_canMove}, timeScale={Time.timeScale}, input={_moveInput}, pos={transform.position}");
            }
        }

        private void ReadKeyboardInput()
        {
            // Read keyboard directly as fallback
            Vector2 input = Vector2.zero;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                    input.y = 1f;
                else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                    input.y = -1f;

                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                    input.x = 1f;
                else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                    input.x = -1f;

                // Attack with space
                if (Keyboard.current.spaceKey.wasPressedThisFrame)
                    _attackPressed = true;

                // Interact with E
                if (Keyboard.current.eKey.wasPressedThisFrame)
                    _interactPressed = true;

                // DEBUG: Press R to force reset movement state
                if (Keyboard.current.rKey.wasPressedThisFrame)
                {
                    Debug.Log("[PlayerController] Manual movement reset triggered (R key)");
                    ForceResetMovement();
                }
            }

            // Normalize diagonal movement
            if (input.sqrMagnitude > 1f)
                input = input.normalized;

            // Use this input if we have any
            if (input.sqrMagnitude > 0.01f)
                _moveInput = input;
            else if (Keyboard.current != null && !Keyboard.current.anyKey.isPressed)
                _moveInput = Vector2.zero; // Clear input when no keys pressed
        }

        /// <summary>
        /// Force resets the movement state. Use for debugging or recovery.
        /// </summary>
        public void ForceResetMovement()
        {
            _canMove = true;
            _velocity = Vector2.zero;
            rb.velocity = Vector2.zero;
            Time.timeScale = 1f;

            // Also ensure Rigidbody2D is properly configured
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

            // Try to unstick from walls by moving slightly
            TryUnstickFromWalls();

            Debug.Log($"[PlayerController] Movement force reset: canMove={_canMove}, bodyType={rb.bodyType}, simulated={rb.simulated}");
        }

        /// <summary>
        /// Attempts to move player out of any walls they might be stuck in.
        /// </summary>
        private void TryUnstickFromWalls()
        {
            // Check if player is overlapping with any colliders
            Collider2D[] overlaps = Physics2D.OverlapCircleAll(transform.position, 0.3f);

            foreach (var overlap in overlaps)
            {
                // Skip the player's own collider
                if (overlap.gameObject == gameObject) continue;

                // Skip triggers
                if (overlap.isTrigger) continue;

                // Found a solid collider we're overlapping with - try to push out
                Vector2 pushDirection = (Vector2)(transform.position - overlap.transform.position).normalized;
                if (pushDirection.sqrMagnitude < 0.01f)
                {
                    pushDirection = Vector2.up; // Default push direction
                }

                // Move player slightly in push direction
                transform.position += (Vector3)(pushDirection * 0.5f);
                Debug.Log($"[PlayerController] Unstuck player from {overlap.name}, pushed {pushDirection * 0.5f}");
            }
        }

        // Track for position-stuck detection
        private Vector3 _lastPosition;
        private int _samePositionFrames;

        private void FixedUpdate()
        {
            // Detect if player is physically stuck (position not changing despite input)
            if (_moveInput.sqrMagnitude > 0.01f && _canMove)
            {
                float positionDelta = Vector3.Distance(transform.position, _lastPosition);
                if (positionDelta < 0.001f)
                {
                    _samePositionFrames++;
                    // If stuck for 2 seconds (100 fixed frames at 50hz), try to auto-recover
                    if (_samePositionFrames > 100)
                    {
                        if (_samePositionFrames % 50 == 0)
                        {
                            Debug.LogWarning($"[PlayerController] Player appears stuck! pos={transform.position}, input={_moveInput}, vel={rb.velocity}");
                        }

                        // After 3 seconds of being stuck, try auto-recovery
                        if (_samePositionFrames == 150)
                        {
                            Debug.LogWarning("[PlayerController] Auto-recovering from stuck state...");
                            ForceResetMovement();
                        }
                    }
                }
                else
                {
                    _samePositionFrames = 0;
                }
            }
            else
            {
                _samePositionFrames = 0;
            }
            _lastPosition = transform.position;

            if (_canMove)
            {
                ApplyMovement();
            }
            else
            {
                ApplyFriction();
            }

            // Debug: Log position every second when moving
            if (IsMoving && Time.frameCount % 60 == 0)
            {
                Debug.Log($"[PlayerController] FixedUpdate: pos={transform.position}, vel={rb.velocity}, input={_moveInput}");
            }
        }

        private void HandleInput()
        {
            // Attack input (handled in Update for responsiveness)
            if (_attackPressed)
            {
                Debug.Log($"[PlayerController] Attack pressed! playerCombat={playerCombat != null}, facingDirection={_facingDirection}");

                if (playerCombat != null)
                {
                    bool attackResult = playerCombat.TryAttack(_facingDirection);
                    Debug.Log($"[PlayerController] Attack result: {attackResult}");
                }
                else
                {
                    // Fallback: try to find or create PlayerCombat
                    playerCombat = GetComponent<PlayerCombat>();
                    if (playerCombat == null)
                    {
                        Debug.LogWarning("[PlayerController] PlayerCombat component missing! Creating fallback attack...");
                        CreateFallbackAttack(_facingDirection);
                    }
                    else
                    {
                        playerCombat.TryAttack(_facingDirection);
                    }
                }
                _attackPressed = false;
            }

            // Interact input
            if (_interactPressed)
            {
                TryInteract();
                _interactPressed = false;
            }
        }

        /// <summary>
        /// Creates a simple fallback projectile attack when PlayerCombat is missing.
        /// </summary>
        private void CreateFallbackAttack(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.01f)
                direction = Vector2.right;

            Vector3 spawnPos = transform.position + (Vector3)direction * 0.5f;

            var projObj = new GameObject("FallbackProjectile");
            projObj.transform.position = spawnPos;

            var rb = projObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.velocity = direction.normalized * 8f;

            var col = projObj.AddComponent<CircleCollider2D>();
            col.radius = 0.2f;
            col.isTrigger = true;

            var sr = projObj.AddComponent<SpriteRenderer>();
            sr.color = Color.yellow;
            // Create simple circle sprite
            var tex = new Texture2D(16, 16);
            for (int x = 0; x < 16; x++)
                for (int y = 0; y < 16; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(8, 8));
                    tex.SetPixel(x, y, dist < 6 ? Color.yellow : Color.clear);
                }
            tex.Apply();
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            sr.sortingOrder = 100;

            // Destroy after 2 seconds
            Destroy(projObj, 2f);

            Debug.Log($"[PlayerController] Created fallback projectile at {spawnPos} going {direction}");
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

            // Debug: Log when velocity is set
            if (_velocity.sqrMagnitude > 0.01f && Time.frameCount % 30 == 0)
            {
                Debug.Log($"[PlayerController] ApplyMovement: setting rb.velocity={_velocity}, actual rb.velocity={rb.velocity}");
            }

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

                // IMPORTANT: Ensure movement is enabled after room transition
                _canMove = true;

                Debug.Log($"[PlayerController] OnRoomLoaded: pos={spawnPos}, room={roomData?.roomId}, canMove={_canMove}");
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
