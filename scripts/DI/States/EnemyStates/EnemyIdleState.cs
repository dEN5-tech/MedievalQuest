using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States.EnemyStates
{
    public class EnemyIdleState : EntityState
    {
        private double _idleTimer;
        private const double IDLE_DURATION = 2.0;

        public EnemyIdleState(IGameEntity entity, EntityStateMachine stateMachine) 
            : base(entity, stateMachine)
        {
            _idleTimer = IDLE_DURATION;
        }

        public override void Enter()
        {
            Entity.SetAnimation("idle");
            Entity.SetVelocity(Vector2.Zero);
            _idleTimer = IDLE_DURATION;
        }

        public override void Update(double delta)
        {
            _idleTimer -= delta;
            
            if (_idleTimer <= 0)
            {
                StateMachine.SetState("patrol");
            }

            // Check for player in range
            var player = GetNearestPlayer();
            if (player != null)
            {
                float distance = Entity.Position.DistanceTo(player.Position);
                if (distance <= 50.0f) // Attack range
                {
                    StateMachine.SetState("attack");
                }
                else if (distance <= 200.0f) // Chase range
                {
                    StateMachine.SetState("chase");
                }
            }
        }

        private IGameEntity GetNearestPlayer()
        {
            // In a real implementation, you would use the scene tree or a game manager
            // to find the nearest player. This is just a placeholder.
            return null;
        }
    }
} 