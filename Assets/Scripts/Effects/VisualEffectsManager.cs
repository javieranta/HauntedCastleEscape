using UnityEngine;
using System.Collections;
// TMPro REMOVED - was causing TMP Importer freeze when AddComponent<TextMeshPro>() at runtime

namespace HauntedCastle.Effects
{
    /// <summary>
    /// Manages visual effects including damage numbers, hit flashes, and particles.
    /// </summary>
    public class VisualEffectsManager : MonoBehaviour
    {
        public static VisualEffectsManager Instance { get; private set; }

        [Header("Damage Numbers")]
        [SerializeField] private GameObject damageNumberPrefab;
        [SerializeField] private float damageNumberRiseSpeed = 1.5f;
        [SerializeField] private float damageNumberDuration = 1f;

        [Header("Hit Flash")]
        [SerializeField] private float hitFlashDuration = 0.1f;
        [SerializeField] private Color hitFlashColor = Color.red;

        [Header("Screen Effects")]
        [SerializeField] private float screenShakeDuration = 0.15f;
        [SerializeField] private float screenShakeIntensity = 0.1f;

        private Camera _mainCamera;
        private Vector3 _originalCameraPos;
        private bool _isShaking;

        // CRITICAL: Cache particle sprite to prevent creating new textures every frame
        private Sprite _cachedParticleSprite;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null)
            {
                _originalCameraPos = _mainCamera.transform.localPosition;
            }
        }

        /// <summary>
        /// Shows floating damage number at a position.
        /// </summary>
        public void ShowDamageNumber(Vector3 position, int damage, bool isCritical = false)
        {
            StartCoroutine(DamageNumberRoutine(position, damage, isCritical));
        }

        /// <summary>
        /// Shows a healing number (green).
        /// </summary>
        public void ShowHealNumber(Vector3 position, int amount)
        {
            StartCoroutine(HealNumberRoutine(position, amount));
        }

        /// <summary>
        /// Flashes a sprite renderer red briefly.
        /// </summary>
        public void FlashHit(SpriteRenderer target)
        {
            if (target != null)
            {
                StartCoroutine(HitFlashRoutine(target));
            }
        }

        /// <summary>
        /// Shakes the camera briefly.
        /// </summary>
        public void ShakeScreen(float intensity = -1f, float duration = -1f)
        {
            if (_mainCamera != null && !_isShaking)
            {
                float i = intensity > 0 ? intensity : screenShakeIntensity;
                float d = duration > 0 ? duration : screenShakeDuration;
                StartCoroutine(ScreenShakeRoutine(i, d));
            }
        }

        /// <summary>
        /// Creates a particle burst effect.
        /// </summary>
        public void SpawnParticleBurst(Vector3 position, Color color, int count = 10)
        {
            StartCoroutine(ParticleBurstRoutine(position, color, count));
        }

        /// <summary>
        /// Shows a text popup (for pickups, messages, etc.).
        /// </summary>
        public void ShowTextPopup(Vector3 position, string text, Color color)
        {
            StartCoroutine(TextPopupRoutine(position, text, color));
        }

        private IEnumerator DamageNumberRoutine(Vector3 position, int damage, bool isCritical)
        {
            // DISABLED: TextMeshPro AddComponent at runtime triggers TMP Importer and causes freeze
            // Using simple sprite-based indicator instead

            var damageObj = new GameObject("DamageNumber");
            damageObj.transform.position = position + new Vector3(Random.Range(-0.3f, 0.3f), 0.5f, 0);

            // Use SpriteRenderer instead of TextMeshPro to avoid TMP Importer freeze
            var sr = damageObj.AddComponent<SpriteRenderer>();
            sr.sprite = GetCachedDamageSprite();
            sr.sortingLayerName = "UI";
            sr.sortingOrder = 1000;
            sr.color = isCritical ? Color.yellow : hitFlashColor;

            float elapsed = 0f;
            Vector3 startPos = damageObj.transform.position;

            while (elapsed < damageNumberDuration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / damageNumberDuration;

                // Rise up
                damageObj.transform.position = startPos + Vector3.up * (damageNumberRiseSpeed * t);

                // Fade out
                Color c = sr.color;
                c.a = 1f - t;
                sr.color = c;

                // Scale effect
                float scale = isCritical ? 0.5f - (t * 0.2f) : 0.4f - (t * 0.15f);
                damageObj.transform.localScale = Vector3.one * scale;

                yield return null;
            }

            Destroy(damageObj);
        }

        // Cache the damage indicator sprite
        private static Sprite _cachedDamageSprite;

        private Sprite GetCachedDamageSprite()
        {
            if (_cachedDamageSprite != null) return _cachedDamageSprite;

            int size = 16;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Point;

            // Create a simple burst/impact shape for damage indicator
            Vector2 center = new Vector2(size / 2f, size / 2f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    bool isVisible = dist < size / 2f - 1;
                    tex.SetPixel(x, y, isVisible ? Color.white : Color.clear);
                }
            }

            tex.Apply();
            _cachedDamageSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return _cachedDamageSprite;
        }

        private IEnumerator HealNumberRoutine(Vector3 position, int amount)
        {
            // DISABLED: TextMeshPro AddComponent at runtime triggers TMP Importer and causes freeze
            // Using simple sprite-based indicator instead

            var healObj = new GameObject("HealNumber");
            healObj.transform.position = position + new Vector3(0, 0.5f, 0);

            // Use SpriteRenderer instead of TextMeshPro to avoid TMP Importer freeze
            var sr = healObj.AddComponent<SpriteRenderer>();
            sr.sprite = GetCachedHealSprite();
            sr.sortingLayerName = "UI";
            sr.sortingOrder = 1000;
            sr.color = Color.green;

            float elapsed = 0f;
            Vector3 startPos = healObj.transform.position;

            while (elapsed < damageNumberDuration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / damageNumberDuration;

                healObj.transform.position = startPos + Vector3.up * (damageNumberRiseSpeed * 0.5f * t);
                Color c = sr.color;
                c.a = 1f - t;
                sr.color = c;

                yield return null;
            }

            Destroy(healObj);
        }

        // Cache the heal indicator sprite
        private static Sprite _cachedHealSprite;

        private Sprite GetCachedHealSprite()
        {
            if (_cachedHealSprite != null) return _cachedHealSprite;

            int size = 16;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Point;

            // Create a simple plus/cross shape for heal indicator
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isHorizontal = y >= 6 && y <= 9;
                    bool isVertical = x >= 6 && x <= 9;
                    bool isPlus = isHorizontal || isVertical;
                    tex.SetPixel(x, y, isPlus ? Color.white : Color.clear);
                }
            }

            tex.Apply();
            _cachedHealSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return _cachedHealSprite;
        }

        private IEnumerator HitFlashRoutine(SpriteRenderer target)
        {
            Color originalColor = target.color;
            target.color = hitFlashColor;

            // CRITICAL: Use WaitForSecondsRealtime to work when timeScale = 0
            yield return new WaitForSecondsRealtime(hitFlashDuration);

            if (target != null)
            {
                target.color = originalColor;
            }
        }

        private IEnumerator ScreenShakeRoutine(float intensity, float duration)
        {
            _isShaking = true;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;

                float x = Random.Range(-1f, 1f) * intensity;
                float y = Random.Range(-1f, 1f) * intensity;

                if (_mainCamera != null)
                {
                    _mainCamera.transform.localPosition = _originalCameraPos + new Vector3(x, y, 0);
                }

                yield return null;
            }

            if (_mainCamera != null)
            {
                _mainCamera.transform.localPosition = _originalCameraPos;
            }
            _isShaking = false;
        }

        private IEnumerator ParticleBurstRoutine(Vector3 position, Color color, int count)
        {
            // CRITICAL FIX: Limit particle count to prevent performance issues
            count = Mathf.Min(count, 15);

            // CRITICAL FIX: Get cached sprite ONCE before the loop
            Sprite particleSprite = GetCachedParticleSprite();

            var particles = new GameObject[count];
            var velocities = new Vector2[count];

            // Create all particles and velocities in one pass
            for (int i = 0; i < count; i++)
            {
                var particle = new GameObject($"Particle_{i}");
                particle.transform.position = position;

                var sr = particle.AddComponent<SpriteRenderer>();
                sr.sprite = particleSprite; // Use cached sprite!
                sr.color = color;
                sr.sortingOrder = 100;

                particle.transform.localScale = Vector3.one * Random.Range(0.1f, 0.3f);
                particles[i] = particle;

                // Calculate velocity at same time
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float speed = Random.Range(2f, 5f);
                velocities[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
            }

            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                for (int i = 0; i < count; i++)
                {
                    if (particles[i] != null)
                    {
                        particles[i].transform.position += (Vector3)(velocities[i] * Time.unscaledDeltaTime);
                        particles[i].transform.localScale = Vector3.one * (0.2f * (1f - t));

                        var sr = particles[i].GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            sr.color = new Color(color.r, color.g, color.b, 1f - t);
                        }
                    }
                }

                yield return null;
            }

            for (int i = 0; i < count; i++)
            {
                if (particles[i] != null)
                {
                    Destroy(particles[i]);
                }
            }
        }

        private IEnumerator TextPopupRoutine(Vector3 position, string text, Color color)
        {
            // DISABLED: TextMeshPro AddComponent at runtime triggers TMP Importer and causes freeze
            // Using simple sprite-based indicator instead

            var popupObj = new GameObject("TextPopup");
            popupObj.transform.position = position + new Vector3(0, 0.8f, 0);

            // Use SpriteRenderer instead of TextMeshPro to avoid TMP Importer freeze
            var sr = popupObj.AddComponent<SpriteRenderer>();
            sr.sprite = GetCachedPopupSprite();
            sr.sortingLayerName = "UI";
            sr.sortingOrder = 1000;
            sr.color = color;

            float elapsed = 0f;
            float duration = 1.5f;
            Vector3 startPos = popupObj.transform.position;

            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                popupObj.transform.position = startPos + Vector3.up * (0.5f * t);
                Color c = sr.color;
                c.a = 1f - (t * t);
                sr.color = c;

                yield return null;
            }

            Destroy(popupObj);
        }

        // Cache the popup indicator sprite
        private static Sprite _cachedPopupSprite;

        private Sprite GetCachedPopupSprite()
        {
            if (_cachedPopupSprite != null) return _cachedPopupSprite;

            int size = 24;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Point;

            // Create a simple notification/exclamation shape for popup indicator
            Vector2 center = new Vector2(size / 2f, size / 2f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), center);
                    bool isOuter = dist < size / 2f - 1 && dist > size / 2f - 4;
                    tex.SetPixel(x, y, isOuter ? Color.white : Color.clear);
                }
            }

            tex.Apply();
            _cachedPopupSprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return _cachedPopupSprite;
        }

        /// <summary>
        /// Gets or creates a cached particle sprite to avoid runtime texture creation.
        /// </summary>
        private Sprite GetCachedParticleSprite()
        {
            if (_cachedParticleSprite == null)
            {
                try
                {
                    var tex = new Texture2D(4, 4, TextureFormat.RGBA32, false);
                    tex.filterMode = FilterMode.Bilinear;

                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            // Simple circle-ish shape
                            bool isCorner = (x == 0 || x == 3) && (y == 0 || y == 3);
                            tex.SetPixel(x, y, isCorner ? Color.clear : Color.white);
                        }
                    }

                    tex.Apply(false, false);
                    _cachedParticleSprite = Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[VisualEffectsManager] Failed to create particle sprite: {e.Message}");
                }
            }
            return _cachedParticleSprite;
        }

        /// <summary>
        /// Creates the VisualEffectsManager if it doesn't exist.
        /// </summary>
        public static VisualEffectsManager EnsureExists()
        {
            if (Instance == null)
            {
                var obj = new GameObject("VisualEffectsManager");
                Instance = obj.AddComponent<VisualEffectsManager>();
                DontDestroyOnLoad(obj);
            }
            return Instance;
        }
    }
}
