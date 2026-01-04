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

	private void Start()
	{
		foreach (Image image in _images)
		{
			image.sprite = _neutralIndicator;
		}
	}

	private void UpdateDisplay(int wins)
	{
		for (int i = 0; i < _images.Count || i < wins; i++)
		{
			_images[i].sprite = _winIndicator;
		}
	}
}
