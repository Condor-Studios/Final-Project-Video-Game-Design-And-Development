using System;
using System.Collections.Generic;
using UnityEngine;
using AI.FSM;
using AI.Enemies.Chaser.States;
using AI.General;
using Common.Entities;

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

        public void IncreaseLostPlayerTimer()
        {
            lostPlayerTimer += Time.deltaTime;
        }

        public void ResetLostPlayerTimer()
        {
            lostPlayerTimer = 0f;
        }

        public bool HasLostPlayerForTooLong()
        {
            return lostPlayerTimer >= maxLostPlayerTime;
        }
    }
}
