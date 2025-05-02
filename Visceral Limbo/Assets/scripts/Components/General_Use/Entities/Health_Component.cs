using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public void TakeDamage(DamageScore DamageDT)
    {
        CurrentHealth -= DamageDT.DamageAmount;
        if(CurrentHealth <= 0)
        {
            Death(DamageDT);
        }
    }

    public void TakeDamageWithKnockback(Vector3 Direction,float knockback,DamageScore DamageDT)
    {
        CurrentHealth -= DamageDT.DamageAmount;
        if(_RB != null)
        {
            Vector3 FinalForce = Direction * knockback;
            _RB.AddForce(FinalForce, ForceMode.Impulse);
        }
        if (CurrentHealth <= 0)
        {
            Death(DamageDT);
        }
    }

    public void SimpleDamage(float Damage)
    {
        CurrentHealth -= Damage;
        if(CurrentHealth <= 0)
        {
            Death();
        }
    }


    void Death(DamageScore DamageDT)
    {

        var FinalScore = CreateFinalScore(DamageDT);

        ScoreManager.Instance.ProcessKill(FinalScore);
        if (DestroyOnDeath)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);
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


    private DamageScore CreateFinalScore(DamageScore DamageDT)
    {
        DamageScore FinalScore = DamageDT;
        if(this.TryGetComponent(out Rigidbody RB)) 
        {
            if(RB.velocity.y > 1)
            {
                FinalScore.IsAirBorneKill = true;
            }
            else
            {
                FinalScore.IsAirBorneKill = false;
            }
        }

        if(CurrentHealth < -MaxHealth * 0.25)
        {
            FinalScore.Overkill = CurrentHealth * -1;
        }
        else
        {
            FinalScore.Overkill = 0;
        }

        FinalScore.IsFriendlyFire = false;

        return FinalScore;
    }


}
