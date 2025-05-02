using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwordTest : Visceral_WeaponBase
{
    public override void Attacking()
    {
        EnableWeaponCollision();
        StartAttack?.Invoke();
    }

    public override void StopAttacking()
    {
        DisableWeaponCollision();
        EndAttack?.Invoke();
    }

    protected override void DisableWeaponCollision()
    {

        foreach (var Item in _WeaponColliders)
        {
            Item.DeactivateCollider();
        }
    }

    protected override void EnableWeaponCollision()
    {
        foreach (var Item in _WeaponColliders)
        {
            Item.activateCollider();
        }

    }

    public override void NotifyHit(Collider other)
    {
        var HPComp = other.GetComponent<Health_Component>();
        DamageScore damageScore = new DamageScore()
        {
            DamageAmount = Damage,
            Attacker = transform.root.gameObject,
            ElementalDamage = ElementType.Physical,
        };
        Vector3 Dir = other.transform.position - this.transform.position;
        Dir = Dir.normalized;

        HPComp.TakeDamageWithKnockback(Dir, KnockBack, damageScore);
    }

}
