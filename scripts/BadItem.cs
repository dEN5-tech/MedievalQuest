using Godot;
using System;
using MedivalQuest.DI;
using MedivalQuest.DI.Interfaces;
using MedivalQuest.DI.Events;
using MedivalQuest.DI.Constants;

public partial class BadItem : Area2D, IGameEntity
{
	[Export]
	public int DamageAmount { get; set; } = 10;

	[Export]
	public bool DestroyOnContact { get; set; } = true;

	[Export]
	public float MovementSpeed { get; set; } = 0f;

	[Export]
	public float Gravity { get; set; } = 0f;

	[Export]
	public int MaxHealth { get; set; } = 1;

	private AnimatedSprite2D _sprite;
	private IHealthService _healthService;
	private bool _isDead;
	private Vector2 _velocity;
	private bool _isFacingRight = true;

	// IGameEntity implementation
	public bool IsGrounded => true;
	public bool IsFacingRight => _isFacingRight;
	public bool IsAttacking => false;
	public bool IsDead => _isDead;
	public int CurrentHealth => MaxHealth;
	public Vector2 Position => GlobalPosition;

	public override void _Ready()
	{
		// Set up collision layers
		CollisionLayer = CollisionLayers.ToBitmask(CollisionLayers.Item);
		CollisionMask = CollisionLayers.ToBitmask(CollisionLayers.Player);

		// Connect the body entered signal
		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;

		// Get sprite reference and play animation
		_sprite = GetNode<AnimatedSprite2D>("Sprite2D");
		if (_sprite != null)
		{
			_sprite.Play("default");
		}

		// Initialize health service for damage handling
		_healthService = DIContainer.GetService<IHealthService>();
		_healthService?.Initialize(this, MaxHealth);
	}

	private void OnBodyEntered(Node2D body)
	{
		HandleCollision(body);
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.Name == "BodyArea2D")
		{
			HandleCollision(area.GetParent<Node2D>());
		}
	}

	private void HandleCollision(Node2D node)
	{
		// Check if the colliding node is a player and is not dead
		if (node is Player player && !player.IsDead)
		{
			GD.Print($"Player collided with bad item! Dealing {DamageAmount} damage");
			
			// Deal damage through the health service
			player.TakeDamage(DamageAmount);
			
			// Raise damage event
			GameEvents.RaiseDamageDealt(this, player, DamageAmount);

			if (DestroyOnContact)
			{
				Die();
			}
		}
	}

	// IGameEntity implementation methods
	public void TakeDamage(int damage)
	{
		if (!_isDead)
		{
			Die();
		}
	}

	public void Die()
	{
		if (!_isDead)
		{
			_isDead = true;
			GameEvents.RaiseEntityDeath(this);
			QueueFree();
		}
	}

	public Vector2 GetVelocity() => _velocity;

	public void SetVelocity(Vector2 velocity)
	{
		_velocity = velocity;
	}

	public void UpdateState(double delta)
	{
		// Bad items don't have complex state updates
	}

	public void SetAnimation(string animationName)
	{
		if (_sprite != null)
		{
			_sprite.Play(animationName);
		}
	}

	public void FlipSprite(bool facingRight)
	{
		_isFacingRight = facingRight;
		if (_sprite != null)
		{
			_sprite.FlipH = !facingRight;
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		_healthService?.Dispose();
	}
}
