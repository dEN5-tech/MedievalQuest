using Godot;
using MedivalQuest.DI.Interfaces;

namespace MedivalQuest.DI.Services
{
    public class SoulItemService : ISoulItemService
    {
        private Area2D _soulItem;
        private Vector2 _initialPosition;
        private float _time = 0.0f;
        private IGUIService _guiService;
        private Sprite2D _sprite;
        private float _baseScale = 0.75f;
        private bool _isBeingCollected = false;

        // Configuration parameters
        private float _floatAmplitude = 8.0f;
        private float _floatSpeed = 3.0f;
        private float _rotationSpeed = 1.0f;
        private float _pulseSpeed = 2.0f;
        private float _pulseAmount = 0.2f;

        // Collection effect parameters
        private const float BODY_COLLECTION_DURATION = 0.3f;
        private const float ATTACK_COLLECTION_DURATION = 0.2f;
        private const float COLLECTION_TEXT_OFFSET_Y = -20f;

        public bool IsBeingCollected => _isBeingCollected;

        public SoulItemService(IGUIService guiService)
        {
            _guiService = guiService;
        }

        public void Initialize(Area2D soulItem)
        {
            _soulItem = soulItem;
        }

        public void SetInitialPosition(Vector2 position)
        {
            _initialPosition = position;
        }

        public void SetSprite(Sprite2D sprite)
        {
            _sprite = sprite;
        }

        public void UpdateFloating(double delta)
        {
            if (_isBeingCollected || _soulItem == null || !_soulItem.IsInsideTree()) return;

            // Update time
            _time += (float)delta;

            // Make the soul float up and down
            _soulItem.Position = _initialPosition + new Vector2(0, Mathf.Sin(_time * _floatSpeed) * _floatAmplitude);

            // Rotate the soul
            _soulItem.Rotation = Mathf.Sin(_time * _rotationSpeed) * 0.2f;

            // Pulse the soul
            if (_sprite != null && _sprite.IsInsideTree())
            {
                float pulse = 1.0f + (Mathf.Sin(_time * _pulseSpeed) * _pulseAmount);
                _sprite.Scale = new Vector2(_baseScale, _baseScale) * pulse;
            }
        }

        public void HandleCollection(Area2D collidingArea, CollisionType collisionType)
        {
            // Prevent double collection or invalid collection
            if (_isBeingCollected || _soulItem == null || !_soulItem.IsInsideTree()) return;

            // Check if the colliding area belongs to a player
            var parent = collidingArea.GetParent();
            if (parent is Player player && !player.IsDead)
            {
                _isBeingCollected = true;
                
                // Defer all physics-related changes
                _soulItem.SetDeferred("process_mode", (int)Node.ProcessModeEnum.Disabled);
                _soulItem.SetDeferred("collision_layer", 0);
                _soulItem.SetDeferred("collision_mask", 0);
                _soulItem.SetDeferred("monitoring", false);
                _soulItem.SetDeferred("monitorable", false);
                
                // Get collision shape and defer its disabling
                var collisionShape = _soulItem.GetNode<CollisionShape2D>("CollisionShape2D");
                if (collisionShape != null)
                {
                    collisionShape.SetDeferred("disabled", true);
                }

                // Update player's soul count
                player.CollectSoul();

                // Show floating text for soul collection
                Vector2 textOffset = new Vector2(
                    collisionType == CollisionType.Attack ? 10 : 0, 
                    COLLECTION_TEXT_OFFSET_Y
                );
                _guiService?.ShowDamageNumber(_soulItem.GlobalPosition + textOffset, 1);

                // Create collection effect
                CreateCollectionEffect(collisionType);
            }
        }

        public void CreateCollectionEffect(CollisionType collisionType = CollisionType.Body)
        {
            if (_soulItem == null || !_soulItem.IsInsideTree()) return;

            // Stop floating animation
            _initialPosition = _soulItem.Position;

            // Create collection animation
            var tween = _soulItem.CreateTween();
            float duration = collisionType == CollisionType.Attack ? 
                ATTACK_COLLECTION_DURATION : BODY_COLLECTION_DURATION;

            tween.SetTrans(Tween.TransitionType.Quad);

            // Initial "pop" effect
            tween.TweenProperty(_soulItem, "scale", Vector2.One * 1.2f, duration * 0.2f)
                .SetEase(Tween.EaseType.Out);

            if (collisionType == CollisionType.Attack)
            {
                // Add "hit" effect for attack collection
                tween.Parallel().TweenProperty(_soulItem, "position", 
                    _soulItem.Position + new Vector2(10, -5), duration * 0.2f)
                    .SetEase(Tween.EaseType.Out);
            }

            // Scale down and fade out with slight float effect
            tween.TweenProperty(_soulItem, "scale", Vector2.Zero, duration * 0.8f)
                .SetEase(Tween.EaseType.InOut);
            tween.Parallel().TweenProperty(_soulItem, "modulate:a", 0.0f, duration * 0.8f);
            tween.Parallel().TweenProperty(_soulItem, "position:y", 
                _soulItem.Position.Y - 10, duration * 0.8f)
                .SetEase(Tween.EaseType.Out);

            // Queue free after all animations complete
            tween.TweenCallback(Callable.From(() => 
            {
                if (_soulItem != null && _soulItem.IsInsideTree())
                {
                    _soulItem.QueueFree();
                }
            }));
        }

        public void CreateCollectionEffect()
        {
            CreateCollectionEffect(CollisionType.Body);
        }
    }
} 