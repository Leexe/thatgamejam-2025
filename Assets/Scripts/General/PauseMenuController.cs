using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private GameObject _pauseMenuCanvas;

	[SerializeField]
	private CanvasGroup _contentCanvasGroup;

	[SerializeField]
	private Image _background;

	[Header("Tweening")]
	[SerializeField]
	private float _tweenDuration = 0.5f;

	// Private Variables
	private Sequence _transitionSequence;
	private float _progress = 0f;
	private bool _gamePaused = false;

	private void Start()
	{
		_pauseMenuCanvas.SetActive(false);
	}

	private void OnEnable()
	{
		InputManager.Instance.OnEscapePerformed.AddListener(TogglePauseMenu);
	}

	private void OnDisable()
	{
		if (InputManager.Instance)
		{
			InputManager.Instance.OnEscapePerformed.RemoveListener(TogglePauseMenu);
		}
	}

	private void OnDestroy()
	{
		_transitionSequence.Stop();
	}

	// Switches between opening the menu and closing the menu
	private void TogglePauseMenu()
	{
		if (_gamePaused)
		{
			_gamePaused = false;
			ClosePauseMenu();
		}
		else
		{
			_gamePaused = true;
			OpenPauseMenu();
		}
	}

	// Plays menu open transitions and pauses the game time
	private void OpenPauseMenu()
	{
		_pauseMenuCanvas.SetActive(true);
		_transitionSequence.Stop();
		EnableButtons();
		GameManager.Instance.UnfreezeTime();
		_transitionSequence = Sequence
			.Create(useUnscaledTime: true)
			.Group(Tween.Custom(target: this, _progress, 1, _tweenDuration, (target, val) => _progress = val))
			.Group(
				Tween.Custom(
					target: this,
					_progress,
					1,
					_tweenDuration,
					(target, val) => target._contentCanvasGroup.alpha = val
				)
			)
			.Group(
				Tween.Custom(
					target: this,
					_progress,
					1,
					_tweenDuration,
					(target, val) => target._background.material.SetFloat("_Progress", val)
				)
			);
	}

	// Plays menu close transitions and resumes the game time
	private void ClosePauseMenu()
	{
		GameManager.Instance.UnfreezeTime();
		_pauseMenuCanvas.SetActive(true);
		_transitionSequence.Stop();
		DisableButtons();
		_transitionSequence = Sequence
			.Create(useUnscaledTime: true)
			.Group(Tween.Custom(target: this, _progress, 0, _tweenDuration, (target, val) => _progress = val))
			.Group(
				Tween.Custom(
					target: this,
					_progress,
					0,
					_tweenDuration,
					(target, val) => target._contentCanvasGroup.alpha = val
				)
			)
			.Group(
				Tween.Custom(
					target: this,
					_progress,
					0,
					_tweenDuration,
					(target, val) => target._background.material.SetFloat("_Progress", val)
				)
			)
			.ChainCallback(() => _pauseMenuCanvas.SetActive(false));
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
		TogglePauseMenu();
	}

	// Opens the settings menu when button is pressed
	public void OnSettingsButtonPressed()
	{
		// TODO
	}

	// Closes the game when button is pressed
	public void OnExitButtonPressed()
	{
		GameManager.Instance.ExitGame();
	}
}
