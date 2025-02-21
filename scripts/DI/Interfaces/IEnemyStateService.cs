using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface IEnemyStateService
    {
        void Initialize(Node2D enemy);
        bool IsGrounded { get; }
        bool IsFacingRight { get; }
        bool IsAttacking { get; }
        bool IsDead { get; }
        bool IsAggro { get; }
        float AggroRange { get; }
        float AttackRange { get; }
        
        void UpdateState();
        void SetAnimation(string animationName);
        void FlipSprite(bool facingRight);
        void Attack();
        void TakeHit();
        void Die();
        void SetAggroRange(float range);
        void SetAttackRange(float range);
    }
} 