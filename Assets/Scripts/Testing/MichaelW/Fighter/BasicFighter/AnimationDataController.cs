using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// A class that interfaces <c>AnimationClip</c> data with fighter data types.
/// </summary
public class AnimationDataController : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private AnimancerComponent _animationController;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[Header("Hitbox References")]
	[SerializeField]
	private Hitbox[] _hurtBoxes;

	[SerializeField]
	private AttackData _attackBox;

	[SerializeField]
	private Hitbox _collisionBox;

	[Button]
	public void SetAnimFrame(AnimationClip clip, int frame)
	{
		// compensate for last frame being dummy
		int trueAnimLength = Mathf.FloorToInt((clip.length * 60f) - 0.5f);
		int trueFrame = ((frame % trueAnimLength) + trueAnimLength) % trueAnimLength;

		AnimancerState state = _animationController.Play(clip);
		// use MoveTime() instead of setting .Time, as the former doesn't skip root motion.
		state.MoveTime((trueFrame * (1 / 60f)) + 0.001f, false);
		state.IsPlaying = false;
	}

	public HitBoxData GetAbsoluteHitBoxData(Fighter self)
	{
		var hurtBoxRects = new Rect[_hurtBoxes.Length];
		for (int i = 0; i < _hurtBoxes.Length; i++)
		{
			// note that bounds are empty if the gameobject is inactive.
			hurtBoxRects[i] = _hurtBoxes[i].GetBoundsAsRect();
		}

		AttackInfo? attack = null;
		if (_attackBox.gameObject.activeSelf)
		{
			bool flipped = transform.localScale.x < 0f;
			Vector2 ogVisualDir = _attackBox.VisualDirection;

			Direction dir = flipped ? TypeUtils.FlipDirection(_attackBox.Direction) : _attackBox.Direction;
			Vector2 visualDir = flipped ? new(-ogVisualDir.x, ogVisualDir.y) : ogVisualDir;

			attack = new()
			{
				From = self,
				Bounds = _attackBox.Hitbox.GetBoundsAsRect(),
				Damage = _attackBox.Damage,
				Direction = dir,
				VisualPosition = _attackBox.VisualPoint.position,
				VisualDirection = visualDir,
				Type = _attackBox.AttackType,
				IsHeavy = _attackBox.IsHeavy,
				IsGrab = _attackBox.IsGrab,
			};
		}

		// yikes
		return new()
		{
			HurtBoxes = hurtBoxRects,
			CollisionBox = _collisionBox.GetBoundsAsRect(),
			Attack = attack,
		};
	}

	public Sprite GetCurrentSprite()
	{
		return _spriteRenderer.sprite;
	}
}
