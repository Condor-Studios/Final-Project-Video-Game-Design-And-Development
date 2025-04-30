using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class DumbChaserAI : MonoBehaviour, ICharacterController
{
    [SerializeField] KinematicCharacterMotor _KKC;
    [SerializeField] Material _mat;
    [SerializeField] Transform target;
    [SerializeField] float speed,movementspeed,attackingspeed,dashspeed,MovementAcceleration,distanceToTarget;
    [SerializeField] float rotationspeed;
    [SerializeField] bool SuccessfullAttack;

    Vector3 RequestedAdditiveVelocity;
    Vector3 RequestedForceVelocity;

    private Vector3 targetdirection;


    private void Start()
    {
        _KKC= GetComponent<KinematicCharacterMotor>();
        _KKC.CharacterController = this;
        _KKC.AttachedRigidbodyOverride = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if(target != null)
        {
            targetdirection = target.transform.position - _KKC.Capsule.transform.position;
            distanceToTarget = Vector3.Distance(_KKC.Capsule.transform.position, target.transform.position);
        }

        if(distanceToTarget < 10)
        {
            if (!SuccessfullAttack)
            {
                StartCoroutine(dashwindup());
            }
        }
        else
        {
            SuccessfullAttack = false;
            _mat.color = Color.white;
            StopAllCoroutines();
            speed = movementspeed;
            _KKC.AttachedRigidbody.velocity = Vector3.Slerp(_KKC.AttachedRigidbodyVelocity,Vector3.zero,0.1f + Time.deltaTime);
            if(_KKC.AttachedRigidbodyVelocity.sqrMagnitude <= 0.1f)
            {
                _KKC.AttachedRigidbody.velocity = Vector3.zero;
            }
        }
    }

    IEnumerator dashwindup()
    {
        _mat.color = Color.yellow;
        speed = attackingspeed;
        SuccessfullAttack = true;
        yield return new WaitForSeconds(3f);
        _mat.color = Color.red;
        Vector3 Dashattack = targetdirection.normalized * dashspeed;
        RequestForceVelocity(Dashattack);
        SuccessfullAttack = false;

    }

    private void RequestExtraVelocity(Vector3 ExtraVelocity)
    {
        _mat.color = Color.red;
        _KKC.ForceUnground(0.1f);
        RequestedAdditiveVelocity += ExtraVelocity;
    }

    private void RequestForceVelocity(Vector3 ForceVelocity)
    {
        _mat.color = Color.red;
        _KKC.ForceUnground(0.1f);
        RequestedForceVelocity = ForceVelocity;
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_KKC.GroundingStatus.IsStableOnGround)
        {


            var groundedMovement = _KKC.GetDirectionTangentToSurface(
                direction: targetdirection,
                surfaceNormal: _KKC.GroundingStatus.GroundNormal

                );


            var TargetVelocity = speed * groundedMovement;
            currentVelocity = Vector3.Slerp
                (
                    a: currentVelocity,
                    b: TargetVelocity,
                    t: 1f - Mathf.Exp(-MovementAcceleration * deltaTime)

                );

        }
        else
        {
            currentVelocity = new Vector3(0, -10, 0);
        }


        if(RequestedAdditiveVelocity.sqrMagnitude > 0)
        {

            currentVelocity += RequestedAdditiveVelocity;
            RequestedAdditiveVelocity = Vector3.zero;
        }

        if(RequestedForceVelocity.sqrMagnitude > 0)
        {
            _KKC.AttachedRigidbody.AddForce(RequestedForceVelocity, ForceMode.Impulse);
            RequestedForceVelocity= Vector3.zero;
        }
    }


    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane(
                vector: targetdirection,
                _KKC.CharacterUp


            );

        currentRotation = Quaternion.LookRotation(forward, _KKC.CharacterUp);
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
