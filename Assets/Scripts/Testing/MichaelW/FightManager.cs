//
//
//

using UnityEngine;

public class FightManager : MonoBehaviour
{
	[SerializeField]
	private Fighter _p1;

	[SerializeField]
	private Fighter _p2;

	private bool _gameStarted = false;
	private readonly float _arenaHalfWidth = 5f;
	private readonly float _maxSeparationSpeed = 0.14f;

	// player inputs
	private InputInfo _upcomingInput = new();
	private InputInfo _upcomingInput2 = new();

	private void FixedUpdate()
	{
		// for now, tick at same interval as fixedupdate
		Tick();
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

	private void Tick()
	{
		if (!_gameStarted)
		{
			Vector2 p1StartingPos = new(-4f, 0f);
			Vector2 p2StartingPos = new(4f, 0f);
			_p1.Init(p1StartingPos, p2StartingPos);
			_p2.Init(p2StartingPos, p1StartingPos);
			_gameStarted = true;
		}

		// snapshot player positions
		Vector2 p1CurrentPos = _p1.Position;
		Vector2 p2CurrentPos = _p2.Position;

		// snapshot player inputs and reset
		InputInfo p1Input = _upcomingInput;
		InputInfo p2Input = _upcomingInput2;
		_upcomingInput.KickButton = false;
		_upcomingInput.PunchButton = false;
		_upcomingInput2.KickButton = false;
		_upcomingInput2.PunchButton = false;

		_p1.Tick(p1Input, p2CurrentPos);
		_p2.Tick(p2Input, p1CurrentPos);
		SeparateColliders();
		ProcessHits();
		_p1.UpdateVisuals();
		_p2.UpdateVisuals();
	}

	private void SeparateColliders()
	{
		// ensure that fighters are not out of bounds (L/R walls, and floor)
		foreach (Fighter p in new[] { _p1, _p2 })
		{
			float offsetFromLeft = p.Position.x + p.HitBoxes.CollisionBox.xMin - (-_arenaHalfWidth);
			float offsetFromRight = p.Position.x + p.HitBoxes.CollisionBox.xMax - _arenaHalfWidth;
			float yPos = Mathf.Max(p.Position.y, 0f);

			if (offsetFromLeft < 0)
			{
				p.Position = new(p.Position.x - offsetFromLeft, yPos);
			}
			else if (offsetFromRight > 0)
			{
				p.Position = new(p.Position.x - offsetFromRight, yPos);
			}
		}

		// separate fighters if they collide.
		Rect r1 = RectUtils.TranslateRect(_p1.HitBoxes.CollisionBox, _p1.Position);
		Rect r2 = RectUtils.TranslateRect(_p2.HitBoxes.CollisionBox, _p2.Position);
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
			float offsetFromLeft = leftRect.xMin + leftOffset - (-_arenaHalfWidth);
			float offsetFromRight = rightRect.xMax + rightOffset - _arenaHalfWidth;

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

			leftFighter.Position = new(leftFighter.Position.x + leftOffset, leftFighter.Position.y);
			rightFighter.Position = new(rightFighter.Position.x + rightOffset, rightFighter.Position.y);
		}
	}

	private AttackInfo? DetermineHits(Fighter attacker, Fighter target)
	{
		if (attacker.HitBoxes.Attack.HasValue)
		{
			Rect translatedAttack = RectUtils.TranslateRect(
				attacker.HitBoxes.Attack.Value.Bounds,
				attacker.Position - target.Position
			);

			foreach (Rect hurtBox in target.HitBoxes.HurtBoxes)
			{
				if (translatedAttack.Overlaps(hurtBox))
				{
					return attacker.HitBoxes.Attack.Value;
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
			attackOnP1Result = _p1.OnHitByAttack(attackOnP1.Value);
		}
		if (attackOnP2.HasValue)
		{
			attackOnP2Result = _p2.OnHitByAttack(attackOnP2.Value);
		}

		if (attackOnP1Result != AttackResult.None)
		{
			_p2.OnAttackLanded();
		}
		if (attackOnP2Result != AttackResult.None)
		{
			_p1.OnAttackLanded();
		}
	}

	private void OnDrawGizmos()
	{
		if (!Application.isPlaying || !_gameStarted)
		{
			return;
		}

		// world bounds
		Gizmos.color = Color.pink;
		Gizmos.DrawLine(new(-_arenaHalfWidth, 0f), new(-_arenaHalfWidth, 20f));
		Gizmos.DrawLine(new(_arenaHalfWidth, 0f), new(_arenaHalfWidth, 20f));
		Gizmos.DrawLine(new(-_arenaHalfWidth, 0f), new(_arenaHalfWidth, 0f));

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
