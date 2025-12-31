using UnityEngine;

public static class RectUtils
{
	/// <summary>
	/// Translates Unity's <c>Rect</c> struct.
	/// </summary>
	public static Rect TranslateRect(Rect rect, Vector2 offset)
	{
		return new(rect.x + offset.x, rect.y + offset.y, rect.width, rect.height);
	}

	/// <summary>
	/// Creates a Collision Box <c>Rect</c> from a width and height.
	///
	/// <para>
	/// Collision boxes have their bottom-center aligned with the player position.
	/// </para>
	/// </summary>
	public static Rect CollisionBoxRect(float w, float h)
	{
		return new(-w * 0.5f, 0f, w, h);
	}

	public static Rect MirrorRect(Rect rect)
	{
		return new(-rect.x - rect.width, rect.y, rect.width, rect.height);
	}
}
