using UnityEngine;
using HauntedCastle.Player;

namespace HauntedCastle.Enemies
{
    /// <summary>
    /// Simplified projectile behavior that avoids the freeze-causing code in EnemyProjectile.
    /// This component handles movement, collision, and auto-destruction.
    /// </summary>
    public class SimpleProjectileBehavior : MonoBehaviour
    {
        private Rigidbody2D _rb;
        private Vector2 _direction;
        private float _speed;
        private int _damage;
        private Color _color;
        private float _lifetime = 5f;
        private float _timer;
        private bool _destroyed;

        public void Initialize(Vector2 direction, float speed, int damage, Color color)
        {
            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _color = color;

            // Validate direction to prevent NaN/Infinity
            if (float.IsNaN(_direction.x) || float.IsNaN(_direction.y) ||
                float.IsInfinity(_direction.x) || float.IsInfinity(_direction.y) ||
                _direction.sqrMagnitude < 0.01f)
            {
                _direction = Vector2.right;
            }

            _rb = GetComponent<Rigidbody2D>();
            if (_rb != null)
            {
                Vector2 velocity = _direction * _speed;

                // Validate velocity
                if (float.IsNaN(velocity.x) || float.IsNaN(velocity.y))
                {
                    velocity = Vector2.right * _speed;
                }

                _rb.velocity = velocity;
            }

            // Rotate to face direction
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Set layer to avoid hitting enemies
            int projLayer = LayerMask.NameToLayer("EnemyProjectiles");
            if (projLayer < 0 || projLayer > 31)
            {
                projLayer = LayerMask.NameToLayer("Default");
                if (projLayer < 0) projLayer = 0;
            }
            gameObject.layer = projLayer;
        }

        private void Update()
        {
            // Use unscaledDeltaTime to prevent freeze when timeScale = 0
            _timer += Time.unscaledDeltaTime;
            if (_timer >= _lifetime)
            {
                DestroyProjectile();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_destroyed) return;

            // Hit player
            if (other.CompareTag("Player"))
            {
                var playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null && !playerHealth.IsInvulnerable)
                {
                    playerHealth.TakeDamage(_damage, _direction);
                }
                DestroyProjectile();
            }
            // Hit wall
            else if (other.CompareTag("Wall") || other.CompareTag("Ground") || other.CompareTag("Obstacle"))
            {
                DestroyProjectile();
            }
            else
            {
                // Check by layer name safely
                int wallLayer = LayerMask.NameToLayer("Walls");
                int groundLayer = LayerMask.NameToLayer("Ground");
                int otherLayer = other.gameObject.layer;

                if ((wallLayer >= 0 && otherLayer == wallLayer) ||
                    (groundLayer >= 0 && otherLayer == groundLayer))
                {
                    DestroyProjectile();
                }
            }
        }

        private void DestroyProjectile()
        {
            if (_destroyed) return;
            _destroyed = true;
            Destroy(gameObject);
        }
    }
}
