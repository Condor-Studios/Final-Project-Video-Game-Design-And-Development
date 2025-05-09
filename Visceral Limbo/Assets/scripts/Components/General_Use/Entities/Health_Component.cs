using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health_Component : Visceral_Component
{
    public float CurrentHealth, MaxHealth;
    public Rigidbody _RB;
    public bool DestroyOnDeath,DesactivateOnDeath,Died;
    [SerializeField] PlayerContext _Context;
    public PlayerContext Context { get { return _Context; } }


    public event Action OnDeath, OnDamaged;

    private void Start()
    {
        CurrentHealth = MaxHealth;
        _RB= GetComponent<Rigidbody>();
        _Context= GetComponentInParent<PlayerContext>();

        if (_Context.faction == FactionID.Player) OnDamaged += updateHealthBar;
            
    }

    public void TakeDamage(DamageScore DamageDT)
    {
        CurrentHealth -= DamageDT.DamageAmount;
        OnDamaged?.Invoke();
        if(CurrentHealth <= 0 && !Died)
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
            if(_Context.KCCMotor != null)
            {
                _Context.KCCMotor.AttachedRigidbody.AddForce(FinalForce, ForceMode.Impulse);
            }
            
        }
        if (CurrentHealth <= 0 && !Died)
        {
            print("calling death");
            Death(DamageDT);
        }
    }

    public void SimpleDamage(float Damage)
    {
        CurrentHealth -= Damage;
        OnDamaged?.Invoke();
        if (CurrentHealth <= 0 && !Died)
        {
            Death(new DamageScore());
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
        if (CurrentHealth <= 0 && !Died)
        {
            Death(new DamageScore());
        }
    }

    void Death(DamageScore DamageDT)
    {
        if(DamageDT.Attacker != null)
        {
            var FinalScore = CreateFinalScore(DamageDT);
            ScoreManager.Instance.ProcessKill(FinalScore);
        }

        OnDeath?.Invoke(); 
        Died = true;
        if (DestroyOnDeath)
        {
            Destroy(this.gameObject);
            _Context.KCCMotor.CharacterController = null;
        }
        else if(DesactivateOnDeath)
        {
            this.gameObject.SetActive(false);
            _Context.PlayerGameObject.SetActive(false);
        }
    }

    private DamageScore CreateFinalScore(DamageScore DamageDT)
    {
        Died = true;
        DamageScore FinalScore = DamageDT;
        if(_Context.KCCMotor != null)
        {
            if (_Context.KCCMotor.GroundingStatus.IsStableOnGround && !DamageDT.IsAirBorneKill)
            {
                FinalScore.IsAirBorneKill = false;
            }
            else
            {
                FinalScore.IsAirBorneKill = true;
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

        FinalScore.Victim = _Context;
        FinalScore.EnemyScoreBase = _Context.EnemyValueScore;

        return FinalScore;
    }

    private void updateHealthBar()
    {
        if(_Context.faction == FactionID.Player)
        {
            Combat_UI_Manager._Instance.UpdatePlayerHealthBar(CurrentHealth,MaxHealth);
            if(CurrentHealth <= 0)
            {
                Combat_UI_Manager._Instance.DisplayLose(true);
            }
        }
    }

}
