using System.Collections.Generic;
using UnityEngine;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Dynamic lighting system that adds atmospheric lighting effects.
    /// Creates torch glow, ambient shadows, and flickering light effects.
    /// </summary>
    public class DynamicLightingSystem : MonoBehaviour
    {
        public static DynamicLightingSystem Instance { get; private set; }

        [Header("Ambient Lighting")]
        [Tooltip("For HD mode, use subtle ambient. For retro mode, use stronger atmosphere.")]
        [SerializeField] private bool useHDLighting = true;
        [SerializeField] private Color ambientColor = new Color(0.15f, 0.12f, 0.2f, 0.6f);
        [SerializeField] private float ambientIntensity = 0.1f; // Reduced for HD mode (was 0.4f)

        [Header("Torch Settings")]
        [SerializeField] private float torchRadius = 4f;
        [SerializeField] private float torchIntensity = 1.2f;
        [SerializeField] private Color torchColor = new Color(1f, 0.7f, 0.3f, 0.8f);
        [SerializeField] private float flickerSpeed = 8f;
        [SerializeField] private float flickerAmount = 0.15f;

        [Header("Player Light")]
        [SerializeField] private float playerLightRadius = 2.5f;
        [SerializeField] private Color playerLightColor = new Color(0.9f, 0.85f, 0.7f, 0.5f);

        // Light sources
        private List<TorchLight> _torchLights = new();
        private GameObject _ambientOverlay;
        private GameObject _playerLight;
        private SpriteRenderer _playerLightRenderer;

        // Cached references
        private Transform _playerTransform;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            CreateAmbientOverlay();
        }

        private void Start()
        {
            // Find player
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
                CreatePlayerLight();
            }
        }

        private void Update()
        {
            UpdateTorchLights();
            UpdatePlayerLight();
        }

        private void CreateAmbientOverlay()
        {
            // Skip ambient overlay entirely in HD mode for cleaner Midjourney textures
            if (useHDLighting)
            {
                Debug.Log("[DynamicLightingSystem] HD Mode: Skipping ambient overlay for crisp textures");
                return; // Actually skip it!
            }

            _ambientOverlay = new GameObject("AmbientOverlay");
            _ambientOverlay.transform.SetParent(transform);

            var sr = _ambientOverlay.AddComponent<SpriteRenderer>();

            // Create a large overlay sprite
            int size = 512;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Bilinear;

            Color[] pixels = new Color[size * size];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = ambientColor;
            }
            tex.SetPixels(pixels);
            tex.Apply();

            sr.sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32);
            sr.sortingLayerName = "Lighting";
            sr.sortingOrder = 1000;

            sr.color = new Color(1, 1, 1, ambientIntensity);

            // Scale to cover the room
            _ambientOverlay.transform.localScale = Vector3.one * 2f;
        }

        private void CreatePlayerLight()
        {
            _playerLight = new GameObject("PlayerLight");
            _playerLight.transform.SetParent(transform);

            _playerLightRenderer = _playerLight.AddComponent<SpriteRenderer>();

            // Create radial gradient light sprite
            int size = 128;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Bilinear;

            float center = size / 2f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    float normalizedDist = dist / center;

                    // Smooth radial falloff
                    float alpha = Mathf.Clamp01(1f - Mathf.Pow(normalizedDist, 1.5f));
                    tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }
            tex.Apply();

            _playerLightRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size / (playerLightRadius * 2));
            _playerLightRenderer.sortingLayerName = "Lighting";
            _playerLightRenderer.sortingOrder = 999;
            _playerLightRenderer.color = playerLightColor;
            _playerLightRenderer.material = GetAdditiveMaterial();
        }

        /// <summary>
        /// Registers a torch at the given position.
        /// </summary>
        public TorchLight AddTorch(Vector3 position, float radius = -1, Color? color = null)
        {
            var torchObj = new GameObject($"TorchLight_{_torchLights.Count}");
            torchObj.transform.SetParent(transform);
            torchObj.transform.position = position;

            var torchLight = new TorchLight
            {
                gameObject = torchObj,
                position = position,
                radius = radius > 0 ? radius : torchRadius,
                color = color ?? torchColor,
                flickerOffset = Random.value * 100f
            };

            // Create light sprite
            var sr = torchObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateLightSprite(128);
            sr.sortingLayerName = "Lighting";
            sr.sortingOrder = 998;
            sr.color = torchLight.color;
            sr.material = GetAdditiveMaterial();

            // Scale based on radius
            float scale = torchLight.radius / 2f;
            torchObj.transform.localScale = Vector3.one * scale;

            torchLight.renderer = sr;
            _torchLights.Add(torchLight);

            // Create secondary glow
            CreateTorchGlow(torchLight);

            return torchLight;
        }

        private void CreateTorchGlow(TorchLight torch)
        {
            var glowObj = new GameObject("Glow");
            glowObj.transform.SetParent(torch.gameObject.transform);
            glowObj.transform.localPosition = Vector3.zero;

            var sr = glowObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateLightSprite(64);
            sr.sortingLayerName = "Lighting";
            sr.sortingOrder = 997;
            sr.color = new Color(torch.color.r, torch.color.g * 0.5f, 0, 0.3f);
            sr.material = GetAdditiveMaterial();

            glowObj.transform.localScale = Vector3.one * 1.8f;

            torch.glowRenderer = sr;
        }

        /// <summary>
        /// Removes all torches (call when changing rooms).
        /// </summary>
        public void ClearTorches()
        {
            foreach (var torch in _torchLights)
            {
                if (torch.gameObject != null)
                {
                    Destroy(torch.gameObject);
                }
            }
            _torchLights.Clear();
        }

        private void UpdateTorchLights()
        {
            float time = Time.time;

            foreach (var torch in _torchLights)
            {
                if (torch.renderer == null) continue;

                // Calculate flicker
                float flicker = Mathf.PerlinNoise(time * flickerSpeed + torch.flickerOffset, 0) * 2 - 1;
                float intensityMod = 1f + flicker * flickerAmount;

                // Apply flicker to color alpha
                Color c = torch.color;
                c.a = Mathf.Clamp01(c.a * intensityMod);
                torch.renderer.color = c;

                // Also flicker the scale slightly
                float scaleMod = 1f + flicker * 0.05f;
                torch.gameObject.transform.localScale = Vector3.one * (torch.radius / 2f) * scaleMod;

                // Flicker glow with offset
                if (torch.glowRenderer != null)
                {
                    float glowFlicker = Mathf.PerlinNoise(time * flickerSpeed * 0.7f + torch.flickerOffset + 50, 0);
                    Color gc = torch.glowRenderer.color;
                    gc.a = 0.2f + glowFlicker * 0.2f;
                    torch.glowRenderer.color = gc;
                }
            }
        }

        private void UpdatePlayerLight()
        {
            if (_playerTransform == null)
            {
                // Try to find player again
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _playerTransform = player.transform;
                    if (_playerLight == null)
                    {
                        CreatePlayerLight();
                    }
                }
                return;
            }

            if (_playerLight != null)
            {
                _playerLight.transform.position = _playerTransform.position;

                // Subtle breathing effect on player light
                float breathe = Mathf.Sin(Time.time * 2f) * 0.05f;
                _playerLight.transform.localScale = Vector3.one * (1f + breathe);
            }

            // Update ambient overlay position to follow camera
            if (_ambientOverlay != null && Camera.main != null)
            {
                _ambientOverlay.transform.position = Camera.main.transform.position;
            }
        }

        private Sprite CreateLightSprite(int size)
        {
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Bilinear;

            float center = size / 2f;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    float normalizedDist = dist / center;

                    // Smooth radial falloff with soft edges
                    float alpha = Mathf.Clamp01(1f - Mathf.Pow(normalizedDist, 2f));
                    alpha = Mathf.SmoothStep(0, 1, alpha);
                    tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }
            tex.Apply();

            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size / 4f);
        }

        private Material GetAdditiveMaterial()
        {
            // Try to find or create an additive sprite material
            var mat = new Material(Shader.Find("Sprites/Default"));
            // For additive blending, we'd normally use a different shader
            // but for now we'll just use the default with adjusted color
            return mat;
        }

        /// <summary>
        /// Sets the ambient light level (0 = dark, 1 = bright).
        /// </summary>
        public void SetAmbientLevel(float level)
        {
            ambientIntensity = 1f - Mathf.Clamp01(level);
            if (_ambientOverlay != null)
            {
                var sr = _ambientOverlay.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = new Color(1, 1, 1, ambientIntensity);
                }
            }
        }

        /// <summary>
        /// Flash effect for combat or special events.
        /// </summary>
        public void FlashLight(Color color, float duration = 0.1f)
        {
            StartCoroutine(FlashRoutine(color, duration));
        }

        private System.Collections.IEnumerator FlashRoutine(Color color, float duration)
        {
            var flashObj = new GameObject("Flash");
            flashObj.transform.SetParent(transform);
            flashObj.transform.position = Camera.main != null ? Camera.main.transform.position : Vector3.zero;

            var sr = flashObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateLightSprite(256);
            sr.sortingLayerName = "Lighting";
            sr.sortingOrder = 1001;
            sr.color = color;

            flashObj.transform.localScale = Vector3.one * 10f;

            float elapsed = 0;
            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                sr.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0f, t));
                yield return null;
            }

            Destroy(flashObj);
        }
    }

    /// <summary>
    /// Represents a torch light source.
    /// </summary>
    public class TorchLight
    {
        public GameObject gameObject;
        public SpriteRenderer renderer;
        public SpriteRenderer glowRenderer;
        public Vector3 position;
        public float radius;
        public Color color;
        public float flickerOffset;
    }
}
