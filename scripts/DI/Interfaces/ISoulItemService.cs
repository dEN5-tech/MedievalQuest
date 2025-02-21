using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface ISoulItemService
    {
        void Initialize(Area2D soulItem);
        void UpdateFloating(double delta);
        void HandleCollection(Area2D collidingArea, CollisionType collisionType);
        void CreateCollectionEffect();
        void SetInitialPosition(Vector2 position);
        void SetSprite(Sprite2D sprite);
        bool IsBeingCollected { get; }
    }

    public enum CollisionType
    {
        Body,
        Attack
    }
} 