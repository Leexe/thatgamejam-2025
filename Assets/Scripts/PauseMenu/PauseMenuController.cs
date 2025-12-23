using PrimeTween;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private GameObject _pauseMenuCanvas;

	[SerializeField]
	private CanvasGroup _contentCanvasGroup;

	[SerializeField]
	private CanvasGroup _mainTabCanvasGroup;

	[SerializeField]
	private CanvasGroup _settingsTabCanvasGroup;

	[SerializeField]
	private Image _background;

	[Header("Tweening")]
	[SerializeField]
	private float _pauseTweenDuration = 0.5f;
	
	[SerializeField]
	private float _setttingsTweenDuration = 0.3f;

	// Events
	[HideInInspector]
	public UnityEvent OnPauseMenuOpen;
	
	[HideInInspector]
	public UnityEvent OnPauseMenuClose;

	// Private Variables
	private Sequence _transitionSequence;
	private float _progress = 0f;

	private void Start()
	{
		_pauseMenuCanvas.SetActive(false);

		// Make sure to be on the main tab and not the settings tab
		_mainTabCanvasGroup.interactable = true;
		_mainTabCanvasGroup.alpha = 1;
		_settingsTabCanvasGroup.interactable = false;
		_settingsTabCanvasGroup.alpha = 0;
	}

	private void OnEnable()
	{
		GameManager.Instance.OnGameResume.AddListener(UnpauseGame);
		GameManager.Instance.OnGamePaused.AddListener(PauseGame);
	}

	private void OnDisable()
	{
		if (InputManager.Instance)
		{
			GameManager.Instance.OnGameResume.RemoveListener(UnpauseGame);
			GameManager.Instance.OnGamePaused.RemoveListener(PauseGame);
		}
	}

	private void OnDestroy()
	{
		_transitionSequence.Stop();
	}

	// Pauses the Game
	private void PauseGame()
	{
		OnPauseMenuOpen?.Invoke();
		OpenPauseMenu();
	}

	// Unpauses the Game
	private void UnpauseGame()
	{
		OnPauseMenuClose?.Invoke();
		ClosePauseMenu();
	}

	// Plays menu open transitions and pauses the game time
	private void OpenPauseMenu()
	{
		_pauseMenuCanvas.SetActive(true);

		// Reset Back to Main Tab
		_mainTabCanvasGroup.interactable = true;
		_mainTabCanvasGroup.alpha = 1;
		_mainTabCanvasGroup.blocksRaycasts = true;
		_settingsTabCanvasGroup.interactable = false;
		_settingsTabCanvasGroup.alpha = 0;
		_settingsTabCanvasGroup.blocksRaycasts = false;

		// Allow buttons to be interactable
		EnableButtons();

		// Tweens
		_transitionSequence.Stop();
		_transitionSequence = Sequence
			.Create(useUnscaledTime: true)
			.Group(Tween.Custom(target: this, _progress, 1, _pauseTweenDuration, (target, val) => _progress = val))
			.Group(
				Tween.Custom(
					target: this,
					_progress,
					1,
					_pauseTweenDuration,
					(target, val) => target._contentCanvasGroup.alpha = val
				)
			)
			.Group(
				Tween.Custom(
					target: this,
					_progress,
					1,
					_pauseTweenDuration,
					(target, val) => target._background.material.SetFloat("_Progress", val)
				)
			);
	}

	// Plays menu close transitions and resumes the game time
	private void ClosePauseMenu()
	{
		_pauseMenuCanvas.SetActive(true);
		_transitionSequence.Stop();
		DisableButtons();

		// Tweens
		_transitionSequence = Sequence
			.Create(useUnscaledTime: true)
			.Group(Tween.Custom(target: this, _progress, 0, _pauseTweenDuration, (target, val) => _progress = val))
			.Group(
				Tween.Custom(
					target: this,
					_progress,
					0,
					_pauseTweenDuration,
					(target, val) => target._contentCanvasGroup.alpha = val
				)
			)
			.Group(
				Tween.Custom(
					target: this,
					_progress,
					0,
					_pauseTweenDuration,
					(target, val) => target._background.material.SetFloat("_Progress", val)
				)
			)
			.ChainCallback(() => _pauseMenuCanvas.SetActive(false));
	}

	private void OpenSettingsMenu()
	{
		_mainTabCanvasGroup.interactable = false;
		_mainTabCanvasGroup.blocksRaycasts = false;
		_settingsTabCanvasGroup.interactable = true;
		_settingsTabCanvasGroup.blocksRaycasts = true;

		// Tweens
		_transitionSequence.Stop();
		_transitionSequence = Sequence
			.Create(useUnscaledTime: true)
			.Group(
				Tween.Custom(
					target: this,
					_mainTabCanvasGroup.alpha,
					0,
					_setttingsTweenDuration,
					(target, val) => target._mainTabCanvasGroup.alpha = val
				)
			)
			.Chain(
				Tween.Custom(
					target: this,
					_settingsTabCanvasGroup.alpha,
					1,
					_setttingsTweenDuration,
					(target, val) => target._settingsTabCanvasGroup.alpha = val
				)
			);
	}

	private void CloseSettingsMenu()
	{
		_mainTabCanvasGroup.interactable = true;
		_mainTabCanvasGroup.blocksRaycasts = true;
		_settingsTabCanvasGroup.interactable = false;
		_settingsTabCanvasGroup.blocksRaycasts = false;

		// Tweens
		_transitionSequence.Stop();
		_transitionSequence = Sequence
			.Create(useUnscaledTime: true)
			.Group(
				Tween.Custom(
					target: this,
					_settingsTabCanvasGroup.alpha,
					0,
					_setttingsTweenDuration,
					(target, val) => target._settingsTabCanvasGroup.alpha = val
				)
			)
			.Chain(
				Tween.Custom(
					target: this,
					_mainTabCanvasGroup.alpha,
					1,
					_setttingsTweenDuration,
					(target, val) => target._mainTabCanvasGroup.alpha = val
				)
			);
	}

	// Makes buttons interactable
	private void EnableButtons()
	{
		_contentCanvasGroup.blocksRaycasts = true;
		_contentCanvasGroup.interactable = true;
	}

	// Makes buttons uninteractable
	private void DisableButtons()
	{
		_contentCanvasGroup.blocksRaycasts = false;
		_contentCanvasGroup.interactable = false;
	}

	// Resumes the game when button is pressed
	public void OnResumeButtonPressed()
	{
		GameManager.Instance.TogglePauseGame();
	}

	// Opens the settings menu when button is pressed
	public void OnSettingsButtonPressed()
	{
		OpenSettingsMenu();
	}

	// Closes the settings menu when button is pressed
	public void OnSettingsCloseButtonPressed()
	{
		CloseSettingsMenu();
	}

	// Closes the game when button is pressed
	public void OnExitButtonPressed()
	{
		GameManager.Instance.ExitGame();
	}
}
