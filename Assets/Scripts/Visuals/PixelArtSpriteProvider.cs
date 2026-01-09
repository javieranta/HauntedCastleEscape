using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Provides pixel art sprites for all game entities.
    /// Acts as a bridge between PixelArtGenerator and game systems.
    /// </summary>
    public static class PixelArtSpriteProvider
    {
        #region Character Sprites

        /// <summary>
        /// Gets the sprite for a character type.
        /// </summary>
        public static Sprite GetCharacterSprite(CharacterType characterType)
        {
            return characterType switch
            {
                CharacterType.Knight => PixelArtGenerator.GetKnightSprite(),
                CharacterType.Wizard => PixelArtGenerator.GetWizardSprite(),
                CharacterType.Serf => PixelArtGenerator.GetSerfSprite(),
                _ => PixelArtGenerator.GetKnightSprite()
            };
        }

        /// <summary>
        /// Gets walk animation frames for a character.
        /// </summary>
        public static Sprite[] GetCharacterWalkSprites(CharacterType characterType)
        {
            Sprite baseSprite = GetCharacterSprite(characterType);
            // Return same sprite for simple animation (could be expanded for actual frame variations)
            return new[] { baseSprite, baseSprite };
        }

        #endregion

        #region Enemy Sprites

        /// <summary>
        /// Gets the sprite for an enemy type.
        /// </summary>
        public static Sprite GetEnemySprite(EnemyType enemyType)
        {
            return enemyType switch
            {
                EnemyType.Bat => PixelArtGenerator.GetBatSprite(),
                EnemyType.Ghost => PixelArtGenerator.GetGhostSprite(),
                EnemyType.Skeleton => PixelArtGenerator.GetSkeletonSprite(),
                EnemyType.Spider => PixelArtGenerator.GetSpiderSprite(),
                EnemyType.Mummy => PixelArtGenerator.GetMummySprite(),
                EnemyType.Witch => PixelArtGenerator.GetWitchSprite(),
                EnemyType.Demon => PixelArtGenerator.GetDemonSprite(),
                EnemyType.Vampire => PixelArtGenerator.GetVampireSprite(),
                EnemyType.Reaper => PixelArtGenerator.GetReaperSprite(),
                EnemyType.Werewolf => PixelArtGenerator.GetWerewolfSprite(),
                _ => PixelArtGenerator.GetBatSprite()
            };
        }

        /// <summary>
        /// Gets animation frames for an enemy.
        /// </summary>
        public static Sprite[] GetEnemyAnimationSprites(EnemyType enemyType)
        {
            Sprite baseSprite = GetEnemySprite(enemyType);
            return new[] { baseSprite, baseSprite };
        }

        #endregion

        #region Item Sprites

        /// <summary>
        /// Gets the sprite for an item based on type and key color.
        /// </summary>
        public static Sprite GetItemSprite(ItemType itemType, KeyColor keyColor = KeyColor.None, ItemSubType subType = ItemSubType.None)
        {
            return itemType switch
            {
                ItemType.Key => GetKeySprite(keyColor),
                ItemType.KeyPiece => GetKeyPieceSprite(0),
                ItemType.Food => GetFoodSprite(subType),
                ItemType.Treasure => GetTreasureSprite(subType),
                ItemType.Special => GetSpecialItemSprite(subType),
                ItemType.GreatKey => PixelArtGenerator.GetKeySprite(new Color(1f, 0.9f, 0.3f)), // Golden
                _ => PixelArtGenerator.GetKeySprite(Color.white)
            };
        }

        /// <summary>
        /// Gets a key sprite with the specified color.
        /// </summary>
        public static Sprite GetKeySprite(KeyColor keyColor)
        {
            Color color = keyColor switch
            {
                KeyColor.Red => Color.red,
                KeyColor.Blue => Color.blue,
                KeyColor.Green => Color.green,
                KeyColor.Yellow => Color.yellow,
                KeyColor.Cyan => Color.cyan,
                KeyColor.Magenta => Color.magenta,
                _ => new Color(1f, 0.85f, 0.2f) // Gold
            };
            return PixelArtGenerator.GetKeySprite(color);
        }

        /// <summary>
        /// Gets a key piece sprite.
        /// </summary>
        public static Sprite GetKeyPieceSprite(int index)
        {
            return PixelArtGenerator.GetKeyPieceSprite(index);
        }

        /// <summary>
        /// Gets a food sprite based on subtype.
        /// </summary>
        public static Sprite GetFoodSprite(ItemSubType subType)
        {
            string foodType = subType switch
            {
                ItemSubType.Chicken => "chicken",
                ItemSubType.Bread => "bread",
                ItemSubType.Apple => "chicken", // Use chicken as default
                ItemSubType.Potion => "bread", // Use bread as placeholder
                _ => "chicken"
            };
            return PixelArtGenerator.GetFoodSprite(foodType);
        }

        /// <summary>
        /// Gets a treasure sprite based on subtype.
        /// </summary>
        public static Sprite GetTreasureSprite(ItemSubType subType)
        {
            string treasureType = subType switch
            {
                ItemSubType.Crown => "crown",
                ItemSubType.Chalice => "chalice",
                ItemSubType.Coin => "chalice",
                ItemSubType.Gem => "crown",
                _ => "chalice"
            };
            return PixelArtGenerator.GetTreasureSprite(treasureType);
        }

        /// <summary>
        /// Gets a special item sprite (weapons, etc).
        /// </summary>
        public static Sprite GetSpecialItemSprite(ItemSubType subType)
        {
            return PixelArtGenerator.GetWeaponSprite("sword");
        }

        #endregion

        #region Projectile Sprites

        /// <summary>
        /// Gets a projectile sprite based on character type.
        /// </summary>
        public static Sprite GetProjectileSprite(CharacterType characterType)
        {
            string projectileType = characterType switch
            {
                CharacterType.Knight => "sword_slash",
                CharacterType.Wizard => "magic_bolt",
                CharacterType.Serf => "axe_throw",
                _ => "magic_bolt"
            };
            return PixelArtGenerator.GetProjectileSprite(projectileType);
        }

        #endregion

        #region Environment Sprites

        /// <summary>
        /// Gets a door sprite.
        /// </summary>
        public static Sprite GetDoorSprite(bool isOpen, KeyColor keyColor = KeyColor.None)
        {
            Color? color = keyColor != KeyColor.None ? GetColorFromKeyColor(keyColor) : null;
            return PixelArtGenerator.GetDoorSprite(isOpen, color);
        }

        /// <summary>
        /// Gets a wall tile sprite.
        /// </summary>
        public static Sprite GetWallSprite()
        {
            return PixelArtGenerator.GetWallTile();
        }

        /// <summary>
        /// Gets a floor tile sprite.
        /// </summary>
        public static Sprite GetFloorSprite()
        {
            return PixelArtGenerator.GetFloorTile();
        }

        /// <summary>
        /// Gets a stairs sprite.
        /// </summary>
        public static Sprite GetStairsSprite(bool goingUp)
        {
            return PixelArtGenerator.GetStairsSprite(goingUp);
        }

        private static Color? GetColorFromKeyColor(KeyColor keyColor)
        {
            return keyColor switch
            {
                KeyColor.Red => Color.red,
                KeyColor.Blue => Color.blue,
                KeyColor.Green => Color.green,
                KeyColor.Yellow => Color.yellow,
                KeyColor.Cyan => Color.cyan,
                KeyColor.Magenta => Color.magenta,
                _ => null
            };
        }

        #endregion
    }
}
