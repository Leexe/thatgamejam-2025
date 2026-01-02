using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
	[Title("Event Channel (Static Entities)")]
	[Tooltip("Use to toggle event channel mode and off for direct reference")]
	[SerializeField]
	private bool _toggleEventChannel;

	[SerializeField]
	[ShowIf("@_toggleEventChannel")]
	private HealthEventChannelSO _channel;

	[SerializeField]
	[HideIf("_toggleEventChannel")]
	private HealthController _directController;

	[Title("UI References")]
	[SerializeField]
	private Image _fillImage;

	private void OnValidate()
	{
		if (_toggleEventChannel && _directController != null)
		{
			_directController = null;
		}

		if (!_toggleEventChannel && _channel != null)
		{
			_channel = null;
		}
	}

	private void OnEnable()
	{
		if (_channel != null)
		{
			_channel.OnHealthChanged += OnChannelHealthChanged;
		}
	}

	private void OnDisable()
	{
		if (_channel != null)
		{
			_channel.OnHealthChanged -= OnChannelHealthChanged;
		}

		if (_directController != null)
		{
			_directController.OnHealthChanged.RemoveListener(OnHealthChanged);
			_directController.OnDeath.RemoveListener(OnDeath);
			_directController = null;
		}
	}

	/// <summary>
	/// Initialize the health bar with a direct reference to a HealthController.
	/// </summary>
	public void Initialize(HealthController controller)
	{
		// Clean up previous subscription if any
		if (_directController != null)
		{
			_directController.OnHealthChanged.RemoveListener(OnHealthChanged);
			_directController.OnDeath.RemoveListener(OnDeath);
		}

		_directController = controller;

		if (_directController != null)
		{
			_directController.OnHealthChanged.AddListener(OnHealthChanged);
			_directController.OnDeath.AddListener(OnDeath);

			UpdateHealthBar(_directController.Health, _directController.MaxHealth);
		}
	}

	private void OnHealthChanged(float delta, float currentHealth, float maxHealth)
	{
		UpdateHealthBar(currentHealth, maxHealth);
	}

	private void OnChannelHealthChanged(float delta, float currentHealth, float maxHealth)
	{
		UpdateHealthBar(currentHealth, maxHealth);
	}

	private void OnDeath()
	{
		gameObject.SetActive(false);
	}

	private void UpdateHealthBar(float currentHealth, float maxHealth)
	{
		if (maxHealth > 0)
		{
			_fillImage.fillAmount = currentHealth / maxHealth;
		}
	}
}
