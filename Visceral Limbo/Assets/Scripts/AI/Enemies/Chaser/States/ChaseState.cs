using AI.FSM;
using UnityEngine;
using AI.Enemies.Chaser;
using AI.General;

namespace AI.Enemies.Chaser.States
{
    public class ChaseState : IState
    {
        private ChaserAgent agent;

        public ChaseState(ChaserAgent agent)
        {
            this.agent = agent;
        }

        public void OnEnter()
        {
            UpdateTarget();
        }

        public void OnUpdate()
        {
            if (agent.PlayerTarget == null)
            {
                agent.IncreaseLostPlayerTimer();
                if (agent.HasLostPlayerForTooLong())
                {
                    agent.ChangeState(ChaserAgent.StateType.Idle);
                }
                return;
            }

            float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.PlayerTarget.position);
            if (distanceToPlayer <= agent.attackRange)
            {
                agent.ChangeState(ChaserAgent.StateType.Attack);
            }
            else
            {
                UpdateTarget();
            }
        }

        public void OnExit()
        {
            agent.ResetLostPlayerTimer();
        }

        private void UpdateTarget()
        {
            Node nearestToPlayer = agent.Grid.GetNearestWalkableNode(agent.PlayerTarget.position);
            if (nearestToPlayer != null)
            {
                agent.RequestPath(nearestToPlayer);
            }
        }
    }
}
