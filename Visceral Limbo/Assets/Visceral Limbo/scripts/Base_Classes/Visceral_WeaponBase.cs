using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Visceral_WeaponBase : Visceral_Script
{
    public float Damage,KnockBack;
    [SerializeField] protected DamageCollisionTrigger[] _WeaponColliders;

    protected virtual void EnableWeaponCollision() { }

    protected virtual void DisableWeaponCollision() { }

    public virtual void Attacking() { }
    public virtual void StopAttacking() { }

}
