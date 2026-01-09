using System.Collections.Generic;
using UnityEngine;

namespace HauntedCastle.Data
{
    /// <summary>
    /// Runtime database for creating and retrieving enemy data.
    /// Used for development when ScriptableObject assets don't exist.
    /// </summary>
    public static class EnemyDatabase
    {
        private static Dictionary<EnemyType, EnemyData> _enemies = new();
        private static bool _initialized = false;

        /// <summary>
        /// Gets enemy data by type.
        /// </summary>
        public static EnemyData GetEnemy(EnemyType type)
        {
            EnsureInitialized();
            return _enemies.TryGetValue(type, out var enemy) ? enemy : null;
        }

        /// <summary>
        /// Gets enemy data by ID string.
        /// </summary>
        public static EnemyData GetEnemyById(string enemyId)
        {
            EnsureInitialized();
            foreach (var enemy in _enemies.Values)
            {
                if (enemy.enemyId == enemyId)
                {
                    return enemy;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets all regular (non-special) enemies.
        /// </summary>
        public static List<EnemyData> GetRegularEnemies()
        {
            EnsureInitialized();
            var result = new List<EnemyData>();
            foreach (var enemy in _enemies.Values)
            {
                if (!enemy.isSpecialEnemy)
                {
                    result.Add(enemy);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all special enemies.
        /// </summary>
        public static List<EnemyData> GetSpecialEnemies()
        {
            EnsureInitialized();
            var result = new List<EnemyData>();
            foreach (var enemy in _enemies.Values)
            {
                if (enemy.isSpecialEnemy)
                {
                    result.Add(enemy);
                }
            }
            return result;
        }

        private static void EnsureInitialized()
        {
            if (_initialized) return;

            CreateRegularEnemies();
            CreateSpecialEnemies();

            _initialized = true;
            Debug.Log($"[EnemyDatabase] Initialized with {_enemies.Count} enemy types");
        }

        private static void CreateRegularEnemies()
        {
            // Ghost - Fast, chases player
            var ghost = ScriptableObject.CreateInstance<EnemyData>();
            ghost.name = "Ghost";
            ghost.enemyId = "enemy_ghost";
            ghost.displayName = "Ghost";
            ghost.enemyType = EnemyType.Ghost;
            ghost.behavior = EnemyBehavior.Chaser;
            ghost.moveSpeed = 3.5f;
            ghost.detectionRange = 6f;
            ghost.wanderRadius = 3f;
            ghost.damage = 10;
            ghost.health = 1;
            ghost.persistsAfterContact = true;
            ghost.spawnChance = 0.7f;
            ghost.maxPerRoom = 3;
            ghost.tintColor = new Color(0.8f, 0.8f, 1f, 0.7f); // Pale blue transparent
            _enemies[EnemyType.Ghost] = ghost;

            // Skeleton - Patrols, medium speed
            var skeleton = ScriptableObject.CreateInstance<EnemyData>();
            skeleton.name = "Skeleton";
            skeleton.enemyId = "enemy_skeleton";
            skeleton.displayName = "Skeleton";
            skeleton.enemyType = EnemyType.Skeleton;
            skeleton.behavior = EnemyBehavior.Patrol;
            skeleton.moveSpeed = 2.5f;
            skeleton.detectionRange = 4f;
            skeleton.wanderRadius = 2f;
            skeleton.damage = 15;
            skeleton.health = 2;
            skeleton.persistsAfterContact = true;
            skeleton.spawnChance = 0.6f;
            skeleton.maxPerRoom = 2;
            skeleton.tintColor = new Color(0.9f, 0.9f, 0.85f); // Bone white
            _enemies[EnemyType.Skeleton] = skeleton;

            // Spider - Wanders, destroys on contact
            var spider = ScriptableObject.CreateInstance<EnemyData>();
            spider.name = "Spider";
            spider.enemyId = "enemy_spider";
            spider.displayName = "Giant Spider";
            spider.enemyType = EnemyType.Spider;
            spider.behavior = EnemyBehavior.Wanderer;
            spider.moveSpeed = 2f;
            spider.detectionRange = 3f;
            spider.wanderRadius = 4f;
            spider.damage = 10;
            spider.health = 1;
            spider.persistsAfterContact = false; // Dies on contact
            spider.spawnChance = 0.8f;
            spider.maxPerRoom = 4;
            spider.tintColor = new Color(0.3f, 0.2f, 0.1f); // Dark brown
            _enemies[EnemyType.Spider] = spider;

            // Bat - Fast wanderer
            var bat = ScriptableObject.CreateInstance<EnemyData>();
            bat.name = "Bat";
            bat.enemyId = "enemy_bat";
            bat.displayName = "Vampire Bat";
            bat.enemyType = EnemyType.Bat;
            bat.behavior = EnemyBehavior.Wanderer;
            bat.moveSpeed = 4f;
            bat.detectionRange = 5f;
            bat.wanderRadius = 5f;
            bat.damage = 5;
            bat.health = 1;
            bat.persistsAfterContact = true;
            bat.spawnChance = 0.7f;
            bat.maxPerRoom = 5;
            bat.tintColor = new Color(0.2f, 0.2f, 0.25f); // Dark gray
            _enemies[EnemyType.Bat] = bat;

            // Demon - Strong, chases
            var demon = ScriptableObject.CreateInstance<EnemyData>();
            demon.name = "Demon";
            demon.enemyId = "enemy_demon";
            demon.displayName = "Lesser Demon";
            demon.enemyType = EnemyType.Demon;
            demon.behavior = EnemyBehavior.Chaser;
            demon.moveSpeed = 3f;
            demon.detectionRange = 7f;
            demon.wanderRadius = 2f;
            demon.damage = 20;
            demon.health = 3;
            demon.persistsAfterContact = true;
            demon.spawnChance = 0.4f;
            demon.maxPerRoom = 1;
            demon.tintColor = new Color(0.8f, 0.2f, 0.2f); // Red
            _enemies[EnemyType.Demon] = demon;

            // Mummy - Slow but tough
            var mummy = ScriptableObject.CreateInstance<EnemyData>();
            mummy.name = "Mummy";
            mummy.enemyId = "enemy_mummy";
            mummy.displayName = "Mummy";
            mummy.enemyType = EnemyType.Mummy;
            mummy.behavior = EnemyBehavior.Patrol;
            mummy.moveSpeed = 1.5f;
            mummy.detectionRange = 4f;
            mummy.wanderRadius = 1.5f;
            mummy.damage = 15;
            mummy.health = 4;
            mummy.persistsAfterContact = true;
            mummy.spawnChance = 0.5f;
            mummy.maxPerRoom = 2;
            mummy.tintColor = new Color(0.8f, 0.75f, 0.6f); // Tan
            _enemies[EnemyType.Mummy] = mummy;

            // Witch - Ranged/keeps distance
            var witch = ScriptableObject.CreateInstance<EnemyData>();
            witch.name = "Witch";
            witch.enemyId = "enemy_witch";
            witch.displayName = "Witch";
            witch.enemyType = EnemyType.Witch;
            witch.behavior = EnemyBehavior.Ranged;
            witch.moveSpeed = 2f;
            witch.detectionRange = 8f;
            witch.wanderRadius = 3f;
            witch.damage = 10;
            witch.health = 2;
            witch.persistsAfterContact = true;
            witch.spawnChance = 0.3f;
            witch.maxPerRoom = 1;
            witch.tintColor = new Color(0.4f, 0.8f, 0.3f); // Green
            _enemies[EnemyType.Witch] = witch;
        }

        private static void CreateSpecialEnemies()
        {
            // Vampire (Dracula) - Countered by Cross
            var vampire = ScriptableObject.CreateInstance<EnemyData>();
            vampire.name = "Vampire";
            vampire.enemyId = "enemy_vampire";
            vampire.displayName = "Dracula";
            vampire.enemyType = EnemyType.Vampire;
            vampire.behavior = EnemyBehavior.Ambush;
            vampire.moveSpeed = 5f;
            vampire.detectionRange = 6f;
            vampire.wanderRadius = 1f;
            vampire.damage = 100; // Instant kill
            vampire.health = 999; // Cannot be killed normally
            vampire.persistsAfterContact = true;
            vampire.isSpecialEnemy = true;
            vampire.counteredByItem = "special_cross";
            vampire.spawnChance = 0.3f;
            vampire.maxPerRoom = 1;
            vampire.tintColor = new Color(0.3f, 0f, 0f); // Dark red
            _enemies[EnemyType.Vampire] = vampire;

            // Werewolf (Hunchback analog) - Countered by Garlic Wreath
            var werewolf = ScriptableObject.CreateInstance<EnemyData>();
            werewolf.name = "Werewolf";
            werewolf.enemyId = "enemy_werewolf";
            werewolf.displayName = "Hunchback";
            werewolf.enemyType = EnemyType.Werewolf;
            werewolf.behavior = EnemyBehavior.Chaser;
            werewolf.moveSpeed = 6f;
            werewolf.detectionRange = 8f;
            werewolf.wanderRadius = 2f;
            werewolf.damage = 100;
            werewolf.health = 999;
            werewolf.persistsAfterContact = true;
            werewolf.isSpecialEnemy = true;
            werewolf.counteredByItem = "special_wreath";
            werewolf.spawnChance = 0.3f;
            werewolf.maxPerRoom = 1;
            werewolf.tintColor = new Color(0.4f, 0.3f, 0.2f); // Brown
            _enemies[EnemyType.Werewolf] = werewolf;

            // Reaper (Frankenstein analog) - Countered by Spellbook
            var reaper = ScriptableObject.CreateInstance<EnemyData>();
            reaper.name = "Reaper";
            reaper.enemyId = "enemy_reaper";
            reaper.displayName = "Frankenstein";
            reaper.enemyType = EnemyType.Reaper;
            reaper.behavior = EnemyBehavior.Patrol;
            reaper.moveSpeed = 3f;
            reaper.detectionRange = 5f;
            reaper.wanderRadius = 1.5f;
            reaper.damage = 100;
            reaper.health = 999;
            reaper.persistsAfterContact = true;
            reaper.isSpecialEnemy = true;
            reaper.counteredByItem = "special_spellbook";
            reaper.spawnChance = 0.3f;
            reaper.maxPerRoom = 1;
            reaper.tintColor = new Color(0.3f, 0.5f, 0.3f); // Gray-green
            _enemies[EnemyType.Reaper] = reaper;
        }

        /// <summary>
        /// Gets all enemy types.
        /// </summary>
        public static IEnumerable<EnemyData> GetAllEnemies()
        {
            EnsureInitialized();
            return _enemies.Values;
        }

        /// <summary>
        /// Gets a random regular enemy for spawning.
        /// </summary>
        public static EnemyData GetRandomRegularEnemy()
        {
            var regulars = GetRegularEnemies();
            if (regulars.Count == 0) return null;
            return regulars[Random.Range(0, regulars.Count)];
        }

        /// <summary>
        /// Gets a random special enemy for spawning.
        /// </summary>
        public static EnemyData GetRandomSpecialEnemy()
        {
            var specials = GetSpecialEnemies();
            if (specials.Count == 0) return null;
            return specials[Random.Range(0, specials.Count)];
        }
    }
}
