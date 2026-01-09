using System.Collections.Generic;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Items
{
    /// <summary>
    /// Handles spawning items in rooms based on RoomData configuration.
    /// Tracks persistent items (key pieces) across room visits.
    /// </summary>
    public class ItemSpawner : MonoBehaviour
    {
        public static ItemSpawner Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private Transform itemContainer;

        [Header("Tracking")]
        [SerializeField] private bool trackPersistentItems = true;

        // Track which items have been collected (persists across room visits)
        private HashSet<string> _collectedItemIds = new();

        // Current room's spawned items
        private List<GameObject> _currentRoomItems = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Subscribe to room changes
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadStarted += OnRoomUnloading;
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomLoaded;
            }

            // Ensure container exists
            if (itemContainer == null)
            {
                var containerObj = GameObject.Find("ItemContainer");
                if (containerObj == null)
                {
                    containerObj = new GameObject("ItemContainer");
                }
                itemContainer = containerObj.transform;
            }
        }

        private void OnDestroy()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadStarted -= OnRoomUnloading;
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomLoaded;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void OnRoomUnloading(string roomId)
        {
            // Clear current room items
            ClearCurrentRoomItems();
        }

        private void OnRoomLoaded(RoomData roomData)
        {
            if (roomData == null) return;

            // Spawn items for the new room
            SpawnRoomItems(roomData);
        }

        /// <summary>
        /// Spawns all items defined in the room data.
        /// </summary>
        public void SpawnRoomItems(RoomData roomData)
        {
            if (roomData.itemSpawns == null) return;

            foreach (var itemSpawn in roomData.itemSpawns)
            {
                SpawnItem(itemSpawn, roomData.roomId);
            }

            Debug.Log($"[ItemSpawner] Spawned {_currentRoomItems.Count} items in {roomData.roomId}");
        }

        private void SpawnItem(ItemSpawn spawn, string roomId)
        {
            if (spawn.itemData == null)
            {
                Debug.LogWarning("[ItemSpawner] ItemSpawn has null itemData");
                return;
            }

            // Generate unique ID for tracking
            string uniqueId = $"{roomId}_{spawn.itemData.itemId}_{spawn.position.x}_{spawn.position.y}";

            // Skip if already collected (for persistent items)
            if (spawn.persistsAcrossVisits && _collectedItemIds.Contains(uniqueId))
            {
                return;
            }

            // Calculate position (with optional randomization)
            Vector2 position = spawn.position;
            if (spawn.randomizePosition)
            {
                position += Random.insideUnitCircle * 1f;
            }

            // Create item
            GameObject itemObj = CreateItemObject(spawn.itemData, position);

            // Track for cleanup
            _currentRoomItems.Add(itemObj);

            // Setup pickup tracking
            var pickup = itemObj.GetComponent<ItemPickup>();
            if (pickup != null && spawn.persistsAcrossVisits)
            {
                // Track when collected
                // This is handled by marking the ID when pickup occurs
                itemObj.name = $"Item_{uniqueId}";
            }
        }

        /// <summary>
        /// Spawns a single item at a position.
        /// </summary>
        public GameObject SpawnItemAt(ItemData itemData, Vector2 position)
        {
            var itemObj = CreateItemObject(itemData, position);
            _currentRoomItems.Add(itemObj);
            return itemObj;
        }

        /// <summary>
        /// Spawns an item by ID at a position.
        /// </summary>
        public GameObject SpawnItemAt(string itemId, Vector2 position)
        {
            var itemData = ItemDatabase.GetItem(itemId);
            if (itemData == null)
            {
                Debug.LogWarning($"[ItemSpawner] Unknown item ID: {itemId}");
                return null;
            }

            return SpawnItemAt(itemData, position);
        }

        private GameObject CreateItemObject(ItemData itemData, Vector2 position)
        {
            var itemObj = new GameObject($"Item_{itemData.displayName}");
            itemObj.transform.SetParent(itemContainer);
            itemObj.transform.position = position;

            // Add pickup component
            var pickup = itemObj.AddComponent<ItemPickup>();
            pickup.Initialize(itemData);

            // Add collider
            var collider = itemObj.AddComponent<CircleCollider2D>();
            collider.radius = 0.4f;
            collider.isTrigger = true;

            return itemObj;
        }

        /// <summary>
        /// Marks an item as collected (for persistent tracking).
        /// </summary>
        public void MarkItemCollected(string uniqueId)
        {
            if (trackPersistentItems)
            {
                _collectedItemIds.Add(uniqueId);
            }
        }

        /// <summary>
        /// Clears all current room items.
        /// </summary>
        public void ClearCurrentRoomItems()
        {
            foreach (var item in _currentRoomItems)
            {
                if (item != null)
                {
                    Destroy(item);
                }
            }
            _currentRoomItems.Clear();
        }

        /// <summary>
        /// Resets collected item tracking (for new game).
        /// </summary>
        public void ResetCollectedItems()
        {
            _collectedItemIds.Clear();
        }

        /// <summary>
        /// Gets count of items in current room.
        /// </summary>
        public int GetCurrentItemCount()
        {
            return _currentRoomItems.Count;
        }
    }
}
