using System.Collections.Generic;
using UnityEngine;

public class FightAI : MonoBehaviour
{
	[Header("References")]
	//

	private BasicFighter _m;
	private BasicFighter _e;

	// queue of upcoming inputs to play.
	private readonly Queue<Act> _actionQueue = new();
	private InputInfo _lastInput = default;

	private Context _ctx;
	private readonly MovePool _movePool = new();

	private Vector2 _dir = Vector2.zero;

	private int _repositionCounter = 1;
	private float _targetRange = 1.6f;

	// constants
	private readonly float _spacing = 1.8f;

	public void Set(BasicFighter me, BasicFighter them)
	{
		_m = me;
		_e = them;
		_actionQueue.Clear();
	}

	public InputInfo Tick()
	{
		// if we have an ongoing input sequence, process it
		if (_actionQueue.Count > 0)
		{
			_lastInput = InputFromAction(_actionQueue.Dequeue());
			return _lastInput;
		}

		_ctx = Context.Create(_m, _e);
		_movePool.Clear();

		Debug.Log(_ctx.OffsetMag);

		StandIdle_Wins();
		StandIdle_RandomAttacks();
		MoveSet_Spacing();
		MoveSet_GrabRecovery();

		// get options
		Act actionThisFrame = Act.N;

		if (_movePool.CanSample())
		{
			MovePool.Option option = _movePool.Sample();
			for (int i = 1; i < option.Actions.Length; i++)
			{
				_actionQueue.Enqueue(option.Actions[i]);
			}
			actionThisFrame = option.Actions[0];
		}

		_lastInput = InputFromAction(actionThisFrame);
		return _lastInput;
	}

	private InputInfo InputFromPositioning()
	{
		_repositionCounter--;
		if (_repositionCounter < 1)
		{
			_targetRange = Random.Range(1.3f, 1.9f);
			_repositionCounter += Random.Range(15, 40);
		}

		if (_ctx.OffsetMag > _targetRange + 0.3f)
		{
			_dir = _ctx.TowardDir * Vector2.right;
		}
		else if (_ctx.OffsetMag < _targetRange - 0.3f)
		{
			_dir = -_ctx.TowardDir * Vector2.right;
		}
		else if (_ctx.FacingInDist(_targetRange, 0.04f))
		{
			_dir = Vector2.zero;
		}

		return new() { Dir = _dir };
	}

	private InputInfo InputFromAction(Act action)
	{
		if (action == Act.N)
		{
			return InputFromPositioning();
		}

		_dir.x = action switch
		{
			Act.MS or Act.JU => 0f,
			Act.ML or Act.JL => -1f,
			Act.MR or Act.JR => 1f,
			Act.MT or Act.JT => _ctx.TowardDir,
			Act.MA or Act.JA or Act.CB => -_ctx.TowardDir,
			_ => _dir.x,
		};
		_dir.y = action switch
		{
			Act.MS or Act.ML or Act.MR or Act.MT or Act.MA => 0f,
			Act.JU or Act.JL or Act.JR or Act.JT or Act.JA => 1f,
			Act.C or Act.CB => -1f,
			Act.UC => 0f,
			_ => _dir.y,
		};

		return new()
		{
			Dir = _dir,
			PunchButton = action == Act.P,
			KickButton = action == Act.K,
			GrabButton = action == Act.G,
		};
	}

	// move adders

	private void RangeOpt(string label, Vector2 range, float freq, string code, int prio = 1)
	{
		if (_ctx.FacingInRange(range.x, range.y))
		{
			_movePool.Add(prio, freq, FromString(code));
		}
	}

	private void StandIdle_Wins()
	{
		if (_ctx.Me.GetState() != BasicFighter.State.StandIdle)
		{
			return;
		}

		// standing, against a standing kick
		if (_ctx.EnemyStartedAnim(BasicFighter.State.StandKick))
		{
			RangeOpt("mb", new(0.4f, 3.0f), 45f, "w4 ma w12", prio: 2);
			RangeOpt("ck", new(0.4f, 1.75f), 20f, "c k", prio: 2);
			RangeOpt("p", new(0.4f, 1.2f), 20f, "p", prio: 2);
			RangeOpt("cp", new(0.4f, 1.2f), 20f, "c p", prio: 2);
			RangeOpt("db", new(0.4f, 1.55f), 6f, "da", prio: 2);
			RangeOpt("dbj", new(0.4f, 1.55f), 6f, "da w4 jt w26 k", prio: 2);
		}
		// standing, against a standing punch
		if (_ctx.EnemyStartedAnim(BasicFighter.State.StandPunch))
		{
			RangeOpt("mb", new(0.4f, 1.3f), 36f, "w2 ma w12", prio: 2);
			RangeOpt("db", new(0.4f, 1.2f), 10f, "da", prio: 2);
			RangeOpt("dbj", new(0.4f, 1.2f), 3f, "da w4 jt w12 k", prio: 2);
		}

		// standing, against a crouching kick
		if (_ctx.EnemyStartedAnim(BasicFighter.State.CrouchKick))
		{
			RangeOpt("mb", new(1.55f, 2.0f), 45f, "w4 ma w12", prio: 2);
			RangeOpt("cb", new(0.4f, 1.55f), 45f, "w4 cb w12", prio: 2);
			RangeOpt("jk", new(0.4f, 1.45f), 25f, "ju w18 k", prio: 2);
			RangeOpt("jtk", new(1.45f, 2.5f), 25f, "jt w18 k", prio: 2);
			RangeOpt("db", new(0.4f, 1.2f), 10f, "w4 da", prio: 2);
			RangeOpt("dbj", new(0.4f, 1.55f), 10f, "da w4 jt w26 k", prio: 2);
		}

		// standing, against a flying kick
		if (_ctx.EnemyStartedAnim(BasicFighter.State.JumpKick))
		{
			RangeOpt("mb", new(0.8f, 2.0f), 45f, "w4 ma w12", prio: 2);
			// RangeOpt("k", new(0.4f, 2.0f), 20f, "w2 k", prio: 2);
			if (_ctx.Enemy.GetVel().y > -0.03f)
			{
				RangeOpt("dt", new(0.4f, 2.0f), 20f, "w2 dt", prio: 2);
			}
		}

		if (_ctx.EnemyStartedAnim(BasicFighter.State.DashForward))
		{
			RangeOpt("mb", new(0.8f, 3.0f), 40f, "w4 p", prio: 2);
			// RangeOpt("k", new(0.4f, 2.0f), 20f, "w2 k", prio: 2);
			RangeOpt("jk", new(0.8f, 2.5f), 20f, "ju w18 k", prio: 2);
		}

		// standing, against a grab
		if (_ctx.EnemyStartedAnim(BasicFighter.State.Grab))
		{
			RangeOpt("db", new(0.4f, 2.0f), 20f, "w4 da", prio: 2);
			RangeOpt("p", new(0.4f, 1.8f), 20f, "w4 p", prio: 2);
			RangeOpt("k", new(0.4f, 1.8f), 20f, "w4 k", prio: 2);
			RangeOpt("jk", new(0.4f, 1.45f), 15f, "ju w18 k", prio: 2);
		}

		// sneaking under a jump
		if (_ctx.Enemy.GetState() == BasicFighter.State.JumpIdle)
		{
			float enemyYVel = _ctx.Enemy.GetVel().y;
			if (enemyYVel is > -0.1f and < 0.15f)
			{
				RangeOpt("dt", new(0.4f, 1.1f), 30f, "w4 dt", prio: 2);
			}
		}
	}

	private void MoveSet_GrabRecovery()
	{
		if (_ctx.Me.GetState() == BasicFighter.State.Knocked)
		{
			RangeOpt("p", new(0f, 5f), 100f, "p");
		}
	}

	private void StandIdle_RandomAttacks()
	{
		if (_ctx.Me.GetState() == BasicFighter.State.StandIdle)
		{
			if (_ctx.Enemy.GetState() != BasicFighter.State.JumpIdle)
			{
				RangeOpt("p", new(0.6f, 1.2f), 3.2f, "p");
				RangeOpt("k", new(1.0f, 1.7f), 3.2f, "k");
				RangeOpt("g", new(0.8f, 1.3f), 0.4f, "g");
				RangeOpt("d g", new(1.5f, 2.5f), 0.2f, "dt g");
				// temp
				RangeOpt("juk", new(0.4f, 1.45f), 0.3f, "ju w14 k");
				RangeOpt("jtk", new(1.45f, 2.5f), 0.3f, "jt w14 k");
				//
				RangeOpt("cp", new(1.2f, 1.5f), 2.6f, "c p");
				RangeOpt("ck", new(1.2f, 1.82f), 2.6f, "c k");
				RangeOpt("db", new(1.2f, 1.82f), 0.4f, "da");
				RangeOpt("mt", new(1.8f, 2.5f), 3.0f, "mt w6");
				RangeOpt("dbjt", new(1.2f, 1.6f), 0.3f, "da w4 jt w14 k");
			}
		}
	}

	private void MoveSet_Spacing()
	{
		// ensure proper spacing
		if (_ctx.OffsetMag > _spacing + 1.2f)
		{
			_movePool.Add(1, 0.05f, new[] { Act.MS, Act.MT, Act.MS, Act.MT });
		}
	}

	// types

	private struct Context
	{
		public BasicFighter Me;
		public BasicFighter Enemy;
		public HitBoxData MBoxes;
		public HitBoxData EBoxes;
		public float OffsetX;
		public float OffsetY;
		public readonly float OffsetMag => Mathf.Abs(OffsetX);
		public readonly float TowardDir => Mathf.Sign(OffsetX);

		public static Context Create(BasicFighter me, BasicFighter enemy)
		{
			float xOffset = enemy.transform.position.x - me.transform.position.x;

			return new()
			{
				Me = me,
				Enemy = enemy,
				EBoxes = enemy.ReadHitBoxes(),
				MBoxes = me.ReadHitBoxes(),
				OffsetX = xOffset,
				OffsetY = enemy.transform.position.y - me.transform.position.y,
			};
		}

		public readonly bool FacingInDist(float dist, float tolerance = 0.1f)
		{
			// todo: check if facing the right way
			return OffsetMag >= dist - tolerance && OffsetMag <= dist + tolerance;
		}

		public readonly bool FacingInRange(float from, float to)
		{
			// todo: check if facing the right way
			return OffsetMag >= from && OffsetMag <= to;
		}

		public readonly bool EnemyStartedAnim(BasicFighter.State state, int frameMargin = 2)
		{
			return Enemy.GetState() == state && Enemy.GetStateCounter() < frameMargin;
		}
	}

	private class MovePool
	{
		private readonly List<Option> _pool = new();

		public void Clear()
		{
			_pool.Clear();
		}

		public bool CanSample()
		{
			return _pool.Count > 0;
		}

		public Option Sample()
		{
			int highestPrio = 0;
			for (int i = 0; i < _pool.Count; i++)
			{
				if (_pool[i].Priority > highestPrio)
				{
					highestPrio = _pool[i].Priority;
				}
			}

			int filteredCount = 0;
			int totalWeight = 0;
			for (int i = 0; i < _pool.Count; i++)
			{
				if (_pool[i].Priority == highestPrio && Random.value < (_pool[i].Rarity / 60f))
				{
					_pool[filteredCount] = _pool[i];
					totalWeight += _pool[filteredCount].Weight;
					filteredCount++;
				}
			}

			if (filteredCount == 0)
			{
				return new() { Actions = new[] { Act.N } };
			}

			int sample = Mathf.FloorToInt((Random.value * totalWeight) - 0.00001f);
			int selection = 0;

			while (sample >= _pool[selection].Weight)
			{
				sample -= _pool[selection].Weight;
				selection++;
			}

			return _pool[selection];
		}

		public void Add(int prio, float rarity, Act[] actions)
		{
			_pool.Add(
				new()
				{
					Priority = prio,
					Rarity = rarity,
					Weight = 1,
					Actions = actions,
				}
			);
		}

		public struct Option
		{
			public int Priority;
			public float Rarity;

			public int Weight;
			public string Key;
			public int MinCoolDown;
			public Act[] Actions;
		}
	}

	private enum Act
	{
		ML,
		MR,
		MT,
		MA,
		MS,
		C,
		UC,
		CB,
		K,
		P,
		G,
		JU,
		JL,
		JR,
		JT,
		JA,
		W1,
		N,
	}

	private Act[] FromString(string st)
	{
		var result = new List<Act>();
		string[] tokens = st.ToLower().Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

		foreach (string token in tokens)
		{
			if (token.StartsWith("d"))
			{
				// Handle dash commands: d[l/r/t/a]
				if (token.Length == 2)
				{
					char dir = token[1];
					Act moveAct = dir switch
					{
						'l' => Act.ML,
						'r' => Act.MR,
						't' => Act.MT,
						'a' => Act.MA,
						_ => Act.N,
					};

					if (moveAct == Act.N)
					{
						Debug.LogError($"invalid dash command: {token}");
					}

					// Dash is: MS, M[dir], MS, M[dir]
					result.Add(Act.MS);
					result.Add(moveAct);
					result.Add(Act.MS);
					result.Add(moveAct);
				}
				else
				{
					Debug.LogError($"invalid dash command: {token}");
				}
			}
			else if (token.StartsWith("w"))
			{
				// Handle wait commands: w[number]
				if (token.Length > 1 && int.TryParse(token[1..], out int count))
				{
					for (int i = 0; i < count; i++)
					{
						result.Add(Act.W1);
					}
				}
				else
				{
					Debug.LogError($"invalid wait command: {token}");
				}
			}
			else
			{
				// Handle regular enum values with explicit matching
				Act act = token switch
				{
					"ml" => Act.ML,
					"mr" => Act.MR,
					"mt" => Act.MT,
					"ma" => Act.MA,
					"ms" => Act.MS,
					"c" => Act.C,
					"uc" => Act.UC,
					"cb" => Act.CB,
					"k" => Act.K,
					"p" => Act.P,
					"g" => Act.G,
					"ju" => Act.JU,
					"jl" => Act.JL,
					"jr" => Act.JR,
					"jt" => Act.JT,
					"ja" => Act.JA,
					"w1" => Act.W1,
					_ => Act.N,
				};

				if (act == Act.N)
				{
					Debug.LogError($"invalid command: {token}");
				}

				result.Add(act);
			}
		}

		return result.ToArray();
	}
}
