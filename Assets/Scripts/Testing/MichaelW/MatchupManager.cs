using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class MatchupManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private FightManager _fightManager;

	[SerializeField]
	private RoundIndicatorUI _p1RoundIndicator;

	[SerializeField]
	private RoundIndicatorUI _p2RoundIndicator;

	[SerializeField]
	private int _winsNeeded = 2;

	[Header("Events")]
	[HideInInspector]
	public UnityEvent OnRoundWin;

	[HideInInspector]
	public UnityEvent OnRoundLose;

	[HideInInspector]
	public UnityEvent OnRoundDraw;

	[HideInInspector]
	public UnityEvent OnGameWin;

	[HideInInspector]
	public UnityEvent OnGameLose;

	private int _p1Wins = 0;
	private int _p2Wins = 0;

	private void OnEnable()
	{
		_fightManager.OnGameEnd.AddListener(OnGameEnd);
	}

	private void OnDisable()
	{
		_fightManager?.OnGameEnd.RemoveListener(OnGameEnd);
	}

	private void Start()
	{
		Reset();
	}

	private void NewFight()
	{
		_fightManager.ResetGame();
		// TODO: brief pause, where we show the countdown
		// this is a temporary thing to show that we can wait before enabling controls.
		Tween.Delay(this, duration: 2f, _fightManager.StartFight);
	}

	[Button]
	public void Reset()
	{
		_p1Wins = 0;
		_p2Wins = 0;
		_p1RoundIndicator.UpdateDisplay(_p1Wins);
		_p2RoundIndicator.UpdateDisplay(_p2Wins);
		NewFight();
	}

	private void OnGameEnd(GameResult result)
	{
		bool matchOver = false;

		if (result == GameResult.P1Win)
		{
			_p1Wins++;
			_p1RoundIndicator.UpdateDisplay(_p1Wins);

			if (_p1Wins >= _winsNeeded)
			{
				OnGameWin?.Invoke();
				matchOver = true;
			}
			else
			{
				OnRoundWin?.Invoke();
			}
		}
		else if (result == GameResult.P2Win)
		{
			_p2Wins++;
			_p2RoundIndicator.UpdateDisplay(_p2Wins);

			if (_p2Wins >= _winsNeeded)
			{
				OnGameLose?.Invoke();
				matchOver = true;
			}
			else
			{
				OnRoundLose?.Invoke();
			}
		}
		else
		{
			OnRoundDraw?.Invoke();
		}

		if (!matchOver)
		{
			// if the match isn't over, start the next round
			Tween.Delay(this, duration: 4f, NewFight);
		}
		else
		{
			// Match is over, display win / lose screen
		}
	}
}
