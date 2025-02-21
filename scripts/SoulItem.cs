using Godot;
using MedivalQuest.DI;
using MedivalQuest.DI.Interfaces;
using MedivalQuest.DI.Constants;

public partial class SoulItem : Area2D
{
	[Export]
	public float FloatAmplitude { get; set; } = 8.0f;
	
	[Export]
	public float FloatSpeed { get; set; } = 3.0f;
	
	[Export]
	public float RotationSpeed { get; set; } = 1.0f;
	
	[Export]
	public float PulseSpeed { get; set; } = 2.0f;
	
	[Export]
	public float PulseAmount { get; set; } = 0.2f;
	
	private ISoulItemService _soulItemService;
	private bool _isInitialized = false;
	private CollisionShape2D _collisionShape;
	
	public override void _EnterTree()
	{
		base._EnterTree();
		AddToGroup("item");
	}
	
	public override void _Ready()
	{
		_soulItemService = DIContainer.Resolve<ISoulItemService>();
		
		// Get collision shape reference
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		
		// Initialize physics properties safely with proper layer masks
		SetDeferred("monitoring", true);
		SetDeferred("monitorable", true);
		SetDeferred("collision_layer", CollisionLayers.ToBitmask(CollisionLayers.Item));
		SetDeferred("collision_mask", CollisionLayers.ToBitmask(CollisionLayers.Player, CollisionLayers.PlayerHitbox));
		
		if (_collisionShape != null)
		{
			_collisionShape.SetDeferred("disabled", false);
		}
		
		// Connect to tree entered signal to ensure proper initialization
		TreeEntered += OnTreeEntered;
		
		// Connect collision signal
		AreaEntered += OnAreaEntered;
	}

	private void OnTreeEntered()
	{
		if (_isInitialized) return;
		_isInitialized = true;

		// Initialize services after we're in the tree
		_soulItemService.Initialize(this);
		_soulItemService.SetInitialPosition(Position);
		
		var sprite = GetNode<Sprite2D>("Sprite2D");
		if (sprite != null)
		{
			_soulItemService.SetSprite(sprite);
		}
	}
	
	public override void _Process(double delta)
	{
		if (!_isInitialized || !IsInstanceValid(this) || !IsInsideTree()) return;
		_soulItemService?.UpdateFloating(delta);
	}
	
	private void OnAreaEntered(Area2D area)
	{
		if (!_isInitialized || !IsInstanceValid(this) || !IsInsideTree()) return;
		
		// Check for valid collision with player areas
		if (area.Name == "BodyArea2D")
		{
			_soulItemService?.HandleCollection(area, CollisionType.Body);
		}
		else if (area.Name == "AttackArea2D")
		{
			_soulItemService?.HandleCollection(area, CollisionType.Attack);
		}
	}
	
	public override void _ExitTree()
	{
		base._ExitTree();
		TreeEntered -= OnTreeEntered;
		AreaEntered -= OnAreaEntered;
		RemoveFromGroup("item");
		
		// Clean up collision shape
		if (_collisionShape != null && IsInstanceValid(_collisionShape))
		{
			_collisionShape.QueueFree();
		}
	}
} 
