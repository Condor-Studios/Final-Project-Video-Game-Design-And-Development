using AI.FSM;
using UnityEngine;
using System.Collections.Generic;
using AI.General;

namespace AI.Enemies.Chaser.States
{
    public class RoamState : IState
    {
        private readonly ChaserAgent agent;
        private readonly float roamRange;

        public RoamState(ChaserAgent agent, float roamRange)
        {
            this.agent = agent;
            this.roamRange = roamRange;
        }

        public void OnEnter()
        {
            Debug.Log("I'm in Roaming State");
            TryFindNewTarget();
        }

        public void OnUpdate()
        {
            if (agent.HasReachedDestination(agent.TargetNode.transform.position))
            {
                agent.ChangeState(ChaserAgent.StateType.Idle);
            }
        }

        public void OnExit() { }

        private void TryFindNewTarget()
        {
            Node startNode = agent.Grid.GetNodeFromWorldPosition(agent.transform.position);
            List<Node> candidates = new List<Node>();
            

            for (int x = -5; x <= 5; x++)
            {
                for (int z = -5; z <= 5; z++)
                {
                    int checkX = startNode.x + x;
                    int checkZ = startNode.z + z;

                    Node candidate = agent.Grid.GetNode(checkX, checkZ);
                    if (candidate && candidate.isWalkable && (Mathf.Abs(x) + Mathf.Abs(z)) <= roamRange)
                    {
                        candidates.Add(candidate);
                    }
                }
            }

            if (candidates.Count > 0)
            {
                Node selected = candidates[Random.Range(0, candidates.Count)];
                if (selected.TrySetAsTarget())
                { 
                    agent.Pathfinding.FindPath(agent.transform.position, selected.transform.position);
                }
                else
                {
                    TryFindNewTarget();
                }
            }
            else
            {
                agent.ChangeState(ChaserAgent.StateType.Idle);
            }
        }
    }
}
