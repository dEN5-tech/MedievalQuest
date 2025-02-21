using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface ICombatEntity : IEntity
    {
        int MaxHealth { get; }
        int CurrentHealth { get; }
        int AttackDamage { get; }
        bool IsAttacking { get; }
        
        void TakeDamage(int damage);
        void PerformAttack();
        void Die();
    }
} 