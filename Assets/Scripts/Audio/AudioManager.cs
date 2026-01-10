using System.Collections.Generic;
using UnityEngine;

namespace HauntedCastle.Audio
{
    /// <summary>
    /// Central audio manager for sound effects and music.
    /// Provides pooled sound playback and music management.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private int sfxPoolSize = 8;

        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 0.8f;

        [Header("Music Tracks")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameMusic;
        [SerializeField] private AudioClip victoryMusic;
        [SerializeField] private AudioClip gameOverMusic;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private AudioClip doorOpenSound;
        [SerializeField] private AudioClip doorLockedSound;
        [SerializeField] private AudioClip keyUnlockSound;
        [SerializeField] private AudioClip playerHurtSound;
        [SerializeField] private AudioClip playerDeathSound;
        [SerializeField] private AudioClip enemyHitSound;
        [SerializeField] private AudioClip enemyDeathSound;
        [SerializeField] private AudioClip attackSound;
        [SerializeField] private AudioClip menuSelectSound;
        [SerializeField] private AudioClip menuConfirmSound;
        [SerializeField] private AudioClip keyPieceSound;
        [SerializeField] private AudioClip greatKeySound;
        [SerializeField] private AudioClip secretPassageSound;
        [SerializeField] private AudioClip stairsSound;

        // SFX pool
        private List<AudioSource> _sfxPool;
        private int _currentPoolIndex = 0;

        // Properties
        public float MasterVolume
        {
            get => masterVolume;
            set
            {
                masterVolume = Mathf.Clamp01(value);
                UpdateVolumes();
            }
        }

        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = Mathf.Clamp01(value);
                UpdateVolumes();
            }
        }

        public float SFXVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = Mathf.Clamp01(value);
                UpdateVolumes();
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
            InitializeSFXPool();
        }

        private void InitializeAudioSources()
        {
            // Create music source if not assigned
            if (musicSource == null)
            {
                var musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            // Create main SFX source if not assigned
            if (sfxSource == null)
            {
                var sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            UpdateVolumes();
        }

        private void InitializeSFXPool()
        {
            _sfxPool = new List<AudioSource>();

            for (int i = 0; i < sfxPoolSize; i++)
            {
                var poolObj = new GameObject($"SFXPool_{i}");
                poolObj.transform.SetParent(transform);
                var source = poolObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                _sfxPool.Add(source);
            }
        }

        private void UpdateVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = masterVolume * musicVolume;
            }

            if (sfxSource != null)
            {
                sfxSource.volume = masterVolume * sfxVolume;
            }

            // Guard against null pool (called before InitializeSFXPool)
            if (_sfxPool != null)
            {
                foreach (var source in _sfxPool)
                {
                    if (source != null)
                    {
                        source.volume = masterVolume * sfxVolume;
                    }
                }
            }
        }

        #region Music Methods

        public void PlayMusic(MusicTrack track)
        {
            AudioClip clip = track switch
            {
                MusicTrack.Menu => menuMusic,
                MusicTrack.Game => gameMusic,
                MusicTrack.Victory => victoryMusic,
                MusicTrack.GameOver => gameOverMusic,
                _ => null
            };

            if (clip != null && musicSource != null)
            {
                if (musicSource.clip == clip && musicSource.isPlaying) return;

                musicSource.clip = clip;
                musicSource.Play();
                Debug.Log($"[AudioManager] Playing music: {track}");
            }
        }

        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        public void PauseMusic()
        {
            if (musicSource != null)
            {
                musicSource.Pause();
            }
        }

        public void ResumeMusic()
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
        }

        public void SetMusicPitch(float pitch)
        {
            if (musicSource != null)
            {
                musicSource.pitch = pitch;
            }
        }

        #endregion

        #region SFX Methods

        // Sound system enabled
        private bool _soundDisabled = false;

        public void PlaySFX(SoundEffect effect)
        {
            if (_soundDisabled)
            {
                return;
            }

            AudioClip clip = GetSFXClip(effect);
            if (clip != null)
            {
                PlaySFXClip(clip);
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (_soundDisabled) return;

            if (clip != null)
            {
                PlaySFXClip(clip);
            }
        }

        private AudioClip GetSFXClip(SoundEffect effect)
        {
            // Try assigned clip first, fallback to procedural generation
            AudioClip clip = effect switch
            {
                SoundEffect.Pickup => pickupSound,
                SoundEffect.DoorOpen => doorOpenSound,
                SoundEffect.DoorLocked => doorLockedSound,
                SoundEffect.KeyUnlock => keyUnlockSound,
                SoundEffect.PlayerHurt => playerHurtSound,
                SoundEffect.PlayerDeath => playerDeathSound,
                SoundEffect.EnemyHit => enemyHitSound,
                SoundEffect.EnemyDeath => enemyDeathSound,
                SoundEffect.Attack => attackSound,
                SoundEffect.MenuSelect => menuSelectSound,
                SoundEffect.MenuConfirm => menuConfirmSound,
                SoundEffect.KeyPiece => keyPieceSound,
                SoundEffect.GreatKey => greatKeySound,
                SoundEffect.SecretPassage => secretPassageSound,
                SoundEffect.Stairs => stairsSound,
                _ => null
            };

            // Use procedural fallback if no clip assigned
            if (clip == null)
            {
                clip = effect switch
                {
                    SoundEffect.Pickup => ProceduralSoundGenerator.GetPickupSound(),
                    SoundEffect.DoorOpen => ProceduralSoundGenerator.GetDoorOpenSound(),
                    SoundEffect.DoorLocked => ProceduralSoundGenerator.GetDoorLockedSound(),
                    SoundEffect.KeyUnlock => ProceduralSoundGenerator.GetKeyPickupSound(),
                    SoundEffect.PlayerHurt => ProceduralSoundGenerator.GetPlayerHurtSound(),
                    SoundEffect.PlayerDeath => ProceduralSoundGenerator.GetPlayerDeathSound(),
                    SoundEffect.EnemyHit => ProceduralSoundGenerator.GetEnemyHitSound(),
                    SoundEffect.EnemyDeath => ProceduralSoundGenerator.GetEnemyDeathSound(),
                    SoundEffect.Attack => ProceduralSoundGenerator.GetAttackSound(),
                    SoundEffect.MenuSelect => ProceduralSoundGenerator.GetMenuSelectSound(),
                    SoundEffect.MenuConfirm => ProceduralSoundGenerator.GetMenuConfirmSound(),
                    SoundEffect.KeyPiece => ProceduralSoundGenerator.GetKeyPickupSound(),
                    SoundEffect.GreatKey => ProceduralSoundGenerator.GetGreatKeySound(),
                    SoundEffect.SecretPassage => ProceduralSoundGenerator.GetSecretPassageSound(),
                    SoundEffect.Stairs => ProceduralSoundGenerator.GetStairsSound(),
                    _ => null
                };
            }

            return clip;
        }

        private void PlaySFXClip(AudioClip clip)
        {
            // Get next available source from pool
            var source = _sfxPool[_currentPoolIndex];
            _currentPoolIndex = (_currentPoolIndex + 1) % _sfxPool.Count;

            source.clip = clip;
            source.volume = masterVolume * sfxVolume;
            source.Play();
        }

        public void PlaySFXAtPosition(SoundEffect effect, Vector3 position)
        {
            AudioClip clip = GetSFXClip(effect);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, masterVolume * sfxVolume);
            }
        }

        #endregion

        #region Convenience Methods

        public void PlayPickupSound() => PlaySFX(SoundEffect.Pickup);
        public void PlayDoorOpenSound() => PlaySFX(SoundEffect.DoorOpen);
        public void PlayDoorLockedSound() => PlaySFX(SoundEffect.DoorLocked);
        public void PlayKeyUnlockSound() => PlaySFX(SoundEffect.KeyUnlock);
        public void PlayPlayerHurtSound() => PlaySFX(SoundEffect.PlayerHurt);
        public void PlayPlayerDeathSound() => PlaySFX(SoundEffect.PlayerDeath);
        public void PlayEnemyHitSound() => PlaySFX(SoundEffect.EnemyHit);
        public void PlayEnemyDeathSound() => PlaySFX(SoundEffect.EnemyDeath);
        public void PlayAttackSound() => PlaySFX(SoundEffect.Attack);
        public void PlayMenuSelectSound() => PlaySFX(SoundEffect.MenuSelect);
        public void PlayMenuConfirmSound() => PlaySFX(SoundEffect.MenuConfirm);
        public void PlayKeyPieceSound() => PlaySFX(SoundEffect.KeyPiece);
        public void PlayGreatKeySound() => PlaySFX(SoundEffect.GreatKey);
        public void PlaySecretPassageSound() => PlaySFX(SoundEffect.SecretPassage);
        public void PlayStairsSound() => PlaySFX(SoundEffect.Stairs);

        #endregion

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }

    public enum MusicTrack
    {
        None,
        Menu,
        Game,
        Victory,
        GameOver
    }

    public enum SoundEffect
    {
        None,
        Pickup,
        DoorOpen,
        DoorLocked,
        KeyUnlock,
        PlayerHurt,
        PlayerDeath,
        EnemyHit,
        EnemyDeath,
        Attack,
        MenuSelect,
        MenuConfirm,
        KeyPiece,
        GreatKey,
        SecretPassage,
        Stairs
    }
}
