using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Visceral_WeaponBase : Visceral_Script
{
    public float Damage,KnockBack;
    [SerializeField] protected Collider[] _WeaponColliders;

    protected virtual void EnableWeaponCollision() { }

    protected virtual void DisableWeaponCollision() { }

}
