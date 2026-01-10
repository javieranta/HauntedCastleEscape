using UnityEngine;

namespace HauntedCastle.Core
{
    /// <summary>
    /// Ensures all required layers and physics settings are configured at runtime.
    /// Runs before any other script to set up layer masks properly.
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class LayerSetup : MonoBehaviour
    {
        private static bool _initialized = false;

        // Layer indices - Unity has 32 layers, user layers start at 8
        public const int PLAYER_LAYER = 8;
        public const int ENEMIES_LAYER = 9;
        public const int PROJECTILES_LAYER = 10;
        public const int ITEMS_LAYER = 11;
        public const int WALLS_LAYER = 12;
        public const int DOORS_LAYER = 13;
        public const int TRIGGERS_LAYER = 14;

        // Layer masks for quick access
        public static int PlayerLayerMask => 1 << PLAYER_LAYER;
        public static int EnemiesLayerMask => 1 << ENEMIES_LAYER;
        public static int ProjectilesLayerMask => 1 << PROJECTILES_LAYER;
        public static int ItemsLayerMask => 1 << ITEMS_LAYER;
        public static int WallsLayerMask => 1 << WALLS_LAYER;

        // Combined masks
        public static int DamageableByPlayerMask => EnemiesLayerMask;
        public static int DamageableByEnemyMask => PlayerLayerMask;

        private void Awake()
        {
            if (_initialized) return;
            _initialized = true;

            ConfigurePhysics();
            Debug.Log("[LayerSetup] Layer configuration complete");
        }

        private void ConfigurePhysics()
        {
            // Configure 2D physics collision matrix
            // Player projectiles should hit enemies
            Physics2D.IgnoreLayerCollision(PROJECTILES_LAYER, PLAYER_LAYER, true);
            Physics2D.IgnoreLayerCollision(PROJECTILES_LAYER, PROJECTILES_LAYER, true);

            // Items don't collide with walls or enemies
            Physics2D.IgnoreLayerCollision(ITEMS_LAYER, WALLS_LAYER, true);
            Physics2D.IgnoreLayerCollision(ITEMS_LAYER, ENEMIES_LAYER, true);

            // Triggers don't physically collide
            Physics2D.IgnoreLayerCollision(TRIGGERS_LAYER, PLAYER_LAYER, false);
            Physics2D.IgnoreLayerCollision(TRIGGERS_LAYER, ENEMIES_LAYER, true);
        }

        /// <summary>
        /// Gets the correct layer for an object type.
        /// </summary>
        public static int GetLayer(ObjectType type)
        {
            return type switch
            {
                ObjectType.Player => PLAYER_LAYER,
                ObjectType.Enemy => ENEMIES_LAYER,
                ObjectType.Projectile => PROJECTILES_LAYER,
                ObjectType.Item => ITEMS_LAYER,
                ObjectType.Wall => WALLS_LAYER,
                ObjectType.Door => DOORS_LAYER,
                ObjectType.Trigger => TRIGGERS_LAYER,
                _ => 0
            };
        }

        /// <summary>
        /// Sets up an object's layer based on its type.
        /// </summary>
        public static void SetupObject(GameObject obj, ObjectType type)
        {
            int layer = GetLayer(type);
            if (layer > 0)
            {
                obj.layer = layer;
            }
        }

        public enum ObjectType
        {
            Player,
            Enemy,
            Projectile,
            Item,
            Wall,
            Door,
            Trigger
        }

        /// <summary>
        /// Ensures LayerSetup exists in the scene.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureExists()
        {
            if (_initialized) return;

            var obj = new GameObject("LayerSetup");
            obj.AddComponent<LayerSetup>();
            DontDestroyOnLoad(obj);
        }
    }
}
