using UnityEngine;
using UnityEngine.U2D;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Sets up pixel-perfect rendering for the game camera.
    /// Attach to the main camera in each scene.
    ///
    /// NOTE: For modern HD graphics (Midjourney textures), set useModernHDMode = true
    /// to disable low-res pixel perfect rendering.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class PixelPerfectSetup : MonoBehaviour
    {
        [Header("Graphics Mode")]
        [Tooltip("Enable for modern HD Midjourney textures. Disable for retro pixel art look.")]
        [SerializeField] private bool useModernHDMode = true;

        [Header("Resolution Settings (Retro Mode Only)")]
        [SerializeField] private int referenceResolutionX = 256;
        [SerializeField] private int referenceResolutionY = 192;
        [SerializeField] private int pixelsPerUnit = 16;

        [Header("HD Mode Settings")]
        [SerializeField] private int hdResolutionX = 1920;
        [SerializeField] private int hdResolutionY = 1080;
        [SerializeField] private int hdPixelsPerUnit = 100;

        [Header("Scaling Options")]
        [SerializeField] private bool upscaleRenderTexture = true;
        [SerializeField] private bool pixelSnapping = false; // Disabled for smooth HD movement
        [SerializeField] private bool cropFrameX = false;
        [SerializeField] private bool cropFrameY = false;
        [SerializeField] private bool stretchFill = false;

        private Camera _camera;
        private PixelPerfectCamera _pixelPerfectCamera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();

            if (useModernHDMode)
            {
                SetupModernHDCamera();
            }
            else
            {
                SetupPixelPerfectCamera();
            }
        }

        private void SetupModernHDCamera()
        {
            // For modern HD mode, disable or remove pixel perfect camera
            _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

            if (_pixelPerfectCamera != null)
            {
                // Disable the pixel perfect camera for HD rendering
                _pixelPerfectCamera.enabled = false;
                Debug.Log("[PixelPerfectSetup] HD Mode: Disabled PixelPerfectCamera for high-quality rendering");
            }

            // Ensure camera is set to orthographic with good size for the room
            _camera.orthographic = true;
            _camera.orthographicSize = 6f; // Adjust as needed for room visibility

            // Set a dark stone-like background color instead of pure black
            _camera.backgroundColor = new Color(0.15f, 0.13f, 0.12f); // Dark gray-brown
            _camera.clearFlags = CameraClearFlags.SolidColor;

            Debug.Log("[PixelPerfectSetup] HD Mode enabled - using native resolution for crisp Midjourney textures");
        }

        private void SetupPixelPerfectCamera()
        {
            // Check if PixelPerfectCamera component exists
            _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

            if (_pixelPerfectCamera == null)
            {
                _pixelPerfectCamera = gameObject.AddComponent<PixelPerfectCamera>();
            }

            // Configure pixel perfect settings for retro look
            _pixelPerfectCamera.enabled = true;
            _pixelPerfectCamera.assetsPPU = pixelsPerUnit;
            _pixelPerfectCamera.refResolutionX = referenceResolutionX;
            _pixelPerfectCamera.refResolutionY = referenceResolutionY;
            _pixelPerfectCamera.upscaleRT = upscaleRenderTexture;
            _pixelPerfectCamera.pixelSnapping = pixelSnapping;
            _pixelPerfectCamera.cropFrameX = cropFrameX;
            _pixelPerfectCamera.cropFrameY = cropFrameY;
            _pixelPerfectCamera.stretchFill = stretchFill;

            // Ensure camera is set to orthographic
            _camera.orthographic = true;

            Debug.Log($"[PixelPerfectSetup] Retro Mode: {referenceResolutionX}x{referenceResolutionY} @ {pixelsPerUnit} PPU");
        }

        /// <summary>
        /// Updates resolution settings at runtime if needed.
        /// </summary>
        public void UpdateResolution(int width, int height)
        {
            referenceResolutionX = width;
            referenceResolutionY = height;

            if (_pixelPerfectCamera != null)
            {
                _pixelPerfectCamera.refResolutionX = width;
                _pixelPerfectCamera.refResolutionY = height;
            }
        }

        /// <summary>
        /// Gets the current pixel ratio for UI scaling.
        /// </summary>
        public int GetPixelRatio()
        {
            if (_pixelPerfectCamera != null)
            {
                return _pixelPerfectCamera.pixelRatio;
            }
            return 1;
        }
    }
}
