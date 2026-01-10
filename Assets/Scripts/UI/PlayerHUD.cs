using UnityEngine;
using HauntedCastle.Player;
using HauntedCastle.Services;
using HauntedCastle.Data;

namespace HauntedCastle.UI
{
    /// <summary>
    /// Simple HUD for displaying player status (energy, lives).
    /// Uses OnGUI for development - will be replaced with proper UI later.
    /// </summary>
    public class PlayerHUD : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool showHUD = true;
        [SerializeField] private float hudWidth = 200f;
        [SerializeField] private float hudHeight = 80f;
        [SerializeField] private float margin = 10f;

        [Header("Colors")]
        [SerializeField] private Color energyBarColor = new Color(0.2f, 0.8f, 0.3f);
        [SerializeField] private Color energyBarBackgroundColor = new Color(0.2f, 0.2f, 0.2f);
        [SerializeField] private Color livesColor = Color.red;

        [Header("Floor Indicator")]
        [SerializeField] private bool showFloorIndicator = true;
        [SerializeField] private float floorIndicatorWidth = 250f;
        [SerializeField] private float floorIndicatorHeight = 60f;

        private PlayerHealth _playerHealth;
        private string _currentFloorName = "";
        private string _currentRoomName = "";
        private int _currentFloorNumber = 1;
        private float _floorIndicatorAlpha = 1f;
        private float _floorIndicatorFadeTimer = 0f;
        private const float FLOOR_INDICATOR_FADE_DELAY = 3f;
        private const float FLOOR_INDICATOR_FADE_DURATION = 1f;
        private GUIStyle _boxStyle;
        private GUIStyle _labelStyle;
        private Texture2D _whiteTexture;

        private void Start()
        {
            // Find player health
            FindPlayerHealth();

            // Create white texture for drawing bars
            _whiteTexture = new Texture2D(1, 1);
            _whiteTexture.SetPixel(0, 0, Color.white);
            _whiteTexture.Apply();

            // Subscribe to room change events
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomChanged;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomChanged;
            }
        }

        private void OnRoomChanged(RoomData newRoom)
        {
            if (newRoom != null)
            {
                _currentRoomName = newRoom.displayName;
                _currentFloorNumber = newRoom.floorNumber;
                _currentFloorName = RoomDatabase.GetFloorName(newRoom.floorNumber);

                // Reset fade timer to show indicator
                _floorIndicatorFadeTimer = 0f;
                _floorIndicatorAlpha = 1f;
            }
        }

        private void FindPlayerHealth()
        {
            if (PlayerSpawner.Instance != null)
            {
                _playerHealth = PlayerSpawner.Instance.PlayerHealth;
            }

            if (_playerHealth == null)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    _playerHealth = player.GetComponent<PlayerHealth>();
                }
            }
        }

        private void Update()
        {
            // Try to find player if not found
            if (_playerHealth == null)
            {
                FindPlayerHealth();
            }

            // Update floor indicator fade
            if (_floorIndicatorAlpha > 0f)
            {
                _floorIndicatorFadeTimer += Time.deltaTime;
                if (_floorIndicatorFadeTimer > FLOOR_INDICATOR_FADE_DELAY)
                {
                    float fadeProgress = (_floorIndicatorFadeTimer - FLOOR_INDICATOR_FADE_DELAY) / FLOOR_INDICATOR_FADE_DURATION;
                    _floorIndicatorAlpha = Mathf.Clamp01(1f - fadeProgress);
                }
            }

            // Try to subscribe to RoomManager if not yet
            if (RoomManager.Instance != null && string.IsNullOrEmpty(_currentRoomName))
            {
                RoomManager.Instance.OnRoomLoadCompleted -= OnRoomChanged;
                RoomManager.Instance.OnRoomLoadCompleted += OnRoomChanged;

                // Get current room info if available
                if (RoomManager.Instance.CurrentRoomData != null)
                {
                    OnRoomChanged(RoomManager.Instance.CurrentRoomData);
                }
            }
        }

        private void OnGUI()
        {
            if (!showHUD) return;

            // Initialize styles if needed
            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle(GUI.skin.box);
                _boxStyle.normal.background = _whiteTexture;
            }

            if (_labelStyle == null)
            {
                _labelStyle = new GUIStyle(GUI.skin.label);
                _labelStyle.fontSize = 14;
                _labelStyle.fontStyle = FontStyle.Bold;
                _labelStyle.normal.textColor = Color.white;
            }

            // Draw HUD background
            Rect hudRect = new Rect(margin, margin, hudWidth, hudHeight);

            // Semi-transparent background
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(hudRect, _whiteTexture);
            GUI.color = Color.white;

            // Draw content
            GUILayout.BeginArea(new Rect(hudRect.x + 10, hudRect.y + 5, hudRect.width - 20, hudRect.height - 10));

            if (_playerHealth != null)
            {
                // Energy label
                GUILayout.Label($"Energy: {Mathf.CeilToInt(_playerHealth.CurrentEnergy)}/{Mathf.CeilToInt(_playerHealth.MaxEnergy)}", _labelStyle);

                // Energy bar
                Rect energyBarRect = GUILayoutUtility.GetRect(hudWidth - 20, 20);
                DrawProgressBar(energyBarRect, _playerHealth.EnergyPercent, energyBarColor, energyBarBackgroundColor);

                GUILayout.Space(5);

                // Lives
                GUILayout.BeginHorizontal();
                GUILayout.Label("Lives: ", _labelStyle);

                // Draw life icons
                for (int i = 0; i < _playerHealth.CurrentLives; i++)
                {
                    GUI.color = livesColor;
                    GUILayout.Label("♥", _labelStyle, GUILayout.Width(20));
                }
                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Player not found", _labelStyle);
            }

            GUILayout.EndArea();

            // Draw character info in corner
            DrawCharacterInfo();

            // Draw floor indicator
            if (showFloorIndicator)
            {
                DrawFloorIndicator();
            }
        }

        private void DrawProgressBar(Rect rect, float percent, Color fillColor, Color backgroundColor)
        {
            // Background
            GUI.color = backgroundColor;
            GUI.DrawTexture(rect, _whiteTexture);

            // Fill
            Rect fillRect = new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(percent), rect.height);
            GUI.color = fillColor;
            GUI.DrawTexture(fillRect, _whiteTexture);

            // Border
            GUI.color = Color.white;
            GUI.Box(rect, GUIContent.none);
        }

        private void DrawCharacterInfo()
        {
            if (_playerHealth == null) return;

            var playerSetup = _playerHealth.GetComponent<PlayerSetup>();
            if (playerSetup == null || playerSetup.CurrentCharacter == null) return;

            var character = playerSetup.CurrentCharacter;

            // Draw in top right
            float infoWidth = 150f;
            float infoHeight = 50f;
            Rect infoRect = new Rect(Screen.width - infoWidth - margin, margin, infoWidth, infoHeight);

            // Background
            GUI.color = new Color(0, 0, 0, 0.7f);
            GUI.DrawTexture(infoRect, _whiteTexture);
            GUI.color = Color.white;

            GUILayout.BeginArea(new Rect(infoRect.x + 5, infoRect.y + 5, infoRect.width - 10, infoRect.height - 10));

            // Character name with colored text
            GUI.color = character.characterColor;
            GUILayout.Label(character.characterName, _labelStyle);
            GUI.color = Color.white;

            // Attack type
            GUILayout.Label($"Attack: {character.attackType}", _labelStyle);

            GUILayout.EndArea();
        }

        /// <summary>
        /// Toggles HUD visibility.
        /// </summary>
        public void ToggleHUD()
        {
            showHUD = !showHUD;
        }

        private void DrawFloorIndicator()
        {
            if (string.IsNullOrEmpty(_currentRoomName)) return;
            if (_floorIndicatorAlpha <= 0.01f) return;

            // Position at bottom center of screen
            Rect indicatorRect = new Rect(
                (Screen.width - floorIndicatorWidth) / 2f,
                Screen.height - floorIndicatorHeight - margin,
                floorIndicatorWidth,
                floorIndicatorHeight
            );

            // Get floor-themed color
            Color floorColor = GetFloorColor(_currentFloorNumber);

            // Background with fade
            GUI.color = new Color(0, 0, 0, 0.8f * _floorIndicatorAlpha);
            GUI.DrawTexture(indicatorRect, _whiteTexture);

            // Border accent
            Rect borderRect = new Rect(indicatorRect.x, indicatorRect.y, indicatorRect.width, 3);
            GUI.color = new Color(floorColor.r, floorColor.g, floorColor.b, _floorIndicatorAlpha);
            GUI.DrawTexture(borderRect, _whiteTexture);

            GUI.color = Color.white;

            // Create styles
            GUIStyle floorStyle = new GUIStyle(_labelStyle);
            floorStyle.fontSize = 12;
            floorStyle.alignment = TextAnchor.MiddleCenter;
            floorStyle.normal.textColor = new Color(floorColor.r, floorColor.g, floorColor.b, _floorIndicatorAlpha);

            GUIStyle roomStyle = new GUIStyle(_labelStyle);
            roomStyle.fontSize = 16;
            roomStyle.alignment = TextAnchor.MiddleCenter;
            roomStyle.fontStyle = FontStyle.Bold;
            roomStyle.normal.textColor = new Color(1f, 1f, 1f, _floorIndicatorAlpha);

            GUILayout.BeginArea(new Rect(indicatorRect.x + 5, indicatorRect.y + 5, indicatorRect.width - 10, indicatorRect.height - 10));

            // Floor name
            GUILayout.Label(_currentFloorName, floorStyle);

            // Room name
            GUILayout.Label(_currentRoomName, roomStyle);

            GUILayout.EndArea();
        }

        private Color GetFloorColor(int floor)
        {
            return floor switch
            {
                0 => new Color(0.6f, 0.4f, 0.7f),    // Basement - purple
                1 => new Color(0.9f, 0.7f, 0.3f),    // Castle - gold
                2 => new Color(0.4f, 0.7f, 0.9f),    // Tower - sky blue
                _ => Color.white
            };
        }

        /// <summary>
        /// Forces the floor indicator to show (useful when manually checking room).
        /// </summary>
        public void ShowFloorIndicator()
        {
            _floorIndicatorAlpha = 1f;
            _floorIndicatorFadeTimer = 0f;
        }
    }
}
