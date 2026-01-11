using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Services;
using HauntedCastle.Utils;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Creates decorative visual elements for rooms (torches, stairs, furniture).
    /// NOTE: Floor and wall visuals are handled by RoomVisuals component attached to each Room.
    /// This separation prevents duplicate visuals and the "fake doors" problem.
    /// </summary>
    public class RoomVisualizer : MonoBehaviour
    {
        public static RoomVisualizer Instance { get; private set; }

        [Header("Room Settings")]
        [SerializeField] private float roomWidth = 14f;
        [SerializeField] private float roomHeight = 10f;
        [SerializeField] private float tileSize = 1f;

        private Transform _visualsContainer;
        private GameObject _currentRoomVisuals;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _visualsContainer = new GameObject("RoomVisuals").transform;
            _visualsContainer.SetParent(transform);
        }

        private void Start()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomLoaded;
            }

            // Create initial room visuals if room already loaded
            if (RoomManager.Instance?.CurrentRoomData != null)
            {
                CreateRoomVisuals(RoomManager.Instance.CurrentRoomData);
            }
        }

        private void OnDestroy()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomLoaded;
            }
        }

        private void OnRoomLoaded(RoomData roomData)
        {
            if (roomData == null)
            {
                Debug.LogError("[RoomVisualizer] Received null roomData!");
                return;
            }

            try
            {
                CreateRoomVisuals(roomData);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[RoomVisualizer] Failed to create visuals: {ex.Message}\n{ex.StackTrace}");
                // Create minimal fallback visuals
                CreateFallbackVisuals(roomData);
            }
        }

        public void CreateRoomVisuals(RoomData roomData)
        {
            // Clean up old visuals
            if (_currentRoomVisuals != null)
            {
                Destroy(_currentRoomVisuals);
            }

            _currentRoomVisuals = new GameObject($"RoomVisuals_{roomData.roomId}");
            _currentRoomVisuals.transform.SetParent(_visualsContainer);
            _currentRoomVisuals.transform.localPosition = Vector3.zero;

            // NOTE: Floor and Walls are created by RoomVisuals component attached to each Room.
            // RoomVisualizer only handles decorations to avoid duplicate visuals and "fake doors".
            // Do NOT call CreateFloor() or CreateWalls() here - they would create duplicate visuals.

            // Create ambient decorations based on room type (torches, stairs, furniture)
            CreateDecorations(roomData);

            Debug.Log($"[RoomVisualizer] Created decorations for room: {roomData.roomId}");
        }

        /// <summary>
        /// Creates minimal decorations when normal creation fails.
        /// Note: Floor/wall visuals are handled by RoomVisuals, this just adds basic torches.
        /// </summary>
        private void CreateFallbackVisuals(RoomData roomData)
        {
            if (_currentRoomVisuals != null)
            {
                Destroy(_currentRoomVisuals);
            }

            _currentRoomVisuals = new GameObject($"RoomVisuals_Fallback_{roomData?.roomId ?? "unknown"}");
            _currentRoomVisuals.transform.SetParent(_visualsContainer);
            _currentRoomVisuals.transform.localPosition = Vector3.zero;

            // Just add basic torches - floor/walls are handled by RoomVisuals component
            var decoContainer = new GameObject("Decorations");
            decoContainer.transform.SetParent(_currentRoomVisuals.transform);
            CreateTorch(decoContainer, new Vector3(-6f, 3f, 0));
            CreateTorch(decoContainer, new Vector3(6f, 3f, 0));

            Debug.LogWarning("[RoomVisualizer] Created fallback decorations (floor/walls handled by RoomVisuals)");
        }

        private Sprite CreateFallbackSprite()
        {
            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear; // Smooth filtering for fallback
            Color floor = new Color(0.3f, 0.25f, 0.2f);
            for (int i = 0; i < 16 * 16; i++)
                tex.SetPixel(i % 16, i / 16, floor);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        }

        private void CreateFloor(RoomData roomData)
        {
            var floorContainer = new GameObject("Floor");
            floorContainer.transform.SetParent(_currentRoomVisuals.transform);
            floorContainer.transform.localPosition = Vector3.zero;

            // Get the Midjourney floor sprite
            Sprite floorSprite = PlaceholderSpriteGenerator.GetFloorTileSprite(roomData.floorNumber, 0);

            // Create a SINGLE large floor that covers the room
            // The texture will tile naturally due to Wrap Mode = Repeat
            var floor = new GameObject("FloorSprite");
            floor.transform.SetParent(floorContainer.transform);
            floor.transform.localPosition = new Vector3(0, 0, 0.1f);

            var sr = floor.AddComponent<SpriteRenderer>();
            sr.sprite = floorSprite;
            sr.sortingLayerName = "Background";
            sr.sortingOrder = 0;
            sr.color = Color.white;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;

            // Set the size to cover the room - texture will tile within this area
            sr.size = new Vector2(roomWidth, roomHeight);

            Debug.Log($"[RoomVisualizer] Created tiled floor: {roomWidth}x{roomHeight} using sprite {floorSprite?.name}");
        }

        private void CreateWalls(RoomData roomData)
        {
            var wallContainer = new GameObject("Walls");
            wallContainer.transform.SetParent(_currentRoomVisuals.transform);
            wallContainer.transform.localPosition = Vector3.zero;

            // Use PlaceholderSpriteGenerator to get Midjourney wall sprites
            Sprite wallSprite = PlaceholderSpriteGenerator.GetWallSprite(roomData.floorNumber, false);

            float halfWidth = roomWidth / 2f;
            float halfHeight = roomHeight / 2f;
            float wallThickness = 1.5f; // Wall thickness

            // Create single tiled wall segments instead of many tiny tiles
            // North wall
            CreateTiledWall(wallContainer, "NorthWall", wallSprite,
                new Vector3(0, halfHeight + wallThickness / 2f, 0),
                new Vector2(roomWidth + wallThickness * 2, wallThickness),
                roomData.northDoor?.exists == true);

            // South wall
            CreateTiledWall(wallContainer, "SouthWall", wallSprite,
                new Vector3(0, -halfHeight - wallThickness / 2f, 0),
                new Vector2(roomWidth + wallThickness * 2, wallThickness),
                roomData.southDoor?.exists == true);

            // West wall
            CreateTiledWall(wallContainer, "WestWall", wallSprite,
                new Vector3(-halfWidth - wallThickness / 2f, 0, 0),
                new Vector2(wallThickness, roomHeight),
                roomData.westDoor?.exists == true);

            // East wall
            CreateTiledWall(wallContainer, "EastWall", wallSprite,
                new Vector3(halfWidth + wallThickness / 2f, 0, 0),
                new Vector2(wallThickness, roomHeight),
                roomData.eastDoor?.exists == true);
        }

        private void CreateTiledWall(GameObject parent, string name, Sprite wallSprite, Vector3 position, Vector2 size, bool hasDoor)
        {
            float doorGapSize = 2.5f; // Size of the door opening

            if (!hasDoor)
            {
                // No door - create single wall
                CreateSingleWallSegment(parent, name, wallSprite, position, size);
            }
            else
            {
                // Has door - create wall with gap
                bool isHorizontal = size.x > size.y;

                if (isHorizontal)
                {
                    // Horizontal wall (north/south) - split left and right
                    float segmentWidth = (size.x - doorGapSize) / 2f;

                    // Left segment
                    CreateSingleWallSegment(parent, name + "_Left", wallSprite,
                        position + new Vector3(-(segmentWidth + doorGapSize) / 2f, 0, 0),
                        new Vector2(segmentWidth, size.y));

                    // Right segment
                    CreateSingleWallSegment(parent, name + "_Right", wallSprite,
                        position + new Vector3((segmentWidth + doorGapSize) / 2f, 0, 0),
                        new Vector2(segmentWidth, size.y));

                    // Door visual (dark opening)
                    CreateDoorOpening(parent, name + "_Door", position, new Vector2(doorGapSize, size.y));
                }
                else
                {
                    // Vertical wall (east/west) - split top and bottom
                    float segmentHeight = (size.y - doorGapSize) / 2f;

                    // Top segment
                    CreateSingleWallSegment(parent, name + "_Top", wallSprite,
                        position + new Vector3(0, (segmentHeight + doorGapSize) / 2f, 0),
                        new Vector2(size.x, segmentHeight));

                    // Bottom segment
                    CreateSingleWallSegment(parent, name + "_Bottom", wallSprite,
                        position + new Vector3(0, -(segmentHeight + doorGapSize) / 2f, 0),
                        new Vector2(size.x, segmentHeight));

                    // Door visual (dark opening)
                    CreateDoorOpening(parent, name + "_Door", position, new Vector2(size.x, doorGapSize));
                }
            }
        }

        private void CreateSingleWallSegment(GameObject parent, string name, Sprite wallSprite, Vector3 position, Vector2 size)
        {
            var wall = new GameObject(name);
            wall.transform.SetParent(parent.transform);
            wall.transform.localPosition = position;

            var sr = wall.AddComponent<SpriteRenderer>();
            sr.sprite = wallSprite;
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 5;
            sr.color = Color.white;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;
            sr.size = size;

            // Add collider
            var collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;

            // Set wall layer
            int wallLayer = LayerMask.NameToLayer("Walls");
            if (wallLayer >= 0)
            {
                wall.layer = wallLayer;
            }
        }

        private void CreateDoorOpening(GameObject parent, string name, Vector3 position, Vector2 size)
        {
            // Create a door sprite in the gap
            var doorObj = new GameObject(name);
            doorObj.transform.SetParent(parent.transform);
            doorObj.transform.localPosition = position;

            var sr = doorObj.AddComponent<SpriteRenderer>();
            Sprite doorSprite = PlaceholderSpriteGenerator.GetDoorSprite(false, "");
            sr.sprite = doorSprite;
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 10; // In front of walls (walls are 5)
            sr.color = Color.white;

            // Use Simple mode with scale (NOT tiled - door sprites are not tileable)
            sr.drawMode = SpriteDrawMode.Simple;

            // Scale the door to fit the gap
            if (doorSprite != null && doorSprite.bounds.size.x > 0 && doorSprite.bounds.size.y > 0)
            {
                float scaleX = size.x / doorSprite.bounds.size.x;
                float scaleY = size.y / doorSprite.bounds.size.y;
                doorObj.transform.localScale = new Vector3(scaleX, scaleY, 1f);
                Debug.Log($"[RoomVisualizer] Door sprite bounds: {doorSprite.bounds.size}, Scale: ({scaleX}, {scaleY})");
            }
            else
            {
                Debug.LogWarning($"[RoomVisualizer] Door sprite has invalid bounds or is null!");
            }

            Debug.Log($"[RoomVisualizer] Created door: {name} at {position} with size {size}, sprite: {doorSprite?.name ?? "NULL"}");
        }

        private void CreateWallSegment(GameObject parent, Sprite wallSprite, float startX, float y,
            float width, float height, bool hasDoor, DoorDirection direction, DoorConnection doorData)
        {
            int tiles = Mathf.CeilToInt(width / tileSize);
            int doorStart = tiles / 2 - 1;
            int doorEnd = tiles / 2 + 1;

            for (int i = 0; i < tiles; i++)
            {
                bool isDoorTile = hasDoor && i >= doorStart && i <= doorEnd;

                var tile = new GameObject($"WallTile_{direction}_{i}");
                tile.transform.SetParent(parent.transform);
                tile.transform.localPosition = new Vector3(startX + i * tileSize + tileSize / 2f, y + height / 2f, 0);

                var sr = tile.AddComponent<SpriteRenderer>();

                if (isDoorTile && i == tiles / 2)
                {
                    // Door tile
                    Color? keyColor = doorData?.doorType == DoorType.Locked ? GetKeyColor(doorData.requiredKeyColor) : null;
                    sr.sprite = PlaceholderSpriteGenerator.GetDoorSprite(false, "");
                }
                else if (!isDoorTile)
                {
                    sr.sprite = wallSprite;
                    // Scale to fit tile size properly
                    float spriteSize = wallSprite.bounds.size.x;
                    float scale = tileSize / spriteSize;
                    tile.transform.localScale = new Vector3(scale, scale, 1);
                }
                else
                {
                    // Door frame tiles - keep white for Midjourney textures
                    sr.sprite = wallSprite;
                    float spriteSize = wallSprite.bounds.size.x;
                    float scale = tileSize / spriteSize;
                    tile.transform.localScale = new Vector3(scale, scale, 1);
                }

                sr.color = Color.white; // Preserve Midjourney texture colors
                sr.sortingLayerName = "Walls";
                sr.sortingOrder = direction == DoorDirection.North ? 0 : 10;

                // NOTE: Don't add colliders here - the Door system handles collision
                // The RoomVisualizer is purely for visual decoration
            }
        }

        private void CreateVerticalWallSegment(GameObject parent, Sprite wallSprite, float x, float startY,
            float width, float height, bool hasDoor, DoorDirection direction, DoorConnection doorData)
        {
            int tiles = Mathf.CeilToInt(height / tileSize);
            int doorStart = tiles / 2 - 1;
            int doorEnd = tiles / 2 + 1;

            for (int i = 0; i < tiles; i++)
            {
                bool isDoorTile = hasDoor && i >= doorStart && i <= doorEnd;

                var tile = new GameObject($"WallTile_{direction}_{i}");
                tile.transform.SetParent(parent.transform);
                tile.transform.localPosition = new Vector3(x + width / 2f, startY + i * tileSize + tileSize / 2f, 0);

                var sr = tile.AddComponent<SpriteRenderer>();

                if (isDoorTile && i == tiles / 2)
                {
                    Color? keyColor = doorData?.doorType == DoorType.Locked ? GetKeyColor(doorData.requiredKeyColor) : null;
                    sr.sprite = PlaceholderSpriteGenerator.GetDoorSprite(false, "");
                    // Rotate for side doors
                    tile.transform.rotation = Quaternion.Euler(0, 0, direction == DoorDirection.West ? 90 : -90);
                }
                else if (!isDoorTile)
                {
                    sr.sprite = wallSprite;
                    // Scale to fit tile size properly
                    float spriteSize = wallSprite.bounds.size.x;
                    float scale = tileSize / spriteSize;
                    tile.transform.localScale = new Vector3(scale, scale, 1);
                }
                else
                {
                    sr.sprite = wallSprite;
                    float spriteSize = wallSprite.bounds.size.x;
                    float scale = tileSize / spriteSize;
                    tile.transform.localScale = new Vector3(scale, scale, 1);
                }

                sr.color = Color.white; // Preserve Midjourney texture colors
                sr.sortingLayerName = "Walls";
                sr.sortingOrder = 5;

                // NOTE: Don't add colliders here - the Door system handles collision
            }
        }

        private void CreateCorner(GameObject parent, Sprite wallSprite, float x, float y)
        {
            var corner = new GameObject("Corner");
            corner.transform.SetParent(parent.transform);
            corner.transform.localPosition = new Vector3(x + tileSize / 2f, y + tileSize / 2f, 0);

            var sr = corner.AddComponent<SpriteRenderer>();
            sr.sprite = wallSprite;
            sr.color = Color.white; // Preserve Midjourney texture colors
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 5;

            // Scale to fit tile size properly
            float spriteSize = wallSprite.bounds.size.x;
            float scale = tileSize / spriteSize;
            corner.transform.localScale = new Vector3(scale, scale, 1);

            // NOTE: Don't add colliders here - purely visual
        }

        private void CreateDecorations(RoomData roomData)
        {
            var decoContainer = new GameObject("Decorations");
            decoContainer.transform.SetParent(_currentRoomVisuals.transform);
            decoContainer.transform.localPosition = Vector3.zero;

            // Add room-specific decorations based on room name/type
            string roomName = roomData.displayName?.ToLower() ?? "";

            if (roomName.Contains("library"))
            {
                CreateBookshelf(decoContainer, new Vector3(-5f, 3f, 0));
                CreateBookshelf(decoContainer, new Vector3(-5f, -1f, 0));
            }
            else if (roomName.Contains("throne"))
            {
                CreateThrone(decoContainer, new Vector3(0, 2f, 0));
            }
            else if (roomName.Contains("kitchen"))
            {
                CreateTable(decoContainer, new Vector3(-2f, 0, 0));
                CreateBarrel(decoContainer, new Vector3(4f, 2f, 0));
            }
            else if (roomName.Contains("armory"))
            {
                CreateWeaponRack(decoContainer, new Vector3(-4f, 2f, 0));
                CreateWeaponRack(decoContainer, new Vector3(4f, 2f, 0));
            }
            else if (roomName.Contains("dungeon") || roomName.Contains("cell"))
            {
                CreateChains(decoContainer, new Vector3(-5f, 2f, 0));
                CreateChains(decoContainer, new Vector3(5f, 2f, 0));
            }

            // Add torches to all rooms
            CreateTorch(decoContainer, new Vector3(-6f, 3f, 0));
            CreateTorch(decoContainer, new Vector3(6f, 3f, 0));

            // Add stairs if present
            if (roomData.stairsUp?.exists == true)
            {
                CreateStairs(decoContainer, roomData.stairsUp.position, true);
            }
            if (roomData.stairsDown?.exists == true)
            {
                CreateStairs(decoContainer, roomData.stairsDown.position, false);
            }
        }

        private void CreateBookshelf(GameObject parent, Vector3 position)
        {
            var shelf = new GameObject("Bookshelf");
            shelf.transform.SetParent(parent.transform);
            shelf.transform.localPosition = position;

            var sr = shelf.AddComponent<SpriteRenderer>();
            sr.sprite = CreateBookshelfSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = -1;
        }

        private void CreateThrone(GameObject parent, Vector3 position)
        {
            var throne = new GameObject("Throne");
            throne.transform.SetParent(parent.transform);
            throne.transform.localPosition = position;

            var sr = throne.AddComponent<SpriteRenderer>();
            sr.sprite = CreateThroneSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = -1;
        }

        private void CreateTable(GameObject parent, Vector3 position)
        {
            var table = new GameObject("Table");
            table.transform.SetParent(parent.transform);
            table.transform.localPosition = position;

            var sr = table.AddComponent<SpriteRenderer>();
            sr.sprite = CreateTableSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = -1;
        }

        private void CreateBarrel(GameObject parent, Vector3 position)
        {
            var barrel = new GameObject("Barrel");
            barrel.transform.SetParent(parent.transform);
            barrel.transform.localPosition = position;

            var sr = barrel.AddComponent<SpriteRenderer>();
            sr.sprite = CreateBarrelSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = -1;
        }

        private void CreateWeaponRack(GameObject parent, Vector3 position)
        {
            var rack = new GameObject("WeaponRack");
            rack.transform.SetParent(parent.transform);
            rack.transform.localPosition = position;

            var sr = rack.AddComponent<SpriteRenderer>();
            sr.sprite = CreateWeaponRackSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = -1;
        }

        private void CreateChains(GameObject parent, Vector3 position)
        {
            var chains = new GameObject("Chains");
            chains.transform.SetParent(parent.transform);
            chains.transform.localPosition = position;

            var sr = chains.AddComponent<SpriteRenderer>();
            sr.sprite = CreateChainsSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = -1;
        }

        private void CreateTorch(GameObject parent, Vector3 position)
        {
            var torch = new GameObject("Torch");
            torch.transform.SetParent(parent.transform);
            torch.transform.localPosition = position;

            var sr = torch.AddComponent<SpriteRenderer>();
            sr.sprite = CreateTorchSprite();
            sr.sortingLayerName = "Items";
            sr.sortingOrder = 0;

            // Add flicker animation
            var flicker = torch.AddComponent<TorchFlicker>();
        }

        private void CreateStairs(GameObject parent, Vector2 position, bool goingUp)
        {
            var stairs = new GameObject(goingUp ? "StairsUp" : "StairsDown");
            stairs.transform.SetParent(parent.transform);
            stairs.transform.localPosition = position;

            var sr = stairs.AddComponent<SpriteRenderer>();
            sr.sprite = PixelArtGenerator.GetStairsSprite(goingUp);
            sr.sortingLayerName = "Items";
            sr.sortingOrder = -2;
        }

        #region Decoration Sprites

        private Sprite CreateBookshelfSprite()
        {
            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;

            Color wood = new Color(0.4f, 0.25f, 0.15f);
            Color woodDark = new Color(0.3f, 0.18f, 0.1f);
            Color[] bookColors = { Color.red, Color.blue, Color.green, new Color(0.6f, 0.4f, 0.2f), Color.magenta };

            // Clear
            for (int i = 0; i < tex.width * tex.height; i++)
                tex.SetPixel(i % tex.width, i / tex.width, Color.clear);

            // Shelf frame
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    if (x < 2 || x >= 30 || y < 2 || y >= 30)
                        tex.SetPixel(x, y, wood);
                    else if (y == 10 || y == 20)
                        tex.SetPixel(x, y, woodDark);
                }
            }

            // Books on shelves
            System.Random rng = new System.Random(42);
            for (int shelf = 0; shelf < 3; shelf++)
            {
                int shelfY = 3 + shelf * 10;
                for (int book = 0; book < 8; book++)
                {
                    int bookX = 3 + book * 3;
                    int bookHeight = 5 + rng.Next(3);
                    Color bookColor = bookColors[rng.Next(bookColors.Length)];
                    for (int y = shelfY; y < shelfY + bookHeight && y < shelfY + 8; y++)
                    {
                        for (int x = bookX; x < bookX + 2; x++)
                            tex.SetPixel(x, y, bookColor);
                    }
                }
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 16);
        }

        private Sprite CreateThroneSprite()
        {
            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;

            Color gold = new Color(0.9f, 0.75f, 0.2f);
            Color red = new Color(0.6f, 0.1f, 0.1f);
            Color redDark = new Color(0.4f, 0.05f, 0.05f);

            for (int i = 0; i < tex.width * tex.height; i++)
                tex.SetPixel(i % tex.width, i / tex.width, Color.clear);

            // Back of throne
            for (int y = 10; y < 30; y++)
            {
                for (int x = 8; x < 24; x++)
                {
                    tex.SetPixel(x, y, red);
                }
            }

            // Gold trim
            for (int x = 8; x < 24; x++) { tex.SetPixel(x, 29, gold); tex.SetPixel(x, 10, gold); }
            for (int y = 10; y < 30; y++) { tex.SetPixel(8, y, gold); tex.SetPixel(23, y, gold); }

            // Seat cushion
            for (int y = 4; y < 10; y++)
            {
                for (int x = 6; x < 26; x++)
                    tex.SetPixel(x, y, redDark);
            }

            // Armrests
            for (int y = 4; y < 14; y++)
            {
                tex.SetPixel(4, y, gold);
                tex.SetPixel(5, y, gold);
                tex.SetPixel(26, y, gold);
                tex.SetPixel(27, y, gold);
            }

            // Crown decoration on top
            tex.SetPixel(15, 31, gold);
            tex.SetPixel(16, 31, gold);

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 16);
        }

        private Sprite CreateTableSprite()
        {
            var tex = new Texture2D(32, 16);
            tex.filterMode = FilterMode.Bilinear;

            Color wood = new Color(0.5f, 0.35f, 0.2f);
            Color woodDark = new Color(0.4f, 0.25f, 0.15f);

            for (int i = 0; i < tex.width * tex.height; i++)
                tex.SetPixel(i % tex.width, i / tex.width, Color.clear);

            // Table top
            for (int x = 2; x < 30; x++)
            {
                for (int y = 10; y < 14; y++)
                    tex.SetPixel(x, y, wood);
            }

            // Legs
            for (int y = 2; y < 10; y++)
            {
                tex.SetPixel(4, y, woodDark);
                tex.SetPixel(5, y, woodDark);
                tex.SetPixel(26, y, woodDark);
                tex.SetPixel(27, y, woodDark);
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 32, 16), new Vector2(0.5f, 0.5f), 16);
        }

        private Sprite CreateBarrelSprite()
        {
            var tex = new Texture2D(16, 16);
            tex.filterMode = FilterMode.Bilinear;

            Color wood = new Color(0.5f, 0.35f, 0.2f);
            Color band = new Color(0.4f, 0.4f, 0.45f);

            for (int i = 0; i < tex.width * tex.height; i++)
                tex.SetPixel(i % tex.width, i / tex.width, Color.clear);

            // Barrel body
            for (int y = 2; y < 14; y++)
            {
                int width = y < 4 || y > 11 ? 8 : 10;
                int startX = 8 - width / 2;
                for (int x = startX; x < startX + width; x++)
                    tex.SetPixel(x, y, wood);
            }

            // Metal bands
            for (int x = 4; x < 12; x++)
            {
                tex.SetPixel(x, 4, band);
                tex.SetPixel(x, 11, band);
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        }

        private Sprite CreateWeaponRackSprite()
        {
            var tex = new Texture2D(32, 32);
            tex.filterMode = FilterMode.Bilinear;

            Color wood = new Color(0.4f, 0.25f, 0.15f);
            Color metal = new Color(0.6f, 0.6f, 0.7f);

            for (int i = 0; i < tex.width * tex.height; i++)
                tex.SetPixel(i % tex.width, i / tex.width, Color.clear);

            // Rack frame
            for (int y = 4; y < 28; y++)
            {
                tex.SetPixel(4, y, wood);
                tex.SetPixel(27, y, wood);
            }
            for (int x = 4; x < 28; x++)
            {
                tex.SetPixel(x, 4, wood);
                tex.SetPixel(x, 16, wood);
            }

            // Swords
            for (int y = 6; y < 14; y++)
            {
                tex.SetPixel(10, y, metal);
                tex.SetPixel(16, y, metal);
                tex.SetPixel(22, y, metal);
            }

            // Shields on bottom
            for (int y = 18; y < 26; y++)
            {
                for (int x = 8; x < 14; x++)
                    tex.SetPixel(x, y, metal);
                for (int x = 18; x < 24; x++)
                    tex.SetPixel(x, y, metal);
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 16);
        }

        private Sprite CreateChainsSprite()
        {
            var tex = new Texture2D(16, 32);
            tex.filterMode = FilterMode.Bilinear;

            Color chain = new Color(0.5f, 0.5f, 0.55f);

            for (int i = 0; i < tex.width * tex.height; i++)
                tex.SetPixel(i % tex.width, i / tex.width, Color.clear);

            // Chain links
            for (int link = 0; link < 8; link++)
            {
                int y = link * 4;
                tex.SetPixel(7, y, chain);
                tex.SetPixel(8, y, chain);
                tex.SetPixel(6, y + 1, chain);
                tex.SetPixel(9, y + 1, chain);
                tex.SetPixel(6, y + 2, chain);
                tex.SetPixel(9, y + 2, chain);
                tex.SetPixel(7, y + 3, chain);
                tex.SetPixel(8, y + 3, chain);
            }

            // Shackle at bottom
            for (int x = 4; x < 12; x++)
                tex.SetPixel(x, 0, chain);

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 16, 32), new Vector2(0.5f, 0.5f), 16);
        }

        private Sprite CreateTorchSprite()
        {
            var tex = new Texture2D(8, 16);
            tex.filterMode = FilterMode.Bilinear;

            Color wood = new Color(0.5f, 0.3f, 0.15f);
            Color flame1 = new Color(1f, 0.8f, 0.2f);
            Color flame2 = new Color(1f, 0.5f, 0.1f);

            for (int i = 0; i < tex.width * tex.height; i++)
                tex.SetPixel(i % tex.width, i / tex.width, Color.clear);

            // Handle
            for (int y = 0; y < 8; y++)
            {
                tex.SetPixel(3, y, wood);
                tex.SetPixel(4, y, wood);
            }

            // Flame
            tex.SetPixel(3, 8, flame2);
            tex.SetPixel(4, 8, flame2);
            tex.SetPixel(2, 9, flame2);
            tex.SetPixel(3, 9, flame1);
            tex.SetPixel(4, 9, flame1);
            tex.SetPixel(5, 9, flame2);
            tex.SetPixel(2, 10, flame1);
            tex.SetPixel(3, 10, flame1);
            tex.SetPixel(4, 10, flame1);
            tex.SetPixel(5, 10, flame1);
            tex.SetPixel(3, 11, flame1);
            tex.SetPixel(4, 11, flame1);
            tex.SetPixel(3, 12, flame2);
            tex.SetPixel(4, 12, flame2);
            tex.SetPixel(3, 13, flame2);

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 8, 16), new Vector2(0.5f, 0.5f), 8);
        }

        #endregion

        private Color? GetKeyColor(KeyColor keyColor)
        {
            return keyColor switch
            {
                KeyColor.Red => Color.red,
                KeyColor.Blue => Color.blue,
                KeyColor.Green => Color.green,
                KeyColor.Yellow => Color.yellow,
                KeyColor.Cyan => Color.cyan,
                KeyColor.Magenta => Color.magenta,
                _ => null
            };
        }
    }

    /// <summary>
    /// Simple torch flicker effect.
    /// </summary>
    public class TorchFlicker : MonoBehaviour
    {
        private SpriteRenderer _sr;
        private float _baseIntensity = 1f;

        private void Start()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            float flicker = 0.8f + Mathf.PerlinNoise(Time.time * 10f, transform.position.x) * 0.4f;
            _sr.color = new Color(flicker, flicker * 0.9f, flicker * 0.7f);
        }
    }
}
