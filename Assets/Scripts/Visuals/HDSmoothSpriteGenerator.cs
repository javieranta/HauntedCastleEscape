using UnityEngine;
using System.Collections.Generic;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Generates high-definition smooth sprites with realistic textures.
    /// Uses 512x512 textures with gradients, noise, and proper shading.
    /// Designed for photorealistic appearance, not pixel art.
    /// </summary>
    public static class HDSmoothSpriteGenerator
    {
        private static Dictionary<string, Sprite> _cache = new();
        public const int SIZE = 512;
        private const float PPU = 512f;

        // Noise seed for consistent generation
        private static int _noiseSeed = 12345;

        #region Floor Tiles

        public static Sprite GetStoneFloorTile(int variation = 0)
        {
            string key = $"hd_stone_floor_{variation}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Repeat;

            // Base stone colors
            Color baseColor = new Color(0.35f, 0.32f, 0.28f);
            Color darkColor = new Color(0.22f, 0.20f, 0.18f);
            Color lightColor = new Color(0.48f, 0.44f, 0.40f);

            // Generate stone texture with noise
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    // Multi-layered Perlin noise for natural stone look
                    float nx = (x + variation * 100) / (float)SIZE;
                    float ny = y / (float)SIZE;

                    float noise1 = Mathf.PerlinNoise(nx * 8f, ny * 8f);
                    float noise2 = Mathf.PerlinNoise(nx * 16f + 50, ny * 16f + 50) * 0.5f;
                    float noise3 = Mathf.PerlinNoise(nx * 32f + 100, ny * 32f + 100) * 0.25f;
                    float combinedNoise = (noise1 + noise2 + noise3) / 1.75f;

                    // Add subtle tile pattern
                    int tileSize = SIZE / 4;
                    bool isGrout = (x % tileSize < 8) || (y % tileSize < 8);
                    if (isGrout)
                    {
                        combinedNoise *= 0.7f;
                    }

                    Color pixelColor = Color.Lerp(darkColor, lightColor, combinedNoise);

                    // Add very subtle color variation
                    float hueShift = Mathf.PerlinNoise(nx * 4f + 200, ny * 4f + 200) * 0.02f - 0.01f;
                    pixelColor.r += hueShift;
                    pixelColor.g += hueShift * 0.8f;

                    tex.SetPixel(x, y, pixelColor);
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetWoodFloorTile(int variation = 0)
        {
            string key = $"hd_wood_floor_{variation}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            // Wood colors
            Color darkWood = new Color(0.28f, 0.18f, 0.10f);
            Color midWood = new Color(0.45f, 0.30f, 0.18f);
            Color lightWood = new Color(0.60f, 0.42f, 0.25f);

            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    float nx = (x + variation * 50) / (float)SIZE;
                    float ny = y / (float)SIZE;

                    // Wood grain pattern (stretched noise)
                    float grain = Mathf.PerlinNoise(nx * 2f, ny * 20f);
                    float detail = Mathf.PerlinNoise(nx * 8f + 30, ny * 40f + 30) * 0.3f;
                    float combined = grain + detail;

                    // Plank divisions
                    int plankWidth = SIZE / 6;
                    bool isGap = (x % plankWidth) < 4;
                    if (isGap) combined *= 0.5f;

                    Color pixelColor = Color.Lerp(darkWood, lightWood, combined);
                    tex.SetPixel(x, y, pixelColor);
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Wall Tiles

        public static Sprite GetStoneBrickWall(int variation = 0)
        {
            string key = $"hd_stone_wall_{variation}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;

            // Wall colors - lighter gray stone
            Color mortarColor = new Color(0.25f, 0.24f, 0.22f);
            Color stoneBase = new Color(0.50f, 0.48f, 0.45f);
            Color stoneDark = new Color(0.38f, 0.36f, 0.34f);
            Color stoneLight = new Color(0.62f, 0.60f, 0.56f);

            int brickHeight = SIZE / 8;
            int brickWidth = SIZE / 4;
            int mortarSize = 8;

            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    // Determine brick row and offset
                    int row = y / brickHeight;
                    int offsetX = (row % 2 == 0) ? 0 : brickWidth / 2;
                    int localX = (x + offsetX) % brickWidth;
                    int localY = y % brickHeight;

                    // Check if in mortar
                    bool isMortar = localX < mortarSize || localY < mortarSize;

                    float nx = x / (float)SIZE;
                    float ny = y / (float)SIZE;

                    if (isMortar)
                    {
                        // Mortar with slight variation
                        float mortarNoise = Mathf.PerlinNoise(nx * 20f, ny * 20f) * 0.1f;
                        Color mc = mortarColor;
                        mc.r += mortarNoise;
                        mc.g += mortarNoise;
                        mc.b += mortarNoise;
                        tex.SetPixel(x, y, mc);
                    }
                    else
                    {
                        // Stone brick with texture
                        int brickSeed = row * 100 + ((x + offsetX) / brickWidth);
                        float stoneNoise = Mathf.PerlinNoise(nx * 12f + brickSeed, ny * 12f);
                        float detailNoise = Mathf.PerlinNoise(nx * 30f + brickSeed * 2, ny * 30f) * 0.3f;

                        // Edge darkening for 3D effect
                        float edgeFactor = 1f;
                        float edgeDistX = Mathf.Min(localX - mortarSize, brickWidth - localX) / (float)brickWidth;
                        float edgeDistY = Mathf.Min(localY - mortarSize, brickHeight - localY) / (float)brickHeight;
                        edgeFactor = Mathf.Min(edgeDistX, edgeDistY) * 4f;
                        edgeFactor = Mathf.Clamp01(edgeFactor);

                        float combined = (stoneNoise + detailNoise) * edgeFactor;
                        Color pixelColor = Color.Lerp(stoneDark, stoneLight, combined);

                        // Subtle color variation per brick
                        float brickHue = Mathf.PerlinNoise(brickSeed * 0.1f, 0) * 0.04f - 0.02f;
                        pixelColor.r += brickHue;
                        pixelColor.b -= brickHue;

                        tex.SetPixel(x, y, pixelColor);
                    }
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Props

        public static Sprite GetTorchSprite()
        {
            string key = "hd_torch";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            // Torch handle
            Color woodDark = new Color(0.30f, 0.18f, 0.08f);
            Color woodLight = new Color(0.50f, 0.32f, 0.15f);

            // Draw wooden handle with gradient
            int handleWidth = SIZE / 6;
            int handleHeight = SIZE / 2;
            int handleX = (SIZE - handleWidth) / 2;
            int handleY = 20;

            for (int y = handleY; y < handleY + handleHeight; y++)
            {
                for (int x = handleX; x < handleX + handleWidth; x++)
                {
                    float t = (x - handleX) / (float)handleWidth;
                    float woodGrain = Mathf.PerlinNoise(x * 0.05f, y * 0.15f);
                    Color c = Color.Lerp(woodDark, woodLight, t * 0.5f + woodGrain * 0.5f);

                    // Round the edges
                    float centerDist = Mathf.Abs((x - handleX) - handleWidth / 2f) / (handleWidth / 2f);
                    if (centerDist < 1f)
                    {
                        tex.SetPixel(x, y, c);
                    }
                }
            }

            // Flame
            Color flameCore = new Color(1f, 0.95f, 0.7f);
            Color flameInner = new Color(1f, 0.7f, 0.2f);
            Color flameOuter = new Color(0.9f, 0.3f, 0.05f);
            Color flameEdge = new Color(0.6f, 0.1f, 0.0f, 0.5f);

            int flameBaseY = handleY + handleHeight - 20;
            int flameCenterX = SIZE / 2;
            int flameHeight = SIZE / 2 + 40;
            int flameWidth = SIZE / 3;

            for (int y = flameBaseY; y < flameBaseY + flameHeight; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    float normalizedY = (y - flameBaseY) / (float)flameHeight;
                    float currentWidth = flameWidth * (1f - normalizedY * 0.8f);

                    float distFromCenter = Mathf.Abs(x - flameCenterX);

                    // Add flickering noise
                    float flicker = Mathf.PerlinNoise(x * 0.02f + 100, y * 0.03f) * 20f;
                    currentWidth += flicker;

                    if (distFromCenter < currentWidth)
                    {
                        float t = distFromCenter / currentWidth;
                        t = t * t; // Quadratic falloff

                        Color flameColor;
                        if (t < 0.3f)
                            flameColor = Color.Lerp(flameCore, flameInner, t / 0.3f);
                        else if (t < 0.7f)
                            flameColor = Color.Lerp(flameInner, flameOuter, (t - 0.3f) / 0.4f);
                        else
                            flameColor = Color.Lerp(flameOuter, flameEdge, (t - 0.7f) / 0.3f);

                        // Fade at top
                        flameColor.a *= (1f - normalizedY * normalizedY);

                        if (flameColor.a > 0.1f)
                        {
                            Color existing = tex.GetPixel(x, y);
                            tex.SetPixel(x, y, Color.Lerp(existing, flameColor, flameColor.a));
                        }
                    }
                }
            }

            // Add glow around flame
            AddGlow(tex, flameCenterX, flameBaseY + flameHeight / 2, flameWidth * 2, new Color(1f, 0.5f, 0.1f, 0.3f));

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetCobwebSprite()
        {
            string key = "hd_cobweb";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color webColor = new Color(0.9f, 0.9f, 0.88f, 0.85f);
            Color webThin = new Color(0.85f, 0.85f, 0.83f, 0.4f);

            int centerX = 0; // Corner web
            int centerY = SIZE;

            // Draw radial strands
            int numStrands = 12;
            for (int i = 0; i < numStrands; i++)
            {
                float angle = (i / (float)numStrands) * Mathf.PI * 0.5f; // Quarter circle
                float endX = centerX + Mathf.Cos(angle) * SIZE * 1.2f;
                float endY = centerY - Mathf.Sin(angle) * SIZE * 1.2f;

                DrawSmoothLine(tex, centerX, centerY, (int)endX, (int)endY, webColor, 3);
            }

            // Draw concentric arcs
            int numArcs = 8;
            for (int arc = 1; arc <= numArcs; arc++)
            {
                float radius = (arc / (float)numArcs) * SIZE * 0.9f;
                int segments = 30;
                for (int seg = 0; seg < segments; seg++)
                {
                    float angle1 = (seg / (float)segments) * Mathf.PI * 0.5f;
                    float angle2 = ((seg + 1) / (float)segments) * Mathf.PI * 0.5f;

                    // Add slight waviness
                    float wave1 = Mathf.Sin(seg * 2f) * 5f;
                    float wave2 = Mathf.Sin((seg + 1) * 2f) * 5f;

                    int x1 = (int)(centerX + Mathf.Cos(angle1) * (radius + wave1));
                    int y1 = (int)(centerY - Mathf.Sin(angle1) * (radius + wave1));
                    int x2 = (int)(centerX + Mathf.Cos(angle2) * (radius + wave2));
                    int y2 = (int)(centerY - Mathf.Sin(angle2) * (radius + wave2));

                    DrawSmoothLine(tex, x1, y1, x2, y2, webThin, 2);
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetBarrelSprite()
        {
            string key = "hd_barrel";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            // Barrel colors
            Color woodDark = new Color(0.35f, 0.22f, 0.12f);
            Color woodMid = new Color(0.50f, 0.32f, 0.18f);
            Color woodLight = new Color(0.62f, 0.42f, 0.24f);
            Color metalDark = new Color(0.25f, 0.25f, 0.28f);
            Color metalLight = new Color(0.45f, 0.45f, 0.50f);

            int barrelWidth = SIZE * 3 / 4;
            int barrelHeight = SIZE - 40;
            int startX = (SIZE - barrelWidth) / 2;
            int startY = 20;

            // Draw barrel body
            for (int y = startY; y < startY + barrelHeight; y++)
            {
                // Barrel bulge
                float normalizedY = (y - startY) / (float)barrelHeight;
                float bulge = Mathf.Sin(normalizedY * Mathf.PI) * 0.15f + 1f;
                int currentWidth = (int)(barrelWidth * bulge);
                int currentStartX = (SIZE - currentWidth) / 2;

                for (int x = currentStartX; x < currentStartX + currentWidth; x++)
                {
                    // Cylindrical shading
                    float normalizedX = (x - currentStartX) / (float)currentWidth;
                    float shade = Mathf.Sin(normalizedX * Mathf.PI);

                    // Wood grain
                    float grain = Mathf.PerlinNoise(x * 0.02f, y * 0.08f);

                    Color woodColor = Color.Lerp(woodDark, woodLight, shade * 0.7f + grain * 0.3f);

                    // Plank divisions (vertical lines)
                    int plankWidth = currentWidth / 8;
                    if (plankWidth > 0 && ((x - currentStartX) % plankWidth) < 3)
                    {
                        woodColor = Color.Lerp(woodColor, woodDark, 0.3f);
                    }

                    tex.SetPixel(x, y, woodColor);
                }
            }

            // Draw metal bands
            int[] bandPositions = { startY + 30, startY + barrelHeight / 2, startY + barrelHeight - 30 };
            foreach (int bandY in bandPositions)
            {
                for (int by = bandY - 12; by < bandY + 12; by++)
                {
                    if (by < startY || by >= startY + barrelHeight) continue;

                    float normalizedY = (by - startY) / (float)barrelHeight;
                    float bulge = Mathf.Sin(normalizedY * Mathf.PI) * 0.15f + 1f;
                    int currentWidth = (int)(barrelWidth * bulge);
                    int currentStartX = (SIZE - currentWidth) / 2;

                    for (int x = currentStartX; x < currentStartX + currentWidth; x++)
                    {
                        float normalizedX = (x - currentStartX) / (float)currentWidth;
                        float shade = Mathf.Sin(normalizedX * Mathf.PI);
                        Color metalColor = Color.Lerp(metalDark, metalLight, shade);

                        // Add rivet highlights
                        if (((x - currentStartX) % 40) < 8 && Mathf.Abs(by - bandY) < 4)
                        {
                            metalColor = Color.Lerp(metalColor, Color.white, 0.3f);
                        }

                        tex.SetPixel(x, by, metalColor);
                    }
                }
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        public static Sprite GetDoorSprite(bool isOpen)
        {
            string key = $"hd_door_{isOpen}";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            if (isOpen)
            {
                // Dark opening
                Color darkness = new Color(0.05f, 0.05f, 0.08f);
                for (int y = 20; y < SIZE - 20; y++)
                {
                    for (int x = SIZE / 6; x < SIZE * 5 / 6; x++)
                    {
                        tex.SetPixel(x, y, darkness);
                    }
                }
            }
            else
            {
                // Wooden door
                Color woodDark = new Color(0.30f, 0.18f, 0.08f);
                Color woodMid = new Color(0.45f, 0.28f, 0.14f);
                Color woodLight = new Color(0.55f, 0.36f, 0.18f);
                Color metalColor = new Color(0.20f, 0.20f, 0.22f);
                Color metalHighlight = new Color(0.40f, 0.40f, 0.45f);

                int doorWidth = SIZE * 2 / 3;
                int doorHeight = SIZE - 40;
                int doorX = (SIZE - doorWidth) / 2;
                int doorY = 20;

                // Draw door planks
                for (int y = doorY; y < doorY + doorHeight; y++)
                {
                    for (int x = doorX; x < doorX + doorWidth; x++)
                    {
                        float nx = x / (float)SIZE;
                        float ny = y / (float)SIZE;

                        float grain = Mathf.PerlinNoise(nx * 3f, ny * 15f);
                        float detail = Mathf.PerlinNoise(nx * 10f, ny * 30f) * 0.3f;

                        // Plank divisions
                        int plankWidth = doorWidth / 5;
                        bool isGap = plankWidth > 0 && ((x - doorX) % plankWidth) < 4;

                        float t = grain + detail;
                        if (isGap) t *= 0.5f;

                        Color c = Color.Lerp(woodDark, woodLight, t);
                        tex.SetPixel(x, y, c);
                    }
                }

                // Metal hinges
                DrawFilledCircle(tex, doorX + 30, doorY + 80, 25, metalColor, metalHighlight);
                DrawFilledCircle(tex, doorX + 30, doorY + doorHeight - 80, 25, metalColor, metalHighlight);

                // Door handle
                DrawFilledCircle(tex, doorX + doorWidth - 60, doorY + doorHeight / 2, 30, metalColor, metalHighlight);
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Enemies

        public static Sprite GetSpiderSprite()
        {
            string key = "hd_spider";
            if (_cache.TryGetValue(key, out var cached)) return cached;

            var tex = new Texture2D(SIZE, SIZE, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Bilinear;
            ClearTexture(tex);

            Color bodyDark = new Color(0.08f, 0.08f, 0.10f);
            Color bodyMid = new Color(0.15f, 0.14f, 0.18f);
            Color bodyHighlight = new Color(0.25f, 0.24f, 0.28f);
            Color eyeRed = new Color(0.8f, 0.1f, 0.1f);

            int centerX = SIZE / 2;
            int centerY = SIZE / 2;

            // Draw legs (8 legs)
            int legLength = SIZE / 3;
            for (int i = 0; i < 8; i++)
            {
                float baseAngle = (i < 4) ? -0.3f : 0.3f;
                float angle = baseAngle + (i % 4 - 1.5f) * 0.4f;

                int sign = (i < 4) ? -1 : 1;

                // Leg segments
                int jointX = centerX + sign * 60;
                int jointY = centerY + (int)(Mathf.Sin(angle) * 40);
                int endX = centerX + sign * (60 + legLength);
                int endY = centerY + (int)(Mathf.Sin(angle + 0.5f * sign) * legLength);

                DrawSmoothLine(tex, centerX + sign * 30, centerY, jointX, jointY, bodyMid, 8);
                DrawSmoothLine(tex, jointX, jointY, endX, endY, bodyDark, 5);
            }

            // Draw abdomen (back, larger)
            DrawFilledEllipse(tex, centerX, centerY - 40, 80, 100, bodyDark, bodyHighlight);

            // Draw cephalothorax (front, smaller)
            DrawFilledEllipse(tex, centerX, centerY + 50, 55, 50, bodyDark, bodyHighlight);

            // Draw eyes (8 eyes in 2 rows)
            int[] eyeOffsets = { -20, -8, 8, 20 };
            foreach (int offset in eyeOffsets)
            {
                DrawFilledCircle(tex, centerX + offset, centerY + 75, 8, eyeRed, new Color(1f, 0.3f, 0.3f));
            }
            int[] smallEyeOffsets = { -12, 12 };
            foreach (int offset in smallEyeOffsets)
            {
                DrawFilledCircle(tex, centerX + offset, centerY + 85, 5, eyeRed, new Color(1f, 0.3f, 0.3f));
            }

            tex.Apply();
            var sprite = Sprite.Create(tex, new Rect(0, 0, SIZE, SIZE), new Vector2(0.5f, 0.5f), PPU);
            _cache[key] = sprite;
            return sprite;
        }

        #endregion

        #region Helper Methods

        private static void ClearTexture(Texture2D tex)
        {
            Color[] clear = new Color[tex.width * tex.height];
            for (int i = 0; i < clear.Length; i++)
                clear[i] = Color.clear;
            tex.SetPixels(clear);
        }

        private static void DrawSmoothLine(Texture2D tex, int x0, int y0, int x1, int y1, Color color, int thickness)
        {
            float dx = x1 - x0;
            float dy = y1 - y0;
            float length = Mathf.Sqrt(dx * dx + dy * dy);
            if (length < 1) return;

            dx /= length;
            dy /= length;

            for (float t = 0; t <= length; t += 0.5f)
            {
                int cx = (int)(x0 + dx * t);
                int cy = (int)(y0 + dy * t);

                for (int ox = -thickness; ox <= thickness; ox++)
                {
                    for (int oy = -thickness; oy <= thickness; oy++)
                    {
                        float dist = Mathf.Sqrt(ox * ox + oy * oy);
                        if (dist <= thickness)
                        {
                            int px = cx + ox;
                            int py = cy + oy;
                            if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                            {
                                float alpha = 1f - (dist / thickness) * 0.5f;
                                Color existing = tex.GetPixel(px, py);
                                Color blended = Color.Lerp(existing, color, alpha * color.a);
                                blended.a = Mathf.Max(existing.a, color.a * alpha);
                                tex.SetPixel(px, py, blended);
                            }
                        }
                    }
                }
            }
        }

        private static void DrawFilledCircle(Texture2D tex, int cx, int cy, int radius, Color dark, Color light)
        {
            for (int y = cy - radius; y <= cy + radius; y++)
            {
                for (int x = cx - radius; x <= cx + radius; x++)
                {
                    if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;

                    float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                    if (dist <= radius)
                    {
                        // Spherical shading
                        float normalizedDist = dist / radius;
                        float shade = 1f - normalizedDist * normalizedDist;

                        // Light from top-left
                        float lightX = (x - cx) / (float)radius;
                        float lightY = (y - cy) / (float)radius;
                        float lightFactor = Mathf.Clamp01((-lightX - lightY + 1f) * 0.5f);

                        Color c = Color.Lerp(dark, light, shade * lightFactor);
                        tex.SetPixel(x, y, c);
                    }
                }
            }
        }

        private static void DrawFilledEllipse(Texture2D tex, int cx, int cy, int radiusX, int radiusY, Color dark, Color light)
        {
            for (int y = cy - radiusY; y <= cy + radiusY; y++)
            {
                for (int x = cx - radiusX; x <= cx + radiusX; x++)
                {
                    if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;

                    float dx = (x - cx) / (float)radiusX;
                    float dy = (y - cy) / (float)radiusY;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);

                    if (dist <= 1f)
                    {
                        float shade = 1f - dist * dist;
                        float lightFactor = Mathf.Clamp01((-dx - dy + 1f) * 0.5f);
                        Color c = Color.Lerp(dark, light, shade * lightFactor);
                        tex.SetPixel(x, y, c);
                    }
                }
            }
        }

        private static void AddGlow(Texture2D tex, int cx, int cy, int radius, Color glowColor)
        {
            for (int y = cy - radius; y <= cy + radius; y++)
            {
                for (int x = cx - radius; x <= cx + radius; x++)
                {
                    if (x < 0 || x >= tex.width || y < 0 || y >= tex.height) continue;

                    float dist = Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                    if (dist <= radius)
                    {
                        float alpha = (1f - dist / radius) * glowColor.a;
                        Color existing = tex.GetPixel(x, y);
                        Color blended = Color.Lerp(existing, glowColor, alpha * 0.3f);
                        blended.a = Mathf.Max(existing.a, alpha * 0.3f);
                        tex.SetPixel(x, y, blended);
                    }
                }
            }
        }

        public static void ClearCache()
        {
            foreach (var sprite in _cache.Values)
            {
                if (sprite != null && sprite.texture != null)
                    Object.Destroy(sprite.texture);
            }
            _cache.Clear();
        }

        #endregion
    }
}
