using Godot;
using MedivalQuest.DI;
using MedivalQuest.DI.Interfaces;

public partial class Camera : Camera2D
{
	private ICameraService _cameraService;
	
	[Export]
	public float SmoothingSpeed { get; set; } = 5.0f;
	
	[Export]
	public bool EnableSmoothing { get; set; } = true;
	
	[Export]
	public Vector2 CameraZoom { get; set; } = new Vector2(1, 1);
	
	[Export]
	public Node2D Target { get; set; }

	[Export]
	public NodePath TileMapPath { get; set; } = "../TileMapLayer";

	// Default tile size for the map (32x32 is common)
	[Export]
	public Vector2I DefaultTileSize { get; set; } = new Vector2I(32, 32);

	// Default map size in tiles (adjust based on your map)
	[Export]
	public Vector2I DefaultMapSize { get; set; } = new Vector2I(100, 100);
	
	public override void _Ready()
	{
		_cameraService = DIContainer.Resolve<ICameraService>();
		_cameraService.Initialize(this);
		
		// Apply initial settings
		_cameraService.SetSmoothingEnabled(EnableSmoothing);
		_cameraService.SetSmoothingSpeed(SmoothingSpeed);
		_cameraService.SetZoom(CameraZoom);
		
		// Set camera limits based on tilemap if available
		SetupCameraLimits();
	}

	private void SetupCameraLimits()
	{
		if (TileMapPath == null || TileMapPath.IsEmpty)
		{
			GD.PushWarning("TileMapPath is not set for the camera.");
			return;
		}

		var tileMapLayer = GetNode<Node2D>(TileMapPath);
		if (tileMapLayer == null)
		{
			GD.PushWarning($"Could not find TileMapLayer at path: {TileMapPath}");
			return;
		}

		// Calculate world size based on default values
		var worldSizePx = DefaultMapSize * DefaultTileSize;
		
		// Set camera limits using the calculated world size
		// We'll set the minimum limit to 0,0 and maximum to the world size
		_cameraService.SetLimits(
			Vector2.Zero,
			new Vector2(worldSizePx.X, worldSizePx.Y)
		);
	}

	public override void _Process(double delta)
	{
		if (Target != null)
		{
			_cameraService.UpdateCamera(Target.GlobalPosition);
		}
	}
} 
