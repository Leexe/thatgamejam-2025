using UnityEngine;

public class BasicFighter : Fighter
{
	[Header("References")]
	[SerializeField]
	private AnimationDataController _animDataController;

	[SerializeField]
	private BasicFighterAnimsSO _animsSO;

	[SerializeField]
	private VFX _vfxPrefab;

	[SerializeField]
	private AfterImage _afterImagePrefab;

	[SerializeField]
	private BasicFighterVFXsSO _vfxList;

	[Header("Parameters")]
	[SerializeField]
	private int _inputBufferDuration = 12; // 12 -> 0.2s

	[SerializeField]
	private int _doubleTapDuration = 16;

	[SerializeField]
	private float _moveSpeed = 0.045f;

	[SerializeField]
	private float _hitKnockback = 0.1f;

	[SerializeField]
	private float _knockbackGroundDecel = 0.008f;

	[SerializeField]
	private float _gravity = 0.008f;

	[SerializeField]
	private float _jumpVel = 0.16f;

	[SerializeField]
	private float _jumpHorizontalVel = 0.045f;

	[SerializeField]
	private float _dashSpeed = 0.16f;

	//

	public override HitBoxData ReadHitBoxes()
	{
		return _animDataController.GetAbsoluteHitBoxData(this);
	}

	// input buffering / dash vars
	private InputInfo _prevInput = default;
	private int _tickCounter = 0;
	private Direction _lastDirInput = Direction.None;
	private int _lastDirInputTime = -1000;
	private int _lastPunchInput = -1000;
	private int _lastKickInput = -1000;
	private int _lastJumpInput = -1000;
	private int _lastGrabInput = -1000;

	//

	private State _state;
	private State _prevIdleState;
	private int _stateCounter;

	public State GetState()
	{
		return _state;
	}

	// idle state vars
	private Direction _walkDirection = Direction.None;
	private Direction _prevWalkDirection = Direction.None;
	private bool _wantsPunch = false;
	private bool _wantsKick = false;
	private bool _wantsGrab = false;
	private bool _wantsToCrouch = false;
	private bool _wantsToJump = false;
	private bool _wantsDash = false;
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

		_prevInput = default;
		_lastDirInput = Direction.None;
		_lastDirInputTime = -1000;
		_lastPunchInput = -1000;
		_lastKickInput = -1000;
		_lastGrabInput = -1000;
		_lastJumpInput = -1000;
		_tickCounter = 0;

		// reset all the state-related vars, just in case...
		_walkDirection = Direction.None;
		_prevWalkDirection = Direction.None;
		_wantsToCrouch = false;
		_wantsToJump = false;
		_wantsKick = false;
		_wantsPunch = false;
		_wantsGrab = false;
		_walkFrame = 0;
		_vel = Vector2.zero;
		_isReadyToBlock = false;

		TransitionState(State.StandIdle);
		State_Anim(_animsSO.StandIdle);
	}

	//

	public override void Tick(in InputInfo input, in Vector2 opponentPosition)
	{
		_stateCounter++;
		_tickCounter++;
		_isReadyToBlock = false;

		SetWalkDirectionAndCrouch(input);
		_prevInput = input;

		switch (_state)
		{
			case State.StandIdle:
				State_Idle(input, opponentPosition);
				break;
			case State.StandPunch:
				State_Anim(_animsSO.StandPunch, State.StandIdle);
				break;
			case State.StandKick:
				State_Anim(_animsSO.StandKick, State.StandIdle);
				break;
			case State.StandHurt:
				State_Knockback(_animsSO.StandHurt, State.StandIdle);
				break;
			case State.StandBlock:
				State_Knockback(_animsSO.StandBlock, State.StandIdle);
				break;
			case State.CrouchIdle:
				State_Crouch(input, opponentPosition);
				break;
			case State.CrouchPunch:
				State_Anim(_animsSO.CrouchPunch, State.CrouchIdle);
				break;
			case State.CrouchKick:
				State_Anim(_animsSO.CrouchKick, State.CrouchIdle);
				break;
			case State.CrouchHurt:
				State_Knockback(_animsSO.CrouchHurt, State.CrouchIdle);
				break;
			case State.CrouchBlock:
				State_Knockback(_animsSO.CrouchBlock, State.CrouchIdle);
				break;
			case State.JumpIdle:
				State_AirborneIdle(input, opponentPosition);
				break;
			case State.JumpKick:
				State_Airborne(_animsSO.JumpKick, State.JumpIdle);
				break;
			case State.JumpPunch:
				State_Airborne(_animsSO.JumpPunch, State.JumpIdle);
				break;
			case State.JumpHurt:
				State_Airborne(_animsSO.JumpHurt, State.JumpIdle);
				break;
			case State.Dead:
				State_Dead();
				break;
			case State.DashForward:
				State_Dash(_animsSO.DashForward, State.StandIdle);
				break;
			case State.DashBack:
				State_Dash(_animsSO.DashBack, State.StandIdle);
				break;
			case State.Grab:
				State_Knockback(_animsSO.Grab, State.StandIdle);
				break;
			case State.Grabbed:
				State_Knockback(_animsSO.Grabbed, State.StandIdle);
				break;
			case State.GrabSuccess:
				State_Anim(_animsSO.GrabSuccess, State.StandIdle);
				break;
			case State.Knocked:
				State_Knocked();
				break;
		}
	}

	public override AttackResult OnHitByAttack(AttackInfo attack)
	{
		if (!attack.IsGrab)
		{
			AttackResult res = AttackResult.None;

			switch (_state)
			{
				case State.StandIdle:
				case State.StandPunch:
				case State.StandKick:
				case State.DashForward:
				case State.DashBack:
					res = OnHit_Standing(attack);
					break;
				case State.Grab:
				{
					HitBoxData data = ReadHitBoxes();
					if (data.Attack.HasValue)
					{
						res = AttackResult.None;
						break;
					}
					else
					{
						res = OnHit_Standing(attack);
						break;
					}
				}

				case State.CrouchIdle:
				case State.CrouchPunch:
				case State.CrouchKick:
					res = OnHit_Crouch(attack);
					break;
				case State.JumpIdle:
				case State.JumpPunch:
				case State.JumpKick:
					res = OnHit_Airborne(attack);
					break;
				case State.Grabbed:
					res = OnHit_Grabbed(attack);
					break;
				default:
					Debug.Log("OnHitByAttack() not handled properly");
					break;
			}

			if (res == AttackResult.Blocked)
			{
				AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Block_Sfx);
				PlayVFX(_vfxList.BlockVFX, attack.VisualPosition, attack.VisualDirection);
			}
			else if (res == AttackResult.Hit)
			{
				bool isKick = false;
				if (attack.From is BasicFighter attacker)
				{
					isKick = attacker._state is State.StandKick or State.CrouchKick or State.JumpKick;
				}

				AudioManager.Instance.PlayOneShot(
					isKick ? FMODEvents.Instance.KickLand_Sfx : FMODEvents.Instance.PunchLand_Sfx
				);

				Health -= attack.Damage;
				if (Health <= 0)
				{
					Health = 0;
					_vel = (_hitKnockback * (int)attack.Direction * Vector3.right) + (0.08f * Vector3.up);
					AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Death_Sfx);
					TransitionState(State.Dead);
				}

				AnimationClip clip = attack.IsHeavy ? _vfxList.KickVFX : _vfxList.PunchVFX;
				PlayVFX(clip, attack.VisualPosition, attack.VisualDirection);
			}

			return res;
		}
		else
		{
			AttackResult res = AttackResult.None;

			switch (_state)
			{
				case State.StandIdle:
				case State.StandPunch:
				case State.StandKick:
				case State.DashForward:
				case State.DashBack:
				case State.CrouchIdle:
				case State.CrouchPunch:
				case State.CrouchKick:
					res = OnGrab_Standing(attack);
					break;
				case State.Grab:
				case State.GrabSuccess:
					// if you are grabbed while grabbing, trade
					res = OnGrabTrade_Standing(attack);
					break;

				case State.StandHurt:
				case State.StandBlock:
				case State.CrouchHurt:
				case State.CrouchBlock:
				case State.JumpIdle:
				case State.JumpPunch:
				case State.JumpKick:
					res = AttackResult.None;
					break;
				default:
					Debug.Log("OnHitByAttack() grab not handled properly");
					break;
			}

			return res;
		}
	}

	public override void OnAttackLanded(AttackResult result)
	{
		// should never be null
		AttackInfo ourAttack = ReadHitBoxes().Attack.Value;

		if (ourAttack.IsGrab)
		{
			if (result == AttackResult.Hit)
			{
				AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Grab_Sfx);
				TransitionState(State.GrabSuccess);
				State_Anim(_animsSO.GrabSuccess);
			}
			else if (result == AttackResult.Blocked)
			{
				_vel = -_hitKnockback * (int)ourAttack.Direction * Vector3.right;
			}
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
		Vector3 scale = transform.localScale;
		float absXScale = Mathf.Abs(scale.x);
		scale.x = _facingDirection == Direction.Forward ? absXScale : -absXScale;
		transform.localScale = scale;
	}

	private Direction GetHDir(Vector2 dir)
	{
		if (dir.x > 0f)
		{
			return Direction.Forward;
		}
		else if (dir.x < 0f)
		{
			return Direction.Backward;
		}
		else
		{
			return Direction.None;
		}
	}

	private void SetWalkDirectionAndCrouch(in InputInfo input)
	{
		_prevWalkDirection = _walkDirection;
		_walkDirection = GetHDir(input.Dir);

		// look through input history to see if dash (double tap)
		_wantsDash = false;
		if (_walkDirection != Direction.None && _prevWalkDirection == Direction.None)
		{
			int gap = _tickCounter - _lastDirInputTime;
			if (gap < _doubleTapDuration && _walkDirection == _lastDirInput)
			{
				_wantsDash = true;
				_lastDirInputTime = -5000;
			}
			else
			{
				_lastDirInput = _walkDirection;
				_lastDirInputTime = _tickCounter;
			}
		}

		// jumping
		if (input.Dir.y > 0f && _prevInput.Dir.y <= 0f)
		{
			_lastJumpInput = _tickCounter;
		}
		_wantsToJump = _tickCounter - _lastJumpInput < _inputBufferDuration;

		// crouching
		_wantsToCrouch = input.Dir.y < 0f;

		// punching, kicking
		if (input.PunchButton)
		{
			_lastPunchInput = _tickCounter;
		}
		_wantsPunch = _tickCounter - _lastPunchInput < _inputBufferDuration;

		if (input.KickButton)
		{
			_lastKickInput = _tickCounter;
		}
		_wantsKick = _tickCounter - _lastKickInput < _inputBufferDuration;

		if (input.GrabButton)
		{
			_lastGrabInput = _tickCounter;
		}
		_wantsGrab = _tickCounter - _lastGrabInput < _inputBufferDuration;
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

	private void PlayVFX(AnimationClip animation, Vector2 position, Vector2 facingDirection)
	{
		VFX vfxObject = Instantiate(_vfxPrefab);
		var rot = Quaternion.Euler(0f, 0f, Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg);
		vfxObject.transform.SetPositionAndRotation(position, rot);
		vfxObject.Play(animation);
	}

	private void SpawnAfterImage()
	{
		AfterImage afterImage = Instantiate(_afterImagePrefab);
		afterImage.Show(transform, _animDataController.GetCurrentSpriteRenderer(), 0.1f);
	}

	#endregion helpers

	#region statemachine

	private void TransitionState(State state)
	{
		if (_state is State.StandIdle or State.CrouchIdle or State.JumpIdle)
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
	private void State_Anim(AnimationClip clip, State endState = State.StandIdle, bool reverse = false)
	{
		if (_stateCounter + 1 > (clip.length * 60f) - 0.001f)
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
	private void State_Knockback(AnimationClip clip, State endState = State.StandIdle)
	{
		transform.Translate(_vel);
		_vel = Vector2.MoveTowards(_vel, Vector2.zero, _knockbackGroundDecel);
		State_Anim(clip, endState);
	}

	/// <summary>
	/// Generic state handler for animations performed in the air
	/// </summary>
	private void State_Airborne(AnimationClip clip, State endState = State.StandIdle)
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

	private void State_Dash(AnimationClip clip, State endState = State.StandIdle)
	{
		if (_stateCounter % 2 == 0)
		{
			SpawnAfterImage();
		}

		transform.Translate(_vel);
		State_Anim(clip, endState);
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

		if (_wantsPunch)
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Punch_Sfx);
			_lastPunchInput = -1000;
			TransitionState(State.StandPunch);
			State_Anim(_animsSO.StandPunch);
			return;
		}
		if (_wantsGrab)
		{
			_lastGrabInput = -1000;
			TransitionState(State.Grab);
			State_Anim(_animsSO.Grab);
			return;
		}
		if (_wantsKick)
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Kick_Sfx);
			_lastPunchInput = -1000;
			TransitionState(State.StandKick);
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
			_lastJumpInput = -1000;

			// set jump velocity
			_vel = (_jumpVel * Vector2.up) + (_jumpHorizontalVel * (int)_walkDirection * Vector2.right);

			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Jump_Sfx);
			TransitionState(State.JumpIdle);
			State_Anim(_animsSO.Jump);
			return;
		}
		if (_wantsDash)
		{
			if (_walkDirection != Direction.None)
			{
				_vel = _dashSpeed * (int)_walkDirection * Vector2.right;
				bool forward = _walkDirection == _facingDirection;
				AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Dash_Sfx);
				TransitionState(forward ? State.DashForward : State.DashBack);
				State_Anim(forward ? _animsSO.DashForward : _animsSO.DashBack);
			}
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

		if (_prevIdleState == State.StandIdle)
		{
			frameCounter = PlayTransitionAnimation(_animsSO.CrouchTransition, false);
			doneTransitioning = frameCounter >= 0;
		}

		// possible inputs
		// note that punches / kicks can be input immediately upon crouching
		if (_wantsPunch)
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Punch_Sfx);
			_lastPunchInput = -1000;
			TransitionState(State.CrouchPunch);
			State_Anim(_animsSO.CrouchPunch);
			return;
		}
		if (_wantsKick)
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Kick_Sfx);
			_lastKickInput = -1000;
			TransitionState(State.CrouchKick);
			State_Anim(_animsSO.CrouchKick);
			return;
		}

		if (!_wantsToCrouch)
		{
			TransitionState(State.StandIdle);
			State_Anim(_animsSO.CrouchTransition, State.StandIdle, true);
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
			TransitionState(State.StandIdle);
			State_Anim(_animsSO.Jump, State.StandIdle, true);
			return;
		}

		// handle transition from standing position
		int frameCounter = _stateCounter;
		bool doneTransitioning = true;

		if (_prevIdleState == State.StandIdle)
		{
			frameCounter = PlayTransitionAnimation(_animsSO.Jump, false);
			doneTransitioning = frameCounter >= 0;
		}

		if (input.PunchButton)
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Punch_Sfx);
			TransitionState(State.JumpPunch);
			State_Anim(_animsSO.JumpPunch);
			return;
		}

		if (input.KickButton)
		{
			AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Kick_Sfx);
			TransitionState(State.JumpKick);
			State_Anim(_animsSO.JumpKick);
			return;
		}

		if (doneTransitioning)
		{
			// visual is just the last frame of the jump animation.
			DoFrame(_animsSO.Jump, -1);
		}
	}

	private void State_Knocked()
	{
		if (transform.position.y > 0f)
		{
			HandleAirbornePhysics();
		}
		else
		{
			// slide for a bit when we hit the ground (LOL)
			transform.Translate(_vel);
			_vel = Vector2.MoveTowards(_vel, Vector2.zero, _knockbackGroundDecel);
		}

		State_Anim(_animsSO.KnockDownGetUp, State.StandIdle);
	}

	private void State_Dead()
	{
		if (transform.position.y > 0f)
		{
			HandleAirbornePhysics();
		}
		else
		{
			// slide for a bit when we hit the ground (LOL)
			transform.Translate(_vel);
			_vel = Vector2.MoveTowards(_vel, Vector2.zero, _knockbackGroundDecel);
		}

		int animLength = Mathf.FloorToInt((_animsSO.Die.length * 60f) - 0.5f);
		int frame = Mathf.Min(_stateCounter, animLength - 1);

		DoFrame(_animsSO.Die, frame);
	}

	private AttackResult OnHit_Standing(AttackInfo attack)
	{
		_vel = _hitKnockback * (int)attack.Direction * Vector3.right;

		if (_isReadyToBlock && (attack.Type == AttackType.High || attack.Type == AttackType.Mid))
		{
			TransitionState(State.StandBlock);
			State_Anim(_animsSO.StandBlock);
			return AttackResult.Blocked;
		}
		else
		{
			TransitionState(State.StandHurt);
			State_Anim(_animsSO.StandHurt);
			return AttackResult.Hit;
		}
	}

	private AttackResult OnHit_Crouch(AttackInfo attack)
	{
		_vel = _hitKnockback * (int)attack.Direction * Vector3.right;

		if (_isReadyToBlock && (attack.Type == AttackType.Low || attack.Type == AttackType.Mid))
		{
			TransitionState(State.CrouchBlock);
			State_Anim(_animsSO.CrouchBlock);
			return AttackResult.Blocked;
		}
		else
		{
			TransitionState(State.CrouchHurt);
			State_Anim(_animsSO.CrouchHurt);
			return AttackResult.Hit;
		}
	}

	private AttackResult OnHit_Airborne(AttackInfo attack)
	{
		_vel.x = _jumpHorizontalVel * (int)attack.Direction;
		if (_vel.y > 0f)
		{
			_vel.y *= 0.7f;
		}

		TransitionState(State.JumpHurt);
		State_Anim(_animsSO.JumpHurt);
		return AttackResult.Hit;
	}

	private AttackResult OnHit_Grabbed(AttackInfo attack)
	{
		_vel = (_hitKnockback * 0.7f * (int)attack.Direction * Vector3.right) + (0.08f * Vector3.up);
		TransitionState(State.Knocked);
		State_Anim(_animsSO.KnockDownGetUp);
		return AttackResult.Hit;
	}

	private AttackResult OnGrab_Standing(AttackInfo attack)
	{
		// negative knockback, to suck the player in
		_vel = -_hitKnockback * 0.4f * (int)attack.Direction * Vector3.right;
		TransitionState(State.Grabbed);
		State_Anim(_animsSO.Grabbed); // temp
		return AttackResult.Hit;
	}

	private AttackResult OnGrabTrade_Standing(AttackInfo attack)
	{
		// bit of a hack: check our attack hitbox data to see if our action frame was this frame too.
		// assume that if we have an attackbox, then it landed (that's probably almost always true)
		HitBoxData data = ReadHitBoxes();
		if (data.Attack.HasValue)
		{
			_vel = _hitKnockback * (int)attack.Direction * Vector3.right;
			// no transition
			return AttackResult.Blocked;
		}
		else
		{
			return OnGrab_Standing(attack);
		}
	}

	#endregion statemachine

	#region types

	public enum State
	{
		StandIdle,
		StandPunch,
		StandKick,
		StandBlock,
		StandHurt,

		DashForward,
		DashBack,

		CrouchIdle,
		CrouchPunch,
		CrouchKick,
		CrouchBlock,
		CrouchHurt,

		JumpIdle,
		JumpPunch,
		JumpKick,
		JumpHurt,

		Grab,
		Grabbed,
		GrabSuccess,
		Knocked,

		Dead,
	}

	#endregion types
}
