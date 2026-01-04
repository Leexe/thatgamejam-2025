using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : PersistentMonoSingleton<GameManager>
{
	public enum SceneNames
	{
		Loading,
		MainMenu,
		FightingGame,
	}

	// Events
	[HideInInspector]
	public UnityEvent OnGamePaused;

	[HideInInspector]
	public UnityEvent OnGameResume;

	// Private Variables
	public bool GamePaused { private set; get; }

	private void OnEnable()
	{
		InputManager.Instance.OnEscapePerformed.AddListener(TogglePauseGame);
		DialogueEvents.Instance.AddBlockingCondition(() => GamePaused);
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.buildIndex == (int)SceneNames.FightingGame)
		{
			HideCursor();
		}
		else
		{
			ShowCursor();
		}
	}

	private void OnDisable()
	{
		if (InputManager.Instance)
		{
			InputManager.Instance.OnEscapePerformed.RemoveListener(TogglePauseGame);
		}

		SceneManager.sceneLoaded -= OnSceneLoaded;

		if (DialogueEvents.Instance != null)
		{
			DialogueEvents.Instance.RemoveBlockingCondition(() => GamePaused);
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

			// Only hide cursor if we are in the fighting game
			if (SceneManager.GetActiveScene().buildIndex == (int)SceneNames.FightingGame)
			{
				HideCursor();
			}
		}
		else
		{
			// Pause Game
			GamePaused = true;
			OnGamePaused?.Invoke();
			FreezeTime();
			ShowCursor();
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

	/// <summary>
	/// Shows the cursor and unlocks it
	/// </summary>
	public void ShowCursor()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	/// <summary>
	/// Hides the cursor and locks it to the center of the screen
	/// </summary>
	public void HideCursor()
	{
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
	}

	public void SwitchScenes(SceneNames sceneName)
	{
		SceneManager.LoadScene((int)sceneName);
	}
}
