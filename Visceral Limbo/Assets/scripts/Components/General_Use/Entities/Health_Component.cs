using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Component : Visceral_Component
{
    public float CurrentHealth, MaxHealth;
    public Rigidbody _RB;
    public bool DestroyOnDeath;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        _RB= GetComponent<Rigidbody>();
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if(CurrentHealth <= 0)
        {
            Death();
        }
    }

    public void TakeDamageWithKnockback(float damage,Vector3 Direction,float knockback)
    {
        CurrentHealth -= damage;
        if(_RB != null)
        {
            Vector3 FinalForce = Direction * knockback;
            _RB.AddForce(FinalForce, ForceMode.Impulse);
        }
        if (CurrentHealth <= 0)
        {
            Death();
        }
    }


    void Death()
    {
        if (DestroyOnDeath)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
