using System.Collections.Generic;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
	[Title("References")]
	[SerializeField]
	private AnimancerComponent _animancer;

	[SerializeField]
	private List<SwayingUI> _swayingUIList = new();

	[Title("Animations")]
	[SerializeField]
	private AnimationClip _introAnimation;

	[SerializeField]
	private AnimationClip _startAnimation;

	[SerializeField]
	private AnimationClip _transitionAnimation;

	[Title("Animation Settings")]
	[SerializeField]
	private float _skipSpeed = 2.5f;

	[HideInInspector]
	public UnityEngine.Events.UnityEvent OnIntroStart;

	[HideInInspector]
	public UnityEngine.Events.UnityEvent OnIntroEnd;

	[HideInInspector]
	public UnityEngine.Events.UnityEvent OnPlayStart;

	[HideInInspector]
	public UnityEngine.Events.UnityEvent OnPlayEnd;

	[HideInInspector]
	public UnityEngine.Events.UnityEvent OnTransitionStart;

	[HideInInspector]
	public UnityEngine.Events.UnityEvent OnTransitionEnd;

	// Private Fields
	private AnimancerState _activeState;
	private bool _canSkipAnimation;
	private bool _isSceneReady;
	private bool _isStartAnimationFinished;

	private void Start()
	{
		InputManager.Instance.OnAnyInputPerformed.AddListener(SkipAnimation);
		GameManager.Instance.OnSceneReady.AddListener(OnScenePreloaded);
		OnIntroEnd.AddListener(ResetAnimatorSpeed);
		PlayIntroAnimation();
	}

	private void OnDestroy()
	{
		if (InputManager.Instance != null)
		{
			InputManager.Instance.OnAnyInputPerformed.RemoveListener(SkipAnimation);
		}

		if (GameManager.Instance != null)
		{
			GameManager.Instance.OnSceneReady.RemoveListener(OnScenePreloaded);
		}
	}

	#region Animation

	private void PlayIntroAnimation()
	{
		if (_introAnimation != null)
		{
			_activeState = _animancer.Play(_introAnimation);
			_activeState.Events(this).OnEnd = () =>
			{
				OnIntroEnd?.Invoke();
				_activeState.Stop();
				_activeState = null;
			};

			OnIntroStart?.Invoke();
			_canSkipAnimation = true;
		}
	}

	private void PlayStartAnimation()
	{
		if (_startAnimation != null)
		{
			_activeState = _animancer.Play(_startAnimation);
			_activeState.Events(this).OnEnd = () =>
			{
				OnPlayEnd?.Invoke();
				_activeState.Stop();
				_activeState = null;
				_isStartAnimationFinished = true;
				TryPlayTransition();
			};

			OnPlayStart?.Invoke();
			foreach (SwayingUI ui in _swayingUIList)
			{
				ui.TweenBackToDefault(1.25f);
			}

			_canSkipAnimation = false;
		}
	}

	private void PlayTransitionAnimation()
	{
		if (_transitionAnimation != null)
		{
			_activeState = _animancer.Play(_transitionAnimation);
			_activeState.Events(this).OnEnd = () =>
			{
				OnTransitionEnd?.Invoke();
				_activeState.Stop();
				ChangeToGameScene();
				_activeState = null;
			};

			OnTransitionStart?.Invoke();
		}
	}

	private void TryPlayTransition()
	{
		if (_isSceneReady && _isStartAnimationFinished)
		{
			PlayTransitionAnimation();
		}
	}

	private void OnScenePreloaded()
	{
		_isSceneReady = true;
		TryPlayTransition();
	}

	#endregion

	#region Animation Helpers

	private void SkipAnimation()
	{
		if (_canSkipAnimation && _activeState is { IsPlaying: true })
		{
			_activeState.Speed = _skipSpeed;
			_canSkipAnimation = false;
		}
	}

	private void ResetAnimatorSpeed()
	{
		if (_activeState != null)
		{
			_activeState.Speed = 1f;
		}
	}

	#endregion

	#region Button Callbacks

	public void StartButton()
	{
		_isSceneReady = false;
		_isStartAnimationFinished = false;

		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick_Sfx);
		PlayStartAnimation();
		GameManager.Instance.LoadSceneAsync(GameManager.SceneNames.FightingGame);
	}

	public void SettingsButton()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick_Sfx);
	}

	public void ExitButton()
	{
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick_Sfx);
		GameManager.Instance.ExitGame();
	}

	private void ChangeToGameScene()
	{
		GameManager.Instance.ActivatePreloadedScene();
	}

	#endregion
}
