using Animancer;
using UnityEngine;

/// <summary>
/// The base class that visuals in the game should inherit from.
/// </summary>
public class VFX : MonoBehaviour
{
	[SerializeField]
	private AnimancerComponent _animationController;

	//

	private AnimationClip _anim;
	private bool _started = false;
	private int _frame;
	private AnimancerState _animState;

	public void Play(AnimationClip animation)
	{
		_started = true;
		_anim = animation;
		_frame = 0;
		_animState = _animationController.Play(animation);
		_animState.IsPlaying = false;
		_animState.MoveTime(0f, false);
	}

	// a bit hacky. this assumes that the simulation runs on FixedUpdate().
	// for better design: supply a Tick() function that FightManager calls.
	private void FixedUpdate()
	{
		if (!_started)
		{
			return;
		}

		int animDuration = Mathf.FloorToInt((_anim.length * 60f) - 0.5f);
		_frame++;

		if (_frame == animDuration)
		{
			_animState.Stop();
			Destroy(gameObject);
		}
		else
		{
			_animState.MoveTime(_frame * (1f / 60f), false);
		}
	}
}
