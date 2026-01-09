using UnityEngine;
using HauntedCastle.Services;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Debug UI for testing room transitions.
    /// Shows current room info and allows manual transitions.
    /// </summary>
    public class RoomDebugUI : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool showDebugUI = true;
        [SerializeField] private KeyCode toggleKey = KeyCode.F1;

        private void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                showDebugUI = !showDebugUI;
            }
        }

        private void OnGUI()
        {
            if (!showDebugUI) return;

            GUILayout.BeginArea(new Rect(10, 10, 250, 400));
            GUILayout.BeginVertical("box");

            GUILayout.Label("=== Room Debug (F1 to toggle) ===");

            if (RoomManager.Instance != null)
            {
                var currentRoom = RoomManager.Instance.CurrentRoomData;

                if (currentRoom != null)
                {
                    GUILayout.Label($"Room: {currentRoom.displayName}");
                    GUILayout.Label($"ID: {currentRoom.roomId}");
                    GUILayout.Label($"Floor: {currentRoom.floorNumber}");
                    GUILayout.Label($"Type: {currentRoom.roomType}");

                    GUILayout.Space(10);
                    GUILayout.Label("--- Doors ---");

                    if (currentRoom.northDoor?.exists == true)
                    {
                        if (GUILayout.Button($"North -> {currentRoom.northDoor.destinationRoomId}"))
                        {
                            RoomManager.Instance.TransitionThroughDoor(DoorDirection.North);
                        }
                    }

                    if (currentRoom.southDoor?.exists == true)
                    {
                        if (GUILayout.Button($"South -> {currentRoom.southDoor.destinationRoomId}"))
                        {
                            RoomManager.Instance.TransitionThroughDoor(DoorDirection.South);
                        }
                    }

                    if (currentRoom.eastDoor?.exists == true)
                    {
                        string doorInfo = currentRoom.eastDoor.doorType == Data.DoorType.Locked
                            ? $" [{currentRoom.eastDoor.requiredKeyColor}]"
                            : "";
                        if (GUILayout.Button($"East -> {currentRoom.eastDoor.destinationRoomId}{doorInfo}"))
                        {
                            RoomManager.Instance.TransitionThroughDoor(DoorDirection.East);
                        }
                    }

                    if (currentRoom.westDoor?.exists == true)
                    {
                        if (GUILayout.Button($"West -> {currentRoom.westDoor.destinationRoomId}"))
                        {
                            RoomManager.Instance.TransitionThroughDoor(DoorDirection.West);
                        }
                    }

                    GUILayout.Space(10);
                    GUILayout.Label("--- Floor Transitions ---");

                    if (currentRoom.stairsUp?.exists == true)
                    {
                        if (GUILayout.Button($"Stairs Up -> {currentRoom.stairsUp.destinationRoomId}"))
                        {
                            RoomManager.Instance.TransitionThroughFloor(FloorTransitionType.StairsUp);
                        }
                    }

                    if (currentRoom.stairsDown?.exists == true)
                    {
                        if (GUILayout.Button($"Stairs Down -> {currentRoom.stairsDown.destinationRoomId}"))
                        {
                            RoomManager.Instance.TransitionThroughFloor(FloorTransitionType.StairsDown);
                        }
                    }

                    if (currentRoom.trapdoor?.exists == true)
                    {
                        if (GUILayout.Button($"Trapdoor -> {currentRoom.trapdoor.destinationRoomId}"))
                        {
                            RoomManager.Instance.TransitionThroughFloor(FloorTransitionType.Trapdoor);
                        }
                    }

                    GUILayout.Space(10);
                    GUILayout.Label("--- Secret Passages ---");

                    foreach (var passage in currentRoom.secretPassages)
                    {
                        string charRequired = passage.passageType switch
                        {
                            Data.SecretPassageType.Bookcase => "Wizard",
                            Data.SecretPassageType.Clock => "Knight",
                            Data.SecretPassageType.Barrel => "Serf",
                            _ => "?"
                        };

                        if (GUILayout.Button($"{passage.passageType} ({charRequired}) -> {passage.destinationRoomId}"))
                        {
                            RoomManager.Instance.TransitionThroughSecretPassage(
                                passage.passageType,
                                passage.destinationRoomId);
                        }
                    }
                }
                else
                {
                    GUILayout.Label("No room loaded");

                    if (GUILayout.Button("Load Starting Room"))
                    {
                        RoomManager.Instance.LoadStartingRoom();
                    }
                }

                GUILayout.Space(10);
                GUILayout.Label($"Transitioning: {RoomManager.Instance.IsTransitioning}");
            }
            else
            {
                GUILayout.Label("RoomManager not found!");
            }

            GUILayout.Space(10);
            GUILayout.Label("--- Game State ---");

            if (GameManager.Instance != null)
            {
                GUILayout.Label($"State: {GameManager.Instance.CurrentState}");
                GUILayout.Label($"Character: {GameManager.Instance.SelectedCharacter}");

                GUILayout.Space(5);
                GUILayout.Label("Select Character:");

                if (GUILayout.Button("Wizard"))
                    GameManager.Instance.SelectedCharacter = CharacterType.Wizard;
                if (GUILayout.Button("Knight"))
                    GameManager.Instance.SelectedCharacter = CharacterType.Knight;
                if (GUILayout.Button("Serf"))
                    GameManager.Instance.SelectedCharacter = CharacterType.Serf;
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
}
