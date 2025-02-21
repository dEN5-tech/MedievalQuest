using Godot;
using MedivalQuest.DI.Interfaces;
using System;

namespace MedivalQuest.DI.Services
{
    public class EnemyMovementService : IEnemyMovementService
    {
        private CharacterBody2D _enemy;
        private Vector2 _velocity = Vector2.Zero;
        private float _movementSpeed = 100.0f;
        private float _patrolSpeed = 50.0f;
        private float _chaseSpeed = 150.0f;
        private float _gravity = 980.0f;
        private Vector2[] _patrolPoints = Array.Empty<Vector2>();
        private int _currentPatrolPoint;
        private float _patrolWaitTime = 2.0f;
        private float _currentWaitTime;
        private bool _isWaiting;
        private float _collisionCooldown = 0f;
        private const float COLLISION_COOLDOWN_TIME = 0.5f;
        private Vector2 _lastValidPosition;
        private bool _wasCollidingLastFrame;

        public void Initialize(CharacterBody2D enemy)
        {
            _enemy = enemy;
            _lastValidPosition = enemy.GlobalPosition;
        }

        public void HandleMovement(double delta)
        {
            if (_enemy == null) return;

            Vector2 velocity = _velocity;

            // Apply gravity
            if (!_enemy.IsOnFloor())
                velocity.Y += _gravity * (float)delta;

            // Update collision cooldown
            if (_collisionCooldown > 0)
            {
                _collisionCooldown -= (float)delta;
            }

            // Store previous position
            Vector2 previousPosition = _enemy.GlobalPosition;

            _velocity = velocity;
            _enemy.Velocity = _velocity;
            _enemy.MoveAndSlide();
            _velocity = _enemy.Velocity;

            // Check for collisions
            bool isColliding = false;
            for (int i = 0; i < _enemy.GetSlideCollisionCount(); i++)
            {
                var collision = _enemy.GetSlideCollision(i);
                if (collision.GetCollider() is CharacterBody2D)
                {
                    isColliding = true;
                    break;
                }
            }

            // Handle collision response
            if (isColliding)
            {
                if (!_wasCollidingLastFrame)
                {
                    // Just started colliding
                    _collisionCooldown = COLLISION_COOLDOWN_TIME;
                    // Reverse direction
                    _velocity.X = -_velocity.X;
                    // Move back slightly
                    _enemy.GlobalPosition = previousPosition;
                }
                _wasCollidingLastFrame = true;
            }
            else
            {
                if (!isColliding && _enemy.IsOnFloor())
                {
                    _lastValidPosition = _enemy.GlobalPosition;
                }
                _wasCollidingLastFrame = false;
            }

            // If stuck or in invalid position, return to last valid position
            if (_enemy.GlobalPosition.DistanceTo(_lastValidPosition) > 100)
            {
                _enemy.GlobalPosition = _lastValidPosition;
                _velocity = Vector2.Zero;
            }
        }

        public void HandlePatrol()
        {
            if (_enemy == null || _patrolPoints.Length == 0 || _collisionCooldown > 0) return;

            if (_isWaiting)
            {
                _currentWaitTime -= (float)_enemy.GetProcessDeltaTime();
                if (_currentWaitTime <= 0)
                {
                    _isWaiting = false;
                }
                _velocity.X = 0;
                return;
            }

            Vector2 targetPoint = _patrolPoints[_currentPatrolPoint];
            float distanceToTarget = _enemy.GlobalPosition.DistanceTo(targetPoint);

            if (distanceToTarget < 10)
            {
                // Reached patrol point, wait and move to next point
                _currentPatrolPoint = (_currentPatrolPoint + 1) % _patrolPoints.Length;
                _isWaiting = true;
                _currentWaitTime = _patrolWaitTime;
                _velocity.X = 0;
            }
            else
            {
                // Move towards patrol point
                float direction = Mathf.Sign(targetPoint.X - _enemy.GlobalPosition.X);
                _velocity.X = direction * _patrolSpeed;
            }
        }

        public void ChaseTarget(Vector2 targetPosition)
        {
            if (_enemy == null || _collisionCooldown > 0) return;

            float direction = Mathf.Sign(targetPosition.X - _enemy.GlobalPosition.X);
            _velocity.X = direction * _chaseSpeed;
        }

        public void SetMovementSpeed(float speed)
        {
            _movementSpeed = speed;
        }

        public void SetPatrolSpeed(float speed)
        {
            _patrolSpeed = speed;
        }

        public void SetChaseSpeed(float speed)
        {
            _chaseSpeed = speed;
        }

        public void SetGravity(float gravity)
        {
            _gravity = gravity;
        }

        public Vector2 GetVelocity()
        {
            return _velocity;
        }

        public void SetVelocity(Vector2 velocity)
        {
            _velocity = velocity;
        }

        public void SetPatrolPoints(Vector2[] points)
        {
            _patrolPoints = points;
            _currentPatrolPoint = 0;
        }

        public void SetPatrolWaitTime(float seconds)
        {
            _patrolWaitTime = seconds;
        }
    }
} 