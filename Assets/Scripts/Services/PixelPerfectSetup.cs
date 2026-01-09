using UnityEngine;
using UnityEngine.U2D;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Sets up pixel-perfect rendering for the game camera.
    /// Attach to the main camera in each scene.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class PixelPerfectSetup : MonoBehaviour
    {
        [Header("Resolution Settings")]
        [SerializeField] private int referenceResolutionX = 256;
        [SerializeField] private int referenceResolutionY = 192;
        [SerializeField] private int pixelsPerUnit = 16;

        [Header("Scaling Options")]
        [SerializeField] private bool upscaleRenderTexture = true;
        [SerializeField] private bool pixelSnapping = true;
        [SerializeField] private bool cropFrameX = false;
        [SerializeField] private bool cropFrameY = false;
        [SerializeField] private bool stretchFill = false;

        private Camera _camera;
        private PixelPerfectCamera _pixelPerfectCamera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            SetupPixelPerfectCamera();
        }

        private void SetupPixelPerfectCamera()
        {
            // Check if PixelPerfectCamera component exists
            _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

            if (_pixelPerfectCamera == null)
            {
                _pixelPerfectCamera = gameObject.AddComponent<PixelPerfectCamera>();
            }

            // Configure pixel perfect settings
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
