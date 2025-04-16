using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTest : Visceral_WeaponBase
{
    public override void Attacking()
    {
        EnableWeaponCollision();
    }

    public override void StopAttacking()
    {
        DisableWeaponCollision();
    }

    protected override void DisableWeaponCollision()
    {

        foreach (var Item in _WeaponColliders)
        {
            Item.Activate(false);
            Item.UpdateValues(Damage, KnockBack);
        }
    }

    protected override void EnableWeaponCollision()
    {
        foreach (var Item in _WeaponColliders)
        {
            Item.Activate(true);
            Item.UpdateValues(Damage, KnockBack);
        }

    }
}
