using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.Services
{
    public class CameraService : ICameraService
    {
        private Camera2D _camera;
        private Vector2 _minLimits;
        private Vector2 _maxLimits;
        private bool _smoothingEnabled;
        private float _smoothingSpeed = 5.0f;
        private bool _limitsEnabled;

        public void Initialize(Camera2D camera)
        {
            _camera = camera;
            _camera.PositionSmoothingEnabled = true;
            _camera.PositionSmoothingSpeed = _smoothingSpeed;
        }

        public void UpdateCamera(Vector2 targetPosition)
        {
            if (_camera == null) return;

            Vector2 newPosition = targetPosition;

            if (_limitsEnabled)
            {
                newPosition.X = Mathf.Clamp(newPosition.X, _minLimits.X, _maxLimits.X);
                newPosition.Y = Mathf.Clamp(newPosition.Y, _minLimits.Y, _maxLimits.Y);
            }

            if (_smoothingEnabled)
            {
                _camera.GlobalPosition = newPosition;
            }
            else
            {
                _camera.GlobalPosition = newPosition;
            }
        }

        public void SetZoom(Vector2 zoom)
        {
            if (_camera == null) return;
            _camera.Zoom = zoom;
        }

        public void SetLimits(Vector2 min, Vector2 max)
        {
            _minLimits = min;
            _maxLimits = max;
            _limitsEnabled = true;
        }

        public void SetSmoothingEnabled(bool enabled)
        {
            _smoothingEnabled = enabled;
            if (_camera != null)
            {
                _camera.PositionSmoothingEnabled = enabled;
            }
        }

        public void SetSmoothingSpeed(float speed)
        {
            _smoothingSpeed = speed;
            if (_camera != null)
            {
                _camera.PositionSmoothingSpeed = speed;
            }
        }
    }
} 