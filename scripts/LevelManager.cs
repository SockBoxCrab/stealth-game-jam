using Godot;
using System;

public partial class LevelManager : Node3D
{
	/*
	=== Level Manager Script ===
	This script does as the name describes; manages various things throughout the course of a level.
	This script should be attached to the root node of every level in the game (theoretically).
	The main things it handles is the level intro (where the camera pans across),
	restarting the level upon being caught, and completing the level once the goal is reached.
	*/

	private Area3D goal;

	[Export]
	private Camera3D introCamera;
	[Export]
	private Camera3D outroCamera;

	[Export(PropertyHint.File, "*.tscn")]
	private string levelFileName;

	private Global global;

	[Signal]
	public delegate void StartIntroCamEventHandler();
	[Signal]
	public delegate void StartOutroCamEventHandler();
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		goal = GetNode<Area3D>("Goal");
		global = GetNode<Global>("/root/Global");
		// since this means we are entering the scene for the first time
		// we should play our intro stuff.
		GetTree().Paused = true;
		// The HUD should not be visible during this intro
		// and the main camera that does the pan should still operate while paused

		// Start the camera intro as long as we aren't supposed to skip it
		if (global.skipIntro)
		{
			StopIntroCam();
		}
		else
		{
			EmitSignal(SignalName.StartIntroCam);
			outroCamera.Current = false;
			introCamera.Current = true;
			global.curState = Global.GameState.Intro;
		}
	}

	private void StopIntroCam()
	{
		introCamera.Current = false;
		outroCamera.Current = false;
		GetTree().Paused = false;
		global.curState = Global.GameState.Play;
	}

	private void StartOutro()
	{
		GetTree().Paused = true;
		EmitSignal(SignalName.StartOutroCam);
		global.curState = Global.GameState.Finished;
	}

	private void SetOutroCam()
	{
		goal.Visible = false;
		introCamera.Current = false;
		outroCamera.Current = true;
	}

	private void RestartLevel()
	{
		// should this check the level state? i.e. are we finished
		// things like that, to determine whether or not to skip the intro?
		global.skipIntro = true;
		global.GotoScene(levelFileName);
	}

	private void RestartLevelWithIntro()
	{
		global.skipIntro = false;
		global.GotoScene(levelFileName);
	}
}
