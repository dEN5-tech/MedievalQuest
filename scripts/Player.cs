using Godot;
using MedivalQuest.DI;
using MedivalQuest.DI.Interfaces;
using MedivalQuest.DI.Events;
using MedivalQuest.DI.States;
using MedivalQuest.DI.States.PlayerStates;
using MedivalQuest.DI.Constants;
using System;

public partial class Player : CharacterBody2D, IGameEntity
{
	private IPlayerMovementService _movementService;
	private IPlayerStateService _stateService;
	private IHealthService _healthService;
	private IGUIService _guiService;
	private Area2D _attackArea;
	private Area2D _bodyArea;
	private CollisionShape2D _attackCollision;
	private CollisionShape2D _bodyCollision;
	private int _currentAttack = 1;
	private double _attackComboTimer;
	private double _lastAttackTime;
	private const double ATTACK_COMBO_WINDOW = 0.8f;
	private const double ATTACK_COOLDOWN = 0.2f;
	private EntityStateMachine _stateMachine;

	[Export]
	public float MovementSpeed { get; set; } = 300.0f;

	[Export]
	public float JumpForce { get; set; } = 400.0f;

	[Export]
	public float Gravity { get; set; } = 980.0f;

	[Export]
	public int MaxHealth { get; set; } = 100;

	[Export]
	public int AttackDamage { get; set; } = 20;

	[Export]
	public int MaxCombo { get; set; } = 4;

	[Export]
	public int SoulCount { get; private set; } = 0;

	private bool _canAttack = true;

	// IGameEntity implementation
	public bool IsGrounded => _stateService.IsGrounded;
	public bool IsFacingRight => _stateService.IsFacingRight;
	public bool IsAttacking => _stateService.IsAttacking;
	public bool IsDead => _stateService.IsDead;
	public int CurrentHealth => _healthService.CurrentHealth;
	public new Vector2 Position => GlobalPosition;

	public override void _Ready()
	{
		// Get services from DI container
		_movementService = DIContainer.Resolve<IPlayerMovementService>();
		_stateService = DIContainer.Resolve<IPlayerStateService>();
		_healthService = DIContainer.Resolve<IHealthService>();
		_guiService = DIContainer.Resolve<IGUIService>();

		// Initialize services
		_movementService.Initialize(this);
		_stateService.Initialize(this);
		_healthService.Initialize(this, MaxHealth);

		// Initialize state machine
		_stateMachine = new EntityStateMachine(this);
		
		// Configure movement parameters
		_movementService.SetMovementSpeed(MovementSpeed);
		_movementService.SetJumpForce(JumpForce);
		_movementService.SetGravity(Gravity);

		// Get area references
		_attackArea = GetNode<Area2D>("AttackArea2D");
		_attackCollision = _attackArea.GetNode<CollisionShape2D>("AttackCollision");
		_bodyArea = GetNode<Area2D>("BodyArea2D");
		_bodyCollision = _bodyArea.GetNode<CollisionShape2D>("BodyCollision");

		if (_attackArea != null)
		{
			_attackArea.AreaEntered += OnAttackAreaEntered;
		}

		if (_bodyArea != null)
		{
			_bodyArea.AreaEntered += OnBodyAreaEntered;
		}

		// Ensure attack area is disabled initially
		DisableAttackArea();

		// Connect animation signals
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		sprite.FrameChanged += OnAnimationFrameChanged;
		sprite.AnimationFinished += OnAnimationFinished;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_stateService.IsDead) return;

		// Update attack timers
		UpdateAttackTimers(delta);

		// Always handle basic movement
		_movementService.HandleMovement(delta);

		// Handle mutually exclusive actions (jump or attack)
		if (Input.IsActionJustPressed("ui_accept") && !_stateService.IsAttacking && IsGrounded)
		{
			_movementService.HandleJump();
		}
		else if (Input.IsActionJustPressed("attack") && _canAttack && IsGrounded)
		{
			Attack();
		}

		// Update player state and animations
		_stateService.UpdateState();
	}

	private void UpdateAttackTimers(double delta)
	{
		// Update combo timer
		if (_attackComboTimer > 0)
		{
			_attackComboTimer -= delta;
			if (_attackComboTimer <= 0)
			{
				ResetCombo();
			}
		}

		// Update attack cooldown
		if (!_canAttack)
		{
			if (Time.GetTicksMsec() / 1000.0 - _lastAttackTime >= ATTACK_COOLDOWN)
			{
				_canAttack = true;
			}
		}
	}

	private void Attack()
	{
		if (_stateService.IsDead || _stateService.IsAttacking || !_canAttack) return;

		_stateService.Attack(_currentAttack);
		_lastAttackTime = Time.GetTicksMsec() / 1000.0;
		_canAttack = false;
		
		// Set up combo
		_attackComboTimer = ATTACK_COMBO_WINDOW;
		_currentAttack = (_currentAttack % MaxCombo) + 1;
	}

	private void ResetCombo()
	{
		_currentAttack = 1;
		_attackComboTimer = 0;
	}

	private void EnableAttackArea()
	{
		if (_attackArea != null && _attackCollision != null)
		{
			_attackArea.SetDeferred("monitoring", true);
			_attackArea.SetDeferred("monitorable", true);
			_attackCollision.SetDeferred("disabled", false);
			
			// Update attack area position based on facing direction
			float direction = _stateService.IsFacingRight ? 1 : -1;
			_attackCollision.Position = new Vector2(Mathf.Abs(_attackCollision.Position.X) * direction, _attackCollision.Position.Y);
		}
	}

	private void DisableAttackArea()
	{
		if (_attackArea != null && _attackCollision != null)
		{
			_attackArea.SetDeferred("monitoring", false);
			_attackArea.SetDeferred("monitorable", false);
			_attackCollision.SetDeferred("disabled", true);
		}
	}

	private void OnAnimationFrameChanged()
	{
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		if (sprite.Animation.ToString().StartsWith("attack"))
		{
			// Enable attack collision on specific frames
			if (sprite.Frame == 1)
			{
				EnableAttackArea();
			}
			// Disable attack collision after the swing
			else if (sprite.Frame == 3)
			{
				DisableAttackArea();
			}
		}
	}

	private void OnAnimationFinished()
	{
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		if (sprite.Animation.ToString().StartsWith("attack"))
		{
			DisableAttackArea();
		}
	}

	private void OnAttackAreaEntered(Area2D area)
	{
		var parent = area.GetParent();
		if (area.Name == "Hurtbox" && parent is Enemy enemy && enemy is IGameEntity entityEnemy && !entityEnemy.IsDead)
		{
			enemy.TakeDamage(AttackDamage);
		}
	}

	private void OnBodyAreaEntered(Area2D area)
	{
		if (area is SoulItem soulItem)
		{
			// The soul item will handle its own collection logic
			// through its OnAreaEntered method
		}
	}

	public void TakeDamage(int damage)
	{
		if (_stateService.IsDead) return;
		_healthService.TakeDamage(this, damage);
		_stateService.TakeHit();
		ResetCombo();
	}

	public void Die()
	{
		if (_stateService.IsDead) return;
		_stateService.Die();
		DisableAttackArea();
		GameEvents.RaiseEntityDeath(this);
		_guiService.ShowGameOverScreen();
	}

	// Public methods to access player state
	public new Vector2 GetVelocity() => _movementService.GetVelocity();

	public void CollectSoul()
	{
		SoulCount++;
		GameEvents.RaiseSoulCollected(SoulCount);
	}

	public void UpdateState(double delta)
	{
		if (_stateService.IsDead) return;
		_stateService.UpdateState();
		_stateMachine?.Update(delta);
	}

	public void SetAnimation(string animationName)
	{
		_stateService.SetAnimation(animationName);
	}

	public void FlipSprite(bool facingRight)
	{
		_stateService.FlipSprite(facingRight);
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		AddToGroup("player");
	}

	public override void _ExitTree()
	{
		RemoveFromGroup("player");
		_healthService?.Dispose();
	}
} 
