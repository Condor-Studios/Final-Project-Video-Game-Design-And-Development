using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Visceral_WeaponBase : Visceral_Script
{
    public float Damage,KnockBack;
    [SerializeField] protected List<HitBox> _WeaponColliders = new List<HitBox>();

    public Action StartAttack;
    public Action EndAttack;

    protected virtual void EnableWeaponCollision() { }

    protected virtual void DisableWeaponCollision() { }

    public virtual void Attacking() { }
    public virtual void StopAttacking() { }

    public virtual void AddWeaponCollider(HitBox HTBox) { if (!_WeaponColliders.Contains(HTBox))
                                                               _WeaponColliders.Add(HTBox); }

    public virtual void NotifyHit(Collider other) { }

}
