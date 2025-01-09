using Godot;
using System;

public partial class GameUI : Control
{
	#region Debug
	[ExportGroup("Debug")]
	[Export]
	public bool debugEnabled = false;
	[Export]
	private Panel _debugPanel;
	private Label _detectedLabel;
	private string _baseDetectedText;

	private bool inVisionCone = false;
	private bool detected = false;
	#endregion

	//[ExportGroup("")]
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Debug starting");
		_detectedLabel = _debugPanel.GetNode<Label>("DetectedLabel");
		_baseDetectedText = _detectedLabel.Text;

		var guards = GetParent().GetNode<Node>("%Guards");
		
		for (int i = 0; i < guards.GetChildCount(); i++)
		{
			guards.GetChild(i).GetNode<Area3D>("%Area3D").BodyEntered += (body) => DebugInVisionCone(true);
			guards.GetChild(i).GetNode<Area3D>("%Area3D").BodyExited += (body) => DebugInVisionCone(false);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (debugEnabled)
		{
			_detectedLabel.Text = String.Format(_baseDetectedText, 
										inVisionCone, detected);
		}
	}

	#region Debug Signal Connectors
	public void DebugInVisionCone(bool entered)
	{
		if (debugEnabled)
		{
			inVisionCone = entered;
			if (!inVisionCone) { detected = false; }
		}
	}

	public void DebugDetected(bool detect)
	{
		if (debugEnabled)
		{
			detected = detect;
		}
	}
	#endregion
}
