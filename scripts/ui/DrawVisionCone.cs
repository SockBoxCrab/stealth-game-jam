using Godot;
using System;

[Tool]
public partial class DrawVisionCone : Sprite2D
{
	[Export]
	private Vector2[] visionConePoints;

	const int MTOPX = 10;

    [Export]
    private Color[] visionColor;

    public override void _Ready()
    {
        // multiply everything by the Meters to Pixels ratio
        for (int i = 0; i < visionConePoints.Length; i++)
        {
            visionConePoints[i] *= MTOPX;
        }
    }
    public override void _Draw()
    {
        DrawPolygon(visionConePoints, visionColor);
    }
}
