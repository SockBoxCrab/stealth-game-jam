using Godot;
using System;

public partial class Minimap : Node2D
{
	[Export]
	private Transform3D playerTransform;
	[Export]
	private Node guardCollection;
	private Transform3D[] guardTransform;
	private int guardCount;

	[Export]
	private Camera2D minimapCam;

	// these will convert the position of the players/guards to 2D
	private Vector2 playerPos;
	private Vector2[] guardPos;

	// an array that references all of the sprites that represent the guards,
	// since the amount that needs to exist will have to be created on loading of the map

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		guardCount = guardCollection.GetChildCount();
		guardTransform = new Transform3D[guardCount];
		guardPos = new Vector2[guardCount];
		for (int i = 0; i < guardCount; i++)
		{
			guardTransform[i] = guardCollection.GetChild<Node3D>(i).Transform;
			guardPos[i] = new Vector2(guardTransform[i].Origin.X, guardTransform[i].Origin.Z);
		}

		playerPos = new Vector2(playerTransform.Origin.X, playerTransform.Origin.Z);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// in here is where we update the positions of everything. This includes the background

		// update the current 2D positions of the players and the guards
		playerPos = new Vector2(playerTransform.Origin.X, playerTransform.Origin.Z);
		for (int i = 0; i < guardCount; i++)
		{
			guardPos[i] = new Vector2(guardTransform[i].Origin.X, guardTransform[i].Origin.Z);
		}

		// set minimap camera to follow the player
		minimapCam.GlobalPosition = playerPos;

		// then, down here we apply the player/guard positions to the sprites
	}
}
