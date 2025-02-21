using Godot;
using System;
using MedivalQuest.DI;

public partial class Main : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DIContainer.Initialize();
		
		// Now you can get services using:
		// var service = DIContainer.GetService<IYourService>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
