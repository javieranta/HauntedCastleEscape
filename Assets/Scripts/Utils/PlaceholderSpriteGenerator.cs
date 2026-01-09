using UnityEngine;
using System.Collections.Generic;

namespace HauntedCastle.Utils
{
    /// <summary>
    /// Generates placeholder sprites at runtime for development/testing.
    /// These colored shapes help visualize game entities before real art is added.
    /// </summary>
    public static class PlaceholderSpriteGenerator
    {
        private static Dictionary<string, Sprite> _spriteCache = new();

        // Character colors (matching CharacterDatabase)
        public static readonly Color WizardColor = new Color(0.6f, 0.3f, 0.8f);      // Purple
        public static readonly Color KnightColor = new Color(0.7f, 0.7f, 0.75f);     // Silver
        public static readonly Color SerfColor = new Color(0.6f, 0.4f, 0.2f);        // Brown

        // Enemy colors
        public static readonly Color GhostColor = new Color(0.8f, 0.8f, 0.9f, 0.7f); // Translucent white
        public static readonly Color SkeletonColor = new Color(0.9f, 0.9f, 0.85f);   // Bone white
        public static readonly Color SpiderColor = new Color(0.2f, 0.2f, 0.2f);      // Dark gray
        public static readonly Color BatColor = new Color(0.3f, 0.2f, 0.3f);         // Dark purple
        public static readonly Color DemonColor = new Color(0.8f, 0.2f, 0.2f);       // Red
        public static readonly Color MummyColor = new Color(0.8f, 0.75f, 0.6f);      // Beige
        public static readonly Color WitchColor = new Color(0.4f, 0.6f, 0.3f);       // Green
        public static readonly Color VampireColor = new Color(0.4f, 0.1f, 0.1f);     // Dark red
        public static readonly Color WerewolfColor = new Color(0.5f, 0.4f, 0.3f);    // Brown-gray
        public static readonly Color ReaperColor = new Color(0.1f, 0.1f, 0.15f);     // Near black

        // Item colors
        public static readonly Color KeyColor = new Color(1f, 0.84f, 0f);            // Gold
        public static readonly Color KeyPieceColor = new Color(0.8f, 0.6f, 0.2f);    // Bronze
        public static readonly Color FoodColor = new Color(0.9f, 0.5f, 0.3f);        // Orange
        public static readonly Color WeaponColor = new Color(0.6f, 0.6f, 0.7f);      // Steel
        public static readonly Color PotionColor = new Color(0.3f, 0.8f, 0.4f);      // Green

        // Projectile colors
        public static readonly Color MagicColor = new Color(0.5f, 0.3f, 1f);         // Purple magic
        public static readonly Color SwordColor = new Color(0.7f, 0.7f, 0.8f);       // Steel
        public static readonly Color ThrownColor = new Color(0.6f, 0.5f, 0.3f);      // Brown

        /// <summary>
        /// Gets or creates a colored square sprite.
        /// </summary>
        public static Sprite GetSquareSprite(string name, Color color, int size = 32)
        {
            string key = $"square_{name}_{size}";
            if (_spriteCache.TryGetValue(key, out Sprite cached))
                return cached;

            Texture2D texture = CreateSquareTexture(color, size);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size // pixels per unit
            );
            sprite.name = name;

            _spriteCache[key] = sprite;
            return sprite;
        }

        /// <summary>
        /// Gets or creates a colored circle sprite.
        /// </summary>
        public static Sprite GetCircleSprite(string name, Color color, int size = 32)
        {
            string key = $"circle_{name}_{size}";
            if (_spriteCache.TryGetValue(key, out Sprite cached))
                return cached;

            Texture2D texture = CreateCircleTexture(color, size);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size
            );
            sprite.name = name;

            _spriteCache[key] = sprite;
            return sprite;
        }

        /// <summary>
        /// Gets or creates a colored diamond sprite.
        /// </summary>
        public static Sprite GetDiamondSprite(string name, Color color, int size = 32)
        {
            string key = $"diamond_{name}_{size}";
            if (_spriteCache.TryGetValue(key, out Sprite cached))
                return cached;

            Texture2D texture = CreateDiamondTexture(color, size);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size
            );
            sprite.name = name;

            _spriteCache[key] = sprite;
            return sprite;
        }

        /// <summary>
        /// Gets or creates a colored triangle sprite (pointing up).
        /// </summary>
        public static Sprite GetTriangleSprite(string name, Color color, int size = 32)
        {
            string key = $"triangle_{name}_{size}";
            if (_spriteCache.TryGetValue(key, out Sprite cached))
                return cached;

            Texture2D texture = CreateTriangleTexture(color, size);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, size, size),
                new Vector2(0.5f, 0.5f),
                size
            );
            sprite.name = name;

            _spriteCache[key] = sprite;
            return sprite;
        }

        // ==================== Character Sprites ====================

        public static Sprite GetWizardSprite()
        {
            return GetTriangleSprite("Wizard", WizardColor, 32);
        }

        public static Sprite GetKnightSprite()
        {
            return GetSquareSprite("Knight", KnightColor, 32);
        }

        public static Sprite GetSerfSprite()
        {
            return GetCircleSprite("Serf", SerfColor, 32);
        }

        // ==================== Enemy Sprites ====================

        public static Sprite GetGhostSprite()
        {
            return GetCircleSprite("Ghost", GhostColor, 24);
        }

        public static Sprite GetSkeletonSprite()
        {
            return GetSquareSprite("Skeleton", SkeletonColor, 28);
        }

        public static Sprite GetSpiderSprite()
        {
            return GetCircleSprite("Spider", SpiderColor, 20);
        }

        public static Sprite GetBatSprite()
        {
            return GetDiamondSprite("Bat", BatColor, 20);
        }

        public static Sprite GetDemonSprite()
        {
            return GetTriangleSprite("Demon", DemonColor, 32);
        }

        public static Sprite GetMummySprite()
        {
            return GetSquareSprite("Mummy", MummyColor, 28);
        }

        public static Sprite GetWitchSprite()
        {
            return GetTriangleSprite("Witch", WitchColor, 28);
        }

        public static Sprite GetVampireSprite()
        {
            return GetDiamondSprite("Vampire", VampireColor, 32);
        }

        public static Sprite GetWerewolfSprite()
        {
            return GetSquareSprite("Werewolf", WerewolfColor, 32);
        }

        public static Sprite GetReaperSprite()
        {
            return GetTriangleSprite("Reaper", ReaperColor, 36);
        }

        /// <summary>
        /// Gets enemy sprite by type.
        /// </summary>
        public static Sprite GetEnemySprite(Data.EnemyType enemyType)
        {
            return enemyType switch
            {
                Data.EnemyType.Ghost => GetGhostSprite(),
                Data.EnemyType.Skeleton => GetSkeletonSprite(),
                Data.EnemyType.Spider => GetSpiderSprite(),
                Data.EnemyType.Bat => GetBatSprite(),
                Data.EnemyType.Demon => GetDemonSprite(),
                Data.EnemyType.Mummy => GetMummySprite(),
                Data.EnemyType.Witch => GetWitchSprite(),
                Data.EnemyType.Vampire => GetVampireSprite(),
                Data.EnemyType.Werewolf => GetWerewolfSprite(),
                Data.EnemyType.Reaper => GetReaperSprite(),
                _ => GetSquareSprite("Unknown", Color.magenta, 24)
            };
        }

        /// <summary>
        /// Gets character sprite by type.
        /// </summary>
        public static Sprite GetCharacterSprite(Services.CharacterType characterType)
        {
            return characterType switch
            {
                Services.CharacterType.Wizard => GetWizardSprite(),
                Services.CharacterType.Knight => GetKnightSprite(),
                Services.CharacterType.Serf => GetSerfSprite(),
                _ => GetSquareSprite("Unknown", Color.white, 32)
            };
        }

        // ==================== Item Sprites ====================

        public static Sprite GetKeySprite()
        {
            return GetDiamondSprite("Key", KeyColor, 16);
        }

        public static Sprite GetKeyPieceSprite()
        {
            return GetTriangleSprite("KeyPiece", KeyPieceColor, 16);
        }

        public static Sprite GetFoodSprite()
        {
            return GetCircleSprite("Food", FoodColor, 16);
        }

        public static Sprite GetWeaponSprite()
        {
            return GetSquareSprite("Weapon", WeaponColor, 16);
        }

        public static Sprite GetPotionSprite()
        {
            return GetCircleSprite("Potion", PotionColor, 16);
        }

        public static Sprite GetTreasureSprite()
        {
            return GetDiamondSprite("Treasure", new Color(1f, 0.6f, 0.2f), 16);
        }

        public static Sprite GetSpecialItemSprite()
        {
            return GetTriangleSprite("Special", new Color(0.8f, 0.2f, 0.8f), 16);
        }

        /// <summary>
        /// Gets item sprite by type and optional key color.
        /// </summary>
        public static Sprite GetItemSprite(Data.ItemType itemType, Data.KeyColor keyColor = Data.KeyColor.None)
        {
            return itemType switch
            {
                Data.ItemType.Food => GetFoodSprite(),
                Data.ItemType.Key => GetKeySprite(keyColor),
                Data.ItemType.KeyPiece => GetKeyPieceSprite(),
                Data.ItemType.Treasure => GetTreasureSprite(),
                Data.ItemType.Special => GetSpecialItemSprite(),
                Data.ItemType.GreatKey => GetDiamondSprite("GreatKey", new Color(1f, 1f, 0.5f), 20),
                _ => GetSquareSprite("UnknownItem", Color.white, 16)
            };
        }

        /// <summary>
        /// Gets key sprite with specific color.
        /// </summary>
        public static Sprite GetKeySprite(Data.KeyColor keyColor)
        {
            Color color = keyColor switch
            {
                Data.KeyColor.Red => Color.red,
                Data.KeyColor.Blue => Color.blue,
                Data.KeyColor.Green => Color.green,
                Data.KeyColor.Yellow => Color.yellow,
                Data.KeyColor.Cyan => Color.cyan,
                Data.KeyColor.Magenta => Color.magenta,
                _ => KeyColor
            };
            return GetDiamondSprite($"Key_{keyColor}", color, 16);
        }

        // ==================== Projectile Sprites ====================

        public static Sprite GetMagicProjectileSprite()
        {
            return GetCircleSprite("MagicProjectile", MagicColor, 12);
        }

        public static Sprite GetSwordSwingSprite()
        {
            return GetSquareSprite("SwordSwing", SwordColor, 24);
        }

        public static Sprite GetThrownProjectileSprite()
        {
            return GetCircleSprite("ThrownProjectile", ThrownColor, 10);
        }

        // ==================== Texture Creation ====================

        private static Texture2D CreateSquareTexture(Color color, int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            Color[] pixels = new Color[size * size];
            Color borderColor = color * 0.6f;
            borderColor.a = color.a;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Border (2 pixels)
                    if (x < 2 || x >= size - 2 || y < 2 || y >= size - 2)
                    {
                        pixels[y * size + x] = borderColor;
                    }
                    else
                    {
                        pixels[y * size + x] = color;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private static Texture2D CreateCircleTexture(Color color, int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            Color[] pixels = new Color[size * size];
            Color borderColor = color * 0.6f;
            borderColor.a = color.a;

            float radius = size / 2f;
            float borderRadius = radius - 2f;
            Vector2 center = new Vector2(radius, radius);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);

                    if (dist <= borderRadius)
                    {
                        pixels[y * size + x] = color;
                    }
                    else if (dist <= radius)
                    {
                        pixels[y * size + x] = borderColor;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private static Texture2D CreateDiamondTexture(Color color, int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            Color[] pixels = new Color[size * size];
            Color borderColor = color * 0.6f;
            borderColor.a = color.a;

            float half = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x + 0.5f - half);
                    float dy = Mathf.Abs(y + 0.5f - half);
                    float dist = dx + dy;

                    if (dist <= half - 2)
                    {
                        pixels[y * size + x] = color;
                    }
                    else if (dist <= half)
                    {
                        pixels[y * size + x] = borderColor;
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private static Texture2D CreateTriangleTexture(Color color, int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            Color[] pixels = new Color[size * size];
            Color borderColor = color * 0.6f;
            borderColor.a = color.a;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    // Triangle pointing up
                    float halfWidth = (float)(y + 1) / size * (size / 2f);
                    float centerX = size / 2f;

                    if (x + 0.5f >= centerX - halfWidth && x + 0.5f <= centerX + halfWidth)
                    {
                        // Check if on border
                        float leftEdge = centerX - halfWidth;
                        float rightEdge = centerX + halfWidth;
                        bool onLeftBorder = x + 0.5f < leftEdge + 2;
                        bool onRightBorder = x + 0.5f > rightEdge - 2;
                        bool onBottomBorder = y < 2;
                        bool onTopBorder = y >= size - 3 && Mathf.Abs(x + 0.5f - centerX) < 2;

                        if (onLeftBorder || onRightBorder || onBottomBorder || onTopBorder)
                        {
                            pixels[y * size + x] = borderColor;
                        }
                        else
                        {
                            pixels[y * size + x] = color;
                        }
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Clears the sprite cache (useful for hot reload).
        /// </summary>
        public static void ClearCache()
        {
            foreach (var sprite in _spriteCache.Values)
            {
                if (sprite != null && sprite.texture != null)
                {
                    Object.Destroy(sprite.texture);
                }
            }
            _spriteCache.Clear();
        }
    }
}
