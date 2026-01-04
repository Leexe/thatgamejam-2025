using UnityEngine;

public class FightingGameMusic : MonoBehaviour
{
	private void Start()
	{
		PlayFightingGameMusic();
	}

	private void PlayFightingGameMusic()
	{
		AudioManager.Instance.SwitchMusicTrack(FMODEvents.Instance.FightingGame_Bgm, true);
	}

	private void PauseFightingGameMusic()
	{
		AudioManager.Instance.SwitchMusicTrack(FMODEvents.Instance.FightingGame_Bgm);
	}
}
