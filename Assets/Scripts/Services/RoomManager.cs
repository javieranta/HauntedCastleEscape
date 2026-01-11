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
            Debug.LogWarning($"[FLOOR TRANSITION] TransitionThroughFloor called with type: {transitionType}");
            Debug.LogWarning($"[FLOOR TRANSITION] CurrentRoomData: {(CurrentRoomData != null ? CurrentRoomData.roomId : "NULL")}");
            Debug.LogWarning($"[FLOOR TRANSITION] _isTransitioning: {_isTransitioning}");

            if (CurrentRoomData == null)
            {
                Debug.LogError($"[FLOOR TRANSITION] FAILED: CurrentRoomData is NULL!");
                return;
            }

            if (_isTransitioning)
            {
                Debug.LogWarning($"[FLOOR TRANSITION] BLOCKED: Already transitioning!");
                return;
            }

            FloorTransition transition = transitionType switch
            {
                FloorTransitionType.StairsUp => CurrentRoomData.stairsUp,
                FloorTransitionType.StairsDown => CurrentRoomData.stairsDown,
                FloorTransitionType.Trapdoor => CurrentRoomData.trapdoor,
                _ => null
            };

            Debug.LogWarning($"[FLOOR TRANSITION] Transition object: {(transition != null ? "EXISTS" : "NULL")}");
            if (transition != null)
            {
                Debug.LogWarning($"[FLOOR TRANSITION] Transition.exists: {transition.exists}");
                Debug.LogWarning($"[FLOOR TRANSITION] Transition.destinationRoomId: {transition.destinationRoomId}");
            }

            if (transition == null)
            {
                Debug.LogError($"[FLOOR TRANSITION] FAILED: No {transitionType} defined for room {CurrentRoomData.roomId}!");
                return;
            }

            if (!transition.exists)
            {
                Debug.LogError($"[FLOOR TRANSITION] FAILED: {transitionType} exists=false for room {CurrentRoomData.roomId}!");
                return;
            }

            string spawnId = transitionType switch
            {
                FloorTransitionType.StairsUp => "stairs_down",
                FloorTransitionType.StairsDown => "stairs_up",
                FloorTransitionType.Trapdoor => "trapdoor_exit",
                _ => "center"
            };

            Debug.LogWarning($"[FLOOR TRANSITION] *** INITIATING TRANSITION ***");
            Debug.LogWarning($"[FLOOR TRANSITION] From: {CurrentRoomData.roomId} -> To: {transition.destinationRoomId}");
            Debug.LogWarning($"[FLOOR TRANSITION] SpawnId: {spawnId}");

            OnRoomTransition?.Invoke(CurrentRoomData.roomId, transition.destinationRoomId);
            LoadRoom(transition.destinationRoomId, spawnId);

            Debug.LogWarning($"[FLOOR TRANSITION] LoadRoom call completed");
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

            Debug.LogWarning($"[RoomManager] ============ STARTING ROOM TRANSITION ============");
            Debug.LogWarning($"[RoomManager] Loading room: {roomData.roomId} ({roomData.displayName}) on floor {roomData.floorNumber}");

            OnRoomLoadStarted?.Invoke(roomData);

            // Fade out
            if (TransitionManager.Instance != null)
            {
                Debug.Log("[RoomManager] Fading out...");
                yield return TransitionManager.Instance.FadeOut(transitionDuration);
            }

            // CRITICAL: Wrap room building in try-finally to ensure fade clears even on error
            bool buildSuccess = false;
            try
            {
                // Unload current room
                Debug.Log("[RoomManager] Unloading current room...");
                UnloadCurrentRoom();

                // Store previous room
                if (CurrentRoomData != null)
                {
                    PreviousRoomId = CurrentRoomData.roomId;
                }

                // Set new room data
                CurrentRoomData = roomData;

                // Build new room
                Debug.Log("[RoomManager] Building new room...");
                CurrentRoom = BuildRoom(roomData);
                Debug.Log($"[RoomManager] Room built: {CurrentRoom?.name ?? "NULL"}");

                // Determine spawn position
                _pendingSpawnPosition = GetSpawnPosition(roomData, spawnPointId);
                Debug.Log($"[RoomManager] Spawn position: {_pendingSpawnPosition}");

                // Notify listeners (player will reposition)
                OnRoomLoadCompleted?.Invoke(roomData);

                // CRITICAL: Snap camera to room center to ensure visibility
                SnapCameraToRoom();

                buildSuccess = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[RoomManager] ERROR during room building: {e.Message}\n{e.StackTrace}");
            }

            // ALWAYS fade in, even if there was an error
            if (TransitionManager.Instance != null)
            {
                Debug.Log("[RoomManager] Fading in...");
                yield return TransitionManager.Instance.FadeIn(transitionDuration);
            }
            else
            {
                Debug.LogWarning("[RoomManager] No TransitionManager - can't fade in!");
            }

            _isTransitioning = false;

            // Play room enter sound
            if (buildSuccess)
            {
                AudioManager.Instance?.PlaySFX(SoundEffect.DoorOpen);
            }

            Debug.LogWarning($"[RoomManager] ============ ROOM LOADED ============");
            Debug.LogWarning($"[RoomManager] Room ID: {roomData.roomId}");
            Debug.LogWarning($"[RoomManager] Room Name: {roomData.displayName}");
            Debug.LogWarning($"[RoomManager] Floor Number: {roomData.floorNumber}");
            Debug.LogWarning($"[RoomManager] Room position: {CurrentRoom?.transform.position}");
            Debug.LogWarning($"[RoomManager] Camera position: {Camera.main?.transform.position}");
            Debug.LogWarning($"[RoomManager] Build success: {buildSuccess}");
            Debug.LogWarning($"[RoomManager] ========================================");

            // Run diagnostics AFTER fade completes
            RunPostTransitionDiagnostics();

            // SAFETY: Start a delayed check to ensure fade cleared
            // This catches edge cases where the fade-in coroutine might have been interrupted
            StartCoroutine(DelayedFadeCheck());
        }

        /// <summary>
        /// Safety check that runs 1 second after transition to ensure fade is cleared.
        /// </summary>
        private IEnumerator DelayedFadeCheck()
        {
            yield return new WaitForSecondsRealtime(1.0f);

            if (TransitionManager.Instance != null)
            {
                float fadeAlpha = TransitionManager.Instance.CurrentFadeAlpha;
                if (fadeAlpha > 0.05f)
                {
                    Debug.LogError($"[RoomManager] SAFETY: Fade still at {fadeAlpha} after 1 second! Force clearing...");
                    TransitionManager.Instance.ForceTransparent();
                }
            }
        }

        /// <summary>
        /// Runs diagnostics after room transition is complete (after fade in).
        /// </summary>
        private void RunPostTransitionDiagnostics()
        {
            Debug.LogWarning("=================================================================");
            Debug.LogWarning("[POST-TRANSITION] ========== POST-TRANSITION CHECK ==========");

            // Check fade state - should be 0 now
            if (TransitionManager.Instance != null)
            {
                float fadeAlpha = TransitionManager.Instance.CurrentFadeAlpha;
                Debug.LogWarning($"[POST-TRANSITION] Fade Alpha: {fadeAlpha}");

                if (fadeAlpha > 0.1f)
                {
                    Debug.LogError($"[POST-TRANSITION] FADE IS STILL VISIBLE! Alpha = {fadeAlpha}. This is likely the cause of darkness!");
                    Debug.LogError($"[POST-TRANSITION] Force-clearing fade...");
                    TransitionManager.Instance.ForceTransparent();
                }
                else
                {
                    Debug.LogWarning($"[POST-TRANSITION] Fade is properly cleared (alpha = {fadeAlpha})");
                }
            }

            // Check camera
            if (Camera.main != null)
            {
                Debug.LogWarning($"[POST-TRANSITION] Camera at: {Camera.main.transform.position}");
                Debug.LogWarning($"[POST-TRANSITION] Camera orthoSize: {Camera.main.orthographicSize}");
            }

            // Check room
            if (CurrentRoom != null)
            {
                Debug.LogWarning($"[POST-TRANSITION] Room at: {CurrentRoom.transform.position}");

                // Find all SpriteRenderers in the room
                var renderers = CurrentRoom.GetComponentsInChildren<SpriteRenderer>();
                Debug.LogWarning($"[POST-TRANSITION] Found {renderers.Length} SpriteRenderers in room");

                int visibleCount = 0;
                foreach (var sr in renderers)
                {
                    if (sr.enabled && sr.color.a > 0 && sr.sprite != null)
                    {
                        visibleCount++;
                    }
                }
                Debug.LogWarning($"[POST-TRANSITION] {visibleCount} of {renderers.Length} renderers are potentially visible");

                if (visibleCount == 0)
                {
                    Debug.LogError("[POST-TRANSITION] NO VISIBLE SPRITES IN ROOM! This is likely the cause of darkness!");
                }

                // Log details of first few renderers for debugging
                int count = 0;
                foreach (var sr in renderers)
                {
                    if (count >= 5) break;
                    Debug.LogWarning($"[POST-TRANSITION] Renderer: {sr.gameObject.name} | enabled={sr.enabled} | sprite={(sr.sprite != null ? sr.sprite.name : "NULL")} | color={sr.color} | layer={sr.sortingLayerName}/{sr.sortingOrder} | pos={sr.transform.position}");
                    count++;
                }
            }

            // Check if sorting layers exist
            Debug.LogWarning($"[POST-TRANSITION] Checking sorting layers...");
            try
            {
                // Try to access common layers used by the game
                string[] layersToCheck = { "Background", "Walls", "Items", "Enemies", "Player", "Projectiles", "UI" };
                foreach (var layer in layersToCheck)
                {
                    int layerId = SortingLayer.NameToID(layer);
                    Debug.LogWarning($"[POST-TRANSITION] Layer '{layer}' -> ID: {layerId} (0 = Default/not found)");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[POST-TRANSITION] Error checking sorting layers: {e.Message}");
            }

            Debug.LogWarning("=================================================================");
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

        /// <summary>
        /// Snaps the camera to the current room's center position.
        /// This ensures the room is always visible after a transition.
        /// </summary>
        private void SnapCameraToRoom()
        {
            if (Camera.main == null)
            {
                Debug.LogWarning("[RoomManager] No main camera found for snapping!");
                return;
            }

            Vector3 targetPos;

            // If we have a current room, snap to its center
            if (CurrentRoom != null)
            {
                targetPos = CurrentRoom.transform.position;
            }
            // If room container exists, snap to its position (should be 0,0,0)
            else if (roomContainer != null)
            {
                targetPos = roomContainer.position;
            }
            // Fallback to origin
            else
            {
                targetPos = Vector3.zero;
            }

            // Preserve camera's Z position (usually -10 for 2D games)
            float cameraZ = Camera.main.transform.position.z;
            Camera.main.transform.position = new Vector3(targetPos.x, targetPos.y, cameraZ);

            Debug.Log($"[RoomManager] Camera snapped to: {Camera.main.transform.position}");

            // Run comprehensive diagnostics
            RunVisibilityDiagnostics();
        }

        /// <summary>
        /// DIAGNOSTIC: Comprehensive check for why room might not be visible.
        /// Creates a guaranteed visible test object and logs all relevant state.
        /// </summary>
        private void RunVisibilityDiagnostics()
        {
            Debug.LogWarning("=================================================================");
            Debug.LogWarning("[DIAGNOSTIC] ========== VISIBILITY DIAGNOSTICS ==========");
            Debug.LogWarning("=================================================================");

            // 1. Camera state
            var cam = Camera.main;
            if (cam != null)
            {
                Debug.LogWarning($"[DIAGNOSTIC] Camera.main found: {cam.name}");
                Debug.LogWarning($"[DIAGNOSTIC] Camera position: {cam.transform.position}");
                Debug.LogWarning($"[DIAGNOSTIC] Camera orthographic: {cam.orthographic}");
                Debug.LogWarning($"[DIAGNOSTIC] Camera orthographicSize: {cam.orthographicSize}");
                Debug.LogWarning($"[DIAGNOSTIC] Camera nearClipPlane: {cam.nearClipPlane}");
                Debug.LogWarning($"[DIAGNOSTIC] Camera farClipPlane: {cam.farClipPlane}");
                Debug.LogWarning($"[DIAGNOSTIC] Camera cullingMask: {cam.cullingMask} (binary: {System.Convert.ToString(cam.cullingMask, 2)})");
                Debug.LogWarning($"[DIAGNOSTIC] Camera enabled: {cam.enabled}");
                Debug.LogWarning($"[DIAGNOSTIC] Camera gameObject active: {cam.gameObject.activeInHierarchy}");
            }
            else
            {
                Debug.LogError("[DIAGNOSTIC] Camera.main is NULL!");
            }

            // 2. Room Container state
            if (roomContainer != null)
            {
                Debug.LogWarning($"[DIAGNOSTIC] RoomContainer position: {roomContainer.position}");
                Debug.LogWarning($"[DIAGNOSTIC] RoomContainer localPosition: {roomContainer.localPosition}");
                Debug.LogWarning($"[DIAGNOSTIC] RoomContainer childCount: {roomContainer.childCount}");
                Debug.LogWarning($"[DIAGNOSTIC] RoomContainer active: {roomContainer.gameObject.activeInHierarchy}");
            }
            else
            {
                Debug.LogError("[DIAGNOSTIC] RoomContainer is NULL!");
            }

            // 3. Current Room state
            if (CurrentRoom != null)
            {
                Debug.LogWarning($"[DIAGNOSTIC] CurrentRoom: {CurrentRoom.name}");
                Debug.LogWarning($"[DIAGNOSTIC] CurrentRoom position: {CurrentRoom.transform.position}");
                Debug.LogWarning($"[DIAGNOSTIC] CurrentRoom localPosition: {CurrentRoom.transform.localPosition}");
                Debug.LogWarning($"[DIAGNOSTIC] CurrentRoom active: {CurrentRoom.gameObject.activeInHierarchy}");
                Debug.LogWarning($"[DIAGNOSTIC] CurrentRoom childCount: {CurrentRoom.transform.childCount}");

                // List all children
                for (int i = 0; i < CurrentRoom.transform.childCount; i++)
                {
                    var child = CurrentRoom.transform.GetChild(i);
                    Debug.LogWarning($"[DIAGNOSTIC]   Child {i}: {child.name} at {child.position}");
                }
            }
            else
            {
                Debug.LogError("[DIAGNOSTIC] CurrentRoom is NULL!");
            }

            // 4. TransitionManager fade state
            if (TransitionManager.Instance != null)
            {
                Debug.LogWarning($"[DIAGNOSTIC] TransitionManager exists");
                Debug.LogWarning($"[DIAGNOSTIC] TransitionManager.IsFading: {TransitionManager.Instance.IsFading}");
                Debug.LogWarning($"[DIAGNOSTIC] TransitionManager.CurrentFadeAlpha: {TransitionManager.Instance.CurrentFadeAlpha}");
                // Note: Fade SHOULD be at 1.0 here since we're mid-transition. Will check again after fade completes.
            }
            else
            {
                Debug.LogError("[DIAGNOSTIC] TransitionManager.Instance is NULL!");
            }

            Debug.LogWarning("=================================================================");
            Debug.LogWarning("[DIAGNOSTIC] ========== END DIAGNOSTICS ==========");
            Debug.LogWarning("=================================================================");
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
                // Use subtle colors - the FloorTransitionTrigger will set proper sprite
                sr.color = Color.white;
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
