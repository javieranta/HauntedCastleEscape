using UnityEngine;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Initializes all visual systems for the game.
    /// Ensures RoomVisualizer, AudioListener, and other visual components are present.
    /// </summary>
    public class GameVisualInitializer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool setupRoomVisualizer = true;
        [SerializeField] private bool ensureAudioListener = true;
        [SerializeField] private bool setupLighting = true;

        private void Awake()
        {
            InitializeVisualSystems();
        }

        private void InitializeVisualSystems()
        {
            // Ensure AudioListener exists on main camera
            if (ensureAudioListener)
            {
                SetupAudioListener();
            }

            // Setup RoomVisualizer
            if (setupRoomVisualizer)
            {
                SetupRoomVisualizer();
            }

            // Setup basic lighting
            if (setupLighting)
            {
                SetupLighting();
            }

            Debug.Log("[GameVisualInitializer] Visual systems initialized");
        }

        private void SetupAudioListener()
        {
            // Check if AudioListener exists anywhere
            var existingListener = FindObjectOfType<AudioListener>();

            if (existingListener == null)
            {
                // Try to add to main camera
                var mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    mainCamera.gameObject.AddComponent<AudioListener>();
                    Debug.Log("[GameVisualInitializer] Added AudioListener to main camera");
                }
                else
                {
                    // Add to this object as fallback
                    gameObject.AddComponent<AudioListener>();
                    Debug.Log("[GameVisualInitializer] Added AudioListener to GameVisualInitializer");
                }
            }
        }

        private void SetupRoomVisualizer()
        {
            // Check if RoomVisualizer exists
            if (RoomVisualizer.Instance == null)
            {
                var visualizerObj = new GameObject("RoomVisualizer");
                visualizerObj.AddComponent<RoomVisualizer>();
                Debug.Log("[GameVisualInitializer] Created RoomVisualizer");
            }
        }

        private void SetupLighting()
        {
            // Set ambient light for castle atmosphere
            RenderSettings.ambientLight = new Color(0.3f, 0.3f, 0.35f);

            // If using URP, you might need different settings
            // For built-in render pipeline, this creates a moody atmosphere
        }

        /// <summary>
        /// Creates a simple point light for local illumination.
        /// </summary>
        public static GameObject CreatePointLight(Vector3 position, Color color, float intensity = 1f, float range = 5f)
        {
            var lightObj = new GameObject("PointLight");
            lightObj.transform.position = position;

            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;

            return lightObj;
        }
    }
}
