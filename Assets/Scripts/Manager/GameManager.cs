using System.Collections;
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

	[HideInInspector]
	public UnityEvent OnSceneReady;

	// Private Variables
	public bool GamePaused { private set; get; }
	private AsyncOperation _asyncOperation;

	private void OnEnable()
	{
		InputManager.Instance.OnEscapePerformed.AddListener(TogglePauseGame);
		DialogueEvents.Instance.AddBlockingCondition(() => GamePaused);
		SceneManager.sceneLoaded += SceneLoaded;
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
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

		SceneManager.sceneLoaded -= SceneLoaded;

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

	public void LoadSceneAsync(SceneNames sceneName)
	{
		StartCoroutine(LoadSceneAsyncEnumerator(sceneName));
	}

	/// <summary>
	/// Loads a scene asynchronously
	/// </summary>
	/// <param name="sceneName">Scene Name To Load</param>
	private IEnumerator LoadSceneAsyncEnumerator(SceneNames sceneName)
	{
		// Start loading the scene asynchronously in the background
		_asyncOperation = SceneManager.LoadSceneAsync((int)sceneName, LoadSceneMode.Single);

		if (_asyncOperation != null)
		{
			// Prevent the scene from activating and displaying immediately
			_asyncOperation.allowSceneActivation = false;

			while (!_asyncOperation.isDone)
			{
				float progress = Mathf.Clamp01(_asyncOperation.progress / 0.9f);
				Debug.Log("Loading progress: " + (progress * 100) + "%");

				if (_asyncOperation.progress >= 0.9f)
				{
					Debug.Log("Scene fully preloaded");
					OnSceneReady?.Invoke();
					break;
				}

				yield return null;
			}
		}
	}

	/// <summary>
	/// Activates the preloaded scene
	/// </summary>
	public void ActivatePreloadedScene()
	{
		if (_asyncOperation is { progress: >= 0.9f })
		{
			_asyncOperation.allowSceneActivation = true;
		}
	}

	/// <summary>
	/// Switches to a different scene
	/// </summary>
	/// <param name="sceneName">Scene Name To Switch To</param>
	public void SwitchScenes(SceneNames sceneName)
	{
		SceneManager.LoadScene((int)sceneName);
	}
}
