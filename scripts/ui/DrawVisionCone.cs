using Godot;
using System;

[Tool]
public partial class DrawVisionCone : Sprite2D
{
    const int MTOPX = 10;

    private Vector2[] _visionConePoints;

	[Export]
	public Vector2[] visionConePoints;

    [Export]
    private Color[] visionColor;

    public override void _Draw()
    {
        DrawPolygon(visionConePoints, visionColor);
    }

    private void OnPointsSet()
    {
        GD.Print("Multiplying values by MTOPX!");
        for (int i = 0; i < visionConePoints.Length; i++)
        {
            visionConePoints[i] *= MTOPX;
            GD.Print(visionConePoints[i]);
        }
    }
}
