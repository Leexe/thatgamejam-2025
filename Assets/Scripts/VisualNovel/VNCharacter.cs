using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class VNCharacter : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private Image _image;

	[SerializeField]
	private LayoutElement _layoutElement;

	// Private Variables
	private Tween _positionTween;

	private void OnDisable()
	{
		_positionTween.Stop();
	}

	/// <summary>
	/// Animates this character sprite from its old position to a target position.
	/// </summary>
	/// <param name="oldPosition">The original position before the layout change.</param>
	/// <param name="targetPosition">The target position to animate to.</param>
	/// <param name="tweenDuration">Duration of the movement animation.</param>
	public void TweenPositions(Vector3 oldPosition, Vector3 targetPosition, float tweenDuration = 1f)
	{
		if (oldPosition == targetPosition)
		{
			return;
		}

		// Complete any existing position tween
		_positionTween.Complete();

		// Ignore layout during animation
		_layoutElement.ignoreLayout = true;
		transform.position = oldPosition;
		_positionTween = Tween
			.Position(transform, targetPosition, tweenDuration)
			.OnComplete(() => _layoutElement.ignoreLayout = false);
	}

	public void FadeIn(float fadeDuration = 1f)
	{
		if (fadeDuration == 0f)
		{
			SetTransparency(1f);
		}
		else
		{
			Tween.Alpha(_image, 1f, fadeDuration);
		}
	}

	public void FadeOut(float fadeDuration = 1f)
	{
		if (fadeDuration == 0f)
		{
			SetTransparency(0f);
		}
		else
		{
			Tween.Alpha(_image, 0f, fadeDuration);
		}
	}

	/// <summary>
	/// Animates the character sliding in from the side while fading in.
	/// </summary>
	/// <param name="slideOffset">Horizontal offset to slide from (positive = from right, negative = from left).</param>
	/// <param name="duration">Duration of the slide and fade animation.</param>
	public void SlideIn(float slideOffset, float duration = 1f)
	{
		// Complete any existing position tween
		_positionTween.Complete();

		// Store the target position (current layout position)
		Vector3 targetPosition = transform.position;

		// Set starting position with offset
		Vector3 startPosition = targetPosition + new Vector3(slideOffset, 0f, 0f);
		transform.position = startPosition;

		// Ignore layout during animation
		_layoutElement.ignoreLayout = true;

		// Start combined slide and fade animations
		_positionTween = Tween
			.Position(transform, targetPosition, duration, Ease.OutCubic)
			.OnComplete(() => _layoutElement.ignoreLayout = false);
	}

	/// <summary>
	/// Animates the character sliding out from the current position.
	/// </summary>
	/// <param name="slideOffset">Horizontal offset to slide to.</param>
	/// <param name="duration">Duration of the slide animation.</param>
	public void SlideOut(float slideOffset, float duration = 1f)
	{
		// Complete any existing position tween
		_positionTween.Complete();

		// Ignore layout during animation
		_layoutElement.ignoreLayout = true;

		// Move to target
		Vector3 targetPosition = transform.position + new Vector3(slideOffset, 0f, 0f);

		_positionTween = Tween.Position(transform, targetPosition, duration, Ease.InCubic);
	}

	public void FadeOutAndDestroy(float fadeDuration = 1f)
	{
		if (fadeDuration == 0)
		{
			Destroy(gameObject);
		}
		else
		{
			Tween.Alpha(_image, 0f, fadeDuration).OnComplete(() => Destroy(gameObject));
		}
	}

	public void ChangeObjectName(string name)
	{
		this.name = name;
	}

	public void SwitchSprite(Sprite sprite)
	{
		_image.sprite = sprite;
	}

	public void SetTransparency(float alpha)
	{
		Color color = _image.color;
		color.a = alpha;
		_image.color = color;
	}

	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public void SetParent(Transform newParent)
	{
		transform.SetParent(newParent);
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
