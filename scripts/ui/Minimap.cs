using Godot;
using System;

public partial class Minimap : Node2D
{
	private Node3D playerNode;
	private int guardCount;
	
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

	// 1 meter in 3D space = 10 pixels in 2D space
	// apply this to playerPos and guardPos everytime they are updated
	const int MTOPX = 10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Grab the player and guard nodes
		playerNode = GetNode<Node3D>("../../../../Player");
		guardCollection = GetNode<Node>("../../../../Guards");

		guardCount = guardCollection.GetChildCount();
		guardTransform = new Node3D[guardCount];
		guardPos = new Vector2[guardCount];
		guardIndicator = new Sprite2D[guardCount];
		for (int i = 0; i < guardCount; i++)
		{
			guardTransform[i] = guardCollection.GetChild<Node3D>(i);
			guardPos[i] = new Vector2(guardTransform[i].GlobalPosition.X, guardTransform[i].GlobalPosition.Z);
			guardPos[i] *= MTOPX;
		}

		playerPos = new Vector2(playerNode.GlobalPosition.X, playerNode.GlobalPosition.Z);
		playerPos *= MTOPX;
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
		// as well as apply this position to the sprites
		playerPos = new Vector2(playerNode.GlobalPosition.X, playerNode.GlobalPosition.Z);
		playerPos *= MTOPX;
		playerIndicator.GlobalPosition = playerPos;

		for (int i = 0; i < guardCount; i++)
		{
			guardPos[i] = new Vector2(guardTransform[i].GlobalPosition.X, guardTransform[i].GlobalPosition.Z);
			guardPos[i] *= MTOPX;
			guardIndicator[i].GlobalPosition = guardPos[i];
			guardIndicator[i].Rotation = guardTransform[i].GetNode<MeshInstance3D>("TempModel").GlobalRotation.Y * -1;
		}
	}
}
