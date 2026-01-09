using UnityEngine;
using HauntedCastle.Data;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Handles procedural visual generation for rooms.
    /// Creates walls, floor, and decorations based on room data.
    /// </summary>
    public class RoomVisuals : MonoBehaviour
    {
        [Header("Room Dimensions")]
        [SerializeField] private float roomWidth = 16f;
        [SerializeField] private float roomHeight = 12f;
        [SerializeField] private float wallThickness = 0.5f;

        [Header("Colors")]
        [SerializeField] private Color floorColor = new Color(0.2f, 0.15f, 0.1f);
        [SerializeField] private Color wallColor = new Color(0.3f, 0.3f, 0.35f);
        [SerializeField] private Color doorwayColor = new Color(0.1f, 0.1f, 0.1f);

        [Header("Door Dimensions")]
        [SerializeField] private float doorWidth = 2f;

        private RoomData _roomData;

        public void Initialize(RoomData roomData)
        {
            _roomData = roomData;
            GenerateVisuals();
        }

        private void GenerateVisuals()
        {
            CreateFloor();
            CreateWalls();
        }

        private void CreateFloor()
        {
            var floorObj = new GameObject("Floor");
            floorObj.transform.SetParent(transform);
            floorObj.transform.localPosition = Vector3.zero;

            var sr = floorObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateRectSprite((int)roomWidth, (int)roomHeight);
            sr.color = floorColor;
            sr.sortingLayerName = "Background";
            sr.sortingOrder = -10;
        }

        private void CreateWalls()
        {
            // North wall
            CreateWall("WallNorth",
                new Vector2(0, roomHeight / 2),
                new Vector2(roomWidth, wallThickness),
                _roomData?.northDoor?.exists ?? false);

            // South wall
            CreateWall("WallSouth",
                new Vector2(0, -roomHeight / 2),
                new Vector2(roomWidth, wallThickness),
                _roomData?.southDoor?.exists ?? false);

            // East wall
            CreateWall("WallEast",
                new Vector2(roomWidth / 2, 0),
                new Vector2(wallThickness, roomHeight),
                _roomData?.eastDoor?.exists ?? false,
                true);

            // West wall
            CreateWall("WallWest",
                new Vector2(-roomWidth / 2, 0),
                new Vector2(wallThickness, roomHeight),
                _roomData?.westDoor?.exists ?? false,
                true);

            // Corners (to fill gaps)
            CreateCorner("CornerNE", new Vector2(roomWidth / 2, roomHeight / 2));
            CreateCorner("CornerNW", new Vector2(-roomWidth / 2, roomHeight / 2));
            CreateCorner("CornerSE", new Vector2(roomWidth / 2, -roomHeight / 2));
            CreateCorner("CornerSW", new Vector2(-roomWidth / 2, -roomHeight / 2));
        }

        private void CreateWall(string name, Vector2 position, Vector2 size, bool hasDoor, bool isVertical = false)
        {
            if (hasDoor)
            {
                // Create wall with doorway gap
                if (isVertical)
                {
                    // Top part
                    CreateWallSegment(name + "_Top",
                        position + new Vector2(0, (size.y / 4) + (doorWidth / 4)),
                        new Vector2(size.x, (size.y - doorWidth) / 2));

                    // Bottom part
                    CreateWallSegment(name + "_Bottom",
                        position - new Vector2(0, (size.y / 4) + (doorWidth / 4)),
                        new Vector2(size.x, (size.y - doorWidth) / 2));
                }
                else
                {
                    // Left part
                    CreateWallSegment(name + "_Left",
                        position - new Vector2((size.x / 4) + (doorWidth / 4), 0),
                        new Vector2((size.x - doorWidth) / 2, size.y));

                    // Right part
                    CreateWallSegment(name + "_Right",
                        position + new Vector2((size.x / 4) + (doorWidth / 4), 0),
                        new Vector2((size.x - doorWidth) / 2, size.y));
                }

                // Doorway visual (darker area)
                CreateDoorway(name + "_Doorway", position, isVertical);
            }
            else
            {
                CreateWallSegment(name, position, size);
            }
        }

        private void CreateWallSegment(string name, Vector2 position, Vector2 size)
        {
            var wallObj = new GameObject(name);
            wallObj.transform.SetParent(transform);
            wallObj.transform.localPosition = position;

            var sr = wallObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateRectSprite((int)size.x, (int)size.y);
            sr.color = wallColor;
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 0;

            // Add collider
            var collider = wallObj.AddComponent<BoxCollider2D>();
            collider.size = size;

            wallObj.layer = LayerMask.NameToLayer("Walls");
        }

        private void CreateDoorway(string name, Vector2 position, bool isVertical)
        {
            var doorwayObj = new GameObject(name);
            doorwayObj.transform.SetParent(transform);
            doorwayObj.transform.localPosition = position;

            var sr = doorwayObj.AddComponent<SpriteRenderer>();

            if (isVertical)
            {
                sr.sprite = CreateRectSprite((int)wallThickness, (int)doorWidth);
            }
            else
            {
                sr.sprite = CreateRectSprite((int)doorWidth, (int)wallThickness);
            }

            sr.color = doorwayColor;
            sr.sortingLayerName = "Background";
            sr.sortingOrder = -5;
        }

        private void CreateCorner(string name, Vector2 position)
        {
            var cornerObj = new GameObject(name);
            cornerObj.transform.SetParent(transform);
            cornerObj.transform.localPosition = position;

            var sr = cornerObj.AddComponent<SpriteRenderer>();
            sr.sprite = CreateRectSprite((int)wallThickness, (int)wallThickness);
            sr.color = wallColor;
            sr.sortingLayerName = "Walls";
            sr.sortingOrder = 1;

            var collider = cornerObj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(wallThickness, wallThickness);

            cornerObj.layer = LayerMask.NameToLayer("Walls");
        }

        /// <summary>
        /// Creates a simple rectangle sprite at runtime.
        /// </summary>
        private Sprite CreateRectSprite(int width, int height)
        {
            // Create a 1x1 white texture
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            texture.filterMode = FilterMode.Point;

            // Create sprite with proper PPU to match size
            return Sprite.Create(
                texture,
                new Rect(0, 0, 1, 1),
                new Vector2(0.5f, 0.5f),
                1f / Mathf.Max(width, height) // Scale to desired size
            );
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
    }
}
