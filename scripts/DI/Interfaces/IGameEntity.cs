using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface IGameEntity
    {
        // Common properties
        bool IsGrounded { get; }
        bool IsFacingRight { get; }
        bool IsAttacking { get; }
        bool IsDead { get; }
        int MaxHealth { get; }
        int CurrentHealth { get; }
        float MovementSpeed { get; }
        float Gravity { get; }
        Vector2 Position { get; }

        // Common methods
        void TakeDamage(int damage);
        void Die();
        Vector2 GetVelocity();
        void SetVelocity(Vector2 velocity);
        void UpdateState(double delta);
        void SetAnimation(string animationName);
        void FlipSprite(bool facingRight);
    }
} 