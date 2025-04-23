using AI.FSM;
using UnityEngine;
using System.Collections.Generic;
using System;
using AI.General;
using AI.Enemies.Chaser;

namespace AI.Enemies.Chaser.States
{
    public class RoamState : IState
    {
        private ChaserAgent agent;
        private float roamRange;

        public RoamState(ChaserAgent agent, float roamRange)
        {
            this.agent = agent;
            this.roamRange = roamRange;
        }

        public void OnEnter()
        {
            TryFindNewTarget();
        }

        public void OnUpdate()
        {
            if (agent.HasReachedDestination())
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
                    if (candidate != null && candidate.isWalkable && (Mathf.Abs(x) + Mathf.Abs(z)) <= roamRange)
                    {
                        candidates.Add(candidate);
                    }
                }
            }

            if (candidates.Count > 0)
            {
                Node selected = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                agent.RequestPath(selected);
            }
            else
            {
                agent.ChangeState(ChaserAgent.StateType.Idle);
            }
        }
    }
}
