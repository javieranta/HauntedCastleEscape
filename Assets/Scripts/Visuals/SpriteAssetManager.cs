using UnityEngine;
using System.Collections.Generic;
using HauntedCastle.Data;
using HauntedCastle.Services;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Manages loading and caching of sprite assets.
    /// Place sprites in Assets/Resources/Sprites/ folder structure.
    /// Falls back to procedural sprites if assets not found.
    /// </summary>
    public class SpriteAssetManager : MonoBehaviour
    {
        public static SpriteAssetManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private bool useProceduralFallback = true;
        [SerializeField] private bool logMissingSprites = true;

        // Sprite caches
        private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
        private Dictionary<string, Sprite[]> _animationCache = new Dictionary<string, Sprite[]>();

        // Paths
        private const string CHARACTERS_PATH = "Sprites/Characters/";
        private const string ENEMIES_PATH = "Sprites/Enemies/";
        private const string ITEMS_PATH = "Sprites/Items/";
        private const string ENVIRONMENT_PATH = "Sprites/Environment/";
        private const string EFFECTS_PATH = "Sprites/Effects/";
        private const string UI_PATH = "Sprites/UI/";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #region Character Sprites

        /// <summary>
        /// Gets the sprite for a player character.
        /// </summary>
        public Sprite GetCharacterSprite(CharacterType characterType, string animationState = "idle")
        {
            string characterName = characterType.ToString().ToLower();
            string path = $"{CHARACTERS_PATH}{characterName}/{characterName}_{animationState}";

            return LoadSprite(path, () => GetProceduralCharacterSprite(characterType));
        }

        /// <summary>
        /// Gets animation frames for a character.
        /// </summary>
        public Sprite[] GetCharacterAnimation(CharacterType characterType, string animationName)
        {
            string characterName = characterType.ToString().ToLower();
            string key = $"{characterName}_{animationName}";

            if (_animationCache.TryGetValue(key, out Sprite[] cached))
            {
                return cached;
            }

            List<Sprite> frames = new List<Sprite>();
            for (int i = 0; i < 10; i++) // Max 10 frames
            {
                string path = $"{CHARACTERS_PATH}{characterName}/{characterName}_{animationName}{i}";
                Sprite frame = Resources.Load<Sprite>(path);
                if (frame != null)
                {
                    frames.Add(frame);
                }
                else if (i == 0)
                {
                    // Try without number for single frame
                    path = $"{CHARACTERS_PATH}{characterName}/{characterName}_{animationName}";
                    frame = Resources.Load<Sprite>(path);
                    if (frame != null) frames.Add(frame);
                    break;
                }
                else
                {
                    break;
                }
            }

            Sprite[] result = frames.Count > 0 ? frames.ToArray() : null;
            _animationCache[key] = result;
            return result;
        }

        #endregion

        #region Enemy Sprites

        /// <summary>
        /// Gets the sprite for an enemy type.
        /// </summary>
        public Sprite GetEnemySprite(EnemyType enemyType, int frame = 0)
        {
            string enemyName = enemyType.ToString().ToLower();
            string path = frame == 0
                ? $"{ENEMIES_PATH}{enemyName}"
                : $"{ENEMIES_PATH}{enemyName}_{frame}";

            return LoadSprite(path, () => Utils.PlaceholderSpriteGenerator.GetEnemySprite(enemyType));
        }

        /// <summary>
        /// Gets animation frames for an enemy.
        /// </summary>
        public Sprite[] GetEnemyAnimation(EnemyType enemyType)
        {
            string enemyName = enemyType.ToString().ToLower();
            string key = $"enemy_{enemyName}";

            if (_animationCache.TryGetValue(key, out Sprite[] cached))
            {
                return cached;
            }

            List<Sprite> frames = new List<Sprite>();

            // Try loading numbered frames
            for (int i = 0; i < 10; i++)
            {
                string path = $"{ENEMIES_PATH}{enemyName}_{i}";
                Sprite frame = Resources.Load<Sprite>(path);
                if (frame != null)
                {
                    frames.Add(frame);
                }
                else if (i > 0)
                {
                    break;
                }
            }

            // If no numbered frames, try single sprite
            if (frames.Count == 0)
            {
                Sprite single = Resources.Load<Sprite>($"{ENEMIES_PATH}{enemyName}");
                if (single != null) frames.Add(single);
            }

            Sprite[] result = frames.Count > 0 ? frames.ToArray() : null;
            _animationCache[key] = result;
            return result;
        }

        #endregion

        #region Item Sprites

        /// <summary>
        /// Gets sprite for an item.
        /// </summary>
        public Sprite GetItemSprite(string itemName)
        {
            string path = $"{ITEMS_PATH}{itemName.ToLower().Replace(" ", "_")}";
            return LoadSprite(path, () => Utils.PlaceholderSpriteGenerator.GetFoodSprite());
        }

        /// <summary>
        /// Gets sprite for a key by color.
        /// </summary>
        public Sprite GetKeySprite(Data.KeyColor color)
        {
            string colorName = color.ToString().ToLower();
            string path = $"{ITEMS_PATH}Keys/key_{colorName}";
            return LoadSprite(path, () => Utils.PlaceholderSpriteGenerator.GetKeySprite(color));
        }

        /// <summary>
        /// Gets sprite for a food item.
        /// </summary>
        public Sprite GetFoodSprite(string foodName)
        {
            string path = $"{ITEMS_PATH}Food/{foodName.ToLower().Replace(" ", "_")}";
            return LoadSprite(path, () => Utils.PlaceholderSpriteGenerator.GetFoodSprite());
        }

        #endregion

        #region Environment Sprites

        /// <summary>
        /// Gets floor tile sprite.
        /// </summary>
        public Sprite GetFloorTile(int variant = 0)
        {
            string path = variant == 0
                ? $"{ENVIRONMENT_PATH}Floors/floor"
                : $"{ENVIRONMENT_PATH}Floors/floor_{variant}";
            return LoadSprite(path, () => GetProceduralFloorTile());
        }

        /// <summary>
        /// Gets wall tile sprite.
        /// </summary>
        public Sprite GetWallTile(int variant = 0)
        {
            string path = variant == 0
                ? $"{ENVIRONMENT_PATH}Walls/wall"
                : $"{ENVIRONMENT_PATH}Walls/wall_{variant}";
            return LoadSprite(path, () => GetProceduralWallTile());
        }

        /// <summary>
        /// Gets door sprite.
        /// </summary>
        public Sprite GetDoorSprite(Data.KeyColor keyColor, bool isOpen = false)
        {
            string state = isOpen ? "open" : "closed";
            string colorName = keyColor.ToString().ToLower();
            string path = $"{ENVIRONMENT_PATH}Doors/door_{colorName}_{state}";
            return LoadSprite(path, () => Utils.PlaceholderSpriteGenerator.GetDoorSprite(isOpen, colorName));
        }

        /// <summary>
        /// Gets prop sprite.
        /// </summary>
        public Sprite GetPropSprite(string propName)
        {
            string path = $"{ENVIRONMENT_PATH}Props/{propName.ToLower().Replace(" ", "_")}";
            return LoadSprite(path, null);
        }

        #endregion

        #region Effect Sprites

        /// <summary>
        /// Gets projectile sprite.
        /// </summary>
        public Sprite GetProjectileSprite(string projectileType)
        {
            string path = $"{EFFECTS_PATH}Projectiles/{projectileType.ToLower()}";
            return LoadSprite(path, null);
        }

        /// <summary>
        /// Gets impact effect animation.
        /// </summary>
        public Sprite[] GetImpactAnimation(string effectName)
        {
            string key = $"impact_{effectName}";

            if (_animationCache.TryGetValue(key, out Sprite[] cached))
            {
                return cached;
            }

            List<Sprite> frames = new List<Sprite>();
            for (int i = 0; i < 10; i++)
            {
                string path = $"{EFFECTS_PATH}Impacts/{effectName}_{i}";
                Sprite frame = Resources.Load<Sprite>(path);
                if (frame != null)
                {
                    frames.Add(frame);
                }
                else if (i > 0)
                {
                    break;
                }
            }

            Sprite[] result = frames.Count > 0 ? frames.ToArray() : null;
            _animationCache[key] = result;
            return result;
        }

        #endregion

        #region UI Sprites

        /// <summary>
        /// Gets UI element sprite.
        /// </summary>
        public Sprite GetUISprite(string elementName)
        {
            string path = $"{UI_PATH}{elementName.ToLower().Replace(" ", "_")}";
            return LoadSprite(path, null);
        }

        #endregion

        #region Helper Methods

        private Sprite LoadSprite(string path, System.Func<Sprite> fallbackGenerator)
        {
            // Check cache first
            if (_spriteCache.TryGetValue(path, out Sprite cached))
            {
                return cached;
            }

            // Try to load from Resources
            Sprite sprite = Resources.Load<Sprite>(path);

            if (sprite != null)
            {
                _spriteCache[path] = sprite;
                return sprite;
            }

            // Log missing sprite
            if (logMissingSprites)
            {
                Debug.Log($"[SpriteAssetManager] Sprite not found: {path}, using fallback");
            }

            // Use procedural fallback
            if (useProceduralFallback && fallbackGenerator != null)
            {
                sprite = fallbackGenerator();
                if (sprite != null)
                {
                    _spriteCache[path] = sprite;
                }
                return sprite;
            }

            return null;
        }

        private Sprite GetProceduralCharacterSprite(CharacterType type)
        {
            return Utils.PlaceholderSpriteGenerator.GetCharacterSprite(type);
        }

        private Sprite GetProceduralFloorTile()
        {
            // Simple grey floor tile
            var tex = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                float noise = Random.Range(0.25f, 0.35f);
                pixels[i] = new Color(noise, noise, noise);
            }
            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        }

        private Sprite GetProceduralWallTile()
        {
            // Simple dark wall tile
            var tex = new Texture2D(32, 32);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                float noise = Random.Range(0.15f, 0.25f);
                pixels[i] = new Color(noise, noise, noise);
            }
            tex.SetPixels(pixels);
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        }

        /// <summary>
        /// Clears the sprite cache.
        /// </summary>
        public void ClearCache()
        {
            _spriteCache.Clear();
            _animationCache.Clear();
        }

        /// <summary>
        /// Preloads sprites for a scene.
        /// </summary>
        public void PreloadSprites(string[] paths)
        {
            foreach (string path in paths)
            {
                LoadSprite(path, null);
            }
        }

        #endregion

        /// <summary>
        /// Creates the manager if it doesn't exist.
        /// </summary>
        public static SpriteAssetManager EnsureExists()
        {
            if (Instance == null)
            {
                var obj = new GameObject("SpriteAssetManager");
                Instance = obj.AddComponent<SpriteAssetManager>();
                DontDestroyOnLoad(obj);
            }
            return Instance;
        }
    }
}
