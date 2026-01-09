using UnityEngine;

namespace HauntedCastle.Audio
{
    /// <summary>
    /// Integrates procedural sound generation with the AudioManager.
    /// Automatically generates and assigns sound effects if not already present.
    /// </summary>
    public class AudioIntegration : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool generateMissingSounds = true;

        private void Start()
        {
            if (generateMissingSounds)
            {
                AssignProceduralSounds();
            }
        }

        /// <summary>
        /// Assigns procedurally generated sounds to the AudioManager.
        /// </summary>
        public void AssignProceduralSounds()
        {
            var audioManager = AudioManager.Instance;
            if (audioManager == null)
            {
                Debug.LogWarning("[AudioIntegration] AudioManager not found");
                return;
            }

            // The AudioManager uses serialized clips, but we can play procedural sounds directly
            // when the serialized clips are null
            Debug.Log("[AudioIntegration] Procedural sound generation available");
        }

        /// <summary>
        /// Plays a procedural pickup sound.
        /// </summary>
        public static void PlayPickup()
        {
            PlayOneShot(ProceduralSoundGenerator.GetPickupSound());
        }

        /// <summary>
        /// Plays a procedural key pickup sound.
        /// </summary>
        public static void PlayKeyPickup()
        {
            PlayOneShot(ProceduralSoundGenerator.GetKeyPickupSound());
        }

        /// <summary>
        /// Plays a procedural door open sound.
        /// </summary>
        public static void PlayDoorOpen()
        {
            PlayOneShot(ProceduralSoundGenerator.GetDoorOpenSound());
        }

        /// <summary>
        /// Plays a procedural door locked sound.
        /// </summary>
        public static void PlayDoorLocked()
        {
            PlayOneShot(ProceduralSoundGenerator.GetDoorLockedSound());
        }

        /// <summary>
        /// Plays a procedural player hurt sound.
        /// </summary>
        public static void PlayPlayerHurt()
        {
            PlayOneShot(ProceduralSoundGenerator.GetPlayerHurtSound());
        }

        /// <summary>
        /// Plays a procedural player death sound.
        /// </summary>
        public static void PlayPlayerDeath()
        {
            PlayOneShot(ProceduralSoundGenerator.GetPlayerDeathSound());
        }

        /// <summary>
        /// Plays a procedural enemy hit sound.
        /// </summary>
        public static void PlayEnemyHit()
        {
            PlayOneShot(ProceduralSoundGenerator.GetEnemyHitSound());
        }

        /// <summary>
        /// Plays a procedural enemy death sound.
        /// </summary>
        public static void PlayEnemyDeath()
        {
            PlayOneShot(ProceduralSoundGenerator.GetEnemyDeathSound());
        }

        /// <summary>
        /// Plays a procedural attack sound.
        /// </summary>
        public static void PlayAttack()
        {
            PlayOneShot(ProceduralSoundGenerator.GetAttackSound());
        }

        /// <summary>
        /// Plays a procedural magic sound.
        /// </summary>
        public static void PlayMagic()
        {
            PlayOneShot(ProceduralSoundGenerator.GetMagicSound());
        }

        /// <summary>
        /// Plays a procedural menu select sound.
        /// </summary>
        public static void PlayMenuSelect()
        {
            PlayOneShot(ProceduralSoundGenerator.GetMenuSelectSound());
        }

        /// <summary>
        /// Plays a procedural menu confirm sound.
        /// </summary>
        public static void PlayMenuConfirm()
        {
            PlayOneShot(ProceduralSoundGenerator.GetMenuConfirmSound());
        }

        /// <summary>
        /// Plays a procedural Great Key sound.
        /// </summary>
        public static void PlayGreatKey()
        {
            PlayOneShot(ProceduralSoundGenerator.GetGreatKeySound());
        }

        /// <summary>
        /// Plays a procedural stairs sound.
        /// </summary>
        public static void PlayStairs()
        {
            PlayOneShot(ProceduralSoundGenerator.GetStairsSound());
        }

        /// <summary>
        /// Plays a procedural secret passage sound.
        /// </summary>
        public static void PlaySecretPassage()
        {
            PlayOneShot(ProceduralSoundGenerator.GetSecretPassageSound());
        }

        private static void PlayOneShot(AudioClip clip)
        {
            if (clip == null) return;

            // Try to use AudioManager
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(clip);
            }
            else
            {
                // Fallback: play at listener position
                AudioSource.PlayClipAtPoint(clip, Camera.main?.transform.position ?? Vector3.zero);
            }
        }
    }
}
