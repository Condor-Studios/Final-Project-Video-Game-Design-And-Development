using AI.FSM;
using UnityEngine;
using System.Collections.Generic;
using AI.General;

namespace AI.Enemies.Chaser.States
{
    public class RoamState : IState
    {
        private readonly ChaserAgent agent;

        public RoamState(ChaserAgent agent)
        {
            this.agent = agent;
        }

        public void OnEnter()
        {
            Debug.Log("I'm in Roaming State");
        }

        public void OnUpdate()
        {
            MoveTowardsTarget();
            agent.DetectPlayer();
            Debug.Log(agent.HasReachedDestination(agent.TargetNode.transform.position));
            if (agent.PlayerTarget)
            {
                agent.ChangeState(ChaserAgent.StateType.Chase);
            }
            if (agent.HasReachedDestination(agent.TargetNode.transform.position))
            {
                agent.ChangeState(ChaserAgent.StateType.Idle);
            }
        }

        private void MoveTowardsTarget()
        {
            agent.LookAt(agent.TargetNode.transform.position);;
            agent.MoveTowards(agent.TargetNode);
        }

        public void OnExit()
        {
        }

    }
}
