using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//patricio malvasio maddalena
//6/4/2025 21:57
//este script sirve como el corazon de la logica basica del jugador
//este script se va a encargar de actualizar cosas, pasar inputs y funcionar =>
// => como un pseudo state machine.

public class Player_Base : Visceral_Script
{

    [SerializeField] private Player_Movement _Player_Movement;
    [SerializeField] private Player_CameraController _Player_CameraController;
    //PlayerInputActions es el mappeo de las acciones de Input del jugador
    //similar al Unreal con su Input Map
    private PlayerInputActions _Player_InputActions;

    void Start()
    {
        _Player_Movement.VS_Initialize();
        _Player_CameraController.VS_InitializeWithParameters(_Player_Movement.GetCameraTarget());


        //creo e inicio Inputs
        _Player_InputActions = new PlayerInputActions();
        _Player_InputActions.Enable();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //inputs del jugador
        var Input = _Player_InputActions.Gameplay;

        //logica de camara
        //
        // recibir camera input y actualizar la rotacion
        //creo struct
        var cameraInput = new InputStruct
        {
            //set variable lookdelta con el valor del input.look creado en playerinput
            LookDelta = Input.Look.ReadValue<Vector2>()

        };
        _Player_CameraController.UpdateRotation(cameraInput);

        //logica de movimiento

        //recibo movement inputs y actualizo 
        //creo struct
        var movementInput = new InputMovement
        {
            //rotation => rotacion del jugador
            //movement => movimiento del jugador
            //jumping/jumpsustaining => salto del jugador

            rotation       = _Player_CameraController.transform.rotation,
            Movement       = Input.Movement.ReadValue<Vector2>(),
            Jumping        = Input.Jump.WasPressedThisFrame(),
            JumpSustaining = Input.Jump.IsPressed(),

            // pseudo codigo => si el boton crouch fue presionado, devolver toggle , de no serlo devolver none
            Crouch         = Input.Crouch.WasPressedThisFrame() ? CrouchEnum.Toggle : CrouchEnum.None,


        };
        _Player_Movement.UpdateBodyPositions(Time.deltaTime);
        _Player_Movement.UpdateInput(movementInput);
        _Player_CameraController.UpdatePosition(_Player_Movement.GetCameraTarget());

    }



    private void OnDestroy()
    {
        _Player_InputActions?.Dispose(); // me deshago de inputs porque si no siguen de fondo
    }
}
