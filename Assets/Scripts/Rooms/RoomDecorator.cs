using UnityEngine;
using System.Collections.Generic;
using HauntedCastle.Data;
using HauntedCastle.Utils;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Spawns floor-specific decorations in rooms.
    /// Basement: chains, cobwebs, skulls, barrels (dungeon theme)
    /// Castle: chandeliers, paintings, furniture (living quarters theme)
    /// Tower: windows, banners, armor stands (battlements theme)
    /// </summary>
    public class RoomDecorator : MonoBehaviour
    {
        [Header("Decoration Settings")]
        [SerializeField] private int minDecorations = 4;
        [SerializeField] private int maxDecorations = 8;
        [SerializeField] private float decorationScale = 1f;

        [Header("Room Bounds")]
        [SerializeField] private float roomWidth = 14f;
        [SerializeField] private float roomHeight = 10f;
        [SerializeField] private float wallMargin = 1.5f;

        private int _floorLevel;
        private Transform _decorationContainer;
        private List<Vector2> _usedPositions = new();
        private System.Random _rng;

        public void Initialize(RoomData roomData)
        {
            _floorLevel = roomData?.floorNumber ?? 1;

            // Use room ID as seed for consistent decoration placement
            int seed = roomData?.roomId?.GetHashCode() ?? Random.Range(0, 10000);
            _rng = new System.Random(seed);

            CreateDecorationContainer();
            SpawnFloorDecorations();
        }

        private void CreateDecorationContainer()
        {
            _decorationContainer = new GameObject("Decorations").transform;
            _decorationContainer.SetParent(transform);
            _decorationContainer.localPosition = Vector3.zero;
        }

        private void SpawnFloorDecorations()
        {
            int decorationCount = _rng.Next(minDecorations, maxDecorations + 1);

            switch (_floorLevel)
            {
                case 0: // Basement - dungeon theme
                    SpawnBasementDecorations(decorationCount);
                    break;
                case 1: // Castle - living quarters theme
                    SpawnCastleDecorations(decorationCount);
                    break;
                case 2: // Tower - battlements theme
                    SpawnTowerDecorations(decorationCount);
                    break;
                default:
                    SpawnCastleDecorations(decorationCount);
                    break;
            }

            Debug.Log($"[RoomDecorator] Spawned {decorationCount} decorations for floor {_floorLevel}");
        }

        #region Basement Decorations

        private void SpawnBasementDecorations(int count)
        {
            // Corner cobwebs (always spawn these)
            SpawnCobweb(new Vector2(-roomWidth / 2 + 0.5f, roomHeight / 2 - 0.5f), false);  // NW
            SpawnCobweb(new Vector2(roomWidth / 2 - 0.5f, roomHeight / 2 - 0.5f), true);    // NE

            // Random basement decorations
            for (int i = 0; i < count; i++)
            {
                int decorType = _rng.Next(0, 4);
                Vector2 pos = GetRandomPosition();

                switch (decorType)
                {
                    case 0: // Chain on wall
                        SpawnChain(GetWallPosition(true));
                        break;
                    case 1: // Skull on ground
                        SpawnSkull(GetGroundPosition());
                        break;
                    case 2: // Barrel
                        SpawnBarrel(GetCornerPosition());
                        break;
                    case 3: // Additional cobweb
                        SpawnCobweb(GetCornerPosition(), _rng.Next(0, 2) == 0);
                        break;
                }
            }
        }

        private void SpawnCobweb(Vector2 position, bool flipX)
        {
            var obj = CreateDecorationObject("Cobweb", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetCobwebSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 3;
            sr.flipX = flipX;
            obj.transform.localScale = Vector3.one * decorationScale * 1.5f;
        }

        private void SpawnChain(Vector2 position)
        {
            var obj = CreateDecorationObject("Chain", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetChainSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 2;
            obj.transform.localScale = Vector3.one * decorationScale;
        }

        private void SpawnSkull(Vector2 position)
        {
            var obj = CreateDecorationObject("Skull", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetSkullSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 1;
            obj.transform.localScale = Vector3.one * decorationScale * 0.7f;

            // Random rotation for variety
            obj.transform.rotation = Quaternion.Euler(0, 0, _rng.Next(-30, 30));
        }

        private void SpawnBarrel(Vector2 position)
        {
            var obj = CreateDecorationObject("Barrel", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetBarrelSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 2;
            obj.transform.localScale = Vector3.one * decorationScale;

            // Add simple collider for obstacle
            var collider = obj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1.2f, 1.5f);
        }

        #endregion

        #region Castle Decorations

        private void SpawnCastleDecorations(int count)
        {
            // Always spawn a chandelier in the center-top area
            SpawnChandelier(new Vector2(0, roomHeight / 4));

            // Random castle decorations
            for (int i = 0; i < count; i++)
            {
                int decorType = _rng.Next(0, 4);

                switch (decorType)
                {
                    case 0: // Painting on wall
                        SpawnPainting(GetWallPosition(false));
                        break;
                    case 1: // Another painting
                        SpawnPainting(GetWallPosition(false));
                        break;
                    case 2: // Furniture (barrel reused as crate)
                        SpawnBarrel(GetCornerPosition());
                        break;
                    case 3: // Banner
                        SpawnBanner(GetWallPosition(true));
                        break;
                }
            }
        }

        private void SpawnChandelier(Vector2 position)
        {
            var obj = CreateDecorationObject("Chandelier", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetChandelierSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 10;
            obj.transform.localScale = Vector3.one * decorationScale * 1.5f;

            // Add light glow effect (URP Light2D if available)
            TryAddLight2D(obj, new Color(1f, 0.9f, 0.7f), 1.5f, 5f);
        }

        private void SpawnPainting(Vector2 position)
        {
            var obj = CreateDecorationObject("Painting", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetPaintingSprite(_rng.Next(0, 3));
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 3;
            obj.transform.localScale = Vector3.one * decorationScale;
        }

        private void SpawnBanner(Vector2 position)
        {
            var obj = CreateDecorationObject("Banner", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetBannerSprite(_rng.Next(0, 4));
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 3;
            obj.transform.localScale = Vector3.one * decorationScale;
        }

        #endregion

        #region Tower Decorations

        private void SpawnTowerDecorations(int count)
        {
            // Always spawn a window
            SpawnWindow(GetWallPosition(true));

            // Spawn armor stands in corners
            if (_rng.Next(0, 2) == 0)
            {
                SpawnArmorStand(new Vector2(-roomWidth / 2 + 2, -roomHeight / 2 + 2));
            }
            if (_rng.Next(0, 2) == 0)
            {
                SpawnArmorStand(new Vector2(roomWidth / 2 - 2, -roomHeight / 2 + 2));
            }

            // Random tower decorations
            for (int i = 0; i < count; i++)
            {
                int decorType = _rng.Next(0, 3);

                switch (decorType)
                {
                    case 0: // Banner
                        SpawnBanner(GetWallPosition(true));
                        break;
                    case 1: // Window
                        SpawnWindow(GetWallPosition(true));
                        break;
                    case 2: // Additional armor stand
                        SpawnArmorStand(GetCornerPosition());
                        break;
                }
            }
        }

        private void SpawnWindow(Vector2 position)
        {
            var obj = CreateDecorationObject("Window", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetWindowSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 2;
            obj.transform.localScale = Vector3.one * decorationScale * 1.2f;

            // Add light from window (URP Light2D if available)
            TryAddLight2D(obj, new Color(0.8f, 0.85f, 1f), 0.8f, 3f, new Vector3(0, -1f, 0));
        }

        private void SpawnArmorStand(Vector2 position)
        {
            var obj = CreateDecorationObject("ArmorStand", position);
            var sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetArmorStandSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 4;
            obj.transform.localScale = Vector3.one * decorationScale;

            // Add collider for obstacle
            var collider = obj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1f, 2f);
        }

        #endregion

        #region Position Helpers

        private GameObject CreateDecorationObject(string name, Vector2 position)
        {
            var obj = new GameObject(name);
            obj.transform.SetParent(_decorationContainer);
            obj.transform.localPosition = position;
            _usedPositions.Add(position);
            return obj;
        }

        private Vector2 GetRandomPosition()
        {
            float margin = wallMargin;
            float x = (float)(_rng.NextDouble() * (roomWidth - margin * 2) - (roomWidth / 2 - margin));
            float y = (float)(_rng.NextDouble() * (roomHeight - margin * 2) - (roomHeight / 2 - margin));
            return new Vector2(x, y);
        }

        private Vector2 GetGroundPosition()
        {
            // Bottom area of the room
            float x = (float)(_rng.NextDouble() * (roomWidth - wallMargin * 2) - (roomWidth / 2 - wallMargin));
            float y = -roomHeight / 2 + wallMargin + (float)_rng.NextDouble() * 2;
            return new Vector2(x, y);
        }

        private Vector2 GetWallPosition(bool vertical)
        {
            if (vertical)
            {
                // Left or right wall - position ON the wall (at the edge, not inside room)
                float x = _rng.Next(0, 2) == 0 ? -roomWidth / 2 + 0.3f : roomWidth / 2 - 0.3f;
                // Place in a narrow band: away from door (center) but not in corners
                // Range: y = +2 to +3 (top) or y = -2 to -3 (bottom)
                bool topHalf = _rng.Next(0, 2) == 0;
                float y = topHalf
                    ? 2f + (float)(_rng.NextDouble() * 1.5f)    // Top band: +2 to +3.5
                    : -2f - (float)(_rng.NextDouble() * 1.5f);  // Bottom band: -2 to -3.5
                return new Vector2(x, y);
            }
            else
            {
                // Top or bottom wall - avoid center area where doors are, but not corners
                // Range: x = +3 to +5 (right) or x = -3 to -5 (left)
                bool leftHalf = _rng.Next(0, 2) == 0;
                float x = leftHalf
                    ? -3f - (float)(_rng.NextDouble() * 2f)     // Left band: -3 to -5
                    : 3f + (float)(_rng.NextDouble() * 2f);     // Right band: +3 to +5
                float y = _rng.Next(0, 2) == 0 ? roomHeight / 2 - 0.3f : -roomHeight / 2 + 0.3f;
                return new Vector2(x, y);
            }
        }

        private Vector2 GetCornerPosition()
        {
            int corner = _rng.Next(0, 4);
            float margin = wallMargin + 1f;

            return corner switch
            {
                0 => new Vector2(-roomWidth / 2 + margin, roomHeight / 2 - margin),      // NW
                1 => new Vector2(roomWidth / 2 - margin, roomHeight / 2 - margin),       // NE
                2 => new Vector2(-roomWidth / 2 + margin, -roomHeight / 2 + margin),     // SW
                3 => new Vector2(roomWidth / 2 - margin, -roomHeight / 2 + margin),      // SE
                _ => Vector2.zero
            };
        }

        #endregion

        /// <summary>
        /// Attempts to add a URP Light2D component if available.
        /// Falls back gracefully if URP is not installed.
        /// </summary>
        private void TryAddLight2D(GameObject parent, Color color, float intensity, float radius, Vector3? offset = null)
        {
            try
            {
                var lightObj = new GameObject("Light");
                lightObj.transform.SetParent(parent.transform);
                lightObj.transform.localPosition = offset ?? Vector3.zero;

                // Try to add Light2D using reflection to avoid compile errors if URP not available
                var light2DType = System.Type.GetType("UnityEngine.Rendering.Universal.Light2D, Unity.RenderPipelines.Universal.Runtime");
                if (light2DType != null)
                {
                    var lightComponent = lightObj.AddComponent(light2DType);
                    if (lightComponent != null)
                    {
                        // Set light type to Point (enum value 2)
                        var lightTypeProp = light2DType.GetProperty("lightType");
                        if (lightTypeProp != null)
                        {
                            var lightTypeEnum = System.Enum.ToObject(lightTypeProp.PropertyType, 2);
                            lightTypeProp.SetValue(lightComponent, lightTypeEnum);
                        }

                        // Set intensity
                        var intensityProp = light2DType.GetProperty("intensity");
                        intensityProp?.SetValue(lightComponent, intensity);

                        // Set outer radius
                        var outerRadiusProp = light2DType.GetProperty("pointLightOuterRadius");
                        outerRadiusProp?.SetValue(lightComponent, radius);

                        // Set color
                        var colorProp = light2DType.GetProperty("color");
                        colorProp?.SetValue(lightComponent, color);
                    }
                }
                else
                {
                    // URP not available, just destroy the light object
                    Destroy(lightObj);
                }
            }
            catch (System.Exception)
            {
                // If anything fails, just continue without the light
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Visualize decoration positions in editor
            Gizmos.color = Color.cyan;
            foreach (var pos in _usedPositions)
            {
                Gizmos.DrawWireSphere(transform.position + (Vector3)pos, 0.3f);
            }

            // Room bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(roomWidth, roomHeight, 0));
        }
    }
}
