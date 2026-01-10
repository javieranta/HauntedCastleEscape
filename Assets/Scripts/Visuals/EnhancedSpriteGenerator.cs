using System.Collections.Generic;
using UnityEngine;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Enhanced sprite generator creating detailed pixel art for the game.
    /// All sprites are 32x32 pixels with rich detail and proper shading.
    /// </summary>
    public static class EnhancedSpriteGenerator
    {
        private static Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

        // Color palettes for different themes
        public static class Palettes
        {
            // Character colors
            public static readonly Color WizardRobe = new Color(0.4f, 0.2f, 0.6f);
            public static readonly Color WizardRobeLight = new Color(0.55f, 0.35f, 0.75f);
            public static readonly Color WizardRobeDark = new Color(0.25f, 0.1f, 0.4f);
            public static readonly Color WizardSkin = new Color(0.95f, 0.85f, 0.75f);
            public static readonly Color WizardBeard = new Color(0.8f, 0.8f, 0.85f);
            public static readonly Color WizardHat = new Color(0.3f, 0.15f, 0.5f);

            public static readonly Color KnightArmor = new Color(0.7f, 0.7f, 0.75f);
            public static readonly Color KnightArmorLight = new Color(0.85f, 0.85f, 0.9f);
            public static readonly Color KnightArmorDark = new Color(0.45f, 0.45f, 0.5f);
            public static readonly Color KnightPlume = new Color(0.8f, 0.2f, 0.2f);
            public static readonly Color KnightSword = new Color(0.8f, 0.8f, 0.85f);

            public static readonly Color SerfClothes = new Color(0.55f, 0.4f, 0.25f);
            public static readonly Color SerfClothesLight = new Color(0.7f, 0.55f, 0.35f);
            public static readonly Color SerfClothesDark = new Color(0.35f, 0.25f, 0.15f);
            public static readonly Color SerfSkin = new Color(0.9f, 0.75f, 0.6f);
            public static readonly Color SerfHair = new Color(0.4f, 0.25f, 0.15f);

            // Floor themes
            public static readonly Color BasementStone = new Color(0.25f, 0.22f, 0.2f);
            public static readonly Color BasementStoneDark = new Color(0.15f, 0.12f, 0.1f);
            public static readonly Color BasementStoneLight = new Color(0.35f, 0.32f, 0.3f);
            public static readonly Color BasementMoss = new Color(0.2f, 0.35f, 0.15f);
            public static readonly Color BasementWater = new Color(0.15f, 0.25f, 0.35f);

            public static readonly Color CastleFloor = new Color(0.45f, 0.35f, 0.25f);
            public static readonly Color CastleFloorDark = new Color(0.3f, 0.22f, 0.15f);
            public static readonly Color CastleFloorLight = new Color(0.6f, 0.48f, 0.35f);
            public static readonly Color CastleCarpet = new Color(0.6f, 0.15f, 0.15f);
            public static readonly Color CastleCarpetDark = new Color(0.4f, 0.1f, 0.1f);

            public static readonly Color TowerStone = new Color(0.5f, 0.48f, 0.45f);
            public static readonly Color TowerStoneDark = new Color(0.35f, 0.33f, 0.3f);
            public static readonly Color TowerStoneLight = new Color(0.65f, 0.62f, 0.6f);
            public static readonly Color TowerSky = new Color(0.4f, 0.5f, 0.7f);

            // Wall colors
            public static readonly Color WallStone = new Color(0.4f, 0.38f, 0.35f);
            public static readonly Color WallStoneDark = new Color(0.25f, 0.23f, 0.2f);
            public static readonly Color WallStoneLight = new Color(0.55f, 0.52f, 0.5f);
            public static readonly Color WallMortar = new Color(0.35f, 0.32f, 0.28f);

            // Item colors
            public static readonly Color GoldKey = new Color(0.95f, 0.8f, 0.2f);
            public static readonly Color GoldKeyDark = new Color(0.7f, 0.55f, 0.1f);
            public static readonly Color SilverKey = new Color(0.8f, 0.8f, 0.85f);
            public static readonly Color CopperKey = new Color(0.8f, 0.5f, 0.3f);
            public static readonly Color Food = new Color(0.8f, 0.6f, 0.4f);
            public static readonly Color FoodMeat = new Color(0.7f, 0.3f, 0.25f);

            // Enemy colors
            public static readonly Color GhostBody = new Color(0.85f, 0.9f, 0.95f, 0.7f);
            public static readonly Color GhostEyes = new Color(0.2f, 0.2f, 0.3f);
            public static readonly Color SkeletonBone = new Color(0.9f, 0.88f, 0.8f);
            public static readonly Color SpiderBody = new Color(0.2f, 0.15f, 0.1f);
            public static readonly Color SpiderEyes = new Color(0.8f, 0.2f, 0.2f);
            public static readonly Color BatWing = new Color(0.25f, 0.2f, 0.25f);
            public static readonly Color BatEyes = new Color(1f, 0.8f, 0.2f);

            // Magic/Effects
            public static readonly Color MagicBlue = new Color(0.3f, 0.5f, 1f);
            public static readonly Color MagicPurple = new Color(0.7f, 0.3f, 0.9f);
            public static readonly Color MagicGreen = new Color(0.3f, 0.9f, 0.4f);
            public static readonly Color Fire = new Color(1f, 0.6f, 0.2f);
            public static readonly Color FireBright = new Color(1f, 0.9f, 0.4f);
        }

        #region Character Sprites

        public static Sprite GetWizardSprite(int frame = 0)
        {
            string key = $"Wizard_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Draw wizard body (robe)
            DrawFilledEllipse(tex, 16, 8, 10, 8, Palettes.WizardRobe);
            DrawFilledEllipse(tex, 16, 9, 9, 7, Palettes.WizardRobeLight);
            DrawFilledEllipse(tex, 16, 7, 8, 5, Palettes.WizardRobeDark);

            // Draw robe folds
            for (int i = 0; i < 3; i++)
            {
                tex.SetPixel(12 + i * 4, 4, Palettes.WizardRobeDark);
                tex.SetPixel(12 + i * 4, 5, Palettes.WizardRobeDark);
            }

            // Draw head
            DrawFilledCircle(tex, 16, 20, 6, Palettes.WizardSkin);
            DrawFilledCircle(tex, 16, 19, 5, new Color(0.98f, 0.88f, 0.78f));

            // Draw wizard hat (pointed)
            DrawTriangle(tex, 16, 31, 10, 27, 22, 27, Palettes.WizardHat);
            DrawLine(tex, 10, 27, 22, 27, Palettes.WizardHat);
            DrawFilledRect(tex, 10, 25, 12, 2, Palettes.WizardHat);

            // Hat band
            DrawLine(tex, 11, 26, 21, 26, Palettes.GoldKey);

            // Draw beard
            DrawFilledEllipse(tex, 16, 14, 4, 4, Palettes.WizardBeard);
            tex.SetPixel(14, 12, Palettes.WizardBeard);
            tex.SetPixel(18, 12, Palettes.WizardBeard);

            // Draw eyes
            tex.SetPixel(14, 21, Color.black);
            tex.SetPixel(18, 21, Color.black);
            tex.SetPixel(14, 22, Color.white);
            tex.SetPixel(18, 22, Color.white);

            // Draw staff
            DrawLine(tex, 24, 2, 26, 24, new Color(0.5f, 0.3f, 0.2f));
            DrawFilledCircle(tex, 26, 25, 2, Palettes.MagicBlue);
            tex.SetPixel(26, 26, Palettes.MagicPurple);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetKnightSprite(int frame = 0)
        {
            string key = $"Knight_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Draw armor body
            DrawFilledEllipse(tex, 16, 10, 9, 9, Palettes.KnightArmor);
            DrawFilledEllipse(tex, 16, 11, 8, 8, Palettes.KnightArmorLight);
            DrawFilledEllipse(tex, 16, 9, 7, 6, Palettes.KnightArmorDark);

            // Armor details - chest plate
            DrawLine(tex, 13, 14, 13, 6, Palettes.KnightArmorDark);
            DrawLine(tex, 19, 14, 19, 6, Palettes.KnightArmorDark);
            DrawLine(tex, 14, 10, 18, 10, Palettes.KnightArmorLight);

            // Draw helmet
            DrawFilledCircle(tex, 16, 22, 6, Palettes.KnightArmor);
            DrawFilledCircle(tex, 16, 23, 5, Palettes.KnightArmorLight);

            // Helmet visor
            DrawFilledRect(tex, 12, 20, 8, 3, Palettes.KnightArmorDark);
            DrawLine(tex, 13, 21, 19, 21, Color.black);
            DrawLine(tex, 16, 19, 16, 22, Palettes.KnightArmorDark);

            // Plume on helmet
            DrawFilledEllipse(tex, 16, 28, 2, 3, Palettes.KnightPlume);
            tex.SetPixel(15, 30, new Color(0.9f, 0.3f, 0.3f));
            tex.SetPixel(17, 30, new Color(0.9f, 0.3f, 0.3f));

            // Draw sword
            DrawLine(tex, 24, 2, 24, 18, Palettes.KnightSword);
            DrawLine(tex, 25, 2, 25, 18, Palettes.KnightArmorLight);
            // Sword handle
            DrawFilledRect(tex, 23, 18, 4, 3, new Color(0.4f, 0.25f, 0.15f));
            // Sword crossguard
            DrawLine(tex, 21, 17, 27, 17, Palettes.GoldKey);

            // Draw shield
            DrawFilledEllipse(tex, 6, 12, 4, 6, Palettes.KnightArmor);
            DrawFilledEllipse(tex, 6, 12, 3, 5, Palettes.KnightPlume);
            tex.SetPixel(6, 12, Palettes.GoldKey);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetSerfSprite(int frame = 0)
        {
            string key = $"Serf_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Draw body (tunic)
            DrawFilledEllipse(tex, 16, 9, 8, 8, Palettes.SerfClothes);
            DrawFilledEllipse(tex, 16, 10, 7, 7, Palettes.SerfClothesLight);
            DrawFilledEllipse(tex, 16, 8, 6, 5, Palettes.SerfClothesDark);

            // Belt
            DrawLine(tex, 10, 8, 22, 8, new Color(0.3f, 0.2f, 0.1f));
            tex.SetPixel(16, 8, Palettes.GoldKeyDark);

            // Draw legs
            DrawFilledRect(tex, 12, 2, 3, 5, Palettes.SerfClothesDark);
            DrawFilledRect(tex, 17, 2, 3, 5, Palettes.SerfClothesDark);

            // Draw head
            DrawFilledCircle(tex, 16, 20, 5, Palettes.SerfSkin);
            DrawFilledCircle(tex, 16, 21, 4, new Color(0.92f, 0.78f, 0.63f));

            // Draw hair
            DrawFilledEllipse(tex, 16, 24, 5, 3, Palettes.SerfHair);
            tex.SetPixel(12, 22, Palettes.SerfHair);
            tex.SetPixel(20, 22, Palettes.SerfHair);

            // Draw eyes
            tex.SetPixel(14, 21, Color.black);
            tex.SetPixel(18, 21, Color.black);
            tex.SetPixel(14, 22, Color.white);
            tex.SetPixel(18, 22, Color.white);

            // Draw smile
            tex.SetPixel(15, 18, new Color(0.7f, 0.5f, 0.45f));
            tex.SetPixel(16, 18, new Color(0.7f, 0.5f, 0.45f));
            tex.SetPixel(17, 18, new Color(0.7f, 0.5f, 0.45f));

            // Draw throwing axe
            DrawLine(tex, 24, 8, 24, 18, new Color(0.5f, 0.35f, 0.2f));
            // Axe head
            DrawFilledRect(tex, 25, 14, 4, 5, Palettes.KnightArmor);
            DrawLine(tex, 28, 14, 28, 18, Palettes.KnightArmorDark);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Enemy Sprites

        public static Sprite GetGhostSprite(int frame = 0)
        {
            string key = $"Ghost_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Ghost body - wavy bottom
            for (int x = 6; x < 26; x++)
            {
                int waveOffset = (int)(Mathf.Sin(x * 0.8f + frame) * 2);
                for (int y = 4 + waveOffset; y < 24; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                    if (dist < 12)
                    {
                        Color c = Palettes.GhostBody;
                        c.a = 0.5f + (1 - dist / 12) * 0.4f;
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            // Ghost face
            DrawFilledCircle(tex, 16, 18, 8, new Color(0.9f, 0.95f, 1f, 0.85f));

            // Eyes - hollow and spooky
            DrawFilledCircle(tex, 12, 19, 3, Palettes.GhostEyes);
            DrawFilledCircle(tex, 20, 19, 3, Palettes.GhostEyes);
            tex.SetPixel(12, 20, new Color(0.4f, 0.4f, 0.6f));
            tex.SetPixel(20, 20, new Color(0.4f, 0.4f, 0.6f));

            // Mouth - wailing
            DrawFilledEllipse(tex, 16, 13, 3, 2, Palettes.GhostEyes);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetSkeletonSprite(int frame = 0)
        {
            string key = $"Skeleton_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Skull
            DrawFilledCircle(tex, 16, 24, 6, Palettes.SkeletonBone);
            DrawFilledCircle(tex, 16, 23, 5, new Color(0.95f, 0.92f, 0.85f));

            // Eye sockets
            DrawFilledCircle(tex, 13, 24, 2, Color.black);
            DrawFilledCircle(tex, 19, 24, 2, Color.black);
            tex.SetPixel(13, 25, new Color(0.3f, 0.1f, 0.1f));
            tex.SetPixel(19, 25, new Color(0.3f, 0.1f, 0.1f));

            // Nose hole
            tex.SetPixel(16, 22, Color.black);
            tex.SetPixel(16, 21, Color.black);

            // Teeth
            for (int i = 13; i <= 19; i++)
            {
                tex.SetPixel(i, 19, Palettes.SkeletonBone);
                if (i % 2 == 0) tex.SetPixel(i, 18, Color.black);
            }

            // Spine
            for (int y = 8; y < 18; y++)
            {
                tex.SetPixel(16, y, Palettes.SkeletonBone);
                if (y % 2 == 0)
                {
                    tex.SetPixel(15, y, new Color(0.8f, 0.78f, 0.7f));
                    tex.SetPixel(17, y, new Color(0.8f, 0.78f, 0.7f));
                }
            }

            // Ribs
            for (int i = 0; i < 4; i++)
            {
                int y = 14 - i * 2;
                DrawLine(tex, 10, y, 16, y + 1, Palettes.SkeletonBone);
                DrawLine(tex, 16, y + 1, 22, y, Palettes.SkeletonBone);
            }

            // Arms
            DrawLine(tex, 8, 10, 10, 16, Palettes.SkeletonBone);
            DrawLine(tex, 24, 10, 22, 16, Palettes.SkeletonBone);
            // Hands
            tex.SetPixel(7, 9, Palettes.SkeletonBone);
            tex.SetPixel(8, 8, Palettes.SkeletonBone);
            tex.SetPixel(25, 9, Palettes.SkeletonBone);
            tex.SetPixel(24, 8, Palettes.SkeletonBone);

            // Legs
            DrawLine(tex, 14, 2, 14, 8, Palettes.SkeletonBone);
            DrawLine(tex, 18, 2, 18, 8, Palettes.SkeletonBone);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetSpiderSprite(int frame = 0)
        {
            string key = $"Spider_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Body
            DrawFilledEllipse(tex, 16, 12, 6, 5, Palettes.SpiderBody);
            DrawFilledEllipse(tex, 16, 18, 4, 3, Palettes.SpiderBody);

            // Body markings
            tex.SetPixel(16, 14, new Color(0.4f, 0.1f, 0.1f));
            tex.SetPixel(15, 13, new Color(0.4f, 0.1f, 0.1f));
            tex.SetPixel(17, 13, new Color(0.4f, 0.1f, 0.1f));

            // Eyes (8 eyes!)
            tex.SetPixel(14, 19, Palettes.SpiderEyes);
            tex.SetPixel(18, 19, Palettes.SpiderEyes);
            tex.SetPixel(13, 18, Palettes.SpiderEyes);
            tex.SetPixel(19, 18, Palettes.SpiderEyes);
            tex.SetPixel(15, 20, Palettes.SpiderEyes);
            tex.SetPixel(17, 20, Palettes.SpiderEyes);
            tex.SetPixel(14, 17, new Color(0.4f, 0.1f, 0.1f));
            tex.SetPixel(18, 17, new Color(0.4f, 0.1f, 0.1f));

            // Legs (8 legs)
            int legWave = frame % 2;
            // Left legs
            DrawLine(tex, 10, 14, 4, 18 + legWave, Palettes.SpiderBody);
            DrawLine(tex, 10, 12, 3, 14 + legWave, Palettes.SpiderBody);
            DrawLine(tex, 10, 10, 4, 8 - legWave, Palettes.SpiderBody);
            DrawLine(tex, 10, 8, 5, 4 - legWave, Palettes.SpiderBody);
            // Right legs
            DrawLine(tex, 22, 14, 28, 18 - legWave, Palettes.SpiderBody);
            DrawLine(tex, 22, 12, 29, 14 - legWave, Palettes.SpiderBody);
            DrawLine(tex, 22, 10, 28, 8 + legWave, Palettes.SpiderBody);
            DrawLine(tex, 22, 8, 27, 4 + legWave, Palettes.SpiderBody);

            // Fangs
            tex.SetPixel(14, 15, new Color(0.3f, 0.3f, 0.3f));
            tex.SetPixel(18, 15, new Color(0.3f, 0.3f, 0.3f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetBatSprite(int frame = 0)
        {
            string key = $"Bat_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            int wingY = (frame % 2 == 0) ? 2 : -2;

            // Body
            DrawFilledEllipse(tex, 16, 16, 4, 5, Palettes.BatWing);

            // Head
            DrawFilledCircle(tex, 16, 22, 3, Palettes.BatWing);

            // Ears
            DrawTriangle(tex, 12, 26, 13, 23, 14, 26, Palettes.BatWing);
            DrawTriangle(tex, 20, 26, 18, 23, 19, 26, Palettes.BatWing);

            // Eyes
            tex.SetPixel(14, 22, Palettes.BatEyes);
            tex.SetPixel(18, 22, Palettes.BatEyes);

            // Wings
            for (int x = 0; x < 12; x++)
            {
                int wingHeight = 6 - Mathf.Abs(x - 6);
                for (int y = 0; y < wingHeight; y++)
                {
                    tex.SetPixel(4 + x, 14 + y + wingY, Palettes.BatWing);
                    tex.SetPixel(28 - x, 14 + y + wingY, Palettes.BatWing);
                }
            }

            // Wing membrane lines
            DrawLine(tex, 4, 14 + wingY, 12, 18 + wingY, new Color(0.35f, 0.3f, 0.35f));
            DrawLine(tex, 6, 14 + wingY, 12, 16 + wingY, new Color(0.35f, 0.3f, 0.35f));
            DrawLine(tex, 28, 14 + wingY, 20, 18 + wingY, new Color(0.35f, 0.3f, 0.35f));
            DrawLine(tex, 26, 14 + wingY, 20, 16 + wingY, new Color(0.35f, 0.3f, 0.35f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetFrankensteinSprite(int frame = 0)
        {
            string key = $"Frank_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Body - large and hulking
            DrawFilledRect(tex, 8, 4, 16, 14, new Color(0.3f, 0.35f, 0.3f));
            DrawFilledRect(tex, 9, 5, 14, 12, new Color(0.35f, 0.4f, 0.35f));

            // Head - flat top
            DrawFilledRect(tex, 10, 18, 12, 10, new Color(0.4f, 0.5f, 0.4f));
            DrawFilledRect(tex, 11, 19, 10, 8, new Color(0.45f, 0.55f, 0.45f));

            // Flat top hair
            DrawFilledRect(tex, 10, 27, 12, 3, Color.black);

            // Bolts
            tex.SetPixel(9, 22, Palettes.KnightArmor);
            tex.SetPixel(8, 22, Palettes.KnightArmor);
            tex.SetPixel(23, 22, Palettes.KnightArmor);
            tex.SetPixel(24, 22, Palettes.KnightArmor);

            // Eyes
            tex.SetPixel(13, 23, Color.black);
            tex.SetPixel(19, 23, Color.black);
            tex.SetPixel(13, 24, new Color(0.8f, 0.8f, 0.2f));
            tex.SetPixel(19, 24, new Color(0.8f, 0.8f, 0.2f));

            // Scar
            DrawLine(tex, 15, 26, 17, 20, new Color(0.5f, 0.3f, 0.3f));

            // Mouth - stitched
            for (int i = 13; i <= 19; i += 2)
            {
                tex.SetPixel(i, 19, Color.black);
            }

            // Arms
            DrawFilledRect(tex, 4, 6, 4, 10, new Color(0.35f, 0.4f, 0.35f));
            DrawFilledRect(tex, 24, 6, 4, 10, new Color(0.35f, 0.4f, 0.35f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetMummySprite(int frame = 0)
        {
            string key = $"Mummy_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color bandage = new Color(0.85f, 0.8f, 0.7f);
            Color bandageDark = new Color(0.65f, 0.6f, 0.5f);

            // Body wrapped in bandages
            DrawFilledEllipse(tex, 16, 12, 7, 10, bandage);

            // Horizontal wrapping lines
            for (int y = 4; y < 22; y += 3)
            {
                DrawLine(tex, 10, y, 22, y, bandageDark);
            }

            // Head
            DrawFilledCircle(tex, 16, 24, 5, bandage);

            // Head wrappings
            DrawLine(tex, 11, 26, 21, 26, bandageDark);
            DrawLine(tex, 11, 23, 21, 23, bandageDark);
            DrawLine(tex, 12, 20, 20, 20, bandageDark);

            // Glowing eyes
            tex.SetPixel(14, 24, new Color(0.2f, 0.8f, 0.2f));
            tex.SetPixel(18, 24, new Color(0.2f, 0.8f, 0.2f));
            tex.SetPixel(14, 25, new Color(0.3f, 0.9f, 0.3f));
            tex.SetPixel(18, 25, new Color(0.3f, 0.9f, 0.3f));

            // Arms outstretched
            DrawFilledRect(tex, 2, 12, 6, 3, bandage);
            DrawFilledRect(tex, 24, 12, 6, 3, bandage);
            DrawLine(tex, 2, 13, 8, 13, bandageDark);
            DrawLine(tex, 24, 13, 30, 13, bandageDark);

            // Loose bandage strips
            DrawLine(tex, 8, 4, 6, 2, bandage);
            DrawLine(tex, 24, 8, 26, 6, bandage);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetDevilSprite(int frame = 0)
        {
            string key = $"Devil_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color devilRed = new Color(0.8f, 0.2f, 0.15f);
            Color devilDark = new Color(0.5f, 0.1f, 0.1f);

            // Body
            DrawFilledEllipse(tex, 16, 10, 8, 9, devilRed);
            DrawFilledEllipse(tex, 16, 11, 7, 8, new Color(0.9f, 0.3f, 0.2f));

            // Head
            DrawFilledCircle(tex, 16, 22, 6, devilRed);

            // Horns
            DrawTriangle(tex, 8, 28, 10, 24, 12, 28, devilDark);
            DrawTriangle(tex, 24, 28, 20, 24, 22, 28, devilDark);

            // Evil eyes
            DrawFilledEllipse(tex, 13, 22, 2, 1, Color.black);
            DrawFilledEllipse(tex, 19, 22, 2, 1, Color.black);
            tex.SetPixel(13, 22, new Color(1f, 0.8f, 0.2f));
            tex.SetPixel(19, 22, new Color(1f, 0.8f, 0.2f));

            // Evil grin
            DrawLine(tex, 12, 18, 16, 17, Color.black);
            DrawLine(tex, 16, 17, 20, 18, Color.black);
            // Fangs
            tex.SetPixel(13, 17, Color.white);
            tex.SetPixel(19, 17, Color.white);

            // Goatee
            DrawTriangle(tex, 16, 14, 14, 18, 18, 18, devilDark);

            // Pitchfork
            DrawLine(tex, 26, 2, 26, 22, new Color(0.3f, 0.2f, 0.1f));
            // Prongs
            DrawLine(tex, 24, 20, 24, 24, devilDark);
            DrawLine(tex, 26, 22, 26, 26, devilDark);
            DrawLine(tex, 28, 20, 28, 24, devilDark);

            // Tail
            DrawLine(tex, 8, 8, 4, 4, devilRed);
            DrawTriangle(tex, 4, 6, 2, 2, 6, 2, devilDark);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetHunchbackSprite(int frame = 0)
        {
            string key = $"Hunchback_{frame}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color skin = new Color(0.7f, 0.65f, 0.55f);
            Color clothes = new Color(0.3f, 0.25f, 0.2f);

            // Hunched body
            DrawFilledEllipse(tex, 14, 14, 8, 10, clothes);
            // Hump
            DrawFilledCircle(tex, 18, 18, 5, clothes);

            // Head (tilted)
            DrawFilledCircle(tex, 10, 22, 5, skin);
            DrawFilledCircle(tex, 10, 21, 4, new Color(0.75f, 0.7f, 0.6f));

            // Messy hair
            tex.SetPixel(8, 26, new Color(0.3f, 0.25f, 0.2f));
            tex.SetPixel(10, 27, new Color(0.3f, 0.25f, 0.2f));
            tex.SetPixel(12, 26, new Color(0.3f, 0.25f, 0.2f));
            tex.SetPixel(9, 27, new Color(0.3f, 0.25f, 0.2f));

            // One eye bigger than other
            tex.SetPixel(8, 22, Color.black);
            DrawFilledCircle(tex, 12, 22, 1, Color.white);
            tex.SetPixel(12, 22, Color.black);

            // Crooked smile
            tex.SetPixel(9, 19, new Color(0.5f, 0.4f, 0.35f));
            tex.SetPixel(10, 18, new Color(0.5f, 0.4f, 0.35f));
            tex.SetPixel(11, 19, new Color(0.5f, 0.4f, 0.35f));

            // Arms - one longer
            DrawLine(tex, 6, 10, 4, 4, skin);
            DrawLine(tex, 22, 12, 26, 6, skin);

            // Legs
            DrawFilledRect(tex, 10, 2, 3, 6, clothes);
            DrawFilledRect(tex, 16, 2, 3, 6, clothes);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Item Sprites

        public static Sprite GetKeySprite(string keyType)
        {
            string key = $"Key_{keyType}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color keyColor = keyType switch
            {
                "Yellow" => Palettes.GoldKey,
                "Red" => new Color(0.9f, 0.3f, 0.25f),
                "Green" => new Color(0.3f, 0.8f, 0.35f),
                "Cyan" => new Color(0.3f, 0.8f, 0.9f),
                _ => Palettes.GoldKey
            };
            Color keyDark = keyColor * 0.7f;
            keyDark.a = 1;

            // Key bow (handle) - ornate circular design
            DrawFilledCircle(tex, 16, 24, 6, keyColor);
            DrawFilledCircle(tex, 16, 24, 4, keyDark);
            DrawFilledCircle(tex, 16, 24, 2, keyColor);

            // Key shaft
            DrawFilledRect(tex, 15, 8, 2, 12, keyColor);
            DrawLine(tex, 15, 8, 15, 18, keyDark);

            // Key teeth
            DrawFilledRect(tex, 12, 8, 3, 2, keyColor);
            DrawFilledRect(tex, 12, 11, 3, 2, keyColor);
            DrawFilledRect(tex, 17, 9, 2, 2, keyColor);

            // Shine effect
            tex.SetPixel(14, 26, Color.white);
            tex.SetPixel(15, 25, new Color(1, 1, 1, 0.7f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetFoodSprite(string foodType)
        {
            string key = $"Food_{foodType}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            switch (foodType)
            {
                case "Drumstick":
                    // Meat
                    DrawFilledEllipse(tex, 12, 18, 6, 5, Palettes.FoodMeat);
                    DrawFilledEllipse(tex, 12, 17, 5, 4, new Color(0.8f, 0.4f, 0.3f));
                    // Bone
                    DrawFilledRect(tex, 18, 14, 8, 3, Palettes.SkeletonBone);
                    DrawFilledCircle(tex, 26, 15, 2, Palettes.SkeletonBone);
                    // Shine
                    tex.SetPixel(10, 20, new Color(1, 0.8f, 0.8f));
                    break;

                case "Bread":
                    DrawFilledEllipse(tex, 16, 14, 10, 6, new Color(0.85f, 0.65f, 0.35f));
                    DrawFilledEllipse(tex, 16, 15, 9, 5, new Color(0.95f, 0.75f, 0.45f));
                    // Crust lines
                    DrawLine(tex, 8, 14, 12, 16, new Color(0.7f, 0.5f, 0.25f));
                    DrawLine(tex, 20, 16, 24, 14, new Color(0.7f, 0.5f, 0.25f));
                    break;

                case "Apple":
                    DrawFilledCircle(tex, 16, 14, 8, new Color(0.9f, 0.2f, 0.15f));
                    DrawFilledCircle(tex, 15, 15, 7, new Color(0.95f, 0.3f, 0.2f));
                    // Stem
                    DrawLine(tex, 16, 22, 18, 26, new Color(0.4f, 0.25f, 0.15f));
                    // Leaf
                    DrawFilledEllipse(tex, 20, 25, 3, 2, new Color(0.3f, 0.7f, 0.25f));
                    // Shine
                    tex.SetPixel(13, 18, new Color(1, 0.8f, 0.8f));
                    break;

                case "Cheese":
                    DrawTriangle(tex, 6, 8, 26, 8, 16, 24, new Color(1f, 0.85f, 0.3f));
                    DrawTriangle(tex, 8, 10, 24, 10, 16, 22, new Color(1f, 0.9f, 0.4f));
                    // Holes
                    DrawFilledCircle(tex, 12, 12, 2, new Color(0.9f, 0.75f, 0.2f));
                    DrawFilledCircle(tex, 18, 14, 1, new Color(0.9f, 0.75f, 0.2f));
                    break;

                default: // Generic food
                    DrawFilledCircle(tex, 16, 16, 8, Palettes.Food);
                    break;
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetACGPartSprite(string part)
        {
            string key = $"ACG_{part}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color gold = Palettes.GoldKey;
            Color goldDark = Palettes.GoldKeyDark;

            // Base shape for ACG parts - ornate key piece
            DrawFilledRect(tex, 10, 8, 12, 16, gold);
            DrawFilledRect(tex, 11, 9, 10, 14, goldDark);
            DrawFilledRect(tex, 12, 10, 8, 12, gold);

            // Part-specific details
            switch (part)
            {
                case "A":
                    // 'A' shape cutout
                    DrawTriangle(tex, 16, 22, 12, 12, 20, 12, goldDark);
                    DrawFilledRect(tex, 14, 12, 4, 2, gold);
                    break;
                case "C":
                    // 'C' shape
                    DrawFilledCircle(tex, 16, 16, 5, goldDark);
                    DrawFilledCircle(tex, 16, 16, 3, gold);
                    DrawFilledRect(tex, 18, 14, 4, 4, gold);
                    break;
                case "G":
                    // 'G' shape
                    DrawFilledCircle(tex, 16, 16, 5, goldDark);
                    DrawFilledCircle(tex, 16, 16, 3, gold);
                    DrawFilledRect(tex, 16, 14, 6, 2, goldDark);
                    DrawFilledRect(tex, 18, 14, 2, 4, goldDark);
                    break;
            }

            // Gem in center
            DrawFilledCircle(tex, 16, 16, 2, part switch
            {
                "A" => new Color(0.9f, 0.2f, 0.2f),
                "C" => new Color(0.2f, 0.9f, 0.3f),
                "G" => new Color(0.2f, 0.4f, 0.9f),
                _ => Color.white
            });

            // Shine
            tex.SetPixel(15, 17, Color.white);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Projectile Sprites

        public static Sprite GetMagicProjectileSprite()
        {
            string key = "MagicProjectile";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Glowing magic orb
            DrawFilledCircle(tex, 8, 8, 6, new Color(0.3f, 0.4f, 0.9f, 0.5f));
            DrawFilledCircle(tex, 8, 8, 4, new Color(0.5f, 0.6f, 1f, 0.7f));
            DrawFilledCircle(tex, 8, 8, 2, new Color(0.8f, 0.9f, 1f, 0.9f));
            tex.SetPixel(8, 9, Color.white);

            // Sparkles
            tex.SetPixel(4, 10, new Color(1, 1, 1, 0.6f));
            tex.SetPixel(12, 6, new Color(1, 1, 1, 0.6f));
            tex.SetPixel(6, 4, new Color(1, 1, 1, 0.4f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetSwordSwingSprite()
        {
            string key = "SwordSwing";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Sword slash arc
            for (int i = 0; i < 28; i++)
            {
                float angle = (i / 28f) * Mathf.PI * 0.6f - Mathf.PI * 0.3f;
                int x = 16 + (int)(Mathf.Cos(angle) * (10 + i * 0.3f));
                int y = 8 + (int)(Mathf.Sin(angle) * 6);

                if (x >= 0 && x < 32 && y >= 0 && y < 16)
                {
                    tex.SetPixel(x, y, new Color(0.9f, 0.9f, 0.95f, 1 - i / 35f));
                    if (y + 1 < 16) tex.SetPixel(x, y + 1, new Color(0.7f, 0.7f, 0.8f, 0.5f - i / 60f));
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetThrownAxeSprite()
        {
            string key = "ThrownAxe";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Handle
            DrawLine(tex, 4, 4, 12, 12, new Color(0.5f, 0.35f, 0.2f));
            DrawLine(tex, 5, 4, 13, 12, new Color(0.6f, 0.4f, 0.25f));

            // Axe head
            DrawFilledRect(tex, 10, 10, 5, 5, Palettes.KnightArmor);
            DrawLine(tex, 14, 10, 14, 14, Palettes.KnightArmorDark);
            DrawLine(tex, 10, 14, 14, 14, Palettes.KnightArmorDark);

            // Shine
            tex.SetPixel(11, 12, Palettes.KnightArmorLight);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Environment Sprites

        public static Sprite GetFloorTile(int floorLevel, int variation = 0)
        {
            string key = $"Floor_{floorLevel}_{variation}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;

            Color baseColor, darkColor, lightColor, accentColor;

            switch (floorLevel)
            {
                case 0: // Basement - dark dungeon stone
                    baseColor = Palettes.BasementStone;
                    darkColor = Palettes.BasementStoneDark;
                    lightColor = Palettes.BasementStoneLight;
                    accentColor = Palettes.BasementMoss;
                    break;
                case 1: // Main floor - wooden/carpet
                    baseColor = Palettes.CastleFloor;
                    darkColor = Palettes.CastleFloorDark;
                    lightColor = Palettes.CastleFloorLight;
                    accentColor = Palettes.CastleCarpet;
                    break;
                case 2: // Top floor - stone tower
                    baseColor = Palettes.TowerStone;
                    darkColor = Palettes.TowerStoneDark;
                    lightColor = Palettes.TowerStoneLight;
                    accentColor = Palettes.TowerSky;
                    break;
                default:
                    baseColor = Palettes.CastleFloor;
                    darkColor = Palettes.CastleFloorDark;
                    lightColor = Palettes.CastleFloorLight;
                    accentColor = Color.gray;
                    break;
            }

            // Fill base
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    tex.SetPixel(x, y, baseColor);
                }
            }

            // Add stone/tile pattern
            Random.InitState(variation * 1000 + floorLevel);

            if (floorLevel == 0) // Basement - irregular stones with moss
            {
                // Draw irregular stone blocks
                for (int bx = 0; bx < 4; bx++)
                {
                    for (int by = 0; by < 4; by++)
                    {
                        int offsetX = Random.Range(-1, 2);
                        int offsetY = Random.Range(-1, 2);
                        int w = 7 + Random.Range(-1, 2);
                        int h = 7 + Random.Range(-1, 2);

                        // Stone outline
                        DrawRect(tex, bx * 8 + offsetX, by * 8 + offsetY, w, h, darkColor);

                        // Random moss patches
                        if (Random.value > 0.7f)
                        {
                            tex.SetPixel(bx * 8 + 2 + Random.Range(0, 4), by * 8 + 2 + Random.Range(0, 4), accentColor);
                            tex.SetPixel(bx * 8 + 3 + Random.Range(0, 3), by * 8 + 3 + Random.Range(0, 3), accentColor);
                        }
                    }
                }

                // Add cracks
                for (int i = 0; i < 3; i++)
                {
                    int startX = Random.Range(4, 28);
                    int startY = Random.Range(4, 28);
                    for (int j = 0; j < 4; j++)
                    {
                        int x = startX + Random.Range(-1, 2);
                        int y = startY + j;
                        if (x >= 0 && x < 32 && y >= 0 && y < 32)
                            tex.SetPixel(x, y, darkColor);
                    }
                }
            }
            else if (floorLevel == 1) // Main floor - wood planks
            {
                // Draw wood grain
                for (int plank = 0; plank < 4; plank++)
                {
                    int plankY = plank * 8;

                    // Plank base
                    for (int x = 0; x < 32; x++)
                    {
                        for (int y = plankY; y < plankY + 8 && y < 32; y++)
                        {
                            float grain = Mathf.PerlinNoise(x * 0.2f + plank * 10, y * 0.1f);
                            Color c = Color.Lerp(darkColor, lightColor, grain);
                            tex.SetPixel(x, y, c);
                        }
                    }

                    // Plank separator
                    for (int x = 0; x < 32; x++)
                    {
                        tex.SetPixel(x, plankY, darkColor);
                    }

                    // Wood grain lines
                    for (int x = 0; x < 32; x++)
                    {
                        if ((x + plank * 3) % 7 == 0)
                        {
                            tex.SetPixel(x, plankY + 3, darkColor);
                            tex.SetPixel(x, plankY + 4, darkColor);
                        }
                    }
                }

                // Add carpet pattern in center (for variation)
                if (variation % 3 == 0)
                {
                    DrawFilledRect(tex, 8, 8, 16, 16, accentColor);
                    DrawRect(tex, 9, 9, 14, 14, Palettes.CastleCarpetDark);
                    DrawRect(tex, 11, 11, 10, 10, Palettes.GoldKeyDark);
                }
            }
            else // Top floor - castle stone
            {
                // Draw brick pattern
                for (int row = 0; row < 4; row++)
                {
                    int offset = (row % 2) * 8;
                    for (int col = 0; col < 5; col++)
                    {
                        int x = col * 8 - 4 + offset;
                        int y = row * 8;

                        // Brick
                        if (x >= 0 && x < 32)
                        {
                            for (int bx = 0; bx < 7 && x + bx < 32; bx++)
                            {
                                for (int by = 0; by < 7 && y + by < 32; by++)
                                {
                                    Color c = baseColor;
                                    // Add slight variation
                                    c.r += Random.Range(-0.03f, 0.03f);
                                    c.g += Random.Range(-0.03f, 0.03f);
                                    c.b += Random.Range(-0.03f, 0.03f);
                                    tex.SetPixel(x + bx, y + by, c);
                                }
                            }
                        }
                    }
                    // Mortar lines
                    for (int x = 0; x < 32; x++)
                    {
                        tex.SetPixel(x, row * 8, Palettes.WallMortar);
                    }
                }

                // Vertical mortar
                for (int row = 0; row < 4; row++)
                {
                    int offset = (row % 2) * 8;
                    for (int col = 0; col < 5; col++)
                    {
                        int x = col * 8 - 4 + offset + 7;
                        if (x >= 0 && x < 32)
                        {
                            for (int y = row * 8; y < (row + 1) * 8 && y < 32; y++)
                            {
                                tex.SetPixel(x, y, Palettes.WallMortar);
                            }
                        }
                    }
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetWallSprite(int floorLevel, bool isVertical)
        {
            string key = $"Wall_{floorLevel}_{isVertical}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;

            Color stoneColor = floorLevel switch
            {
                0 => Palettes.BasementStone,
                1 => Palettes.WallStone,
                2 => Palettes.TowerStone,
                _ => Palettes.WallStone
            };
            Color stoneDark = stoneColor * 0.7f;
            stoneDark.a = 1;
            Color stoneLight = stoneColor * 1.3f;
            stoneLight.a = 1;
            stoneLight = new Color(
                Mathf.Min(stoneLight.r, 1f),
                Mathf.Min(stoneLight.g, 1f),
                Mathf.Min(stoneLight.b, 1f),
                1f
            );

            // Fill base
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    tex.SetPixel(x, y, stoneColor);
                }
            }

            // Draw stone brick pattern
            Random.InitState(floorLevel * 100 + (isVertical ? 1 : 0));

            for (int row = 0; row < 4; row++)
            {
                int offset = (row % 2) * 8;
                for (int col = 0; col < 5; col++)
                {
                    int x = col * 8 - 4 + offset;
                    int y = row * 8;

                    // Individual brick with lighting
                    for (int bx = 1; bx < 7 && x + bx < 32 && x + bx >= 0; bx++)
                    {
                        for (int by = 1; by < 7 && y + by < 32; by++)
                        {
                            // Top-left highlight
                            if (bx == 1 || by == 6)
                                tex.SetPixel(x + bx, y + by, stoneLight);
                            // Bottom-right shadow
                            else if (bx == 6 || by == 1)
                                tex.SetPixel(x + bx, y + by, stoneDark);
                            else
                            {
                                // Slight variation
                                Color c = stoneColor;
                                c.r += Random.Range(-0.02f, 0.02f);
                                c.g += Random.Range(-0.02f, 0.02f);
                                c.b += Random.Range(-0.02f, 0.02f);
                                tex.SetPixel(x + bx, y + by, c);
                            }
                        }
                    }
                }

                // Mortar lines
                for (int x = 0; x < 32; x++)
                {
                    tex.SetPixel(x, row * 8, Palettes.WallMortar);
                }
            }

            // Add decorative elements based on floor
            if (floorLevel == 0) // Basement - chains, moss
            {
                // Moss patches
                for (int i = 0; i < 5; i++)
                {
                    int mx = Random.Range(2, 30);
                    int my = Random.Range(2, 30);
                    tex.SetPixel(mx, my, Palettes.BasementMoss);
                    tex.SetPixel(mx + 1, my, Palettes.BasementMoss);
                }
            }
            else if (floorLevel == 1) // Main floor - torch holder marks
            {
                // Soot marks from torches
                if (Random.value > 0.5f)
                {
                    for (int y = 20; y < 30; y++)
                    {
                        int spread = (30 - y) / 3;
                        for (int x = 14 - spread; x < 18 + spread; x++)
                        {
                            if (x >= 0 && x < 32)
                            {
                                Color c = tex.GetPixel(x, y);
                                c = Color.Lerp(c, Color.black, 0.2f);
                                tex.SetPixel(x, y, c);
                            }
                        }
                    }
                }
            }
            else // Top floor - window light
            {
                // Light beam effect
                for (int y = 0; y < 32; y++)
                {
                    for (int x = 12; x < 20; x++)
                    {
                        Color c = tex.GetPixel(x, y);
                        c = Color.Lerp(c, Palettes.TowerSky, 0.15f);
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetDoorSprite(bool isOpen, string keyType = "")
        {
            string key = $"Door_{isOpen}_{keyType}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color doorColor = new Color(0.45f, 0.3f, 0.2f);
            Color doorDark = new Color(0.3f, 0.2f, 0.12f);
            Color doorLight = new Color(0.55f, 0.4f, 0.28f);

            if (!isOpen)
            {
                // Closed door
                DrawFilledRect(tex, 4, 2, 24, 28, doorColor);
                DrawFilledRect(tex, 5, 3, 22, 26, doorDark);
                DrawFilledRect(tex, 6, 4, 20, 24, doorColor);

                // Door panels
                DrawFilledRect(tex, 8, 16, 7, 10, doorDark);
                DrawFilledRect(tex, 17, 16, 7, 10, doorDark);
                DrawFilledRect(tex, 8, 4, 7, 10, doorDark);
                DrawFilledRect(tex, 17, 4, 7, 10, doorDark);

                // Panel highlights
                DrawRect(tex, 9, 17, 5, 8, doorLight);
                DrawRect(tex, 18, 17, 5, 8, doorLight);
                DrawRect(tex, 9, 5, 5, 8, doorLight);
                DrawRect(tex, 18, 5, 5, 8, doorLight);

                // Door handle
                DrawFilledCircle(tex, 24, 14, 2, Palettes.GoldKey);
                tex.SetPixel(24, 15, Palettes.GoldKeyDark);

                // Lock indicator if locked
                if (!string.IsNullOrEmpty(keyType))
                {
                    Color lockColor = keyType switch
                    {
                        "Yellow" => Palettes.GoldKey,
                        "Red" => new Color(0.9f, 0.3f, 0.25f),
                        "Green" => new Color(0.3f, 0.8f, 0.35f),
                        "Cyan" => new Color(0.3f, 0.8f, 0.9f),
                        _ => Palettes.GoldKey
                    };
                    DrawFilledRect(tex, 22, 10, 4, 5, lockColor);
                    tex.SetPixel(24, 11, Color.black); // Keyhole
                }
            }
            else
            {
                // Open door - dark doorway
                DrawFilledRect(tex, 4, 2, 24, 28, new Color(0.1f, 0.08f, 0.05f));

                // Door frame
                DrawFilledRect(tex, 2, 0, 4, 32, doorColor);
                DrawFilledRect(tex, 26, 0, 4, 32, doorColor);
                DrawFilledRect(tex, 2, 28, 28, 4, doorColor);

                // Frame detail
                DrawLine(tex, 4, 0, 4, 32, doorLight);
                DrawLine(tex, 27, 0, 27, 32, doorDark);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetStaircaseSprite(bool goingUp)
        {
            string key = $"Stairs_{goingUp}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color stoneColor = Palettes.WallStone;
            Color stoneDark = Palettes.WallStoneDark;
            Color stoneLight = Palettes.WallStoneLight;

            // Draw stairs
            int numSteps = 6;
            for (int step = 0; step < numSteps; step++)
            {
                int y = goingUp ? step * 5 : (numSteps - 1 - step) * 5;
                int x = step * 4 + 2;
                int width = 32 - step * 4 - 4;

                // Step top
                DrawFilledRect(tex, x, y + 3, width, 2, stoneLight);
                // Step front
                DrawFilledRect(tex, x, y, width, 3, stoneColor);
                // Step shadow
                DrawLine(tex, x, y, x + width, y, stoneDark);
            }

            // Railing
            DrawLine(tex, 2, 2, 28, 28, stoneDark);
            DrawLine(tex, 3, 2, 29, 28, stoneColor);

            // Railing posts
            for (int i = 0; i < 4; i++)
            {
                int x = 4 + i * 8;
                int y = 4 + i * 7;
                DrawFilledRect(tex, x, y, 2, 4, stoneColor);
                tex.SetPixel(x, y + 4, stoneLight);
            }

            // Arrow indicator
            if (goingUp)
            {
                DrawTriangle(tex, 26, 28, 22, 22, 30, 22, Palettes.GoldKey);
            }
            else
            {
                DrawTriangle(tex, 6, 4, 2, 10, 10, 10, Palettes.GoldKey);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetTorchSprite(int frame)
        {
            string key = $"Torch_{frame % 4}";
            if (_spriteCache.TryGetValue(key, out Sprite cached)) return cached;

            var tex = new Texture2D(16, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Torch handle
            DrawFilledRect(tex, 6, 2, 4, 14, new Color(0.4f, 0.25f, 0.15f));
            DrawLine(tex, 7, 2, 7, 16, new Color(0.5f, 0.35f, 0.2f));

            // Torch head wrap
            DrawFilledRect(tex, 5, 14, 6, 4, new Color(0.6f, 0.5f, 0.3f));

            // Flame base
            DrawFilledEllipse(tex, 8, 22, 4, 5, Palettes.Fire);
            DrawFilledEllipse(tex, 8, 23, 3, 4, Palettes.FireBright);

            // Animated flame top
            int flameOffset = frame % 4;
            int[] flameHeights = { 6, 7, 5, 7 };
            int flameH = flameHeights[flameOffset];

            DrawFilledEllipse(tex, 8 + (frame % 2), 24 + flameH / 2, 2, flameH / 2, Palettes.FireBright);
            tex.SetPixel(8, 26 + flameH / 2, new Color(1, 1, 0.8f));

            // Sparks
            if (frame % 2 == 0)
            {
                tex.SetPixel(6 + Random.Range(0, 4), 28 + Random.Range(0, 3), new Color(1, 0.8f, 0.3f, 0.8f));
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 32), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Helper Methods

        private static void ClearTexture(Texture2D tex, Color color)
        {
            Color[] pixels = new Color[tex.width * tex.height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;
            tex.SetPixels(pixels);
        }

        private static void DrawFilledRect(Texture2D tex, int x, int y, int width, int height, Color color)
        {
            for (int px = x; px < x + width && px < tex.width; px++)
            {
                for (int py = y; py < y + height && py < tex.height; py++)
                {
                    if (px >= 0 && py >= 0)
                        tex.SetPixel(px, py, color);
                }
            }
        }

        private static void DrawRect(Texture2D tex, int x, int y, int width, int height, Color color)
        {
            for (int px = x; px < x + width && px < tex.width; px++)
            {
                if (px >= 0)
                {
                    if (y >= 0 && y < tex.height) tex.SetPixel(px, y, color);
                    if (y + height - 1 >= 0 && y + height - 1 < tex.height) tex.SetPixel(px, y + height - 1, color);
                }
            }
            for (int py = y; py < y + height && py < tex.height; py++)
            {
                if (py >= 0)
                {
                    if (x >= 0 && x < tex.width) tex.SetPixel(x, py, color);
                    if (x + width - 1 >= 0 && x + width - 1 < tex.width) tex.SetPixel(x + width - 1, py, color);
                }
            }
        }

        private static void DrawFilledCircle(Texture2D tex, int cx, int cy, int radius, Color color)
        {
            for (int x = cx - radius; x <= cx + radius; x++)
            {
                for (int y = cy - radius; y <= cy + radius; y++)
                {
                    if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                    {
                        if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= radius * radius)
                        {
                            tex.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }

        private static void DrawFilledEllipse(Texture2D tex, int cx, int cy, int rx, int ry, Color color)
        {
            for (int x = cx - rx; x <= cx + rx; x++)
            {
                for (int y = cy - ry; y <= cy + ry; y++)
                {
                    if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                    {
                        float dx = (float)(x - cx) / rx;
                        float dy = (float)(y - cy) / ry;
                        if (dx * dx + dy * dy <= 1)
                        {
                            tex.SetPixel(x, y, color);
                        }
                    }
                }
            }
        }

        private static void DrawLine(Texture2D tex, int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (x0 >= 0 && x0 < tex.width && y0 >= 0 && y0 < tex.height)
                    tex.SetPixel(x0, y0, color);

                if (x0 == x1 && y0 == y1) break;

                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        private static void DrawTriangle(Texture2D tex, int x0, int y0, int x1, int y1, int x2, int y2, Color color)
        {
            // Sort vertices by y
            if (y0 > y1) { Swap(ref x0, ref x1); Swap(ref y0, ref y1); }
            if (y1 > y2) { Swap(ref x1, ref x2); Swap(ref y1, ref y2); }
            if (y0 > y1) { Swap(ref x0, ref x1); Swap(ref y0, ref y1); }

            // Fill triangle
            for (int y = y0; y <= y2; y++)
            {
                int xa, xb;
                if (y < y1)
                {
                    xa = y0 != y1 ? x0 + (x1 - x0) * (y - y0) / (y1 - y0) : x0;
                    xb = y0 != y2 ? x0 + (x2 - x0) * (y - y0) / (y2 - y0) : x0;
                }
                else
                {
                    xa = y1 != y2 ? x1 + (x2 - x1) * (y - y1) / (y2 - y1) : x1;
                    xb = y0 != y2 ? x0 + (x2 - x0) * (y - y0) / (y2 - y0) : x0;
                }

                if (xa > xb) Swap(ref xa, ref xb);

                for (int x = xa; x <= xb; x++)
                {
                    if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                        tex.SetPixel(x, y, color);
                }
            }
        }

        private static void Swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        public static void ClearCache()
        {
            _spriteCache.Clear();
        }

        #endregion
    }
}
