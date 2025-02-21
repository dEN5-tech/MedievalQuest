using Godot;
using MedivalQuest.DI.States;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States.EnemyStates
{
    public class EnemyHitState : EntityState
    {
        public EnemyHitState(IGameEntity entity, EntityStateMachine stateMachine) 
            : base(entity, stateMachine)
        {
        }

        public override void Enter()
        {
            Entity.SetAnimation("hit");
        }

        public override void Exit()
        {
        }

        public override void Update(double delta)
        {
            // Hit state is handled by animation events
        }
    }
} 