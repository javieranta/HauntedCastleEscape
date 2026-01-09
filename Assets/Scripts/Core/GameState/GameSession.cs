using System;
using System.Collections.Generic;

namespace HauntedCastle.Core.GameState
{
    /// <summary>
    /// Represents a single game session (run).
    /// Contains all persistent state for the current playthrough.
    /// </summary>
    [Serializable]
    public class GameSession
    {
        // Static selected character (persists across sessions until changed)
        public static HauntedCastle.Services.CharacterType SelectedCharacter { get; set; } = HauntedCastle.Services.CharacterType.Knight;

        // Player state
        public int Lives { get; set; } = 3;
        public float Energy { get; set; } = 100f;
        public float MaxEnergy { get; set; } = 100f;
        public int Score { get; set; } = 0;

        // Progression
        public int CurrentFloor { get; set; } = 0;
        public string CurrentRoomId { get; set; } = "entrance";
        public bool[] KeyPiecesCollected { get; private set; } = new bool[3];
        public bool HasGreatKey { get; set; } = false;

        // Death markers (roomId -> position)
        public Dictionary<string, DeathMarker> DeathMarkers { get; private set; } = new();

        // Inventory
        public List<string> InventoryItemIds { get; private set; } = new();
        public const int MaxInventorySize = 3;

        // Session timing
        public float PlayTime { get; set; } = 0f;
        public DateTime StartTime { get; private set; }

        // Random seed for procedural generation
        public int Seed { get; private set; }

        public GameSession()
        {
            StartTime = DateTime.Now;
            Seed = Environment.TickCount;
        }

        public GameSession(int seed)
        {
            StartTime = DateTime.Now;
            Seed = seed;
        }

        public void CollectKeyPiece(int index)
        {
            if (index >= 0 && index < 3)
            {
                KeyPiecesCollected[index] = true;
                CheckForGreatKey();
            }
        }

        private void CheckForGreatKey()
        {
            if (KeyPiecesCollected[0] && KeyPiecesCollected[1] && KeyPiecesCollected[2])
            {
                HasGreatKey = true;
            }
        }

        public int GetCollectedKeyPieceCount()
        {
            int count = 0;
            foreach (bool collected in KeyPiecesCollected)
            {
                if (collected) count++;
            }
            return count;
        }

        public bool TryAddToInventory(string itemId)
        {
            if (InventoryItemIds.Count < MaxInventorySize)
            {
                InventoryItemIds.Add(itemId);
                return true;
            }
            return false;
        }

        public bool RemoveFromInventory(string itemId)
        {
            return InventoryItemIds.Remove(itemId);
        }

        public bool HasItem(string itemId)
        {
            return InventoryItemIds.Contains(itemId);
        }

        public void AddDeathMarker(string roomId, float x, float y)
        {
            DeathMarkers[roomId] = new DeathMarker(x, y);
        }

        public void LoseLife()
        {
            Lives--;
            Energy = MaxEnergy; // Restore energy on respawn
        }

        public bool IsGameOver()
        {
            return Lives <= 0;
        }

        public void AddScore(int points)
        {
            Score += points;
        }

        public void RestoreEnergy(float amount)
        {
            Energy = Math.Min(Energy + amount, MaxEnergy);
        }

        public void DrainEnergy(float amount)
        {
            Energy = Math.Max(Energy - amount, 0f);
        }
    }

    [Serializable]
    public struct DeathMarker
    {
        public float X;
        public float Y;

        public DeathMarker(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
