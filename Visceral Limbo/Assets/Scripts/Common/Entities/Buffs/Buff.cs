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
            if (isDelta)
            {
                if (isDebuff)
                {
                    entity.TakeDamage(Mathf.Abs(healthModifier));
                    Debug.Log($"[Buff] Applied TICKING DEBUFF '{buffName}' to '{entity.gameObject.name}', dealing {Mathf.Abs(healthModifier)} damage.");
                }
                else
                {
                    entity.Heal(healthModifier);
                    Debug.Log($"[Buff] Applied TICKING BUFF '{buffName}' to '{entity.gameObject.name}', healing {healthModifier} health.");
                }
            }
            else
            {
                // For non-delta buffs, only apply effects immediately, expected once.
                if (isDebuff)
                {
                    entity.TakeDamage(Mathf.Abs(healthModifier));
                    Debug.Log($"[Buff] Applied INSTANT DEBUFF '{buffName}' to '{entity.gameObject.name}', dealing {Mathf.Abs(healthModifier)} damage.");
                }
                else
                {
                    entity.Heal(healthModifier);
                    Debug.Log($"[Buff] Applied INSTANT BUFF '{buffName}' to '{entity.gameObject.name}', healing {healthModifier} health.");
                }
            }
        }

        public void Remove(Entity entity)
        {
            Debug.Log($"[Buff] Buff '{buffName}' expired and was removed from '{entity.gameObject.name}'.");
        }
    }
}