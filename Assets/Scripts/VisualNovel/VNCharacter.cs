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
	/// Animates this character sprite to a new parent layout group.
	/// </summary>
	/// <param name="newParent">The new parent transform (layout group) to move to.</param>
	/// <param name="tweenDuration">Duration of the movement animation.</param>
	public void TweenToParent(Transform newParent, float tweenDuration = 1f)
	{
		// Stop any existing position tween
		_positionTween.Stop();

		// Save current position before reparenting
		Vector3 oldPosition = transform.position;

		// Reparent and force layout to calculate target position
		_layoutElement.ignoreLayout = false;
		transform.SetParent(newParent);
		LayoutRebuilder.ForceRebuildLayoutImmediate(newParent as RectTransform);
		Vector3 newPosition = transform.position;

		// Reset to old position and tween to new position
		_layoutElement.ignoreLayout = true;
		transform.SetPositionAndRotation(oldPosition, transform.rotation);
		_positionTween = Tween
			.Position(transform, newPosition, tweenDuration)
			.OnComplete(() => _layoutElement.ignoreLayout = false);
	}

	/// <summary>
	/// Animates this character sprite from its old position to it's new layout position.
	/// </summary>
	/// <param name="oldPosition">The original position before snapping to a layout group.</param>
	/// <param name="tweenDuration">Duration of the movement animation.</param>
	public void TweenFromParent(Vector3 oldPosition, float tweenDuration = 1f)
	{
		// Stop any existing position tween
		_positionTween.Stop();

		// Get the target position from layout
		_layoutElement.ignoreLayout = false;
		LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent as RectTransform);
		Vector3 newPosition = transform.position;

		// Reset to old position and tween to new position
		_layoutElement.ignoreLayout = true;
		transform.position = oldPosition;
		_positionTween = Tween
			.Position(transform, newPosition, tweenDuration)
			.OnComplete(() => _layoutElement.ignoreLayout = false);
	}

	public void FadeIn(float fadeDuration = 1f)
	{
		if (fadeDuration == 0f)
		{
			SetTransparency(0f);
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

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
