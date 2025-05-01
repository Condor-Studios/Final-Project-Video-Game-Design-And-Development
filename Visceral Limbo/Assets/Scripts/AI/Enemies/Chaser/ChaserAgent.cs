using System;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using AI.Enemies.Chaser.States;
using AI.General;
using Common.Entities;

namespace AI.Enemies.Chaser
{
    public class ChaserAgent : Agent
    {
        public enum StateType
        {
            Idle,
            Roam,
            Chase,
            Attack
        }

        private StateMachine stateMachine;

        public LayerMask playerLayer;
        public Material TargetMaterial;

        private Transform playerTarget;
        private Entity playerEntity;

        private float lostPlayerTimer = 0f;
        private float maxLostPlayerTime = 5f;
        
        public Transform PlayerTarget => playerTarget;
        public Entity PlayerEntity => playerEntity;

        private void Start()
        {
            base.Awake();

            var states = new Dictionary<Enum, IState>
            {
                { StateType.Idle, new IdleState(this, idleDuration) },
                { StateType.Roam, new RoamState(this, roamRange) },
                { StateType.Chase, new ChaseState(this, playerTarget) },
                { StateType.Attack, new AttackState(this) }
            };

            stateMachine = new StateMachine(states, StateType.Idle);
        }

        private void Update()
        {
            stateMachine.Update();
            DetectPlayer();
        }

        public void ChangeState(StateType newState)
        {
            stateMachine.ChangeState(newState);
        }

        public void IncreaseLostPlayerTimer()
        {
            lostPlayerTimer += Time.deltaTime;
        }

        public void ResetLostPlayerTimer()
        {
            lostPlayerTimer = 0f;
        }

        public bool HasLostPlayerForTooLong()
        {
            return lostPlayerTimer >= maxLostPlayerTime;
        }
        
        public bool CanSeePlayer()
        {
            Ray ray = new Ray(agent.transform.position + Vector3.up * 0.5f, agent.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, visionRange, playerLayer))
            {
                return true;
            }
            return false;
        }

        public bool HasReachedDestination(Vector3 destinationPosition)
        {
            return Vector3.Distance(transform.position, destinationPosition) <= 0.5f;
        }

        private void DetectPlayer()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, visionRange, playerLayer);

            foreach (Collider hit in hits)
            {
                Vector3 dirToTarget = (hit.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToTarget);

                if (angle <= visionAngle / 2f)
                {
                    playerTarget = hit.transform;
                    playerEntity = playerTarget?.GetComponent<Entity>();
                    Renderer renderer = playerTarget?.GetComponent<Renderer>();
                    if (renderer)
                    {
                        renderer.material = TargetMaterial;
                        ChangeState(StateType.Chase);
                    }
                    return;
                }
            }

            if (playerTarget && hits.Length == 0)
            {
                // Perdio de vista al jugador
                playerTarget = null;
                playerEntity = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 leftBoundary = Quaternion.Euler(0, -visionAngle / 2f, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, visionAngle / 2f, 0) * transform.forward;

            Gizmos.DrawRay(transform.position, leftBoundary * visionRange);
            Gizmos.DrawRay(transform.position, rightBoundary * visionRange);
            Gizmos.DrawWireSphere(transform.position, visionRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position,roamRange);
        }
    }
}
