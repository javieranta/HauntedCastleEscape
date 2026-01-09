using UnityEngine;

namespace HauntedCastle.Rooms
{
    /// <summary>
    /// Marks a spawn point for the player in a room.
    /// Used by RoomManager to position player after transitions.
    /// </summary>
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [Header("Spawn Point Configuration")]
        [SerializeField] private string spawnId = "default";
        [SerializeField] private SpawnPointType spawnType = SpawnPointType.Default;

        [Header("Visual (Editor Only)")]
        [SerializeField] private Color gizmoColor = Color.green;
        [SerializeField] private float gizmoRadius = 0.3f;

        public string SpawnId => spawnId;
        public SpawnPointType SpawnType => spawnType;
        public Vector2 Position => transform.position;

        /// <summary>
        /// Sets up this spawn point with the given ID and type.
        /// </summary>
        public void Setup(string id, SpawnPointType type)
        {
            spawnId = id;
            spawnType = type;
            gameObject.name = $"SpawnPoint_{id}";
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoRadius);

            // Draw direction indicator
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.5f);

            // Draw spawn type icon
            DrawSpawnTypeGizmo();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, gizmoRadius * 1.5f);

#if UNITY_EDITOR
            // Draw label in editor
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, spawnId);
#endif
        }

        private void DrawSpawnTypeGizmo()
        {
            Vector3 pos = transform.position;

            switch (spawnType)
            {
                case SpawnPointType.NorthDoor:
                    Gizmos.DrawLine(pos, pos + Vector3.down * 0.3f);
                    break;
                case SpawnPointType.SouthDoor:
                    Gizmos.DrawLine(pos, pos + Vector3.up * 0.3f);
                    break;
                case SpawnPointType.EastDoor:
                    Gizmos.DrawLine(pos, pos + Vector3.left * 0.3f);
                    break;
                case SpawnPointType.WestDoor:
                    Gizmos.DrawLine(pos, pos + Vector3.right * 0.3f);
                    break;
                case SpawnPointType.StairsUp:
                case SpawnPointType.StairsDown:
                    Gizmos.DrawWireCube(pos, Vector3.one * 0.3f);
                    break;
                case SpawnPointType.SecretPassage:
                    // Draw star shape
                    for (int i = 0; i < 5; i++)
                    {
                        float angle1 = i * 72f * Mathf.Deg2Rad;
                        float angle2 = (i + 2) * 72f * Mathf.Deg2Rad;
                        Vector3 p1 = pos + new Vector3(Mathf.Sin(angle1), Mathf.Cos(angle1), 0) * 0.2f;
                        Vector3 p2 = pos + new Vector3(Mathf.Sin(angle2), Mathf.Cos(angle2), 0) * 0.2f;
                        Gizmos.DrawLine(p1, p2);
                    }
                    break;
            }
        }
    }

    public enum SpawnPointType
    {
        Default,
        NorthDoor,
        SouthDoor,
        EastDoor,
        WestDoor,
        StairsUp,
        StairsDown,
        Trapdoor,
        SecretPassage,
        Start,
        Respawn
    }
}
