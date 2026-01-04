using UnityEngine;

/// <summary>
/// The base class that visuals in the game should inherit from.
/// </summary>
public class AfterImage : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer _spriteRenderer;

	[SerializeField]
	private int _lifespan = 30;

	[SerializeField]
	private float _startingOpacity = 0.2f;

	//

	private bool _started = false;
	private int _age;

	public void Show(Vector2 position, Sprite sprite)
	{
		transform.position = position;
		_spriteRenderer.sprite = sprite;
		_started = true;
		_age = 0;
	}

	// a bit hacky. this assumes that the simulation runs on FixedUpdate().
	// for better design: supply a Tick() function that FightManager calls.
	private void FixedUpdate()
	{
		if (!_started)
		{
			return;
		}

		_spriteRenderer.color = new Color(1f, 1f, 1f, _startingOpacity * (1f - ((float)_age / _lifespan)));
		transform.Translate(Vector3.forward * 0.1f); // do this to ensure a solid sorting order
		_age++;

		if (_age == _lifespan)
		{
			Destroy(gameObject);
		}
	}
}
