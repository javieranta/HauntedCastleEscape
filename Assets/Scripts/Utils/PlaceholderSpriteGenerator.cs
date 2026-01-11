using UnityEngine;
using System.Collections.Generic;
using HauntedCastle.Visuals;

namespace HauntedCastle.Utils
{
    /// <summary>
    /// Generates placeholder sprites at runtime for development/testing.
    /// Integrates with UltraHDSpriteGenerator (128x128 HD) as primary source.
    /// Falls back to PhotorealisticSpriteGenerator (64x64), EnhancedSpriteGenerator (32x32),
    /// PixelArtGenerator (16x16), or simple shapes.
    /// </summary>
    public static class PlaceholderSpriteGenerator
    {
        private static Dictionary<string, Sprite> _spriteCache = new();
        private static bool _useUltraHDSprites = true; // Use 128x128 Ultra HD sprites (primary - best quality!)
        private static bool _usePhotorealisticSprites = true; // Use 64x64 HD sprites (fallback)
        private static bool _useEnhancedSprites = true; // Use 32x32 enhanced sprites
        private static bool _usePixelArt = true; // Fallback to 16x16 pixel art

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

        // Cache for loaded Midjourney character sprites
        private static Dictionary<string, Sprite> _cachedCharacterSprites = new Dictionary<string, Sprite>();

        public static Sprite GetWizardSprite()
        {
            // PRIORITY 1: User-imported Midjourney sprites (BEST QUALITY)
            if (_cachedCharacterSprites.TryGetValue("wizard", out Sprite cached) && cached != null)
                return cached;

            string[] wizardPaths = new[] {
                "Sprites/Characters/Wizard/wizard_idle",
                "Sprites/Characters/wizard_idle",
                "Sprites/Characters/wizard"
            };

            foreach (string path in wizardPaths)
            {
                Sprite imported = Resources.Load<Sprite>(path);
                if (imported != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] SUCCESS: Loaded wizard sprite from {path}");
                    _cachedCharacterSprites["wizard"] = imported;
                    return imported;
                }

                Texture2D tex = Resources.Load<Texture2D>(path);
                if (tex != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] Loaded wizard as Texture2D: {path}");
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 256f);
                    _cachedCharacterSprites["wizard"] = sprite;
                    return sprite;
                }
            }

            // PRIORITY 2: Procedural fallbacks
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetWizardSprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetWizardSprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetWizardSprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetWizardSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetTriangleSprite("Wizard", WizardColor, 32);
        }

        public static Sprite GetKnightSprite()
        {
            // PRIORITY 1: User-imported Midjourney sprites (BEST QUALITY)
            if (_cachedCharacterSprites.TryGetValue("knight", out Sprite cached) && cached != null)
            {
                Debug.Log($"[PlaceholderSpriteGenerator] Returning cached knight sprite: {cached.name}");
                return cached;
            }

            Debug.Log("[PlaceholderSpriteGenerator] Loading knight sprite...");

            string[] knightPaths = new[] {
                "Sprites/Characters/Knight/knight_idle",
                "Sprites/Characters/knight_idle",
                "Sprites/Characters/knight"
            };

            foreach (string path in knightPaths)
            {
                Debug.Log($"[PlaceholderSpriteGenerator] Trying path: {path}");

                Sprite imported = Resources.Load<Sprite>(path);
                if (imported != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] SUCCESS: Loaded knight sprite from {path}, size: {imported.texture.width}x{imported.texture.height}");
                    _cachedCharacterSprites["knight"] = imported;
                    return imported;
                }

                Texture2D tex = Resources.Load<Texture2D>(path);
                if (tex != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] Loaded knight as Texture2D: {path}, size: {tex.width}x{tex.height}");
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 256f);
                    _cachedCharacterSprites["knight"] = sprite;
                    return sprite;
                }

                Debug.LogWarning($"[PlaceholderSpriteGenerator] Could not load knight from: {path}");
            }

            Debug.LogError("[PlaceholderSpriteGenerator] NO Midjourney knight sprites found! Using procedural fallback.");

            // PRIORITY 2: Procedural fallbacks
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetKnightSprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetKnightSprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetKnightSprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetKnightSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Knight", KnightColor, 32);
        }

        public static Sprite GetSerfSprite()
        {
            // PRIORITY 1: User-imported Midjourney sprites (BEST QUALITY)
            if (_cachedCharacterSprites.TryGetValue("serf", out Sprite cached) && cached != null)
                return cached;

            string[] serfPaths = new[] {
                "Sprites/Characters/Serf/serf_idle",
                "Sprites/Characters/serf_idle",
                "Sprites/Characters/serf"
            };

            foreach (string path in serfPaths)
            {
                Sprite imported = Resources.Load<Sprite>(path);
                if (imported != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] SUCCESS: Loaded serf sprite from {path}");
                    _cachedCharacterSprites["serf"] = imported;
                    return imported;
                }

                Texture2D tex = Resources.Load<Texture2D>(path);
                if (tex != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] Loaded serf as Texture2D: {path}");
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 256f);
                    _cachedCharacterSprites["serf"] = sprite;
                    return sprite;
                }
            }

            // PRIORITY 2: Procedural fallbacks
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetSerfSprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetSerfSprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetSerfSprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetSerfSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetCircleSprite("Serf", SerfColor, 32);
        }

        // ==================== Enemy Sprites ====================

        public static Sprite GetGhostSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetGhostSprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetGhostSprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetGhostSprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetGhostSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetCircleSprite("Ghost", GhostColor, 24);
        }

        public static Sprite GetSkeletonSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetSkeletonSprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetSkeletonSprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetSkeletonSprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetSkeletonSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Skeleton", SkeletonColor, 28);
        }

        public static Sprite GetSpiderSprite()
        {
            // PRIORITY: New HD Smooth sprites (512x512 with proper textures)
            try { return Visuals.HDSmoothSpriteGenerator.GetSpiderSprite(); }
            catch { /* Fall back to other generators */ }

            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetSpiderSprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetSpiderSprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetSpiderSprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetSpiderSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetCircleSprite("Spider", SpiderColor, 20);
        }

        public static Sprite GetBatSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetBatSprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetBatSprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetBatSprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetBatSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetDiamondSprite("Bat", BatColor, 20);
        }

        public static Sprite GetDemonSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetDemonSprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetDemonSprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetDevilSprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetDemonSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetTriangleSprite("Demon", DemonColor, 32);
        }

        public static Sprite GetMummySprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetMummySprite(); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetMummySprite(); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetMummySprite(); }
                catch { /* Fall back to basic pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetMummySprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Mummy", MummyColor, 28);
        }

        public static Sprite GetWitchSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetWitchSprite(); }
                catch { /* Fall back to pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetWitchSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetTriangleSprite("Witch", WitchColor, 28);
        }

        public static Sprite GetVampireSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetVampireSprite(); }
                catch { /* Fall back to pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetVampireSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetDiamondSprite("Vampire", VampireColor, 32);
        }

        public static Sprite GetWerewolfSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetWerewolfSprite(); }
                catch { /* Fall back to pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetWerewolfSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Werewolf", WerewolfColor, 32);
        }

        public static Sprite GetReaperSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetReaperSprite(); }
                catch { /* Fall back to pixel art */ }
            }
            if (_usePixelArt)
            {
                try { return PixelArtGenerator.GetReaperSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetTriangleSprite("Reaper", ReaperColor, 36);
        }

        public static Sprite GetFrankensteinSprite()
        {
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetFrankensteinSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Frankenstein", new Color(0.4f, 0.5f, 0.4f), 32);
        }

        public static Sprite GetHunchbackSprite()
        {
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetHunchbackSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetCircleSprite("Hunchback", new Color(0.7f, 0.65f, 0.55f), 28);
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
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetKeySprite(KeyColor); }
                catch { /* Fall back to simple shape */ }
            }
            return GetDiamondSprite("Key", KeyColor, 16);
        }

        public static Sprite GetKeyPieceSprite()
        {
            return GetTriangleSprite("KeyPiece", KeyPieceColor, 16);
        }

        public static Sprite GetFoodSprite()
        {
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetFoodSprite(); }
                catch { /* Fall back to simple shape */ }
            }
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
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetTreasureSprite(); }
                catch { /* Fall back to simple shape */ }
            }
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
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetMagicProjectileSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetCircleSprite("MagicProjectile", MagicColor, 12);
        }

        public static Sprite GetSwordSwingSprite()
        {
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetSwordSwingSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("SwordSwing", SwordColor, 24);
        }

        public static Sprite GetThrownProjectileSprite()
        {
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetThrownAxeSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetCircleSprite("ThrownProjectile", ThrownColor, 10);
        }

        // ==================== Environment Sprites ====================

        // Cache for loaded Midjourney sprites
        // Per-floor sprite caches
        private static Dictionary<int, Sprite> _cachedFloorSprites = new Dictionary<int, Sprite>();
        private static Dictionary<int, Sprite> _cachedWallSprites = new Dictionary<int, Sprite>();

        public static Sprite GetFloorTileSprite(int floorLevel, int variation = 0)
        {
            // Return cached sprite if already loaded for this floor
            if (_cachedFloorSprites.TryGetValue(floorLevel, out Sprite cached) && cached != null)
            {
                return cached;
            }

            // PRIORITY 1: Floor-specific Midjourney sprites
            // Floor 0 = Dungeon/Basement, Floor 1 = Castle, Floor 2 = Tower
            string[] floorPaths = floorLevel switch
            {
                0 => new[] { "Sprites/Environment/Floors/dungeon_floor", "Sprites/Environment/Floors/stone_floor" },
                1 => new[] { "Sprites/Environment/Floors/stone_floor", "Sprites/Environment/Floors/wood_floor" },
                2 => new[] { "Sprites/Environment/Floors/tower_floor", "Sprites/Environment/Floors/stone_floor" },
                _ => new[] { "Sprites/Environment/Floors/stone_floor", "Sprites/Environment/Floors/wood_floor" }
            };

            foreach (string path in floorPaths)
            {
                // Try loading as Sprite first
                Sprite imported = Resources.Load<Sprite>(path);
                if (imported != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] SUCCESS: Loaded floor sprite for level {floorLevel} from {path}");
                    Debug.Log($"  -> Texture: {imported.texture.name}, Size: {imported.texture.width}x{imported.texture.height}");
                    _cachedFloorSprites[floorLevel] = imported;
                    return imported;
                }

                // Try loading as Texture2D and convert to Sprite
                Texture2D tex = Resources.Load<Texture2D>(path);
                if (tex != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] Loaded floor as Texture2D for level {floorLevel}: {path}");
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 256f);
                    _cachedFloorSprites[floorLevel] = sprite;
                    return sprite;
                }
            }

            Debug.LogWarning($"[PlaceholderSpriteGenerator] No floor sprites found for level {floorLevel}, using procedural fallback.");

            // PRIORITY 2: HD Smooth procedural sprites (512x512)
            try { return Visuals.HDSmoothSpriteGenerator.GetStoneFloorTile(variation + floorLevel * 10); }
            catch { /* Fall back to other generators */ }

            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetFloorTileSprite(floorLevel, variation); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetFloorTileSprite(floorLevel, variation); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetFloorTile(floorLevel, variation); }
                catch { /* Fall back to simple shape */ }
            }
            Color floorColor = floorLevel switch
            {
                0 => new Color(0.25f, 0.22f, 0.2f),  // Basement
                1 => new Color(0.45f, 0.35f, 0.25f), // Castle
                2 => new Color(0.5f, 0.48f, 0.45f),  // Tower
                _ => new Color(0.4f, 0.35f, 0.3f)
            };
            return GetSquareSprite($"Floor_{floorLevel}_{variation}", floorColor, 32);
        }

        public static Sprite GetWallSprite(int floorLevel, bool isVertical = false)
        {
            // Return cached sprite if already loaded for this floor
            if (_cachedWallSprites.TryGetValue(floorLevel, out Sprite cached) && cached != null)
            {
                return cached;
            }

            // PRIORITY 1: Floor-specific Midjourney sprites
            // Floor 0 = Dungeon (dungeon_wall), Floor 1 = Castle (stone_wall), Floor 2 = Tower (brick_wall)
            string[] wallPaths = floorLevel switch
            {
                0 => new[] { "Sprites/Environment/Walls/dungeon_wall", "Sprites/Environment/Walls/stone_wall" },
                1 => new[] { "Sprites/Environment/Walls/stone_wall", "Sprites/Environment/Walls/brick_wall" },
                2 => new[] { "Sprites/Environment/Walls/brick_wall", "Sprites/Environment/Walls/stone_wall" },
                _ => new[] { "Sprites/Environment/Walls/stone_wall", "Sprites/Environment/Walls/dungeon_wall" }
            };

            foreach (string path in wallPaths)
            {
                // Try loading as Sprite first
                Sprite imported = Resources.Load<Sprite>(path);
                if (imported != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] SUCCESS: Loaded wall sprite for level {floorLevel} from {path}");
                    Debug.Log($"  -> Texture: {imported.texture.name}, Size: {imported.texture.width}x{imported.texture.height}");
                    _cachedWallSprites[floorLevel] = imported;
                    return imported;
                }

                // Try loading as Texture2D and convert to Sprite
                Texture2D tex = Resources.Load<Texture2D>(path);
                if (tex != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] Loaded wall as Texture2D for level {floorLevel}: {path}");
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 256f);
                    _cachedWallSprites[floorLevel] = sprite;
                    return sprite;
                }
            }

            Debug.LogWarning($"[PlaceholderSpriteGenerator] No wall sprites found for level {floorLevel}, using procedural fallback.");

            // PRIORITY 2: HD Smooth procedural sprites (512x512)
            try { return Visuals.HDSmoothSpriteGenerator.GetStoneBrickWall(floorLevel); }
            catch { /* Fall back to other generators */ }

            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetWallSprite(floorLevel, isVertical); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetWallSprite(floorLevel, isVertical); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetWallSprite(floorLevel, isVertical); }
                catch { /* Fall back to simple shape */ }
            }
            Color wallColor = floorLevel switch
            {
                0 => new Color(0.2f, 0.18f, 0.15f),  // Basement
                1 => new Color(0.4f, 0.38f, 0.35f),  // Castle
                2 => new Color(0.45f, 0.43f, 0.4f),  // Tower
                _ => new Color(0.35f, 0.33f, 0.3f)
            };
            return GetSquareSprite($"Wall_{floorLevel}_{isVertical}", wallColor, 32);
        }

        // Per-door-type sprite caches (wooden, iron, secret)
        private static Dictionary<string, Sprite> _cachedDoorSprites = new Dictionary<string, Sprite>();

        /// <summary>
        /// Clears all sprite caches. Call this when sprites are updated.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ClearAllCaches()
        {
            _cachedFloorSprites.Clear();
            _cachedWallSprites.Clear();
            _cachedDoorSprites.Clear();
            _cachedCharacterSprites.Clear();
            Debug.Log("[PlaceholderSpriteGenerator] Cleared all sprite caches (floors, walls, doors, characters)");
        }

        public static Sprite GetDoorSprite(bool isOpen, string keyType = "")
        {
            // For open doors, return a dark/transparent sprite (doorway)
            if (isOpen)
            {
                return GetSquareSprite("Door_Open", new Color(0.08f, 0.06f, 0.04f, 0.5f), 32);
            }

            // Determine door type from keyType (wooden = default, iron = locked, secret = hidden)
            string doorType = keyType.ToLower() switch
            {
                "iron" or "locked" or "red" or "blue" or "green" or "yellow" => "iron",
                "secret" or "hidden" or "passage" => "secret",
                _ => "wooden"
            };

            // Return cached sprite if already loaded for this door type
            if (_cachedDoorSprites.TryGetValue(doorType, out Sprite cached) && cached != null)
            {
                return cached;
            }

            // PRIORITY 1: Door type-specific Midjourney sprites
            string[] doorPaths = doorType switch
            {
                "iron" => new[] { "Sprites/Environment/Doors/iron_door", "Sprites/Environment/Doors/wooden_door" },
                "secret" => new[] { "Sprites/Environment/Doors/secret_door", "Sprites/Environment/Doors/wooden_door" },
                _ => new[] { "Sprites/Environment/Doors/wooden_door", "Sprites/Environment/Doors/door", "Sprites/Environment/Doors/castle_door" }
            };

            foreach (string path in doorPaths)
            {
                // Try loading as Sprite first
                Sprite imported = Resources.Load<Sprite>(path);
                if (imported != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] SUCCESS: Loaded {doorType} door sprite from {path}");
                    Debug.Log($"  -> Texture: {imported.texture.name}, Size: {imported.texture.width}x{imported.texture.height}");
                    _cachedDoorSprites[doorType] = imported;
                    return imported;
                }

                // Try loading as Texture2D and convert to Sprite
                Texture2D tex = Resources.Load<Texture2D>(path);
                if (tex != null)
                {
                    Debug.Log($"[PlaceholderSpriteGenerator] Loaded {doorType} door as Texture2D: {path}");
                    Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 256f);
                    _cachedDoorSprites[doorType] = sprite;
                    return sprite;
                }
            }

            Debug.LogWarning($"[PlaceholderSpriteGenerator] No Midjourney {doorType} door sprite found, using procedural fallback");

            // PRIORITY 2: HD Smooth sprites (procedural fallback)
            try { return Visuals.HDSmoothSpriteGenerator.GetDoorSprite(isOpen); }
            catch { /* Fall back to other generators */ }

            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetDoorSprite(isOpen, keyType); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetDoorSprite(isOpen, keyType); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetDoorSprite(isOpen, keyType); }
                catch { /* Fall back to simple shape */ }
            }
            Color doorColor = new Color(0.45f, 0.3f, 0.2f);
            return GetSquareSprite($"Door_{isOpen}_{keyType}", doorColor, 32);
        }

        public static Sprite GetStaircaseSprite(bool goingUp)
        {
            // PRIORITY: Ultra HD sprites with large, clear UP/DOWN indicators
            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetStaircaseSprite(goingUp); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetStaircaseSprite(goingUp); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetStaircaseSprite(goingUp); }
                catch { /* Fall back to simple shape */ }
            }
            return GetTriangleSprite($"Stairs_{goingUp}", new Color(0.5f, 0.5f, 0.55f), 32);
        }

        public static Sprite GetTorchSprite(int frame = 0)
        {
            // PRIORITY: New HD Smooth sprites (512x512 with proper textures)
            try { return Visuals.HDSmoothSpriteGenerator.GetTorchSprite(); }
            catch { /* Fall back to other generators */ }

            if (_useUltraHDSprites)
            {
                try { return UltraHDSpriteGenerator.GetTorchSprite(frame); }
                catch { /* Fall back to photorealistic sprites */ }
            }
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetTorchSprite(frame); }
                catch { /* Fall back to enhanced sprites */ }
            }
            if (_useEnhancedSprites)
            {
                try { return EnhancedSpriteGenerator.GetTorchSprite(frame); }
                catch { /* Fall back to simple shape */ }
            }
            return GetDiamondSprite($"Torch_{frame}", new Color(1f, 0.6f, 0.2f), 16);
        }

        // ==================== Decoration Sprites ====================

        public static Sprite GetChainSprite()
        {
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetChainSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Chain", new Color(0.35f, 0.32f, 0.28f), 16);
        }

        public static Sprite GetCobwebSprite()
        {
            // PRIORITY: New HD Smooth sprites (512x512 with proper textures)
            try { return Visuals.HDSmoothSpriteGenerator.GetCobwebSprite(); }
            catch { /* Fall back to other generators */ }

            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetCobwebSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Cobweb", new Color(0.9f, 0.9f, 0.88f, 0.5f), 24);
        }

        public static Sprite GetSkullSprite()
        {
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetSkullSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetCircleSprite("Skull", new Color(0.9f, 0.88f, 0.82f), 16);
        }

        public static Sprite GetBarrelSprite()
        {
            // PRIORITY: New HD Smooth sprites (512x512 with proper textures)
            try { return Visuals.HDSmoothSpriteGenerator.GetBarrelSprite(); }
            catch { /* Fall back to other generators */ }

            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetBarrelSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Barrel", new Color(0.5f, 0.35f, 0.2f), 24);
        }

        public static Sprite GetChandelierSprite()
        {
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetChandelierSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetDiamondSprite("Chandelier", new Color(0.75f, 0.6f, 0.25f), 32);
        }

        public static Sprite GetPaintingSprite(int variant = 0)
        {
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetPaintingSprite(variant); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite($"Painting_{variant}", new Color(0.45f, 0.32f, 0.15f), 24);
        }

        public static Sprite GetArmorStandSprite()
        {
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetArmorStandSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("ArmorStand", new Color(0.55f, 0.52f, 0.48f), 32);
        }

        public static Sprite GetBannerSprite(int variant = 0)
        {
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetBannerSprite(variant); }
                catch { /* Fall back to simple shape */ }
            }
            Color[] colors = { Color.red, Color.blue, Color.green, new Color(0.8f, 0.7f, 0.2f) };
            return GetSquareSprite($"Banner_{variant}", colors[variant % 4], 16);
        }

        public static Sprite GetWindowSprite()
        {
            if (_usePhotorealisticSprites)
            {
                try { return PhotorealisticSpriteGenerator.GetWindowSprite(); }
                catch { /* Fall back to simple shape */ }
            }
            return GetSquareSprite("Window", new Color(0.3f, 0.4f, 0.55f, 0.7f), 24);
        }

        // ==================== Texture Creation ====================

        private static Texture2D CreateSquareTexture(Color color, int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Bilinear;

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
            texture.filterMode = FilterMode.Bilinear;

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
            texture.filterMode = FilterMode.Bilinear;

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
            texture.filterMode = FilterMode.Bilinear;

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
