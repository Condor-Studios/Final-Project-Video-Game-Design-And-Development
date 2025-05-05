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
    public PlayerContext Attacker;
    public PlayerContext Victim;
    public float DamageAmount;
    public float EnemyScoreBase;
    public float Overkill;
    public bool IsFriendlyFire;
    public bool IsAirBorneKill;

    public ElementType ElementalDamage;
    public FactionID FactionID;

}


///
///  Script creado por Patricio Malvasio Maddalena 2/5/2025
///  
/// este script ser� usado para calcular el score de la kill del jugador
/// y pasar info del ataque letal de un enemigo
///
///