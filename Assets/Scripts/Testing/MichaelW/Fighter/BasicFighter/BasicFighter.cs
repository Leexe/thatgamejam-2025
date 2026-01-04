using UnityEngine;

public class BasicFighter : Fighter
{
	[Header("References")]
	[SerializeField]
	private AnimationDataController _animDataController;

	[SerializeField]
	private BasicFighterAnimsSO _animsSO;

	[Header("Parameters")]
	[SerializeField]
	private float _moveSpeed = 0.06f;

	[SerializeField]
	private float _hitKnockback = 0.1f;

	[SerializeField]
	private float _knockbackGroundDecel = 0.008f;

	[SerializeField]
	private float _gravity = 0.008f;

	[SerializeField]
	private float _jumpVel = 0.16f;

	[SerializeField]
	private float _jumpHorizontalVel = 0.07f;

	//

	public override HitBoxData ReadHitBoxes()
	{
		return _animDataController.GetAbsoluteHitBoxData(this);
	}

	//

	private State _state;
	private State _prevIdleState;
	private int _stateCounter;

	// idle state vars
	private Direction _walkDirection = Direction.None;
	private Direction _prevWalkDirection = Direction.None;
	private bool _wantsToCrouch = false;
	private bool _wantsToJump = false;
	private int _walkFrame = 0;

	// block/damage state vars
	private Vector2 _vel;

	private bool _isReadyToBlock;
	private Direction _facingDirection;

	#region events

	public override void Init(Vector2 pos, Vector2 opponentPos)
	{
		Health = MaxHealth;
		transform.position = pos;
		_facingDirection = opponentPos.x > pos.x ? Direction.Forward : Direction.Backward;

		TransitionState(State.Idle);
	}

	//

	public override void Tick(in InputInfo input, in Vector2 opponentPosition)
	{
		_stateCounter++;
		_isReadyToBlock = false;

		SetWalkDirectionAndCrouch(input);

		switch (_state)
		{
			case State.Idle:
				State_Idle(input, opponentPosition);
				break;
			case State.MidPunch:
				State_Anim(_animsSO.StandPunch, State.Idle);
				break;
			case State.MidKick:
				State_Anim(_animsSO.StandKick, State.Idle);
				break;
			case State.HitStunned:
				State_Knockback(_animsSO.StandHurt, State.Idle);
				break;
			case State.HighBlock:
				State_Knockback(_animsSO.StandBlock, State.Idle);
				break;
			case State.CrouchIdle:
				State_Crouch(input, opponentPosition);
				break;
			case State.LowPunch:
				State_Anim(_animsSO.CrouchPunch, State.CrouchIdle);
				break;
			case State.LowKick:
				State_Anim(_animsSO.CrouchKick, State.CrouchIdle);
				break;
			case State.LowStunned:
				State_Knockback(_animsSO.CrouchHurt, State.CrouchIdle);
				break;
			case State.LowBlock:
				State_Knockback(_animsSO.CrouchBlock, State.CrouchIdle);
				break;
			case State.JumpIdle:
				State_AirborneIdle(input, opponentPosition);
				break;
			case State.HighKick:
				State_Airborne(_animsSO.JumpKick, State.JumpIdle);
				break;
			case State.HighPunch:
				State_Airborne(_animsSO.JumpPunch, State.JumpIdle);
				break;
			case State.HighStunned:
				State_Airborne(_animsSO.JumpHurt, State.JumpIdle);
				break;
		}
	}

	public override AttackResult OnHitByAttack(AttackInfo attack)
	{
		switch (_state)
		{
			case State.Idle:
			case State.MidPunch:
			case State.MidKick:
				return OnHit_Standing(attack);
			case State.CrouchIdle:
			case State.LowPunch:
			case State.LowKick:
				return OnHit_Crouch(attack);
			case State.JumpIdle:
			case State.HighPunch:
			case State.HighKick:
				return OnHit_Airborne(attack);
		}

		Debug.LogError("OnHitByAttack() not handled properly");
		return AttackResult.None;
	}

	public override void OnAttackLanded(AttackResult result)
	{
		if (result == AttackResult.Hit)
		{
			Debug.Log("LANDED!");
		}
	}

	#endregion events

	#region helpers

	private void DoFrame(AnimationClip clip, int frame)
	{
		_animDataController.SetAnimFrame(clip, frame);
	}

	private void FaceTowardsTarget(Vector2 targetPos)
	{
		// determine facing direction
		_facingDirection = targetPos.x > transform.position.x ? Direction.Forward : Direction.Backward;
		transform.localScale = new(_facingDirection == Direction.Forward ? 1f : -1f, 1f, 1f);
	}

	private void SetWalkDirectionAndCrouch(in InputInfo input)
	{
		_prevWalkDirection = _walkDirection;

		if (input.Dir.x > 0f)
		{
			_walkDirection = Direction.Forward;
		}
		else if (input.Dir.x < 0f)
		{
			_walkDirection = Direction.Backward;
		}
		else
		{
			_walkDirection = Direction.None;
		}

		_wantsToCrouch = input.Dir.y < 0f;
		_wantsToJump = input.Dir.y > 0f;
	}

	private void SetBlockReadiness()
	{
		_isReadyToBlock = _walkDirection != Direction.None && _facingDirection != _walkDirection;
	}

	private bool HandleAirbornePhysics()
	{
		transform.Translate(_vel);
		_vel += _gravity * Vector2.down;

		if (transform.position.y <= 0f)
		{
			transform.Translate(-transform.position.y * Vector2.up);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Helper to play a transition animation
	/// </summary>
	/// <returns>The <c>_stateCounter</c> variable offset by the transition duration.</returns>
	private int PlayTransitionAnimation(AnimationClip animation, bool reverse = false)
	{
		int transitionFrameCount = Mathf.FloorToInt((animation.length * 60f) - 0.5f);

		if (_stateCounter < transitionFrameCount)
		{
			DoFrame(animation, reverse ? -1 - _stateCounter : _stateCounter);
		}

		return _stateCounter - transitionFrameCount;
	}

	#endregion helpers

	#region statemachine

	private void TransitionState(State state)
	{
		if (_state is State.Idle or State.CrouchIdle or State.JumpIdle)
		{
			_prevIdleState = _state;
		}

		_state = state;
		_stateCounter = 0;
	}

	/// <summary>
	/// Generic state handler for animations.
	/// The state handler processes the animation and does nothing else.
	/// </summary>
	private void State_Anim(AnimationClip clip, State endState = State.Idle, bool reverse = false)
	{
		if ((_stateCounter + 1) > (clip.length * 60f) - 0.001f)
		{
			TransitionState(endState);
		}
		else
		{
			DoFrame(clip, reverse ? -1 - _stateCounter : _stateCounter);
			// todo: position?
		}
	}

	/// <summary>
	/// Generic state handler for animations with knockback.
	/// </summary>
	private void State_Knockback(AnimationClip clip, State endState = State.Idle)
	{
		transform.Translate(_vel);
		_vel = Vector2.MoveTowards(_vel, Vector2.zero, _knockbackGroundDecel);
		State_Anim(clip, endState);
	}

	/// <summary>
	/// Generic state handler for animations performed in the air
	/// </summary>
	private void State_Airborne(AnimationClip clip, State endState = State.Idle)
	{
		if (HandleAirbornePhysics())
		{
			TransitionState(endState);
		}
		else
		{
			State_Anim(clip, endState);
		}
	}

	/// <summary>
	/// State handler for the standing idle state.
	/// </summary>
	private void State_Idle(in InputInfo input, Vector2 opponentPos)
	{
		FaceTowardsTarget(opponentPos);

		// handle transition from standing position
		int frameCounter;
		bool doneTransitioning = true;

		if (_prevIdleState == State.CrouchIdle)
		{
			frameCounter = PlayTransitionAnimation(_animsSO.CrouchTransition, true);
			doneTransitioning = frameCounter >= 0;
		}
		else if (_prevIdleState == State.JumpIdle)
		{
			frameCounter = PlayTransitionAnimation(_animsSO.Jump, true);
			doneTransitioning = frameCounter >= 0;
		}

		if (input.PunchButton)
		{
			TransitionState(State.MidPunch);
			State_Anim(_animsSO.StandPunch);
			return;
		}
		if (input.KickButton)
		{
			TransitionState(State.MidKick);
			State_Anim(_animsSO.StandKick);
			return;
		}
		if (_wantsToCrouch)
		{
			TransitionState(State.CrouchIdle);
			State_Anim(_animsSO.CrouchTransition);
			return;
		}
		if (_wantsToJump)
		{
			// set jump velocity
			_vel = (_jumpVel * Vector2.up) + (_jumpHorizontalVel * (int)_walkDirection * Vector2.right);

			TransitionState(State.JumpIdle);
			State_Anim(_animsSO.Jump);
			return;
		}

		if (doneTransitioning)
		{
			// move fighter
			transform.position += (int)_walkDirection * _moveSpeed * Vector3.right;

			// you cannot block while transitioning to crouch
			SetBlockReadiness();

			// determine animation
			if (_walkDirection != _prevWalkDirection)
			{
				_walkFrame = 0;
			}
			if (_walkDirection == Direction.None)
			{
				DoFrame(_animsSO.StandIdle, _walkFrame);
			}
			else if (_walkDirection == _facingDirection)
			{
				DoFrame(_animsSO.Walk, _walkFrame);
			}
			else
			{
				DoFrame(_animsSO.Walk, -_walkFrame);
			}

			_walkFrame++;
		}
	}

	/// <summary>
	/// State handler for the crouch idle state.
	/// </summary>
	private void State_Crouch(in InputInfo input, Vector2 opponentPos)
	{
		FaceTowardsTarget(opponentPos);

		// handle transition from standing position
		int frameCounter = _stateCounter;
		bool doneTransitioning = true;

		if (_prevIdleState == State.Idle)
		{
			frameCounter = PlayTransitionAnimation(_animsSO.CrouchTransition, false);
			doneTransitioning = frameCounter >= 0;
		}

		// possible inputs
		// note that punches / kicks can be input immediately upon crouching
		if (input.PunchButton)
		{
			TransitionState(State.LowPunch);
			State_Anim(_animsSO.CrouchPunch);
			return;
		}
		if (input.KickButton)
		{
			TransitionState(State.LowKick);
			State_Anim(_animsSO.CrouchKick);
			return;
		}
		if (!_wantsToCrouch)
		{
			TransitionState(State.Idle);
			State_Anim(_animsSO.CrouchTransition, State.Idle, true);
			return;
		}

		if (doneTransitioning)
		{
			// you cannot block while transitioning to crouch
			SetBlockReadiness();

			// play idle animation
			DoFrame(_animsSO.CrouchIdle, frameCounter);
		}
	}

	/// <summary>
	/// State handler for the airborne idle state.
	/// </summary>
	private void State_AirborneIdle(in InputInfo input, Vector2 opponentPos)
	{
		if (HandleAirbornePhysics())
		{
			TransitionState(State.Idle);
			State_Anim(_animsSO.Jump, State.Idle, true);
			return;
		}

		// handle transition from standing position
		int frameCounter = _stateCounter;
		bool doneTransitioning = true;

		if (_prevIdleState == State.Idle)
		{
			frameCounter = PlayTransitionAnimation(_animsSO.Jump, false);
			doneTransitioning = frameCounter >= 0;
		}

		if (input.PunchButton)
		{
			TransitionState(State.HighPunch);
			State_Anim(_animsSO.JumpPunch);
			return;
		}
		if (input.KickButton)
		{
			TransitionState(State.HighKick);
			State_Anim(_animsSO.JumpKick);
			return;
		}

		if (doneTransitioning)
		{
			// visual is just the last frame of the jump animation.
			DoFrame(_animsSO.Jump, -1);
		}
	}

	private AttackResult OnHit_Standing(AttackInfo attack)
	{
		_vel = _hitKnockback * (int)attack.Direction * Vector3.right;

		if (_isReadyToBlock && (attack.Type == AttackType.High || attack.Type == AttackType.Mid))
		{
			TransitionState(State.HighBlock);
			State_Anim(_animsSO.StandBlock);
			return AttackResult.Blocked;
		}
		else
		{
			TransitionState(State.HitStunned);
			State_Anim(_animsSO.StandHurt);
			return AttackResult.Hit;
		}
	}

	private AttackResult OnHit_Crouch(AttackInfo attack)
	{
		if (_isReadyToBlock && (attack.Type == AttackType.Low || attack.Type == AttackType.Mid))
		{
			TransitionState(State.LowBlock);
			State_Anim(_animsSO.CrouchBlock);
			return AttackResult.Blocked;
		}
		else
		{
			TransitionState(State.LowStunned);
			State_Anim(_animsSO.CrouchHurt);
			return AttackResult.Hit;
		}
	}

	private AttackResult OnHit_Airborne(AttackInfo attack)
	{
		TransitionState(State.HighStunned);
		State_Anim(_animsSO.JumpHurt);
		return AttackResult.Hit;
	}

	#endregion statemachine

	#region types

	private enum State
	{
		Idle,
		CrouchIdle,

		LowPunch,
		MidPunch,
		HighPunch,
		LowKick,
		MidKick,
		HighKick,
		SpecialAttack, // ???

		HitStunned,
		HeavyHitStunned,
		LowStunned,
		HighBlock,
		LowBlock,
		HighStunned,

		Grab,
		Grabbed,

		JumpIdle,
	}

	#endregion types
}
