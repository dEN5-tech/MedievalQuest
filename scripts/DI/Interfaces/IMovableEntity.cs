using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface IMovableEntity : IEntity
    {
        float MovementSpeed { get; }
        float Gravity { get; }
        bool IsGrounded { get; }
        bool IsFacingRight { get; }
        
        Vector2 GetVelocity();
        void SetVelocity(Vector2 velocity);
        void FlipSprite(bool facingRight);
    }
} 