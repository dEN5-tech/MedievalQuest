using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.Services
{
    public class PlayerMovementService : IPlayerMovementService
    {
        private CharacterBody2D _player;
        private float _movementSpeed = 300.0f;
        private float _jumpForce = 400.0f;
        private float _gravity = 980.0f;
        private Vector2 _velocity = Vector2.Zero;

        public void Initialize(CharacterBody2D player)
        {
            _player = player;
        }

        public void HandleMovement(double delta)
        {
            if (_player == null) return;

            Vector2 velocity = _velocity;

            // Add gravity
            if (!_player.IsOnFloor())
                velocity.Y += _gravity * (float)delta;

            // Handle horizontal movement
            float direction = Input.GetAxis("ui_left", "ui_right");
            velocity.X = direction * _movementSpeed;

            _velocity = velocity;
            _player.Velocity = _velocity;
            _player.MoveAndSlide();
            _velocity = _player.Velocity;
        }

        public void HandleJump()
        {
            if (_player == null) return;

            if (_player.IsOnFloor())
            {
                _velocity.Y = -_jumpForce;
                _player.Velocity = _velocity;
            }
        }

        public void SetMovementSpeed(float speed)
        {
            _movementSpeed = speed;
        }

        public void SetJumpForce(float force)
        {
            _jumpForce = force;
        }

        public void SetGravity(float gravity)
        {
            _gravity = gravity;
        }

        public Vector2 GetVelocity()
        {
            return _velocity;
        }

        public void SetVelocity(Vector2 velocity)
        {
            _velocity = velocity;
        }
    }
} 