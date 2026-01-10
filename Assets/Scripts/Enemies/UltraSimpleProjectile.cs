using UnityEngine;
using HauntedCastle.Player;

namespace HauntedCastle.Enemies
{
    /// <summary>
    /// Simple projectile behavior - uses string comparison instead of CompareTag
    /// to avoid freeze when tags don't exist in project.
    /// </summary>
    public class UltraSimpleProjectile : MonoBehaviour
    {
        [HideInInspector] public int damage = 1;
        [HideInInspector] public Vector2 direction;

        private void OnTriggerEnter2D(Collider2D other)
        {
            // IMPORTANT: Use string == instead of CompareTag to avoid freeze
            // when tags don't exist in the project
            string tag = other.tag;

            if (tag == "Player")
            {
                var playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage, direction);
                }
                Destroy(gameObject);
            }
            else if (tag == "Wall" || tag == "Ground" || tag == "Obstacle")
            {
                Destroy(gameObject);
            }
        }
    }
}
