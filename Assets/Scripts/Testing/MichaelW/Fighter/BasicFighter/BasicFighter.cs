using System.Linq;
using UnityEngine;

public class BasicFighter : Fighter
{
	[Header("References")]
	[SerializeField]
	private BasicFighterSpritesSO _spritesSO;

	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	private State _state;
	private int _stateCounter;
	private BasicFighterAnims _fighterAnims;
	private Sprite _currentSprite;

	private bool _isReadyToBlock;
	private Direction _facingDirection;

	#region events

	public override void Init(Vector2 pos, Vector2 opponentPos)
	{
		_fighterAnims = new BasicFighterAnims(this, _spritesSO);

		Health = MaxHealth;

		Position = pos;
		HitBoxes = new()
		{
			CollisionBox = RectUtils.CollisionBoxRect(0.6f, 1.4f),
			HurtBoxes = new Rect[] { new(-0.4f, 0f, 0.8f, 0.8f), new(-0.3f, 0.8f, 0.6f, 0.8f) },
			Attack = null,
		};

		_facingDirection = opponentPos.x > pos.x ? Direction.Forward : Direction.Backward;

		TransitionState(State.Idle);
	}

	public override void Destroy() { }

	//

	public override void Tick(in InputInfo input, in Vector2 opponentPosition)
	{
		_stateCounter++;

		_isReadyToBlock = false;

		if (_state == State.Idle)
		{
			State_Idle(input, opponentPosition);
		}

		switch (_state)
		{
			case State.MidPunch:
				State_FixedAnim(_fighterAnims.MidPunchAnim);
				break;
			case State.LowPunch:
				State_FixedAnim(_fighterAnims.LowPunchAnim);
				break;
			case State.HitStunned:
				State_HitStunned();
				break;
		}
	}

	public override AttackResult OnHitByAttack(AttackInfo attack)
	{
		TransitionState(State.HitStunned);
		State_HitStunned();

		return AttackResult.Hit;
	}

	public override AttackResult OnHitByGrab(AttackInfo attack)
	{
		// no grabs for now
		return default;
	}

	public override void OnAttackLanded()
	{
		Debug.Log("LANDED!");
		return;
	}

	public override void UpdateVisuals()
	{
		_spriteRenderer.transform.localScale = new Vector3((int)_facingDirection, 1f, 1f);
		_spriteRenderer.transform.position = Position + new Vector2(0.5525f * (int)_facingDirection, 1.0525f);
		_spriteRenderer.sprite = _currentSprite;
	}

	#endregion events

	#region helpers

	private HitBoxData OrientHitboxData(HitBoxData data, Direction dir)
	{
		// this is bad
		if (dir == Direction.Backward)
		{
			HitBoxData flipped = new()
			{
				CollisionBox = RectUtils.MirrorRect(data.CollisionBox),
				HurtBoxes = data.HurtBoxes.Select(RectUtils.MirrorRect).ToArray(), // yikes
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

		return data;
	}

	private void State_FixedAnim(FixedAnimation anim, State endState = State.Idle)
	{
		if (anim.Over(_stateCounter))
		{
			TransitionState(endState);
		}
		else
		{
			OrientableHitbox frame = anim.HitboxAtFrame(_stateCounter);

			HitBoxes = frame.OrientedHitBoxData(_facingDirection);
			_currentSprite = anim.SpriteAtFrame(_stateCounter);
			Position += anim.MovementAtFrame(_stateCounter);
		}
	}

	#endregion helpers

	#region statemachine

	private void TransitionState(State state)
	{
		_state = state;
		_stateCounter = 0;
	}

	private void State_Idle(in InputInfo input, Vector2 opponentPosition)
	{
		// determine facing direction
		_facingDirection = opponentPosition.x > Position.x ? Direction.Forward : Direction.Backward;

		if (input.PunchButton)
		{
			if (input.Dir.y == 0f)
			{
				TransitionState(State.MidPunch);
				return;
			}
		}

		float dx = 0f;
		if (input.Dir.x > 0f)
		{
			dx = 0.06f;
		}
		else if (input.Dir.x < 0f)
		{
			dx = -0.06f;
			_isReadyToBlock = true;
		}
		Position += Vector2.right * dx;

		HitBoxes = OrientHitboxData(
			new()
			{
				CollisionBox = RectUtils.CollisionBoxRect(0.6f, 1.4f),
				HurtBoxes = new Rect[] { new(-0.4f, 0f, 0.8f, 1.0f), new(-0.2f, 0.8f, 0.5f, 0.4f) },
				Attack = null,
			},
			_facingDirection
		);
	}

	private void State_HitStunned()
	{
		HitBoxes = OrientHitboxData(
			new()
			{
				CollisionBox = RectUtils.CollisionBoxRect(0.6f, 1.4f),
				HurtBoxes = new Rect[] { },
				Attack = null,
			},
			_facingDirection
		);

		Position += 0.04f * (int)_facingDirection * Vector2.left;

		if (_stateCounter >= 10)
		{
			TransitionState(State.Idle);
		}
	}

	#endregion statemachine

	#region animations

	private class BasicFighterAnims
	{
		public readonly FixedAnimation MidPunchAnim;
		public readonly FixedAnimation LowPunchAnim;
		public readonly FixedAnimation MidKickAnim;
		public readonly FixedAnimation LowKickAnim;

		public BasicFighterAnims(Fighter fighter, BasicFighterSpritesSO sprites) // todo: accept spritemap as input
		{
			MidPunchAnim = FixedAnimation.FromParams(
				FixedAnimation.AnimationPart.FromFlattenedData(
					collisionBox: RectUtils.CollisionBoxRect(0.6f, 1.4f),
					hurtBoxes: new Rect[] { new(-0.4f, 0f, 0.8f, 0.8f), new(-0.3f, 0.8f, 0.6f, 0.8f) },
					attack: null,
					sprite: sprites.StandPunch[0],
					duration: 5,
					constantMovement: 0.03f * Vector2.right
				),
				FixedAnimation.AnimationPart.FromFlattenedData(
					collisionBox: RectUtils.CollisionBoxRect(0.6f, 1.4f),
					hurtBoxes: new Rect[] { new(-0.4f, 0f, 0.8f, 0.8f), new(-0.3f, 0.8f, 0.6f, 0.8f) },
					attack: new()
					{
						From = fighter,
						Bounds = new(0.5f, 0f, 2f, 0.2f),
						Direction = Direction.Forward,
						Type = AttackType.Mid,
						Damage = 100,
					},
					sprite: sprites.StandPunch[0],
					duration: 5,
					constantMovement: Vector2.zero
				),
				FixedAnimation.AnimationPart.FromFlattenedData(
					collisionBox: RectUtils.CollisionBoxRect(0.6f, 1.4f),
					hurtBoxes: new Rect[] { new(-0.4f, 0f, 0.8f, 0.8f), new(-0.3f, 0.8f, 0.6f, 0.8f) },
					attack: null,
					sprite: sprites.StandPunch[1],
					duration: 6,
					constantMovement: Vector2.zero
				)
			);
		}
	}

	#endregion animations

	#region types

	private enum State
	{
		Idle,

		LowPunch,
		MidPunch,
		HighPunch,
		LowKick,
		MidKick,
		HighKick,
		SpecialAttack, // ???

		HitStunned,
		HeavyHitStunned,
		HighBlock,
		LowBlock,

		Grab,
		Grabbed,
	}

	#endregion types
}
