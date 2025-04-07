using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//struct usado para comunicar variables de accion
public struct InputMovement
{
    public Quaternion rotation;
    public Vector2 Movement;
    public bool Jumping;
}

//patricio malvasio maddalena
// 6/4/2025 21:57
//este script es la base del movimiento del jugador
//recibira valores mediante un STRUCT INPUT MOVEMENT
// y los usara para realizar las diversas acciones del juego

public class Player_Movement : Visceral_Script, ICharacterController
{
    [Header("References")]
    [SerializeField] private KinematicCharacterMotor _KCCMotor; //controlador kinematico
    [SerializeField] private Transform _CameraTarget; //la posicion donde quiero que este la camara

    [Header("Variables")]
    [SerializeField] private float _WalkSpeed =10f;
    [SerializeField] private float _JumpStrenght = 20f;
    [SerializeField] private float _GravityStrenght = -90f;
 
    //
    //-----------Requests---------
    //Explicacion, el KCC utiliza un frame rate distinto al update function basico
    //eso significa que existe la posibilidad de que el update KCC ocurra entre frame de input
    //vamos a crear un pseudo cache de acciones necesarias para funcionar
    //
    private Quaternion _RequestedRotation;
    private Vector3 _RequestedMovement;
    private bool _RequestedJump;

    public override void VS_Initialize()
    {
        _KCCMotor.CharacterController = this;
    }

    public void UpdateInput(InputMovement Inputs)
    {
        //setear requestedrotation, esto es el raw input a procesar
        _RequestedRotation = Inputs.rotation;

        //setear requestedmovement, esto es el raw input a procesar
        _RequestedMovement = new Vector3(Inputs.Movement.x,0, Inputs.Movement.y);
        _RequestedMovement.Normalize();

        //orientamos el movimiento al frente de la camara
        _RequestedMovement = Inputs.rotation * _RequestedMovement;

        //seteamos RequestedJump cuando el Boton de Jump es presionado
        _RequestedJump = _RequestedJump || Inputs.Jumping;
    }



    //funcion usada para obtener el anchor de la camara
    public Transform GetCameraTarget() => _CameraTarget;



    ///
    ///
    ///--------- Kinematic Character Controller Functions
    ///
    /// todas las funciones a partir de acá son parte del KCC utilizado para el movimiento
    ///

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        //queremos actualizar la rotacion del modelo a partir del (_RequestedRotation)

        //pero no queremos rotarlo de arriba-abajo porque sería raro
        //por ende vamos a tener que clampear la rotacion usada

        //para lograr esto vamos a tener que proyectar un vector apuntado a la misma direccion
        //que el jugador esta viendo a un plano de tierra

        //matematica avanzada, here be dragons

        var forward = Vector3.ProjectOnPlane
            (
                _RequestedRotation * Vector3.forward,
                _KCCMotor.CharacterUp
            );
        //asignamos la rotacion del modelo
        currentRotation = Quaternion.LookRotation(forward,_KCCMotor.CharacterUp);
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {

        //check si el KCC esta en el suelo estable
        if (_KCCMotor.GroundingStatus.IsStableOnGround)
        {
            #region groundmovement
            //queremos actualizar la posicion del jugador a partir del raw input (RequestedMovement)

            //Problema! nuestra direccion de movimiento esta dada X la camara, entonces si miramos
            //hacia arriba, nuestro vector de movimiento va hacia el aire,
            // Y el KCC resuelve eso pegando al jugador al suelo, lo cual lo ralentiza
            //alternativamente, el jugador vuela 

            //solucion, vamos a proyectar el movimiento al suelo, en vez de usar el raw de la camara
            //El KCC implementa eso mismo en la funcion GetDirectionTangentToSurface
            var groundedMovement = _KCCMotor.GetDirectionTangentToSurface
                (
                    direction: _RequestedMovement,
                    surfaceNormal: _KCCMotor.GroundingStatus.GroundNormal


                //es necesario multiplicar por magnitud porque esta funcion devuelve una unidad de vector / mata magnitud
                ) * _RequestedMovement.magnitude;

            //movernos en la direccion deseada
            currentVelocity = _WalkSpeed * groundedMovement;
            #endregion
        }
        else
        //estamos en el aire
        {
            //aplicar gravedad
            currentVelocity += _GravityStrenght * deltaTime * _KCCMotor.CharacterUp;
        }

        if (_RequestedJump)
        {
            _RequestedJump = false;

            //despegamos al personaje del suelo
            //(funcion del KCC)

            _KCCMotor.ForceUnground(0f);

            //seteamos la velocidad minima vertical a la de salto
            var CurrentVerticalSpeed = Vector3.Dot(currentVelocity, _KCCMotor.CharacterUp);
            var TargetVecticalSpeed = Mathf.Max(CurrentVerticalSpeed, _JumpStrenght);

            //añadimos la diferencia entre velocidad actual y la deseada al KCC

            currentVelocity += _KCCMotor.CharacterUp * (TargetVecticalSpeed - CurrentVerticalSpeed);
        }

    }
    public void AfterCharacterUpdate(float deltaTime)
    {

    }

    public void BeforeCharacterUpdate(float deltaTime)
    {

    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void PostGroundingUpdate(float deltaTime)
    {

    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {

    }
}
