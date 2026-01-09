using System;
using System.Collections.Generic;
using UnityEngine;

namespace HauntedCastle.Data
{
    /// <summary>
    /// ScriptableObject defining room layout, connections, and spawn data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewRoom", menuName = "Haunted Castle/Room Data")]
    public class RoomData : ScriptableObject
    {
        [Header("Identity")]
        public string roomId;
        public string displayName;
        public int floorNumber; // 0-4 for 5 floors

        [Header("Room Type")]
        public RoomType roomType;
        public bool isStartRoom = false;
        public bool isExitRoom = false;

        [Header("Doors")]
        public DoorConnection northDoor;
        public DoorConnection eastDoor;
        public DoorConnection southDoor;
        public DoorConnection westDoor;

        [Header("Floor Transitions")]
        public FloorTransition stairsUp;
        public FloorTransition stairsDown;
        public FloorTransition trapdoor;

        [Header("Secret Passages")]
        public List<SecretPassage> secretPassages = new();

        [Header("Spawn Points")]
        public List<SpawnPoint> playerSpawnPoints = new();
        public List<EnemySpawn> enemySpawns = new();
        public List<ItemSpawn> itemSpawns = new();
        public List<HazardSpawn> hazardSpawns = new();

        [Header("Visual")]
        public Sprite backgroundSprite;
        public Color ambientColor = Color.white;

        [Header("Audio")]
        public AudioClip ambientSound;
    }

    public enum RoomType
    {
        Normal,
        Corridor,
        Hall,
        Chamber,
        Crypt,
        Tower,
        Dungeon,
        Kitchen,
        Library,
        Throne
    }

    [Serializable]
    public class DoorConnection
    {
        public bool exists = false;
        public DoorType doorType = DoorType.Open;
        public KeyColor requiredKeyColor = KeyColor.None;
        public string destinationRoomId;
        public Vector2 spawnOffset; // Where player appears when entering through this door
    }

    public enum DoorType
    {
        Open,           // Always passable
        Locked,         // Requires colored key
        OneWay,         // Can only pass in one direction
        Hidden          // Not visible until approached
    }

    [Serializable]
    public class FloorTransition
    {
        public bool exists = false;
        public string destinationRoomId;
        public Vector2 position;
        public Vector2 spawnOffset;
    }

    [Serializable]
    public class SecretPassage
    {
        public SecretPassageType passageType;
        public string destinationRoomId;
        public Vector2 position;
        public Vector2 spawnOffset;
        public Sprite disguiseSprite; // Bookcase, clock, barrel appearance
    }

    [Serializable]
    public class SpawnPoint
    {
        public string spawnId; // e.g., "north_door", "south_door", "stairs_up"
        public Vector2 position;
    }

    [Serializable]
    public class EnemySpawn
    {
        public EnemyData enemyData;
        public Vector2 position;
        [Range(0f, 1f)]
        public float spawnChance = 1f;
        public bool respawnsOnReentry = true;
    }

    [Serializable]
    public class ItemSpawn
    {
        public ItemData itemData;
        public Vector2 position;
        public bool persistsAcrossVisits = false; // True for key pieces
        public bool randomizePosition = false;
    }

    [Serializable]
    public class HazardSpawn
    {
        public HazardType hazardType;
        public Vector2 position;
        public Vector2 size;
        public float damagePerSecond = 10f;
    }

    public enum HazardType
    {
        Spikes,
        Fire,
        Poison,
        Acid,
        Electricity
    }
}
