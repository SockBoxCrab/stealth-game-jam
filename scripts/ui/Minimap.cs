using Godot;
using System;

public partial class Minimap : Node2D
{
	[Export]
	private Node3D playerTransform;
	private int guardCount;
	[Export]
	private Node guardCollection;
	private Node3D[] guardTransform;
	private Node guardGroup;

	// variables to store the sprite nodes
	private Sprite2D playerIndicator;
	private Sprite2D[] guardIndicator;


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
		guardTransform = new Node3D[guardCount];
		guardPos = new Vector2[guardCount];
		guardIndicator = new Sprite2D[guardCount];
		for (int i = 0; i < guardCount; i++)
		{
			guardTransform[i] = guardCollection.GetChild<Node3D>(i);
			guardPos[i] = new Vector2(guardTransform[i].GlobalPosition.X, guardTransform[i].GlobalPosition.Z);
		}

		playerPos = new Vector2(playerTransform.GlobalPosition.X, playerTransform.GlobalPosition.Z);
		playerIndicator = GetNode<Sprite2D>("PlayerIndicator");

		guardGroup = GetNode<Node>("%EnemyGroup");
		// make copies of each of the enemy sprite to populate under this group
		var baseGuardSprite = GetNode<Sprite2D>("%EnemyIndicatorBase");
		for (int i = 0; i < guardCount; i++)
		{
			var newGuard = (Sprite2D)baseGuardSprite.Duplicate();
			guardGroup.AddChild(newGuard);
			newGuard.GlobalPosition = guardPos[i];
			newGuard.Visible = true;
			guardIndicator[i] = newGuard;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// in here is where we update the positions of everything. This includes the background

		// update the current 2D positions of the players and the guards
		playerPos = new Vector2(playerTransform.GlobalPosition.X, playerTransform.GlobalPosition.Z);
		for (int i = 0; i < guardCount; i++)
		{
			guardPos[i] = new Vector2(guardTransform[i].GlobalPosition.X, guardTransform[i].GlobalPosition.Z);
		}

		// set minimap camera to follow the player
		//minimapCam.GlobalPosition = playerPos;

		// then, down here we apply the player/guard positions to the sprites
		
	}
}
