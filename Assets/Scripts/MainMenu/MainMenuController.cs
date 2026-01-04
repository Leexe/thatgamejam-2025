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

	// Private Fields
	private AnimancerState _activeState;
	private bool _canSkipAnimation;

	private void Start()
	{
		InputManager.Instance.OnAnyInputPerformed.AddListener(SkipAnimation);
		OnIntroEnd.AddListener(ResetAnimatorSpeed);
		OnPlayEnd.AddListener(() => GameManager.Instance.SwitchScenes(GameManager.SceneNames.FightingGame));
		PlayIntroAnimation();
	}

	private void OnDestroy()
	{
		if (InputManager.Instance != null)
		{
			InputManager.Instance.OnAnyInputPerformed.RemoveListener(SkipAnimation);
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
			};

			OnPlayStart?.Invoke();
			foreach (SwayingUI ui in _swayingUIList)
			{
				ui.TweenBackToDefault(1.25f);
			}

			_canSkipAnimation = false;
		}
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
		AudioManager.Instance.PlayOneShot(FMODEvents.Instance.ButtonClick_Sfx);
		PlayStartAnimation();
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

	public void ChangeToGameScene() { }

	#endregion
}
