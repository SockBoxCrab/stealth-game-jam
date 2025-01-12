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

	[Signal]
	public delegate void StartIntroCamEventHandler();
	[Signal]
	public delegate void StartOutroCamEventHandler();
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		goal = GetNode<Area3D>("Goal");
		// since this means we are entering the scene for the first time
		// we should play our intro stuff.
		GetTree().Paused = true;
		// The HUD should not be visible during this intro
		// and the main camera that does the pan should still operate while paused

		// This starts the camera intro thingy
		EmitSignal(SignalName.StartIntroCam);
		outroCamera.Current = false;
		introCamera.Current = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void StopIntroCam()
	{
		introCamera.Current = false;
		outroCamera.Current = false;
		GetTree().Paused = false;
	}

	private void StartOutro()
	{
		GetTree().Paused = true;
		EmitSignal(SignalName.StartOutroCam);
	}

	private void SetOutroCam()
	{
		goal.Visible = false;
		introCamera.Current = false;
		outroCamera.Current = true;
	}

	private void RestartLevel()
	{
		// here we would set something like,
		// "Yes, we are doing a quick restart" which would tell
		// this level manager to skip the intro sequence
		// But for now we can just reload the entire scene
		GetTree().ChangeSceneToFile(levelFileName);
	}
}
