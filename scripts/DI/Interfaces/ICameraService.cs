using Godot;

namespace MedivalQuest.DI.Interfaces
{
    public interface ICameraService
    {
        void Initialize(Camera2D camera);
        void UpdateCamera(Vector2 targetPosition);
        void SetZoom(Vector2 zoom);
        void SetLimits(Vector2 min, Vector2 max);
        void SetSmoothingEnabled(bool enabled);
        void SetSmoothingSpeed(float speed);
    }
} 