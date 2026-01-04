using UnityEngine;

public struct InputInfo
{
	public Vector2 Dir;
	public bool KickButton;
	public bool PunchButton;
}

public enum Direction
{
	Backward = -1,
	Forward = 1,
	None = 0,
}

public enum AttackType
{
	High,
	Mid,
	Low,
}

public enum AttackResult
{
	Hit,
	Blocked,
	None,
}

public struct AttackInfo
{
	public Fighter From;
	public Rect Bounds;
	public Direction Direction;
	public Vector2 VisualPosition;
	public Vector2 VisualDirection;
	public AttackType Type;
	public int Damage;
	public bool IsHeavy;
	public bool IsGrab;
}

public struct HitBoxData
{
	public Rect CollisionBox;
	public Rect[] HurtBoxes;
	public AttackInfo? Attack;

	public static HitBoxData Dummy = new()
	{
		CollisionBox = RectUtils.CollisionBoxRect(0.6f, 1.4f),
		HurtBoxes = new Rect[] { new(-0.4f, 0f, 0.8f, 1.0f), new(-0.2f, 0.8f, 0.5f, 0.4f) },
		Attack = null,
	};
}
