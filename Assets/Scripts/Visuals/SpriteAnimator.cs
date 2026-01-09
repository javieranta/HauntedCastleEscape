using UnityEngine;
using System.Collections.Generic;

namespace HauntedCastle.Visuals
{
    /// <summary>
    /// Generic sprite animation component for any game object.
    /// Supports multiple animation states with pixel-perfect rendering.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float frameRate = 0.15f;
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool loop = true;
        [SerializeField] private string defaultAnimation = "idle";

        private SpriteRenderer _spriteRenderer;
        private Dictionary<string, SpriteAnimation> _animations = new Dictionary<string, SpriteAnimation>();
        private SpriteAnimation _currentAnimation;
        private string _currentAnimationName;
        private int _currentFrame;
        private float _frameTimer;
        private bool _isPlaying;
        private bool _flipX;

        public bool IsPlaying => _isPlaying;
        public string CurrentAnimation => _currentAnimationName;
        public bool FlipX
        {
            get => _flipX;
            set
            {
                _flipX = value;
                if (_spriteRenderer != null)
                    _spriteRenderer.flipX = value;
            }
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (playOnAwake && _animations.ContainsKey(defaultAnimation))
            {
                Play(defaultAnimation);
            }
        }

        private void Update()
        {
            if (!_isPlaying || _currentAnimation == null || _currentAnimation.frames == null)
                return;

            _frameTimer += Time.deltaTime;

            float effectiveFrameRate = _currentAnimation.customFrameRate > 0
                ? _currentAnimation.customFrameRate
                : frameRate;

            if (_frameTimer >= effectiveFrameRate)
            {
                _frameTimer = 0f;
                _currentFrame++;

                if (_currentFrame >= _currentAnimation.frames.Length)
                {
                    if (loop || _currentAnimation.loop)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = _currentAnimation.frames.Length - 1;
                        _isPlaying = false;
                        _currentAnimation.onComplete?.Invoke();
                        return;
                    }
                }

                UpdateSprite();
            }
        }

        /// <summary>
        /// Adds an animation with the given name and frames.
        /// </summary>
        public void AddAnimation(string name, Sprite[] frames, bool looping = true, float customFrameRate = 0f)
        {
            _animations[name] = new SpriteAnimation
            {
                name = name,
                frames = frames,
                loop = looping,
                customFrameRate = customFrameRate
            };
        }

        /// <summary>
        /// Adds an animation with a completion callback.
        /// </summary>
        public void AddAnimation(string name, Sprite[] frames, System.Action onComplete)
        {
            _animations[name] = new SpriteAnimation
            {
                name = name,
                frames = frames,
                loop = false,
                onComplete = onComplete
            };
        }

        /// <summary>
        /// Plays the animation with the given name.
        /// </summary>
        public void Play(string animationName, bool restart = false)
        {
            if (!_animations.TryGetValue(animationName, out var animation))
            {
                Debug.LogWarning($"[SpriteAnimator] Animation '{animationName}' not found");
                return;
            }

            if (_currentAnimationName == animationName && !restart && _isPlaying)
                return;

            _currentAnimation = animation;
            _currentAnimationName = animationName;
            _currentFrame = 0;
            _frameTimer = 0f;
            _isPlaying = true;

            UpdateSprite();
        }

        /// <summary>
        /// Stops the current animation.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
        }

        /// <summary>
        /// Pauses the current animation.
        /// </summary>
        public void Pause()
        {
            _isPlaying = false;
        }

        /// <summary>
        /// Resumes the current animation.
        /// </summary>
        public void Resume()
        {
            if (_currentAnimation != null)
                _isPlaying = true;
        }

        /// <summary>
        /// Sets a single sprite (stops animation).
        /// </summary>
        public void SetSprite(Sprite sprite)
        {
            _isPlaying = false;
            _spriteRenderer.sprite = sprite;
        }

        private void UpdateSprite()
        {
            if (_currentAnimation?.frames != null && _currentFrame < _currentAnimation.frames.Length)
            {
                _spriteRenderer.sprite = _currentAnimation.frames[_currentFrame];
            }
        }

        /// <summary>
        /// Sets the frame rate for animations.
        /// </summary>
        public void SetFrameRate(float rate)
        {
            frameRate = rate;
        }

        /// <summary>
        /// Checks if an animation exists.
        /// </summary>
        public bool HasAnimation(string name)
        {
            return _animations.ContainsKey(name);
        }

        /// <summary>
        /// Gets the sprite renderer for external color/sorting changes.
        /// </summary>
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        private class SpriteAnimation
        {
            public string name;
            public Sprite[] frames;
            public bool loop = true;
            public float customFrameRate;
            public System.Action onComplete;
        }
    }

    /// <summary>
    /// Extension methods for easily creating animations from PixelArtGenerator.
    /// </summary>
    public static class SpriteAnimatorExtensions
    {
        /// <summary>
        /// Sets up character animations using pixel art sprites.
        /// </summary>
        public static void SetupCharacterAnimations(this SpriteAnimator animator, Services.CharacterType characterType)
        {
            Sprite baseSprite = characterType switch
            {
                Services.CharacterType.Knight => PixelArtGenerator.GetKnightSprite(),
                Services.CharacterType.Wizard => PixelArtGenerator.GetWizardSprite(),
                Services.CharacterType.Serf => PixelArtGenerator.GetSerfSprite(),
                _ => PixelArtGenerator.GetKnightSprite()
            };

            // Create simple 2-frame walk animation by using same sprite
            // In a full implementation, you'd generate variations
            Sprite[] walkFrames = new[] { baseSprite, CreateWalkFrame(baseSprite, 1), baseSprite, CreateWalkFrame(baseSprite, 2) };

            animator.AddAnimation("idle", new[] { baseSprite });
            animator.AddAnimation("walk_down", walkFrames, true, 0.1f);
            animator.AddAnimation("walk_up", walkFrames, true, 0.1f);
            animator.AddAnimation("walk_left", walkFrames, true, 0.1f);
            animator.AddAnimation("walk_right", walkFrames, true, 0.1f);
            animator.AddAnimation("attack", new[] { baseSprite, baseSprite }, false, 0.1f);
            animator.AddAnimation("hurt", new[] { baseSprite }, false, 0.2f);

            animator.Play("idle");
        }

        /// <summary>
        /// Sets up enemy animations using pixel art sprites.
        /// </summary>
        public static void SetupEnemyAnimations(this SpriteAnimator animator, Data.EnemyType enemyType)
        {
            Sprite baseSprite = enemyType switch
            {
                Data.EnemyType.Bat => PixelArtGenerator.GetBatSprite(),
                Data.EnemyType.Ghost => PixelArtGenerator.GetGhostSprite(),
                Data.EnemyType.Skeleton => PixelArtGenerator.GetSkeletonSprite(),
                Data.EnemyType.Spider => PixelArtGenerator.GetSpiderSprite(),
                Data.EnemyType.Mummy => PixelArtGenerator.GetMummySprite(),
                Data.EnemyType.Witch => PixelArtGenerator.GetWitchSprite(),
                Data.EnemyType.Demon => PixelArtGenerator.GetDemonSprite(),
                Data.EnemyType.Vampire => PixelArtGenerator.GetVampireSprite(),
                Data.EnemyType.Reaper => PixelArtGenerator.GetReaperSprite(),
                Data.EnemyType.Werewolf => PixelArtGenerator.GetWerewolfSprite(),
                _ => PixelArtGenerator.GetBatSprite()
            };

            Sprite[] idleFrames = new[] { baseSprite };
            Sprite[] moveFrames = new[] { baseSprite, CreateWalkFrame(baseSprite, 1) };

            animator.AddAnimation("idle", idleFrames);
            animator.AddAnimation("move", moveFrames, true, 0.15f);
            animator.AddAnimation("attack", idleFrames, false, 0.2f);
            animator.AddAnimation("hurt", idleFrames, false, 0.1f);
            animator.AddAnimation("death", idleFrames, false, 0.3f);

            animator.Play("idle");
        }

        /// <summary>
        /// Creates a slight variation of a sprite for walk animation.
        /// </summary>
        private static Sprite CreateWalkFrame(Sprite baseSprite, int frameIndex)
        {
            // For now, return the same sprite - in a more advanced implementation,
            // you could modify the texture slightly for animation
            return baseSprite;
        }
    }
}
