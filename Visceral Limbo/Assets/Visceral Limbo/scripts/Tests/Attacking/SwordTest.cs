using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordTest : Visceral_WeaponBase
{
    

    protected override void DisableWeaponCollision()
    {

        foreach (var Item in _WeaponColliders)
        {
            Item.enabled = false;
        }
    }

    protected override void EnableWeaponCollision()
    {
        foreach(var Item in _WeaponColliders)
        {
            Item.enabled = true;
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger!!");
        if(other.TryGetComponent(out Health_Component HPComp))
        {
            var dir = (other.transform.position - this.transform.position).normalized;
            HPComp.TakeDamageWithKnockback(Damage, dir, KnockBack);
        }
    }
}
