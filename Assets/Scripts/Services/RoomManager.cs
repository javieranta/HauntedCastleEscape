using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Rooms;
using HauntedCastle.Audio;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Manages room loading, transitions, and current room state.
    /// Works with RoomData ScriptableObjects to build rooms dynamically.
    /// </summary>
    public class RoomManager : MonoBehaviour
    {
        public static RoomManager Instance { get; private set; }

        [Header("Room Database")]
        [SerializeField] private List<RoomData> allRooms = new();
        [SerializeField] private RoomData startingRoom;

        [Header("Prefabs")]
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private GameObject doorPrefab;
        [SerializeField] private GameObject secretPassagePrefab;
        [SerializeField] private GameObject stairsPrefab;
        [SerializeField] private GameObject trapdoorPrefab;
        [SerializeField] private GameObject hazardPrefab;

        [Header("Container")]
        [SerializeField] private Transform roomContainer;

        [Header("Transition Settings")]
        [SerializeField] private float transitionDuration = 0.5f;

        // Current state
        public RoomData CurrentRoomData { get; private set; }
        public Room CurrentRoom { get; private set; }
        public string PreviousRoomId { get; private set; }

        // Events
        public event Action<RoomData> OnRoomLoadStarted;
        public event Action<RoomData> OnRoomLoadCompleted;
        public event Action<string, string> OnRoomTransition; // fromRoomId, toRoomId

        // Room lookup cache
        private Dictionary<string, RoomData> _roomLookup = new();

        // Transition state
        private bool _isTransitioning = false;
        private Vector2 _pendingSpawnPosition;
        private string _pendingSpawnId;
        private float _transitionStartTime; // Safety timeout tracking

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            BuildRoomLookup();
        }

        private void Start()
        {
            if (roomContainer == null)
            {
                var containerObj = GameObject.Find("RoomContainer");
                if (containerObj != null)
                {
                    roomContainer = containerObj.transform;
                }
                else
                {
                    roomContainer = new GameObject("RoomContainer").transform;
                }
            }
        }

        private void Update()
        {
            // SAFETY: Ensure transition doesn't get stuck
            if (_isTransitioning && Time.unscaledTime - _transitionStartTime > 5f)
            {
                Debug.LogWarning("[RoomManager] Transition was stuck for 5+ seconds, forcing reset");
                _isTransitioning = false;
            }
        }

        private void BuildRoomLookup()
        {
            _roomLookup.Clear();
            foreach (var room in allRooms)
            {
                if (room != null && !string.IsNullOrEmpty(room.roomId))
                {
                    _roomLookup[room.roomId] = room;
                }
            }
        }

        /// <summary>
        /// Registers a room to the manager (used for procedurally generated rooms).
        /// </summary>
        public void RegisterRoom(RoomData roomData)
        {
            if (roomData != null && !string.IsNullOrEmpty(roomData.roomId))
            {
                _roomLookup[roomData.roomId] = roomData;
                if (!allRooms.Contains(roomData))
                {
                    allRooms.Add(roomData);
                }
            }
        }

        /// <summary>
        /// Gets a room by its ID.
        /// </summary>
        public RoomData GetRoom(string roomId)
        {
            return _roomLookup.TryGetValue(roomId, out var room) ? room : null;
        }

        /// <summary>
        /// Loads the starting room.
        /// </summary>
        public void LoadStartingRoom()
        {
            if (startingRoom != null)
            {
                LoadRoom(startingRoom.roomId, "start");
            }
            else if (allRooms.Count > 0)
            {
                LoadRoom(allRooms[0].roomId, "start");
            }
            else
            {
                Debug.LogError("[RoomManager] No rooms available to load!");
            }
        }

        /// <summary>
        /// Loads a room by ID with transition effect.
        /// </summary>
        public void LoadRoom(string roomId, string spawnPointId = null)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("[RoomManager] Already transitioning, ignoring load request");
                return;
            }

            var roomData = GetRoom(roomId);
            if (roomData == null)
            {
                Debug.LogError($"[RoomManager] Room not found: {roomId}");
                return;
            }

            StartCoroutine(LoadRoomCoroutine(roomData, spawnPointId));
        }

        /// <summary>
        /// Transitions to an adjacent room through a door.
        /// </summary>
        public void TransitionThroughDoor(DoorDirection direction)
        {
            if (CurrentRoomData == null || _isTransitioning) return;

            DoorConnection door = GetDoorConnection(CurrentRoomData, direction);
            if (door == null || !door.exists) return;

            string destinationId = door.destinationRoomId;
            string spawnId = GetOppositeDirection(direction).ToString().ToLower() + "_door";

            OnRoomTransition?.Invoke(CurrentRoomData.roomId, destinationId);
            LoadRoom(destinationId, spawnId);
        }

        /// <summary>
        /// Transitions through stairs or trapdoor.
        /// </summary>
        public void TransitionThroughFloor(FloorTransitionType transitionType)
        {
            if (CurrentRoomData == null || _isTransitioning) return;

            FloorTransition transition = transitionType switch
            {
                FloorTransitionType.StairsUp => CurrentRoomData.stairsUp,
                FloorTransitionType.StairsDown => CurrentRoomData.stairsDown,
                FloorTransitionType.Trapdoor => CurrentRoomData.trapdoor,
                _ => null
            };

            if (transition == null || !transition.exists) return;

            string spawnId = transitionType switch
            {
                FloorTransitionType.StairsUp => "stairs_down",
                FloorTransitionType.StairsDown => "stairs_up",
                FloorTransitionType.Trapdoor => "trapdoor_exit",
                _ => "center"
            };

            OnRoomTransition?.Invoke(CurrentRoomData.roomId, transition.destinationRoomId);
            LoadRoom(transition.destinationRoomId, spawnId);
        }

        /// <summary>
        /// Transitions through a secret passage.
        /// </summary>
        public void TransitionThroughSecretPassage(SecretPassageType passageType, string destinationRoomId)
        {
            if (_isTransitioning) return;

            OnRoomTransition?.Invoke(CurrentRoomData?.roomId ?? "", destinationRoomId);
            LoadRoom(destinationRoomId, "secret_" + passageType.ToString().ToLower());
        }

        private IEnumerator LoadRoomCoroutine(RoomData roomData, string spawnPointId)
        {
            _isTransitioning = true;
            _transitionStartTime = Time.unscaledTime;
            _pendingSpawnId = spawnPointId;

            OnRoomLoadStarted?.Invoke(roomData);

            // Fade out
            if (TransitionManager.Instance != null)
            {
                yield return TransitionManager.Instance.FadeOut(transitionDuration);
            }

            // Unload current room
            UnloadCurrentRoom();

            // Store previous room
            if (CurrentRoomData != null)
            {
                PreviousRoomId = CurrentRoomData.roomId;
            }

            // Set new room data
            CurrentRoomData = roomData;

            // Build new room
            CurrentRoom = BuildRoom(roomData);

            // Determine spawn position
            _pendingSpawnPosition = GetSpawnPosition(roomData, spawnPointId);

            // Notify listeners (player will reposition)
            OnRoomLoadCompleted?.Invoke(roomData);

            // Fade in
            if (TransitionManager.Instance != null)
            {
                yield return TransitionManager.Instance.FadeIn(transitionDuration);
            }

            _isTransitioning = false;

            // Play room enter sound
            AudioManager.Instance?.PlaySFX(SoundEffect.DoorOpen);

            Debug.Log($"[RoomManager] Loaded room: {roomData.roomId} (Floor {roomData.floorNumber})");
        }

        private void UnloadCurrentRoom()
        {
            if (CurrentRoom != null)
            {
                Destroy(CurrentRoom.gameObject);
                CurrentRoom = null;
            }

            // Clean up any remaining room objects
            if (roomContainer != null)
            {
                foreach (Transform child in roomContainer)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private Room BuildRoom(RoomData roomData)
        {
            GameObject roomObj;

            if (roomPrefab != null)
            {
                roomObj = Instantiate(roomPrefab, roomContainer);
            }
            else
            {
                roomObj = new GameObject($"Room_{roomData.roomId}");
                roomObj.transform.SetParent(roomContainer);
            }

            roomObj.name = $"Room_{roomData.roomId}";

            // Add or get Room component
            var room = roomObj.GetComponent<Room>();
            if (room == null)
            {
                room = roomObj.AddComponent<Room>();
            }

            room.Initialize(roomData);

            // Build room contents
            BuildDoors(room, roomData);
            BuildFloorTransitions(room, roomData);
            BuildSecretPassages(room, roomData);
            BuildHazards(room, roomData);

            return room;
        }

        private void BuildDoors(Room room, RoomData roomData)
        {
            BuildDoor(room, roomData.northDoor, DoorDirection.North, new Vector2(0, 4));
            BuildDoor(room, roomData.eastDoor, DoorDirection.East, new Vector2(7, 0));
            BuildDoor(room, roomData.southDoor, DoorDirection.South, new Vector2(0, -4));
            BuildDoor(room, roomData.westDoor, DoorDirection.West, new Vector2(-7, 0));
        }

        private void BuildDoor(Room room, DoorConnection doorData, DoorDirection direction, Vector2 defaultPosition)
        {
            if (doorData == null || !doorData.exists) return;

            GameObject doorObj;
            if (doorPrefab != null)
            {
                doorObj = Instantiate(doorPrefab, room.transform);
            }
            else
            {
                doorObj = new GameObject($"Door_{direction}");
                doorObj.transform.SetParent(room.transform);

                // Add collider for trigger - larger size for better detection
                var collider = doorObj.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;

                // Adjust collider size based on door direction
                bool isVertical = (direction == DoorDirection.East || direction == DoorDirection.West);
                collider.size = isVertical ? new Vector2(1.5f, 3f) : new Vector2(3f, 1.5f);

                // Add sprite renderer placeholder
                var sr = doorObj.AddComponent<SpriteRenderer>();
                sr.color = GetDoorColor(doorData.doorType, doorData.requiredKeyColor);
                sr.sortingLayerName = "Items";
                sr.sortingOrder = 2;
            }

            doorObj.transform.localPosition = defaultPosition;

            var door = doorObj.GetComponent<Door>();
            if (door == null)
            {
                door = doorObj.AddComponent<Door>();
            }

            door.Initialize(doorData, direction);
            room.RegisterDoor(direction, door);

            Debug.Log($"[RoomManager] Built door {direction} -> {doorData.destinationRoomId}");
        }

        private void BuildFloorTransitions(Room room, RoomData roomData)
        {
            if (roomData.stairsUp != null && roomData.stairsUp.exists)
            {
                BuildFloorTransition(room, roomData.stairsUp, FloorTransitionType.StairsUp);
            }

            if (roomData.stairsDown != null && roomData.stairsDown.exists)
            {
                BuildFloorTransition(room, roomData.stairsDown, FloorTransitionType.StairsDown);
            }

            if (roomData.trapdoor != null && roomData.trapdoor.exists)
            {
                BuildFloorTransition(room, roomData.trapdoor, FloorTransitionType.Trapdoor);
            }
        }

        private void BuildFloorTransition(Room room, FloorTransition transitionData, FloorTransitionType type)
        {
            GameObject transObj;
            GameObject prefab = type == FloorTransitionType.Trapdoor ? trapdoorPrefab : stairsPrefab;

            if (prefab != null)
            {
                transObj = Instantiate(prefab, room.transform);
            }
            else
            {
                transObj = new GameObject($"FloorTransition_{type}");
                transObj.transform.SetParent(room.transform);

                var collider = transObj.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                // Larger collider for easier stair interaction
                collider.size = type == FloorTransitionType.Trapdoor
                    ? new Vector2(1.5f, 1.5f)
                    : new Vector2(2.5f, 2.5f);

                var sr = transObj.AddComponent<SpriteRenderer>();
                sr.color = type == FloorTransitionType.Trapdoor ? Color.magenta : Color.cyan;
            }

            // Scale stairs for visibility
            if (type != FloorTransitionType.Trapdoor)
            {
                transObj.transform.localScale = Vector3.one * 1.5f;
            }

            transObj.transform.localPosition = transitionData.position;

            var transition = transObj.GetComponent<FloorTransitionTrigger>();
            if (transition == null)
            {
                transition = transObj.AddComponent<FloorTransitionTrigger>();
            }

            transition.Initialize(transitionData, type);
        }

        private void BuildSecretPassages(Room room, RoomData roomData)
        {
            foreach (var passage in roomData.secretPassages)
            {
                BuildSecretPassage(room, passage);
            }
        }

        private void BuildSecretPassage(Room room, SecretPassage passageData)
        {
            GameObject passageObj;

            if (secretPassagePrefab != null)
            {
                passageObj = Instantiate(secretPassagePrefab, room.transform);
            }
            else
            {
                passageObj = new GameObject($"SecretPassage_{passageData.passageType}");
                passageObj.transform.SetParent(room.transform);

                var collider = passageObj.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = new Vector2(1f, 1.5f);

                var sr = passageObj.AddComponent<SpriteRenderer>();
                sr.sprite = passageData.disguiseSprite;
                sr.color = GetSecretPassageColor(passageData.passageType);
            }

            passageObj.transform.localPosition = passageData.position;

            var passage = passageObj.GetComponent<SecretPassageTrigger>();
            if (passage == null)
            {
                passage = passageObj.AddComponent<SecretPassageTrigger>();
            }

            passage.Initialize(passageData);
        }

        private void BuildHazards(Room room, RoomData roomData)
        {
            foreach (var hazardSpawn in roomData.hazardSpawns)
            {
                BuildHazard(room, hazardSpawn);
            }
        }

        private void BuildHazard(Room room, HazardSpawn hazardData)
        {
            GameObject hazardObj;

            if (hazardPrefab != null)
            {
                hazardObj = Instantiate(hazardPrefab, room.transform);
            }
            else
            {
                hazardObj = new GameObject($"Hazard_{hazardData.hazardType}");
                hazardObj.transform.SetParent(room.transform);

                var collider = hazardObj.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                collider.size = hazardData.size;

                var sr = hazardObj.AddComponent<SpriteRenderer>();
                sr.color = GetHazardColor(hazardData.hazardType);
            }

            hazardObj.transform.localPosition = hazardData.position;

            var hazard = hazardObj.GetComponent<Hazard>();
            if (hazard == null)
            {
                hazard = hazardObj.AddComponent<Hazard>();
            }

            hazard.Initialize(hazardData);
        }

        public Vector2 GetSpawnPosition(RoomData roomData, string spawnPointId)
        {
            if (string.IsNullOrEmpty(spawnPointId))
            {
                return Vector2.zero;
            }

            // Check defined spawn points
            foreach (var spawn in roomData.playerSpawnPoints)
            {
                if (spawn.spawnId == spawnPointId)
                {
                    return spawn.position;
                }
            }

            // Default positions based on spawn ID
            return spawnPointId.ToLower() switch
            {
                "north_door" => new Vector2(0, 3),
                "south_door" => new Vector2(0, -3),
                "east_door" => new Vector2(6, 0),
                "west_door" => new Vector2(-6, 0),
                "stairs_up" => new Vector2(3, 2),
                "stairs_down" => new Vector2(-3, 2),
                "trapdoor_exit" => new Vector2(0, -2),
                "center" or "start" => Vector2.zero,
                _ => Vector2.zero
            };
        }

        /// <summary>
        /// Gets spawn position in current room by spawn point ID.
        /// </summary>
        public Vector2 GetSpawnPosition(string spawnPointId)
        {
            return GetSpawnPosition(CurrentRoomData, spawnPointId);
        }

        /// <summary>
        /// Gets the pending spawn position for player placement.
        /// </summary>
        public Vector2 GetPendingSpawnPosition()
        {
            return _pendingSpawnPosition;
        }

        private DoorConnection GetDoorConnection(RoomData roomData, DoorDirection direction)
        {
            return direction switch
            {
                DoorDirection.North => roomData.northDoor,
                DoorDirection.East => roomData.eastDoor,
                DoorDirection.South => roomData.southDoor,
                DoorDirection.West => roomData.westDoor,
                _ => null
            };
        }

        private DoorDirection GetOppositeDirection(DoorDirection direction)
        {
            return direction switch
            {
                DoorDirection.North => DoorDirection.South,
                DoorDirection.South => DoorDirection.North,
                DoorDirection.East => DoorDirection.West,
                DoorDirection.West => DoorDirection.East,
                _ => DoorDirection.North
            };
        }

        private Color GetDoorColor(DoorType doorType, KeyColor keyColor)
        {
            if (doorType == DoorType.Locked)
            {
                return keyColor switch
                {
                    KeyColor.Red => Color.red,
                    KeyColor.Blue => Color.blue,
                    KeyColor.Green => Color.green,
                    KeyColor.Yellow => Color.yellow,
                    KeyColor.Cyan => Color.cyan,
                    KeyColor.Magenta => Color.magenta,
                    _ => Color.gray
                };
            }
            return Color.white;
        }

        private Color GetSecretPassageColor(SecretPassageType type)
        {
            return type switch
            {
                SecretPassageType.Bookcase => new Color(0.4f, 0.2f, 0.1f), // Brown
                SecretPassageType.Clock => new Color(0.6f, 0.5f, 0.2f),    // Gold
                SecretPassageType.Barrel => new Color(0.5f, 0.3f, 0.1f),   // Dark brown
                _ => Color.gray
            };
        }

        private Color GetHazardColor(HazardType type)
        {
            return type switch
            {
                HazardType.Spikes => Color.gray,
                HazardType.Fire => new Color(1f, 0.5f, 0f),
                HazardType.Poison => new Color(0.5f, 1f, 0f),
                HazardType.Acid => new Color(0.8f, 1f, 0f),
                HazardType.Electricity => new Color(0.5f, 0.5f, 1f),
                _ => Color.red
            };
        }

        public bool IsTransitioning => _isTransitioning;
    }

    public enum DoorDirection
    {
        North,
        East,
        South,
        West
    }

    public enum FloorTransitionType
    {
        StairsUp,
        StairsDown,
        Trapdoor
    }
}
