using Godot;
using System;

public partial class MainMenuUI : Control
{
	void StartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://default.tscn");
	}

	void OptionsButtonPressed()
	{

	}

	void QuitButtonPressed()
	{
		GetTree().Quit();
	}
}
