using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States
{
    public abstract class EntityState
    {
        protected IGameEntity Entity { get; private set; }
        protected EntityStateMachine StateMachine { get; private set; }

        public EntityState(IGameEntity entity, EntityStateMachine stateMachine)
        {
            Entity = entity;
            StateMachine = stateMachine;
        }

        public virtual void Enter() { }
        public virtual void Exit() { }
        public virtual void Update(double delta) { }
        public virtual void PhysicsUpdate(double delta) { }
        public virtual void HandleInput() { }
        public virtual void OnAnimationFinished() { }
    }
} 