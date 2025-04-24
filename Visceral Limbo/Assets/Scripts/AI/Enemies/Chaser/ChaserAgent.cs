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

        [Header("Chaser Settings")]
        public float idleDuration = 5f;
        public float roamRange = 5f;
        public float visionRange = 2f;
        public float visionAngle = 45f;
        public float attackRange = 1.5f;
        public float attackInterval = 3f;
        public int attackDamage = 1;
        public LayerMask playerLayer;

        private Transform playerTarget;
        private Entity playerEntity;

        private float lostPlayerTimer = 0f;
        private float maxLostPlayerTime = 5f;
        
        public NodeGrid Grid => grid;
        public Transform PlayerTarget => playerTarget;
        public Entity PlayerEntity => playerEntity;

        private void Start()
        {
            base.Awake();
            Debug.Log(grid);

            var states = new Dictionary<Enum, IState>
            {
                { StateType.Idle, new IdleState(this, idleDuration) },
                { StateType.Roam, new RoamState(this, roamRange) },
                { StateType.Chase, new ChaseState(this) },
                { StateType.Attack, new AttackState(this, attackDamage, attackInterval) }
            };

            stateMachine = new StateMachine(states, StateType.Idle);
        }

        private void Update()
        {
            base.Update(); // Sigue manejando movimiento y escucha de tecla Enter si quer�s mantenerlo
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
                    playerEntity = playerTarget.GetComponent<Entity>();
                    ChangeState(StateType.Chase);
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
