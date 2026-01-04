using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundIndicatorUI : MonoBehaviour
{
	[SerializeField]
	private Sprite _winIndicator;

	[SerializeField]
	private Sprite _neutralIndicator;

	[SerializeField]
	private List<Image> _images;

	private void Awake()
	{
		foreach (Image image in _images)
		{
			image.sprite = _neutralIndicator;
		}
	}

	public void UpdateDisplay(int wins)
	{
		for (int i = 0; i < _images.Count; i++)
		{
			_images[i].sprite = i < wins ? _winIndicator : _neutralIndicator;
		}
	}
}
