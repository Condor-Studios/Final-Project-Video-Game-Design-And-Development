using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Enum para determinar si el agacharse es manteniendo el boton, o tocando y soltando
/// <summary>
/// este enum indica que tipo de input de agachado usa el juego
/// </summary>
public enum CrouchEnum
{
    None,Toggle
}

//Enum para saber en que estado se encuentra el jugador
/// <summary>
/// este enum sirve para entender en que situacion de movimiento encuentra el jugador
/// </summary>
public enum Stance
{
    Standing,crouching
}

//struct usado para comunicar variables de accion
/// <summary>
/// Los inputs que puede recibir el Personaje
/// </summary>
///

public struct InputMovement
{
    /// <summary>
    /// El quaternion de rotacion que tiene que realizar
    /// </summary>
    public Quaternion rotation;

    /// <summary>
    /// el vector de movimiento que tiene que utilizar
    /// </summary>
    public Vector2 Movement;

    /// <summary>
    /// booleano que indica que esta saltando
    /// </summary>
    public bool Jumping;

    /// <summary>
    /// booleano que indica que sigue saltando
    /// </summary>
    public bool JumpSustaining; //bool que se mantiene true mientras el jugador presione espacio

    /// <summary>
    /// Enum que indica que estamos agachandonos
    /// </summary>
    public CrouchEnum Crouch;
}

//  patricio malvasio maddalena
//  6/4/2025 21:57
//  este script es la base del movimiento del jugador
//  recibira valores mediante un STRUCT INPUT MOVEMENT
//  y los usara para realizar las diversas acciones del juego

public class Player_Movement : Visceral_Script, ICharacterController
{
    [Header("References")]
    [SerializeField] private KinematicCharacterMotor _KCCMotor; //controlador kinematico
    [SerializeField] private Transform _CameraTarget; //la posicion donde quiero que este la camara
    [SerializeField] private Transform _RootTransform; // el transform del padre maximo del jugador
    [Space]

    [Header("Ground Movement Variables")]
    [SerializeField] private float _WalkSpeed =15f;
    [SerializeField] private float _CrouchSpeed = 8f;
    [SerializeField] private float _WalkAcceleration = 25f; // controla la aceleracion
    [SerializeField] private float _CrouchAcceleration = 20f; // controla la aceleracion

    [Space]

    [Header("Air Movement Variables")]
    [SerializeField] private float _JumpStrenght = 20f;
    [Range(0,1f)]
    [SerializeField] private float _JumpSustainGravity = 0.4f; //multiplicador de gravedad para el sustain del salto
    [SerializeField] private float _GravityStrenght = -90f;
    [SerializeField] private float _AirSpeed = 10f;
    [SerializeField] private float _AirResponse = 70f;

    [Space]
    [Header("Stance Variables")]
    [SerializeField] private float _StandHeight = 1f;
    [SerializeField] private float _CrouchHeight = 0.5f;

    [Range(-1f,2f)]
    [SerializeField] private float _CameraStandHeight = 0.9f;
    [Range(-1f,2f)]
    [SerializeField] private float _CameraCrouchHeight= 0.6f;
    [SerializeField] private float _CrouchHeightResponse =15f; // controla que tan rapido se agacha

    [Space]

    [SerializeField]private Stance _CurrentStance;
    /// <summary>
    /// El Stance actual del personaje
    /// </summary>
    public Stance CurrentStance { get => _CurrentStance;} // getter
    private Collider[] _UncrouchOverlapColliders; // solo usado para detectar si estamos golpeando algo

    //
    //-----------Requests---------
    //  Explicacion, el KCC utiliza un frame rate distinto al update function basico
    //  eso significa que existe la posibilidad de que el update KCC ocurra entre frame de input
    //  vamos a crear un pseudo cache de acciones necesarias para funcionar
    //
    private Quaternion _RequestedRotation;
    private Vector3 _RequestedMovement;
    private bool _RequestedJump;
    private bool _RequestedSustainJump;
    private bool _RequestedCrouch;

    public override void VS_Initialize()
    {
        _KCCMotor.CharacterController = this;
        _CurrentStance = Stance.Standing;
        _UncrouchOverlapColliders= new Collider[8];
    }

    /// <summary>
    /// esta funcion sirve para procesar los inputs que recibe el Player Movement
    /// </summary>
    /// <param name="Inputs"> los Inputs que recibe el Player Movement</param>
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
        // y seteamos RequestedSustainJump mientras que el boton jump este presionado
        _RequestedJump = _RequestedJump || Inputs.Jumping;

        _RequestedSustainJump = Inputs.JumpSustaining;

        //Seteamos RequestedCrouch
        _RequestedCrouch = Inputs.Crouch switch
        {
            CrouchEnum.Toggle => !_RequestedCrouch,
            CrouchEnum.None => _RequestedCrouch,
            _ => _RequestedCrouch,
        };
          
    }

    // en esta funcion se va a enviar toda la info de posicion y actualizacion
    // del cuerpo del personaje
    
    /// <summary>
    /// Esta funcion sirve para actualizar la posicion de objetos dentro del personaje
    /// </summary>
    /// <param name="DeltaTime"></param>
    public void UpdateBodyPositions(float DeltaTime)
    {

        var CurrentCapsuleHeight = _KCCMotor.Capsule.height;

        //como la escala del transform NO es un valor absoluto, tenemos que calcularla
        //usando la altura base del _KCC y la altura que definimos como parado
        var NormalizedHeight = CurrentCapsuleHeight / _StandHeight;

        //switch de posicion de camara
        var CameraTargetHeight = _CurrentStance switch
        {
            Stance.Standing => _CameraStandHeight,
            Stance.crouching => _CameraCrouchHeight,
            _ => _CameraStandHeight,
        };


        //usamos el Normalized como valor de la nueva escala
        var RootTargetScale = new Vector3(1f, NormalizedHeight, 1f);

        //movemos la camara con lerp hacia la posicion deseada
        _CameraTarget.localPosition = Vector3.Lerp
            (
                a: _CameraTarget.localPosition,
                b: new Vector3(0f, CameraTargetHeight, 0f),
                t: 1f- Mathf.Exp(-_CrouchHeightResponse * DeltaTime) // sirve para generar mas consistencia entre frames
            );

        _RootTransform.localScale = RootTargetScale;
    }



    //funcion usada para obtener el anchor de la camara
    /// <summary>
    /// esta funcion devuelve la posicion del anchor de la camara
    /// </summary>
    /// <returns> posicion de la camara</returns>
    public Transform GetCameraTarget() => _CameraTarget;

    //funcion usada para teletransportar al character

    /// <summary>
    /// esta funcion sirve para setear la posicion del personaje, ya que el KCC no permite usar transform.position
    /// </summary>
    /// <param name="position"> la nueva posicion que queremos que tenga el personaje</param>
    /// <param name="KillVelocity"> si queremos conservar la velocidad actual del personaje</param>
    public void SetCharacterPosition(Vector3 position,bool KillVelocity = true)
    {
        _KCCMotor.SetPosition(position);
        if (KillVelocity)
        {
            _KCCMotor.BaseVelocity = Vector3.zero;
        }
    }
    ///
    ///
    ///--------- Kinematic Character Controller Functions
    ///
    /// todas las funciones a partir de acá son parte del KCC utilizado para el movimiento
    ///
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


            //switches para calcular la velocidad y la aceleracion del personaje
            float movementSpeed = _CurrentStance switch
            {
                Stance.Standing => _WalkSpeed,
                Stance.crouching => _CrouchSpeed,
                _ => _WalkSpeed,
            };

            var MovementAcceleration = _CurrentStance switch
            {
                Stance.Standing => _WalkAcceleration,
                Stance.crouching => _CrouchAcceleration,
                _ => _WalkSpeed,
            };

            //movernos suavemente en la direccion deseada
            var TargetVelocity = movementSpeed * groundedMovement;
            currentVelocity = Vector3.Lerp
                (
                    a: currentVelocity,
                    b: TargetVelocity,
                    t: 1f - Mathf.Exp(-MovementAcceleration * deltaTime)
                );
            #endregion
        }
        else
        //estamos en el aire
        {
            #region airmovement
            //si estamos en el aire tratando de movernos
            if (_RequestedMovement.sqrMagnitude> 0f)
            {
                //calcularemos el movimiento planar que queremos tener
                //similar al movimiento vectorial del suelo pero snappeamos a un plano liso
                //porque no estamos en el suelo 
                var AirTimePlanarMovement = Vector3.ProjectOnPlane
                    (
                        vector:_RequestedMovement,
                        planeNormal: _KCCMotor.CharacterUp
                    ) * _RequestedMovement.sqrMagnitude;

                //calcular la velocidad actual en el plano del aire
                var currentPlanarVelocity = Vector3.ProjectOnPlane
                    (
                        vector: currentVelocity,
                        planeNormal: _KCCMotor.CharacterUp
                    );

                //calcular la fuerza de movimiento
                var InAirMovementForce = AirTimePlanarMovement * _AirResponse * deltaTime;

                //añadir el valor de fuerza a la velocidad planar actual para tener un objetivo de movimiento
                var TargetAirMovementForce = currentPlanarVelocity + InAirMovementForce;

                //limitamos la velocidad maxima a la velocidad de movimiento del aire
                TargetAirMovementForce = Vector3.ClampMagnitude(TargetAirMovementForce, _AirSpeed);

                //steer hacia la velocidad objetivo
                currentVelocity += TargetAirMovementForce - currentPlanarVelocity;
            }

            //aplicar gravedad
            var effectivegravity = _GravityStrenght;
            var verticalSpeed = Vector3.Dot(currentVelocity,_KCCMotor.CharacterUp);
            if (_RequestedSustainJump && verticalSpeed > 0f) //sostener salto
            {
                effectivegravity *= _JumpSustainGravity;
            }
            currentVelocity += effectivegravity * deltaTime * _KCCMotor.CharacterUp;
            #endregion
        }

        // saltar del suelo
        if (_RequestedJump && _KCCMotor.GroundingStatus.IsStableOnGround)
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

    //este evento corre despues de la logica de movimiento
    public void AfterCharacterUpdate(float deltaTime)
    {
        //probamos dejar de agacharnos
        if(!_RequestedCrouch && _CurrentStance is not Stance.Standing)
        {
            //_CurrentStance = Stance.Standing;
            _KCCMotor.SetCapsuleDimensions(_KCCMotor.Capsule.radius, _StandHeight, _StandHeight * 0.5f);

            // chequeo si estariamos a punto de golpear contra algun objeto por encima del jugador
            //transcient => la posicion futura del jugador
            if (_KCCMotor.CharacterOverlap(_KCCMotor.TransientPosition, _KCCMotor.TransientRotation, _UncrouchOverlapColliders, _KCCMotor.CollidableLayers, QueryTriggerInteraction.Ignore) > 0)
            {
                //re agacharse
                _RequestedCrouch = true;
                _KCCMotor.SetCapsuleDimensions(_KCCMotor.Capsule.radius, _CrouchHeight, _CrouchHeight * 0.5f);
            }
            else
            {
                //levantarnos
                _CurrentStance = Stance.Standing;
            }
        }

    }


    //este evento corre antes de la logica de movimiento
    public void BeforeCharacterUpdate(float deltaTime)
    {
        //agacharse
        if (_RequestedCrouch&& _CurrentStance is Stance.Standing)
        {
            _CurrentStance = Stance.crouching;
            _KCCMotor.SetCapsuleDimensions(_KCCMotor.Capsule.radius, _CrouchHeight, _CrouchHeight * 0.5f);
        }

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

    //
    ////Change logs
    //
    // patricio: 9/4/2025 - 19:30 -
    // añadi : movimiento en el aire y agacharce
    // añadi : aceleracion en la tierra y aire
    // añadi : transicion de camara entre agacharse y levantarse
    // añadi : comentarios y summaries a (casi) todas las funciones custom
    // acomode las variables para mas legibilidad
    // 
}
