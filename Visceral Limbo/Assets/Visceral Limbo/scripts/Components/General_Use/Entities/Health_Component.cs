using UnityEngine;
using Visceral_Limbo.scripts.Base_Classes;

namespace Visceral_Limbo.scripts.Components.General_Use.Entities
{
    public class Health_Component : Visceral_Component
    {
        public float CurrentHealth, MaxHealth;
        public Rigidbody _RB;

        private void Start()
        {
            CurrentHealth = MaxHealth;
            _RB= GetComponent<Rigidbody>();
        }

        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
        }

        public void TakeDamageWithKnockback(float damage,Vector3 Direction,float knockback)
        {
            CurrentHealth -= damage;
            if(_RB != null)
            {
                Vector3 FinalForce = Direction * knockback;
                _RB.AddForce(FinalForce, ForceMode.Impulse);
            }
        }

    }
}
