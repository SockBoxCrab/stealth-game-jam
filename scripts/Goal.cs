using Godot;
using System;

public partial class Goal : Area3D
{
	[Signal]
	public delegate void GoalReachedEventHandler();
	void CheckCollision(Node3D body)
	{
		if (body.IsInGroup("Player")) { EmitSignal(SignalName.GoalReached); }
	}
}
