using UnityEngine;
using HauntedCastle.Player;
using HauntedCastle.Services;

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

        private PlayerHealth _playerHealth;
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
    }
}
