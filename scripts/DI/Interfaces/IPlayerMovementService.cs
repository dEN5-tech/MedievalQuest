using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface IPlayerMovementService
    {
        void Initialize(CharacterBody2D player);
        void HandleMovement(double delta);
        void HandleJump();
        void SetMovementSpeed(float speed);
        void SetJumpForce(float force);
        void SetGravity(float gravity);
        Vector2 GetVelocity();
        void SetVelocity(Vector2 velocity);
    }
} 