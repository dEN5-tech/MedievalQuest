using Godot;
using System;
using System.Collections.Generic;
using MedivalQuest.DI;
using MedivalQuest.DI.Events;
using MedivalQuest.DI.States;
using MedivalQuest.DI.States.EnemyStates;
using MedivalQuest.DI.Interfaces;
using MedivalQuest.DI.Constants;
using MedivalQuest.DI.Base;

public partial class Enemy : BaseEnemy
{
	private Node2D _target;
	private Area2D _hurtbox;
	private Area2D _attackArea;
	private double _lastAttackTime;
	private const double ATTACK_COOLDOWN = 1.0;
	private static PackedScene _soulScene;

	[Export]
	public float PatrolSpeed { get; set; } = 50f;

	[Export]
	public float ChaseSpeed { get; set; } = 150f;

	[Export]
	public float AggroRange { get; set; } = 200f;

	[Export]
	public float AttackRange { get; set; } = 50f;

	[Export]
	public NodePath TargetPath { get; set; }

	[Export]
	public Vector2[] PatrolPoints { get; set; } = new Vector2[0];

	private bool _canAttack = true;

	public override void _Ready()
	{
		// Cache soul scene if not already cached
		if (_soulScene == null)
		{
			_soulScene = GD.Load<PackedScene>("res://scenes/soul_item.tscn");
		}

		// Initialize base components
		Initialize();

		// Configure additional movement parameters
		MovementService.SetPatrolSpeed(PatrolSpeed);
		MovementService.SetChaseSpeed(ChaseSpeed);
		MovementService.SetPatrolPoints(PatrolPoints);

		// Configure state parameters
		StateService.SetAggroRange(AggroRange);
		StateService.SetAttackRange(AttackRange);

		// Get target if path is set
		if (!TargetPath.IsEmpty)
		{
			_target = GetNode<Node2D>(TargetPath);
		}

		SetupCollisions();
		SetupAnimations();
	}

	private void SetupCollisions()
	{
		// Set up collision detection with proper layer masks
		SetDeferred("collision_layer", CollisionLayers.ToBitmask(CollisionLayers.Enemy));
		SetDeferred("collision_mask", CollisionLayers.ToBitmask(CollisionLayers.World, CollisionLayers.Player));

		_hurtbox = GetNode<Area2D>("Hurtbox");
		if (_hurtbox != null)
		{
			_hurtbox.SetDeferred("collision_layer", CollisionLayers.ToBitmask(CollisionLayers.Enemy));
			_hurtbox.SetDeferred("collision_mask", CollisionLayers.ToBitmask(CollisionLayers.PlayerHitbox));
			_hurtbox.AreaEntered += OnAreaEntered;
		}

		_attackArea = GetNode<Area2D>("AttackArea");
		if (_attackArea != null)
		{
			_attackArea.SetDeferred("collision_layer", CollisionLayers.ToBitmask(CollisionLayers.EnemyHitbox));
			_attackArea.SetDeferred("collision_mask", CollisionLayers.ToBitmask(CollisionLayers.Player));
			DisableAttackArea();
			_attackArea.AreaEntered += OnAttackAreaEntered;
		}
	}

	private void SetupAnimations()
	{
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		sprite.FrameChanged += OnAnimationFrameChanged;
		sprite.AnimationFinished += OnAnimationFinished;
	}

	protected override void InitializeStateMachine()
	{
		StateMachine = new EntityStateMachine(this);
		StateMachine.AddState("idle", new EnemyIdleState(this, StateMachine));
		StateMachine.AddState("patrol", new EnemyPatrolState(this, StateMachine));
		StateMachine.AddState("chase", new EnemyChaseState(this, StateMachine));
		StateMachine.AddState("attack", new EnemyAttackState(this, StateMachine));
		StateMachine.AddState("hit", new EnemyHitState(this, StateMachine));
		StateMachine.AddState("death", new EnemyDeathState(this, StateMachine));
		StateMachine.SetState("idle");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsDead) return;

		// Update attack cooldown
		if (!_canAttack)
		{
			if (Time.GetTicksMsec() / 1000.0 - _lastAttackTime >= ATTACK_COOLDOWN)
			{
				_canAttack = true;
			}
		}

		// Check for attack opportunity
		if (_target != null && _canAttack && !IsAttacking)
		{
			float distanceToTarget = GlobalPosition.DistanceTo(_target.GlobalPosition);
			if (distanceToTarget <= AttackRange)
			{
				PerformAttack();
				_lastAttackTime = Time.GetTicksMsec() / 1000.0;
				_canAttack = false;
			}
		}

		// Update base state
		UpdateState(delta);
	}

	private void OnAnimationFrameChanged()
	{
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		if (sprite.Animation.ToString() == "attack")
		{
			// Enable attack area during the active attack frames (frames 4-6)
			if (sprite.Frame == 4)
			{
				EnableAttackArea();
			}
			else if (sprite.Frame >= 7)
			{
				DisableAttackArea();
				if (sprite.Frame == 7)
				{
					GameEvents.RaiseAttackEnded(this);
				}
			}
		}
	}

	private void DisableAttackArea()
	{
		if (_attackArea != null)
		{
			_attackArea.SetDeferred("monitoring", false);
			_attackArea.SetDeferred("monitorable", false);
			var attackCollision = _attackArea.GetNode<CollisionShape2D>("AttackCollision");
			if (attackCollision != null)
			{
				attackCollision.SetDeferred("disabled", true);
			}
		}
	}

	private void EnableAttackArea()
	{
		if (_attackArea != null)
		{
			_attackArea.SetDeferred("monitoring", true);
			_attackArea.SetDeferred("monitorable", true);

			var attackCollision = _attackArea.GetNode<CollisionShape2D>("AttackCollision");
			if (attackCollision != null)
			{
				float direction = IsFacingRight ? 1 : -1;
				Vector2 newPosition = new Vector2(Mathf.Abs(attackCollision.Position.X) * direction, attackCollision.Position.Y);
				attackCollision.SetDeferred("position", newPosition);
				attackCollision.SetDeferred("disabled", false);
			}
		}
	}

	private void OnAnimationFinished()
	{
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		string animation = sprite.Animation.ToString();

		if (animation == "attack")
		{
			DisableAttackArea();
			if (!IsDead)
			{
				StateService.SetAnimation("idle");
				_canAttack = true;
			}
		}
		else if (animation == "hit")
		{
			if (!IsDead)
			{
				StateService.SetAnimation("idle");
			}
		}
		else if (animation == "death")
		{
			SpawnSoul();
			QueueFree();
		}

		StateMachine.OnAnimationFinished();
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.Name == "AttackArea2D" && area.GetParent() is Player player)
		{
			TakeDamage(player.AttackDamage);
		}
	}

	private void OnAttackAreaEntered(Area2D area)
	{
		var parent = area.GetParent();
		if (parent is Player player && !player.IsDead)
		{
			GameEvents.RaiseDamageDealt(this, player, AttackDamage);
			player.TakeDamage(AttackDamage);
		}
	}

	private void SpawnSoul()
	{
		if (_soulScene == null) return;

		var soul = _soulScene.Instantiate<Area2D>();
		if (soul == null) return;

		var parent = GetParent();
		if (parent == null) return;

		Vector2 spawnPosition = GlobalPosition + new Vector2(0, -20);
		
		soul.TopLevel = true;
		soul.GlobalPosition = spawnPosition;
		soul.Modulate = new Color(1, 1, 1, 0);
		soul.Scale = Vector2.Zero;

		float appearDelay = 0.5f;
		var sprite = GetNode<AnimatedSprite2D>("Sprite");
		if (sprite != null)
		{
			appearDelay = (float)(sprite.SpriteFrames.GetFrameCount("death") / sprite.SpriteFrames.GetAnimationSpeed("death")) * 0.5f;
		}

		soul.Ready += () =>
		{
			var tween = soul.CreateTween();
			tween.SetTrans(Tween.TransitionType.Quad);
			
			tween.TweenProperty(soul, "modulate:a", 1.0f, 0.3f)
				.SetDelay(appearDelay);
			
			tween.Parallel().TweenProperty(soul, "scale", Vector2.One * 1.2f, 0.2f)
				.SetEase(Tween.EaseType.Out)
				.SetDelay(appearDelay);
				
			tween.TweenProperty(soul, "scale", Vector2.One, 0.1f)
				.SetEase(Tween.EaseType.In);
		};

		parent.CallDeferred(Node.MethodName.AddChild, soul);
	}
} 
