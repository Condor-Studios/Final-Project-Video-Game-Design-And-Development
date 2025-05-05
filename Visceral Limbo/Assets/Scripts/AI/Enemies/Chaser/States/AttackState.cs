// AttackState.cs

using AI.FSM;
using UnityEngine;

namespace AI.Enemies.Chaser.States
{
    public class AttackState : IState
    {
        private readonly ChaserAgent _agent;
        private float _attackCooldownTimer;

        public AttackState(ChaserAgent agent)
        {
            this._agent = agent;
        }

        public void OnEnter()
        {
            _agent.StopMovement();
            _attackCooldownTimer = 0f;
        }

        public void OnUpdate()
        {
            if (!_agent.CanSeePlayer())
            {
                _agent.ChangeState(ChaserAgent.StateType.Roam);
                return;
            }

            float distanceToPlayer = Vector3.Distance(_agent.transform.position, _agent.PlayerTarget.transform.position);
            if (distanceToPlayer > _agent.attackRange)
            {
                _agent.ChangeState(ChaserAgent.StateType.Chase);
                return;
            }

            _attackCooldownTimer -= Time.deltaTime;

            if (_attackCooldownTimer <= 0f)
            {
                _agent.PlayerEntity.TakeDamage(_agent.attackDamage);
                _attackCooldownTimer = _agent.attackInterval;
            }
        }

        public void OnExit()
        {
            // Se puede resetear lógica si fuera necesario
        }
    }
}