using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.Services
{
    public class PlayerStateService : IPlayerStateService
    {
        private Node2D _player;
        private AnimatedSprite2D _sprite;
        private bool _isGrounded;
        private bool _isFacingRight = true;
        private bool _isAttacking;
        private bool _isDead;
        private double _attackCooldown;
        private const double ATTACK_COOLDOWN_TIME = 0.5; // Half second between attacks

        public bool IsGrounded => _isGrounded;
        public bool IsFacingRight => _isFacingRight;
        public bool IsAttacking => _isAttacking;
        public bool IsDead => _isDead;

        public void Initialize(Node2D player)
        {
            _player = player;
            _sprite = _player.GetNode<AnimatedSprite2D>("Sprite");
            _sprite.AnimationFinished += OnAnimationFinished;
        }

        public void UpdateState()
        {
            if (_player == null || _isDead) return;

            // Update attack cooldown
            if (_attackCooldown > 0)
            {
                _attackCooldown -= _player.GetProcessDeltaTime();
            }

            // Update grounded state if player is CharacterBody2D
            if (_player is CharacterBody2D characterBody)
            {
                _isGrounded = characterBody.IsOnFloor();
            }

            // Update facing direction based on movement
            float direction = Input.GetAxis("ui_left", "ui_right");
            if (direction != 0 && !_isAttacking)
            {
                _isFacingRight = direction > 0;
                FlipSprite(_isFacingRight);
            }

            // Update animation based on state
            if (!_isAttacking)
            {
                UpdateAnimation();
            }
        }

        public void SetAnimation(string animationName)
        {
            if (_sprite != null && _sprite.SpriteFrames.HasAnimation(animationName))
            {
                _sprite.Play(animationName);
            }
        }

        public void FlipSprite(bool facingRight)
        {
            if (_sprite != null)
            {
                _sprite.FlipH = !facingRight;
            }
        }

        public void Attack(int attackNumber)
        {
            if (_isDead || _isAttacking || _attackCooldown > 0) return;

            _isAttacking = true;
            SetAnimation("attack1"); // Only use attack1 animation from player.tscn
            _attackCooldown = ATTACK_COOLDOWN_TIME;
        }

        public void TakeHit()
        {
            if (_isDead) return;

            _isAttacking = false;
            SetAnimation("hit");
        }

        public void Die()
        {
            if (_isDead) return;

            _isDead = true;
            _isAttacking = false;
            SetAnimation("death");
        }

        private void UpdateAnimation()
        {
            if (_sprite == null) return;

            float direction = Input.GetAxis("ui_left", "ui_right");
            var characterBody = _player as CharacterBody2D;
            
            if (characterBody != null)
            {
                if (!characterBody.IsOnFloor())
                {
                    SetAnimation(characterBody.Velocity.Y < 0 ? "jump" : "fall");
                }
                else if (direction != 0)
                {
                    SetAnimation("run");
                }
                else
                {
                    SetAnimation("idle");
                }
            }
        }

        private void OnAnimationFinished()
        {
            if (_sprite.Animation.ToString().StartsWith("attack"))
            {
                _isAttacking = false;
            }
            else if (_sprite.Animation == "hit")
            {
                UpdateAnimation();
            }
        }
    }
} 