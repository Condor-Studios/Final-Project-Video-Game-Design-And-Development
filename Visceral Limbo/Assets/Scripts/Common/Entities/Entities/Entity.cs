using System;
using Common.Entities.Buffs;
using Interfaces;
using UnityEngine;

namespace Common.Entities.Entities
{
    public class Entity : MonoBehaviour, IDamageable, IHealed
    {
        [SerializeField]
        protected int health;
        [SerializeField]
        protected int maxHealth;
        protected bool isAlive = true;
        private PriorityQueue _activeBuffs;

        protected virtual void Awake()
        {
            health = maxHealth;

            if (_activeBuffs == null)
            {
                _activeBuffs = new PriorityQueue();
                Debug.Log("[Entity] PriorityQueue initialized successfully.");
                _activeBuffs.Awake();
            }
        }
        
        protected virtual void Start()
        {
            _activeBuffs.Start();
        }

        protected virtual void Update()
        {
            _activeBuffs.Update(Time.deltaTime);
        }

        public bool CheckIfBuffActive(BuffInstance buff)
        {
           return _activeBuffs.Contains(buff);
        }

        public int Health
        {
            get => health;
            set
            {
                health = value;
                if (health <= 0)
                {
                    Die();
                }
                
                if (health > maxHealth)
                {
                    health = maxHealth;
                }
            }
        }

        public int MaxHealth
        {
            get => maxHealth;
            set => maxHealth = value;
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
                Destroy(this.gameObject);
        }

        public void TakeDamage(int damage)
        {
            if (health > 0)
            {
                Health -= damage;
                Debug.Log($"{gameObject.name} has taken {damage} damage.");
            }
        }
        
        public void Heal(int amount)
        {
            if (health > 0 && amount > 0 && Health < MaxHealth)
            {
                Health += amount;
                Debug.Log($"{gameObject.name} has been healed by {amount} hp.");
                // Handle healing logic here (e.g., play animation, etc.)
            }
        }
        
        public void AddBuff(Buff buff)
        {
            BuffInstance newBuff = new BuffInstance(buff, this);
            _activeBuffs.Enqueue(newBuff);
            Debug.Log($"{gameObject.name} received buff: {buff.buffName}");
        }
        
        public void AddBuffInstance(BuffInstance buffInstance)
        {
            if (_activeBuffs == null)
            {
                Debug.LogError($"[Entity] ERROR: Tried to add BuffInstance '{buffInstance.Buff.buffName}' but _activeBuffs is NULL!");
                return;
            }

            _activeBuffs.Enqueue(buffInstance);
            Debug.Log($"[Entity] {gameObject.name} received BuffInstance: {buffInstance.Buff.buffName}");
        }

        
    }
}
