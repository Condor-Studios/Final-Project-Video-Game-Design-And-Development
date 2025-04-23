using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public class StateMachine
    {
        private Dictionary<Enum, IState> states;
        private IState currentState;

        public StateMachine(Dictionary<Enum, IState> states, Enum startingState)
        {
            this.states = states;
            currentState = states[startingState];
            currentState.OnEnter();
        }

        public void Update()
        {
            currentState?.OnUpdate();
        }

        public void ChangeState(Enum newState)
        {
            currentState?.OnExit();
            currentState = states[newState];
            currentState?.OnEnter();
        }

        public IState CurrentState => currentState;
    }
}
