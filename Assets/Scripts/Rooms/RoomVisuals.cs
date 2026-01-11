using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Utils;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Handles procedural visual generation for rooms.
    /// Creates walls, floor, and decorations based on room data.
    /// Uses enhanced sprites with floor-themed visuals.
    /// </summary>
    public class RoomVisuals : MonoBehaviour
    {
        [Header("Room Dimensions")]
        [SerializeField] private float roomWidth = 14f;
        [SerializeField] private float roomHeight = 10f;
        [SerializeField] private float wallThickness = 1f;
        [SerializeField] private float tileSize = 1.5f; // Good size for Midjourney texture tiling

        [Header("Door Dimensions")]
        [SerializeField] private float doorWidth = 2f;

        [Header("Decoration")]
        [SerializeField] private bool addTorches = true;
        [SerializeField] private int torchCount = 4;

        private RoomData _roomData;
        private int _floorLevel;

        public void Initialize(RoomData roomData)
        {
            _roomData = roomData;
            _floorLevel = roomData?.floorNumber ?? 1;

            // DIAGNOSTIC: Log exactly what room and floor we're initializing
            Debug.LogWarning($"[RoomVisuals] ========== INITIALIZING ROOM ==========");
            Debug.LogWarning($"[RoomVisuals] Room ID: {roomData?.roomId ?? "NULL"}");
            Debug.LogWarning($"[RoomVisuals] Room Name: {roomData?.displayName ?? "NULL"}");
            Debug.LogWarning($"[RoomVisuals] Floor Number from RoomData: {roomData?.floorNumber}");
            Debug.LogWarning($"[RoomVisuals] _floorLevel set to: {_floorLevel}");
            Debug.LogWarning($"[RoomVisuals] This transform position: {transform.position}");
            Debug.LogWarning($"[RoomVisuals] =======================================");

            try
            {
                GenerateVisuals();
                Debug.LogWarning($"[RoomVisuals] *** GenerateVisuals completed successfully for {roomData?.roomId} ***");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[RoomVisuals] *** ERROR in GenerateVisuals for {roomData?.roomId}: {e.Message}\n{e.StackTrace}");
            }
        }

        private void GenerateVisuals()
        {
            CreateTiledFloor();
            CreateThemedWalls();

            if (addTorches)
            {
                CreateTorches();
            }

            // Add floor-specific decorations
            CreateDecorations();

            // Apply ambient color from room data
            if (_roomData != null)
            {
                ApplyAmbientTint(_roomData.ambientColor);
            }
        }

        private void CreateDecorations()
        {
            // Add RoomDecorator component and initialize it
            var decorator = gameObject.AddComponent<RoomDecorator>();
            decorator.Initialize(_roomData);
        }

        private void CreateTiledFloor()
        {
            Debug.LogWarning($"[RoomVisuals.CreateTiledFloor] Creating floor for level {_floorLevel}");

            var floorContainer = new GameObject("FloorTiles");
            floorContainer.transform.SetParent(transform);
            floorContainer.transform.localPosition = Vector3.zero;

            // Get the Midjourney floor sprite
            Debug.LogWarning($"[RoomVisuals.CreateTiledFloor] Calling GetFloorTileSprite({_floorLevel}, 0)");
            Sprite floorSprite = PlaceholderSpriteGenerator.GetFloorTileSprite(_floorLevel, 0);
            Debug.LogWarning($"[RoomVisuals.CreateTiledFloor] Got sprite: {floorSprite?.name ?? "NULL"}");

            // Create a SINGLE large floor that covers the room
            // The texture will tile naturally due to Wrap Mode = Repeat
            var floor = new GameObject("FloorSprite");
            floor.transform.SetParent(floorContainer.transform);
            floor.transform.localPosition = Vector3.zero;

            var sr = floor.AddComponent<SpriteRenderer>();
            sr.sprite = floorSprite;
            sr.sortingLayerName = "Background";
            sr.sortingOrder = -10;
            sr.color = Color.white;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;

            // Set the size to cover the room - texture will tile within this area
            sr.size = new Vector2(roomWidth, roomHeight);

            // Detailed debug logging for floor visibility issues
            Debug.Log($"[RoomVisuals] FLOOR CREATED for level {_floorLevel}:");
            Debug.Log($"  -> Sprite: {floorSprite?.name ?? "NULL"}, Size: {roomWidth}x{roomHeight}");
            Debug.Log($"  -> SpriteRenderer: drawMode={sr.drawMode}, size={sr.size}, layer={sr.sortingLayerName}/{sr.sortingOrder}");
            Debug.Log($"  -> Floor position: {floor.transform.position}, parent: {floorContainer.transform.parent?.name}");
            if (floorSprite != null && floorSprite.texture != null)
            {
                try
                {
                    Color[] pixels = floorSprite.texture.GetPixels(0, 0, 1, 1);
                    if (pixels.Length > 0)
                    {
                        Debug.Log($"  -> Sample pixel color: R={pixels[0].r:F2} G={pixels[0].g:F2} B={pixels[0].b:F2}");
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"  -> Could not read texture pixels: {ex.Message}");
                }
            }

        }

        private void CreateThemedWalls()
        {
            Color wallColor = GetFloorWallColor();
            // BRIGHTENED doorway color - was almost black (0.08, 0.06, 0.04)
            Color doorwayColor = new Color(0.35f, 0.30f, 0.32f);

            // North wall
            CreateWall("WallNorth",
                new Vector2(0, roomHeight / 2),
                new Vector2(roomWidth, wallThickness),
                _roomData?.northDoor?.exists ?? false,
                false, wallColor, doorwayColor);

            // South wall
            CreateWall("WallSouth",
                new Vector2(0, -roomHeight / 2),
                new Vector2(roomWidth, wallThickness),
                _roomData?.southDoor?.exists ?? false,
                false, wallColor, doorwayColor);

            // East wall
            CreateWall("WallEast",
                new Vector2(roomWidth / 2, 0),
                new Vector2(wallThickness, roomHeight),
                _roomData?.eastDoor?.exists ?? false,
                true, wallColor, doorwayColor);

            // West wall
            CreateWall("WallWest",
                new Vector2(-roomWidth / 2, 0),
                new Vector2(wallThickness, roomHeight),
                _roomData?.westDoor?.exists ?? false,
                true, wallColor, doorwayColor);

            // Corners
            CreateCorner("CornerNE", new Vector2(roomWidth / 2, roomHeight / 2), wallColor);
            CreateCorner("CornerNW", new Vector2(-roomWidth / 2, roomHeight / 2), wallColor);
            CreateCorner("CornerSE", new Vector2(roomWidth / 2, -roomHeight / 2), wallColor);
            CreateCorner("CornerSW", new Vector2(-roomWidth / 2, -roomHeight / 2), wallColor);
        }

        private Color GetFloorWallColor()
        {
            // BRIGHTENED for visibility - walls should be visible even in basement
            return _floorLevel switch
            {
                0 => new Color(0.50f, 0.45f, 0.48f),   // Basement - BRIGHTENED purple-gray
                1 => new Color(0.55f, 0.50f, 0.45f),  // Castle - medium warm stone
                2 => new Color(0.60f, 0.58f, 0.55f),  // Tower - lighter stone
                _ => new Color(0.52f, 0.48f, 0.45f)
            };
        }

        private void CreateTorches()
        {
            var torchContainer = new GameObject("Torches");
            torchContainer.transform.SetParent(transform);
            torchContainer.transform.localPosition = Vector3.zero;

            // Place torches along walls
            Vector2[] torchPositions = new Vector2[]
            {
                new Vector2(-roomWidth / 2 + 1, roomHeight / 4),
                new Vector2(-roomWidth / 2 + 1, -roomHeight / 4),
                new Vector2(roomWidth / 2 - 1, roomHeight / 4),
                new Vector2(roomWidth / 2 - 1, -roomHeight / 4)
            };

            for (int i = 0; i < Mathf.Min(torchCount, torchPositions.Length); i++)
            {
                var torchObj = new GameObject($"Torch_{i}");
                torchObj.transform.SetParent(torchContainer.transform);
                torchObj.transform.localPosition = torchPositions[i];

                var sr = torchObj.AddComponent<SpriteRenderer>();
                sr.sprite = PlaceholderSpriteGenerator.GetTorchSprite(i);
                sr.sortingLayerName = "Items";
                sr.sortingOrder = 5;

                // Add animated torch component
                var animTorch = torchObj.AddComponent<AnimatedTorch>();
                animTorch.Initialize(i);
            }
        }

        private void ApplyAmbientTint(Color ambientColor)
        {
            // Don't apply ambient tint - preserve Midjourney texture colors
            // If needed, ambient can be done via lighting instead
        }

        private void CreateWall(string name, Vector2 position, Vector2 size, bool hasDoor, bool isVertical, Color wallColor, Color doorwayColor)
        {
            if (hasDoor)
            {
                // Create wall with doorway gap
                if (isVertical)
                {
                    // Top part
                    CreateWallSegment(name + "_Top",
                        position + new Vector2(0, (size.y / 4) + (doorWidth / 4)),
                        new Vector2(size.x, (size.y - doorWidth) / 2), wallColor);

                    // Bottom part
                    CreateWallSegment(name + "_Bottom",
                        position - new Vector2(0, (size.y / 4) + (doorWidth / 4)),
                        new Vector2(size.x, (size.y - doorWidth) / 2), wallColor);
                }
                else
                {
                    // Left part
                    CreateWallSegment(name + "_Left",
                        position - new Vector2((size.x / 4) + (doorWidth / 4), 0),
                        new Vector2((size.x - doorWidth) / 2, size.y), wallColor);

                    // Right part
                    CreateWallSegment(name + "_Right",
                        position + new Vector2((size.x / 4) + (doorWidth / 4), 0),
                        new Vector2((size.x - doorWidth) / 2, size.y), wallColor);
                }

                // Doorway visual (darker area)
                CreateDoorway(name + "_Doorway", position, isVertical, doorwayColor);
            }
            else
            {
                CreateWallSegment(name, position, size, wallColor);
            }
        }

        private void CreateWallSegment(string name, Vector2 position, Vector2 size, Color color)
        {
            var wallObj = new GameObject(name);
            wallObj.transform.SetParent(transform);
            wallObj.transform.localPosition = position;

            var sr = wallObj.AddComponent<SpriteRenderer>();

            // Use wall sprite for vertical walls
            bool isVertical = size.x < size.y;
            sr.sprite = PlaceholderSpriteGenerator.GetWallSprite(_floorLevel, isVertical);

            // Keep white color to show Midjourney texture colors properly
            sr.color = Color.white;
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 0;

            // Use tiled rendering for proper texture display
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;
            sr.size = size;

            // Add collider with proper size
            var collider = wallObj.AddComponent<BoxCollider2D>();
            collider.size = size;

            int wallLayer = LayerMask.NameToLayer("Walls");
            if (wallLayer >= 0)
            {
                wallObj.layer = wallLayer;
            }
        }

        private void CreateDoorway(string name, Vector2 position, bool isVertical, Color color)
        {
            // Create door sprite in the gap between wall segments
            var doorObj = new GameObject(name);
            doorObj.transform.SetParent(transform);
            doorObj.transform.localPosition = position;

            var sr = doorObj.AddComponent<SpriteRenderer>();

            // Get door sprite
            Sprite doorSprite = PlaceholderSpriteGenerator.GetDoorSprite(false, "");
            sr.sprite = doorSprite;
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 2; // In front of walls
            sr.color = Color.white; // Keep Midjourney texture colors

            // Size the door to fit the gap
            Vector2 doorSize = isVertical
                ? new Vector2(wallThickness * 1.5f, doorWidth)
                : new Vector2(doorWidth, wallThickness * 1.5f);

            // Use simple draw mode with scale for door sprites (they're not tileable)
            sr.drawMode = SpriteDrawMode.Simple;

            if (doorSprite != null && doorSprite.bounds.size.x > 0 && doorSprite.bounds.size.y > 0)
            {
                float scaleX = doorSize.x / doorSprite.bounds.size.x;
                float scaleY = doorSize.y / doorSprite.bounds.size.y;
                doorObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }

            Debug.Log($"[RoomVisuals] Created door: {name} at {position}, isVertical: {isVertical}");
        }

        private void CreateCorner(string name, Vector2 position, Color color)
        {
            var cornerObj = new GameObject(name);
            cornerObj.transform.SetParent(transform);
            cornerObj.transform.localPosition = position;

            var sr = cornerObj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetWallSprite(_floorLevel, false);
            sr.color = Color.white; // Keep original Midjourney texture colors
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 1;

            // Use tiled rendering for proper texture display
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;
            sr.size = new Vector2(wallThickness, wallThickness);

            var collider = cornerObj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(wallThickness, wallThickness);

            int wallLayer = LayerMask.NameToLayer("Walls");
            if (wallLayer >= 0)
            {
                cornerObj.layer = wallLayer;
            }
        }

        /// <summary>
        /// Gets the room bounds for camera or other systems.
        /// </summary>
        public Bounds GetRoomBounds()
        {
            return new Bounds(
                transform.position,
                new Vector3(roomWidth, roomHeight, 0)
            );
        }

        /// <summary>
        /// Gets floor display name for UI.
        /// </summary>
        public string GetFloorName()
        {
            return RoomDatabase.GetFloorName(_floorLevel);
        }
    }

    /// <summary>
    /// Simple animated torch component for flickering effect.
    /// </summary>
    public class AnimatedTorch : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private int _frameOffset;
        private float _animTimer;
        private const float FRAME_DURATION = 0.15f;

        public void Initialize(int offset)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _frameOffset = offset;
        }

        private void Update()
        {
            _animTimer += Time.deltaTime;
            if (_animTimer >= FRAME_DURATION)
            {
                _animTimer = 0;
                int frame = (int)(Time.time * 6) + _frameOffset;
                _spriteRenderer.sprite = PlaceholderSpriteGenerator.GetTorchSprite(frame % 4);
            }
        }
    }
}

