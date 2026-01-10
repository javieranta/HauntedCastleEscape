using UnityEngine;
using System.Collections.Generic;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Generates pixel art style sprites at runtime for the game.
    /// Creates retro-style 16x16 and 32x32 sprites for characters, enemies, items, and environment.
    /// </summary>
    public static class PixelArtGenerator
    {
        private static Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();

        #region Character Sprites

        public static Sprite GetKnightSprite()
        {
            if (_spriteCache.TryGetValue("knight", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Knight - silver armor with red plume
            Color armor = new Color(0.7f, 0.7f, 0.8f);
            Color armorDark = new Color(0.5f, 0.5f, 0.6f);
            Color plume = new Color(0.8f, 0.2f, 0.2f);
            Color skin = new Color(0.9f, 0.75f, 0.6f);
            Color visor = new Color(0.2f, 0.2f, 0.3f);

            // Helmet with plume
            SetPixels(tex, 6, 14, 4, 2, plume);
            SetPixels(tex, 5, 12, 6, 2, armor);
            SetPixels(tex, 6, 11, 4, 1, visor);

            // Head/face
            SetPixels(tex, 6, 10, 4, 1, skin);

            // Body armor
            SetPixels(tex, 5, 5, 6, 5, armor);
            SetPixels(tex, 6, 6, 4, 3, armorDark);

            // Arms
            SetPixels(tex, 3, 6, 2, 4, armor);
            SetPixels(tex, 11, 6, 2, 4, armor);

            // Legs
            SetPixels(tex, 5, 1, 2, 4, armorDark);
            SetPixels(tex, 9, 1, 2, 4, armorDark);

            // Sword (right side)
            SetPixels(tex, 13, 4, 1, 6, new Color(0.8f, 0.8f, 0.9f));
            SetPixels(tex, 12, 6, 3, 1, new Color(0.6f, 0.5f, 0.2f)); // hilt

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["knight"] = sprite;
            return sprite;
        }

        public static Sprite GetWizardSprite()
        {
            if (_spriteCache.TryGetValue("wizard", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Wizard - blue robe with pointy hat
            Color robe = new Color(0.2f, 0.2f, 0.6f);
            Color robeDark = new Color(0.15f, 0.15f, 0.4f);
            Color hat = new Color(0.3f, 0.2f, 0.5f);
            Color skin = new Color(0.9f, 0.75f, 0.6f);
            Color beard = new Color(0.8f, 0.8f, 0.8f);
            Color staff = new Color(0.5f, 0.3f, 0.1f);
            Color magic = new Color(0.3f, 0.8f, 1f);

            // Pointy hat
            SetPixels(tex, 7, 15, 2, 1, hat);
            SetPixels(tex, 6, 13, 4, 2, hat);
            SetPixels(tex, 5, 12, 6, 1, hat);

            // Face
            SetPixels(tex, 6, 10, 4, 2, skin);

            // Beard
            SetPixels(tex, 6, 8, 4, 2, beard);
            SetPixels(tex, 7, 7, 2, 1, beard);

            // Robe body
            SetPixels(tex, 4, 2, 8, 5, robe);
            SetPixels(tex, 5, 3, 6, 3, robeDark);

            // Robe bottom (wider)
            SetPixels(tex, 3, 0, 10, 2, robe);

            // Staff
            SetPixels(tex, 13, 2, 1, 10, staff);
            SetPixels(tex, 12, 11, 3, 2, magic); // magic orb

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["wizard"] = sprite;
            return sprite;
        }

        public static Sprite GetSerfSprite()
        {
            if (_spriteCache.TryGetValue("serf", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            // Serf - brown peasant clothes
            Color tunic = new Color(0.5f, 0.35f, 0.2f);
            Color tunicDark = new Color(0.4f, 0.25f, 0.15f);
            Color skin = new Color(0.9f, 0.75f, 0.6f);
            Color hair = new Color(0.4f, 0.3f, 0.2f);
            Color pants = new Color(0.3f, 0.25f, 0.2f);

            // Hair
            SetPixels(tex, 5, 13, 6, 2, hair);
            SetPixels(tex, 6, 12, 4, 1, hair);

            // Face
            SetPixels(tex, 6, 10, 4, 2, skin);

            // Tunic
            SetPixels(tex, 5, 5, 6, 5, tunic);
            SetPixels(tex, 6, 6, 4, 3, tunicDark);

            // Arms
            SetPixels(tex, 3, 6, 2, 3, skin);
            SetPixels(tex, 11, 6, 2, 3, skin);

            // Pants
            SetPixels(tex, 5, 1, 2, 4, pants);
            SetPixels(tex, 9, 1, 2, 4, pants);

            // Feet
            SetPixels(tex, 5, 0, 2, 1, tunicDark);
            SetPixels(tex, 9, 0, 2, 1, tunicDark);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["serf"] = sprite;
            return sprite;
        }

        #endregion

        #region Enemy Sprites

        public static Sprite GetBatSprite()
        {
            if (_spriteCache.TryGetValue("bat", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color body = new Color(0.3f, 0.2f, 0.3f);
            Color wing = new Color(0.4f, 0.3f, 0.4f);
            Color eye = new Color(1f, 0.2f, 0.2f);

            // Wings spread
            SetPixels(tex, 1, 8, 4, 3, wing);
            SetPixels(tex, 11, 8, 4, 3, wing);

            // Body
            SetPixels(tex, 6, 7, 4, 4, body);

            // Head
            SetPixels(tex, 7, 11, 2, 2, body);

            // Ears
            SetPixel(tex, 6, 13, body);
            SetPixel(tex, 9, 13, body);

            // Eyes
            SetPixel(tex, 7, 12, eye);
            SetPixel(tex, 8, 12, eye);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["bat"] = sprite;
            return sprite;
        }

        public static Sprite GetGhostSprite()
        {
            if (_spriteCache.TryGetValue("ghost", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color body = new Color(0.9f, 0.9f, 1f, 0.8f);
            Color bodyDark = new Color(0.7f, 0.7f, 0.9f, 0.8f);
            Color eye = new Color(0.1f, 0.1f, 0.2f);

            // Body (wavy bottom)
            SetPixels(tex, 4, 4, 8, 8, body);
            SetPixels(tex, 5, 5, 6, 5, bodyDark);

            // Wavy bottom
            SetPixel(tex, 4, 2, body);
            SetPixel(tex, 5, 3, body);
            SetPixel(tex, 6, 2, body);
            SetPixel(tex, 7, 3, body);
            SetPixel(tex, 8, 2, body);
            SetPixel(tex, 9, 3, body);
            SetPixel(tex, 10, 2, body);
            SetPixel(tex, 11, 3, body);

            // Eyes
            SetPixels(tex, 5, 8, 2, 2, eye);
            SetPixels(tex, 9, 8, 2, 2, eye);

            // Mouth (o shape)
            SetPixel(tex, 7, 5, eye);
            SetPixel(tex, 8, 5, eye);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["ghost"] = sprite;
            return sprite;
        }

        public static Sprite GetSkeletonSprite()
        {
            if (_spriteCache.TryGetValue("skeleton", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color bone = new Color(0.9f, 0.9f, 0.85f);
            Color boneDark = new Color(0.7f, 0.7f, 0.65f);
            Color eye = new Color(0.2f, 0f, 0f);

            // Skull
            SetPixels(tex, 5, 11, 6, 4, bone);
            SetPixels(tex, 6, 10, 4, 1, boneDark);

            // Eye sockets
            SetPixels(tex, 6, 12, 2, 2, eye);
            SetPixels(tex, 9, 12, 2, 2, eye);

            // Jaw
            SetPixels(tex, 6, 10, 4, 1, boneDark);

            // Ribcage
            SetPixels(tex, 5, 5, 6, 4, bone);
            SetPixel(tex, 6, 7, boneDark);
            SetPixel(tex, 7, 6, boneDark);
            SetPixel(tex, 8, 7, boneDark);
            SetPixel(tex, 9, 6, boneDark);

            // Spine
            SetPixels(tex, 7, 4, 2, 1, bone);

            // Arms (bones)
            SetPixels(tex, 3, 6, 2, 1, bone);
            SetPixels(tex, 2, 5, 1, 2, bone);
            SetPixels(tex, 11, 6, 2, 1, bone);
            SetPixels(tex, 13, 5, 1, 2, bone);

            // Legs
            SetPixels(tex, 5, 1, 2, 3, bone);
            SetPixels(tex, 9, 1, 2, 3, bone);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["skeleton"] = sprite;
            return sprite;
        }

        public static Sprite GetSpiderSprite()
        {
            if (_spriteCache.TryGetValue("spider", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color body = new Color(0.2f, 0.15f, 0.1f);
            Color bodyLight = new Color(0.3f, 0.2f, 0.15f);
            Color eye = new Color(1f, 0f, 0f);
            Color leg = new Color(0.25f, 0.2f, 0.15f);

            // Body
            SetPixels(tex, 6, 6, 4, 4, body);
            SetPixels(tex, 5, 7, 6, 2, bodyLight);

            // Head
            SetPixels(tex, 6, 10, 4, 2, body);

            // Eyes (8 eyes!)
            SetPixel(tex, 6, 11, eye);
            SetPixel(tex, 7, 11, eye);
            SetPixel(tex, 8, 11, eye);
            SetPixel(tex, 9, 11, eye);

            // Legs (4 on each side)
            // Left legs
            SetPixel(tex, 4, 9, leg); SetPixel(tex, 3, 10, leg);
            SetPixel(tex, 4, 8, leg); SetPixel(tex, 2, 8, leg);
            SetPixel(tex, 4, 7, leg); SetPixel(tex, 2, 6, leg);
            SetPixel(tex, 4, 6, leg); SetPixel(tex, 3, 5, leg);
            // Right legs
            SetPixel(tex, 11, 9, leg); SetPixel(tex, 12, 10, leg);
            SetPixel(tex, 11, 8, leg); SetPixel(tex, 13, 8, leg);
            SetPixel(tex, 11, 7, leg); SetPixel(tex, 13, 6, leg);
            SetPixel(tex, 11, 6, leg); SetPixel(tex, 12, 5, leg);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["spider"] = sprite;
            return sprite;
        }

        public static Sprite GetMummySprite()
        {
            if (_spriteCache.TryGetValue("mummy", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color wrap = new Color(0.85f, 0.8f, 0.7f);
            Color wrapDark = new Color(0.7f, 0.65f, 0.55f);
            Color eye = new Color(0.8f, 0.2f, 0.2f);

            // Head wrapped
            SetPixels(tex, 5, 11, 6, 4, wrap);
            SetPixels(tex, 6, 12, 4, 2, wrapDark);

            // Eyes glowing through wraps
            SetPixel(tex, 6, 13, eye);
            SetPixel(tex, 9, 13, eye);

            // Body wrapped
            SetPixels(tex, 4, 4, 8, 7, wrap);
            // Wrap lines
            SetPixels(tex, 4, 9, 8, 1, wrapDark);
            SetPixels(tex, 4, 6, 8, 1, wrapDark);

            // Arms outstretched
            SetPixels(tex, 1, 7, 3, 2, wrap);
            SetPixels(tex, 12, 7, 3, 2, wrap);

            // Legs
            SetPixels(tex, 5, 0, 2, 4, wrap);
            SetPixels(tex, 9, 0, 2, 4, wrap);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["mummy"] = sprite;
            return sprite;
        }

        public static Sprite GetWitchSprite()
        {
            if (_spriteCache.TryGetValue("witch", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color dress = new Color(0.1f, 0.1f, 0.15f);
            Color dressPurple = new Color(0.3f, 0.1f, 0.3f);
            Color skin = new Color(0.5f, 0.8f, 0.5f); // green skin
            Color hat = new Color(0.1f, 0.1f, 0.1f);
            Color hair = new Color(0.2f, 0.2f, 0.2f);

            // Pointy witch hat
            SetPixel(tex, 7, 15, hat);
            SetPixels(tex, 6, 14, 4, 1, hat);
            SetPixels(tex, 5, 13, 6, 1, hat);
            SetPixels(tex, 4, 12, 8, 1, hat);

            // Hair
            SetPixels(tex, 4, 10, 2, 2, hair);
            SetPixels(tex, 10, 10, 2, 2, hair);

            // Face (green)
            SetPixels(tex, 6, 10, 4, 2, skin);

            // Dress
            SetPixels(tex, 4, 3, 8, 7, dress);
            SetPixels(tex, 5, 4, 6, 4, dressPurple);

            // Dress bottom (wider)
            SetPixels(tex, 3, 0, 10, 3, dress);

            // Broom
            SetPixels(tex, 13, 0, 1, 10, new Color(0.5f, 0.3f, 0.1f));
            SetPixels(tex, 12, 0, 3, 2, new Color(0.6f, 0.5f, 0.2f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["witch"] = sprite;
            return sprite;
        }

        public static Sprite GetDemonSprite()
        {
            if (_spriteCache.TryGetValue("demon", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color body = new Color(0.6f, 0.1f, 0.1f);
            Color bodyDark = new Color(0.4f, 0.05f, 0.05f);
            Color horn = new Color(0.3f, 0.2f, 0.1f);
            Color eye = new Color(1f, 0.8f, 0f);

            // Horns
            SetPixel(tex, 4, 15, horn);
            SetPixel(tex, 5, 14, horn);
            SetPixel(tex, 11, 15, horn);
            SetPixel(tex, 10, 14, horn);

            // Head
            SetPixels(tex, 5, 11, 6, 3, body);

            // Eyes
            SetPixels(tex, 6, 12, 2, 1, eye);
            SetPixels(tex, 9, 12, 2, 1, eye);

            // Body
            SetPixels(tex, 4, 4, 8, 7, body);
            SetPixels(tex, 5, 5, 6, 4, bodyDark);

            // Wings
            SetPixels(tex, 1, 6, 3, 4, bodyDark);
            SetPixels(tex, 12, 6, 3, 4, bodyDark);

            // Tail
            SetPixel(tex, 8, 3, body);
            SetPixel(tex, 9, 2, body);
            SetPixel(tex, 10, 1, body);
            SetPixel(tex, 11, 1, bodyDark);

            // Legs
            SetPixels(tex, 5, 0, 2, 4, body);
            SetPixels(tex, 9, 0, 2, 4, body);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["demon"] = sprite;
            return sprite;
        }

        public static Sprite GetVampireSprite()
        {
            if (_spriteCache.TryGetValue("vampire", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color cape = new Color(0.2f, 0f, 0f);
            Color capeLining = new Color(0.5f, 0.1f, 0.1f);
            Color skin = new Color(0.85f, 0.85f, 0.9f); // pale
            Color hair = new Color(0.1f, 0.1f, 0.1f);
            Color suit = new Color(0.15f, 0.15f, 0.15f);
            Color eye = new Color(0.8f, 0f, 0f);

            // Hair (slicked back)
            SetPixels(tex, 5, 13, 6, 2, hair);

            // Face (pale)
            SetPixels(tex, 6, 10, 4, 3, skin);

            // Eyes
            SetPixel(tex, 6, 12, eye);
            SetPixel(tex, 9, 12, eye);

            // Fangs
            SetPixel(tex, 7, 10, Color.white);
            SetPixel(tex, 8, 10, Color.white);

            // Cape (spread wide)
            SetPixels(tex, 2, 2, 12, 8, cape);
            SetPixels(tex, 3, 3, 10, 6, capeLining);

            // Suit underneath
            SetPixels(tex, 5, 3, 6, 6, suit);

            // Hands emerging from cape
            SetPixels(tex, 2, 5, 2, 2, skin);
            SetPixels(tex, 12, 5, 2, 2, skin);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["vampire"] = sprite;
            return sprite;
        }

        public static Sprite GetReaperSprite()
        {
            if (_spriteCache.TryGetValue("reaper", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color robe = new Color(0.1f, 0.1f, 0.1f);
            Color robeDark = new Color(0.05f, 0.05f, 0.05f);
            Color skull = new Color(0.9f, 0.9f, 0.85f);
            Color scythe = new Color(0.6f, 0.6f, 0.7f);
            Color handle = new Color(0.4f, 0.3f, 0.2f);

            // Hood
            SetPixels(tex, 4, 11, 8, 4, robe);
            SetPixels(tex, 5, 12, 6, 2, robeDark);

            // Skull face in hood
            SetPixels(tex, 6, 11, 4, 3, skull);
            SetPixel(tex, 6, 12, robeDark); // eye socket
            SetPixel(tex, 9, 12, robeDark); // eye socket

            // Robe body
            SetPixels(tex, 3, 1, 10, 10, robe);
            SetPixels(tex, 4, 2, 8, 7, robeDark);

            // Scythe
            SetPixels(tex, 12, 6, 1, 9, handle);
            SetPixels(tex, 9, 14, 4, 1, scythe);
            SetPixels(tex, 8, 13, 2, 1, scythe);
            SetPixel(tex, 7, 12, scythe);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["reaper"] = sprite;
            return sprite;
        }

        public static Sprite GetWerewolfSprite()
        {
            if (_spriteCache.TryGetValue("werewolf", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color fur = new Color(0.4f, 0.3f, 0.2f);
            Color furDark = new Color(0.3f, 0.2f, 0.15f);
            Color eye = new Color(1f, 0.8f, 0f);
            Color nose = new Color(0.2f, 0.1f, 0.1f);
            Color teeth = Color.white;

            // Ears
            SetPixel(tex, 4, 15, fur);
            SetPixel(tex, 5, 14, fur);
            SetPixel(tex, 11, 15, fur);
            SetPixel(tex, 10, 14, fur);

            // Head/snout
            SetPixels(tex, 5, 11, 6, 3, fur);
            SetPixels(tex, 6, 10, 4, 1, furDark); // snout

            // Eyes
            SetPixel(tex, 6, 13, eye);
            SetPixel(tex, 9, 13, eye);

            // Nose
            SetPixel(tex, 7, 10, nose);
            SetPixel(tex, 8, 10, nose);

            // Teeth
            SetPixel(tex, 6, 10, teeth);
            SetPixel(tex, 9, 10, teeth);

            // Muscular body
            SetPixels(tex, 4, 4, 8, 6, fur);
            SetPixels(tex, 5, 5, 6, 4, furDark);

            // Arms (clawed)
            SetPixels(tex, 2, 5, 2, 4, fur);
            SetPixels(tex, 12, 5, 2, 4, fur);
            SetPixel(tex, 1, 5, furDark); // claws
            SetPixel(tex, 14, 5, furDark);

            // Legs
            SetPixels(tex, 5, 0, 2, 4, fur);
            SetPixels(tex, 9, 0, 2, 4, fur);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["werewolf"] = sprite;
            return sprite;
        }

        #endregion

        #region Item Sprites

        public static Sprite GetKeySprite(Color keyColor)
        {
            string key = $"key_{keyColor}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color dark = keyColor * 0.7f;
            dark.a = 1;

            // Key head (circular)
            SetPixels(tex, 5, 10, 6, 4, keyColor);
            SetPixels(tex, 6, 11, 4, 2, dark); // hole

            // Key shaft
            SetPixels(tex, 7, 3, 2, 7, keyColor);

            // Key teeth
            SetPixels(tex, 9, 3, 2, 1, keyColor);
            SetPixels(tex, 9, 5, 2, 1, keyColor);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetKeyPieceSprite(int index)
        {
            string key = $"keypiece_{index}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color gold = new Color(1f, 0.85f, 0.2f);
            Color goldDark = new Color(0.8f, 0.65f, 0.1f);
            Color gem = index switch
            {
                0 => Color.red,
                1 => Color.blue,
                2 => Color.green,
                _ => Color.white
            };

            // Ornate key piece
            SetPixels(tex, 4, 8, 8, 6, gold);
            SetPixels(tex, 5, 9, 6, 4, goldDark);

            // Gem in center
            SetPixels(tex, 6, 10, 4, 2, gem);

            // Decorative bottom
            SetPixels(tex, 5, 4, 6, 4, gold);
            SetPixel(tex, 7, 3, gold);
            SetPixel(tex, 8, 3, gold);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetFoodSprite(string foodType)
        {
            string key = $"food_{foodType}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            if (foodType == "chicken")
            {
                Color meat = new Color(0.8f, 0.6f, 0.4f);
                Color meatDark = new Color(0.6f, 0.4f, 0.3f);
                Color bone = new Color(0.9f, 0.9f, 0.85f);

                // Drumstick shape
                SetPixels(tex, 5, 7, 6, 4, meat);
                SetPixels(tex, 6, 8, 4, 2, meatDark);
                SetPixels(tex, 6, 4, 4, 3, meat);
                // Bone
                SetPixels(tex, 7, 11, 2, 3, bone);
            }
            else // bread
            {
                Color crust = new Color(0.7f, 0.5f, 0.2f);
                Color crumb = new Color(0.9f, 0.8f, 0.5f);

                // Loaf shape
                SetPixels(tex, 4, 5, 8, 5, crust);
                SetPixels(tex, 5, 6, 6, 3, crumb);
                SetPixels(tex, 3, 5, 1, 3, crust);
                SetPixels(tex, 12, 5, 1, 3, crust);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetTreasureSprite(string treasureType)
        {
            string key = $"treasure_{treasureType}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color gold = new Color(1f, 0.85f, 0.2f);
            Color goldDark = new Color(0.8f, 0.65f, 0.1f);

            if (treasureType == "crown")
            {
                Color gem = Color.red;
                // Crown base
                SetPixels(tex, 3, 5, 10, 4, gold);
                SetPixels(tex, 4, 6, 8, 2, goldDark);
                // Points
                SetPixels(tex, 3, 9, 2, 3, gold);
                SetPixels(tex, 7, 9, 2, 4, gold);
                SetPixels(tex, 11, 9, 2, 3, gold);
                // Gems
                SetPixel(tex, 4, 10, gem);
                SetPixel(tex, 8, 11, gem);
                SetPixel(tex, 12, 10, gem);
            }
            else // chalice
            {
                // Cup
                SetPixels(tex, 5, 9, 6, 5, gold);
                SetPixels(tex, 6, 10, 4, 3, goldDark);
                // Stem
                SetPixels(tex, 7, 5, 2, 4, gold);
                // Base
                SetPixels(tex, 5, 3, 6, 2, gold);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetWeaponSprite(string weaponType)
        {
            string key = $"weapon_{weaponType}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            if (weaponType == "sword")
            {
                Color blade = new Color(0.8f, 0.8f, 0.9f);
                Color hilt = new Color(0.5f, 0.4f, 0.2f);
                Color guard = new Color(0.6f, 0.6f, 0.7f);

                // Blade
                SetPixels(tex, 7, 6, 2, 9, blade);
                SetPixel(tex, 7, 15, blade);
                // Guard
                SetPixels(tex, 5, 5, 6, 1, guard);
                // Hilt
                SetPixels(tex, 7, 2, 2, 3, hilt);
            }
            else if (weaponType == "axe")
            {
                Color blade = new Color(0.6f, 0.6f, 0.7f);
                Color handle = new Color(0.5f, 0.3f, 0.2f);

                // Handle
                SetPixels(tex, 7, 2, 2, 10, handle);
                // Axe head
                SetPixels(tex, 9, 9, 4, 4, blade);
                SetPixels(tex, 10, 8, 2, 1, blade);
                SetPixels(tex, 10, 13, 2, 1, blade);
            }
            else // dagger
            {
                Color blade = new Color(0.7f, 0.7f, 0.8f);
                Color hilt = new Color(0.4f, 0.3f, 0.2f);

                // Short blade
                SetPixels(tex, 7, 8, 2, 6, blade);
                SetPixel(tex, 7, 14, blade);
                // Guard
                SetPixels(tex, 5, 7, 6, 1, hilt);
                // Hilt
                SetPixels(tex, 7, 4, 2, 3, hilt);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Environment Sprites

        public static Sprite GetDoorSprite(bool isOpen, Color? keyColor = null)
        {
            string key = $"door_{isOpen}_{keyColor}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color wood = new Color(0.5f, 0.3f, 0.15f);
            Color woodDark = new Color(0.35f, 0.2f, 0.1f);
            Color metal = new Color(0.4f, 0.4f, 0.45f);

            if (isOpen)
            {
                // Open door frame
                SetPixels(tex, 2, 0, 2, 16, wood);
                SetPixels(tex, 12, 0, 2, 16, wood);
                SetPixels(tex, 2, 14, 12, 2, wood);
                // Dark opening
                SetPixels(tex, 4, 0, 8, 14, new Color(0.1f, 0.1f, 0.15f));
            }
            else
            {
                // Closed door
                SetPixels(tex, 2, 0, 12, 16, wood);
                SetPixels(tex, 4, 2, 8, 12, woodDark);

                // Planks
                SetPixels(tex, 2, 5, 12, 1, woodDark);
                SetPixels(tex, 2, 10, 12, 1, woodDark);

                // Handle/lock
                if (keyColor.HasValue)
                {
                    SetPixels(tex, 10, 7, 2, 2, keyColor.Value);
                }
                else
                {
                    SetPixels(tex, 10, 7, 2, 2, metal);
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        public static Sprite GetWallTile()
        {
            if (_spriteCache.TryGetValue("wall", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;

            Color stone = new Color(0.4f, 0.4f, 0.45f);
            Color stoneDark = new Color(0.3f, 0.3f, 0.35f);
            Color mortar = new Color(0.25f, 0.25f, 0.3f);

            // Fill with stone
            ClearTexture(tex, stone);

            // Add brick pattern
            // Row 1
            SetPixels(tex, 0, 0, 16, 1, mortar);
            SetPixels(tex, 7, 0, 1, 8, mortar);
            // Row 2
            SetPixels(tex, 0, 8, 16, 1, mortar);
            SetPixels(tex, 3, 8, 1, 8, mortar);
            SetPixels(tex, 11, 8, 1, 8, mortar);

            // Add some texture variation
            SetPixel(tex, 2, 3, stoneDark);
            SetPixel(tex, 5, 5, stoneDark);
            SetPixel(tex, 10, 2, stoneDark);
            SetPixel(tex, 13, 6, stoneDark);
            SetPixel(tex, 1, 11, stoneDark);
            SetPixel(tex, 6, 13, stoneDark);
            SetPixel(tex, 14, 10, stoneDark);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["wall"] = sprite;
            return sprite;
        }

        public static Sprite GetFloorTile()
        {
            if (_spriteCache.TryGetValue("floor", out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;

            Color floor = new Color(0.25f, 0.2f, 0.18f);
            Color floorLight = new Color(0.3f, 0.25f, 0.2f);
            Color floorDark = new Color(0.2f, 0.15f, 0.12f);

            // Checkered pattern
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool isLight = ((x / 4) + (y / 4)) % 2 == 0;
                    tex.SetPixel(x, y, isLight ? floorLight : floor);
                }
            }

            // Add some dirt/wear
            SetPixel(tex, 3, 3, floorDark);
            SetPixel(tex, 7, 11, floorDark);
            SetPixel(tex, 12, 5, floorDark);
            SetPixel(tex, 9, 14, floorDark);

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache["floor"] = sprite;
            return sprite;
        }

        public static Sprite GetStairsSprite(bool goingUp)
        {
            string key = goingUp ? "stairs_up" : "stairs_down";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            Color stone = new Color(0.5f, 0.5f, 0.55f);
            Color stoneDark = new Color(0.35f, 0.35f, 0.4f);

            if (goingUp)
            {
                // Stairs going up (back to front)
                SetPixels(tex, 2, 12, 12, 3, stone);
                SetPixels(tex, 2, 9, 12, 2, stoneDark);
                SetPixels(tex, 2, 6, 12, 2, stone);
                SetPixels(tex, 2, 3, 12, 2, stoneDark);
                SetPixels(tex, 2, 0, 12, 2, stone);
            }
            else
            {
                // Stairs going down (front to back)
                SetPixels(tex, 2, 0, 12, 3, stone);
                SetPixels(tex, 2, 4, 12, 2, stoneDark);
                SetPixels(tex, 2, 7, 12, 2, stone);
                SetPixels(tex, 2, 10, 12, 2, stoneDark);
                SetPixels(tex, 2, 13, 12, 2, stone);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
            _spriteCache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Projectile Sprites

        public static Sprite GetProjectileSprite(string projectileType)
        {
            string key = $"projectile_{projectileType}";
            if (_spriteCache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(8, 8);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex, Color.clear);

            if (projectileType == "sword_slash")
            {
                Color blade = new Color(0.9f, 0.9f, 1f, 0.8f);
                SetPixels(tex, 1, 3, 6, 2, blade);
                SetPixel(tex, 7, 4, blade);
            }
            else if (projectileType == "magic_bolt")
            {
                Color magic = new Color(0.3f, 0.5f, 1f);
                Color glow = new Color(0.6f, 0.8f, 1f);
                SetPixels(tex, 2, 2, 4, 4, magic);
                SetPixels(tex, 3, 3, 2, 2, glow);
            }
            else // axe throw
            {
                Color metal = new Color(0.6f, 0.6f, 0.7f);
                Color handle = new Color(0.5f, 0.3f, 0.2f);
                SetPixels(tex, 3, 1, 2, 6, handle);
                SetPixels(tex, 5, 4, 2, 3, metal);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
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

        private static void SetPixel(Texture2D tex, int x, int y, Color color)
        {
            if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
                tex.SetPixel(x, y, color);
        }

        private static void SetPixels(Texture2D tex, int startX, int startY, int width, int height, Color color)
        {
            for (int y = startY; y < startY + height; y++)
            {
                for (int x = startX; x < startX + width; x++)
                {
                    SetPixel(tex, x, y, color);
                }
            }
        }

        #endregion
    }
}
