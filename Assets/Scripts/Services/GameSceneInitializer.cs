using UnityEngine;
using HauntedCastle.UI;
using HauntedCastle.Inventory;
using HauntedCastle.Items;
using HauntedCastle.Enemies;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Initializes the Game scene with required managers and starts gameplay.
    /// Attach to a GameObject in the Game scene.
    /// </summary>
    public class GameSceneInitializer : MonoBehaviour
    {
        [Header("Manager Prefabs")]
        [SerializeField] private GameObject roomManagerPrefab;
        [SerializeField] private GameObject transitionManagerPrefab;
        [SerializeField] private GameObject playerSpawnerPrefab;

        [Header("Scene References")]
        [SerializeField] private Transform roomContainer;
        [SerializeField] private Camera mainCamera;

        private void Awake()
        {
            EnsureManagers();
        }

        private void Start()
        {
            InitializeGame();
        }

        private void EnsureManagers()
        {
            // Ensure GameManager exists
            if (GameManager.Instance == null)
            {
                var gmObj = new GameObject("GameManager");
                gmObj.AddComponent<GameManager>();
            }

            // Ensure TransitionManager exists
            if (TransitionManager.Instance == null)
            {
                if (transitionManagerPrefab != null)
                {
                    Instantiate(transitionManagerPrefab);
                }
                else
                {
                    var tmObj = new GameObject("TransitionManager");
                    tmObj.AddComponent<TransitionManager>();
                }
            }

            // Ensure RoomManager exists
            if (RoomManager.Instance == null)
            {
                if (roomManagerPrefab != null)
                {
                    Instantiate(roomManagerPrefab);
                }
                else
                {
                    var rmObj = new GameObject("RoomManager");
                    rmObj.AddComponent<RoomManager>();
                }
            }

            // Ensure PlayerSpawner exists
            if (PlayerSpawner.Instance == null)
            {
                if (playerSpawnerPrefab != null)
                {
                    Instantiate(playerSpawnerPrefab);
                }
                else
                {
                    var psObj = new GameObject("PlayerSpawner");
                    psObj.AddComponent<PlayerSpawner>();
                }
            }

            // Ensure PlayerInventory exists
            if (PlayerInventory.Instance == null)
            {
                var invObj = new GameObject("PlayerInventory");
                invObj.AddComponent<PlayerInventory>();
            }

            // Ensure ItemSpawner exists
            if (ItemSpawner.Instance == null)
            {
                var isObj = new GameObject("ItemSpawner");
                isObj.AddComponent<ItemSpawner>();
            }

            // Ensure EnemySpawner exists
            if (EnemySpawner.Instance == null)
            {
                var esObj = new GameObject("EnemySpawner");
                esObj.AddComponent<EnemySpawner>();
            }

            // Ensure PlayerHUD exists
            if (FindFirstObjectByType<PlayerHUD>() == null)
            {
                var hudObj = new GameObject("PlayerHUD");
                hudObj.AddComponent<PlayerHUD>();
            }

            // Ensure InventoryUI exists
            if (FindFirstObjectByType<InventoryUI>() == null)
            {
                var invUIObj = new GameObject("InventoryUI");
                invUIObj.AddComponent<InventoryUI>();
            }

            // Setup room container
            if (roomContainer == null)
            {
                var containerObj = GameObject.Find("RoomContainer");
                if (containerObj == null)
                {
                    containerObj = new GameObject("RoomContainer");
                }
                roomContainer = containerObj.transform;
            }

            // Setup camera
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            if (mainCamera != null && mainCamera.GetComponent<PixelPerfectSetup>() == null)
            {
                mainCamera.gameObject.AddComponent<PixelPerfectSetup>();
            }
        }

        private void InitializeGame()
        {
            // Update game state
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.Playing);
            }

            // Load starting room
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.LoadStartingRoom();
            }

            Debug.Log("[GameSceneInitializer] Game initialized");
        }
    }
}
