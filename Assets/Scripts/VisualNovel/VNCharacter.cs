using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class VNCharacter : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private Image _image;

	[SerializeField]
	private Image _tempImage;

	[SerializeField]
	private LayoutElement _layoutElement;

	// Private Variables
	private Tween _positionTween;
	private Tween _alphaTween;
	private Tween _shakeTween;
	private Tween _scaleTween;

	private void OnDisable()
	{
		_positionTween.Stop();
		_alphaTween.Stop();
		_shakeTween.Stop();
		_scaleTween.Stop();
	}

	public void FadeIn(float fadeDuration = 1f)
	{
		if (fadeDuration == 0f)
		{
			SetTransparency(1f);
		}
		else
		{
			_alphaTween.Complete();
			_alphaTween = Tween.Alpha(_image, 1f, fadeDuration);
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
			_alphaTween.Complete();
			_alphaTween = Tween.Alpha(_image, 0f, fadeDuration);
		}
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

	/// <summary>
	/// Shakes the character sprite horizontally.
	/// </summary>
	/// <param name="shakeOffset">Offset for the shake effect.</param>
	/// <param name="duration">Duration of the shake animation.</param>
	public void ShakeHorizontal(float shakeOffset = 10f, float duration = 1f)
	{
		_shakeTween.Complete();
		_layoutElement.ignoreLayout = true;
		_shakeTween = Tween
			.ShakeLocalPosition(
				_image.transform,
				new Vector3(shakeOffset, 0f, 0f),
				duration,
				frequency: 10,
				easeBetweenShakes: Ease.Default
			)
			.OnComplete(() => _layoutElement.ignoreLayout = false);
	}

	/// <summary>
	/// Shakes the character sprite vertically.
	/// </summary>
	/// <param name="shakeOffset">Offset for the shake effect.</param>
	/// <param name="duration">Duration of the shake animation.</param>
	public void ShakeVertical(float shakeOffset = 10f, float duration = 1f)
	{
		_shakeTween.Complete();
		_layoutElement.ignoreLayout = true;
		_shakeTween = Tween
			.ShakeLocalPosition(
				_image.transform,
				new Vector3(0f, shakeOffset, 0f),
				duration,
				frequency: 10,
				easeBetweenShakes: Ease.Default
			)
			.OnComplete(() => _layoutElement.ignoreLayout = false);
	}

	/// <summary>
	/// Punches the character sprite scale.
	/// </summary>
	/// <param name="punchStrength">Strength of the punch (how much bigger it gets).</param>
	/// <param name="duration">Duration of the punch animation.</param>
	public void Punch(float punchStrength = 0.1f, float duration = 0.5f)
	{
		_scaleTween.Complete();
		_scaleTween = Tween.PunchScale(_image.transform, new Vector3(punchStrength, punchStrength, 0f), duration);
	}

	/// <summary>
	/// Cross-fades the character sprite with a new sprite.
	/// </summary>
	/// <param name="newSprite">The new sprite to cross-fade to.</param>
	/// <param name="duration">Duration of the cross-fade animation.</param>
	public void CrossFadeSprite(Sprite newSprite, float duration = 1f)
	{
		// Set the temp image to be the new sprite
		_tempImage.sprite = newSprite;
		_tempImage.gameObject.SetActive(true);

		// Set the temp image to be transparent
		Color startColor = _image.color;
		startColor.a = 0f;
		_tempImage.color = startColor;
		_tempImage.preserveAspect = _image.preserveAspect;

		Tween
			.Alpha(_tempImage, 1f, duration)
			.OnComplete(() =>
			{
				_image.sprite = newSprite;
				_tempImage.sprite = null;
				_tempImage.gameObject.SetActive(false);
			});
	}

	/// <summary>
	/// Fades out the character sprite and destroys it.
	/// </summary>
	/// <param name="fadeDuration">Duration of the fade animation.</param>
	public void FadeOutAndDestroy(float fadeDuration = 1f)
	{
		if (fadeDuration == 0)
		{
			Destroy(gameObject);
		}
		else
		{
			_alphaTween.Complete();
			_alphaTween = Tween.Alpha(_image, 0f, fadeDuration).OnComplete(() => Destroy(gameObject));
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
