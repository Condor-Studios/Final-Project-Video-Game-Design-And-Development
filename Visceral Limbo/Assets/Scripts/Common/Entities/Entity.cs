using Interfaces;
using UnityEngine;

namespace Common.Entities
{
    public class Entity : MonoBehaviour, IDamageable
    {
        protected int health;
        protected int maxHealth;
        protected bool isAlive;

        public int Health
        {
            get { return health; }
            set
            {
                health = value;
                if (health <= 0)
                {
                    Die();
                }
            }
        }

        public int MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }
        
        protected void Die()
        {
                health = 0;
                isAlive = false;
                // Handle death logic here (e.g., play animation, remove from game, etc.)
                Debug.Log($"{gameObject.name} has died.");
        }

        public void TakeDamage(int damage)
        {
            if (health > 0)
            {
                Health -= damage;
            }
        }
    }
}
