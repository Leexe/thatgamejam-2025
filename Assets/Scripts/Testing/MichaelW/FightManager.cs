using Sirenix.OdinInspector;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class FightManager : MonoBehaviour
{
	[Header("Fighters")]
	[SerializeField]
	private Fighter _p1;

	[SerializeField]
	private Fighter _p2;

	[Header("References")]
	[SerializeField]
	private CinemachineBrain _cameraBrain;

	[Header("Events")]
	[HideInInspector]
	public UnityEvent<GameResult> OnGameEnd;

	[Header("Parameters")]
	[SerializeField]
	private int _freezeFrameCount = 14;

	[SerializeField]
	[Tooltip("Half the width of the arena")]
	private float _arenaHalfWidth = 4f;

	[SerializeField]
	[Tooltip("How far apart the players can move from each other")]
	private float _maxPlayerDistance = 8.3f;

	[SerializeField]
	[Tooltip("How far the players spawn from x = 0")]
	private float _playerSeparation = 2f;

	[SerializeField]
	[Tooltip("Speed at which overlapping collision boxes are moved apart")]
	private float _maxSeparationSpeed = 0.14f;

	//

	[FoldoutGroup("Event Channel")]
	[Tooltip("Player 1 health channel")]
	[SerializeField]
	[Indent]
	private HealthEventChannelSO _p1Channel;

	[FoldoutGroup("Event Channel")]
	[Tooltip("Player 2 health channel")]
	[SerializeField]
	[Indent]
	private HealthEventChannelSO _p2Channel;

	//
	private float _leftWall;
	private float _rightWall;

	//

	private bool _simulating = false; // whether or not the whole simulation is running
	private bool _fightRunning = false;
	private bool _gameResolved = false; // set to true on win/draw. does not stop simulation
	private int _debugDirCooldown = 10;
	private float _debugP2Dir = 0;
	private float _debugP2VertDir = 0f;
	private Vector2 _debugP2PrevPos = Vector2.zero;

	private int _freezeFrames = 0;
	private int _slowMoRate = 1;
	private int _slowMoCounter = 0;
	private int _rawTickCounter = 0;

	private bool _newFrame = false;

	// player inputs
	private InputInfo _upcomingInput = new();
	private InputInfo _upcomingInput2 = new();

	private void FixedUpdate()
	{
		if (!_simulating)
		{
			return;
		}

		if (_freezeFrames > 0)
		{
			_freezeFrames--;
			return;
		}

		_rawTickCounter++;
		if (_rawTickCounter % _slowMoRate == 0)
		{
			Tick();
			_newFrame = true;

			_slowMoCounter--;
			if (_slowMoCounter == 0)
			{
				SetSlowMoRate(1, 0);
			}
		}
	}

	private void LateUpdate()
	{
		if (_newFrame)
		{
			if (_cameraBrain.UpdateMethod != CinemachineBrain.UpdateMethods.ManualUpdate)
			{
				_cameraBrain.UpdateMethod = CinemachineBrain.UpdateMethods.ManualUpdate;
			}

			// manually update the camera, instead of having it update every FixedUpdate.
			// setting it to update on FixedUpdate stops jitter, but the problem comes back
			// during slowmo as we are only changing position every nth frame.
			_cameraBrain.ManualUpdate();
			_newFrame = false;
		}
	}

	private void Start()
	{
		// Unload the main menu scene
		GameManager.Instance.UnloadSceneAsync(GameManager.SceneNames.MainMenu);
	}

	private void OnEnable()
	{
		InputManager.Instance.OnMovement.AddListener(OnMovement);
		InputManager.Instance.OnMovement2.AddListener(OnMovement2);
		InputManager.Instance.OnPunchPerformed.AddListener(OnPunch);
		InputManager.Instance.OnKickPerformed.AddListener(OnKick);
	}

	private void OnDisable()
	{
		if (InputManager.Instance)
		{
			InputManager.Instance.OnMovement.RemoveListener(OnMovement);
			InputManager.Instance.OnMovement2.RemoveListener(OnMovement2);
			InputManager.Instance.OnPunchPerformed.RemoveListener(OnPunch);
			InputManager.Instance.OnKickPerformed.RemoveListener(OnKick);
		}
	}

	private void OnMovement(Vector2 dir)
	{
		_upcomingInput.Dir = dir;
	}

	private void OnPunch()
	{
		_upcomingInput.PunchButton = true;
	}

	private void OnKick()
	{
		_upcomingInput.KickButton = true;
	}

	private void OnMovement2(Vector2 dir)
	{
		_upcomingInput2.Dir = dir;
	}

	public void SetSlowMoRate(int rate, int duration = 0)
	{
		if (rate >= 1)
		{
			_rawTickCounter = 0;
			_slowMoRate = rate;
			_slowMoCounter = duration;
		}
	}

	[Button]
	public void ResetGame()
	{
		Vector2 p1StartingPos = new(-_playerSeparation, 0f);
		Vector2 p2StartingPos = new(_playerSeparation, 0f);
		_p1.Init(p1StartingPos, p2StartingPos);
		_p2.Init(p2StartingPos, p1StartingPos);

		_p1Channel.RaiseInitiateHealth(_p1.Health, _p1.MaxHealth);
		_p1Channel.RaiseHealthChanged(0f, _p1.Health, _p1.MaxHealth);
		_p2Channel.RaiseInitiateHealth(_p2.Health, _p2.MaxHealth);
		_p2Channel.RaiseHealthChanged(0f, _p2.Health, _p2.MaxHealth);

		SetSlowMoRate(1);

		ComputeArenaBounds();

		// stuff for the enemy AI (should be temp)
		_debugDirCooldown = 10;
		_debugP2Dir = 0;
		_debugP2VertDir = 0f;
		_debugP2PrevPos = Vector2.zero;

		_gameResolved = false;
		_fightRunning = false;
		_simulating = true;
	}

	public void StartFight()
	{
		_fightRunning = true;
	}

	private void Tick()
	{
		// snapshot player positions
		Vector2 p1CurrentPos = _p1.transform.position;
		Vector2 p2CurrentPos = _p2.transform.position;

		InputInfo p1Input = default;
		InputInfo p2Input = default;

		if (_fightRunning)
		{
			// snapshot player inputs and reset
			p1Input = _upcomingInput;
			p2Input = _upcomingInput2;

			if (true)
			{
				// for testing: make player 2 kick periodically
				float horzDist = Mathf.Abs(_p1.transform.position.x - _p2.transform.position.x);

				if (horzDist < 1.6f)
				{
					if (_p2.transform.position.y < 0.01f)
					{
						p2Input.KickButton = Random.value < 0.05f;
						p2Input.PunchButton = Random.value < 0.05f;
					}

					// toggle crouch randomly
					if (Random.value < 0.008f)
					{
						_debugP2VertDir = _debugP2VertDir < 0f ? 0f : -1f;
					}
				}

				// jump randomly
				if (horzDist < 2.6f)
				{
					if (Random.value < 0.006f)
					{
						_debugP2VertDir = 1f;
						if (horzDist > 1.7f)
						{
							_debugP2Dir = Mathf.Sign(_p1.transform.position.x - _p2.transform.position.x);
						}
						else
						{
							_debugP2Dir = 0f;
						}
					}
				}

				// no crouch if too far.
				if (horzDist > 2.0f && _debugP2VertDir < 0f)
				{
					_debugP2VertDir = 0f;
				}

				if (_p2.transform.position.y > 0.01f && _debugP2VertDir > 0f)
				{
					_debugP2VertDir = 0f;
				}

				bool falling = _debugP2PrevPos.y > _p2.transform.position.y;
				if (_p2.transform.position.y - _p1.transform.position.y < 2.0f && falling)
				{
					p2Input.KickButton = true;
				}

				_debugDirCooldown--;
				if (_debugDirCooldown < 1)
				{
					_debugP2Dir = 0;
					float rand = Random.value;
					if (rand < 0.2f + (0.1f * Mathf.Max(0f, _p2.transform.position.x)))
					{
						_debugP2Dir = -1f;
						_debugDirCooldown += Random.Range(10, 25);
					}
					else if (rand > 0.8f + (0.1f * Mathf.Min(0f, _p2.transform.position.x)))
					{
						_debugP2Dir = 1f;
						_debugDirCooldown += Random.Range(10, 25);
					}
					else
					{
						_debugDirCooldown += Random.Range(5, 15);
					}
				}

				p2Input.Dir = new Vector2(_debugP2Dir, _debugP2VertDir);
			}
		}

		_debugP2PrevPos = _p2.transform.position;

		_upcomingInput.KickButton = false;
		_upcomingInput.PunchButton = false;
		_upcomingInput2.KickButton = false;
		_upcomingInput2.PunchButton = false;

		_p1.Tick(p1Input, p2CurrentPos);
		_p2.Tick(p2Input, p1CurrentPos);
		SeparateColliders();
		ProcessHits();

		if (!_gameResolved)
		{
			if (_p1.Health <= 0 || _p2.Health <= 0)
			{
				_freezeFrames = 8;
				SetSlowMoRate(2, 20);
				_gameResolved = true;
				_fightRunning = false;

				if (_p1.Health <= 0 && _p2.Health <= 0) // Draw Round
				{
					AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Draw_Sfx);
					OnGameEnd.Invoke(GameResult.Draw);
				}
				else if (_p1.Health <= 0) // Lose Round
				{
					AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Death_Sfx);
					OnGameEnd.Invoke(GameResult.P2Win);
				}
				else if (_p2.Health <= 0) // Win Round
				{
					AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Win_Sfx);
					OnGameEnd.Invoke(GameResult.P1Win);
				}
			}
		}
	}

	private void ComputeArenaBounds()
	{
		float center = 0.5f * (_p1.transform.position.x + _p2.transform.position.x);
		_leftWall = Mathf.Max(-_arenaHalfWidth, center - (_maxPlayerDistance * 0.5f));
		_rightWall = Mathf.Min(_arenaHalfWidth, center + (_maxPlayerDistance * 0.5f));
	}

	private void SeparateColliders()
	{
		// the "bounds" are the left/right bounds, or a given distance
		// from the center of the two players - whichever is tighter.

		// ensure that fighters are not out of bounds (L/R walls, and floor)
		foreach (Fighter p in new[] { _p1, _p2 })
		{
			HitBoxData hitboxes = p.ReadHitBoxes();

			float leftWallAdjustment = Mathf.Max(0f, -(hitboxes.CollisionBox.xMin - _leftWall));
			float rightWallAdjustment = Mathf.Max(0f, hitboxes.CollisionBox.xMax - _rightWall);
			float floorAdjustment = Mathf.Max(0f, -hitboxes.CollisionBox.yMin);

			p.transform.Translate(
				(leftWallAdjustment * Vector3.right)
					+ (rightWallAdjustment * Vector3.left)
					+ (floorAdjustment * Vector3.up)
			);
		}

		// separate fighters if they collide.
		Rect r1 = _p1.ReadHitBoxes().CollisionBox;
		Rect r2 = _p2.ReadHitBoxes().CollisionBox;
		Rect leftRect;
		Rect rightRect;
		Fighter leftFighter;
		Fighter rightFighter;

		if (r1.Overlaps(r2))
		{
			bool r1_LeftOf_r2 = r1.center.x <= r2.center.x;
			leftRect = r1_LeftOf_r2 ? r1 : r2;
			rightRect = r1_LeftOf_r2 ? r2 : r1;
			leftFighter = r1_LeftOf_r2 ? _p1 : _p2;
			rightFighter = r1_LeftOf_r2 ? _p2 : _p1;

			// determine amount to move left/right fighters so that they don't overlap
			// the separation speed is clamped.
			float separation = Mathf.Min(leftRect.xMax - rightRect.xMin, _maxSeparationSpeed);
			float leftOffset = -separation * 0.5f;
			float rightOffset = separation * 0.5f;

			// ensure that fighters aren't out of bounds, again
			float offsetFromLeft = leftRect.xMin + leftOffset - _leftWall;
			float offsetFromRight = rightRect.xMax + rightOffset - _rightWall;

			if (offsetFromLeft < 0f)
			{
				leftOffset -= offsetFromLeft;
				rightOffset -= offsetFromLeft;
			}
			else if (offsetFromRight > 0f)
			{
				leftOffset -= offsetFromRight;
				rightOffset -= offsetFromRight;
			}

			leftFighter.transform.Translate(leftOffset * Vector3.right);
			rightFighter.transform.Translate(rightOffset * Vector3.right);
		}

		// compute left and right bounds AFTER positions are fully resolved.
		// this basically means that the bounds are based on the previous frame.
		// doing it this way prevents players from "dragging" each other using the bounds.
		ComputeArenaBounds();
	}

	private AttackInfo? DetermineHits(Fighter attacker, Fighter target)
	{
		AttackInfo? attack = attacker.ReadHitBoxes().Attack;

		if (attack.HasValue)
		{
			Rect attackRect = attack.Value.Bounds;

			foreach (Rect hurtBox in target.ReadHitBoxes().HurtBoxes)
			{
				if (hurtBox.width > 0f && hurtBox.height > 0f && attackRect.Overlaps(hurtBox))
				{
					return attack.Value;
				}
			}
		}

		return null;
	}

	private void ProcessHits()
	{
		AttackInfo? attackOnP1 = DetermineHits(_p2, _p1);
		AttackInfo? attackOnP2 = DetermineHits(_p1, _p2);
		AttackResult attackOnP1Result = AttackResult.None;
		AttackResult attackOnP2Result = AttackResult.None;

		if (attackOnP1.HasValue)
		{
			// yikes
			int oldHealth = _p1.Health;
			attackOnP1Result = _p1.OnHitByAttack(attackOnP1.Value);
			_p1Channel.RaiseHealthChanged(_p1.Health - oldHealth, _p1.Health, _p1.MaxHealth);
		}

		if (attackOnP2.HasValue)
		{
			int oldHealth = _p2.Health;
			attackOnP2Result = _p2.OnHitByAttack(attackOnP2.Value);
			_p2Channel.RaiseHealthChanged(_p2.Health - oldHealth, _p2.Health, _p2.MaxHealth);
		}

		if (attackOnP1Result != AttackResult.None)
		{
			_p2.OnAttackLanded(attackOnP1Result);
		}

		if (attackOnP2Result != AttackResult.None)
		{
			_p1.OnAttackLanded(attackOnP2Result);
		}

		if (attackOnP1Result != AttackResult.None || attackOnP2Result != AttackResult.None)
		{
			_freezeFrames += _freezeFrameCount;
		}
	}

	//

	private void OnDrawGizmos()
	{
		// world bounds
		Gizmos.color = Color.pink;
		Gizmos.DrawLine(new Vector3(-_arenaHalfWidth, 0f), new Vector3(-_arenaHalfWidth, 20f));
		Gizmos.DrawLine(new Vector3(_arenaHalfWidth, 0f), new Vector3(_arenaHalfWidth, 20f));
		Gizmos.DrawLine(new Vector3(-_arenaHalfWidth, 0f), new Vector3(_arenaHalfWidth, 0f));
		Gizmos.color = Color.red;
		Gizmos.DrawLine(new Vector3(_leftWall, 0f), new Vector3(_leftWall, 20f));
		Gizmos.DrawLine(new Vector3(_rightWall, 0f), new Vector3(_rightWall, 20f));

		if (!Application.isPlaying || !_simulating)
		{
			return;
		}

		if (_p1 != null)
		{
			_p1.DrawGizmos();
		}

		if (_p2 != null)
		{
			_p2.DrawGizmos();
		}
	}
}
