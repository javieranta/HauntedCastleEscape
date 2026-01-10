using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HauntedCastle.Data;

namespace HauntedCastle.Inventory
{
    /// <summary>
    /// Manages player's 3-slot inventory system.
    /// Handles item pickup, dropping, and usage.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Instance { get; private set; }

        [Header("Configuration")]
        [SerializeField] private int maxSlots = 3;

        [Header("Key Pieces")]
        [SerializeField] private bool[] keyPiecesCollected = new bool[3];

        // Current inventory items
        private List<ItemData> _items = new();

        // Events
        public event Action<int, ItemData> OnItemAdded;          // slot, item
        public event Action<int, ItemData> OnItemRemoved;        // slot, item
        public event Action<int, ItemData> OnItemUsed;           // slot, item
        public event Action<ItemData> OnItemDropped;
        public event Action<int> OnKeyPieceCollected;            // piece index (0-2)
        public event Action OnGreatKeyFormed;
        public event Action OnInventoryChanged;

        // Properties
        public int MaxSlots => maxSlots;
        public int ItemCount => _items.Count;
        public bool IsFull => _items.Count >= maxSlots;
        public IReadOnlyList<ItemData> Items => _items.AsReadOnly();
        public bool HasGreatKey { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>
        /// Attempts to add an item to the inventory.
        /// </summary>
        public bool TryAddItem(ItemData item)
        {
            if (item == null) return false;

            // Special handling for key pieces - they don't take inventory slots
            if (item.itemType == ItemType.KeyPiece)
            {
                return CollectKeyPiece(item);
            }

            // Check if inventory is full
            if (IsFull)
            {
                Debug.Log("[PlayerInventory] Inventory full!");
                return false;
            }

            // Add item
            _items.Add(item);
            int slot = _items.Count - 1;

            OnItemAdded?.Invoke(slot, item);
            OnInventoryChanged?.Invoke();

            Debug.Log($"[PlayerInventory] Added {item.displayName} to slot {slot}");
            return true;
        }

        /// <summary>
        /// Removes an item from a specific slot.
        /// </summary>
        public ItemData RemoveItem(int slot)
        {
            if (slot < 0 || slot >= _items.Count)
            {
                return null;
            }

            ItemData item = _items[slot];
            _items.RemoveAt(slot);

            OnItemRemoved?.Invoke(slot, item);
            OnInventoryChanged?.Invoke();

            Debug.Log($"[PlayerInventory] Removed {item.displayName} from slot {slot}");
            return item;
        }

        /// <summary>
        /// Uses an item from a specific slot.
        /// </summary>
        public bool UseItem(int slot)
        {
            if (slot < 0 || slot >= _items.Count)
            {
                return false;
            }

            ItemData item = _items[slot];

            // Apply item effects
            bool used = ApplyItemEffect(item);

            if (used)
            {
                OnItemUsed?.Invoke(slot, item);

                // Remove if consumable
                if (item.consumeOnUse)
                {
                    RemoveItem(slot);
                }
            }

            return used;
        }

        /// <summary>
        /// Drops an item at the player's position.
        /// </summary>
        public bool DropItem(int slot, Vector2 position)
        {
            ItemData item = RemoveItem(slot);
            if (item == null) return false;

            // Create dropped item in world
            CreateDroppedItem(item, position);

            OnItemDropped?.Invoke(item);
            return true;
        }

        /// <summary>
        /// Gets item at a specific slot.
        /// </summary>
        public ItemData GetItem(int slot)
        {
            if (slot < 0 || slot >= _items.Count)
            {
                return null;
            }
            return _items[slot];
        }

        /// <summary>
        /// Checks if the player has a specific key color.
        /// </summary>
        public bool HasKey(KeyColor keyColor)
        {
            return _items.Any(item =>
                item.itemType == ItemType.Key &&
                item.keyColor == keyColor);
        }

        /// <summary>
        /// Tries to use a key of the specified color.
        /// </summary>
        public bool TryUseKey(KeyColor keyColor)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].itemType == ItemType.Key &&
                    _items[i].keyColor == keyColor)
                {
                    // Keys are consumed when used
                    UseItem(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the player has a specific item by ID.
        /// </summary>
        public bool HasItem(string itemId)
        {
            return _items.Any(item => item.itemId == itemId);
        }

        /// <summary>
        /// Gets item IDs as a list (for door system compatibility).
        /// </summary>
        public List<string> GetItemIds()
        {
            return _items.Select(item => item.itemId).ToList();
        }

        /// <summary>
        /// Gets all keys owned by the player.
        /// </summary>
        public List<KeyColor> GetOwnedKeys()
        {
            return _items
                .Where(item => item.itemType == ItemType.Key)
                .Select(item => item.keyColor)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Checks if the player has all three key pieces.
        /// </summary>
        public bool HasAllKeyPieces()
        {
            return keyPiecesCollected[0] && keyPiecesCollected[1] && keyPiecesCollected[2];
        }

        /// <summary>
        /// Gets the count of collected key pieces.
        /// </summary>
        public int GetKeyPieceCount()
        {
            return keyPiecesCollected.Count(p => p);
        }

        private bool CollectKeyPiece(ItemData item)
        {
            int pieceIndex = item.keyPieceIndex;
            if (pieceIndex < 0 || pieceIndex >= 3)
            {
                Debug.LogError($"[PlayerInventory] Invalid key piece index: {pieceIndex}");
                return false;
            }

            if (keyPiecesCollected[pieceIndex])
            {
                Debug.Log($"[PlayerInventory] Already have key piece {pieceIndex}");
                return false;
            }

            keyPiecesCollected[pieceIndex] = true;
            OnKeyPieceCollected?.Invoke(pieceIndex);
            OnInventoryChanged?.Invoke();

            Debug.Log($"[PlayerInventory] Collected key piece {pieceIndex + 1}/3");

            // Check if we have all pieces
            if (HasAllKeyPieces() && !HasGreatKey)
            {
                FormGreatKey();
            }

            return true;
        }

        private void FormGreatKey()
        {
            HasGreatKey = true;
            OnGreatKeyFormed?.Invoke();
            Debug.Log("[PlayerInventory] GREAT KEY FORMED! Find the exit!");
        }

        private bool ApplyItemEffect(ItemData item)
        {
            switch (item.itemType)
            {
                case ItemType.Food:
                    return ApplyFoodEffect(item);

                case ItemType.Key:
                    // Keys are used automatically when interacting with doors
                    Debug.Log($"[PlayerInventory] {item.displayName} - use on matching door");
                    return false;

                case ItemType.Treasure:
                    // Treasures just add score
                    AddScore(item.scoreValue);
                    return true;

                case ItemType.Special:
                    // Special items counter specific enemies
                    Debug.Log($"[PlayerInventory] {item.displayName} counters {item.countersEnemyType}");
                    return false;

                default:
                    return false;
            }
        }

        private bool ApplyFoodEffect(ItemData item)
        {
            if (item.energyRestore <= 0) return false;

            // Find player health
            var playerHealth = Player.PlayerHealth.FindFirstObjectByType<Player.PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RestoreEnergy(item.energyRestore);
                Debug.Log($"[PlayerInventory] Restored {item.energyRestore} energy from {item.displayName}");
                return true;
            }

            return false;
        }

        private void AddScore(int points)
        {
            // Score system will be implemented in Milestone 5
            Debug.Log($"[PlayerInventory] Score +{points}");
        }

        private void CreateDroppedItem(ItemData item, Vector2 position)
        {
            // Create world item
            var droppedObj = new GameObject($"DroppedItem_{item.displayName}");
            droppedObj.transform.position = position;

            var pickup = droppedObj.AddComponent<Items.ItemPickup>();
            pickup.Initialize(item);

            Debug.Log($"[PlayerInventory] Dropped {item.displayName} at {position}");
        }

        /// <summary>
        /// Clears the entire inventory (for game over/restart).
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            keyPiecesCollected = new bool[3];
            HasGreatKey = false;
            OnInventoryChanged?.Invoke();
        }
    }
}
