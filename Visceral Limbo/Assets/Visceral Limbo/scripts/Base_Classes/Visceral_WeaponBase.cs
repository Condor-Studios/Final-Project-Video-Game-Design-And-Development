using System;
using UnityEngine;
using Visceral_Limbo.scripts.Components.General_Use.Triggers;

namespace Visceral_Limbo.scripts.Base_Classes
{
    public class Visceral_WeaponBase : Visceral_Script
    {
        public float Damage,KnockBack;
        [SerializeField] protected DamageCollisionTrigger[] _WeaponColliders;

        public Action StartAttack;
        public Action EndAttack;

        protected virtual void EnableWeaponCollision() { }

        protected virtual void DisableWeaponCollision() { }

        public virtual void Attacking() { }
        public virtual void StopAttacking() { }

    }
}
