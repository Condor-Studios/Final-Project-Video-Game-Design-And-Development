// --- Buff.cs ---

using Common.Entities.Entities;
using Interfaces;
using UnityEngine;

namespace Common.Entities.Buffs
{
    [CreateAssetMenu(fileName = "NewBuff", menuName = "Visceral_Limbo/Buffs/Buff", order = 1)]
    public sealed class Buff : ScriptableObject, IBuffEffect
    {
        public string buffName;
        public float duration;
        public bool isDebuff;
        public bool isStackable;
        public bool isDelta;

        public int healthModifier;
        public float speedModifier;
        public int damageModifier;

        public void Apply(Entity entity)
        {
            if (isDebuff)
            {
                entity.TakeDamage(Mathf.Abs(healthModifier));
                Debug.Log($"[Buff] Applied DEBUFF '{buffName}' to '{entity.gameObject.name}', dealing {Mathf.Abs(healthModifier)} damage.");
            }
            else
            {
                entity.Heal(healthModifier);
                Debug.Log($"[Buff] Applied BUFF '{buffName}' to '{entity.gameObject.name}', healing {healthModifier} health.");
            }
        }

        public void Remove(Entity entity)
        {
            Debug.Log($"[Buff] Buff '{buffName}' expired and was removed from '{entity.gameObject.name}'.");
        }
    }
}