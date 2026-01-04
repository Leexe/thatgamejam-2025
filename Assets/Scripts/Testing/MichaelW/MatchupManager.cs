using Sirenix.OdinInspector;
using UnityEngine;

public class MatchupManager : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private FightManager _fightManager;

	private int _p1Wins = 0;
	private int _p2Wins = 0;

	private int _counter = 0;

	private void OnEnable()
	{
		_fightManager.OnHealthUpdate.AddListener(OnHealthUpdate);
		_fightManager.OnGameEnd.AddListener(OnGameEnd);
	}

	private void OnDisable()
	{
		if (_fightManager)
		{
			_fightManager.OnHealthUpdate.RemoveListener(OnHealthUpdate);
			_fightManager.OnGameEnd.RemoveListener(OnGameEnd);
		}
	}

	private void Start()
	{
		Reset();
	}

	private void FixedUpdate()
	{
		if (_counter > 0)
		{
			_counter--;
			if (_counter == 0)
			{
				_fightManager.ResetGame();
				_fightManager.StartFight();
			}
		}
	}

	[Button]
	private void Reset()
	{
		_p1Wins = 0;
		_p2Wins = 0;
		_fightManager.ResetGame();
		_fightManager.StartFight();
	}

	private void OnHealthUpdate(int p1Health, int p1Max, int p2Health, int p2Max)
	{
		// ui change?
	}

	private void OnGameEnd(GameResult result)
	{
		if (result == GameResult.P1Win)
		{
			_p1Wins++;
		}
		else if (result == GameResult.P2Win)
		{
			_p2Wins++;
		}

		// yikes
		_counter = 240;

		Debug.Log($"{_p1Wins} {_p2Wins}]");
	}
}
