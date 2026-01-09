using UnityEngine;
using System.Collections.Generic;

namespace HauntedCastle.Audio
{
    /// <summary>
    /// Generates procedural sound effects at runtime for retro-style game audio.
    /// Creates classic 8-bit style sounds using waveform synthesis.
    /// </summary>
    public static class ProceduralSoundGenerator
    {
        private static Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();
        private const int SampleRate = 44100;

        #region Game Sound Effects

        /// <summary>
        /// Gets or generates a pickup sound (ascending tone).
        /// </summary>
        public static AudioClip GetPickupSound()
        {
            return GetOrCreateClip("pickup", () => CreateToneSequence(
                new[] { 440f, 554f, 659f, 880f },
                new[] { 0.05f, 0.05f, 0.05f, 0.1f },
                WaveType.Square
            ));
        }

        /// <summary>
        /// Gets or generates a key pickup sound (special arpeggio).
        /// </summary>
        public static AudioClip GetKeyPickupSound()
        {
            return GetOrCreateClip("key_pickup", () => CreateToneSequence(
                new[] { 523f, 659f, 784f, 1047f, 784f, 659f },
                new[] { 0.08f, 0.08f, 0.08f, 0.15f, 0.08f, 0.08f },
                WaveType.Square
            ));
        }

        /// <summary>
        /// Gets or generates a door open sound.
        /// </summary>
        public static AudioClip GetDoorOpenSound()
        {
            return GetOrCreateClip("door_open", () => CreateSweep(200f, 80f, 0.3f, WaveType.Noise));
        }

        /// <summary>
        /// Gets or generates a door locked sound (buzzer).
        /// </summary>
        public static AudioClip GetDoorLockedSound()
        {
            return GetOrCreateClip("door_locked", () => CreateToneSequence(
                new[] { 150f, 100f },
                new[] { 0.15f, 0.2f },
                WaveType.Square
            ));
        }

        /// <summary>
        /// Gets or generates a player hurt sound.
        /// </summary>
        public static AudioClip GetPlayerHurtSound()
        {
            return GetOrCreateClip("player_hurt", () => CreateSweep(400f, 100f, 0.2f, WaveType.Square));
        }

        /// <summary>
        /// Gets or generates a player death sound.
        /// </summary>
        public static AudioClip GetPlayerDeathSound()
        {
            return GetOrCreateClip("player_death", () => CreateToneSequence(
                new[] { 440f, 349f, 293f, 220f, 146f },
                new[] { 0.15f, 0.15f, 0.2f, 0.25f, 0.4f },
                WaveType.Square
            ));
        }

        /// <summary>
        /// Gets or generates an enemy hit sound.
        /// </summary>
        public static AudioClip GetEnemyHitSound()
        {
            return GetOrCreateClip("enemy_hit", () => CreateNoise(0.1f));
        }

        /// <summary>
        /// Gets or generates an enemy death sound.
        /// </summary>
        public static AudioClip GetEnemyDeathSound()
        {
            return GetOrCreateClip("enemy_death", () => CreateSweep(300f, 50f, 0.25f, WaveType.Square));
        }

        /// <summary>
        /// Gets or generates an attack sound.
        /// </summary>
        public static AudioClip GetAttackSound()
        {
            return GetOrCreateClip("attack", () => CreateSweep(600f, 200f, 0.08f, WaveType.Square));
        }

        /// <summary>
        /// Gets or generates a magic attack sound.
        /// </summary>
        public static AudioClip GetMagicSound()
        {
            return GetOrCreateClip("magic", () => CreateToneSequence(
                new[] { 800f, 1000f, 1200f, 800f },
                new[] { 0.05f, 0.05f, 0.05f, 0.1f },
                WaveType.Sine
            ));
        }

        /// <summary>
        /// Gets or generates a menu select sound.
        /// </summary>
        public static AudioClip GetMenuSelectSound()
        {
            return GetOrCreateClip("menu_select", () => CreateTone(440f, 0.08f, WaveType.Square));
        }

        /// <summary>
        /// Gets or generates a menu confirm sound.
        /// </summary>
        public static AudioClip GetMenuConfirmSound()
        {
            return GetOrCreateClip("menu_confirm", () => CreateToneSequence(
                new[] { 523f, 659f },
                new[] { 0.08f, 0.12f },
                WaveType.Square
            ));
        }

        /// <summary>
        /// Gets or generates a Great Key sound (victory fanfare).
        /// </summary>
        public static AudioClip GetGreatKeySound()
        {
            return GetOrCreateClip("great_key", () => CreateToneSequence(
                new[] { 523f, 659f, 784f, 1047f, 1319f, 1568f },
                new[] { 0.1f, 0.1f, 0.1f, 0.15f, 0.15f, 0.3f },
                WaveType.Square
            ));
        }

        /// <summary>
        /// Gets or generates a stairs sound.
        /// </summary>
        public static AudioClip GetStairsSound()
        {
            return GetOrCreateClip("stairs", () => CreateToneSequence(
                new[] { 200f, 250f, 300f, 350f },
                new[] { 0.08f, 0.08f, 0.08f, 0.08f },
                WaveType.Square
            ));
        }

        /// <summary>
        /// Gets or generates a secret passage sound.
        /// </summary>
        public static AudioClip GetSecretPassageSound()
        {
            return GetOrCreateClip("secret", () => CreateSweep(100f, 400f, 0.4f, WaveType.Square));
        }

        #endregion

        #region Sound Generation

        private static AudioClip GetOrCreateClip(string name, System.Func<AudioClip> creator)
        {
            if (!_clipCache.TryGetValue(name, out var clip) || clip == null)
            {
                clip = creator();
                _clipCache[name] = clip;
            }
            return clip;
        }

        /// <summary>
        /// Creates a simple tone at the specified frequency.
        /// </summary>
        private static AudioClip CreateTone(float frequency, float duration, WaveType waveType)
        {
            int samples = (int)(SampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = 1f - (t / duration); // Simple decay
                data[i] = GetWaveValue(t, frequency, waveType) * envelope * 0.5f;
            }

            var clip = AudioClip.Create($"Tone_{frequency}", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Creates a sequence of tones.
        /// </summary>
        private static AudioClip CreateToneSequence(float[] frequencies, float[] durations, WaveType waveType)
        {
            int totalSamples = 0;
            foreach (float d in durations)
                totalSamples += (int)(SampleRate * d);

            float[] data = new float[totalSamples];
            int currentSample = 0;

            for (int i = 0; i < frequencies.Length; i++)
            {
                int samples = (int)(SampleRate * durations[i]);
                float freq = frequencies[i];

                for (int j = 0; j < samples && currentSample < totalSamples; j++)
                {
                    float t = j / (float)SampleRate;
                    float envelope = 1f - (t / durations[i]) * 0.5f; // Slight decay
                    data[currentSample++] = GetWaveValue(t, freq, waveType) * envelope * 0.5f;
                }
            }

            var clip = AudioClip.Create("ToneSequence", totalSamples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Creates a frequency sweep (ascending or descending).
        /// </summary>
        private static AudioClip CreateSweep(float startFreq, float endFreq, float duration, WaveType waveType)
        {
            int samples = (int)(SampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float progress = t / duration;
                float freq = Mathf.Lerp(startFreq, endFreq, progress);
                float envelope = 1f - progress; // Decay
                data[i] = GetWaveValue(t, freq, waveType) * envelope * 0.5f;
            }

            var clip = AudioClip.Create("Sweep", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Creates white noise.
        /// </summary>
        private static AudioClip CreateNoise(float duration)
        {
            int samples = (int)(SampleRate * duration);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)samples;
                float envelope = 1f - t; // Decay
                data[i] = (Random.value * 2f - 1f) * envelope * 0.3f;
            }

            var clip = AudioClip.Create("Noise", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Gets the wave value at time t for the specified frequency and type.
        /// </summary>
        private static float GetWaveValue(float t, float frequency, WaveType type)
        {
            float phase = t * frequency * 2f * Mathf.PI;

            return type switch
            {
                WaveType.Sine => Mathf.Sin(phase),
                WaveType.Square => Mathf.Sin(phase) > 0 ? 1f : -1f,
                WaveType.Triangle => Mathf.PingPong(t * frequency * 4f, 2f) - 1f,
                WaveType.Sawtooth => (t * frequency % 1f) * 2f - 1f,
                WaveType.Noise => Random.value * 2f - 1f,
                _ => Mathf.Sin(phase)
            };
        }

        private enum WaveType
        {
            Sine,
            Square,
            Triangle,
            Sawtooth,
            Noise
        }

        #endregion

        #region Music Generation

        /// <summary>
        /// Creates a simple atmospheric loop for menu/game.
        /// </summary>
        public static AudioClip CreateAtmosphericLoop(float duration = 8f)
        {
            int samples = (int)(SampleRate * duration);
            float[] data = new float[samples];

            // Simple bass drone with occasional notes
            float[] baseNotes = { 110f, 82.4f, 98f, 73.4f }; // Low notes
            float noteLength = duration / baseNotes.Length;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                int noteIndex = (int)(t / noteLength) % baseNotes.Length;
                float freq = baseNotes[noteIndex];

                // Bass drone
                float bass = GetWaveValue(t, freq, WaveType.Sine) * 0.15f;

                // Add some atmosphere with filtered noise
                float atmosphere = (Random.value * 2f - 1f) * 0.02f;

                // Occasional high notes
                float high = 0f;
                if (Random.value < 0.001f)
                {
                    high = GetWaveValue(t, freq * 4f, WaveType.Sine) * 0.1f;
                }

                data[i] = bass + atmosphere + high;
            }

            var clip = AudioClip.Create("AtmosphericLoop", samples, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        #endregion

        /// <summary>
        /// Clears the audio clip cache.
        /// </summary>
        public static void ClearCache()
        {
            _clipCache.Clear();
        }
    }
}
