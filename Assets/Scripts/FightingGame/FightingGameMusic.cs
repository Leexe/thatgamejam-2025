using UnityEngine;

public class FightingGameMusic : MonoBehaviour
{
	[SerializeField]
	private MatchupManager _matchupManager;

	private void OnEnable()
	{
		if (_matchupManager != null)
		{
			_matchupManager.OnGameWin.AddListener(PlayWinSong);
			_matchupManager.OnGameLose.AddListener(PlayLoseSong);
		}
	}

	private void OnDisable()
	{
		if (_matchupManager != null)
		{
			_matchupManager.OnGameWin.RemoveListener(PlayWinSong);
			_matchupManager.OnGameLose.RemoveListener(PlayLoseSong);
		}
	}

	private void PlayWinSong()
	{
		AudioManager.Instance.StopCurrentMusicTrack();
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.TitleJingle_Sfx);
	}

	private void PlayLoseSong()
	{
		Debug.Log("Play Lose Song");
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Lose_Sfx);
		AudioManager.Instance.SwitchMusicTrack(FMODEvents.Instance.Lose_LoopSFX);
	}
}
