using UnityEngine;

/// <summary>
/// A helper class that takes a <c>HitBoxData</c> struct and makes it easy to query its flipped version.
/// </summary>
public class OrientableHitbox
{
	private HitBoxData _forward;
	private HitBoxData _backward;

	public OrientableHitbox(HitBoxData hitBoxData)
	{
		_forward = hitBoxData;
		_backward = FlipHitboxData(hitBoxData);
	}

	public HitBoxData OrientedHitBoxData(Direction direction)
	{
		return direction == Direction.Backward ? _backward : _forward;
	}

	private HitBoxData FlipHitboxData(HitBoxData data)
	{
		var flippedHurtBoxes = new Rect[data.HurtBoxes.Length];
		for (int i = 0; i < data.HurtBoxes.Length; i++)
		{
			flippedHurtBoxes[i] = RectUtils.MirrorRect(data.HurtBoxes[i]);
		}

		HitBoxData flipped = new()
		{
			CollisionBox = RectUtils.MirrorRect(data.CollisionBox),
			HurtBoxes = flippedHurtBoxes,
			Attack = data.Attack,
		};

		if (flipped.Attack.HasValue)
		{
			AttackInfo flippedAttack = flipped.Attack.Value;
			Direction newDir = flippedAttack.Direction switch
			{
				Direction.Backward => Direction.Forward,
				Direction.Forward => Direction.Backward,
				_ => Direction.None,
			};
			flippedAttack.Direction = newDir;
			flipped.Attack = flippedAttack;
		}

		return flipped;
	}
}
