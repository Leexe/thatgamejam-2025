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
		AudioManager.Instance.SwitchMusicTrack(FMODEvents.Instance.Title_Bgm);
	}

	private void PlayRealTitle()
	{
		AudioManager.Instance.StopCurrentMusicTrack();
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.TitleDap_Sfx);
	}

	public void PlayTitleRevealSfx()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.TitleReveal_Sfx);
	}
}
