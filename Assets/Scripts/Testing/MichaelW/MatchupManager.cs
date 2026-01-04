using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public class MatchupManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private FightManager _fightManager;

	[SerializeField]
	private RoundIndicatorUI _p1RoundIndicator;

	[SerializeField]
	private RoundIndicatorUI _p2RoundIndicator;

	private int _p1Wins = 0;
	private int _p2Wins = 0;

	private void OnEnable()
	{
		_fightManager.OnGameEnd.AddListener(OnGameEnd);
	}

	private void OnDisable()
	{
		if (_fightManager)
		{
			_fightManager.OnGameEnd.RemoveListener(OnGameEnd);
		}
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
	private void Reset()
	{
		_p1Wins = 0;
		_p2Wins = 0;
		_p1RoundIndicator.UpdateDisplay(_p1Wins);
		_p2RoundIndicator.UpdateDisplay(_p2Wins);
		NewFight();
	}

	private void OnGameEnd(GameResult result)
	{
		if (result == GameResult.P1Win)
		{
			_p1Wins++;
			_p1RoundIndicator.UpdateDisplay(_p1Wins);
		}
		else if (result == GameResult.P2Win)
		{
			_p2Wins++;
			_p2RoundIndicator.UpdateDisplay(_p2Wins);
		}

		// TODO: brief pause, where we display the winner
		// this is a temporary thing to show that we can wait before starting a new fight.
		Tween.Delay(this, duration: 4f, NewFight);
	}
}
