using Godot;
using System;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.Events
{
    public static class GameEvents
    {
        // Health events
        public delegate void HealthChangedHandler(IGameEntity entity, int newHealth);
        public static event HealthChangedHandler OnHealthChanged;
        public static void RaiseHealthChanged(IGameEntity entity, int newHealth) => OnHealthChanged?.Invoke(entity, newHealth);

        public delegate void DamageDealtHandler(IGameEntity attacker, IGameEntity target, int damage);
        public static event DamageDealtHandler OnDamageDealt;
        public static void RaiseDamageDealt(IGameEntity attacker, IGameEntity target, int damage) => OnDamageDealt?.Invoke(attacker, target, damage);

        // Entity events
        public delegate void EntitySpawnedHandler(IGameEntity entity);
        public static event EntitySpawnedHandler OnEntitySpawned;
        public static void RaiseEntitySpawned(IGameEntity entity) => OnEntitySpawned?.Invoke(entity);

        public delegate void EntityDeathHandler(IGameEntity entity);
        public static event EntityDeathHandler OnEntityDeath;
        public static void RaiseEntityDeath(IGameEntity entity) => OnEntityDeath?.Invoke(entity);

        // Combat events
        public delegate void AttackStartedHandler(IGameEntity entity);
        public static event AttackStartedHandler OnAttackStarted;
        public static void RaiseAttackStarted(IGameEntity entity) => OnAttackStarted?.Invoke(entity);

        public delegate void AttackEndedHandler(IGameEntity entity);
        public static event AttackEndedHandler OnAttackEnded;
        public static void RaiseAttackEnded(IGameEntity entity) => OnAttackEnded?.Invoke(entity);

        // Hit confirmation event
        public delegate void HitConfirmedHandler();
        public static event HitConfirmedHandler OnHitConfirmed;
        public static void RaiseHitConfirmed() => OnHitConfirmed?.Invoke();

        // Soul events
        public delegate void SoulCollectedHandler(int totalSouls);
        public static event SoulCollectedHandler OnSoulCollected;
        public static void RaiseSoulCollected(int totalSouls) => OnSoulCollected?.Invoke(totalSouls);

        // Game state events
        public delegate void VictoryHandler();
        public static event VictoryHandler OnVictory;
        public static void RaiseVictory() => OnVictory?.Invoke();
    }
} 