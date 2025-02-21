using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.Services
{
    public class EnemyStateService : IEnemyStateService
    {
        private Node2D _enemy;
        private AnimatedSprite2D _sprite;
        private bool _isGrounded;
        private bool _isFacingRight = true;
        private bool _isAttacking;
        private bool _isDead;
        private bool _isAggro = false;
        private float _aggroRange = 200f;
        private float _attackRange = 50f;
        private double _attackCooldown;
        private const double ATTACK_COOLDOWN_TIME = 1.0;
        private bool _isAttackAnimationPlaying;

        public bool IsGrounded => _isGrounded;
        public bool IsFacingRight => _isFacingRight;
        public bool IsAttacking => _isAttacking || _isAttackAnimationPlaying;
        public bool IsDead => _isDead;
        public bool IsAggro => _isAggro;
        public float AggroRange => _aggroRange;
        public float AttackRange => _attackRange;

        public void Initialize(Node2D enemy)
        {
            _enemy = enemy;
            _sprite = _enemy.GetNode<AnimatedSprite2D>("Sprite");
            _sprite.AnimationFinished += OnAnimationFinished;
        }

        public void UpdateState()
        {
            if (_enemy == null || _isDead) return;

            // Update attack cooldown
            if (_attackCooldown > 0)
            {
                _attackCooldown -= _enemy.GetProcessDeltaTime();
                if (_attackCooldown <= 0)
                {
                    _attackCooldown = 0;
                    _isAttacking = false;
                }
            }

            // Update grounded state if enemy is CharacterBody2D
            if (_enemy is CharacterBody2D characterBody)
            {
                _isGrounded = characterBody.IsOnFloor();
            }
        }

        public void SetAnimation(string animationName)
        {
            if (_sprite != null && _sprite.SpriteFrames.HasAnimation(animationName))
            {
                _sprite.Play(animationName);
                _isAttackAnimationPlaying = animationName == "attack";
            }
        }

        public void FlipSprite(bool facingRight)
        {
            if (_sprite != null)
            {
                _isFacingRight = facingRight;
                _sprite.FlipH = !facingRight;
            }
        }

        public void Attack()
        {
            if (_isDead || _isAttacking || _attackCooldown > 0) return;

            _isAttacking = true;
            _isAttackAnimationPlaying = true;
            SetAnimation("attack");
            _attackCooldown = ATTACK_COOLDOWN_TIME;
        }

        public void TakeHit()
        {
            if (_isDead) return;

            _isAttacking = false;
            _isAttackAnimationPlaying = false;
            SetAnimation("hit");
        }

        public void Die()
        {
            if (_isDead) return;

            _isDead = true;
            _isAttacking = false;
            _isAttackAnimationPlaying = false;
            SetAnimation("death");
        }

        public void SetAggroRange(float range)
        {
            _aggroRange = range;
        }

        public void SetAttackRange(float range)
        {
            _attackRange = range;
        }

        private void OnAnimationFinished()
        {
            if (_sprite.Animation == "attack")
            {
                _isAttackAnimationPlaying = false;
                // Don't automatically set animation to idle, let state machine handle it
            }
            else if (_sprite.Animation == "hit")
            {
                if (!_isDead)
                {
                    _isAttackAnimationPlaying = false;
                    SetAnimation("idle");
                }
            }
            else if (_sprite.Animation == "death")
            {
                _enemy.QueueFree();
            }
        }
    }
} 