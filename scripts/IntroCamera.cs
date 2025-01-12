using Godot;
using System;

public partial class IntroCamera : PathFollow3D
{
	[Signal]
	public delegate void TweenFinishedEventHandler();

    private void FollowPath()
	{
		var tween = CreateTween().TweenProperty(this, "progress_ratio", 1, 10);
		tween.Finished += () => EmitSignal(SignalName.TweenFinished);
	}
}