using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    //to do: reemplazar por un GlobalBusCommunicator => un script que comunique todos los eventos, similar a un event manager
    public static ScoreManager Instance;

    [SerializeField] float AirKillExtra;
    [SerializeField] float FriendlyFireExtra;
    [SerializeField] float PlayerScore;
    [SerializeField] TextMeshProUGUI ScoreText;


    private void Awake()
    {
        if (Instance == null && Instance != this) Instance = this;
        else Destroy(this.gameObject);
    }


    public void ProcessKill(DamageScore DamageData)
    {
        float Score = DamageData.EnemyScoreBase;
        if (DamageData.IsAirBorneKill) Score += AirKillExtra;
        if (DamageData.Overkill > 0) Score += DamageData.Overkill;

        if(VerifyFriendlyFire(DamageData)) Score +=FriendlyFireExtra;

        print(" is Airborne Kill " + DamageData.IsAirBorneKill +" Is overkill for: " + DamageData.Overkill);
        PlayerScore += Score;
        ScoreText.text = PlayerScore.ToString();
    }


    private bool VerifyFriendlyFire(DamageScore DamageData) 
    {
        if (DamageData.Attacker.faction == DamageData.Victim.faction)
        {
            print("FriendlyFire");
            return true;
        }
        else
        {
            return false;
        }
    }

}

///
/// Script creado por Patricio Malvasio 2/5/2025
///
/// este script sirve como manager del sistema de puntuacion.
///
