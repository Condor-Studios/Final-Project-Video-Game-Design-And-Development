using AI.FSM;
using System.Diagnostics;
using UnityEngine;
using AI.Enemies.Chaser;

namespace AI.Enemies.Chaser.States
{
    public class AttackState : IState
    {
        private ChaserAgent agent;
        private int damage;
        private float attackInterval;
        private float attackTimer;

        public AttackState(ChaserAgent agent, int damage, float attackInterval)
        {
            this.agent = agent;
            this.damage = damage;
            this.attackInterval = attackInterval;
        }

        public void OnEnter()
        {
            attackTimer = 0f;
            agent.StopMoving();
        }

        public void OnUpdate()
        {
            if (agent.PlayerTarget == null || !agent.PlayerEntity.IsAlive)
            {
                agent.ChangeState(ChaserAgent.StateType.Idle);
                return;
            }

            agent.transform.LookAt(agent.PlayerTarget);

            attackTimer += Time.deltaTime;
            if (attackTimer >= attackInterval)
            {
                attackTimer = 0f;
                agent.PlayerEntity.TakeDamage(damage);
                UnityEngine.Debug.Log($"{agent.name} golpeó a {agent.PlayerTarget.name} por {damage} de daño.");
            }

            float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.PlayerTarget.position);
            if (distanceToPlayer > agent.attackRange + 1f)
            {
                agent.ChangeState(ChaserAgent.StateType.Chase);
            }
        }

        public void OnExit() { }
    }
}
