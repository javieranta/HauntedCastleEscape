using UnityEngine;

namespace HauntedCastle.Data
{
    /// <summary>
    /// ScriptableObject defining enemy properties and behaviors.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemy", menuName = "Haunted Castle/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string enemyId;
        public string displayName;
        public EnemyType enemyType;

        [Header("Movement")]
        public EnemyBehavior behavior;
        [Range(0.5f, 10f)]
        public float moveSpeed = 3f;
        [Range(0f, 10f)]
        public float detectionRange = 5f;
        [Range(0f, 5f)]
        public float wanderRadius = 2f;

        [Header("Combat")]
        [Range(1, 10)]
        public int damage = 1;
        [Range(1, 10)]
        public int health = 1;
        public bool persistsAfterContact = true; // false = destroyed on contact
        public bool isSpecialEnemy = false;
        public string counteredByItem = ""; // ItemId that can defeat this enemy

        [Header("Spawning")]
        [Range(0f, 1f)]
        public float spawnChance = 0.5f;
        public int maxPerRoom = 3;

        [Header("Visual")]
        public Sprite sprite;
        public Sprite[] animationSprites;
        public RuntimeAnimatorController animatorController;
        public Color tintColor = Color.white;

        [Header("Audio")]
        public AudioClip spawnSound;
        public AudioClip attackSound;
        public AudioClip deathSound;
        public AudioClip idleSound;
    }

    public enum EnemyType
    {
        Ghost,
        Skeleton,
        Spider,
        Bat,
        Demon,
        Mummy,
        Witch,
        Vampire,    // Special enemy
        Werewolf,   // Special enemy
        Reaper      // Special enemy
    }

    public enum EnemyBehavior
    {
        Chaser,     // Directly pursues player
        Wanderer,   // Random movement, chases when close
        Patrol,     // Follows set path
        Stationary, // Doesn't move (hazard-like)
        Ranged,     // Keeps distance, shoots projectiles
        Ambush      // Waits until player is near, then rushes
    }
}
