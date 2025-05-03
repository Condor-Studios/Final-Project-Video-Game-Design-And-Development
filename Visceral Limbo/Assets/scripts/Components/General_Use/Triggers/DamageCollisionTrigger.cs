using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollisionTrigger : MonoBehaviour
{
    [SerializeField]Collider _Collider;
    float Damage, KnockBack;
    bool CanDealDamage;
    bool DealKnockback;

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
            if (DealKnockback)
            {
                HPComp.SimpleDamage(Damage);
            }
            else
            {
                HPComp.SimpleDamage(Damage);
            }
        }
    }

    public void UpdateValues(float NDamage,float NKnockBack,bool KNOCK)
    {
        Damage = NDamage;
        KnockBack = NKnockBack;
        DealKnockback = KNOCK;
    }

    public void Activate(bool NewValue)
    {
        CanDealDamage = NewValue;
        _Collider.enabled = true;
    }
}
