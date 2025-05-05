using AI.FSM;
using AI.Enemies.Chaser;
using UnityEngine;

namespace AI.Enemies.Chaser.States
{
    public class IdleState : IState
    {
        private ChaserAgent agent;
        private float idleDuration;
        private float timer;

        public IdleState(ChaserAgent agent, float idleDuration)
        {
            this.agent = agent;
            this.idleDuration = idleDuration;
        }

        public void OnEnter()
        {
            timer = 0f;
            agent.StopMovement();
            agent.GetRandomTargetNode();
            Debug.Log("I'm in Idle State");
        }

        public void OnUpdate()
        {
            if (!agent.TargetNode && agent.Grid.generated)
            {
                agent.GetRandomTargetNode();
            }
            agent.DetectPlayer();
            timer += Time.deltaTime;
            if (agent.PlayerTarget)
            {
                agent.ChangeState(ChaserAgent.StateType.Chase);
            }
            if (timer >= idleDuration && agent.TargetNode)
            {
                agent.ChangeState(ChaserAgent.StateType.Roam);
            }
        }

        public void OnExit() { }
    }
}
