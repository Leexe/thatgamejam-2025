using UnityEngine;

public class Hitbox : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	private void Awake()
	{
		_spriteRenderer.color = new Color(0f, 0f, 0f, 0f);
	}

	public Rect GetBoundsAsRect()
	{
		if (!gameObject.activeSelf)
		{
			return new(0f, 0f, 0f, 0f);
		}

		return new()
		{
			width = Mathf.Abs(transform.lossyScale.x),
			height = Mathf.Abs(transform.lossyScale.y),
			center = new(transform.position.x, transform.position.y),
		};
	}
}
