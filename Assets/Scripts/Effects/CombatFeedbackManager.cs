using UnityEngine;
using System.Collections;
using TMPro;

namespace HauntedCastle.Effects
{
    /// <summary>
    /// Advanced combat feedback effects including hit stop, attack trails,
    /// slow motion kills, and impact effects.
    /// </summary>
    public class CombatFeedbackManager : MonoBehaviour
    {
        public static CombatFeedbackManager Instance { get; private set; }

        [Header("Hit Stop")]
        [SerializeField] private bool enableHitStop = true;
        [SerializeField] private float hitStopDuration = 0.05f;
        [SerializeField] private float criticalHitStopDuration = 0.1f;

        [Header("Slow Motion")]
        [SerializeField] private bool enableSlowMotionKills = true;
        [SerializeField] private float slowMotionScale = 0.3f;
        [SerializeField] private float slowMotionDuration = 0.4f;

        [Header("Attack Trails")]
        [SerializeField] private bool enableAttackTrails = true;
        [SerializeField] private Color meleeTrailColor = new Color(1f, 1f, 1f, 0.8f);
        [SerializeField] private Color magicTrailColor = new Color(0.5f, 0.8f, 1f, 0.8f);

        [Header("Impact Effects")]
        [SerializeField] private bool enableImpactEffects = true;
        [SerializeField] private float impactRingMaxSize = 2f;
        [SerializeField] private float impactRingDuration = 0.3f;

        [Header("Combo Effects")]
        [SerializeField] private bool enableComboEffects = true;
        [SerializeField] private Color[] comboColors = new Color[]
        {
            Color.white,
            Color.yellow,
            new Color(1f, 0.5f, 0f), // Orange
            Color.red,
            new Color(1f, 0f, 1f) // Magenta for high combos
        };

        private bool _isInHitStop;
        private bool _isInSlowMotion;
        private float _originalTimeScale = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        #region Hit Stop

        /// <summary>
        /// Triggers a brief freeze-frame effect when hitting an enemy.
        /// DISABLED - was causing permanent freeze issues
        /// </summary>
        public void TriggerHitStop(bool isCritical = false)
        {
            // DISABLED - Time.timeScale = 0 was causing permanent freezes
            // The game continues without hit stop effect
            Debug.Log("[CombatFeedbackManager] HitStop disabled to prevent freeze");
            return;

            /*
            if (!enableHitStop || _isInHitStop) return;

            float duration = isCritical ? criticalHitStopDuration : hitStopDuration;
            StartCoroutine(HitStopRoutine(duration));
            */
        }

        private IEnumerator HitStopRoutine(float duration)
        {
            // DISABLED - was causing permanent freezes
            yield break;

            /*
            _isInHitStop = true;
            _originalTimeScale = Time.timeScale;

            Time.timeScale = 0f;

            yield return new WaitForSecondsRealtime(duration);

            if (!_isInSlowMotion)
            {
                Time.timeScale = _originalTimeScale;
            }
            _isInHitStop = false;
            */
        }

        #endregion

        #region Slow Motion

        /// <summary>
        /// Triggers slow motion effect for dramatic kills.
        /// DISABLED - was causing timeScale issues
        /// </summary>
        public void TriggerSlowMotionKill()
        {
            // DISABLED - Time.timeScale modifications were causing freeze issues
            Debug.Log("[CombatFeedbackManager] SlowMotion disabled to prevent freeze");
            return;

            /*
            if (!enableSlowMotionKills || _isInSlowMotion) return;

            StartCoroutine(SlowMotionRoutine());
            */
        }

        private IEnumerator SlowMotionRoutine()
        {
            // DISABLED - was causing permanent freezes
            yield break;

            /*
            _isInSlowMotion = true;
            _originalTimeScale = Time.timeScale;

            Time.timeScale = slowMotionScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            yield return new WaitForSecondsRealtime(slowMotionDuration);

            // Gradually return to normal
            float elapsed = 0f;
            float transitionDuration = 0.2f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / transitionDuration;
                Time.timeScale = Mathf.Lerp(slowMotionScale, 1f, t);
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                yield return null;
            }

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            _isInSlowMotion = false;
            */
        }

        #endregion

        #region Attack Trails

        /// <summary>
        /// Creates an arc trail for melee attacks.
        /// </summary>
        public void CreateMeleeSwingTrail(Vector3 origin, Vector2 direction, float radius = 1f)
        {
            if (!enableAttackTrails) return;

            StartCoroutine(MeleeSwingTrailRoutine(origin, direction, radius));
        }

        private IEnumerator MeleeSwingTrailRoutine(Vector3 origin, Vector2 direction, float radius)
        {
            int segments = 8;
            float arcAngle = 120f; // Degrees
            float startAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - arcAngle / 2f;

            var trailObj = new GameObject("MeleeTrail");
            trailObj.transform.position = origin;

            // Create line renderer for the arc
            var lineRenderer = trailObj.AddComponent<LineRenderer>();
            lineRenderer.positionCount = segments + 1;
            lineRenderer.startWidth = 0.15f;
            lineRenderer.endWidth = 0.05f;
            lineRenderer.startColor = meleeTrailColor;
            lineRenderer.endColor = new Color(meleeTrailColor.r, meleeTrailColor.g, meleeTrailColor.b, 0f);
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.sortingLayerName = "Projectiles";
            lineRenderer.sortingOrder = 50;

            // Animate the arc appearing
            float elapsed = 0f;
            float duration = 0.15f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                int visibleSegments = Mathf.CeilToInt(t * segments);
                lineRenderer.positionCount = visibleSegments + 1;

                for (int i = 0; i <= visibleSegments && i <= segments; i++)
                {
                    float segmentT = (float)i / segments;
                    float angle = (startAngle + segmentT * arcAngle) * Mathf.Deg2Rad;
                    Vector3 pos = origin + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
                    lineRenderer.SetPosition(i, pos);
                }

                yield return null;
            }

            // Set final positions
            for (int i = 0; i <= segments; i++)
            {
                float segmentT = (float)i / segments;
                float angle = (startAngle + segmentT * arcAngle) * Mathf.Deg2Rad;
                Vector3 pos = origin + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
                lineRenderer.SetPosition(i, pos);
            }

            // Fade out
            elapsed = 0f;
            float fadeTime = 0.1f;
            Color startColor = meleeTrailColor;

            while (elapsed < fadeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / fadeTime;

                Color c = startColor;
                c.a = (1f - t) * startColor.a;
                lineRenderer.startColor = c;
                lineRenderer.endColor = new Color(c.r, c.g, c.b, 0f);

                yield return null;
            }

            Destroy(trailObj);
        }

        /// <summary>
        /// Creates a projectile trail effect.
        /// </summary>
        public void CreateProjectileTrail(GameObject projectile, Color color)
        {
            if (!enableAttackTrails || projectile == null) return;

            var trail = projectile.AddComponent<TrailRenderer>();
            trail.time = 0.2f;
            trail.startWidth = 0.15f;
            trail.endWidth = 0.02f;
            trail.startColor = color;
            trail.endColor = new Color(color.r, color.g, color.b, 0f);
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.sortingLayerName = "Projectiles";
            trail.sortingOrder = 49;
        }

        #endregion

        #region Impact Effects

        /// <summary>
        /// Creates an impact ring effect at the hit location.
        /// </summary>
        public void CreateImpactRing(Vector3 position, Color color, float size = -1f)
        {
            if (!enableImpactEffects) return;

            float finalSize = size > 0 ? size : impactRingMaxSize;
            StartCoroutine(ImpactRingRoutine(position, color, finalSize));
        }

        private IEnumerator ImpactRingRoutine(Vector3 position, Color color, float maxSize)
        {
            var ringObj = new GameObject("ImpactRing");
            ringObj.transform.position = position;

            var sr = ringObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateRingSprite();
            sr.color = color;
            sr.sortingLayerName = "Projectiles";
            sr.sortingOrder = 45;

            float elapsed = 0f;

            while (elapsed < impactRingDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / impactRingDuration;

                // Expand ring
                float scale = Mathf.Lerp(0.1f, maxSize, t);
                ringObj.transform.localScale = Vector3.one * scale;

                // Fade out
                Color c = color;
                c.a = (1f - t) * color.a;
                sr.color = c;

                yield return null;
            }

            Destroy(ringObj);
        }

        private Sprite CreateRingSprite()
        {
            int size = 32;
            var tex = new Texture2D(size, size);
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float centerX = size / 2f;
                    float centerY = size / 2f;
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    float normalized = dist / (size / 2f);

                    // Ring shape: visible between 0.7 and 1.0
                    if (normalized > 0.7f && normalized < 1f)
                    {
                        float alpha = 1f - Mathf.Abs(normalized - 0.85f) / 0.15f;
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        /// <summary>
        /// Creates directional impact lines.
        /// </summary>
        public void CreateImpactLines(Vector3 position, Vector2 direction, Color color, int lineCount = 4)
        {
            if (!enableImpactEffects) return;

            StartCoroutine(ImpactLinesRoutine(position, direction, color, lineCount));
        }

        private IEnumerator ImpactLinesRoutine(Vector3 position, Vector2 direction, Color color, int lineCount)
        {
            var lines = new GameObject[lineCount];
            var velocities = new Vector2[lineCount];

            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            for (int i = 0; i < lineCount; i++)
            {
                var lineObj = new GameObject($"ImpactLine_{i}");
                lineObj.transform.position = position;

                var sr = lineObj.AddComponent<SpriteRenderer>();
                sr.sprite = CreateLineSprite();
                sr.color = color;
                sr.sortingLayerName = "Projectiles";
                sr.sortingOrder = 46;

                // Spread lines in a cone
                float angleOffset = (i - lineCount / 2f) * 15f;
                float angle = baseAngle + angleOffset + Random.Range(-5f, 5f);

                lineObj.transform.rotation = Quaternion.Euler(0, 0, angle);
                lineObj.transform.localScale = new Vector3(0.5f + Random.Range(0f, 0.3f), 0.1f, 1f);

                float speed = 5f + Random.Range(0f, 3f);
                velocities[i] = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                ) * speed;

                lines[i] = lineObj;
            }

            float elapsed = 0f;
            float duration = 0.2f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                for (int i = 0; i < lineCount; i++)
                {
                    if (lines[i] != null)
                    {
                        lines[i].transform.position += (Vector3)(velocities[i] * Time.unscaledDeltaTime);

                        var sr = lines[i].GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            Color c = color;
                            c.a = (1f - t) * color.a;
                            sr.color = c;
                        }
                    }
                }

                yield return null;
            }

            foreach (var line in lines)
            {
                if (line != null) Destroy(line);
            }
        }

        private Sprite CreateLineSprite()
        {
            int width = 16;
            int height = 4;
            var tex = new Texture2D(width, height);
            tex.filterMode = FilterMode.Bilinear;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float t = (float)x / width;
                    float alpha = 1f - t; // Fade from left to right
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, width, height), new Vector2(0, 0.5f), 16);
        }

        #endregion

        #region Combo Effects

        /// <summary>
        /// Shows combo counter with appropriate styling.
        /// </summary>
        public void ShowComboCounter(Vector3 position, int comboCount)
        {
            if (!enableComboEffects) return;

            StartCoroutine(ComboCounterRoutine(position, comboCount));
        }

        private IEnumerator ComboCounterRoutine(Vector3 position, int comboCount)
        {
            var comboObj = new GameObject("ComboCounter");
            comboObj.transform.position = position + new Vector3(0, 1.5f, 0);

            var tmp = comboObj.AddComponent<TextMeshPro>();
            tmp.text = $"{comboCount} HIT!";
            tmp.fontSize = 4f + Mathf.Min(comboCount * 0.5f, 4f); // Scale with combo
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.sortingOrder = 1001;

            // Get combo color
            int colorIndex = Mathf.Min(comboCount - 1, comboColors.Length - 1);
            tmp.color = comboColors[Mathf.Max(0, colorIndex)];

            // Punch scale animation
            float elapsed = 0f;
            float duration = 0.8f;
            Vector3 startScale = Vector3.one * 1.5f;
            Vector3 endScale = Vector3.one;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Bounce in
                if (t < 0.2f)
                {
                    float scaleT = t / 0.2f;
                    comboObj.transform.localScale = Vector3.Lerp(startScale, endScale, scaleT);
                }

                // Rise and fade
                comboObj.transform.position = position + new Vector3(0, 1.5f + t * 0.5f, 0);
                tmp.alpha = 1f - (t * t);

                yield return null;
            }

            Destroy(comboObj);
        }

        /// <summary>
        /// Creates a combo streak effect.
        /// </summary>
        public void CreateComboStreak(Vector3 position, int comboCount)
        {
            if (!enableComboEffects || comboCount < 3) return;

            int colorIndex = Mathf.Min(comboCount - 1, comboColors.Length - 1);
            Color color = comboColors[Mathf.Max(0, colorIndex)];

            // Create radial particle burst
            if (VisualEffectsManager.Instance != null)
            {
                VisualEffectsManager.Instance.SpawnParticleBurst(position, color, 5 + comboCount * 2);
            }

            // Create impact ring
            CreateImpactRing(position, color, 1f + comboCount * 0.2f);
        }

        #endregion

        #region Utility

        /// <summary>
        /// Full impact effect combining multiple effects.
        /// </summary>
        public void FullImpactEffect(Vector3 position, Vector2 direction, int damage, bool isKill = false)
        {
            // Hit stop
            TriggerHitStop(damage >= 5);

            // Visual effects
            Color impactColor = isKill ? Color.red : Color.white;
            CreateImpactRing(position, impactColor, isKill ? 2f : 1f);
            CreateImpactLines(position, direction, impactColor, isKill ? 6 : 3);

            // Slow motion for kills
            if (isKill && enableSlowMotionKills)
            {
                TriggerSlowMotionKill();
            }
        }

        #endregion

        /// <summary>
        /// Creates the CombatFeedbackManager if it doesn't exist.
        /// </summary>
        public static CombatFeedbackManager EnsureExists()
        {
            if (Instance == null)
            {
                var obj = new GameObject("CombatFeedbackManager");
                Instance = obj.AddComponent<CombatFeedbackManager>();
                DontDestroyOnLoad(obj);
            }
            return Instance;
        }
    }
}
