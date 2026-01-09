using UnityEngine;
using HauntedCastle.Services;

namespace HauntedCastle.Data
{
    /// <summary>
    /// Runtime database that creates and holds CharacterData for all three characters.
    /// Used when ScriptableObject assets haven't been created yet.
    /// </summary>
    public static class CharacterDatabase
    {
        private static CharacterData _wizard;
        private static CharacterData _knight;
        private static CharacterData _serf;

        /// <summary>
        /// Gets character data by type.
        /// </summary>
        public static CharacterData GetCharacter(CharacterType type)
        {
            EnsureInitialized();

            return type switch
            {
                CharacterType.Wizard => _wizard,
                CharacterType.Knight => _knight,
                CharacterType.Serf => _serf,
                _ => _wizard
            };
        }

        /// <summary>
        /// Gets the Wizard character data.
        /// </summary>
        public static CharacterData Wizard
        {
            get
            {
                EnsureInitialized();
                return _wizard;
            }
        }

        /// <summary>
        /// Gets the Knight character data.
        /// </summary>
        public static CharacterData Knight
        {
            get
            {
                EnsureInitialized();
                return _knight;
            }
        }

        /// <summary>
        /// Gets the Serf character data.
        /// </summary>
        public static CharacterData Serf
        {
            get
            {
                EnsureInitialized();
                return _serf;
            }
        }

        private static void EnsureInitialized()
        {
            if (_wizard == null)
            {
                CreateCharacters();
            }
        }

        private static void CreateCharacters()
        {
            // Create Wizard - Fast projectile attacks, can use bookcase passages
            _wizard = ScriptableObject.CreateInstance<CharacterData>();
            _wizard.name = "Wizard";
            _wizard.characterName = "Dorian the Wizard";
            _wizard.characterType = CharacterType.Wizard;
            _wizard.description = "A powerful mage who attacks with magical projectiles. Can pass through bookcases.";

            _wizard.moveSpeed = 4.5f;
            _wizard.attackType = AttackType.Projectile;
            _wizard.attackDamage = 1;
            _wizard.attackRange = 8f;
            _wizard.attackCooldown = 0.4f;
            _wizard.projectileSpeed = 10f;

            _wizard.accessiblePassageType = SecretPassageType.Bookcase;
            _wizard.characterColor = new Color(0.5f, 0.3f, 0.8f); // Purple

            // Create Knight - Melee attacks, can use clock passages
            _knight = ScriptableObject.CreateInstance<CharacterData>();
            _knight.name = "Knight";
            _knight.characterName = "Sir Dorian the Knight";
            _knight.characterType = CharacterType.Knight;
            _knight.description = "A brave warrior who attacks with a sword. Can pass through grandfather clocks.";

            _knight.moveSpeed = 4f;
            _knight.attackType = AttackType.Melee;
            _knight.attackDamage = 2;
            _knight.attackRange = 1.2f;
            _knight.attackCooldown = 0.5f;
            _knight.projectileSpeed = 0f;

            _knight.accessiblePassageType = SecretPassageType.Clock;
            _knight.characterColor = new Color(0.7f, 0.7f, 0.7f); // Silver/Gray

            // Create Serf - Thrown attacks, can use barrel passages
            _serf = ScriptableObject.CreateInstance<CharacterData>();
            _serf.name = "Serf";
            _serf.characterName = "Dorian the Serf";
            _serf.characterType = CharacterType.Serf;
            _serf.description = "A resourceful peasant who throws objects. Can pass through barrels.";

            _serf.moveSpeed = 5f; // Fastest movement
            _serf.attackType = AttackType.Thrown;
            _serf.attackDamage = 1;
            _serf.attackRange = 5f;
            _serf.attackCooldown = 0.6f;
            _serf.projectileSpeed = 7f;

            _serf.accessiblePassageType = SecretPassageType.Barrel;
            _serf.characterColor = new Color(0.6f, 0.4f, 0.2f); // Brown

            Debug.Log("[CharacterDatabase] Created runtime character data for Wizard, Knight, and Serf");
        }

        /// <summary>
        /// Gets all characters as an array.
        /// </summary>
        public static CharacterData[] GetAllCharacters()
        {
            EnsureInitialized();
            return new[] { _wizard, _knight, _serf };
        }
    }
}
