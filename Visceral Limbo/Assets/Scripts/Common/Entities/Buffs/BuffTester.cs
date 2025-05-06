using Common.Entities.Entities;
using UnityEngine;

namespace Common.Entities.Buffs
{
    public class BuffTester : MonoBehaviour
    {
        public Buff[] activeBuffs;
        public Entity _entity;
        

        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Space)) return;
            if (!_entity || activeBuffs == null) return;
            foreach (var buff in activeBuffs)
            {
                if (buff)
                {
                    var buffInstance = new BuffInstance(buff, _entity);
                    Debug.Log(buffInstance.Buff.buffName);
                    _entity.AddBuffInstance(buffInstance);
                }
            }
        }
    }
}
