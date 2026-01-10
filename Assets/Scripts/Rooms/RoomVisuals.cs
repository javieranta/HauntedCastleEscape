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
        [SerializeField] private float tileSize = 2f;

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
            GenerateVisuals();
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
            var floorContainer = new GameObject("FloorTiles");
            floorContainer.transform.SetParent(transform);
            floorContainer.transform.localPosition = Vector3.zero;

            // Calculate number of tiles needed
            int tilesX = Mathf.CeilToInt(roomWidth / tileSize);
            int tilesY = Mathf.CeilToInt(roomHeight / tileSize);

            float startX = -(tilesX * tileSize) / 2f + tileSize / 2f;
            float startY = -(tilesY * tileSize) / 2f + tileSize / 2f;

            int variation = 0;
            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    var tileObj = new GameObject($"Tile_{x}_{y}");
                    tileObj.transform.SetParent(floorContainer.transform);
                    tileObj.transform.localPosition = new Vector3(
                        startX + x * tileSize,
                        startY + y * tileSize,
                        0
                    );
                    tileObj.transform.localScale = new Vector3(tileSize / 32f * 2f, tileSize / 32f * 2f, 1);

                    var sr = tileObj.AddComponent<SpriteRenderer>();
                    sr.sprite = PlaceholderSpriteGenerator.GetFloorTileSprite(_floorLevel, variation++);
                    sr.sortingLayerName = "Background";
                    sr.sortingOrder = -10;
                }
            }
        }

        private void CreateThemedWalls()
        {
            Color wallColor = GetFloorWallColor();
            Color doorwayColor = new Color(0.08f, 0.06f, 0.04f);

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
            return _floorLevel switch
            {
                0 => new Color(0.22f, 0.2f, 0.18f),   // Basement - dark stone
                1 => new Color(0.38f, 0.35f, 0.32f),  // Castle - medium stone
                2 => new Color(0.48f, 0.46f, 0.44f),  // Tower - lighter stone
                _ => new Color(0.35f, 0.33f, 0.3f)
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
            // Slightly tint all sprites in the room
            var renderers = GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                if (renderer.sortingLayerName == "Background")
                {
                    renderer.color = Color.Lerp(renderer.color, ambientColor, 0.3f);
                }
            }
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
            sr.color = color;
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 0;

            // Scale sprite to fit wall size
            float scaleX = size.x / 1f;
            float scaleY = size.y / 1f;
            wallObj.transform.localScale = new Vector3(scaleX, scaleY, 1);

            // Add collider (use original size, not scaled)
            var collider = wallObj.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one; // Will be scaled by transform

            int wallLayer = LayerMask.NameToLayer("Walls");
            if (wallLayer >= 0)
            {
                wallObj.layer = wallLayer;
            }
        }

        private void CreateDoorway(string name, Vector2 position, bool isVertical, Color color)
        {
            var doorwayObj = new GameObject(name);
            doorwayObj.transform.SetParent(transform);
            doorwayObj.transform.localPosition = position;

            var sr = doorwayObj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetDoorSprite(true, "");
            sr.color = color;
            sr.sortingLayerName = "Background";
            sr.sortingOrder = -5;

            // Scale for doorway
            float scaleX = isVertical ? wallThickness / 1f : doorWidth / 1f;
            float scaleY = isVertical ? doorWidth / 1f : wallThickness / 1f;
            doorwayObj.transform.localScale = new Vector3(scaleX, scaleY, 1);
        }

        private void CreateCorner(string name, Vector2 position, Color color)
        {
            var cornerObj = new GameObject(name);
            cornerObj.transform.SetParent(transform);
            cornerObj.transform.localPosition = position;

            var sr = cornerObj.AddComponent<SpriteRenderer>();
            sr.sprite = PlaceholderSpriteGenerator.GetWallSprite(_floorLevel, false);
            sr.color = color;
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 1;

            cornerObj.transform.localScale = new Vector3(wallThickness, wallThickness, 1);

            var collider = cornerObj.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one;

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

