using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States.EnemyStates
{
    public class EnemyChaseState : EntityState
    {
        private const float ATTACK_RANGE = 50.0f;
        private const float CHASE_RANGE = 200.0f;
        private IGameEntity _target;

        public EnemyChaseState(IGameEntity entity, EntityStateMachine stateMachine) 
            : base(entity, stateMachine)
        {
        }

        public override void Enter()
        {
            Entity.SetAnimation("walk");
            // Find the player in the scene
            _target = GetPlayer();
        }

        public override void Update(double delta)
        {
            if (_target == null || _target.IsDead)
            {
                StateMachine.SetState("idle");
                return;
            }

            float distanceToTarget = Entity.Position.DistanceTo(_target.Position);

            if (distanceToTarget <= ATTACK_RANGE)
            {
                StateMachine.SetState("attack");
            }
            else if (distanceToTarget > CHASE_RANGE)
            {
                StateMachine.SetState("patrol");
            }
            else
            {
                // Update facing direction
                bool shouldFaceRight = _target.Position.X > Entity.Position.X;
                Entity.FlipSprite(shouldFaceRight);

                // Move towards target
                Vector2 direction = (_target.Position - Entity.Position).Normalized();
                Vector2 currentVelocity = Entity.GetVelocity();
                currentVelocity.X = direction.X * 150.0f; // Chase speed
                Entity.SetVelocity(currentVelocity);
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