using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollisionTrigger : MonoBehaviour
{
    float Damage, KnockBack;
    bool CanDealDamage;
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
    }
}
