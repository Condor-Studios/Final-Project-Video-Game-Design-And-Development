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
            Debug.Log("I'm in Chase State");
        }

        public void OnUpdate()
        {
            
            float distanceToPlayer = Vector3.Distance(agent.transform.position, agent.PlayerTarget.position);
            if (distanceToPlayer <= agent.attackRange)
            {
                agent.ChangeState(ChaserAgent.StateType.Attack);
            }
            else
            {
                MoveTowardsTargetWithVision(agent.PlayerTarget.transform);
            }
        }

        public void OnExit()
        {
        }

        void MoveTowardsTargetWithVision(Transform target)
        {
            Vector3 direction = target.position - agent.transform.position;
            float distance = direction.magnitude;

            Ray ray = new Ray(agent.transform.position + Vector3.up * 0.5f, direction.normalized);
            if (Physics.Raycast(ray, out RaycastHit hit, distance, agent.playerLayer))
            {
                    Node playerNode = agent.Grid.GetNearestWalkableNode(hit.point);
                    // Lo ve directamente: moverse directo y mirar
                    agent.LookAt(target.position);
                    agent.MoveTowards(playerNode);
            }
            else
            {
                // Está bloqueado: usar pathfinding
                Node targetNode = agent.Grid.GetNearestWalkableNode(target.position);
                agent.Pathfinding.FindPath(agent.transform.position, targetNode.transform.position);
            }
        }


    }
}
