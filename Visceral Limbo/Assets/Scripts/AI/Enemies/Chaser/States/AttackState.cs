// AttackState.cs

using AI.FSM;
using UnityEngine;

namespace AI.Enemies.Chaser.States
{
    public class AttackState : IState
    {
        private ChaserAgent agent;
        private float attackCooldownTimer;

        public AttackState(ChaserAgent agent)
        {
            this.agent = agent;
        }

        public void OnEnter()
        {
            agent.StopMovement();
            attackCooldownTimer = 0f;
        }

        public void OnUpdate()
        {
            if (!agent.CanSeePlayer())
            {
                agent.ChangeState(ChaserAgent.StateType.Roam);
                return;
            }

            float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.PlayerTarget.transform.position);
            if (distanceToPlayer > agent.attackRange)
            {
                agent.ChangeState(ChaserAgent.StateType.Chase);
                return;
            }

            attackCooldownTimer -= Time.deltaTime;

            if (attackCooldownTimer <= 0f)
            {
                agent.PlayerEntity.TakeDamage(agent.attackDamage);
                attackCooldownTimer = agent.attackInterval;
            }
        }

        public void OnExit()
        {
            // Se puede resetear lógica si fuera necesario
        }
    }
}