using Godot;
using MedivalQuest.DI.States;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States.EnemyStates
{
    public class EnemyDeathState : EntityState
    {
        public EnemyDeathState(IGameEntity entity, EntityStateMachine stateMachine) 
            : base(entity, stateMachine)
        {
        }

        public override void Enter()
        {
            Entity.SetAnimation("death");
        }

        public override void Exit()
        {
        }

        public override void Update(double delta)
        {
            // Death state is handled by animation events
        }
    }
} 