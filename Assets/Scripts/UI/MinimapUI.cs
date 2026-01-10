using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using HauntedCastle.Services;
using HauntedCastle.Data;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Displays a minimap showing visited rooms and the player's current location.
    /// Shows room connections and floor information.
    /// </summary>
    public class MinimapUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform mapContainer;
        [SerializeField] private Image playerMarker;

        [Header("Room Settings")]
        [SerializeField] private float roomSize = 20f;
        [SerializeField] private float roomSpacing = 5f;
        [SerializeField] private Color visitedRoomColor = new Color(0.3f, 0.5f, 0.3f, 0.8f);
        [SerializeField] private Color currentRoomColor = new Color(0.5f, 0.8f, 0.5f, 1f);
        [SerializeField] private Color unvisitedRoomColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private Color connectionColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);

        [Header("Floor Colors")]
        [SerializeField] private Color basementTint = new Color(0.6f, 0.5f, 0.7f);
        [SerializeField] private Color castleTint = new Color(0.8f, 0.7f, 0.5f);
        [SerializeField] private Color towerTint = new Color(0.5f, 0.7f, 0.9f);

        private Dictionary<string, RoomMapData> _roomMarkers = new();
        private string _currentRoomId;
        private HashSet<string> _visitedRooms = new();
        private Canvas _canvas;

        private class RoomMapData
        {
            public GameObject container;
            public Image roomImage;
            public Vector2 gridPosition;
            public int floor;
            public List<Image> connections = new();
        }

        private void Start()
        {
            CreateMapContainer();

            // Subscribe to room changes
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomLoaded;
            }

            // Build initial map from database
            BuildMapFromDatabase();
        }

        private void OnDestroy()
        {
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomLoaded;
            }
        }

        private void CreateMapContainer()
        {
            // Find or create canvas
            _canvas = FindFirstObjectByType<Canvas>();
            if (_canvas == null)
            {
                var canvasObj = new GameObject("MinimapCanvas");
                _canvas = canvasObj.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = 50;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Create minimap container
            var minimapObj = new GameObject("Minimap");
            minimapObj.transform.SetParent(_canvas.transform, false);

            mapContainer = minimapObj.AddComponent<RectTransform>();
            mapContainer.anchorMin = new Vector2(1, 1);
            mapContainer.anchorMax = new Vector2(1, 1);
            mapContainer.pivot = new Vector2(1, 1);
            mapContainer.anchoredPosition = new Vector2(-10, -100); // Below floor indicator
            mapContainer.sizeDelta = new Vector2(180, 150);

            // Background
            var bg = minimapObj.AddComponent<Image>();
            bg.color = new Color(0, 0, 0, 0.6f);

            // Add mask for clipping
            var mask = minimapObj.AddComponent<Mask>();
            mask.showMaskGraphic = true;

            // Create player marker
            var playerObj = new GameObject("PlayerMarker");
            playerObj.transform.SetParent(mapContainer, false);
            playerMarker = playerObj.AddComponent<Image>();
            playerMarker.color = Color.yellow;
            playerMarker.rectTransform.sizeDelta = new Vector2(8, 8);
        }

        private void BuildMapFromDatabase()
        {
            if (RoomDatabase.Instance == null) return;

            // Define approximate grid positions for each floor's rooms
            BuildFloorMap(0, RoomDatabase.Instance.basementRooms);
            BuildFloorMap(1, RoomDatabase.Instance.castleRooms);
            BuildFloorMap(2, RoomDatabase.Instance.towerRooms);
        }

        private void BuildFloorMap(int floor, List<RoomData> rooms)
        {
            if (rooms == null) return;

            // Simple grid layout based on room connections
            Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>();

            // Use a simple layout algorithm
            int col = 0;
            int row = 0;
            int maxCol = 4;

            foreach (var room in rooms)
            {
                if (room == null) continue;

                Vector2 pos = new Vector2(col * (roomSize + roomSpacing), -row * (roomSize + roomSpacing) - floor * 50);
                positions[room.roomId] = pos;

                CreateRoomMarker(room, pos, floor);

                col++;
                if (col >= maxCol)
                {
                    col = 0;
                    row++;
                }
            }

            // Create connections
            foreach (var room in rooms)
            {
                if (room == null) continue;
                CreateConnections(room, positions);
            }
        }

        private void CreateRoomMarker(RoomData room, Vector2 position, int floor)
        {
            var roomObj = new GameObject($"Room_{room.roomId}");
            roomObj.transform.SetParent(mapContainer, false);

            var rectTransform = roomObj.AddComponent<RectTransform>();
            rectTransform.anchoredPosition = position;
            rectTransform.sizeDelta = new Vector2(roomSize, roomSize);

            var image = roomObj.AddComponent<Image>();
            image.color = unvisitedRoomColor;

            // Apply floor tint
            Color tint = floor switch
            {
                0 => basementTint,
                1 => castleTint,
                2 => towerTint,
                _ => Color.white
            };
            image.color *= tint;

            var mapData = new RoomMapData
            {
                container = roomObj,
                roomImage = image,
                gridPosition = position,
                floor = floor
            };

            _roomMarkers[room.roomId] = mapData;
        }

        private void CreateConnections(RoomData room, Dictionary<string, Vector2> positions)
        {
            if (!positions.TryGetValue(room.roomId, out Vector2 fromPos)) return;
            if (!_roomMarkers.TryGetValue(room.roomId, out RoomMapData mapData)) return;

            // North connection
            if (room.northDoor?.exists == true && positions.TryGetValue(room.northDoor.destinationRoomId, out Vector2 toPos))
            {
                CreateConnectionLine(mapData, fromPos, toPos);
            }

            // East connection
            if (room.eastDoor?.exists == true && positions.TryGetValue(room.eastDoor.destinationRoomId, out toPos))
            {
                CreateConnectionLine(mapData, fromPos, toPos);
            }
        }

        private void CreateConnectionLine(RoomMapData mapData, Vector2 from, Vector2 to)
        {
            var lineObj = new GameObject("Connection");
            lineObj.transform.SetParent(mapContainer, false);
            lineObj.transform.SetSiblingIndex(0); // Behind rooms

            var rectTransform = lineObj.AddComponent<RectTransform>();

            // Position at midpoint
            Vector2 midpoint = (from + to) / 2f;
            rectTransform.anchoredPosition = midpoint;

            // Calculate size and rotation
            float distance = Vector2.Distance(from, to);
            float angle = Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg;

            rectTransform.sizeDelta = new Vector2(distance, 3f);
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);

            var image = lineObj.AddComponent<Image>();
            image.color = connectionColor;

            mapData.connections.Add(image);
        }

        private void OnRoomLoaded(RoomData roomData)
        {
            if (roomData == null) return;

            string roomId = roomData.roomId;

            // Mark as visited
            _visitedRooms.Add(roomId);

            // Update previous room color
            if (!string.IsNullOrEmpty(_currentRoomId) && _roomMarkers.TryGetValue(_currentRoomId, out RoomMapData prevData))
            {
                prevData.roomImage.color = visitedRoomColor * GetFloorTint(prevData.floor);
            }

            // Update current room color
            if (_roomMarkers.TryGetValue(roomId, out RoomMapData currentData))
            {
                currentData.roomImage.color = currentRoomColor * GetFloorTint(currentData.floor);

                // Move player marker
                if (playerMarker != null)
                {
                    playerMarker.rectTransform.anchoredPosition = currentData.gridPosition;
                    playerMarker.transform.SetAsLastSibling();
                }
            }

            _currentRoomId = roomId;

            // Center map on current room
            CenterMapOnRoom(roomId);
        }

        private Color GetFloorTint(int floor)
        {
            return floor switch
            {
                0 => basementTint,
                1 => castleTint,
                2 => towerTint,
                _ => Color.white
            };
        }

        private void CenterMapOnRoom(string roomId)
        {
            if (!_roomMarkers.TryGetValue(roomId, out RoomMapData data)) return;

            // Offset map so current room is centered
            Vector2 centerOffset = new Vector2(90, -75); // Half of map size
            Vector2 targetPos = -data.gridPosition + centerOffset;

            // Smoothly move to target
            StartCoroutine(SmoothCenterMap(targetPos));
        }

        private System.Collections.IEnumerator SmoothCenterMap(Vector2 targetOffset)
        {
            float duration = 0.3f;
            float elapsed = 0f;
            Vector2 startPos = Vector2.zero;

            // Get first room marker as reference
            foreach (var kvp in _roomMarkers)
            {
                startPos = kvp.Value.container.GetComponent<RectTransform>().anchoredPosition + kvp.Value.gridPosition;
                break;
            }

            while (elapsed < duration)
            {
                // CRITICAL: Use unscaledDeltaTime to prevent infinite loop when timeScale = 0
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                // Apply easing
                t = 1f - Mathf.Pow(1f - t, 3f);

                // Move all room markers
                foreach (var kvp in _roomMarkers)
                {
                    Vector2 originalPos = kvp.Value.gridPosition;
                    Vector2 newPos = originalPos + Vector2.Lerp(Vector2.zero, targetOffset - startPos, t);
                    kvp.Value.container.GetComponent<RectTransform>().anchoredPosition = newPos;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Creates the minimap UI if it doesn't exist.
        /// </summary>
        public static MinimapUI CreateMinimap()
        {
            var minimapObj = new GameObject("MinimapUI");
            return minimapObj.AddComponent<MinimapUI>();
        }
    }
}
