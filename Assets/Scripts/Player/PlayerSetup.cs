using UnityEngine;
using UnityEngine.InputSystem;
using HauntedCastle.Core.GameState;
using HauntedCastle.Data;
using HauntedCastle.Services;
using HauntedCastle.Utils;

namespace HauntedCastle.Player
{
    /// <summary>
    /// Sets up the player GameObject with all required components.
    /// Handles initialization and character data assignment.
    /// </summary>
    public class PlayerSetup : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private CharacterType defaultCharacter = CharacterType.Wizard;

        [Header("Component References")]
        [SerializeField] private PlayerController playerController;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private PlayerCombat playerCombat;
        [SerializeField] private PlayerAnimator playerAnimator;
        [SerializeField] private PlayerInputHandler inputHandler;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;

        private CharacterData _currentCharacter;
        private bool _initialized;

        private void Awake()
        {
            // Get or add required components
            EnsureComponents();
        }

        private void Start()
        {
            if (autoInitialize)
            {
                // Get character from GameSession if available, otherwise use default
                CharacterType charType = GameSession.SelectedCharacter != CharacterType.Wizard
                    ? GameSession.SelectedCharacter
                    : defaultCharacter;

                Initialize(charType);
            }
        }

        /// <summary>
        /// Ensures all required components exist on this GameObject.
        /// </summary>
        private void EnsureComponents()
        {
            // Core components
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

            if (col == null) col = GetComponent<Collider2D>();
            if (col == null)
            {
                var boxCol = gameObject.AddComponent<BoxCollider2D>();
                boxCol.size = new Vector2(0.8f, 0.8f);
                col = boxCol;
            }

            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            // Player components
            if (playerController == null) playerController = GetComponent<PlayerController>();
            if (playerController == null) playerController = gameObject.AddComponent<PlayerController>();

            if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null) playerHealth = gameObject.AddComponent<PlayerHealth>();

            if (playerCombat == null) playerCombat = GetComponent<PlayerCombat>();
            if (playerCombat == null) playerCombat = gameObject.AddComponent<PlayerCombat>();

            if (playerAnimator == null) playerAnimator = GetComponent<PlayerAnimator>();
            if (playerAnimator == null) playerAnimator = gameObject.AddComponent<PlayerAnimator>();

            // Input handling
            var playerInput = GetComponent<PlayerInput>();
            if (playerInput == null)
            {
                playerInput = gameObject.AddComponent<PlayerInput>();
                // PlayerInput will be configured via InputActions asset
            }

            if (inputHandler == null) inputHandler = GetComponent<PlayerInputHandler>();
            if (inputHandler == null) inputHandler = gameObject.AddComponent<PlayerInputHandler>();

            // Configure Rigidbody2D
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            // Configure sprite renderer
            spriteRenderer.sortingLayerName = "Player";
            spriteRenderer.sortingOrder = 0;

            // Set tag and layer
            gameObject.tag = "Player";
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        /// <summary>
        /// Initializes the player with the specified character type.
        /// </summary>
        public void Initialize(CharacterType characterType)
        {
            if (_initialized)
            {
                Debug.LogWarning("[PlayerSetup] Already initialized");
                return;
            }

            _currentCharacter = CharacterDatabase.GetCharacter(characterType);

            if (_currentCharacter == null)
            {
                Debug.LogError($"[PlayerSetup] Could not find character data for {characterType}");
                return;
            }

            // Configure player controller
            if (playerController != null)
            {
                playerController.SetCharacterData(_currentCharacter);
            }

            // Configure combat
            if (playerCombat != null)
            {
                playerCombat.SetCharacterData(_currentCharacter);
            }

            // Configure animator
            if (playerAnimator != null)
            {
                playerAnimator.SetCharacterData(_currentCharacter);
            }

            // Set visual appearance - use character-specific placeholder shapes
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white; // Let the sprite color show through
                spriteRenderer.sprite = PlaceholderSpriteGenerator.GetCharacterSprite(characterType);
            }

            _initialized = true;

            Debug.Log($"[PlayerSetup] Initialized as {_currentCharacter.characterName}");
        }

        /// <summary>
        /// Changes the player's character at runtime.
        /// </summary>
        public void ChangeCharacter(CharacterType newCharacterType)
        {
            _initialized = false;
            Initialize(newCharacterType);
        }

        /// <summary>
        /// Gets the current character data.
        /// </summary>
        public CharacterData CurrentCharacter => _currentCharacter;

        /// <summary>
        /// Static factory method to create a fully configured player.
        /// </summary>
        public static GameObject CreatePlayer(CharacterType characterType, Vector3 position)
        {
            GameObject playerObj = new GameObject("Player");
            playerObj.transform.position = position;

            var setup = playerObj.AddComponent<PlayerSetup>();
            setup.autoInitialize = false;

            // Force component creation
            setup.EnsureComponents();

            // Initialize with character
            setup.Initialize(characterType);

            return playerObj;
        }
    }
}
