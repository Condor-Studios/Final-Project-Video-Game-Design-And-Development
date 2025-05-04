using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health_Component : Visceral_Component
{
    public float CurrentHealth, MaxHealth;
    public Rigidbody _RB;
    public bool DestroyOnDeath;
    [SerializeField] PlayerContext _Context;


    public event Action OnDeath, OnDamaged;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        _RB= GetComponent<Rigidbody>();
        _Context= GetComponentInParent<PlayerContext>();

    }

    public void TakeDamage(DamageScore DamageDT)
    {
        CurrentHealth -= DamageDT.DamageAmount;
        OnDamaged?.Invoke();
        if(CurrentHealth <= 0)
        {
            Death(DamageDT);
        }
    }

    public void TakeDamageWithKnockback(Vector3 Direction,float knockback,DamageScore DamageDT)
    {
        CurrentHealth -= DamageDT.DamageAmount;
        OnDamaged?.Invoke();
        if (_RB != null)
        {
            Vector3 FinalForce = Direction * knockback;
            //_RB.AddForce(FinalForce, ForceMode.Impulse);
            _Context.KCCMotor.AttachedRigidbody.AddForce(FinalForce, ForceMode.Impulse);
        }
        if (CurrentHealth <= 0)
        {
            Death(DamageDT);
        }
    }

    public void SimpleDamage(float Damage)
    {
        CurrentHealth -= Damage;
        OnDamaged?.Invoke();
        if (CurrentHealth <= 0)
        {
            Death();
        }
    }

    //use of tuples - patricio malvasio
    /// <summary>
    /// vector3 : la direccion del ataque
    /// float 1 : el daño del ataque
    /// float 2 : el knockback del ataque
    /// </summary>
    /// <param name="MyTuple"></param>
    public void SimpleDamage(Tuple<Vector3,float,float> MyTuple)
    {
        CurrentHealth -= MyTuple.Item2;
        OnDamaged?.Invoke();
        if (CurrentHealth <= 0)
        {
            Death();
        }
    }

    void Death(DamageScore DamageDT)
    {
        OnDeath?.Invoke(); 
        var FinalScore = CreateFinalScore(DamageDT);

        ScoreManager.Instance.ProcessKill(FinalScore);
        if (DestroyOnDeath)
        {
            Destroy(this.gameObject);
            _Context.KCCMotor.CharacterController = null;
        }
        else
        {
            this.gameObject.SetActive(false);
            _Context.PlayerGameObject.SetActive(false);
        }
    }

    void Death()
    {
        OnDeath?.Invoke();

        if (DestroyOnDeath)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);
            _Context.PlayerGameObject.SetActive(false);
        }
    }


    private DamageScore CreateFinalScore(DamageScore DamageDT)
    {
        DamageScore FinalScore = DamageDT;
        if(_Context.KCCMotor.GroundingStatus.IsStableOnGround && !DamageDT.IsAirBorneKill)
        {
            FinalScore.IsAirBorneKill = false;
        }
        else
        {
            FinalScore.IsAirBorneKill = true;
        }
   

        if(CurrentHealth < -MaxHealth * 0.25)
        {
            FinalScore.Overkill = CurrentHealth * -1;
        }
        else
        {
            FinalScore.Overkill = 0;
        }

        FinalScore.Victim = _Context;
        FinalScore.EnemyScoreBase = _Context.EnemyValueScore;

        return FinalScore;
    }


}
