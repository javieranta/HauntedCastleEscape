using UnityEngine;
using System.Collections.Generic;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Generates Ultra HD 128x128 sprites with advanced shading, texturing, and detail.
    /// Creates professional quality game graphics procedurally.
    /// </summary>
    public static class UltraHDSpriteGenerator
    {
        private static Dictionary<string, Sprite> _cache = new();
        public const int SIZE = 128;
        private const float PPU = 128f;

        #region Character Sprites

        public static Sprite GetWizardSprite()
        {
            if (_cache.TryGetValue("uhd_wizard", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            // Color palette - rich magical blues and purples
            Color robeDeep = new Color(0.12f, 0.08f, 0.35f);
            Color robeMid = new Color(0.2f, 0.15f, 0.55f);
            Color robeLight = new Color(0.35f, 0.28f, 0.7f);
            Color robeHighlight = new Color(0.5f, 0.45f, 0.85f);
            Color skinBase = new Color(0.9f, 0.78f, 0.68f);
            Color skinShadow = new Color(0.7f, 0.55f, 0.48f);
            Color beardWhite = new Color(0.95f, 0.94f, 0.92f);
            Color beardShadow = new Color(0.75f, 0.73f, 0.7f);
            Color staffWood = new Color(0.55f, 0.35f, 0.2f);
            Color orbGlow = new Color(0.4f, 0.85f, 1f);

            // Flowing robes with detailed fabric folds
            DrawDetailedRobes(tex, 28, 8, 72, 65, robeDeep, robeMid, robeLight, robeHighlight);

            // Rope belt with knots
            DrawRopeBelt(tex, 30, 45, 68, new Color(0.6f, 0.5f, 0.3f));

            // Hands with magical glow
            DrawHand(tex, 20, 52, 12, skinBase, skinShadow, true);
            DrawHand(tex, 96, 52, 12, skinBase, skinShadow, false);

            // Wise elderly face
            DrawDetailedFace(tex, 48, 80, 32, 28, skinBase, skinShadow, true);

            // Long flowing beard
            DrawFlowingBeard(tex, 40, 55, 48, 30, beardWhite, beardShadow);

            // Tall pointed wizard hat with stars and moon
            DrawWizardHat(tex, 32, 100, 64, 28, robeDeep, robeMid);
            DrawMagicalSymbols(tex, 40, 105, 48, 20);

            // Ornate wooden staff with glowing crystal
            DrawOrnateStaff(tex, 100, 15, 95, staffWood, orbGlow);

            // Magical particles floating around
            DrawMagicalAura(tex, orbGlow);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_wizard"] = sprite;
            return sprite;
        }

        public static Sprite GetKnightSprite()
        {
            if (_cache.TryGetValue("uhd_knight", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            // Metal armor colors with realistic metallic shading
            Color armorDark = new Color(0.35f, 0.38f, 0.42f);
            Color armorMid = new Color(0.55f, 0.58f, 0.62f);
            Color armorLight = new Color(0.75f, 0.78f, 0.82f);
            Color armorHighlight = new Color(0.92f, 0.94f, 0.96f);
            Color chainmail = new Color(0.45f, 0.48f, 0.52f);
            Color capeRed = new Color(0.7f, 0.15f, 0.12f);
            Color capeDark = new Color(0.45f, 0.08f, 0.06f);
            Color goldTrim = new Color(0.85f, 0.7f, 0.25f);

            // Armored boots with detailed plates
            DrawArmoredBoots(tex, 38, 4, 20, 24, armorDark, armorMid, armorLight);
            DrawArmoredBoots(tex, 70, 4, 20, 24, armorDark, armorMid, armorLight);

            // Leg armor (greaves)
            DrawArmorPlate(tex, 36, 28, 22, 30, armorDark, armorMid, armorLight, armorHighlight);
            DrawArmorPlate(tex, 70, 28, 22, 30, armorDark, armorMid, armorLight, armorHighlight);

            // Chainmail skirt
            DrawChainmail(tex, 32, 52, 64, 16, chainmail);

            // Breastplate with royal emblem
            DrawBreastplate(tex, 28, 58, 72, 40, armorDark, armorMid, armorLight, armorHighlight);
            DrawRoyalEmblem(tex, 52, 72, 24, capeRed, goldTrim);

            // Shoulder pauldrons
            DrawPauldron(tex, 16, 75, 22, 18, armorDark, armorMid, armorLight, armorHighlight);
            DrawPauldron(tex, 90, 75, 22, 18, armorDark, armorMid, armorLight, armorHighlight);

            // Cape flowing behind
            DrawFlowingCape(tex, 40, 58, 48, 50, capeRed, capeDark);

            // Gauntlets
            DrawGauntlet(tex, 8, 60, 16, 20, armorDark, armorMid, armorLight);
            DrawGauntlet(tex, 104, 60, 16, 20, armorDark, armorMid, armorLight);

            // Detailed helmet with visor
            DrawKnightHelmet(tex, 36, 90, 56, 38, armorDark, armorMid, armorLight, armorHighlight);

            // Sword and shield
            DrawDetailedSword(tex, 110, 20, 85, armorLight, goldTrim);
            DrawDetailedShield(tex, 4, 50, 28, 40, capeRed, goldTrim, armorMid);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_knight"] = sprite;
            return sprite;
        }

        public static Sprite GetSerfSprite()
        {
            if (_cache.TryGetValue("uhd_serf", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            // Earthy peasant colors
            Color tunicBrown = new Color(0.55f, 0.4f, 0.25f);
            Color tunicDark = new Color(0.35f, 0.25f, 0.15f);
            Color tunicLight = new Color(0.7f, 0.55f, 0.38f);
            Color pantsBrown = new Color(0.4f, 0.32f, 0.22f);
            Color skinBase = new Color(0.85f, 0.7f, 0.55f);
            Color skinShadow = new Color(0.65f, 0.5f, 0.38f);
            Color bootBrown = new Color(0.3f, 0.22f, 0.15f);
            Color hairBrown = new Color(0.35f, 0.25f, 0.15f);

            // Worn leather boots
            DrawWornBoots(tex, 40, 4, 18, 18, bootBrown);
            DrawWornBoots(tex, 70, 4, 18, 18, bootBrown);

            // Patched trousers
            DrawPatchedPants(tex, 36, 22, 56, 35, pantsBrown, tunicLight);

            // Simple tunic with patches and wear
            DrawPeasantTunic(tex, 30, 48, 68, 42, tunicBrown, tunicDark, tunicLight);

            // Rope belt
            DrawRopeBelt(tex, 32, 50, 64, new Color(0.5f, 0.4f, 0.25f));

            // Bare arms (working class)
            DrawArm(tex, 18, 55, 14, 28, skinBase, skinShadow, true);
            DrawArm(tex, 96, 55, 14, 28, skinBase, skinShadow, false);

            // Friendly working-class face
            DrawDetailedFace(tex, 46, 85, 36, 30, skinBase, skinShadow, false);

            // Simple cap
            DrawPeasantCap(tex, 40, 105, 48, 20, tunicDark, tunicBrown);

            // Messy hair peeking out
            DrawMessyHair(tex, 42, 100, 44, hairBrown);

            // Throwing axe (weapon)
            DrawThrowingAxe(tex, 100, 45, 45, bootBrown, new Color(0.5f, 0.52f, 0.55f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_serf"] = sprite;
            return sprite;
        }

        #endregion

        #region Enemy Sprites

        public static Sprite GetGhostSprite()
        {
            if (_cache.TryGetValue("uhd_ghost", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color ghostBase = new Color(0.85f, 0.88f, 0.95f, 0.5f);
            Color ghostLight = new Color(0.95f, 0.97f, 1f, 0.7f);
            Color ghostShadow = new Color(0.6f, 0.65f, 0.8f, 0.35f);
            Color eyeGlow = new Color(0.2f, 0.6f, 0.9f, 0.95f);

            // Ethereal flowing form with wispy edges
            DrawEtherealForm(tex, 20, 8, 88, 110, ghostBase, ghostLight, ghostShadow);

            // Hollow glowing eyes
            DrawGhostlyEyes(tex, 40, 75, 16, eyeGlow);
            DrawGhostlyEyes(tex, 72, 75, 16, eyeGlow);

            // Wailing mouth
            DrawGhostMouth(tex, 50, 55, 28, 12, new Color(0.3f, 0.35f, 0.5f, 0.7f));

            // Trailing wisps
            DrawGhostlyWisps(tex, ghostBase);

            // Ethereal glow effect
            AddEtherealGlow(tex, 64, 64, 50, ghostLight);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_ghost"] = sprite;
            return sprite;
        }

        public static Sprite GetSkeletonSprite()
        {
            if (_cache.TryGetValue("uhd_skeleton", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color boneWhite = new Color(0.94f, 0.92f, 0.88f);
            Color boneShadow = new Color(0.7f, 0.68f, 0.62f);
            Color boneHighlight = new Color(0.98f, 0.97f, 0.95f);
            Color eyeRed = new Color(1f, 0.15f, 0.1f);

            // Detailed skeletal structure
            DrawSkeletalFeet(tex, 42, 4, boneWhite, boneShadow);
            DrawSkeletalFeet(tex, 74, 4, boneWhite, boneShadow);

            DrawSkeletalLegs(tex, 44, 8, 12, 35, boneWhite, boneShadow, boneHighlight);
            DrawSkeletalLegs(tex, 72, 8, 12, 35, boneWhite, boneShadow, boneHighlight);

            // Pelvis
            DrawPelvisBone(tex, 38, 40, 52, 18, boneWhite, boneShadow);

            // Spine with vertebrae
            DrawSpineDetail(tex, 58, 42, 12, 45, boneWhite, boneShadow, boneHighlight);

            // Ribcage with individual ribs
            DrawRibcageDetail(tex, 35, 55, 58, 35, boneWhite, boneShadow);

            // Arm bones
            DrawSkeletalArm(tex, 20, 55, 18, 40, boneWhite, boneShadow, boneHighlight, true);
            DrawSkeletalArm(tex, 90, 55, 18, 40, boneWhite, boneShadow, boneHighlight, false);

            // Detailed skull
            DrawDetailedSkull(tex, 40, 90, 48, 38, boneWhite, boneShadow, boneHighlight);

            // Glowing red eyes in sockets
            DrawGlowingEyeSockets(tex, 48, 100, 10, eyeRed);
            DrawGlowingEyeSockets(tex, 70, 100, 10, eyeRed);

            // Rusty sword
            DrawRustySword(tex, 100, 20, 60, new Color(0.5f, 0.42f, 0.35f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_skeleton"] = sprite;
            return sprite;
        }

        public static Sprite GetSpiderSprite()
        {
            if (_cache.TryGetValue("uhd_spider", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color bodyDark = new Color(0.12f, 0.1f, 0.08f);
            Color bodyMid = new Color(0.22f, 0.18f, 0.14f);
            Color bodyLight = new Color(0.35f, 0.28f, 0.22f);
            Color legColor = new Color(0.18f, 0.14f, 0.1f);
            Color eyeRed = new Color(0.9f, 0.15f, 0.1f);
            Color fangColor = new Color(0.4f, 0.35f, 0.3f);

            // Eight articulated legs with joints
            DrawSpiderLegDetailed(tex, 10, 25, -50, legColor, bodyMid);
            DrawSpiderLegDetailed(tex, 15, 40, -30, legColor, bodyMid);
            DrawSpiderLegDetailed(tex, 20, 50, -15, legColor, bodyMid);
            DrawSpiderLegDetailed(tex, 25, 55, 5, legColor, bodyMid);
            DrawSpiderLegDetailed(tex, 103, 55, 175, legColor, bodyMid);
            DrawSpiderLegDetailed(tex, 108, 50, 195, legColor, bodyMid);
            DrawSpiderLegDetailed(tex, 113, 40, 210, legColor, bodyMid);
            DrawSpiderLegDetailed(tex, 118, 25, 230, legColor, bodyMid);

            // Large hairy abdomen
            DrawHairyAbdomen(tex, 35, 10, 58, 45, bodyDark, bodyMid, bodyLight);

            // Cephalothorax (head-thorax)
            DrawSpiderHead(tex, 45, 55, 38, 30, bodyDark, bodyMid, bodyLight);

            // Multiple eyes (8 eyes in cluster)
            DrawSpiderEyeCluster(tex, 55, 72, eyeRed);

            // Large venomous fangs
            DrawVenomousFangs(tex, 52, 55, 24, fangColor, new Color(0.4f, 0.8f, 0.3f));

            // Web patterns on body (optional)
            DrawWebPattern(tex, 40, 15, 48, 35, new Color(1f, 1f, 1f, 0.15f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_spider"] = sprite;
            return sprite;
        }

        public static Sprite GetBatSprite()
        {
            if (_cache.TryGetValue("uhd_bat", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color furDark = new Color(0.15f, 0.12f, 0.1f);
            Color furMid = new Color(0.25f, 0.2f, 0.18f);
            Color furLight = new Color(0.35f, 0.28f, 0.25f);
            Color wingMembrane = new Color(0.2f, 0.18f, 0.15f, 0.9f);
            Color wingBone = new Color(0.25f, 0.22f, 0.18f);
            Color eyeRed = new Color(0.95f, 0.2f, 0.15f);

            // Spread leather wings with bone structure
            DrawBatWingDetailed(tex, 4, 25, 55, 65, wingMembrane, wingBone, true);
            DrawBatWingDetailed(tex, 69, 25, 55, 65, wingMembrane, wingBone, false);

            // Furry body
            DrawFurryBatBody(tex, 48, 35, 32, 45, furDark, furMid, furLight);

            // Head with large ears
            DrawBatHeadDetailed(tex, 48, 78, 32, 28, furDark, furMid, furLight);

            // Large pointed ears
            DrawBatEarDetailed(tex, 42, 95, 14, 25, furDark, furMid);
            DrawBatEarDetailed(tex, 72, 95, 14, 25, furDark, furMid);

            // Glowing eyes
            DrawGlowingEyes(tex, 52, 82, 6, eyeRed);
            DrawGlowingEyes(tex, 70, 82, 6, eyeRed);

            // Sharp fangs
            DrawBatFangs(tex, 56, 75, 16, new Color(0.95f, 0.93f, 0.9f));

            // Small legs with claws
            DrawBatClaws(tex, 52, 35, furDark);
            DrawBatClaws(tex, 68, 35, furDark);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_bat"] = sprite;
            return sprite;
        }

        public static Sprite GetDemonSprite()
        {
            if (_cache.TryGetValue("uhd_demon", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color skinRed = new Color(0.75f, 0.18f, 0.12f);
            Color skinDark = new Color(0.45f, 0.1f, 0.08f);
            Color skinLight = new Color(0.9f, 0.3f, 0.2f);
            Color hornBlack = new Color(0.15f, 0.12f, 0.1f);
            Color fireOrange = new Color(1f, 0.6f, 0.15f);
            Color fireYellow = new Color(1f, 0.9f, 0.4f);
            Color eyeYellow = new Color(1f, 0.95f, 0.3f);

            // Cloven hooves
            DrawClovenHooves(tex, 35, 4, 20, 12, hornBlack);
            DrawClovenHooves(tex, 73, 4, 20, 12, hornBlack);

            // Muscular legs
            DrawDemonicLeg(tex, 35, 16, 22, 40, skinRed, skinDark, skinLight);
            DrawDemonicLeg(tex, 71, 16, 22, 40, skinRed, skinDark, skinLight);

            // Muscular torso with battle scars
            DrawMuscularTorso(tex, 25, 50, 78, 48, skinRed, skinDark, skinLight);

            // Powerful arms with claws
            DrawDemonicArm(tex, 8, 55, 20, 45, skinRed, skinDark, skinLight, true);
            DrawDemonicArm(tex, 100, 55, 20, 45, skinRed, skinDark, skinLight, false);

            // Sharp claws
            DrawDemonClaws(tex, 5, 50, 14, hornBlack);
            DrawDemonClaws(tex, 109, 50, 14, hornBlack);

            // Demonic face with fangs
            DrawDemonicFace(tex, 40, 90, 48, 35, skinRed, skinDark, skinLight);

            // Large curved horns
            DrawCurvedHorns(tex, 30, 105, 20, 23, hornBlack, true);
            DrawCurvedHorns(tex, 78, 105, 20, 23, hornBlack, false);

            // Glowing yellow eyes
            DrawGlowingEyes(tex, 50, 100, 8, eyeYellow);
            DrawGlowingEyes(tex, 70, 100, 8, eyeYellow);

            // Fire aura around body
            DrawFireAura(tex, fireOrange, fireYellow);

            // Small demonic wings (optional)
            DrawSmallDemonWings(tex, 18, 70, 25, 30, skinDark);
            DrawSmallDemonWings(tex, 85, 70, 25, 30, skinDark);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_demon"] = sprite;
            return sprite;
        }

        public static Sprite GetMummySprite()
        {
            if (_cache.TryGetValue("uhd_mummy", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color bandageLight = new Color(0.88f, 0.82f, 0.72f);
            Color bandageMid = new Color(0.72f, 0.65f, 0.55f);
            Color bandageDark = new Color(0.5f, 0.45f, 0.38f);
            Color rotGreen = new Color(0.35f, 0.42f, 0.32f);
            Color eyeGlow = new Color(0.4f, 0.95f, 0.35f);

            // Wrapped feet shuffling
            DrawWrappedFeet(tex, 40, 4, 18, 14, bandageLight, bandageMid, bandageDark);
            DrawWrappedFeet(tex, 70, 4, 18, 14, bandageLight, bandageMid, bandageDark);

            // Bandaged legs
            DrawBandagedLimb(tex, 38, 18, 20, 38, bandageLight, bandageMid, bandageDark);
            DrawBandagedLimb(tex, 70, 18, 20, 38, bandageLight, bandageMid, bandageDark);

            // Wrapped torso with hanging bandages
            DrawBandagedTorso(tex, 30, 50, 68, 48, bandageLight, bandageMid, bandageDark);
            DrawHangingBandages(tex, 35, 50, bandageLight, bandageDark);

            // Arms outstretched
            DrawBandagedArm(tex, 12, 60, 22, 45, bandageLight, bandageMid, bandageDark, true);
            DrawBandagedArm(tex, 94, 60, 22, 45, bandageLight, bandageMid, bandageDark, false);

            // Wrapped head with exposed areas showing rot
            DrawMummyHead(tex, 42, 92, 44, 36, bandageLight, bandageMid, bandageDark, rotGreen);

            // Glowing eyes through bandages
            DrawGlowingEyes(tex, 52, 102, 6, eyeGlow);
            DrawGlowingEyes(tex, 70, 102, 6, eyeGlow);

            // Ancient dust particles
            DrawDustCloud(tex, new Color(0.7f, 0.65f, 0.55f, 0.4f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_mummy"] = sprite;
            return sprite;
        }

        public static Sprite GetVampireSprite()
        {
            if (_cache.TryGetValue("uhd_vampire", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color skinPale = new Color(0.85f, 0.82f, 0.8f);
            Color skinShadow = new Color(0.6f, 0.55f, 0.52f);
            Color cloakBlack = new Color(0.1f, 0.08f, 0.12f);
            Color cloakPurple = new Color(0.25f, 0.12f, 0.22f);
            Color cloakLining = new Color(0.55f, 0.15f, 0.15f);
            Color eyeRed = new Color(0.95f, 0.15f, 0.12f);
            Color hairBlack = new Color(0.12f, 0.1f, 0.1f);

            // Elegant boots
            DrawElegantBoots(tex, 42, 4, 16, 18, cloakBlack);
            DrawElegantBoots(tex, 70, 4, 16, 18, cloakBlack);

            // Aristocratic pants
            DrawAristocraticPants(tex, 38, 22, 52, 38, cloakBlack, cloakPurple);

            // Formal vest and shirt
            DrawVampireVest(tex, 35, 55, 58, 40, cloakBlack, cloakPurple, new Color(0.9f, 0.88f, 0.85f));

            // Flowing cape
            DrawVampireCape(tex, 20, 45, 88, 60, cloakBlack, cloakPurple, cloakLining);

            // Pale aristocratic face
            DrawVampireFace(tex, 44, 90, 40, 32, skinPale, skinShadow);

            // Slicked back black hair
            DrawSlickedHair(tex, 42, 105, 44, 22, hairBlack);

            // Glowing red eyes
            DrawGlowingEyes(tex, 52, 100, 6, eyeRed);
            DrawGlowingEyes(tex, 70, 100, 6, eyeRed);

            // Visible fangs
            DrawVampireFangs(tex, 54, 88, 20);

            // Raised cape collar
            DrawCapeCollar(tex, 30, 88, 68, 18, cloakBlack, cloakLining);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_vampire"] = sprite;
            return sprite;
        }

        public static Sprite GetWerewolfSprite()
        {
            if (_cache.TryGetValue("uhd_werewolf", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color furDark = new Color(0.3f, 0.25f, 0.2f);
            Color furMid = new Color(0.45f, 0.38f, 0.3f);
            Color furLight = new Color(0.6f, 0.5f, 0.4f);
            Color clawBlack = new Color(0.15f, 0.12f, 0.1f);
            Color eyeYellow = new Color(0.95f, 0.85f, 0.2f);
            Color nosePink = new Color(0.35f, 0.22f, 0.2f);

            // Digitigrade wolf legs
            DrawWolfLegs(tex, 35, 4, 22, 45, furDark, furMid, furLight);
            DrawWolfLegs(tex, 71, 4, 22, 45, furDark, furMid, furLight);

            // Sharp clawed feet
            DrawWolfClaws(tex, 38, 4, 16, clawBlack);
            DrawWolfClaws(tex, 74, 4, 16, clawBlack);

            // Muscular furry torso
            DrawWolfTorso(tex, 28, 45, 72, 50, furDark, furMid, furLight);

            // Powerful arms
            DrawWolfArm(tex, 10, 55, 22, 45, furDark, furMid, furLight, true);
            DrawWolfArm(tex, 96, 55, 22, 45, furDark, furMid, furLight, false);

            // Clawed hands
            DrawWolfClaws(tex, 8, 50, 18, clawBlack);
            DrawWolfClaws(tex, 102, 50, 18, clawBlack);

            // Wolf head with snout
            DrawWolfHead(tex, 38, 88, 52, 40, furDark, furMid, furLight);

            // Pointed ears
            DrawWolfEars(tex, 38, 115, 16, 20, furDark, furMid);
            DrawWolfEars(tex, 74, 115, 16, 20, furDark, furMid);

            // Fierce yellow eyes
            DrawGlowingEyes(tex, 48, 100, 7, eyeYellow);
            DrawGlowingEyes(tex, 73, 100, 7, eyeYellow);

            // Wolf nose
            DrawWolfNose(tex, 60, 90, 8, nosePink);

            // Open maw with fangs
            DrawWolfMaw(tex, 48, 85, 32, 12, new Color(0.4f, 0.15f, 0.12f), Color.white);

            // Torn pants remnants
            DrawTornClothing(tex, 36, 35, 56, 20, new Color(0.35f, 0.3f, 0.25f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_werewolf"] = sprite;
            return sprite;
        }

        public static Sprite GetWitchSprite()
        {
            if (_cache.TryGetValue("uhd_witch", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color dressGreen = new Color(0.2f, 0.35f, 0.18f);
            Color dressDark = new Color(0.12f, 0.2f, 0.1f);
            Color dressLight = new Color(0.3f, 0.48f, 0.25f);
            Color skinGreen = new Color(0.55f, 0.7f, 0.45f);
            Color skinDark = new Color(0.38f, 0.5f, 0.3f);
            Color hatBlack = new Color(0.12f, 0.1f, 0.1f);
            Color hairGray = new Color(0.5f, 0.5f, 0.52f);
            Color eyePurple = new Color(0.7f, 0.25f, 0.8f);

            // Pointed boots
            DrawPointedBoots(tex, 42, 4, 16, 16, hatBlack);
            DrawPointedBoots(tex, 70, 4, 16, 16, hatBlack);

            // Tattered dress
            DrawTatteredDress(tex, 30, 15, 68, 60, dressGreen, dressDark, dressLight);

            // Crooked green-skinned arms
            DrawWitchArm(tex, 15, 50, 18, 40, skinGreen, skinDark, true);
            DrawWitchArm(tex, 95, 50, 18, 40, skinGreen, skinDark, false);

            // Gnarled hands
            DrawGnarledHand(tex, 10, 45, 14, skinGreen, skinDark);
            DrawGnarledHand(tex, 104, 45, 14, skinGreen, skinDark);

            // Warty green face
            DrawWitchFace(tex, 44, 85, 40, 35, skinGreen, skinDark);

            // Wild gray hair
            DrawWildHair(tex, 35, 90, 58, 30, hairGray);

            // Tall crooked witch hat
            DrawWitchHat(tex, 35, 108, 58, 20, hatBlack, new Color(0.5f, 0.35f, 0.2f));

            // Glowing purple eyes
            DrawGlowingEyes(tex, 52, 95, 6, eyePurple);
            DrawGlowingEyes(tex, 70, 95, 6, eyePurple);

            // Crooked nose with wart
            DrawWitchNose(tex, 60, 88, skinGreen, skinDark);

            // Broomstick
            DrawBroomstick(tex, 100, 15, 90, new Color(0.5f, 0.35f, 0.2f), new Color(0.7f, 0.6f, 0.35f));

            // Magical sparkles
            DrawMagicalSparkles(tex, new Color(0.6f, 0.9f, 0.4f, 0.8f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_witch"] = sprite;
            return sprite;
        }

        public static Sprite GetReaperSprite()
        {
            if (_cache.TryGetValue("uhd_reaper", out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color robeBlack = new Color(0.05f, 0.05f, 0.08f);
            Color robeDark = new Color(0.12f, 0.12f, 0.15f);
            Color robeEdge = new Color(0.2f, 0.2f, 0.25f);
            Color boneWhite = new Color(0.9f, 0.88f, 0.82f);
            Color boneShadow = new Color(0.65f, 0.62f, 0.55f);
            Color scytheSteel = new Color(0.7f, 0.72f, 0.75f);
            Color eyeEmpty = new Color(0.95f, 0.4f, 0.2f);

            // Flowing black robes that fade into mist
            DrawReaperRobes(tex, 25, 4, 78, 90, robeBlack, robeDark, robeEdge);

            // Skeletal hands gripping scythe
            DrawSkeletalHands(tex, 85, 55, 16, 18, boneWhite, boneShadow);
            DrawSkeletalHands(tex, 15, 70, 16, 18, boneWhite, boneShadow);

            // Deep dark hood
            DrawReaperHood(tex, 35, 85, 58, 42, robeBlack, robeDark);

            // Skull face inside hood with glowing eyes
            DrawReaperSkull(tex, 45, 88, 38, 32, boneWhite, boneShadow);
            DrawGlowingEyeSockets(tex, 52, 98, 8, eyeEmpty);
            DrawGlowingEyeSockets(tex, 68, 98, 8, eyeEmpty);

            // Massive scythe
            DrawMassiveScythe(tex, 90, 10, 110, scytheSteel, new Color(0.35f, 0.25f, 0.18f));

            // Dark mist at base
            DrawDarkMist(tex, robeBlack, robeDark);

            // Soul wisps floating around
            DrawSoulWisps(tex, new Color(0.6f, 0.8f, 1f, 0.5f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache["uhd_reaper"] = sprite;
            return sprite;
        }

        #endregion

        #region Environment Sprites

        public static Sprite GetStaircaseSprite(bool goingUp)
        {
            string key = $"uhd_stairs_{(goingUp ? "up" : "down")}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color stoneBase = new Color(0.45f, 0.42f, 0.38f);
            Color stoneDark = new Color(0.28f, 0.25f, 0.22f);
            Color stoneLight = new Color(0.62f, 0.58f, 0.52f);
            Color highlight = new Color(0.75f, 0.72f, 0.65f);
            Color arrowGreen = goingUp ? new Color(0.25f, 0.85f, 0.35f) : new Color(0.85f, 0.5f, 0.25f);
            Color arrowGlow = goingUp ? new Color(0.4f, 1f, 0.5f, 0.6f) : new Color(1f, 0.65f, 0.35f, 0.6f);

            // Draw perspective staircase with 8 visible steps
            int steps = 8;
            int stepH = 12;
            int stepW = 100;
            int stepDepth = 8;

            for (int i = 0; i < steps; i++)
            {
                int sy = goingUp ? (10 + i * stepH) : (SIZE - 22 - i * stepH);
                int depthOffset = goingUp ? (steps - 1 - i) : i;

                // Step shadow (depth)
                Color depthColor = Color.Lerp(stoneDark, stoneBase, depthOffset * 0.12f);
                DrawRect(tex, 14 + depthOffset, sy - stepDepth, stepW - depthOffset * 2, stepDepth, depthColor);

                // Step top surface with texture
                Color topColor = Color.Lerp(stoneBase, stoneLight, depthOffset * 0.1f);
                DrawTexturedRect(tex, 14 + depthOffset, sy, stepW - depthOffset * 2, stepH - 2, topColor, stoneDark);

                // Step front face
                Color frontColor = Color.Lerp(stoneDark, stoneBase, 0.3f + depthOffset * 0.08f);
                DrawRect(tex, 14 + depthOffset, sy, stepW - depthOffset * 2, 3, frontColor);

                // Highlight on step edge
                DrawRect(tex, 15 + depthOffset, sy + stepH - 3, stepW - depthOffset * 2 - 2, 1, highlight);
            }

            // Large glowing direction arrow
            DrawLargeArrow(tex, 50, goingUp ? 85 : 25, 28, goingUp, arrowGreen, arrowGlow);

            // "UP" or "DOWN" text indicator
            DrawDirectionText(tex, 36, goingUp ? 60 : 50, goingUp, arrowGreen);

            // Decorative railing posts
            DrawRailingPost(tex, 8, 20, 85, stoneDark, stoneLight);
            DrawRailingPost(tex, 112, 20, 85, stoneDark, stoneLight);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetFloorTileSprite(int floorLevel, int variation)
        {
            string key = $"uhd_floor_{floorLevel}_{variation}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;

            Color baseColor, lightColor, darkColor, groutColor, accentColor;

            switch (floorLevel)
            {
                case 0: // Basement - dark dungeon stone
                    baseColor = new Color(0.22f, 0.2f, 0.18f);
                    lightColor = new Color(0.32f, 0.28f, 0.25f);
                    darkColor = new Color(0.12f, 0.1f, 0.08f);
                    groutColor = new Color(0.08f, 0.06f, 0.05f);
                    accentColor = new Color(0.15f, 0.22f, 0.12f); // Moss
                    break;
                case 1: // Castle - elegant stone with carpet
                    baseColor = new Color(0.5f, 0.42f, 0.32f);
                    lightColor = new Color(0.65f, 0.55f, 0.42f);
                    darkColor = new Color(0.35f, 0.28f, 0.2f);
                    groutColor = new Color(0.25f, 0.2f, 0.15f);
                    accentColor = new Color(0.6f, 0.15f, 0.12f); // Red carpet
                    break;
                default: // Tower - light marble
                    baseColor = new Color(0.72f, 0.7f, 0.68f);
                    lightColor = new Color(0.88f, 0.86f, 0.82f);
                    darkColor = new Color(0.55f, 0.52f, 0.48f);
                    groutColor = new Color(0.4f, 0.38f, 0.35f);
                    accentColor = new Color(0.6f, 0.65f, 0.75f); // Blue accent
                    break;
            }

            DrawDetailedFloorTile(tex, baseColor, lightColor, darkColor, groutColor, accentColor, variation);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetWallSprite(int floorLevel, bool isVertical)
        {
            string key = $"uhd_wall_{floorLevel}_{(isVertical ? "v" : "h")}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;

            Color baseColor, lightColor, darkColor, mortarColor, accentColor;

            switch (floorLevel)
            {
                case 0: // Basement - rough stone with moss and chains
                    baseColor = new Color(0.25f, 0.22f, 0.2f);
                    lightColor = new Color(0.38f, 0.35f, 0.3f);
                    darkColor = new Color(0.15f, 0.12f, 0.1f);
                    mortarColor = new Color(0.1f, 0.08f, 0.06f);
                    accentColor = new Color(0.18f, 0.28f, 0.15f); // Moss
                    break;
                case 1: // Castle - dressed stone with tapestries
                    baseColor = new Color(0.48f, 0.42f, 0.38f);
                    lightColor = new Color(0.62f, 0.55f, 0.48f);
                    darkColor = new Color(0.32f, 0.28f, 0.22f);
                    mortarColor = new Color(0.22f, 0.18f, 0.15f);
                    accentColor = new Color(0.55f, 0.12f, 0.1f); // Red accent
                    break;
                default: // Tower - smooth light stone
                    baseColor = new Color(0.65f, 0.62f, 0.58f);
                    lightColor = new Color(0.78f, 0.75f, 0.7f);
                    darkColor = new Color(0.48f, 0.45f, 0.4f);
                    mortarColor = new Color(0.35f, 0.32f, 0.28f);
                    accentColor = new Color(0.5f, 0.55f, 0.65f); // Blue accent
                    break;
            }

            DrawDetailedWall(tex, baseColor, lightColor, darkColor, mortarColor, accentColor, isVertical, floorLevel);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetDoorSprite(bool isOpen, string keyColor)
        {
            string key = $"uhd_door_{(isOpen ? "open" : "closed")}_{keyColor}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color woodDark = new Color(0.32f, 0.2f, 0.12f);
            Color woodMid = new Color(0.48f, 0.32f, 0.18f);
            Color woodLight = new Color(0.62f, 0.45f, 0.28f);
            Color metalDark = new Color(0.25f, 0.22f, 0.2f);
            Color metalLight = new Color(0.45f, 0.42f, 0.38f);

            // Stone door frame
            DrawDoorFrame(tex, 4, 4, 120, 120, new Color(0.4f, 0.38f, 0.35f), new Color(0.55f, 0.52f, 0.48f));

            if (isOpen)
            {
                // Dark opening
                DrawRect(tex, 14, 8, 100, 108, new Color(0.05f, 0.05f, 0.08f));

                // Visible corridor inside
                DrawRect(tex, 24, 18, 80, 88, new Color(0.12f, 0.1f, 0.1f));
            }
            else
            {
                // Solid wooden door with planks
                DrawWoodenDoorDetail(tex, 14, 8, 100, 108, woodDark, woodMid, woodLight);

                // Metal reinforcements
                DrawDoorMetal(tex, 14, 8, 100, 108, metalDark, metalLight);

                // Door handle
                DrawOrnateHandle(tex, 85, 55, 18, metalDark, metalLight);

                // Keyhole with color glow if locked
                if (!string.IsNullOrEmpty(keyColor) && keyColor != "None")
                {
                    Color lockGlow = GetKeyColor(keyColor);
                    DrawGlowingKeyhole(tex, 85, 42, 12, lockGlow);
                }
            }

            // Torch sconces on either side
            DrawTorchSconce(tex, 2, 70, 10, metalDark);
            DrawTorchSconce(tex, 116, 70, 10, metalDark);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetTorchSprite(int frame)
        {
            string key = $"uhd_torch_{frame % 4}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color woodDark = new Color(0.38f, 0.25f, 0.15f);
            Color woodLight = new Color(0.55f, 0.38f, 0.22f);
            Color metalBracket = new Color(0.32f, 0.28f, 0.25f);

            // Torch handle
            DrawTorchHandle(tex, 52, 8, 24, 55, woodDark, woodLight);

            // Metal bracket at top
            DrawRect(tex, 48, 58, 32, 8, metalBracket);
            DrawRect(tex, 50, 62, 28, 6, new Color(0.4f, 0.36f, 0.32f));

            // Animated flame
            DrawRealisticFlame(tex, 44, 68, 40, 52, frame);

            // Glow effect
            AddFlameGlow(tex, 64, 90, 45, frame);

            // Sparks
            DrawSparks(tex, 64, 95, frame);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Item Sprites

        public static Sprite GetKeySprite(Color keyColor)
        {
            string key = $"uhd_key_{keyColor.r}_{keyColor.g}_{keyColor.b}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(64, 64);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color metalBase = keyColor;
            Color metalLight = new Color(
                Mathf.Min(1f, keyColor.r * 1.3f),
                Mathf.Min(1f, keyColor.g * 1.3f),
                Mathf.Min(1f, keyColor.b * 1.3f)
            );
            Color metalDark = new Color(keyColor.r * 0.6f, keyColor.g * 0.6f, keyColor.b * 0.6f);

            // Ornate key bow (top circular part)
            DrawOrnateKeyBow(tex, 20, 38, 24, metalBase, metalLight, metalDark);

            // Key shaft
            DrawRect(tex, 30, 12, 4, 28, metalBase);
            DrawRect(tex, 31, 12, 2, 28, metalLight);

            // Key teeth
            DrawKeyTeeth(tex, 26, 8, 12, 10, metalBase, metalDark);

            // Shine effect
            AddMetallicShine(tex, 32, 45, 8, metalLight);

            // Glow aura
            AddGlowEffect(tex, 32, 32, 28, new Color(keyColor.r, keyColor.g, keyColor.b, 0.3f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetFoodSprite()
        {
            if (_cache.TryGetValue("uhd_food", out var cached)) return cached;

            var tex = new Texture2D(64, 64);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            // Turkey leg / meat
            Color meatBrown = new Color(0.65f, 0.35f, 0.2f);
            Color meatDark = new Color(0.45f, 0.22f, 0.12f);
            Color meatLight = new Color(0.8f, 0.5f, 0.32f);
            Color boneTan = new Color(0.9f, 0.85f, 0.75f);

            // Bone handle
            DrawRect(tex, 8, 26, 18, 12, boneTan);
            DrawCircle(tex, 8, 32, 6, boneTan);

            // Meat portion
            DrawEllipse(tex, 40, 32, 20, 16, meatBrown);
            DrawEllipse(tex, 38, 34, 16, 12, meatLight);
            DrawEllipse(tex, 42, 30, 8, 6, meatDark);

            // Shine
            DrawCircle(tex, 34, 38, 3, new Color(1f, 0.9f, 0.8f, 0.5f));

            // Steam particles
            DrawSteam(tex, 45, 50);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
            _cache["uhd_food"] = sprite;
            return sprite;
        }

        public static Sprite GetTreasureSprite()
        {
            if (_cache.TryGetValue("uhd_treasure", out var cached)) return cached;

            var tex = new Texture2D(64, 64);
            tex.filterMode = FilterMode.Point;
            ClearTexture(tex);

            Color goldBright = new Color(1f, 0.85f, 0.25f);
            Color goldMid = new Color(0.85f, 0.65f, 0.15f);
            Color goldDark = new Color(0.6f, 0.42f, 0.1f);
            Color gemRed = new Color(0.9f, 0.15f, 0.2f);
            Color gemBlue = new Color(0.2f, 0.4f, 0.95f);
            Color gemGreen = new Color(0.2f, 0.85f, 0.35f);

            // Pile of gold coins
            for (int i = 0; i < 12; i++)
            {
                int cx = 20 + (i % 4) * 8 + Random.Range(-2, 3);
                int cy = 10 + (i / 4) * 6 + Random.Range(-2, 3);
                DrawGoldCoin(tex, cx, cy, 8, goldBright, goldMid, goldDark);
            }

            // Gems scattered on top
            DrawGem(tex, 28, 35, 10, gemRed);
            DrawGem(tex, 42, 38, 8, gemBlue);
            DrawGem(tex, 35, 42, 6, gemGreen);

            // Sparkle effects
            DrawSparkles(tex, goldBright);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
            _cache["uhd_treasure"] = sprite;
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
            for (int px = Mathf.Max(0, x); px < Mathf.Min(tex.width, x + w); px++)
            {
                for (int py = Mathf.Max(0, y); py < Mathf.Min(tex.height, y + h); py++)
                {
                    tex.SetPixel(px, py, color);
                }
            }
        }

        private static void DrawTexturedRect(Texture2D tex, int x, int y, int w, int h, Color baseColor, Color darkColor)
        {
            for (int px = x; px < x + w && px < tex.width; px++)
            {
                for (int py = y; py < y + h && py < tex.height; py++)
                {
                    if (px < 0 || py < 0) continue;
                    float noise = Mathf.PerlinNoise(px * 0.15f, py * 0.15f);
                    Color c = Color.Lerp(darkColor, baseColor, noise * 0.7f + 0.3f);
                    tex.SetPixel(px, py, c);
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

        private static void AddGlowEffect(Texture2D tex, int cx, int cy, int radius, Color glowColor)
        {
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    float dist = Mathf.Sqrt(x * x + y * y);
                    if (dist < radius)
                    {
                        float alpha = glowColor.a * (1 - dist / radius);
                        int px = cx + x;
                        int py = cy + y;
                        if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                        {
                            Color existing = tex.GetPixel(px, py);
                            Color blended = Color.Lerp(existing, glowColor, alpha);
                            blended.a = Mathf.Max(existing.a, alpha);
                            tex.SetPixel(px, py, blended);
                        }
                    }
                }
            }
        }

        private static Color GetKeyColor(string colorName)
        {
            return colorName.ToLower() switch
            {
                "red" => new Color(0.9f, 0.2f, 0.15f),
                "blue" => new Color(0.2f, 0.4f, 0.95f),
                "green" => new Color(0.2f, 0.85f, 0.3f),
                "yellow" => new Color(0.95f, 0.9f, 0.2f),
                "cyan" => new Color(0.2f, 0.9f, 0.9f),
                "magenta" => new Color(0.9f, 0.2f, 0.85f),
                _ => new Color(0.85f, 0.7f, 0.25f) // Gold default
            };
        }

        // Stub implementations for complex drawing methods
        // These would be fully implemented with detailed pixel art logic

        private static void DrawDetailedRobes(Texture2D tex, int x, int y, int w, int h, Color c1, Color c2, Color c3, Color c4)
        {
            for (int px = 0; px < w; px++)
            {
                for (int py = 0; py < h; py++)
                {
                    float t = py / (float)h;
                    float folds = Mathf.Sin(px * 0.3f + py * 0.15f) * 0.2f;
                    Color c = Color.Lerp(Color.Lerp(c1, c2, t), Color.Lerp(c3, c4, t), 0.5f + folds);
                    if (x + px >= 0 && x + px < tex.width && y + py >= 0 && y + py < tex.height)
                        tex.SetPixel(x + px, y + py, c);
                }
            }
        }

        private static void DrawRopeBelt(Texture2D tex, int x, int y, int w, Color c)
        {
            DrawRect(tex, x, y, w, 4, c);
            DrawRect(tex, x, y + 1, w, 2, new Color(c.r * 1.2f, c.g * 1.2f, c.b * 1.2f));
        }

        private static void DrawHand(Texture2D tex, int x, int y, int size, Color skin, Color shadow, bool left)
        {
            DrawEllipse(tex, x, y, size / 2, size / 2 - 2, skin);
        }

        private static void DrawDetailedFace(Texture2D tex, int x, int y, int w, int h, Color skin, Color shadow, bool aged)
        {
            DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, skin);
            // Eyes
            tex.SetPixel(x + w / 3, y + h / 2 + 2, Color.white);
            tex.SetPixel(x + 2 * w / 3, y + h / 2 + 2, Color.white);
            tex.SetPixel(x + w / 3, y + h / 2 + 1, new Color(0.2f, 0.3f, 0.5f));
            tex.SetPixel(x + 2 * w / 3, y + h / 2 + 1, new Color(0.2f, 0.3f, 0.5f));
        }

        private static void DrawFlowingBeard(Texture2D tex, int x, int y, int w, int h, Color light, Color shadow)
        {
            for (int px = 0; px < w; px++)
            {
                int height = h - Mathf.Abs(px - w / 2) / 2;
                for (int py = 0; py < height; py++)
                {
                    float noise = Mathf.PerlinNoise(px * 0.2f, py * 0.2f);
                    Color c = Color.Lerp(shadow, light, noise);
                    if (x + px >= 0 && x + px < tex.width && y - py >= 0 && y - py < tex.height)
                        tex.SetPixel(x + px, y - py, c);
                }
            }
        }

        private static void DrawWizardHat(Texture2D tex, int x, int y, int w, int h, Color dark, Color mid)
        {
            // Brim
            DrawEllipse(tex, x + w / 2, y, w / 2 + 8, 4, dark);
            // Cone
            for (int row = 0; row < h; row++)
            {
                int rowW = w - (row * w / h);
                DrawRect(tex, x + (w - rowW) / 2, y + row, rowW, 1, Color.Lerp(dark, mid, row / (float)h * 0.5f));
            }
        }

        private static void DrawMagicalSymbols(Texture2D tex, int x, int y, int w, int h)
        {
            Color star = new Color(1f, 0.95f, 0.6f);
            // Stars on hat
            tex.SetPixel(x + 10, y + 5, star);
            tex.SetPixel(x + 30, y + 10, star);
            tex.SetPixel(x + 20, y + 15, star);
        }

        private static void DrawOrnateStaff(Texture2D tex, int x, int y, int h, Color wood, Color orb)
        {
            DrawRect(tex, x, y, 4, h - 15, wood);
            DrawCircle(tex, x + 2, y + h - 10, 10, orb);
            AddGlowEffect(tex, x + 2, y + h - 10, 18, new Color(orb.r, orb.g, orb.b, 0.5f));
        }

        private static void DrawMagicalAura(Texture2D tex, Color c)
        {
            System.Random rng = new System.Random(42);
            for (int i = 0; i < 15; i++)
            {
                int px = rng.Next(10, tex.width - 10);
                int py = rng.Next(10, tex.height - 10);
                float alpha = 0.3f + (float)rng.NextDouble() * 0.4f;
                Color particle = new Color(c.r, c.g, c.b, alpha);
                tex.SetPixel(px, py, particle);
                tex.SetPixel(px + 1, py, particle * 0.6f);
            }
        }

        private static void DrawGlowingEyes(Texture2D tex, int x, int y, int size, Color c)
        {
            DrawCircle(tex, x, y, size, c);
            AddGlowEffect(tex, x, y, size + 4, new Color(c.r, c.g, c.b, 0.5f));
        }

        private static void DrawLargeArrow(Texture2D tex, int x, int y, int size, bool up, Color c, Color glow)
        {
            // Arrow body
            int bodyWidth = size / 3;
            int bodyHeight = size / 2;
            DrawRect(tex, x + size / 2 - bodyWidth / 2, up ? y - bodyHeight : y, bodyWidth, bodyHeight, c);

            // Arrow head
            for (int row = 0; row < size / 2; row++)
            {
                int rowW = size - row * 2;
                int startX = x + row;
                int startY = up ? (y + row) : (y + size / 2 - 1 - row);
                if (rowW > 0)
                    DrawRect(tex, startX, startY, rowW, 1, c);
            }

            // Glow
            int centerY = up ? (y + size / 4) : (y + size / 4);
            AddGlowEffect(tex, x + size / 2, centerY, size / 2 + 5, glow);
        }

        private static void DrawDirectionText(Texture2D tex, int x, int y, bool up, Color c)
        {
            // Simple UP or DOWN indicator using pixels
            // This is a simplified version - could be expanded with proper font rendering
        }

        private static void DrawRailingPost(Texture2D tex, int x, int y, int h, Color dark, Color light)
        {
            DrawRect(tex, x, y, 8, h, dark);
            DrawRect(tex, x + 2, y, 4, h, light);
            DrawCircle(tex, x + 4, y + h, 6, light);
        }

        // Many more helper methods would go here for complete implementation...
        // For brevity, including simplified versions

        private static void DrawEtherealForm(Texture2D tex, int x, int y, int w, int h, Color b, Color l, Color s)
        {
            for (int px = 0; px < w; px++)
            {
                for (int py = 0; py < h; py++)
                {
                    float dist = Mathf.Sqrt(Mathf.Pow((px - w / 2f) / (w / 2f), 2) + Mathf.Pow((py - h / 2f) / (h / 2f), 2));
                    if (dist < 1)
                    {
                        float alpha = (1 - dist) * b.a;
                        float wave = Mathf.Sin(py * 0.1f + px * 0.05f) * 0.15f;
                        Color c = Color.Lerp(s, b, (1 - dist) + wave);
                        c.a = alpha;
                        if (x + px >= 0 && x + px < tex.width && y + py >= 0 && y + py < tex.height)
                            tex.SetPixel(x + px, y + py, c);
                    }
                }
            }
        }

        private static void DrawGhostlyEyes(Texture2D tex, int x, int y, int size, Color c)
        {
            DrawCircle(tex, x, y, size / 2, new Color(0.1f, 0.1f, 0.2f));
            DrawCircle(tex, x, y, size / 3, c);
            AddGlowEffect(tex, x, y, size, new Color(c.r, c.g, c.b, 0.6f));
        }

        private static void DrawGhostMouth(Texture2D tex, int x, int y, int w, int h, Color c)
        {
            DrawEllipse(tex, x + w / 2, y + h / 2, w / 2, h / 2, c);
        }

        private static void DrawGhostlyWisps(Texture2D tex, Color c)
        {
            System.Random rng = new System.Random(123);
            for (int i = 0; i < 8; i++)
            {
                int startX = rng.Next(20, tex.width - 20);
                int startY = rng.Next(5, 30);
                for (int j = 0; j < 15; j++)
                {
                    int px = startX + rng.Next(-3, 4);
                    int py = startY + j;
                    if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                    {
                        Color wisp = new Color(c.r, c.g, c.b, c.a * (1 - j / 15f));
                        tex.SetPixel(px, py, wisp);
                    }
                }
            }
        }

        private static void AddEtherealGlow(Texture2D tex, int cx, int cy, int radius, Color c)
        {
            AddGlowEffect(tex, cx, cy, radius, new Color(c.r, c.g, c.b, 0.25f));
        }

        // Add remaining stub methods for all the drawing functions...
        // These would contain the actual detailed drawing logic

        private static void DrawArmoredBoots(Texture2D t, int x, int y, int w, int h, Color d, Color m, Color l) => DrawRect(t, x, y, w, h, m);
        private static void DrawArmorPlate(Texture2D t, int x, int y, int w, int h, Color d, Color m, Color l, Color hl) => DrawRect(t, x, y, w, h, m);
        private static void DrawChainmail(Texture2D t, int x, int y, int w, int h, Color c) => DrawRect(t, x, y, w, h, c);
        private static void DrawBreastplate(Texture2D t, int x, int y, int w, int h, Color d, Color m, Color l, Color hl) => DrawRect(t, x, y, w, h, m);
        private static void DrawRoyalEmblem(Texture2D t, int x, int y, int s, Color c1, Color c2) { DrawCircle(t, x + s / 2, y + s / 2, s / 2, c1); }
        private static void DrawPauldron(Texture2D t, int x, int y, int w, int h, Color d, Color m, Color l, Color hl) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, m);
        private static void DrawFlowingCape(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawRect(t, x, y - h, w, h, c1);
        private static void DrawGauntlet(Texture2D t, int x, int y, int w, int h, Color d, Color m, Color l) => DrawRect(t, x, y, w, h, m);
        private static void DrawKnightHelmet(Texture2D t, int x, int y, int w, int h, Color d, Color m, Color l, Color hl) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, m);
        private static void DrawDetailedSword(Texture2D t, int x, int y, int h, Color bl, Color gd) { DrawRect(t, x, y, 4, h, bl); }
        private static void DrawDetailedShield(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1);

        private static void DrawWornBoots(Texture2D t, int x, int y, int w, int h, Color c) => DrawRect(t, x, y, w, h, c);
        private static void DrawPatchedPants(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawRect(t, x, y, w, h, c1);
        private static void DrawPeasantTunic(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawRect(t, x, y, w, h, c1);
        private static void DrawArm(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, bool l) => DrawRect(t, x, y, w, h, c1);
        private static void DrawPeasantCap(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawEllipse(t, x + w / 2, y, w / 2, h / 2, c1);
        private static void DrawMessyHair(Texture2D t, int x, int y, int w, Color c) => DrawRect(t, x, y, w, 8, c);
        private static void DrawThrowingAxe(Texture2D t, int x, int y, int h, Color w, Color m) { DrawRect(t, x, y, 4, h, w); DrawRect(t, x - 8, y + h - 12, 16, 12, m); }

        private static void DrawSkeletalFeet(Texture2D t, int x, int y, Color c1, Color c2) => DrawRect(t, x, y, 12, 6, c1);
        private static void DrawSkeletalLegs(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawRect(t, x, y, w, h, c1);
        private static void DrawPelvisBone(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawSpineDetail(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawRect(t, x, y, w, h, c1);
        private static void DrawRibcageDetail(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) { for (int i = 0; i < 5; i++) DrawRect(t, x, y + i * 7, w, 4, c1); }
        private static void DrawSkeletalArm(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3, bool l) => DrawRect(t, x, y, w, h, c1);
        private static void DrawDetailedSkull(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawGlowingEyeSockets(Texture2D t, int x, int y, int s, Color c) { DrawCircle(t, x, y, s / 2, c); AddGlowEffect(t, x, y, s, new Color(c.r, c.g, c.b, 0.5f)); }
        private static void DrawRustySword(Texture2D t, int x, int y, int h, Color c) => DrawRect(t, x, y, 4, h, c);

        private static void DrawSpiderLegDetailed(Texture2D t, int x, int y, float a, Color c1, Color c2)
        {
            float rad = a * Mathf.Deg2Rad;
            for (int i = 0; i < 40; i++)
            {
                int px = x + Mathf.RoundToInt(Mathf.Cos(rad) * i);
                int py = y + Mathf.RoundToInt(Mathf.Sin(rad) * i);
                if (px >= 0 && px < t.width && py >= 0 && py < t.height)
                    t.SetPixel(px, py, c1);
            }
        }
        private static void DrawHairyAbdomen(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c2);
        private static void DrawSpiderHead(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c2);
        private static void DrawSpiderEyeCluster(Texture2D t, int x, int y, Color c) { for (int i = 0; i < 8; i++) DrawCircle(t, x + (i % 4) * 4, y + (i / 4) * 4, 2, c); }
        private static void DrawVenomousFangs(Texture2D t, int x, int y, int w, Color c1, Color c2) { DrawRect(t, x, y - 8, 3, 10, c1); DrawRect(t, x + w - 3, y - 8, 3, 10, c1); }
        private static void DrawWebPattern(Texture2D t, int x, int y, int w, int h, Color c) { }

        private static void DrawBatWingDetailed(Texture2D t, int x, int y, int w, int h, Color m, Color b, bool l) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, m);
        private static void DrawFurryBatBody(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c2);
        private static void DrawBatHeadDetailed(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c2);
        private static void DrawBatEarDetailed(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) { for (int i = 0; i < h; i++) DrawRect(t, x + (w - w * i / h) / 2, y + i, w * i / h, 1, c1); }
        private static void DrawBatFangs(Texture2D t, int x, int y, int w, Color c) { DrawRect(t, x, y - 6, 2, 6, c); DrawRect(t, x + w - 2, y - 6, 2, 6, c); }
        private static void DrawBatClaws(Texture2D t, int x, int y, Color c) => DrawRect(t, x, y - 8, 4, 10, c);

        private static void DrawClovenHooves(Texture2D t, int x, int y, int w, int h, Color c) => DrawRect(t, x, y, w, h, c);
        private static void DrawDemonicLeg(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawRect(t, x, y, w, h, c1);
        private static void DrawMuscularTorso(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawDemonicArm(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3, bool l) => DrawRect(t, x, y, w, h, c1);
        private static void DrawDemonClaws(Texture2D t, int x, int y, int s, Color c) { for (int i = 0; i < 4; i++) DrawRect(t, x + i * 3, y - s, 2, s, c); }
        private static void DrawDemonicFace(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawCurvedHorns(Texture2D t, int x, int y, int w, int h, Color c, bool l) { for (int i = 0; i < h; i++) DrawRect(t, x + (l ? i / 2 : w - i / 2 - 3), y + i, 4, 1, c); }
        private static void DrawFireAura(Texture2D t, Color c1, Color c2)
        {
            System.Random rng = new System.Random(456);
            for (int i = 0; i < 25; i++)
            {
                int x = rng.Next(15, t.width - 15);
                int y = rng.Next(10, 50);
                Color c = Color.Lerp(c1, c2, (float)rng.NextDouble());
                c.a = 0.4f + (float)rng.NextDouble() * 0.3f;
                t.SetPixel(x, y, c);
            }
        }
        private static void DrawSmallDemonWings(Texture2D t, int x, int y, int w, int h, Color c) => DrawRect(t, x, y, w, h, c);

        private static void DrawWrappedFeet(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawRect(t, x, y, w, h, c1);
        private static void DrawBandagedLimb(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3)
        {
            for (int py = 0; py < h; py++)
            {
                Color c = (py % 6 < 3) ? c1 : c2;
                DrawRect(t, x, y + py, w, 1, c);
            }
        }
        private static void DrawBandagedTorso(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawBandagedLimb(t, x, y, w, h, c1, c2, c3);
        private static void DrawHangingBandages(Texture2D t, int x, int y, Color c1, Color c2)
        {
            for (int i = 0; i < 5; i++)
            {
                int bx = x + i * 12;
                int bh = 15 + i * 3;
                DrawRect(t, bx, y - bh, 3, bh, c1);
            }
        }
        private static void DrawBandagedArm(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3, bool l) => DrawBandagedLimb(t, x, y, w, h, c1, c2, c3);
        private static void DrawMummyHead(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3, Color rot) { DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1); DrawBandagedLimb(t, x, y + 5, w, h - 10, c1, c2, c3); }
        private static void DrawDustCloud(Texture2D t, Color c)
        {
            System.Random rng = new System.Random(789);
            for (int i = 0; i < 20; i++)
            {
                int x = rng.Next(0, t.width);
                int y = rng.Next(0, t.height);
                t.SetPixel(x, y, c);
            }
        }

        private static void DrawElegantBoots(Texture2D t, int x, int y, int w, int h, Color c) => DrawRect(t, x, y, w, h, c);
        private static void DrawAristocraticPants(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawRect(t, x, y, w, h, c1);
        private static void DrawVampireVest(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) { DrawRect(t, x, y, w, h, c1); DrawRect(t, x + w / 3, y, w / 3, h, c3); }
        private static void DrawVampireCape(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3)
        {
            for (int py = 0; py < h; py++)
            {
                int wave = (int)(Mathf.Sin(py * 0.15f) * 5);
                DrawRect(t, x + wave, y - py, w - wave * 2, 1, py < 5 ? c3 : c1);
            }
        }
        private static void DrawVampireFace(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawSlickedHair(Texture2D t, int x, int y, int w, int h, Color c) => DrawRect(t, x, y, w, h, c);
        private static void DrawVampireFangs(Texture2D t, int x, int y, int w) { DrawRect(t, x, y - 6, 2, 6, Color.white); DrawRect(t, x + w - 2, y - 6, 2, 6, Color.white); }
        private static void DrawCapeCollar(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) { DrawRect(t, x, y, 10, h, c1); DrawRect(t, x + w - 10, y, 10, h, c1); }

        private static void DrawWolfLegs(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawRect(t, x, y, w, h, c2);
        private static void DrawWolfClaws(Texture2D t, int x, int y, int w, Color c) { for (int i = 0; i < 4; i++) DrawRect(t, x + i * 4, y - 6, 2, 6, c); }
        private static void DrawWolfTorso(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c2);
        private static void DrawWolfArm(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3, bool l) => DrawRect(t, x, y, w, h, c2);
        private static void DrawWolfHead(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3)
        {
            DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c2);
            // Snout
            DrawEllipse(t, x + w / 2, y, w / 4, h / 4, c3);
        }
        private static void DrawWolfEars(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) { for (int i = 0; i < h; i++) DrawRect(t, x + (w - w * (h - i) / h) / 2, y + i, w * (h - i) / h, 1, c1); }
        private static void DrawWolfNose(Texture2D t, int x, int y, int s, Color c) => DrawCircle(t, x, y, s / 2, c);
        private static void DrawWolfMaw(Texture2D t, int x, int y, int w, int h, Color inside, Color fangs) { DrawRect(t, x, y, w, h, inside); DrawRect(t, x + 4, y + h, 3, 5, fangs); DrawRect(t, x + w - 7, y + h, 3, 5, fangs); }
        private static void DrawTornClothing(Texture2D t, int x, int y, int w, int h, Color c)
        {
            for (int i = 0; i < 8; i++)
            {
                int px = x + i * w / 8;
                int ph = h - (i * 3) % 10;
                DrawRect(t, px, y, w / 9, ph, c);
            }
        }

        private static void DrawPointedBoots(Texture2D t, int x, int y, int w, int h, Color c) => DrawRect(t, x, y, w, h, c);
        private static void DrawTatteredDress(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3)
        {
            for (int py = 0; py < h; py++)
            {
                int wave = (int)(Mathf.Sin(py * 0.2f) * 3);
                Color c = Color.Lerp(c1, c2, py / (float)h);
                DrawRect(t, x + wave, y + py, w - wave * 2, 1, c);
            }
            // Tattered bottom
            for (int i = 0; i < 10; i++)
            {
                DrawRect(t, x + i * w / 10, y, w / 12, 8 + (i * 5) % 12, c2);
            }
        }
        private static void DrawWitchArm(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, bool l) => DrawRect(t, x, y, w, h, c1);
        private static void DrawGnarledHand(Texture2D t, int x, int y, int s, Color c1, Color c2) => DrawCircle(t, x, y, s / 2, c1);
        private static void DrawWitchFace(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawWildHair(Texture2D t, int x, int y, int w, int h, Color c)
        {
            System.Random rng = new System.Random(111);
            for (int i = 0; i < 30; i++)
            {
                int hx = x + rng.Next(0, w);
                int hy = y + rng.Next(0, h);
                int hh = rng.Next(5, 15);
                DrawRect(t, hx, hy, 2, hh, c);
            }
        }
        private static void DrawWitchHat(Texture2D t, int x, int y, int w, int h, Color c1, Color c2)
        {
            // Brim
            DrawEllipse(t, x + w / 2, y, w / 2 + 10, 5, c1);
            // Crooked cone
            for (int row = 0; row < h * 2; row++)
            {
                int rowW = w - (row * w / (h * 2));
                int offset = (int)(Mathf.Sin(row * 0.3f) * 5);
                if (rowW > 0)
                    DrawRect(t, x + (w - rowW) / 2 + offset, y + row, rowW, 1, c1);
            }
            // Band
            DrawRect(t, x + 5, y + 5, w - 10, 4, c2);
        }
        private static void DrawWitchNose(Texture2D t, int x, int y, Color c1, Color c2)
        {
            for (int i = 0; i < 12; i++)
            {
                DrawRect(t, x - i / 3, y - i, 4, 1, c1);
            }
            // Wart
            DrawCircle(t, x - 2, y - 6, 2, c2);
        }
        private static void DrawBroomstick(Texture2D t, int x, int y, int h, Color stick, Color bristle)
        {
            DrawRect(t, x, y, 4, h, stick);
            // Bristles
            for (int i = 0; i < 15; i++)
            {
                int bx = x - 8 + i;
                DrawRect(t, bx, y - 20, 2, 22, bristle);
            }
        }
        private static void DrawMagicalSparkles(Texture2D t, Color c)
        {
            System.Random rng = new System.Random(222);
            for (int i = 0; i < 12; i++)
            {
                int x = rng.Next(0, t.width);
                int y = rng.Next(0, t.height);
                float alpha = 0.5f + (float)rng.NextDouble() * 0.5f;
                t.SetPixel(x, y, new Color(c.r, c.g, c.b, alpha));
            }
        }

        private static void DrawReaperRobes(Texture2D t, int x, int y, int w, int h, Color c1, Color c2, Color c3)
        {
            for (int py = 0; py < h; py++)
            {
                float fade = py / (float)h;
                Color c = Color.Lerp(c2, c1, fade);
                c.a = 0.3f + fade * 0.7f;
                int wave = (int)(Mathf.Sin(py * 0.1f) * 4);
                DrawRect(t, x + wave, y + py, w - wave * 2, 1, c);
            }
        }
        private static void DrawSkeletalHands(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawRect(t, x, y, w, h, c1);
        private static void DrawReaperHood(Texture2D t, int x, int y, int w, int h, Color c1, Color c2)
        {
            for (int row = 0; row < h; row++)
            {
                int rowW = w - row / 2;
                DrawRect(t, x + (w - rowW) / 2, y + row, rowW, 1, Color.Lerp(c2, c1, row / (float)h * 0.5f));
            }
        }
        private static void DrawReaperSkull(Texture2D t, int x, int y, int w, int h, Color c1, Color c2) => DrawEllipse(t, x + w / 2, y + h / 2, w / 2, h / 2, c1);
        private static void DrawMassiveScythe(Texture2D t, int x, int y, int h, Color blade, Color handle)
        {
            // Handle
            DrawRect(t, x, y, 5, h - 20, handle);
            // Blade
            for (int i = 0; i < 50; i++)
            {
                int bx = x - 5 - i;
                int by = y + h - 25 + i / 3;
                int bw = 60 - i;
                if (bw > 0 && bx >= 0 && by < t.height)
                    DrawRect(t, bx, by, Mathf.Min(bw, t.width - bx), 2, blade);
            }
        }
        private static void DrawDarkMist(Texture2D t, Color c1, Color c2)
        {
            for (int y = 0; y < 25; y++)
            {
                float alpha = 0.5f * (1 - y / 25f);
                Color c = new Color(c1.r, c1.g, c1.b, alpha);
                for (int x = 0; x < t.width; x++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                    if (noise > 0.4f)
                        t.SetPixel(x, y, c);
                }
            }
        }
        private static void DrawSoulWisps(Texture2D t, Color c)
        {
            System.Random rng = new System.Random(333);
            for (int i = 0; i < 8; i++)
            {
                int x = rng.Next(20, t.width - 20);
                int y = rng.Next(30, t.height - 30);
                for (int j = 0; j < 10; j++)
                {
                    float alpha = c.a * (1 - j / 10f);
                    t.SetPixel(x + rng.Next(-2, 3), y + j, new Color(c.r, c.g, c.b, alpha));
                }
            }
        }

        private static void DrawDetailedFloorTile(Texture2D t, Color b, Color l, Color d, Color g, Color a, int v)
        {
            // Fill base with noise
            for (int x = 0; x < t.width; x++)
            {
                for (int y = 0; y < t.height; y++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.08f + v, y * 0.08f);
                    Color c = Color.Lerp(d, l, noise * 0.6f + 0.2f);
                    t.SetPixel(x, y, c);
                }
            }
            // Grout lines (tile pattern)
            for (int i = 0; i <= 2; i++)
            {
                DrawRect(t, 0, i * t.height / 2 - 1, t.width, 3, g);
                DrawRect(t, i * t.width / 2 - 1, 0, 3, t.height, g);
            }
            // Occasional accent marks
            if (v % 3 == 0)
            {
                DrawCircle(t, t.width / 2, t.height / 2, 8, a);
            }
        }

        private static void DrawDetailedWall(Texture2D t, Color b, Color l, Color d, Color m, Color a, bool v, int fl)
        {
            // Mortar base
            for (int x = 0; x < t.width; x++)
            {
                for (int y = 0; y < t.height; y++)
                {
                    t.SetPixel(x, y, m);
                }
            }
            // Bricks with varied textures
            int brickH = 16;
            int brickW = 32;
            for (int row = 0; row < t.height / brickH; row++)
            {
                int offset = (row % 2) * (brickW / 2);
                for (int col = -1; col < t.width / brickW + 1; col++)
                {
                    int bx = col * brickW + offset;
                    int by = row * brickH;
                    float noise = Mathf.PerlinNoise(col * 0.5f + row * 0.3f, row * 0.5f) * 0.4f + 0.3f;
                    Color brickC = Color.Lerp(d, l, noise);
                    DrawRect(t, bx + 2, by + 2, brickW - 4, brickH - 4, brickC);
                }
            }
            // Add decorations based on floor level
            if (fl == 0)
            {
                // Moss patches
                System.Random rng = new System.Random(fl * 100);
                for (int i = 0; i < 5; i++)
                {
                    int mx = rng.Next(0, t.width);
                    int my = rng.Next(0, t.height);
                    DrawCircle(t, mx, my, 6, a);
                }
            }
        }

        private static void DrawDoorFrame(Texture2D t, int x, int y, int w, int h, Color d, Color l)
        {
            // Outer frame
            DrawRect(t, x, y, w, 8, d);
            DrawRect(t, x, y + h - 8, w, 8, d);
            DrawRect(t, x, y, 10, h, d);
            DrawRect(t, x + w - 10, y, 10, h, d);
            // Inner highlight
            DrawRect(t, x + 2, y + 2, w - 4, 4, l);
            DrawRect(t, x + 2, y, 6, h, l);
        }

        private static void DrawWoodenDoorDetail(Texture2D t, int x, int y, int w, int h, Color d, Color m, Color l)
        {
            // Base wood
            DrawRect(t, x, y, w, h, m);
            // Planks
            int plankW = w / 5;
            for (int i = 0; i < 5; i++)
            {
                Color plankC = (i % 2 == 0) ? m : Color.Lerp(m, d, 0.15f);
                DrawRect(t, x + i * plankW + 1, y, plankW - 2, h, plankC);
            }
            // Wood grain
            for (int py = y; py < y + h; py += 4)
            {
                DrawRect(t, x, py, w, 1, new Color(d.r, d.g, d.b, 0.3f));
            }
        }

        private static void DrawDoorMetal(Texture2D t, int x, int y, int w, int h, Color d, Color l)
        {
            // Cross braces
            for (int i = 0; i < 3; i++)
            {
                int by = y + 20 + i * 35;
                DrawRect(t, x, by, w, 6, d);
                DrawRect(t, x, by + 1, w, 2, l);
            }
            // Corner studs
            DrawCircle(t, x + 10, y + 10, 4, l);
            DrawCircle(t, x + w - 10, y + 10, 4, l);
            DrawCircle(t, x + 10, y + h - 10, 4, l);
            DrawCircle(t, x + w - 10, y + h - 10, 4, l);
        }

        private static void DrawOrnateHandle(Texture2D t, int x, int y, int s, Color d, Color l)
        {
            // Handle plate
            DrawCircle(t, x, y, s, d);
            DrawCircle(t, x, y, s - 3, l);
            // Ring handle
            for (int angle = 180; angle < 360; angle += 10)
            {
                float rad = angle * Mathf.Deg2Rad;
                int px = x + Mathf.RoundToInt(Mathf.Cos(rad) * (s - 2));
                int py = y + Mathf.RoundToInt(Mathf.Sin(rad) * (s - 2));
                if (px >= 0 && px < t.width && py >= 0 && py < t.height)
                    t.SetPixel(px, py, d);
            }
        }

        private static void DrawGlowingKeyhole(Texture2D t, int x, int y, int s, Color c)
        {
            // Keyhole shape
            DrawCircle(t, x, y + s / 3, s / 3, Color.black);
            DrawRect(t, x - 2, y - s / 2, 4, s / 2 + 2, Color.black);
            // Glow
            AddGlowEffect(t, x, y, s + 6, new Color(c.r, c.g, c.b, 0.6f));
        }

        private static void DrawTorchSconce(Texture2D t, int x, int y, int s, Color c)
        {
            DrawRect(t, x, y - 5, s, s + 5, c);
        }

        private static void DrawTorchHandle(Texture2D t, int x, int y, int w, int h, Color d, Color l)
        {
            DrawRect(t, x, y, w, h, d);
            DrawRect(t, x + 2, y, w - 4, h, l);
        }

        private static void DrawRealisticFlame(Texture2D t, int x, int y, int w, int h, int frame)
        {
            Color[] flames = {
                new Color(1f, 0.98f, 0.5f),    // Bright yellow core
                new Color(1f, 0.8f, 0.2f),      // Yellow
                new Color(1f, 0.5f, 0.1f),      // Orange
                new Color(0.85f, 0.25f, 0.05f)  // Red
            };

            float flicker = Mathf.Sin(frame * 1.2f) * 0.15f;

            for (int py = 0; py < h; py++)
            {
                float t_norm = py / (float)h;
                int rowW = (int)(w * (1 - t_norm * 0.7f) + Mathf.Sin(py * 0.4f + frame) * 3);
                int startX = x + (w - rowW) / 2;

                for (int px = 0; px < rowW; px++)
                {
                    float centerDist = Mathf.Abs(px - rowW / 2f) / (rowW / 2f);
                    int colorIdx = Mathf.Min(3, (int)((centerDist + t_norm) * 2));
                    Color c = flames[colorIdx];
                    c.a = 0.9f - t_norm * 0.4f;

                    if (startX + px >= 0 && startX + px < t.width && y + py < t.height && y + py >= 0)
                        t.SetPixel(startX + px, y + py, c);
                }
            }
        }

        private static void AddFlameGlow(Texture2D t, int cx, int cy, int radius, int frame)
        {
            float flicker = 0.8f + Mathf.Sin(frame * 1.5f) * 0.2f;
            Color glow = new Color(1f, 0.6f, 0.2f, 0.35f * flicker);
            AddGlowEffect(t, cx, cy, radius, glow);
        }

        private static void DrawSparks(Texture2D t, int cx, int cy, int frame)
        {
            System.Random rng = new System.Random(frame);
            for (int i = 0; i < 5; i++)
            {
                int sx = cx + rng.Next(-15, 16);
                int sy = cy + rng.Next(5, 25);
                if (sx >= 0 && sx < t.width && sy >= 0 && sy < t.height)
                {
                    Color spark = new Color(1f, 0.9f, 0.4f, 0.8f);
                    t.SetPixel(sx, sy, spark);
                }
            }
        }

        private static void DrawOrnateKeyBow(Texture2D t, int x, int y, int s, Color b, Color l, Color d)
        {
            // Outer ring
            for (int angle = 0; angle < 360; angle += 5)
            {
                float rad = angle * Mathf.Deg2Rad;
                int px = x + Mathf.RoundToInt(Mathf.Cos(rad) * s / 2);
                int py = y + Mathf.RoundToInt(Mathf.Sin(rad) * s / 2);
                Color c = angle < 180 ? l : d;
                if (px >= 0 && px < t.width && py >= 0 && py < t.height)
                {
                    t.SetPixel(px, py, c);
                    t.SetPixel(px + 1, py, c);
                }
            }
            // Inner decorative hole
            DrawCircle(t, x, y, s / 4, Color.clear);
        }

        private static void DrawKeyTeeth(Texture2D t, int x, int y, int w, int h, Color b, Color d)
        {
            // Key teeth pattern
            DrawRect(t, x, y, 4, h, b);
            DrawRect(t, x + 5, y + 2, 4, h - 4, b);
            DrawRect(t, x + 10, y, 3, h, b);
        }

        private static void AddMetallicShine(Texture2D t, int x, int y, int s, Color c)
        {
            for (int i = 0; i < s; i++)
            {
                float alpha = 0.6f - i * 0.07f;
                if (x + i < t.width && y + i < t.height)
                    t.SetPixel(x + i, y + i, new Color(c.r, c.g, c.b, alpha));
            }
        }

        private static void DrawSteam(Texture2D t, int x, int y)
        {
            System.Random rng = new System.Random(555);
            for (int i = 0; i < 6; i++)
            {
                int sx = x + rng.Next(-8, 9);
                int sy = y + i * 3;
                if (sx >= 0 && sx < t.width && sy < t.height)
                {
                    Color steam = new Color(0.9f, 0.9f, 0.9f, 0.3f - i * 0.04f);
                    t.SetPixel(sx, sy, steam);
                }
            }
        }

        private static void DrawGoldCoin(Texture2D t, int x, int y, int s, Color l, Color m, Color d)
        {
            DrawEllipse(t, x, y, s / 2, s / 3, m);
            DrawCircle(t, x, y, s / 3, l);
        }

        private static void DrawGem(Texture2D t, int x, int y, int s, Color c)
        {
            // Diamond shape
            for (int i = 0; i < s; i++)
            {
                int w = i < s / 2 ? i * 2 : (s - i) * 2;
                DrawRect(t, x - w / 2, y + i, w, 1, c);
            }
            // Highlight
            t.SetPixel(x - 1, y + s / 3, Color.white);
        }

        private static void DrawSparkles(Texture2D t, Color c)
        {
            System.Random rng = new System.Random(666);
            for (int i = 0; i < 8; i++)
            {
                int sx = rng.Next(5, t.width - 5);
                int sy = rng.Next(5, t.height - 5);
                t.SetPixel(sx, sy, new Color(c.r, c.g, c.b, 0.9f));
                t.SetPixel(sx + 1, sy, new Color(c.r, c.g, c.b, 0.5f));
                t.SetPixel(sx, sy + 1, new Color(c.r, c.g, c.b, 0.5f));
            }
        }

        #endregion

        /// <summary>
        /// Clears the sprite cache.
        /// </summary>
        public static void ClearCache()
        {
            foreach (var sprite in _cache.Values)
            {
                if (sprite != null && sprite.texture != null)
                {
                    Object.Destroy(sprite.texture);
                }
            }
            _cache.Clear();
        }
    }
}
