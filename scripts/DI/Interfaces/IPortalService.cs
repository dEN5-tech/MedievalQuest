using Godot;
using System;

namespace MedivalQuest.DI.Interfaces
{
    public interface IPortalService
    {
        void Initialize(Node2D portalNode);
        void Teleport(IGameEntity entity);
        void SetDestination(Vector2 destination);
        bool IsEntityInRange(IGameEntity entity);
        void SetPortalRange(float range);
        void SetPortalActive(bool active);
        bool IsPortalActive();
    }
} 