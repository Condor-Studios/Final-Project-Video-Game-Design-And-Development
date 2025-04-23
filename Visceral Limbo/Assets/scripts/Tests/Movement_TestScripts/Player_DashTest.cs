using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_DashTest : Visceral_Script
{
    [SerializeField] Player_Movement _KCCMotorMovement;
    [SerializeField] private float _DashStrenght;

    public override void VS_InitializeWithParameters(params object[] a)
    {
        if (a[0] != null)
        {
            _KCCMotorMovement= (Player_Movement)a[0];
        }
    }

    public void PerformDash(InputMovement _Inputs)
    {
        if(_Inputs.Ability_Support == true && _KCCMotorMovement.KKCMotor.GroundingStatus.IsStableOnGround)
        {
            float MovementDirX = _Inputs.Movement.x;
            float MovementDirY = _Inputs.Movement.y;
            Transform UpDirector = _KCCMotorMovement.KKCMotor.Transform;

            Vector3 DashMovement = new Vector3(_Inputs.Movement.x,0,_Inputs.Movement.y).normalized;
            Vector3 FinalDashMovement = Quaternion.Euler(0,UpDirector.eulerAngles.y,0) * DashMovement;
            FinalDashMovement *= _DashStrenght;
            _KCCMotorMovement.AddExternalVelocity(FinalDashMovement,true);
        }
    }
}
