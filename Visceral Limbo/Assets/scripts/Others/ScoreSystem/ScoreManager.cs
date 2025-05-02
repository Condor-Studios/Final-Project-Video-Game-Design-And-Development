using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{

    //to do: reemplazar por un GlobalBusCommunicator => un script que comunique todos los eventos, similar a un event manager
    public static ScoreManager Instance;

    [SerializeField] float AirKillExtra;
    [SerializeField] float FriendlyFireExtra;



    public void ProcessKill(DamageScore DamageData)
    {
        float Score = DamageData.EnemyScoreBase;
        if (DamageData.IsAirBorneKill) Score += AirKillExtra;
        if (DamageData.Overkill > 0) Score += DamageData.Overkill;
        
        if(VerifyFriendlyFire(DamageData)) Score += FriendlyFireExtra;

        print(Score);
    }


    private bool VerifyFriendlyFire(DamageScore DamageData) 
    {
        return true;
    }
}
