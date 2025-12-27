using UnityEngine;
using UnityEngine.Events;

public class GameManager : PersistentSingleton<GameManager>
{
	// Events
	[HideInInspector]
	public DialogueEvents DialogueEventsRef = new DialogueEvents();

	[HideInInspector]
	public UnityEvent OnGamePaused;

	[HideInInspector]
	public UnityEvent OnGameResume;

	// Private Variables
	public bool GamePaused { private set; get; } = false;

	private void OnEnable()
	{
		InputManager.Instance.OnEscapePerformed.AddListener(TogglePauseGame);
	}

	private void OnDisable()
	{
		if (InputManager.Instance)
		{
			InputManager.Instance.OnEscapePerformed.RemoveListener(TogglePauseGame);
		}
	}

	// Toggles between pausing and unpausing the game
	public void TogglePauseGame()
	{
		if (GamePaused)
		{
			// Resume Game
			GamePaused = false;
			OnGameResume?.Invoke();
			UnfreezeTime();
		}
		else
		{
			// Pause Game
			GamePaused = true;
			OnGamePaused?.Invoke();
			FreezeTime();
		}
	}

	/// <summary>
	/// Unpauses the game, sets the timescale to 0
	/// </summary>
	private void FreezeTime()
	{
		Time.timeScale = 0f;
	}

	/// <summary>
	/// Unpauses the game, sets the timescale to 1
	/// </summary>
	private void UnfreezeTime()
	{
		Time.timeScale = 1f;
	}

	/// <summary>
	/// Exits game by quitting the application
	/// </summary>
	public void ExitGame()
	{
		Application.Quit();
	}
}
