using Common.Entities.Entities;
using UnityEngine;

namespace Common.Entities.Buffs.Buff_Data.Buffs.Effects
{
    [CreateAssetMenu(fileName = "NewHealEffect", menuName = "Visceral_Limbo/BuffEffects/HealEffect")]
    public sealed class HealEffect : BuffEffect
    {
        public override void Apply(Entity entity, bool isDelta, bool isDebuff)
        {
            if (isDebuff)
            {
                if (isDelta)
                {
                    entity.TakeDamage(Mathf.Abs(value));
                    Debug.Log($"[HealEffect] Ticking DEBUFF: Dealt {Mathf.Abs(value)} damage over time to {entity.gameObject.name}");
                }
                else
                {
                    entity.TakeDamage(Mathf.Abs(value));
                    Debug.Log($"[HealEffect] Instant DEBUFF: Dealt {Mathf.Abs(value)} damage to {entity.gameObject.name}");
                }
            }
            else
            {
                if (isDelta)
                {
                    entity.Heal(value);
                    Debug.Log($"[HealEffect] Ticking BUFF: Healed {value} health over time on {entity.gameObject.name}");
                }
                else
                {
                    entity.Heal(value);
                    Debug.Log($"[HealEffect] Instant BUFF: Instantly healed {value} health on {entity.gameObject.name}");
                }
            }
        }

        public override void Remove(Entity entity)
        {
            // Heal effects don't need to be reverted
        }
    }
}