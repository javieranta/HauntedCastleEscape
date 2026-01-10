using UnityEngine;
using UnityEngine.Rendering;
using HauntedCastle.Services;
using HauntedCastle.Data;
using System.Collections.Generic;

namespace HauntedCastle.Effects
{
    /// <summary>
    /// Manages atmospheric effects like fog, ambient lighting, and vignette.
    /// Adjusts atmosphere based on current floor.
    /// Uses simple sprite-based particles to avoid ParticleSystem module dependency.
    /// </summary>
    public class AtmosphereManager : MonoBehaviour
    {
        public static AtmosphereManager Instance { get; private set; }

        [Header("Global Settings")]
        [SerializeField] private bool enableAtmosphere = true;
        [SerializeField] private float transitionDuration = 1f;

        [Header("Floor Atmosphere Settings")]
        [SerializeField] private FloorAtmosphere basementAtmosphere;
        [SerializeField] private FloorAtmosphere castleAtmosphere;
        [SerializeField] private FloorAtmosphere towerAtmosphere;

        [Header("Vignette")]
        [SerializeField] private GameObject vignetteOverlay;
        [SerializeField] private SpriteRenderer vignetteRenderer;
        [SerializeField] private float vignetteIntensity = 0.5f;

        [Header("Dust Particles")]
        [SerializeField] private int maxDustParticles = 20;
        [SerializeField] private float dustSpawnRate = 2f;

        private Camera _mainCamera;
        private int _currentFloor = -1;
        private FloorAtmosphere _currentAtmosphere;
        private FloorAtmosphere _targetAtmosphere;
        private float _transitionProgress = 1f;

        // Simple sprite-based dust system
        private List<DustParticle> _dustParticles = new List<DustParticle>();
        private float _dustSpawnTimer;
        private Sprite _dustSprite;
        private Transform _dustContainer;

        private class DustParticle
        {
            public GameObject gameObject;
            public SpriteRenderer renderer;
            public Vector3 velocity;
            public float lifetime;
            public float maxLifetime;
        }

        [System.Serializable]
        public class FloorAtmosphere
        {
            public Color ambientColor = Color.white;
            public Color fogColor = new Color(0.1f, 0.1f, 0.15f, 0.3f);
            public float fogDensity = 0.02f;
            public float vignetteIntensity = 0.4f;
            public Color vignetteColor = Color.black;
            public bool enableDustParticles = true;
            public Color dustColor = new Color(1f, 0.95f, 0.9f, 0.3f);
            public float particleRate = 5f;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeDefaults();
        }

        private void Start()
        {
            _mainCamera = Camera.main;

            // Subscribe to room changes
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomLoaded;
            }

            CreateVignetteOverlay();
            CreateDustSprite();
            CreateDustContainer();
        }

        private void OnDestroy()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomLoaded;
            }

            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void InitializeDefaults()
        {
            // Basement - dark, dusty, cold
            basementAtmosphere = new FloorAtmosphere
            {
                ambientColor = new Color(0.5f, 0.4f, 0.55f),
                fogColor = new Color(0.15f, 0.1f, 0.2f, 0.4f),
                fogDensity = 0.03f,
                vignetteIntensity = 0.6f,
                vignetteColor = new Color(0.1f, 0.05f, 0.15f),
                enableDustParticles = true,
                dustColor = new Color(0.6f, 0.5f, 0.7f, 0.4f),
                particleRate = 8f
            };

            // Castle - warm, golden, elegant
            castleAtmosphere = new FloorAtmosphere
            {
                ambientColor = new Color(0.95f, 0.85f, 0.7f),
                fogColor = new Color(0.25f, 0.2f, 0.15f, 0.2f),
                fogDensity = 0.015f,
                vignetteIntensity = 0.35f,
                vignetteColor = new Color(0.2f, 0.15f, 0.1f),
                enableDustParticles = true,
                dustColor = new Color(1f, 0.9f, 0.7f, 0.3f),
                particleRate = 3f
            };

            // Tower - cool, windy, bright
            towerAtmosphere = new FloorAtmosphere
            {
                ambientColor = new Color(0.7f, 0.8f, 0.95f),
                fogColor = new Color(0.6f, 0.7f, 0.85f, 0.25f),
                fogDensity = 0.02f,
                vignetteIntensity = 0.3f,
                vignetteColor = new Color(0.1f, 0.15f, 0.25f),
                enableDustParticles = false,
                dustColor = new Color(0.8f, 0.85f, 1f, 0.2f),
                particleRate = 0f
            };

            _currentAtmosphere = castleAtmosphere;
            _targetAtmosphere = castleAtmosphere;
        }

        private void Update()
        {
            if (!enableAtmosphere) return;

            // Smooth transition between atmospheres
            if (_transitionProgress < 1f)
            {
                _transitionProgress += Time.deltaTime / transitionDuration;
                _transitionProgress = Mathf.Clamp01(_transitionProgress);
                UpdateAtmosphereBlend();
            }

            // Update dust particles
            UpdateDustParticles();
        }

        private void OnRoomLoaded(RoomData roomData)
        {
            if (roomData == null) return;

            int newFloor = roomData.floorNumber;
            if (newFloor != _currentFloor)
            {
                _currentFloor = newFloor;
                TransitionToFloorAtmosphere(newFloor);
            }
        }

        private void TransitionToFloorAtmosphere(int floor)
        {
            _targetAtmosphere = floor switch
            {
                0 => basementAtmosphere,
                1 => castleAtmosphere,
                2 => towerAtmosphere,
                _ => castleAtmosphere
            };

            _transitionProgress = 0f;
        }

        private void UpdateAtmosphereBlend()
        {
            float t = _transitionProgress;

            // Blend ambient color
            Color ambient = Color.Lerp(_currentAtmosphere.ambientColor, _targetAtmosphere.ambientColor, t);
            RenderSettings.ambientLight = ambient;

            // Blend vignette
            if (vignetteRenderer != null)
            {
                float vigIntensity = Mathf.Lerp(_currentAtmosphere.vignetteIntensity, _targetAtmosphere.vignetteIntensity, t);
                Color vigColor = Color.Lerp(_currentAtmosphere.vignetteColor, _targetAtmosphere.vignetteColor, t);
                vigColor.a = vigIntensity;
                vignetteRenderer.color = vigColor;
            }

            // Update current when transition completes
            if (_transitionProgress >= 1f)
            {
                _currentAtmosphere = _targetAtmosphere;
            }
        }

        #region Dust Particle System (Sprite-based)

        private void CreateDustSprite()
        {
            // Create a small circular sprite for dust
            int size = 8;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Bilinear;

            Vector2 center = new Vector2(size / 2f, size / 2f);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float alpha = Mathf.Clamp01(1f - (dist / (size / 2f)));
                    alpha = alpha * alpha; // Softer falloff
                    tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }

            tex.Apply();
            _dustSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private void CreateDustContainer()
        {
            var container = new GameObject("DustContainer");
            container.transform.SetParent(transform);
            _dustContainer = container.transform;
        }

        private void UpdateDustParticles()
        {
            if (_currentAtmosphere == null) return;

            // Spawn new particles
            if (_currentAtmosphere.enableDustParticles && _dustParticles.Count < maxDustParticles)
            {
                _dustSpawnTimer += Time.deltaTime;
                float spawnInterval = 1f / Mathf.Max(_currentAtmosphere.particleRate, 0.1f);

                while (_dustSpawnTimer >= spawnInterval && _dustParticles.Count < maxDustParticles)
                {
                    _dustSpawnTimer -= spawnInterval;
                    SpawnDustParticle();
                }
            }

            // Update existing particles
            for (int i = _dustParticles.Count - 1; i >= 0; i--)
            {
                var particle = _dustParticles[i];
                particle.lifetime += Time.deltaTime;

                if (particle.lifetime >= particle.maxLifetime)
                {
                    // Remove expired particle
                    Destroy(particle.gameObject);
                    _dustParticles.RemoveAt(i);
                    continue;
                }

                // Move particle
                particle.gameObject.transform.position += particle.velocity * Time.deltaTime;

                // Fade based on lifetime
                float t = particle.lifetime / particle.maxLifetime;
                float alpha = 1f - t;
                if (t < 0.2f) alpha = t / 0.2f; // Fade in

                Color c = particle.renderer.color;
                c.a = alpha * _currentAtmosphere.dustColor.a;
                particle.renderer.color = c;
            }
        }

        private void SpawnDustParticle()
        {
            if (_mainCamera == null || _dustSprite == null || _dustContainer == null) return;

            // Spawn in camera view
            Vector3 camPos = _mainCamera.transform.position;
            float halfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
            float halfHeight = _mainCamera.orthographicSize;

            Vector3 spawnPos = new Vector3(
                camPos.x + Random.Range(-halfWidth, halfWidth),
                camPos.y + Random.Range(-halfHeight, halfHeight),
                0
            );

            var dustObj = new GameObject("Dust");
            dustObj.transform.SetParent(_dustContainer);
            dustObj.transform.position = spawnPos;
            dustObj.transform.localScale = Vector3.one * Random.Range(0.3f, 0.8f);

            var sr = dustObj.AddComponent<SpriteRenderer>();
            sr.sprite = _dustSprite;
            sr.color = _currentAtmosphere.dustColor;
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 50;

            var particle = new DustParticle
            {
                gameObject = dustObj,
                renderer = sr,
                velocity = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.1f, 0.15f), 0),
                lifetime = 0f,
                maxLifetime = Random.Range(2f, 4f)
            };

            _dustParticles.Add(particle);
        }

        #endregion

        private void CreateVignetteOverlay()
        {
            if (vignetteOverlay != null) return;

            vignetteOverlay = new GameObject("VignetteOverlay");
            vignetteOverlay.transform.SetParent(_mainCamera != null ? _mainCamera.transform : transform);
            vignetteOverlay.transform.localPosition = new Vector3(0, 0, 5);

            vignetteRenderer = vignetteOverlay.AddComponent<SpriteRenderer>();
            vignetteRenderer.sprite = CreateVignetteSprite();
            vignetteRenderer.sortingLayerName = "UI";
            vignetteRenderer.sortingOrder = 999;
            vignetteRenderer.color = new Color(0, 0, 0, vignetteIntensity);

            // Scale to cover screen
            vignetteOverlay.transform.localScale = new Vector3(25, 20, 1);
        }

        private Sprite CreateVignetteSprite()
        {
            int size = 64;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Bilinear;

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float maxDist = size / 2f;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    float normalizedDist = dist / maxDist;

                    // Smooth vignette falloff
                    float alpha = Mathf.Clamp01(Mathf.Pow(normalizedDist, 1.5f));

                    tex.SetPixel(x, y, new Color(1, 1, 1, alpha));
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        /// <summary>
        /// Ensures the AtmosphereManager exists in the scene.
        /// </summary>
        public static AtmosphereManager EnsureExists()
        {
            if (Instance == null)
            {
                var obj = new GameObject("AtmosphereManager");
                Instance = obj.AddComponent<AtmosphereManager>();
            }
            return Instance;
        }

        /// <summary>
        /// Sets atmosphere intensity (for gameplay effects).
        /// </summary>
        public void SetIntensity(float intensity)
        {
            vignetteIntensity = Mathf.Clamp01(intensity);
            if (vignetteRenderer != null)
            {
                Color c = vignetteRenderer.color;
                c.a = vignetteIntensity;
                vignetteRenderer.color = c;
            }
        }

        /// <summary>
        /// Clears all dust particles (e.g., on room transition).
        /// </summary>
        public void ClearDustParticles()
        {
            foreach (var particle in _dustParticles)
            {
                if (particle.gameObject != null)
                {
                    Destroy(particle.gameObject);
                }
            }
            _dustParticles.Clear();
        }
    }
}
