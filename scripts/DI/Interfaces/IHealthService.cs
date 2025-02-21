using System;
using MedivalQuest.DI.Services;

namespace MedivalQuest.DI.Interfaces
{
    public interface IHealthService : IDisposable
    {
        IObservable<int> HealthChanges { get; }
        IObservable<DamageInfo> DamageEvents { get; }
        int CurrentHealth { get; }
        int MaxHealth { get; }
        void Initialize(IGameEntity entity, int maxHealth);
        void TakeDamage(IGameEntity attacker, int damage);
    }
} 