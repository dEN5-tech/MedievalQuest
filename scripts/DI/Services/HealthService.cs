using Godot;
using System;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using MedivalQuest.DI.Interfaces;
using MedivalQuest.DI.Events;

namespace MedivalQuest.DI.Services
{
    public class HealthService : IHealthService
    {
        private readonly Subject<int> _healthChanges;
        private readonly Subject<DamageInfo> _damageEvents;
        private readonly IGUIService _guiService;
        private IGameEntity _entity;
        private int _currentHealth;
        private int _maxHealth;

        public IObservable<int> HealthChanges => _healthChanges.AsObservable();
        public IObservable<DamageInfo> DamageEvents => _damageEvents.AsObservable();
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;

        public HealthService(IGUIService guiService)
        {
            _guiService = guiService;
            _healthChanges = new Subject<int>();
            _damageEvents = new Subject<DamageInfo>();

            // Subscribe to damage events to update health
            _damageEvents
                .Subscribe(damageInfo =>
                {
                    UpdateHealth(damageInfo.Damage);
                    ShowDamageNumber(damageInfo);
                });

            // Subscribe to health changes to update UI and check death
            _healthChanges
                .Subscribe(health =>
                {
                    UpdateUI(health);
                    CheckDeath();
                });
        }

        public void Initialize(IGameEntity entity, int maxHealth)
        {
            _entity = entity;
            _maxHealth = maxHealth;
            _currentHealth = maxHealth;
            _healthChanges.OnNext(_currentHealth);
        }

        public void TakeDamage(IGameEntity attacker, int damage)
        {
            if (_entity.IsDead) return;

            var damageInfo = new DamageInfo
            {
                Attacker = attacker,
                Target = _entity,
                Damage = damage,
                Position = _entity.Position + new Vector2(0, -30)
            };

            _damageEvents.OnNext(damageInfo);
            GameEvents.RaiseDamageDealt(attacker, _entity, damage);
        }

        private void UpdateHealth(int damage)
        {
            if (_entity.IsDead) return;

            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            _healthChanges.OnNext(_currentHealth);
        }

        private void UpdateUI(int health)
        {
            if (_entity is Player player)
            {
                _guiService.UpdatePlayerHealth(health, _maxHealth);
            }
            else if (_entity is Enemy enemy)
            {
                _guiService.UpdateEnemyHealth(enemy, health, _maxHealth);
            }
        }

        private void ShowDamageNumber(DamageInfo damageInfo)
        {
            _guiService.ShowDamageNumber(damageInfo.Position, damageInfo.Damage);
        }

        private void CheckDeath()
        {
            if (_currentHealth <= 0 && !_entity.IsDead)
            {
                _entity.Die();
            }
        }

        public void Dispose()
        {
            _healthChanges?.Dispose();
            _damageEvents?.Dispose();
        }
    }

    public class DamageInfo
    {
        public IGameEntity Attacker { get; set; }
        public IGameEntity Target { get; set; }
        public int Damage { get; set; }
        public Vector2 Position { get; set; }
    }
} 