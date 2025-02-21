using Godot;
using System;

namespace MedivalQuest.DI.Interfaces
{
    public interface IGUIService : IDisposable
    {
        void Initialize(Node guiRoot);
        void UpdatePlayerHealth(int currentHealth, int maxHealth);
        void UpdateEnemyHealth(Enemy enemy, int currentHealth, int maxHealth);
        void ShowDamageNumber(Vector2 position, int damage);
        void UpdateSoulCount(int count);
        void ShowGameOverScreen();
        void ShowVictoryScreen();
    }
} 