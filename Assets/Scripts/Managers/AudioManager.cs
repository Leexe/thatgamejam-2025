using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioManager : PersistantSingleton<AudioManager>
{
	private float _masterVolume = 1f;
	private float _musicVolume = 1f;
	private float _ambientVolume = 1f;
	private float _gameVolume = 1f;

	private Bus _masterBus;
	private Bus _musicBus;
	private Bus _ambientBus;
	private Bus _gameBus;

	private List<EventInstance> _eventInstances;
	private List<StudioEventEmitter> _eventEmitters;

	private EventReference _currentMusicReference;

	// Music Instances
	private EventInstance _currentMusicTrack;
	private EventInstance _iceBossaMusicTrack;
	private EventInstance _bloodMoonMusicTrack;
	private EventInstance _roamMusicTrack;
	private EventInstance _currentAmbientTrack;

	protected override void Awake()
	{
		base.Awake();

		_eventInstances = new List<EventInstance>();
		_eventEmitters = new List<StudioEventEmitter>();

		// Get Bus Names
		_masterBus = RuntimeManager.GetBus("bus:/");
		_musicBus = RuntimeManager.GetBus("bus:/BGM");
		_ambientBus = RuntimeManager.GetBus("bus:/AMB");
		_gameBus = RuntimeManager.GetBus("bus:/SFX");

		// Fetch audio preferences
		_masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
		_ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 1f);
		_musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
		_gameVolume = PlayerPrefs.GetFloat("GameVolume", 0.7f);
	}

	private void Start()
	{
		_iceBossaMusicTrack = CreateInstance(FMODEvents.Instance.IceBossa_Bgm);
		_bloodMoonMusicTrack = CreateInstance(FMODEvents.Instance.BloodMoon_Bgm);
		_roamMusicTrack = CreateInstance(FMODEvents.Instance.Roam_Bgm);
		_currentAmbientTrack = CreateInstance(FMODEvents.Instance.Ambience_Amb);
		_currentMusicTrack = _roamMusicTrack;
		_currentMusicReference = FMODEvents.Instance.Roam_Bgm;
	}

	private void OnDestroy()
	{
		CleanUp();
		SaveAudioPref();
	}

	/// <summary>
	/// Resume the current music track
	/// </summary>
	public void PlayCurrentMusicTrack()
	{
		PlayInstance(_currentMusicTrack);
	}

	/// <summary>
	/// Pause the current music track
	/// </summary>
	public void PauseCurrentMusicTrack()
	{
		PauseInstance(_currentMusicTrack);
	}

	/// <summary>
	/// Pause the current music track
	/// </summary>
	public void StopCurrentMusicTrack()
	{
		StopInstance(_currentMusicTrack);
	}

	/// <summary>
	/// Play the current ambient track
	/// </summary>
	public void PlayCurrentAmbientTrack()
	{
		PlayInstance(_currentAmbientTrack);
	}

	/// <summary>
	/// Pause the current ambient track
	/// </summary>
	public void PauseCurrentAmbientTrack()
	{
		StopInstance(_currentAmbientTrack);
	}

	/// <summary>
	/// Pause the current music track
	/// </summary>
	public void StopCurrentAmbientTrack()
	{
		StopInstance(_currentAmbientTrack, true);
	}

	/// <summary>
	/// Switches the music track (TODO: Create a smarter way to switch music without if statements)
	/// </summary>
	public void SwitchMusicTrack(EventReference musicTrack)
	{
		if (musicTrack.Guid == _currentMusicReference.Guid)
		{
			PlayCurrentMusicTrack();
			return;
		}

		_currentMusicReference = musicTrack;
		_currentMusicTrack.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

		if (musicTrack.Guid == FMODEvents.Instance.IceBossa_Bgm.Guid)
		{
			_currentMusicTrack = _iceBossaMusicTrack;
		}
		else if (musicTrack.Guid == FMODEvents.Instance.BloodMoon_Bgm.Guid)
		{
			_currentMusicTrack = _bloodMoonMusicTrack;
		}
		else if (musicTrack.Guid == FMODEvents.Instance.Roam_Bgm.Guid)
		{
			_currentMusicTrack = _roamMusicTrack;
		}

		_currentMusicTrack.start();
	}

	/// <summary>
	/// Plays a sound effect
	/// </summary>
	/// <param name="sound">The FMOD event reference of the sound</param>
	public void PlayOneShot(EventReference sound)
	{
		RuntimeManager.PlayOneShot(sound);
	}

	/// <summary>
	/// Plays a sound effect attached to a game object
	/// </summary>
	/// <param name="sound">The FMOD event reference of the sound</param>
	/// <param name="gameObject">The game object to be attached to</param>
	public void PlayOneShot(EventReference sound, GameObject gameObject)
	{
		RuntimeManager.PlayOneShotAttached(sound, gameObject);
	}

	/// <summary>
	/// Plays a sound effect at some position in the world
	/// </summary>
	/// <param name="sound">The FMOD event reference of the sound</param>
	/// <param name="pos">Some position in the world</param>
	public void PlayOneShot(EventReference sound, Vector3 pos)
	{
		RuntimeManager.PlayOneShot(sound, position: pos);
	}

	/// <summary>
	/// Creates an event instance of a sound, which allows a sound to be looped or it's FMOD parameters to be modified
	/// </summary>
	/// <param name="sound">The FMOD event reference of the sound</param>
	/// <returns>Returns a cached event instance of the sound, that can be used to modify the sound at runtime</returns>
	public EventInstance CreateInstance(EventReference sound)
	{
		EventInstance eventInstance = RuntimeManager.CreateInstance(sound);
		eventInstance.setTimelinePosition(0);
		_eventInstances.Add(eventInstance);
		return eventInstance;
	}

	/// <summary>
	/// Creates an event instance of a sound and attaches it to a game object
	/// </summary>
	/// <param name="sound"><The FMOD event reference of the sound/param>
	/// <param name="gameObject">The game object to attach to</param>
	/// <returns>Returns a cached event instance of the sound, that can be used to modify the sound at runtime</returns>
	public EventInstance CreateInstance(EventReference sound, GameObject gameObject)
	{
		EventInstance eventInstance = RuntimeManager.CreateInstance(sound);
		RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
		_eventInstances.Add(eventInstance);
		return eventInstance;
	}

	/// <summary>
	/// Delete an event instance, pausing and freeing it from memory
	/// </summary>
	public void DeleteInstance(EventInstance eventInstance)
	{
		_eventInstances.Remove(eventInstance);
		eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		eventInstance.release();
	}

	/// <summary>
	/// Play an event instance sound at the start of the instance
	/// </summary>
	public void PlayInstanceAtStart(EventInstance eventInstance)
	{
		eventInstance.setTimelinePosition(0);
		eventInstance.start();
	}

	/// <summary>
	/// Resume playing the event instance sound
	/// </summary>
	public void PlayInstance(EventInstance instance)
	{
		// If the track is playing, do nothing
		instance.getPlaybackState(out PLAYBACK_STATE state);
		if (state == PLAYBACK_STATE.PLAYING)
		{
			return;
		}

		// If the track is paused, unpause it
		instance.getPaused(out bool isPaused);
		if (isPaused)
		{
			instance.setPaused(false);
		}
		// If the track was never played, start it
		else
		{
			instance.start();
		}
	}

	/// <summary>
	/// Pauses an event instance sound
	/// </summary>
	/// <param name="instance">The event instance sound</param>
	public void PauseInstance(EventInstance instance)
	{
		instance.setPaused(true);
	}

	/// <summary>
	/// Stops an event instance sound
	/// </summary>
	/// <param name="instance">The event instance sound</param>
	/// <param name="allowFadeOut">Allow the sound to fade out or not</param>
	public void StopInstance(EventInstance instance, bool allowFadeOut = true)
	{
		if (allowFadeOut)
		{
			instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		else
		{
			instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		}
	}

	/// <summary>
	/// Free up an event instance sound
	/// </summary>
	public void DestroyInstance(EventInstance instance)
	{
		instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
		instance.release();
		_eventInstances.Remove(instance);
	}

	/// <summary>
	/// Returns a boolean indicating if an event instance sound is playing
	/// </summary>
	public bool InstanceIsPlaying(EventInstance instance)
	{
		instance.getPlaybackState(out PLAYBACK_STATE state);
		return state != PLAYBACK_STATE.STOPPED;
	}

	// Cleans up sound events and event emitters
	private void CleanUp()
	{
		foreach (EventInstance eventInstance in _eventInstances)
		{
			eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			eventInstance.release();
		}
		foreach (StudioEventEmitter emitter in _eventEmitters)
		{
			emitter.Stop();
		}
	}

	/// <summary>
	/// Saves audio level preferences to player prefs
	/// </summary>
	public void SaveAudioPref()
	{
		PlayerPrefs.SetFloat("MasterVolume", _masterVolume);
		PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
		PlayerPrefs.SetFloat("GameVolume", _gameVolume);
	}

	// Getters and Setters
	public float GetMasterVolume() => _masterVolume;

	public float GetMusicVolume() => _musicVolume;

	public float GetAmbienceVolume() => _musicVolume;

	public float GetGameVolume() => _gameVolume;

	public void SetMasterVolume(float masterVolume)
	{
		_masterVolume = Mathf.Clamp(masterVolume, 0f, 1f);
		_masterBus.setVolume(_masterVolume);
	}

	public void SetMusicVolume(float musicVolume)
	{
		_musicVolume = Mathf.Clamp(musicVolume, 0f, 1f);
		_musicBus.setVolume(_musicVolume);
	}

	public void SetGameVolume(float gameVolume)
	{
		_gameVolume = Mathf.Clamp(gameVolume, 0f, 1f);
		_gameBus.setVolume(_gameVolume);
	}

	public void SetAmbienceVolume(float ambientVolume)
	{
		_ambientVolume = Mathf.Clamp(ambientVolume, 0f, 1f);
		_ambientBus.setVolume(_ambientVolume);
	}
}
