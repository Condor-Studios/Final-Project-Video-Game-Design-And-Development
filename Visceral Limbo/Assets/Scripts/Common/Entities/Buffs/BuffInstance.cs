using Common.Entities.Entities;
using UnityEngine;

namespace Common.Entities.Buffs
{
    public class BuffInstance
    {
        public Buff Buff { get; private set; }
        public float timeRemaining;

        private Entity entity;
        private bool applied = false;
        private float tickTimer = 0f;
        private float tickInterval = 1f;

        public BuffInstance(Buff buff, Entity entity)
        {
            this.Buff = buff;
            this.entity = entity;
            this.timeRemaining = buff.duration;
        }

        public void Awake()
        {
            if (!Buff.isStackable && entity.CheckIfBuffActive(this))
            {
                Buff.Remove(entity);
                Debug.Log($"[BuffInstance] Buff '{Buff.buffName}' has already been applied on Entity and is not stackable. '{entity.gameObject.name}'. Removing buff.");
            }
        }

        public void Start()
        {
            //Here we check if the buff is already applied, if so, we don't apply it again.
            //Also, if the buff is delta, then the effects of the buff will be applied on each tick, else it will be applied once.
            if (applied) return;
            applied = true;
            if (Buff.isDelta) return;
            Buff.Apply(entity);
            tickInterval = Mathf.Infinity;
            Debug.Log($"[BuffInstance] Started Buff '{Buff.buffName}' on Entity '{entity.gameObject.name}'.");
        }

        public void Update(float deltaTime)
        {
            timeRemaining -= deltaTime;
            tickTimer += deltaTime;

            if (tickTimer >= tickInterval && !IsExpired())
            {
                tickTimer = 0f;
                Debug.Log($"[BuffInstance] Ticking Buff '{Buff.buffName}' on Entity '{entity.gameObject.name}'. Time remaining: {timeRemaining:F2}s");
                Buff.Apply(entity);
            }

            if (IsExpired())
            {
                Debug.Log($"[BuffInstance] Buff '{Buff.buffName}' expired on Entity '{entity.gameObject.name}'. Removing buff.");
                Buff.Remove(entity);
            }
        }

        public bool IsExpired()
        {
            return timeRemaining <= 0f;
        }
    }
}