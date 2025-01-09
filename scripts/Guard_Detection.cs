using Godot;
using System;

public partial class Guard_Detection : CharacterBody3D
{
	// Body of player, used to continually cast rays
	private Node3D playerBody = null;

	private CollisionShape3D _movementCollider;
	private MeshInstance3D _skin;
	private Area3D _visionCone;

	private bool _followingCommand = false;

	#region Movement
	[ExportCategory("Movement")]
	[Export]
	public float Speed = 2.5f;
	public float rotationSpeed = 10.0f;

	private float _targetAngle = 0;
	#endregion

	#region Pathfinding
	[ExportCategory("Pathfinding")]
	[Export]
	// An empty Node that contains a collection of Marker3Ds that represent
	// a patrol path. The order matters, make sure the top child is the start
	public Node patrolCollection = null;
	[Export]
	// Distance from each path point in a navigation path that the navigation
	// agent considers "finished" when moving to each point
	private float _pathDesiredDistance = .5f;
	[Export]
	// Distance from the final targeted location that the navigation
	// agent considers "finished" with pathfinding.
	private float _targetDesiredDistance = 0;
	// an array of all the patrol points
	public Vector3[] patrolPoints;
	public string[] patrolCommands;
	private int _currentPatrolPoint = 1;
	private NavigationAgent3D _navigationAgent;
	#endregion

	[ExportCategory("Debug")]
	[Export]
	private bool debugEnabled = false;

	#region Signals
	[Signal]
	public delegate void DetectedEventHandler(bool entered);
	#endregion

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// grab all necessary nodes that need to be instantiated on Ready
		_movementCollider = GetNode<CollisionShape3D>("MovementCollider");
		_skin = GetNode<MeshInstance3D>("TempModel");
		_visionCone = GetNode<Area3D>("%Area3D");

		_navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
		_navigationAgent.PathDesiredDistance = _pathDesiredDistance;
		_navigationAgent.TargetDesiredDistance = _targetDesiredDistance;

		// grab all the patrol points for this guard from the assigned
		// patrol collection
		if (patrolCollection != null)
		{
			patrolPoints = new Vector3[patrolCollection.GetChildCount()];
			patrolCommands = new string[patrolCollection.GetChildCount()];
			for (int i = 0; i < patrolCollection.GetChildCount(); i++)
			{
				patrolPoints[i] = patrolCollection.GetChild<Marker3D>(i).GlobalPosition;
				
				// ensure that a marker has a command on it. if it doesn't, mark it as 'none'
				if (patrolCollection.GetChild(i).HasMeta("patrolCommand")) 
				{
					patrolCommands[i] = (string)patrolCollection.GetChild(i).GetMeta("patrolCommand");
				}
				else { patrolCommands[i] = "none"; }
			}
			this.GlobalPosition = patrolPoints[0];

			ActorSetup();
		}
	}

	// connects to the physics signal so that things can stay synchronized
	// as recommended in the documentation: https://docs.godotengine.org/en/stable/tutorials/navigation/navigation_introduction_3d.html
	private void ActorSetup()
	{
		// set the target to the second patrol point, since we 
		// spawn our guard at the first patrol point
		_navigationAgent.TargetPosition = patrolPoints[_currentPatrolPoint];
		GetTargetRotation();
	}

    public override void _PhysicsProcess(double delta)
    {
		// if playerBody has a reference (AKA, it has entered the vision cone)
		// then continually cast rays to ensure player can actually be detected
        if (playerBody != null) { _raycast_to_player(playerBody); }

		var gravity = Vector3.Zero;
		if (!IsOnFloor())
		{
			gravity += GetGravity() * (float)delta;
		}

		/*
		=== Things to think about for Pathfinding ===
		How often to update the "get next path" function?
		*/

		// Follow all of the points in patrolPoints, and follow their commands
		if (patrolCollection != null || !_followingCommand) { FollowPatrolRoute(delta); }

		Velocity += gravity;
		// apply all movement stuff now
		MoveAndSlide();

		if (debugEnabled)
		{
			GD.Print("Target Angle: " + _targetAngle);
		}
    }

	private void FollowPatrolRoute(double delta)
	{
		var currentPos = this.GlobalPosition;
		if (_navigationAgent.IsNavigationFinished())
		{
			// our navigation to the point is finished, move to the next patrol point
			_currentPatrolPoint += 1;
			if (!(_currentPatrolPoint < patrolPoints.Length)) { _currentPatrolPoint = 0; }
			_navigationAgent.TargetPosition = patrolPoints[_currentPatrolPoint];
		}
		var nextPos = _navigationAgent.GetNextPathPosition();

		GetTargetRotation();

		Velocity = currentPos.DirectionTo(nextPos) * Speed;
		
		RotateNodes(delta);
	}

	private void GetTargetRotation()
	{
		var nextPos = _navigationAgent.GetNextPathPosition();

		var index = _currentPatrolPoint - 1;
		if (_currentPatrolPoint == 0) { index = 3; }
		
		var pos2d = new Vector2(this.GlobalPosition.X, -this.GlobalPosition.Z);
		var lookPos2d = new Vector2(nextPos.X, -nextPos.Z);
		var dir = -(pos2d - lookPos2d);
		_targetAngle = Mathf.Atan2(dir.Y, dir.X);
		if (_targetAngle == Mathf.Pi || _targetAngle == -Mathf.Pi) { _targetAngle = 0; }
		else if (_targetAngle == 0) { _targetAngle = Mathf.Pi; }
		_targetAngle += Mathf.DegToRad(90);
	}

	private void RotateNodes(double delta)
	{
		//GD.Print("Current Rotation: " + _skin.Rotation.Y);
		//GD.Print("Target Angle: " + _targetAngle);
		var newY = Mathf.LerpAngle(_skin.Rotation.Y, _targetAngle, (float)delta / .5f);
		var newRotation = new Vector3(_skin.Rotation.X, newY, _skin.Rotation.Z);
		_skin.Rotation = newRotation;

		// need to decide if this should apply to the collision box as well. Unsure if this
		// would cause any problems with the CharacterBody3D node.
		epic
	}

	/*
	Things to do at patrol points:
	1) Stop and look left and right
	2) Stop and look forward
	3) Stop and look one direction, go the other
	4) Quick check behind
	5) Stop and look down
	*/

	#region Detection
    // use this to check when colliding with player
    private void _on_area_3d_body_entered(Node3D body)
	{
		GD.Print("Colliding with something!");
		GD.Print(body);
		if (body.IsInGroup("Player")) { playerBody = body; }
	}

	private void _on_area_3d_body_exited(Node3D body)
	{
		if (body.IsInGroup("Player")) { playerBody = null; }
	}

	private void _raycast_to_player(Node3D body)
	{
		// Grab the detection points on the player's body
		Vector3[] detectionPoints = new Vector3[4]; 
		detectionPoints[0] = body.GetNode<Node3D>("%Point1").GlobalPosition;
		detectionPoints[1] = body.GetNode<Node3D>("%Point2").GlobalPosition;
		detectionPoints[2] = body.GetNode<Node3D>("%Point3").GlobalPosition;
		detectionPoints[3] = body.GetNode<Node3D>("%Point4").GlobalPosition;
		// Raycast to those points and make sure they only collide with the
		// player's collision box and nothing else.
		var visibleCount = 0;
		var spaceState = GetWorld3D().DirectSpaceState;
		for (int i = 0; i < 4; i++)
		{
			// create a ray from the Guards position to the detection point
			// THIS SHOULD CHANGE TO GUARD'S HEAD POS LATER!!!
			var query = PhysicsRayQueryParameters3D.Create(this.GlobalPosition, detectionPoints[i]);
			// Here we are excluding both the collider of the Guard
			// and the player, so it's only looking for walls
			query.Exclude = new Godot.Collections.Array<Rid> 
			{ 
				body.GetNode<CharacterBody3D>(".").GetRid() , 
				GetNode<Area3D>("%Area3D").GetRid(), 
				this.GetRid(),
			};
			var intersection = spaceState.IntersectRay(query);

			if (intersection.Count == 0) { visibleCount++; }
		}

		// check if there was at least two collisions
		if (visibleCount > 1)
		{
			EmitSignal(SignalName.Detected, true);
		}
		else
		{
			EmitSignal(SignalName.Detected, false);
		}
	}
	#endregion
}
