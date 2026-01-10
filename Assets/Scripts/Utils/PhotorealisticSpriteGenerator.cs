using UnityEngine;
using System.Collections.Generic;

namespace HauntedCastle.Utils
{
    /// <summary>
    /// Generates high-resolution (64x64 and 128x128) photorealistic sprites
    /// with detailed shading, textures, and lighting effects.
    /// </summary>
    public static class PhotorealisticSpriteGenerator
    {
        private static Dictionary<string, Sprite> _spriteCache = new();
        private const int HD_SIZE = 64;
        private const int UHD_SIZE = 128;

        #region Character Sprites

        public static Sprite GetWizardSprite()
        {
            if (_spriteCache.TryGetValue("wizard_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;

            // Clear with transparency
            ClearTexture(tex);

            // Draw wizard with detailed robes, beard, and hat
            // Body/Robes - deep blue with fabric folds
            DrawRobes(tex, 20, 8, 24, 32, new Color(0.15f, 0.15f, 0.5f), new Color(0.25f, 0.25f, 0.7f));

            // Face - aged wizard
            DrawDetailedFace(tex, 24, 42, 16, new Color(0.9f, 0.75f, 0.65f), true);

            // Long white beard
            DrawBeard(tex, 24, 32, 20, 16, new Color(0.95f, 0.95f, 0.95f));

            // Wizard hat - pointed with stars
            DrawWizardHat(tex, 16, 50, 32, 14, new Color(0.15f, 0.15f, 0.5f));

            // Staff with glowing orb
            DrawStaff(tex, 44, 8, 40, new Color(0.6f, 0.4f, 0.2f), new Color(0.3f, 0.8f, 1f));

            // Magic particles around
            DrawMagicParticles(tex, new Color(0.5f, 0.7f, 1f, 0.8f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["wizard_hd"] = sprite;
            return sprite;
        }

        public static Sprite GetKnightSprite()
        {
            if (_spriteCache.TryGetValue("knight_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            // Metal armor with realistic shading
            Color armorBase = new Color(0.6f, 0.6f, 0.65f);
            Color armorHighlight = new Color(0.85f, 0.85f, 0.9f);
            Color armorShadow = new Color(0.35f, 0.35f, 0.4f);

            // Legs/Greaves
            DrawArmorPiece(tex, 22, 4, 8, 18, armorBase, armorHighlight, armorShadow);
            DrawArmorPiece(tex, 34, 4, 8, 18, armorBase, armorHighlight, armorShadow);

            // Body/Breastplate with emblem
            DrawArmorPiece(tex, 18, 22, 28, 24, armorBase, armorHighlight, armorShadow);
            DrawEmblem(tex, 28, 30, 8, new Color(0.8f, 0.2f, 0.2f));

            // Arms/Pauldrons
            DrawArmorPiece(tex, 10, 28, 10, 14, armorBase, armorHighlight, armorShadow);
            DrawArmorPiece(tex, 44, 28, 10, 14, armorBase, armorHighlight, armorShadow);

            // Helmet with visor
            DrawHelmet(tex, 20, 46, 24, 18, armorBase, armorHighlight, armorShadow);

            // Sword
            DrawSword(tex, 50, 10, 36, new Color(0.7f, 0.7f, 0.75f), new Color(0.6f, 0.4f, 0.2f));

            // Shield
            DrawShield(tex, 6, 20, 12, 20, new Color(0.7f, 0.2f, 0.2f), armorBase);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["knight_hd"] = sprite;
            return sprite;
        }

        public static Sprite GetSerfSprite()
        {
            if (_spriteCache.TryGetValue("serf_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color skinTone = new Color(0.85f, 0.7f, 0.55f);
            Color clothBrown = new Color(0.5f, 0.35f, 0.2f);
            Color clothLight = new Color(0.65f, 0.5f, 0.35f);

            // Boots
            DrawBoots(tex, 22, 2, 8, 10, new Color(0.3f, 0.2f, 0.1f));
            DrawBoots(tex, 34, 2, 8, 10, new Color(0.3f, 0.2f, 0.1f));

            // Pants - patched
            DrawPatchedClothing(tex, 20, 12, 24, 16, clothBrown, clothLight);

            // Tunic with belt
            DrawTunic(tex, 18, 28, 28, 20, clothLight, clothBrown);
            DrawBelt(tex, 18, 28, 28, new Color(0.25f, 0.15f, 0.1f));

            // Head with cap
            DrawDetailedFace(tex, 24, 48, 16, skinTone, false);
            DrawPeasantCap(tex, 20, 54, 24, 10, new Color(0.4f, 0.25f, 0.15f));

            // Axe (for Serf character)
            DrawAxe(tex, 48, 24, 24, new Color(0.5f, 0.35f, 0.2f), new Color(0.5f, 0.5f, 0.55f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["serf_hd"] = sprite;
            return sprite;
        }

        #endregion

        #region Enemy Sprites

        public static Sprite GetGhostSprite()
        {
            if (_spriteCache.TryGetValue("ghost_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            // Ethereal flowing ghost shape
            Color ghostBase = new Color(0.8f, 0.85f, 0.95f, 0.6f);
            Color ghostHighlight = new Color(0.95f, 0.97f, 1f, 0.8f);
            Color ghostShadow = new Color(0.5f, 0.55f, 0.7f, 0.4f);

            // Main ghostly body with flowing edges
            DrawGhostlyForm(tex, 12, 4, 40, 56, ghostBase, ghostHighlight, ghostShadow);

            // Hollow eyes
            DrawHollowEyes(tex, 22, 38, 8, new Color(0.1f, 0.1f, 0.3f, 0.9f));
            DrawHollowEyes(tex, 38, 38, 8, new Color(0.1f, 0.1f, 0.3f, 0.9f));

            // Ghostly mouth/wail
            DrawGhostMouth(tex, 28, 28, 8, 6, new Color(0.2f, 0.2f, 0.4f, 0.7f));

            // Ethereal glow effect
            AddGlowEffect(tex, 32, 32, 28, new Color(0.7f, 0.8f, 1f, 0.3f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["ghost_hd"] = sprite;
            return sprite;
        }

        public static Sprite GetSkeletonSprite()
        {
            if (_spriteCache.TryGetValue("skeleton_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color boneWhite = new Color(0.92f, 0.9f, 0.85f);
            Color boneShadow = new Color(0.7f, 0.65f, 0.6f);
            Color boneHighlight = new Color(0.98f, 0.97f, 0.95f);

            // Leg bones
            DrawBone(tex, 24, 2, 4, 20, boneWhite, boneShadow, boneHighlight);
            DrawBone(tex, 36, 2, 4, 20, boneWhite, boneShadow, boneHighlight);

            // Pelvis
            DrawPelvis(tex, 22, 22, 20, 8, boneWhite, boneShadow);

            // Ribcage
            DrawRibcage(tex, 20, 30, 24, 16, boneWhite, boneShadow);

            // Spine
            DrawSpine(tex, 30, 22, 4, 24, boneWhite, boneShadow);

            // Arm bones
            DrawBone(tex, 12, 32, 4, 16, boneWhite, boneShadow, boneHighlight);
            DrawBone(tex, 48, 32, 4, 16, boneWhite, boneShadow, boneHighlight);

            // Skull
            DrawSkull(tex, 22, 46, 20, 18, boneWhite, boneShadow, boneHighlight);

            // Glowing red eyes
            DrawGlowingEyes(tex, 26, 52, 4, new Color(1f, 0.2f, 0.1f));
            DrawGlowingEyes(tex, 38, 52, 4, new Color(1f, 0.2f, 0.1f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["skeleton_hd"] = sprite;
            return sprite;
        }

        public static Sprite GetSpiderSprite()
        {
            if (_spriteCache.TryGetValue("spider_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color bodyDark = new Color(0.15f, 0.1f, 0.1f);
            Color bodyLight = new Color(0.3f, 0.2f, 0.15f);
            Color legColor = new Color(0.2f, 0.15f, 0.1f);
            Color eyeColor = new Color(0.9f, 0.1f, 0.1f);

            // Eight legs with joints
            DrawSpiderLeg(tex, 8, 20, -30, legColor);
            DrawSpiderLeg(tex, 10, 28, -20, legColor);
            DrawSpiderLeg(tex, 12, 36, -10, legColor);
            DrawSpiderLeg(tex, 14, 40, 0, legColor);
            DrawSpiderLeg(tex, 50, 20, 30, legColor);
            DrawSpiderLeg(tex, 48, 28, 20, legColor);
            DrawSpiderLeg(tex, 46, 36, 10, legColor);
            DrawSpiderLeg(tex, 44, 40, 0, legColor);

            // Abdomen (back body)
            DrawSpiderBody(tex, 20, 8, 24, 20, bodyDark, bodyLight);

            // Cephalothorax (front body)
            DrawSpiderBody(tex, 24, 28, 16, 14, bodyDark, bodyLight);

            // Multiple eyes
            DrawSpiderEyes(tex, 26, 36, eyeColor);

            // Fangs
            DrawFangs(tex, 28, 28, 8, new Color(0.4f, 0.35f, 0.3f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["spider_hd"] = sprite;
            return sprite;
        }

        public static Sprite GetBatSprite()
        {
            if (_spriteCache.TryGetValue("bat_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color bodyColor = new Color(0.2f, 0.18f, 0.22f);
            Color wingMembrane = new Color(0.25f, 0.2f, 0.25f, 0.9f);
            Color eyeColor = new Color(0.9f, 0.3f, 0.2f);

            // Wings spread wide
            DrawBatWing(tex, 2, 20, 28, 32, wingMembrane, bodyColor, true);
            DrawBatWing(tex, 34, 20, 28, 32, wingMembrane, bodyColor, false);

            // Furry body
            DrawFurryBody(tex, 26, 24, 12, 20, bodyColor);

            // Head with ears
            DrawBatHead(tex, 26, 44, 12, 14, bodyColor);
            DrawBatEars(tex, 24, 54, 6, 10, bodyColor);
            DrawBatEars(tex, 38, 54, 6, 10, bodyColor);

            // Eyes
            DrawGlowingEyes(tex, 28, 48, 3, eyeColor);
            DrawGlowingEyes(tex, 37, 48, 3, eyeColor);

            // Fangs
            DrawFangs(tex, 30, 44, 4, new Color(0.95f, 0.95f, 0.9f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["bat_hd"] = sprite;
            return sprite;
        }

        public static Sprite GetDemonSprite()
        {
            if (_spriteCache.TryGetValue("demon_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color skinRed = new Color(0.7f, 0.15f, 0.1f);
            Color skinDark = new Color(0.4f, 0.08f, 0.05f);
            Color skinHighlight = new Color(0.85f, 0.25f, 0.15f);
            Color hornColor = new Color(0.2f, 0.15f, 0.1f);

            // Hooved feet
            DrawHooves(tex, 20, 2, 10, new Color(0.15f, 0.1f, 0.1f));
            DrawHooves(tex, 36, 2, 10, new Color(0.15f, 0.1f, 0.1f));

            // Muscular legs
            DrawMuscularLimb(tex, 20, 12, 10, 16, skinRed, skinDark, skinHighlight);
            DrawMuscularLimb(tex, 36, 12, 10, 16, skinRed, skinDark, skinHighlight);

            // Muscular torso
            DrawMuscularTorso(tex, 16, 28, 32, 24, skinRed, skinDark, skinHighlight);

            // Arms with claws
            DrawMuscularLimb(tex, 6, 32, 10, 18, skinRed, skinDark, skinHighlight);
            DrawMuscularLimb(tex, 50, 32, 10, 18, skinRed, skinDark, skinHighlight);
            DrawClaws(tex, 4, 28, 6, new Color(0.2f, 0.15f, 0.1f));
            DrawClaws(tex, 56, 28, 6, new Color(0.2f, 0.15f, 0.1f));

            // Demonic head
            DrawDemonHead(tex, 20, 48, 24, 16, skinRed, skinDark);

            // Horns
            DrawHorn(tex, 18, 56, 8, 12, hornColor, true);
            DrawHorn(tex, 44, 56, 8, 12, hornColor, false);

            // Glowing yellow eyes
            DrawGlowingEyes(tex, 26, 52, 4, new Color(1f, 0.9f, 0.2f));
            DrawGlowingEyes(tex, 38, 52, 4, new Color(1f, 0.9f, 0.2f));

            // Fire aura
            AddFireAura(tex, 32, 32, 30);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["demon_hd"] = sprite;
            return sprite;
        }

        public static Sprite GetMummySprite()
        {
            if (_spriteCache.TryGetValue("mummy_hd", out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color bandageLight = new Color(0.85f, 0.8f, 0.7f);
            Color bandageDark = new Color(0.6f, 0.55f, 0.45f);
            Color bandageShadow = new Color(0.4f, 0.35f, 0.3f);
            Color rotColor = new Color(0.3f, 0.25f, 0.2f);

            // Wrapped legs
            DrawBandagedLimb(tex, 22, 2, 8, 22, bandageLight, bandageDark, bandageShadow);
            DrawBandagedLimb(tex, 36, 2, 8, 22, bandageLight, bandageDark, bandageShadow);

            // Wrapped torso
            DrawBandagedTorso(tex, 18, 24, 28, 26, bandageLight, bandageDark, bandageShadow);

            // Wrapped arms (one outstretched)
            DrawBandagedLimb(tex, 8, 30, 10, 20, bandageLight, bandageDark, bandageShadow);
            DrawBandagedLimb(tex, 46, 36, 10, 14, bandageLight, bandageDark, bandageShadow);

            // Wrapped head with exposed areas
            DrawMummyHead(tex, 22, 50, 20, 14, bandageLight, bandageDark, rotColor);

            // Glowing eyes through bandages
            DrawGlowingEyes(tex, 28, 54, 3, new Color(0.4f, 0.9f, 0.3f));
            DrawGlowingEyes(tex, 38, 54, 3, new Color(0.4f, 0.9f, 0.3f));

            // Dust/sand particles
            AddDustParticles(tex, new Color(0.7f, 0.65f, 0.5f, 0.5f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache["mummy_hd"] = sprite;
            return sprite;
        }

        #endregion

        #region Environment Sprites

        public static Sprite GetFloorTileSprite(int floorLevel, int variation)
        {
            string key = $"floor_{floorLevel}_{variation % 4}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;

            Color baseColor, lightColor, darkColor, groutColor;

            switch (floorLevel)
            {
                case 0: // Basement - dark stone/dungeon
                    baseColor = new Color(0.25f, 0.22f, 0.2f);
                    lightColor = new Color(0.35f, 0.32f, 0.28f);
                    darkColor = new Color(0.15f, 0.12f, 0.1f);
                    groutColor = new Color(0.1f, 0.08f, 0.06f);
                    break;
                case 1: // Castle - polished stone/wood
                    baseColor = new Color(0.55f, 0.45f, 0.35f);
                    lightColor = new Color(0.7f, 0.58f, 0.45f);
                    darkColor = new Color(0.4f, 0.32f, 0.25f);
                    groutColor = new Color(0.3f, 0.25f, 0.2f);
                    break;
                default: // Tower - light stone
                    baseColor = new Color(0.65f, 0.62f, 0.58f);
                    lightColor = new Color(0.8f, 0.77f, 0.72f);
                    darkColor = new Color(0.5f, 0.47f, 0.43f);
                    groutColor = new Color(0.4f, 0.38f, 0.35f);
                    break;
            }

            // Draw stone tile pattern with cracks and weathering
            DrawStoneTile(tex, baseColor, lightColor, darkColor, groutColor, variation);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetWallSprite(int floorLevel, bool isVertical)
        {
            string key = $"wall_{floorLevel}_{(isVertical ? "v" : "h")}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;

            Color baseColor, lightColor, darkColor, mortarColor;

            switch (floorLevel)
            {
                case 0: // Basement - rough dark stone
                    baseColor = new Color(0.28f, 0.25f, 0.22f);
                    lightColor = new Color(0.38f, 0.35f, 0.3f);
                    darkColor = new Color(0.18f, 0.15f, 0.12f);
                    mortarColor = new Color(0.12f, 0.1f, 0.08f);
                    break;
                case 1: // Castle - dressed stone
                    baseColor = new Color(0.5f, 0.45f, 0.4f);
                    lightColor = new Color(0.65f, 0.58f, 0.5f);
                    darkColor = new Color(0.35f, 0.3f, 0.25f);
                    mortarColor = new Color(0.25f, 0.22f, 0.18f);
                    break;
                default: // Tower - light stone
                    baseColor = new Color(0.6f, 0.58f, 0.55f);
                    lightColor = new Color(0.75f, 0.72f, 0.68f);
                    darkColor = new Color(0.45f, 0.42f, 0.38f);
                    mortarColor = new Color(0.35f, 0.32f, 0.28f);
                    break;
            }

            DrawBrickWall(tex, baseColor, lightColor, darkColor, mortarColor, isVertical);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetDoorSprite(bool isOpen, string keyColor)
        {
            string key = $"door_{(isOpen ? "open" : "closed")}_{keyColor}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color woodDark = new Color(0.35f, 0.22f, 0.12f);
            Color woodLight = new Color(0.55f, 0.38f, 0.22f);
            Color woodMid = new Color(0.45f, 0.3f, 0.18f);
            Color metalColor = new Color(0.3f, 0.28f, 0.25f);

            // Draw wooden door with planks
            DrawWoodenDoor(tex, 8, 4, 48, 56, woodDark, woodMid, woodLight);

            // Metal hinges
            DrawMetalHinge(tex, 12, 48, 8, metalColor);
            DrawMetalHinge(tex, 12, 20, 8, metalColor);

            // Door handle
            DrawDoorHandle(tex, 42, 32, 8, metalColor);

            // If locked, add keyhole with color tint
            if (!string.IsNullOrEmpty(keyColor) && keyColor != "None")
            {
                Color lockColor = GetKeyColorFromString(keyColor);
                DrawKeyhole(tex, 42, 28, 6, lockColor);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetTorchSprite(int frame)
        {
            string key = $"torch_{frame % 4}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            // Torch handle
            Color handleColor = new Color(0.45f, 0.3f, 0.18f);
            DrawRect(tex, 28, 4, 8, 28, handleColor);

            // Metal bracket
            Color metalColor = new Color(0.35f, 0.32f, 0.28f);
            DrawRect(tex, 26, 28, 12, 6, metalColor);

            // Flame with animation
            DrawAnimatedFlame(tex, 24, 34, 16, 26, frame);

            // Glow effect
            AddGlowEffect(tex, 32, 48, 20, new Color(1f, 0.7f, 0.3f, 0.4f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetStaircaseSprite(bool goingUp)
        {
            string key = $"stairs_{(goingUp ? "up" : "down")}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(HD_SIZE, HD_SIZE);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color stoneBase = new Color(0.45f, 0.42f, 0.38f);
            Color stoneDark = new Color(0.3f, 0.28f, 0.25f);
            Color stoneLight = new Color(0.6f, 0.57f, 0.52f);

            // Draw staircase steps
            DrawStaircaseSteps(tex, 8, 4, 48, 56, stoneBase, stoneDark, stoneLight, goingUp);

            // Arrow indicator
            Color arrowColor = goingUp ? new Color(0.3f, 0.8f, 0.4f) : new Color(0.8f, 0.5f, 0.3f);
            DrawArrowIndicator(tex, 28, goingUp ? 48 : 16, 8, goingUp, arrowColor);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, HD_SIZE, HD_SIZE), new Vector2(0.5f, 0.5f), HD_SIZE);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Decoration Sprites

        public static Sprite GetChainSprite()
        {
            string key = "decor_chain";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(32, 64);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color metalDark = new Color(0.25f, 0.22f, 0.2f);
            Color metalLight = new Color(0.45f, 0.42f, 0.38f);

            // Draw chain links
            for (int i = 0; i < 8; i++)
            {
                DrawChainLink(tex, 10, 4 + i * 8, 12, 10, metalDark, metalLight);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 64), new Vector2(0.5f, 1f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetCobwebSprite()
        {
            string key = "decor_cobweb";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(48, 48);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color webColor = new Color(0.9f, 0.9f, 0.88f, 0.7f);

            // Draw cobweb pattern from corner
            DrawCobweb(tex, 0, 48, 48, webColor);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 48, 48), new Vector2(0f, 1f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetSkullSprite()
        {
            string key = "decor_skull";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color boneWhite = new Color(0.9f, 0.88f, 0.82f);
            Color boneShadow = new Color(0.65f, 0.6f, 0.55f);

            DrawSkull(tex, 4, 4, 24, 24, boneWhite, boneShadow, boneWhite);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetBarrelSprite()
        {
            string key = "decor_barrel";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(48, 56);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color woodDark = new Color(0.4f, 0.28f, 0.15f);
            Color woodLight = new Color(0.6f, 0.45f, 0.28f);
            Color metalBand = new Color(0.35f, 0.32f, 0.28f);

            DrawBarrel(tex, 4, 4, 40, 48, woodDark, woodLight, metalBand);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 48, 56), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetChandelierSprite()
        {
            string key = "decor_chandelier";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(64, 48);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color metalGold = new Color(0.75f, 0.6f, 0.25f);
            Color metalDark = new Color(0.5f, 0.38f, 0.15f);
            Color candleColor = new Color(0.95f, 0.92f, 0.85f);

            DrawChandelier(tex, 8, 8, 48, 36, metalGold, metalDark, candleColor);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 64, 48), new Vector2(0.5f, 1f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetPaintingSprite(int variant)
        {
            string key = $"decor_painting_{variant}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(48, 40);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color frameDark = new Color(0.45f, 0.32f, 0.15f);
            Color frameGold = new Color(0.7f, 0.55f, 0.25f);

            DrawPaintingFrame(tex, 0, 0, 48, 40, frameDark, frameGold);
            DrawPaintingContent(tex, 4, 4, 40, 32, variant);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 48, 40), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetArmorStandSprite()
        {
            string key = "decor_armor_stand";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(48, 64);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color metalBase = new Color(0.55f, 0.52f, 0.48f);
            Color metalHighlight = new Color(0.75f, 0.72f, 0.68f);
            Color metalShadow = new Color(0.35f, 0.32f, 0.28f);
            Color woodColor = new Color(0.4f, 0.28f, 0.15f);

            DrawArmorStand(tex, 8, 4, 32, 56, metalBase, metalHighlight, metalShadow, woodColor);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 48, 64), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetBannerSprite(int variant)
        {
            string key = $"decor_banner_{variant}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(32, 56);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color[] bannerColors = {
                new Color(0.7f, 0.15f, 0.15f), // Red
                new Color(0.15f, 0.2f, 0.6f),  // Blue
                new Color(0.15f, 0.5f, 0.2f),  // Green
                new Color(0.6f, 0.5f, 0.15f)   // Gold
            };

            Color fabricColor = bannerColors[variant % 4];
            Color poleColor = new Color(0.45f, 0.35f, 0.2f);

            DrawBanner(tex, 4, 8, 24, 44, fabricColor, poleColor);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 32, 56), new Vector2(0.5f, 1f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetWindowSprite()
        {
            string key = "decor_window";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(40, 56);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color frameStone = new Color(0.5f, 0.48f, 0.45f);
            Color glassColor = new Color(0.3f, 0.4f, 0.55f, 0.7f);
            Color leadColor = new Color(0.25f, 0.22f, 0.2f);

            DrawGothicWindow(tex, 4, 4, 32, 48, frameStone, glassColor, leadColor);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 40, 56), new Vector2(0.5f, 0.5f), 32);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Drawing Helpers

        private static void ClearTexture(Texture2D tex)
        {
            Color[] clear = new Color[tex.width * tex.height];
            for (int i = 0; i < clear.Length; i++)
                clear[i] = Color.clear;
            tex.SetPixels(clear);
        }

        private static void DrawRect(Texture2D tex, int x, int y, int w, int h, Color color)
        {
            for (int px = x; px < x + w && px < tex.width; px++)
            {
                for (int py = y; py < y + h && py < tex.height; py++)
                {
                    if (px >= 0 && py >= 0)
                        tex.SetPixel(px, py, color);
                }
            }
        }

        private static void DrawCircle(Texture2D tex, int cx, int cy, int radius, Color color)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                            tex.SetPixel(px, py, color);
                    }
                }
            }
        }

        private static void DrawEllipse(Texture2D tex, int cx, int cy, int rx, int ry, Color color)
        {
            for (int x = -rx; x <= rx; x++)
            {
                for (int y = -ry; y <= ry; y++)
                {
                    float nx = x / (float)rx;
                    float ny = y / (float)ry;
                    if (nx * nx + ny * ny <= 1f)
                    {
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                            tex.SetPixel(px, py, color);
                    }
                }
            }
        }

        // Detailed drawing methods
        private static void DrawRobes(Texture2D tex, int x, int y, int w, int h, Color dark, Color light)
        {
            for (int px = 0; px < w; px++)
            {
                for (int py = 0; py < h; py++)
                {
                    float t = py / (float)h;
                    Color c = Color.Lerp(light, dark, t * 0.7f);
                    // Add fabric fold variation
                    float fold = Mathf.Sin(px * 0.5f + py * 0.3f) * 0.1f;
                    c = Color.Lerp(c, light, fold + 0.1f);
                    tex.SetPixel(x + px, y + py, c);
                }
            }
        }

        private static void DrawDetailedFace(Texture2D tex, int cx, int cy, int size, Color skin, bool aged)
        {
            DrawCircle(tex, cx, cy, size / 2, skin);
            Color shadow = new Color(skin.r * 0.7f, skin.g * 0.7f, skin.b * 0.7f);
            // Eyes
            tex.SetPixel(cx - 3, cy + 2, Color.white);
            tex.SetPixel(cx + 3, cy + 2, Color.white);
            tex.SetPixel(cx - 3, cy + 1, new Color(0.2f, 0.3f, 0.5f));
            tex.SetPixel(cx + 3, cy + 1, new Color(0.2f, 0.3f, 0.5f));
            // Nose
            tex.SetPixel(cx, cy - 1, shadow);
            // Mouth
            tex.SetPixel(cx - 1, cy - 3, shadow);
            tex.SetPixel(cx, cy - 3, shadow);
            tex.SetPixel(cx + 1, cy - 3, shadow);
        }

        private static void DrawBeard(Texture2D tex, int cx, int y, int w, int h, Color color)
        {
            for (int px = -w / 2; px < w / 2; px++)
            {
                int height = h - Mathf.Abs(px) / 2;
                for (int py = 0; py < height; py++)
                {
                    float noise = Mathf.PerlinNoise(px * 0.3f, py * 0.3f) * 0.2f;
                    Color c = Color.Lerp(color, color * 0.8f, noise);
                    tex.SetPixel(cx + px, y - py, c);
                }
            }
        }

        private static void DrawWizardHat(Texture2D tex, int x, int y, int w, int h, Color color)
        {
            // Brim
            DrawRect(tex, x - 4, y, w + 8, 3, color);
            // Cone
            for (int row = 0; row < h; row++)
            {
                int rowWidth = w - (row * w / h);
                int startX = x + (w - rowWidth) / 2;
                DrawRect(tex, startX, y + row, rowWidth, 1, color);
            }
            // Star decoration
            tex.SetPixel(x + w / 2, y + h - 4, Color.yellow);
            tex.SetPixel(x + w / 3, y + h / 2, Color.yellow);
        }

        private static void DrawStaff(Texture2D tex, int x, int y, int height, Color wood, Color orb)
        {
            DrawRect(tex, x, y, 3, height, wood);
            DrawCircle(tex, x + 1, y + height, 5, orb);
            // Glow around orb
            for (int ox = -7; ox <= 7; ox++)
            {
                for (int oy = -7; oy <= 7; oy++)
                {
                    float dist = Mathf.Sqrt(ox * ox + oy * oy);
                    if (dist > 5 && dist < 8)
                    {
                        Color glow = new Color(orb.r, orb.g, orb.b, 0.3f * (1 - (dist - 5) / 3f));
                        int px = x + 1 + ox;
                        int py = y + height + oy;
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        {
                            Color existing = tex.GetPixel(px, py);
                            tex.SetPixel(px, py, Color.Lerp(existing, glow, glow.a));
                        }
                    }
                }
            }
        }

        private static void DrawMagicParticles(Texture2D tex, Color color)
        {
            System.Random rng = new System.Random(42);
            for (int i = 0; i < 8; i++)
            {
                int x = rng.Next(5, tex.width - 5);
                int y = rng.Next(5, tex.height - 5);
                float alpha = 0.3f + (float)rng.NextDouble() * 0.5f;
                Color c = new Color(color.r, color.g, color.b, alpha);
                tex.SetPixel(x, y, c);
                tex.SetPixel(x + 1, y, c * 0.5f);
                tex.SetPixel(x - 1, y, c * 0.5f);
            }
        }

        // More helper methods for other sprites...
        private static void DrawArmorPiece(Texture2D tex, int x, int y, int w, int h, Color baseC, Color highlight, Color shadow)
        {
            for (int px = 0; px < w; px++)
            {
                for (int py = 0; py < h; py++)
                {
                    float edgeDist = Mathf.Min(px, w - px - 1, py, h - py - 1) / 3f;
                    Color c = Color.Lerp(shadow, baseC, Mathf.Clamp01(edgeDist));
                    float shine = Mathf.Max(0, 1 - Mathf.Abs(px - w * 0.3f) / (w * 0.3f));
                    c = Color.Lerp(c, highlight, shine * 0.3f);
                    tex.SetPixel(x + px, y + py, c);
                }
            }
        }

        private static void DrawEmblem(Texture2D tex, int cx, int cy, int size, Color color)
        {
            // Simple cross emblem
            DrawRect(tex, cx - 1, cy - size / 2, 3, size, color);
            DrawRect(tex, cx - size / 2, cy - 1, size, 3, color);
        }

        private static void DrawHelmet(Texture2D tex, int x, int y, int w, int h, Color baseC, Color highlight, Color shadow)
        {
            DrawArmorPiece(tex, x, y, w, h, baseC, highlight, shadow);
            // Visor slits
            int slitY = y + h / 2;
            DrawRect(tex, x + 4, slitY, w - 8, 2, shadow);
        }

        private static void DrawSword(Texture2D tex, int x, int y, int length, Color blade, Color handle)
        {
            // Handle
            DrawRect(tex, x, y, 4, 8, handle);
            // Guard
            DrawRect(tex, x - 2, y + 7, 8, 3, blade * 0.7f);
            // Blade
            for (int i = 0; i < length - 10; i++)
            {
                int width = 3 - (i * 2 / (length - 10));
                if (width < 1) width = 1;
                DrawRect(tex, x + 1 - width / 2, y + 10 + i, width, 1, blade);
            }
        }

        private static void DrawShield(Texture2D tex, int x, int y, int w, int h, Color main, Color border)
        {
            DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, main);
            // Border
            for (int angle = 0; angle < 360; angle += 5)
            {
                float rad = angle * Mathf.Deg2Rad;
                int px = x + w / 2 + Mathf.RoundToInt(Mathf.Cos(rad) * (w / 2 - 1));
                int py = y + h / 2 + Mathf.RoundToInt(Mathf.Sin(rad) * (h / 2 - 1));
                if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(px, py, border);
            }
        }

        // Placeholder implementations for remaining methods
        private static void DrawBoots(Texture2D tex, int x, int y, int w, int h, Color c) => DrawRect(tex, x, y, w, h, c);
        private static void DrawPatchedClothing(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2) => DrawRect(tex, x, y, w, h, c1);
        private static void DrawTunic(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2) => DrawRect(tex, x, y, w, h, c1);
        private static void DrawBelt(Texture2D tex, int x, int y, int w, Color c) => DrawRect(tex, x, y, w, 2, c);
        private static void DrawPeasantCap(Texture2D tex, int x, int y, int w, int h, Color c) => DrawRect(tex, x, y, w, h, c);
        private static void DrawAxe(Texture2D tex, int x, int y, int h, Color handle, Color head) { DrawRect(tex, x, y, 3, h, handle); DrawRect(tex, x - 4, y + h - 8, 8, 8, head); }

        private static void DrawGhostlyForm(Texture2D tex, int x, int y, int w, int h, Color baseC, Color highlight, Color shadow)
        {
            for (int px = 0; px < w; px++)
            {
                for (int py = 0; py < h; py++)
                {
                    float dist = Mathf.Sqrt(Mathf.Pow((px - w / 2f) / (w / 2f), 2) + Mathf.Pow((py - h / 2f) / (h / 2f), 2));
                    if (dist < 1)
                    {
                        float alpha = 1 - dist;
                        float wave = Mathf.Sin(py * 0.3f + px * 0.1f) * 0.2f;
                        Color c = Color.Lerp(shadow, baseC, alpha + wave);
                        c.a = alpha * baseC.a;
                        tex.SetPixel(x + px, y + py, c);
                    }
                }
            }
        }

        private static void DrawHollowEyes(Texture2D tex, int x, int y, int size, Color c) => DrawCircle(tex, x, y, size / 2, c);
        private static void DrawGhostMouth(Texture2D tex, int x, int y, int w, int h, Color c) => DrawEllipse(tex, x, y, w / 2, h / 2, c);
        private static void AddGlowEffect(Texture2D tex, int cx, int cy, int radius, Color c)
        {
            for (int ox = -radius; ox <= radius; ox++)
            {
                for (int oy = -radius; oy <= radius; oy++)
                {
                    float dist = Mathf.Sqrt(ox * ox + oy * oy);
                    if (dist < radius)
                    {
                        float alpha = c.a * (1 - dist / radius);
                        int px = cx + ox;
                        int py = cy + oy;
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        {
                            Color existing = tex.GetPixel(px, py);
                            Color glow = new Color(c.r, c.g, c.b, alpha);
                            tex.SetPixel(px, py, Color.Lerp(existing, glow, alpha));
                        }
                    }
                }
            }
        }

        private static void DrawBone(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawRect(tex, x, y, w, h, c1);
        private static void DrawPelvis(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2) => DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawRibcage(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2)
        {
            for (int i = 0; i < 4; i++)
            {
                DrawRect(tex, x, y + i * 4, w, 2, c1);
            }
        }
        private static void DrawSpine(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2) => DrawRect(tex, x, y, w, h, c1);
        private static void DrawSkull(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2, Color c3)
        {
            DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c1);
            // Eye sockets
            DrawCircle(tex, x + w / 3, y + h / 2, 3, c2);
            DrawCircle(tex, x + 2 * w / 3, y + h / 2, 3, c2);
        }
        private static void DrawGlowingEyes(Texture2D tex, int x, int y, int size, Color c)
        {
            DrawCircle(tex, x, y, size, c);
            AddGlowEffect(tex, x, y, size + 2, new Color(c.r, c.g, c.b, 0.5f));
        }

        private static void DrawSpiderLeg(Texture2D tex, int x, int y, float angle, Color c)
        {
            float rad = angle * Mathf.Deg2Rad;
            for (int i = 0; i < 16; i++)
            {
                int px = x + Mathf.RoundToInt(Mathf.Cos(rad) * i);
                int py = y + Mathf.RoundToInt(Mathf.Sin(rad) * i);
                if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(px, py, c);
            }
        }
        private static void DrawSpiderBody(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2) => DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawSpiderEyes(Texture2D tex, int x, int y, Color c)
        {
            for (int i = 0; i < 8; i++)
            {
                tex.SetPixel(x + (i % 4) * 2, y + (i / 4) * 2, c);
            }
        }
        private static void DrawFangs(Texture2D tex, int x, int y, int size, Color c)
        {
            DrawRect(tex, x, y, 2, size, c);
            DrawRect(tex, x + 4, y, 2, size, c);
        }

        private static void DrawBatWing(Texture2D tex, int x, int y, int w, int h, Color membrane, Color bone, bool left)
        {
            DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, membrane);
            // Wing bones
            for (int i = 0; i < 3; i++)
            {
                float angle = (left ? 30 : -30) + i * (left ? -20 : 20);
                float rad = angle * Mathf.Deg2Rad;
                for (int j = 0; j < w / 2; j++)
                {
                    int px = x + w / 2 + Mathf.RoundToInt(Mathf.Cos(rad) * j);
                    int py = y + h / 2 + Mathf.RoundToInt(Mathf.Sin(rad) * j);
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, bone);
                }
            }
        }
        private static void DrawFurryBody(Texture2D tex, int x, int y, int w, int h, Color c) => DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c);
        private static void DrawBatHead(Texture2D tex, int x, int y, int w, int h, Color c) => DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c);
        private static void DrawBatEars(Texture2D tex, int x, int y, int w, int h, Color c)
        {
            for (int i = 0; i < h; i++)
            {
                int rowW = w - (i * w / h);
                DrawRect(tex, x + (w - rowW) / 2, y + i, rowW, 1, c);
            }
        }

        private static void DrawHooves(Texture2D tex, int x, int y, int size, Color c) => DrawRect(tex, x, y, size, size, c);
        private static void DrawMuscularLimb(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawMuscularTorso(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawClaws(Texture2D tex, int x, int y, int size, Color c)
        {
            for (int i = 0; i < 3; i++)
            {
                DrawRect(tex, x + i * 2, y, 1, size, c);
            }
        }
        private static void DrawDemonHead(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2) => DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawHorn(Texture2D tex, int x, int y, int w, int h, Color c, bool left)
        {
            for (int i = 0; i < h; i++)
            {
                int rowW = w - (i * w / h);
                int offset = left ? 0 : (w - rowW);
                DrawRect(tex, x + offset, y + i, rowW, 1, c);
            }
        }
        private static void AddFireAura(Texture2D tex, int cx, int cy, int radius)
        {
            System.Random rng = new System.Random(123);
            for (int i = 0; i < 20; i++)
            {
                float angle = (float)rng.NextDouble() * 360;
                float dist = radius + (float)rng.NextDouble() * 8;
                int px = cx + Mathf.RoundToInt(Mathf.Cos(angle * Mathf.Deg2Rad) * dist);
                int py = cy + Mathf.RoundToInt(Mathf.Sin(angle * Mathf.Deg2Rad) * dist);
                if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                {
                    Color fireColor = Color.Lerp(new Color(1f, 0.3f, 0f, 0.5f), new Color(1f, 0.8f, 0f, 0.3f), (float)rng.NextDouble());
                    tex.SetPixel(px, py, fireColor);
                }
            }
        }

        private static void DrawBandagedLimb(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2, Color c3)
        {
            for (int py = 0; py < h; py++)
            {
                Color c = (py % 4 < 2) ? c1 : c2;
                DrawRect(tex, x, y + py, w, 1, c);
            }
        }
        private static void DrawBandagedTorso(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawBandagedLimb(tex, x, y, w, h, c1, c2, c3);
        private static void DrawMummyHead(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2, Color rot)
        {
            DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c1);
            // Bandage wrapping pattern
            for (int i = 0; i < 3; i++)
            {
                DrawRect(tex, x, y + 2 + i * 4, w, 2, c2);
            }
        }
        private static void AddDustParticles(Texture2D tex, Color c)
        {
            System.Random rng = new System.Random(321);
            for (int i = 0; i < 12; i++)
            {
                int x = rng.Next(0, tex.width);
                int y = rng.Next(0, tex.height);
                tex.SetPixel(x, y, c);
            }
        }

        private static void DrawStoneTile(Texture2D tex, Color baseC, Color light, Color dark, Color grout, int variation)
        {
            // Fill base
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.1f + variation, y * 0.1f) * 0.3f;
                    Color c = Color.Lerp(dark, light, noise + 0.35f);
                    tex.SetPixel(x, y, c);
                }
            }
            // Grout lines
            DrawRect(tex, 0, 0, tex.width, 2, grout);
            DrawRect(tex, 0, tex.height - 2, tex.width, 2, grout);
            DrawRect(tex, 0, 0, 2, tex.height, grout);
            DrawRect(tex, tex.width - 2, 0, 2, tex.height, grout);
            // Center cross grout
            DrawRect(tex, tex.width / 2 - 1, 0, 2, tex.height, grout);
            DrawRect(tex, 0, tex.height / 2 - 1, tex.width, 2, grout);
        }

        private static void DrawBrickWall(Texture2D tex, Color baseC, Color light, Color dark, Color mortar, bool vertical)
        {
            // Fill with mortar
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    tex.SetPixel(x, y, mortar);
                }
            }
            // Draw bricks
            int brickH = 8;
            int brickW = 16;
            for (int row = 0; row < tex.height / brickH; row++)
            {
                int offset = (row % 2) * (brickW / 2);
                for (int col = -1; col < tex.width / brickW + 1; col++)
                {
                    int bx = col * brickW + offset;
                    int by = row * brickH;
                    float noise = Mathf.PerlinNoise(col * 0.5f, row * 0.5f) * 0.3f;
                    Color brickColor = Color.Lerp(dark, light, noise + 0.35f);
                    DrawRect(tex, bx + 1, by + 1, brickW - 2, brickH - 2, brickColor);
                }
            }
        }

        private static void DrawWoodenDoor(Texture2D tex, int x, int y, int w, int h, Color dark, Color mid, Color light)
        {
            // Door background
            DrawRect(tex, x, y, w, h, mid);
            // Planks
            int plankW = w / 4;
            for (int i = 0; i < 4; i++)
            {
                int px = x + i * plankW;
                float shade = (i % 2 == 0) ? 0.9f : 1.1f;
                Color plankColor = new Color(mid.r * shade, mid.g * shade, mid.b * shade);
                DrawRect(tex, px + 1, y, plankW - 2, h, plankColor);
            }
            // Wood grain
            for (int py = y; py < y + h; py += 3)
            {
                Color grainColor = new Color(dark.r, dark.g, dark.b, 0.3f);
                DrawRect(tex, x, py, w, 1, grainColor);
            }
        }

        private static void DrawMetalHinge(Texture2D tex, int x, int y, int size, Color c)
        {
            DrawRect(tex, x, y, size, size / 2, c);
            DrawCircle(tex, x + size / 2, y + size / 4, size / 4, c);
        }

        private static void DrawDoorHandle(Texture2D tex, int x, int y, int size, Color c)
        {
            DrawCircle(tex, x, y, size / 2, c);
            DrawCircle(tex, x, y, size / 4, c * 0.7f);
        }

        private static void DrawKeyhole(Texture2D tex, int x, int y, int size, Color c)
        {
            DrawCircle(tex, x, y + size / 3, size / 3, c);
            DrawRect(tex, x - 1, y - size / 2, 3, size / 2, c);
        }

        private static Color GetKeyColorFromString(string colorName)
        {
            return colorName.ToLower() switch
            {
                "red" => Color.red,
                "blue" => Color.blue,
                "green" => Color.green,
                "yellow" => Color.yellow,
                "cyan" => Color.cyan,
                "magenta" => Color.magenta,
                _ => Color.gray
            };
        }

        private static void DrawAnimatedFlame(Texture2D tex, int x, int y, int w, int h, int frame)
        {
            float flicker = Mathf.Sin(frame * 1.5f) * 0.2f;
            Color[] flameColors = {
                new Color(1f, 0.95f, 0.4f),     // Yellow core
                new Color(1f, 0.6f, 0.1f),       // Orange
                new Color(0.9f, 0.2f, 0.05f),    // Red
                new Color(0.3f, 0.1f, 0.05f)     // Dark edge
            };

            for (int py = 0; py < h; py++)
            {
                float t = py / (float)h;
                int rowW = (int)(w * (1 - t * 0.7f) + Mathf.Sin(py * 0.5f + frame) * 2);
                int startX = x + (w - rowW) / 2;

                for (int px = 0; px < rowW; px++)
                {
                    float centerDist = Mathf.Abs(px - rowW / 2f) / (rowW / 2f);
                    int colorIdx = Mathf.Min(3, (int)(centerDist * 3 + t));
                    Color c = flameColors[colorIdx];
                    c.a = 1 - t * 0.3f;
                    if (startX + px >= 0 && startX + px < tex.width && y + py < tex.height)
                        tex.SetPixel(startX + px, y + py, c);
                }
            }
        }

        private static void DrawStaircaseSteps(Texture2D tex, int x, int y, int w, int h, Color baseC, Color dark, Color light, bool goingUp)
        {
            int steps = 6;
            int stepH = h / steps;
            int stepW = w;

            for (int i = 0; i < steps; i++)
            {
                int sy = goingUp ? (y + i * stepH) : (y + h - (i + 1) * stepH);
                int depth = goingUp ? (steps - i) : (i + 1);

                // Step top
                DrawRect(tex, x, sy, stepW, stepH - 2, light);
                // Step front
                DrawRect(tex, x, sy, stepW, 2, dark);
                // Shading
                Color shaded = Color.Lerp(baseC, dark, depth * 0.1f);
                DrawRect(tex, x + 2, sy + 2, stepW - 4, stepH - 4, shaded);
            }
        }

        private static void DrawArrowIndicator(Texture2D tex, int x, int y, int size, bool up, Color c)
        {
            for (int row = 0; row < size; row++)
            {
                int rowW = up ? (size - row) : (row + 1);
                int startX = x + (size - rowW) / 2;
                DrawRect(tex, startX, up ? (y + row) : (y + size - row - 1), rowW, 1, c);
            }
        }

        // Decoration drawing methods
        private static void DrawChainLink(Texture2D tex, int x, int y, int w, int h, Color dark, Color light)
        {
            // Outer ring
            for (int angle = 0; angle < 360; angle += 10)
            {
                float rad = angle * Mathf.Deg2Rad;
                int px = x + w / 2 + Mathf.RoundToInt(Mathf.Cos(rad) * (w / 2 - 1));
                int py = y + h / 2 + Mathf.RoundToInt(Mathf.Sin(rad) * (h / 2 - 1));
                Color c = (angle > 90 && angle < 270) ? dark : light;
                if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(px, py, c);
            }
        }

        private static void DrawCobweb(Texture2D tex, int x, int y, int size, Color c)
        {
            // Radial threads from corner
            for (int i = 0; i < 8; i++)
            {
                float angle = (90f / 8) * i;
                float rad = angle * Mathf.Deg2Rad;
                for (int d = 0; d < size; d++)
                {
                    int px = x + Mathf.RoundToInt(Mathf.Cos(rad) * d);
                    int py = y - Mathf.RoundToInt(Mathf.Sin(rad) * d);
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, c);
                }
            }
            // Concentric arcs
            for (int r = size / 4; r < size; r += size / 4)
            {
                for (int angle = 0; angle < 90; angle += 5)
                {
                    float rad = angle * Mathf.Deg2Rad;
                    int px = x + Mathf.RoundToInt(Mathf.Cos(rad) * r);
                    int py = y - Mathf.RoundToInt(Mathf.Sin(rad) * r);
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        tex.SetPixel(px, py, c);
                }
            }
        }

        private static void DrawBarrel(Texture2D tex, int x, int y, int w, int h, Color dark, Color light, Color metal)
        {
            // Main body
            DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, light);
            // Planks
            int plankW = w / 5;
            for (int i = 0; i < 5; i++)
            {
                float shade = (i % 2 == 0) ? 0.9f : 1f;
                Color c = new Color(light.r * shade, light.g * shade, light.b * shade);
                DrawRect(tex, x + i * plankW, y + h / 4, plankW - 1, h / 2, c);
            }
            // Metal bands
            DrawRect(tex, x, y + 4, w, 3, metal);
            DrawRect(tex, x, y + h - 7, w, 3, metal);
            DrawRect(tex, x, y + h / 2 - 1, w, 2, metal);
        }

        private static void DrawChandelier(Texture2D tex, int x, int y, int w, int h, Color gold, Color dark, Color candle)
        {
            // Center piece
            DrawRect(tex, x + w / 2 - 2, y + h - 8, 4, 8, dark);
            // Arms
            int armY = y + h - 12;
            for (int i = 0; i < 5; i++)
            {
                int armX = x + (i * w / 4);
                DrawRect(tex, x + w / 2, armY, armX - x - w / 2 + 4, 2, gold);
                // Candle holder
                DrawRect(tex, armX, armY - 8, 4, 10, dark);
                // Candle
                DrawRect(tex, armX + 1, armY - 16, 2, 8, candle);
                // Flame
                tex.SetPixel(armX + 1, armY - 17, new Color(1f, 0.8f, 0.2f));
                tex.SetPixel(armX + 2, armY - 17, new Color(1f, 0.8f, 0.2f));
            }
        }

        private static void DrawPaintingFrame(Texture2D tex, int x, int y, int w, int h, Color dark, Color gold)
        {
            // Outer frame
            DrawRect(tex, x, y, w, h, dark);
            // Gold trim
            DrawRect(tex, x + 1, y + 1, w - 2, 2, gold);
            DrawRect(tex, x + 1, y + h - 3, w - 2, 2, gold);
            DrawRect(tex, x + 1, y + 1, 2, h - 2, gold);
            DrawRect(tex, x + w - 3, y + 1, 2, h - 2, gold);
        }

        private static void DrawPaintingContent(Texture2D tex, int x, int y, int w, int h, int variant)
        {
            Color[] bgColors = { new Color(0.3f, 0.2f, 0.15f), new Color(0.15f, 0.2f, 0.3f), new Color(0.2f, 0.25f, 0.15f) };
            DrawRect(tex, x, y, w, h, bgColors[variant % 3]);
            // Simple portrait silhouette
            DrawEllipse(tex, x + w / 2, y + h * 2 / 3, w / 4, h / 4, new Color(0.2f, 0.15f, 0.1f));
            DrawRect(tex, x + w / 3, y, w / 3, h / 2, new Color(0.2f, 0.15f, 0.1f));
        }

        private static void DrawArmorStand(Texture2D tex, int x, int y, int w, int h, Color metal, Color highlight, Color shadow, Color wood)
        {
            // Wooden stand
            DrawRect(tex, x + w / 2 - 2, y, 4, 8, wood);
            DrawRect(tex, x + w / 4, y, w / 2, 3, wood);
            // Armor display
            // Helmet
            DrawEllipse(tex, x + w / 2, y + h - 6, w / 3, 6, metal);
            // Breastplate
            DrawRect(tex, x + w / 4, y + h / 3, w / 2, h / 3, metal);
            // Shoulder pads
            DrawEllipse(tex, x + w / 5, y + h / 2 + 4, 6, 4, highlight);
            DrawEllipse(tex, x + 4 * w / 5, y + h / 2 + 4, 6, 4, highlight);
        }

        private static void DrawBanner(Texture2D tex, int x, int y, int w, int h, Color fabric, Color pole)
        {
            // Pole
            DrawRect(tex, x + w / 2 - 1, y - 4, 2, h + 8, pole);
            // Banner fabric with wave
            for (int py = 0; py < h; py++)
            {
                int wave = (int)(Mathf.Sin(py * 0.2f) * 2);
                DrawRect(tex, x + wave, y + py, w, 1, fabric);
            }
            // Bottom fringe
            for (int i = 0; i < w; i += 3)
            {
                DrawRect(tex, x + i, y, 2, 4, fabric * 0.8f);
            }
        }

        private static void DrawGothicWindow(Texture2D tex, int x, int y, int w, int h, Color frame, Color glass, Color lead)
        {
            // Stone frame
            DrawRect(tex, x, y, w, h, frame);
            // Glass area (pointed arch top)
            int glassX = x + 3;
            int glassW = w - 6;
            int glassH = h - 12;
            DrawRect(tex, glassX, y + 3, glassW, glassH, glass);
            // Pointed top
            for (int row = 0; row < 8; row++)
            {
                int rowW = glassW - row * 2;
                if (rowW > 0)
                    DrawRect(tex, glassX + row, y + 3 + glassH + row, rowW, 1, glass);
            }
            // Lead dividers
            DrawRect(tex, x + w / 2 - 1, y + 3, 2, h - 6, lead);
            DrawRect(tex, glassX, y + h / 3, glassW, 2, lead);
            DrawRect(tex, glassX, y + 2 * h / 3, glassW, 2, lead);
        }

        #endregion
    }
}
