using Godot;
using System;

public partial class GameUI : Control
{
	[ExportCategory("HUD Elements")]
	[Export]
	SubViewportContainer minimap;
	[Export]
	Label pauseLabel;
	[Export]
	Label outroLabel;
	[Export]
	ColorRect fadeTransitionRect;
	AnimationPlayer animationPlayer;

	private bool isFinished = false;

	[Signal]
	public delegate void IntroFadeTransitionEventHandler();
	[Signal]
	public delegate void OutroFadeTransitionEventHandler();
	[Signal]
	public delegate void RestartLevelEventHandler();

    public override void _Ready()
    {
        pauseLabel.Visible = false;
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("pause"))
		{
			if (!isFinished)
			{
				pauseLabel.Visible = !pauseLabel.Visible;
				if (pauseLabel.Visible) { GetTree().Paused = true; }
				else { GetTree().Paused = false; }
			}
			else
			{
				EmitSignal(SignalName.RestartLevel);
			}
		}
    }

	public void EmitIntroFade()
	{
		EmitSignal(SignalName.IntroFadeTransition);
	}

	public void EmitOutroFade()
	{
		EmitSignal(SignalName.OutroFadeTransition);
		outroLabel.Visible = true;
	}

	private void StartIntroFade()
	{
		animationPlayer.Play("introFadeTransition");
	}

	private void StartOutroFade()
	{
		animationPlayer.Play("outroFadeTransition");
		isFinished = true;
	}
}
