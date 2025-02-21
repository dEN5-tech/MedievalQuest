using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.States.PlayerStates
{
    public class PlayerIdleState : EntityState
    {
        public PlayerIdleState(IGameEntity entity, EntityStateMachine stateMachine) 
            : base(entity, stateMachine) { }

        public override void Enter()
        {
            Entity.SetAnimation("idle");
        }

        public override void HandleInput()
        {
            float direction = Input.GetAxis("ui_left", "ui_right");
            if (direction != 0)
            {
                StateMachine.SetState("run");
            }

            if (Input.IsActionJustPressed("ui_accept") && Entity.IsGrounded)
            {
                StateMachine.SetState("jump");
            }

            if (Input.IsActionJustPressed("attack"))
            {
                StateMachine.SetState("attack");
            }
        }

        public override void Update(double delta)
        {
            if (!Entity.IsGrounded)
            {
                StateMachine.SetState("fall");
            }
        }
    }
} 