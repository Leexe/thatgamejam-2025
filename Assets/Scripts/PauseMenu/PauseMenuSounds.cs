using UnityEngine;
using UnityEngine.UI;

public class PauseMenuSounds : MonoBehaviour {
	[Header("References")]
	[SerializeField] private PauseMenuController _pauseMenuController;

	[Header("Sound Sliders")]
	[SerializeField] private Slider _masterVolumeSlider;
	[SerializeField] private Slider _gameVolumeSlider;
	[SerializeField] private Slider _musicVolumeSlider;

	// Sound Instances
	// private EventInstance _masterTestInstance;
	// private EventInstance _gameTestInstance;
	// private EventInstance _musicTestInstance;

	private void Start() {
		// Create Audio Instances
		// _masterTestInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.MasterVolumeTest_sfx);
		// _gameTestInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.GameVolumeTest_sfx);
		// _musicTestInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.MusicVolumeTest_sfx);
		StopAllSounds();

		// Get Sound Settings
		_masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
		_gameVolumeSlider.value = AudioManager.Instance.GetGameVolume();
		_musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
	}

    private void OnEnable()
    {
        _pauseMenuController.OnPauseMenuClose.AddListener(SaveAllSoundPrefs);
    }

    private void OnDisable()
    {
        if (_pauseMenuController)
        {
            _pauseMenuController.OnPauseMenuClose.RemoveListener(SaveAllSoundPrefs);
        }
    }

    private void PlayButtonClickSound() {
		// AudioManager.Instance.PlayOneShot(FMODEvents.Instance.Click_sfx, Vector3.zero);
	}

	// Button Functions

	public void ChangeMasterVolume() {
		// if (!AudioManager.Instance.InstanceIsPlaying(_masterTestInstance)) {
		// 	_masterTestInstance.start();
		// }
		AudioManager.Instance.SetMasterVolume(_masterVolumeSlider.value);
	}

	public void ChangeGameVolume() {
		// if (!AudioManager.Instance.InstanceIsPlaying(_gameTestInstance)) {
		// 	_gameTestInstance.start();
		// }
		AudioManager.Instance.SetGameVolume(_gameVolumeSlider.value);
	}

	public void ChangeMusicVolume() {
		// if (!AudioManager.Instance.InstanceIsPlaying(_musicTestInstance)) {
		// 	_musicTestInstance.start();
		// }
		AudioManager.Instance.SetMusicVolume(_musicVolumeSlider.value);
	}

    private void SaveAllSoundPrefs()
    {
        AudioManager.Instance.SaveAudioPref();
    }

	private void StopAllSounds() {
		// _masterTestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		// _gameTestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		// _musicTestInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}
}
