using Godot;
using System.Collections.Generic;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States
{
    public class EntityStateMachine
    {
        private IGameEntity _entity;
        private Dictionary<string, EntityState> _states;
        private EntityState _currentState;

        public EntityState CurrentState => _currentState;

        public EntityStateMachine(IGameEntity entity)
        {
            _entity = entity;
            _states = new Dictionary<string, EntityState>();
        }

        public void AddState(string stateName, EntityState state)
        {
            _states[stateName] = state;
        }

        public void SetState(string stateName)
        {
            if (!_states.ContainsKey(stateName))
            {
                GD.PrintErr($"State {stateName} not found in state machine");
                return;
            }

            _currentState?.Exit();
            _currentState = _states[stateName];
            _currentState.Enter();
        }

        public void Update(double delta)
        {
            _currentState?.Update(delta);
        }

        public void PhysicsUpdate(double delta)
        {
            _currentState?.PhysicsUpdate(delta);
        }

        public void HandleInput()
        {
            _currentState?.HandleInput();
        }

        public void OnAnimationFinished()
        {
            _currentState?.OnAnimationFinished();
        }
    }
} 