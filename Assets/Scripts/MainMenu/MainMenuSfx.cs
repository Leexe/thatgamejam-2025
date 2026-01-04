using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
	[SerializeField]
	private MainMenuController _mainMenuController;

	private void OnEnable()
	{
		_mainMenuController.OnIntroStart.AddListener(PlaySoftTitle);
		_mainMenuController.OnPlayStart.AddListener(PlayRealTitle);
	}

	private void OnDisable()
	{
		_mainMenuController.OnIntroStart.RemoveListener(PlaySoftTitle);
		_mainMenuController.OnPlayStart.RemoveListener(PlayRealTitle);
	}

	private void PlaySoftTitle()
	{
		AudioManager.Instance.PlayMusicTrack(FMODEvents.Instance.SoftTitle_Bgm);
	}

	private void PlayRealTitle()
	{
		AudioManager.Instance.PlayMusicTrack(FMODEvents.Instance.RealTitle_Bgm);
	}
}
