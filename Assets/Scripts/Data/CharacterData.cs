using UnityEngine;
using HauntedCastle.Services;

namespace HauntedCastle.Data
{
    /// <summary>
    /// ScriptableObject defining character-specific attributes and behaviors.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacter", menuName = "Haunted Castle/Character Data")]
    public class CharacterData : ScriptableObject
    {
        [Header("Identity")]
        public CharacterType characterType;
        public string characterName;
        public string displayName;
        [TextArea(2, 4)]
        public string description;
        public Color characterColor = Color.white;

        [Header("Movement")]
        [Range(1f, 10f)]
        public float moveSpeed = 5f;
        [Range(1f, 50f)]
        public float acceleration = 20f;
        [Range(0f, 1f)]
        public float friction = 0.9f;

        [Header("Combat")]
        public AttackType attackType = AttackType.Projectile;
        [Range(0.1f, 2f)]
        public float attackCooldown = 0.5f;
        [Range(1, 10)]
        public int attackDamage = 1;
        [Range(1f, 20f)]
        public float projectileSpeed = 10f;
        [Range(0.5f, 5f)]
        public float attackRange = 2f;

        [Header("Secret Passages")]
        public SecretPassageType accessiblePassageType;

        [Header("Visuals")]
        public Sprite idleSprite;
        public Sprite[] walkSprites;
        public Sprite attackSprite;
        public RuntimeAnimatorController animatorController;

        [Header("Audio")]
        public AudioClip attackSound;
        public AudioClip hurtSound;
        public AudioClip deathSound;
    }

    public enum AttackType
    {
        Projectile,     // Wizard - ranged magic
        Melee,          // Knight - sword swing
        Thrown          // Serf - thrown weapon
    }

    public enum SecretPassageType
    {
        None,
        Bookcase,       // Wizard access
        Clock,          // Knight access (grandfather clock)
        Barrel          // Serf access (barrel/vat)
    }
}
