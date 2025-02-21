using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface IEntity
    {
        Vector2 Position { get; }
        bool IsDead { get; }
        void Initialize();
        void UpdateState(double delta);
    }
} 