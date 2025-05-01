using System;
using Interfaces;
using UnityEngine;

namespace Common.Entities
{
    public class Entity : MonoBehaviour, IDamageable
    {
        [SerializeField]
        protected int health;
        [SerializeField]
        protected int maxHealth;
        protected bool isAlive = true;

        private void Awake()
        {
            health = maxHealth;
        }

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
                Debug.Log($"{gameObject.name} has taken {damage} damage.");
            }
        }
    }
}
