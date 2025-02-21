using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States.EnemyStates
{
    public class EnemyPatrolState : EntityState
    {
        private double _patrolTimer;
        private const double PATROL_DURATION = 5.0;

        public EnemyPatrolState(IGameEntity entity, EntityStateMachine stateMachine) 
            : base(entity, stateMachine)
        {
            _patrolTimer = PATROL_DURATION;
        }

        public override void Enter()
        {
            Entity.SetAnimation("walk");
            _patrolTimer = PATROL_DURATION;
        }

        public override void Update(double delta)
        {
            _patrolTimer -= delta;
            
            // Check for player in range
            var player = GetNearestPlayer();
            if (player != null)
            {
                float distance = Entity.Position.DistanceTo(player.Position);
                if (distance <= 50.0f) // Attack range
                {
                    StateMachine.SetState("attack");
                    return;
                }
                else if (distance <= 200.0f) // Chase range
                {
                    StateMachine.SetState("chase");
                    return;
                }
            }

            // Return to idle after patrol duration
            if (_patrolTimer <= 0)
            {
                StateMachine.SetState("idle");
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