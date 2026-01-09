using UnityEngine;
using UnityEngine.InputSystem;

namespace HauntedCastle.Player
{
    /// <summary>
    /// Bridges Unity Input System with PlayerController.
    /// Handles input action callbacks and routes them to the player.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerCombat playerCombat;

        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private InputAction _attackAction;
        private InputAction _interactAction;
        private InputAction _pauseAction;
        private InputAction _dropItemAction;

        private bool _isInitialized = false;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

            if (playerController == null)
            {
                playerController = GetComponent<PlayerController>();
            }

            if (playerCombat == null)
            {
                playerCombat = GetComponent<PlayerCombat>();
            }

            // Don't setup input actions here - wait for Start when PlayerInput is fully configured
        }

        private void Start()
        {
            // Setup input actions in Start, after PlayerSetup has configured the PlayerInput component
            SetupInputActions();
        }

        private void OnEnable()
        {
            // Only enable if already initialized
            if (_isInitialized)
            {
                EnableInput();
            }
        }

        private void OnDisable()
        {
            DisableInput();
        }

        private void SetupInputActions()
        {
            // Check if PlayerInput and actions are properly configured
            if (_playerInput == null)
            {
                _playerInput = GetComponent<PlayerInput>();
            }

            if (_playerInput == null || _playerInput.actions == null)
            {
                Debug.LogWarning("[PlayerInputHandler] PlayerInput or actions not configured yet, will retry");
                StartCoroutine(RetrySetupInputActions());
                return;
            }

            var actionMap = _playerInput.actions.FindActionMap("Player");

            if (actionMap != null)
            {
                // Enable the action map first!
                actionMap.Enable();

                _moveAction = actionMap.FindAction("Move");
                _attackAction = actionMap.FindAction("Attack");
                _interactAction = actionMap.FindAction("Interact");
                _pauseAction = actionMap.FindAction("Pause");
                _dropItemAction = actionMap.FindAction("DropItem");

                Debug.Log($"[PlayerInputHandler] Action map 'Player' enabled. Move action: {_moveAction != null}");
            }
            else
            {
                Debug.LogWarning("[PlayerInputHandler] Could not find 'Player' action map");
            }

            _isInitialized = true;
            EnableInput();
            Debug.Log("[PlayerInputHandler] Input actions initialized successfully");
        }

        private System.Collections.IEnumerator RetrySetupInputActions()
        {
            // Wait a few frames for PlayerSetup to configure the PlayerInput component
            for (int i = 0; i < 10; i++)
            {
                yield return null;

                if (_playerInput != null && _playerInput.actions != null)
                {
                    SetupInputActions();
                    yield break;
                }
            }

            Debug.LogError("[PlayerInputHandler] Failed to setup input actions after retries - PlayerInput not configured!");
        }

        private void EnableInput()
        {
            if (_moveAction != null)
            {
                _moveAction.performed += OnMovePerformed;
                _moveAction.canceled += OnMoveCanceled;
            }

            if (_attackAction != null)
            {
                _attackAction.performed += OnAttackPerformed;
            }

            if (_interactAction != null)
            {
                _interactAction.performed += OnInteractPerformed;
            }

            if (_pauseAction != null)
            {
                _pauseAction.performed += OnPausePerformed;
            }

            if (_dropItemAction != null)
            {
                _dropItemAction.performed += OnDropItemPerformed;
            }
        }

        private void DisableInput()
        {
            if (_moveAction != null)
            {
                _moveAction.performed -= OnMovePerformed;
                _moveAction.canceled -= OnMoveCanceled;
            }

            if (_attackAction != null)
            {
                _attackAction.performed -= OnAttackPerformed;
            }

            if (_interactAction != null)
            {
                _interactAction.performed -= OnInteractPerformed;
            }

            if (_pauseAction != null)
            {
                _pauseAction.performed -= OnPausePerformed;
            }

            if (_dropItemAction != null)
            {
                _dropItemAction.performed -= OnDropItemPerformed;
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.OnMove(context);
            }
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.OnMove(context);
            }
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.OnAttack(context);
            }
        }

        private void OnInteractPerformed(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.OnInteract(context);
            }
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            if (playerController != null)
            {
                playerController.OnPause(context);
            }
        }

        private void OnDropItemPerformed(InputAction.CallbackContext context)
        {
            // Will be handled by inventory system in Milestone 3
            Debug.Log("[PlayerInputHandler] Drop item pressed");
        }

        /// <summary>
        /// Switches to a different control scheme.
        /// </summary>
        public void SwitchControlScheme(string schemeName)
        {
            if (_playerInput != null)
            {
                _playerInput.SwitchCurrentControlScheme(schemeName);
                Debug.Log($"[PlayerInputHandler] Switched to control scheme: {schemeName}");
            }
        }

        /// <summary>
        /// Gets the current control scheme name.
        /// </summary>
        public string GetCurrentControlScheme()
        {
            return _playerInput?.currentControlScheme ?? "Unknown";
        }

        /// <summary>
        /// Enables or disables all player input.
        /// </summary>
        public void SetInputEnabled(bool enabled)
        {
            if (enabled)
            {
                _playerInput?.ActivateInput();
            }
            else
            {
                _playerInput?.DeactivateInput();
            }
        }
    }
}
