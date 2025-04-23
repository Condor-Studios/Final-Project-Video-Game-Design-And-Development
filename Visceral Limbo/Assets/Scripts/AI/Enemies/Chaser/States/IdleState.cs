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
            agent.StopMoving();
        }

        public void OnUpdate()
        {
            timer += Time.deltaTime;
            if (timer >= idleDuration)
            {
                agent.ChangeState(ChaserAgent.StateType.Roam);
            }
        }

        public void OnExit() { }
    }
}
