using UnityEngine;

/// <summary>
/// A helper class for creating and processing "fixed animations".
///
/// <para>
/// Fixed animations are animations that always play through the exact same way
/// (unless they're interrupted). This is in contrast to a punch animation where
/// the recovery frames are different depending on if it hit / missed.
/// </para>
/// </summary>
public class FixedAnimation
{
	private readonly int _frameCount;

	// array[i] --> animation data corresponding to frame i
	private readonly OrientableHitbox[] _hitboxes;
	private readonly Sprite[] _sprites;
	private readonly Vector2[] _movements;

	public FixedAnimation(AnimationPart[] animationParts)
	{
		_frameCount = 0;
		foreach (AnimationPart animationPart in animationParts)
		{
			_frameCount += animationPart.Duration;
		}

		_hitboxes = new OrientableHitbox[_frameCount];
		_sprites = new Sprite[_frameCount];
		_movements = new Vector2[_frameCount];

		int i = 0;
		foreach (AnimationPart animationPart in animationParts)
		{
			for (int j = 0; j < animationPart.Duration; j++)
			{
				_hitboxes[i] = animationPart.Frame;
				_movements[i] = animationPart.ConstantMovement;
				_sprites[i] = animationPart.Sprite;
				i++;
			}
		}
	}

	public static FixedAnimation FromParams(params AnimationPart[] parts)
	{
		return new FixedAnimation(parts);
	}

	//

	public OrientableHitbox HitboxAtFrame(int frame)
	{
		return _hitboxes[frame];
	}

	public Sprite SpriteAtFrame(int frame)
	{
		return _sprites[frame];
	}

	public Vector2 MovementAtFrame(int frame)
	{
		return _movements[frame];
	}

	public bool Over(int frame)
	{
		return frame >= _frameCount;
	}

	#region types

	public struct AnimationPart
	{
		public OrientableHitbox Frame;
		public Sprite Sprite;
		public int Duration;
		public Vector2 ConstantMovement;

		// helper to create AnimationParts without spamming new()
		public static AnimationPart FromFlattenedData(
			Rect collisionBox,
			Rect[] hurtBoxes,
			AttackInfo? attack,
			Sprite sprite,
			int duration,
			Vector2 constantMovement
		)
		{
			OrientableHitbox frame = new(
				new HitBoxData()
				{
					CollisionBox = collisionBox,
					HurtBoxes = hurtBoxes,
					Attack = attack,
				}
			);

			return new AnimationPart()
			{
				Frame = frame,
				Sprite = sprite,
				Duration = duration,
				ConstantMovement = constantMovement,
			};
		}
	}

	#endregion types
}
