using System;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using AI.Enemies.Chaser.States;
using AI.General;

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

        private float lostPlayerTimer = 0f;
        private float maxLostPlayerTime = 5f;



        private void Start()
        {
            base.Awake();
            base.Start();

            var states = new Dictionary<Enum, IState>
            {
                { StateType.Idle, new IdleState(this, idleDuration) },
                { StateType.Roam, new RoamState(this) },
                { StateType.Chase, new ChaseState(this) },
                { StateType.Attack, new AttackState(this) }
            };

            stateMachine = new StateMachine(states, StateType.Idle);
        }

        private void Update()
        {
            stateMachine.Update();
        }

        public void ChangeState(StateType newState)
        {
            stateMachine.ChangeState(newState);
        }
    }
}
