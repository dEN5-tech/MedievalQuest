using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface IEnemyMovementService
    {
        void Initialize(CharacterBody2D enemy);
        void HandleMovement(double delta);
        void HandlePatrol();
        void ChaseTarget(Vector2 targetPosition);
        void SetMovementSpeed(float speed);
        void SetPatrolSpeed(float speed);
        void SetChaseSpeed(float speed);
        void SetGravity(float gravity);
        Vector2 GetVelocity();
        void SetVelocity(Vector2 velocity);
        void SetPatrolPoints(Vector2[] points);
        void SetPatrolWaitTime(float seconds);
    }
} 