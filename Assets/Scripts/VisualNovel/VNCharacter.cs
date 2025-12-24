using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class VNCharacter : MonoBehaviour
{
	[SerializeField]
	private Image _image;

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

	public void Destroy()
	{
		Destroy(gameObject);
	}
}
