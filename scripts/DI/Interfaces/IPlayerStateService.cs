using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface IPlayerStateService
    {
        void Initialize(Node2D player);
        bool IsGrounded { get; }
        bool IsFacingRight { get; }
        bool IsAttacking { get; }
        bool IsDead { get; }
        void UpdateState();
        void SetAnimation(string animationName);
        void FlipSprite(bool facingRight);
        void Attack(int attackNumber);
        void TakeHit();
        void Die();
    }
} 