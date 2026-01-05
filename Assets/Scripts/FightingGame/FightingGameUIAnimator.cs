using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

public class FightingGameUIAnimator : MonoBehaviour
{
	[Title("References")]
	[SerializeField]
	private AnimancerComponent _animancer;

	[Title("Animations")]
	[SerializeField]
	private AnimationClip _introAnimation;

	[HideInInspector]
	public UnityEvent OnIntroStart;

	[HideInInspector]
	public UnityEvent OnIntroEnd;

	private AnimancerState _activeState;

	private void Start()
	{
		PlayIntroAnimation();
	}

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
		}
	}
}
