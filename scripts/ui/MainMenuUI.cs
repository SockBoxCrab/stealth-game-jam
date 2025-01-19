using Godot;
using System;

public partial class MainMenuUI : Control
{
	[Export]
	string StartSceneName = "res://levels/default.tscn";

	void StartButtonPressed()
	{
		var global = GetNode<Global>("/root/Global");
    	global.GotoScene(StartSceneName);
	}

	void OptionsButtonPressed()
	{

	}

	void QuitButtonPressed()
	{
		GetTree().Quit();
	}
}
