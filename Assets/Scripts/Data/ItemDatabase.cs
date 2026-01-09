using System.Collections.Generic;
using UnityEngine;

namespace HauntedCastle.Data
{
    /// <summary>
    /// Runtime database for creating and retrieving item data.
    /// Used for development when ScriptableObject assets don't exist.
    /// </summary>
    public static class ItemDatabase
    {
        private static Dictionary<string, ItemData> _items = new();
        private static bool _initialized = false;

        /// <summary>
        /// Gets item data by ID.
        /// </summary>
        public static ItemData GetItem(string itemId)
        {
            EnsureInitialized();
            return _items.TryGetValue(itemId, out var item) ? item : null;
        }

        /// <summary>
        /// Gets all items of a specific type.
        /// </summary>
        public static List<ItemData> GetItemsByType(ItemType type)
        {
            EnsureInitialized();
            var result = new List<ItemData>();
            foreach (var item in _items.Values)
            {
                if (item.itemType == type)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a key item by color.
        /// </summary>
        public static ItemData GetKey(KeyColor color)
        {
            return GetItem($"key_{color.ToString().ToLower()}");
        }

        /// <summary>
        /// Gets a key piece by index (0-2).
        /// </summary>
        public static ItemData GetKeyPiece(int index)
        {
            return GetItem($"key_piece_{index}");
        }

        /// <summary>
        /// Gets a food item by subtype.
        /// </summary>
        public static ItemData GetFood(ItemSubType subType)
        {
            return GetItem($"food_{subType.ToString().ToLower()}");
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;

            CreateKeys();
            CreateKeyPieces();
            CreateFood();
            CreateTreasures();
            CreateSpecialItems();

            _initialized = true;
            Debug.Log($"[ItemDatabase] Initialized with {_items.Count} items");
        }

        private static void CreateKeys()
        {
            // Create colored keys
            var keyColors = new[] {
                KeyColor.Red, KeyColor.Blue, KeyColor.Green,
                KeyColor.Yellow, KeyColor.Cyan, KeyColor.Magenta
            };

            foreach (var color in keyColors)
            {
                var key = ScriptableObject.CreateInstance<ItemData>();
                key.name = $"Key_{color}";
                key.itemId = $"key_{color.ToString().ToLower()}";
                key.displayName = $"{color} Key";
                key.description = $"A {color.ToString().ToLower()} key. Opens matching doors.";
                key.itemType = ItemType.Key;
                key.keyColor = color;
                key.tintColor = GetColorValue(color);
                key.consumeOnUse = true;
                key.scoreValue = 50;

                _items[key.itemId] = key;
            }
        }

        private static void CreateKeyPieces()
        {
            string[] pieceNames = { "Crown Fragment", "Scepter Fragment", "Orb Fragment" };
            string[] pieceDescs = {
                "A fragment of the Great Key - the crown portion.",
                "A fragment of the Great Key - the scepter portion.",
                "A fragment of the Great Key - the orb portion."
            };

            for (int i = 0; i < 3; i++)
            {
                var piece = ScriptableObject.CreateInstance<ItemData>();
                piece.name = $"KeyPiece_{i}";
                piece.itemId = $"key_piece_{i}";
                piece.displayName = pieceNames[i];
                piece.description = pieceDescs[i];
                piece.itemType = ItemType.KeyPiece;
                piece.keyPieceIndex = i;
                piece.tintColor = new Color(1f, 0.84f, 0f); // Gold
                piece.autoPickup = true;
                piece.consumeOnUse = false;

                _items[piece.itemId] = piece;
            }
        }

        private static void CreateFood()
        {
            // Chicken - High energy restore
            var chicken = ScriptableObject.CreateInstance<ItemData>();
            chicken.name = "Food_Chicken";
            chicken.itemId = "food_chicken";
            chicken.displayName = "Roast Chicken";
            chicken.description = "A delicious roast chicken. Restores 50 energy.";
            chicken.itemType = ItemType.Food;
            chicken.subType = ItemSubType.Chicken;
            chicken.energyRestore = 50f;
            chicken.tintColor = new Color(0.8f, 0.6f, 0.4f);
            chicken.consumeOnUse = true;
            _items[chicken.itemId] = chicken;

            // Bread - Medium energy restore
            var bread = ScriptableObject.CreateInstance<ItemData>();
            bread.name = "Food_Bread";
            bread.itemId = "food_bread";
            bread.displayName = "Bread Loaf";
            bread.description = "A fresh loaf of bread. Restores 30 energy.";
            bread.itemType = ItemType.Food;
            bread.subType = ItemSubType.Bread;
            bread.energyRestore = 30f;
            bread.tintColor = new Color(0.9f, 0.7f, 0.4f);
            bread.consumeOnUse = true;
            _items[bread.itemId] = bread;

            // Apple - Low energy restore
            var apple = ScriptableObject.CreateInstance<ItemData>();
            apple.name = "Food_Apple";
            apple.itemId = "food_apple";
            apple.displayName = "Apple";
            apple.description = "A crisp apple. Restores 15 energy.";
            apple.itemType = ItemType.Food;
            apple.subType = ItemSubType.Apple;
            apple.energyRestore = 15f;
            apple.tintColor = Color.red;
            apple.consumeOnUse = true;
            _items[apple.itemId] = apple;

            // Potion - Full restore
            var potion = ScriptableObject.CreateInstance<ItemData>();
            potion.name = "Food_Potion";
            potion.itemId = "food_potion";
            potion.displayName = "Energy Potion";
            potion.description = "A magical potion. Fully restores energy.";
            potion.itemType = ItemType.Food;
            potion.subType = ItemSubType.Potion;
            potion.energyRestore = 100f;
            potion.tintColor = new Color(0.5f, 0.8f, 1f);
            potion.consumeOnUse = true;
            _items[potion.itemId] = potion;
        }

        private static void CreateTreasures()
        {
            // Coin
            var coin = ScriptableObject.CreateInstance<ItemData>();
            coin.name = "Treasure_Coin";
            coin.itemId = "treasure_coin";
            coin.displayName = "Gold Coin";
            coin.description = "A shiny gold coin. Worth 100 points.";
            coin.itemType = ItemType.Treasure;
            coin.subType = ItemSubType.Coin;
            coin.scoreValue = 100;
            coin.tintColor = new Color(1f, 0.84f, 0f);
            coin.autoPickup = true;
            coin.consumeOnUse = true;
            _items[coin.itemId] = coin;

            // Gem
            var gem = ScriptableObject.CreateInstance<ItemData>();
            gem.name = "Treasure_Gem";
            gem.itemId = "treasure_gem";
            gem.displayName = "Ruby Gem";
            gem.description = "A precious ruby. Worth 250 points.";
            gem.itemType = ItemType.Treasure;
            gem.subType = ItemSubType.Gem;
            gem.scoreValue = 250;
            gem.tintColor = new Color(1f, 0.2f, 0.3f);
            gem.consumeOnUse = true;
            _items[gem.itemId] = gem;

            // Crown
            var crown = ScriptableObject.CreateInstance<ItemData>();
            crown.name = "Treasure_Crown";
            crown.itemId = "treasure_crown";
            crown.displayName = "Silver Crown";
            crown.description = "A royal silver crown. Worth 500 points.";
            crown.itemType = ItemType.Treasure;
            crown.subType = ItemSubType.Crown;
            crown.scoreValue = 500;
            crown.tintColor = new Color(0.75f, 0.75f, 0.8f);
            crown.consumeOnUse = true;
            _items[crown.itemId] = crown;

            // Chalice
            var chalice = ScriptableObject.CreateInstance<ItemData>();
            chalice.name = "Treasure_Chalice";
            chalice.itemId = "treasure_chalice";
            chalice.displayName = "Golden Chalice";
            chalice.description = "An ornate golden chalice. Worth 1000 points.";
            chalice.itemType = ItemType.Treasure;
            chalice.subType = ItemSubType.Chalice;
            chalice.scoreValue = 1000;
            chalice.tintColor = new Color(1f, 0.9f, 0.2f);
            chalice.consumeOnUse = true;
            _items[chalice.itemId] = chalice;
        }

        private static void CreateSpecialItems()
        {
            // Cross - counters Dracula
            var cross = ScriptableObject.CreateInstance<ItemData>();
            cross.name = "Special_Cross";
            cross.itemId = "special_cross";
            cross.displayName = "Holy Cross";
            cross.description = "A blessed cross. Repels Dracula.";
            cross.itemType = ItemType.Special;
            cross.countersEnemyType = "Dracula";
            cross.tintColor = Color.white;
            cross.consumeOnUse = false;
            _items[cross.itemId] = cross;

            // Wreath - counters Hunchback
            var wreath = ScriptableObject.CreateInstance<ItemData>();
            wreath.name = "Special_Wreath";
            wreath.itemId = "special_wreath";
            wreath.displayName = "Garlic Wreath";
            wreath.description = "A wreath of garlic. Repels Hunchback.";
            wreath.itemType = ItemType.Special;
            wreath.countersEnemyType = "Hunchback";
            wreath.tintColor = new Color(0.9f, 0.9f, 0.8f);
            wreath.consumeOnUse = false;
            _items[wreath.itemId] = wreath;

            // Spellbook - counters Frankenstein
            var spellbook = ScriptableObject.CreateInstance<ItemData>();
            spellbook.name = "Special_Spellbook";
            spellbook.itemId = "special_spellbook";
            spellbook.displayName = "Spellbook";
            spellbook.description = "An ancient spellbook. Repels Frankenstein.";
            spellbook.itemType = ItemType.Special;
            spellbook.countersEnemyType = "Frankenstein";
            spellbook.tintColor = new Color(0.5f, 0.3f, 0.7f);
            spellbook.consumeOnUse = false;
            _items[spellbook.itemId] = spellbook;

            // Amulet - counters Devil
            var amulet = ScriptableObject.CreateInstance<ItemData>();
            amulet.name = "Special_Amulet";
            amulet.itemId = "special_amulet";
            amulet.displayName = "Magic Amulet";
            amulet.description = "A powerful amulet. Repels the Devil.";
            amulet.itemType = ItemType.Special;
            amulet.countersEnemyType = "Devil";
            amulet.tintColor = new Color(0.3f, 0.8f, 0.9f);
            amulet.consumeOnUse = false;
            _items[amulet.itemId] = amulet;
        }

        private static Color GetColorValue(KeyColor keyColor)
        {
            return keyColor switch
            {
                KeyColor.Red => Color.red,
                KeyColor.Blue => Color.blue,
                KeyColor.Green => Color.green,
                KeyColor.Yellow => Color.yellow,
                KeyColor.Cyan => Color.cyan,
                KeyColor.Magenta => Color.magenta,
                _ => Color.white
            };
        }

        /// <summary>
        /// Gets all item IDs.
        /// </summary>
        public static IEnumerable<string> GetAllItemIds()
        {
            EnsureInitialized();
            return _items.Keys;
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        public static IEnumerable<ItemData> GetAllItems()
        {
            EnsureInitialized();
            return _items.Values;
        }
    }
}
