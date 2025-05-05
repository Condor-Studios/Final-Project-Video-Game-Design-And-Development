using UnityEngine;
using Visceral_Limbo.scripts.Components.General_Use.Entities;

namespace Visceral_Limbo.scripts.Components.General_Use.Triggers
{
    public class DamageCollisionTrigger : MonoBehaviour
    {
        [SerializeField]Collider _Collider;
        float Damage, KnockBack;
        bool CanDealDamage;

        private void Start()
        {
            _Collider= GetComponent<Collider>();
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("trigger!!");
            if (other.TryGetComponent(out Health_Component HPComp) && CanDealDamage)
            {
                var dir = (other.transform.position - this.transform.position).normalized;
                HPComp.TakeDamageWithKnockback(Damage, dir, KnockBack);
            }
        }

        public void UpdateValues(float NDamage,float NKnockBack)
        {
            Damage = NDamage;
            KnockBack = NKnockBack;
        }

        public void Activate(bool NewValue)
        {
            CanDealDamage = NewValue;
            _Collider.enabled = true;
        }
    }
}
