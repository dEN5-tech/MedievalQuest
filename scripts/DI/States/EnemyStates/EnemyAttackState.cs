using Godot;
using MedivalQuest.DI.States;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States.EnemyStates
{
    public class EnemyAttackState : EntityState
    {
        private const float ATTACK_RANGE = 50.0f;
        private const float CHASE_RANGE = 200.0f;
        private IGameEntity _target;
        private bool _attackStarted;
        private double _attackDuration = 0.0;
        private const double MAX_ATTACK_DURATION = 1.0;

        public EnemyAttackState(IGameEntity entity, EntityStateMachine stateMachine) 
            : base(entity, stateMachine)
        {
        }

        public override void Enter()
        {
            Entity.SetAnimation("attack");
            _target = GetPlayer();
            _attackStarted = true;
            _attackDuration = 0.0;
        }

        public override void Exit()
        {
            _attackStarted = false;
            _attackDuration = 0.0;
        }

        public override void Update(double delta)
        {
            if (_target == null || _target.IsDead)
            {
                StateMachine.SetState("idle");
                return;
            }

            // Update attack duration
            if (_attackStarted)
            {
                _attackDuration += delta;
                if (_attackDuration >= MAX_ATTACK_DURATION)
                {
                    _attackStarted = false;
                    _attackDuration = 0.0;
                }
                return; // Don't check for transitions while attack is in progress
            }

            float distanceToTarget = Entity.Position.DistanceTo(_target.Position);

            // After attack completion, check distances and transition
            if (distanceToTarget <= ATTACK_RANGE && !Entity.IsAttacking)
            {
                // Start new attack if in range
                Entity.SetAnimation("attack");
                _attackStarted = true;
            }
            else if (distanceToTarget <= CHASE_RANGE)
            {
                StateMachine.SetState("chase");
            }
            else
            {
                StateMachine.SetState("patrol");
            }
        }

        private IGameEntity GetPlayer()
        {
            // Cast Entity to Node to access scene tree functionality
            if (Entity is Node node)
            {
                var players = node.GetTree().GetNodesInGroup("player");
                if (players != null && players.Count > 0 && players[0] is IGameEntity player)
                {
                    return player;
                }
            }
            return null;
        }
    }
} 