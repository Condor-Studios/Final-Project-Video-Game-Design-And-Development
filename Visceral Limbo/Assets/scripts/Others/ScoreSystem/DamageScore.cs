using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    Physical,
    Fire,
    Magic,

}


public class DamageScore : MonoBehaviour
{
    public GameObject Attacker;
    public GameObject Victim;
    public float DamageAmount;
    public float EnemyScoreBase;
    public float Overkill;
    public bool IsFriendlyFire;
    public bool IsAirBorneKill;

    public ElementType ElementalDamage;

}
