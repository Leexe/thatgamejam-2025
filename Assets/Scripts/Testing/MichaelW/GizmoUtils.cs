using UnityEngine;

public class GizmoUtils
{
	/// <summary>
	/// Draws a crosshair of a specified size.
	/// </summary>
	public static void DrawCrossHair(Vector2 pos, float size = 0.2f)
	{
		Gizmos.DrawLine(pos * size * Vector2.left, pos * size * Vector2.right);
		Gizmos.DrawLine(pos * size * Vector2.up, pos * size * Vector2.down);
	}

	/// <summary>
	/// Draws a <c>Rect</c>.
	/// </summary>
	public static void DrawRect(Rect rect)
	{
		Gizmos.DrawLine(rect.min, rect.min + (rect.height * Vector2.up));
		Gizmos.DrawLine(rect.min, rect.min + (rect.width * Vector2.right));
		Gizmos.DrawLine(rect.max, rect.max - (rect.height * Vector2.up));
		Gizmos.DrawLine(rect.max, rect.max - (rect.width * Vector2.right));
	}
}
