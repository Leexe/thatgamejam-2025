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
	public AttackType Type;
	public int Damage;
}

public struct HitBoxData
{
	public Rect CollisionBox;
	public Rect[] HurtBoxes;
	public AttackInfo? Attack;
}
