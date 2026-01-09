using UnityEngine;

namespace HauntedCastle.Data
{
    /// <summary>
    /// ScriptableObject defining item properties and effects.
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "Haunted Castle/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        public string itemId;
        public string displayName;
        [TextArea(2, 4)]
        public string description;

        [Header("Type")]
        public ItemType itemType;
        public ItemSubType subType;

        [Header("Visual")]
        public Sprite icon;
        public Sprite worldSprite;
        public Color tintColor = Color.white;

        [Header("Behavior")]
        public bool consumeOnUse = true;
        public bool autoPickup = false;

        [Header("Effects")]
        [Range(0f, 100f)]
        public float energyRestore = 0f;
        [Range(0, 1000)]
        public int scoreValue = 0;

        [Header("Key Properties")]
        public KeyColor keyColor = KeyColor.None;
        public int keyPieceIndex = -1; // 0, 1, 2 for the three key pieces

        [Header("Special")]
        public string countersEnemyType = ""; // Empty if not a counter item
        public float effectDuration = 0f;

        [Header("Audio")]
        public AudioClip pickupSound;
        public AudioClip useSound;
    }

    public enum ItemType
    {
        Food,           // Restores energy
        Key,            // Opens colored doors
        KeyPiece,       // Part of the Great Key
        Treasure,       // Score items (red herrings)
        Special,        // Counter items for special enemies
        GreatKey        // Combined key for exit
    }

    public enum ItemSubType
    {
        None,
        // Food subtypes
        Chicken,
        Bread,
        Apple,
        Potion,
        // Treasure subtypes
        Coin,
        Gem,
        Crown,
        Chalice
    }

    public enum KeyColor
    {
        None,
        Red,
        Blue,
        Green,
        Yellow,
        Cyan,
        Magenta
    }
}
