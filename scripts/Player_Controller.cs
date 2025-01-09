using Godot;
using System;
using System.Security.Cryptography;

public partial class Player_Controller : CharacterBody3D
{
	[ExportGroup("Movement Properties")]
	[Export]
	public float RunSpeed = 5.0f;
	[Export]
	public float WalkSpeed = 2.5f;

	private float _speed;

	// Variables for Slope Snapping
	[Export]
	private float _maxStepHeight = .5f;
	private RayCast3D _stepRay;
	// Needed in cases where snapping does not count the player as 
	// being "on the floor"
	private bool _snappedLastFrame = false;
	private bool _onFloorLastFrame = false;
	private bool _waitOneFrame = true;

    public override void _Ready()
    {
        _stepRay = GetNode<RayCast3D>("%SnappingRay");
		_stepRay.TargetPosition = new Vector3(0, -_maxStepHeight, 0);

		_speed = RunSpeed;
    }

    public override void _PhysicsProcess(double delta)
	{
		// hold Shift to slowdown movement
		if (Input.IsActionPressed("walk"))
		{
			_speed = WalkSpeed;
		}
		else
		{
			_speed = RunSpeed;
		}
		// This is the movement code that came default with the CharacterBody3D
		// Using it for now, as it gets the basics done. Changes will likely be 
		// made soon enough.
		DefaultMovementFunc(delta);

		/*
		TODO:
		  * Slope/stair snapping
		  you can fly off slopes. You should be sticking to them. Going up
		  them is typically fine, it looks like. I'll see what happens later
		  The basic idea of how this should work though is have a raycast in 
		  front of and slightly below the player's walking area. If it detects 
		  that there is a walkable area on that ray, we should figure out where
		  the player should "snap" to basically. That PhysicsServer3D testing
		  function from this video: https://www.youtube.com/watch?v=Tb-R3l0SQdc
		  Does seem quite useful. Check where it would go, and then put the 
		  player in that spot.
		*/
		SnapDownSlopes();
	}

	private void SnapDownSlopes()
	{
		/*
		A few things to keep in mind:
		1) falling off a ledge, don't snap when the happens. 
			I think just disabling the snapping behavior 
			once gravity kicks in would be fine. 
		
		*/
		var didSnap = false;

		if ((!IsOnFloor()) && Velocity.Y <= 0 && (_onFloorLastFrame || _snappedLastFrame))
		{
			GD.Print("Doing the snapping!");
			var physics_test_result = new PhysicsTestMotionResult3D();
			if (RunBodyTest(this.GlobalTransform, new Vector3(0, -_maxStepHeight, 0), physics_test_result))
			{
				var distanceToMove_y = physics_test_result.GetTravel().Y;
				this.Position = new Vector3(this.Position.X, this.Position.Y + distanceToMove_y, this.Position.Z);
				ApplyFloorSnap();
				didSnap = true;
			}
		}
		_snappedLastFrame = didSnap;
	}

	// handles running the body test stuff, so it's easier to call
	// from is going to be this.GlobalTransform 99% of the time
	// motion is the vector the physics will test, so it should be in
	// the direction of the raycast
	private bool RunBodyTest(Transform3D from, Vector3 motion, 
							PhysicsTestMotionResult3D result = null)
	{
		// if result isn't passed in, create a new one.
		if (result == null) { result = new PhysicsTestMotionResult3D(); }

		// document all this shit, pretty please
		var parameters = new PhysicsTestMotionParameters3D();
		parameters.From = from;
		parameters.Motion = motion;
		// Return if the body would collide with the given simulated physics movement.
		return PhysicsServer3D.BodyTestMotion(this.GetRid(), parameters, result);
	}

	private void DefaultMovementFunc(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left_move", "right_move", 
											"forward_move", "backward_move");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * _speed;
			velocity.Z = direction.Z * _speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		// check for floor on last frame
		// set onFloorLastFrame
		if (!IsOnFloor()) 
		{ 
			if (_waitOneFrame) { _waitOneFrame = false; }
			else { _onFloorLastFrame = false;}
		}
		else 
		{ 
			_onFloorLastFrame = true;
			_waitOneFrame = true;
		}
	}
}
