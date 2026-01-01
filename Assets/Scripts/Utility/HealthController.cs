using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour
{
	[Header("Max Health")]
	[Tooltip("How much health the player has")]
	[SerializeField]
	private float _maxHealth = 100f;

	[Header("Regeneration")]
	[Tooltip("Allows the player to regenerate health over time")]
	[SerializeField]
	private bool _toggleRegeneration;

	[Tooltip("How much health the player heals per second")]
	[ShowIf("_toggleRegeneration")]
	[SerializeField]
	private float _baseRegen = 1f;

	[Tooltip("How long after being hit does regeneration start")]
	[ShowIf("_toggleRegeneration")]
	[SerializeField]
	private float _regenerationDelay;

	[Header("Invincibility")]
	[Tooltip("How long the i-frames last after getting damaged")]
	[SerializeField]
	private float _invincibilityTime = 1f;

	private float _timeSinceHurt;
	private float _regeneration;
	private float _health;
	private bool _isDead;
	private bool CanTakeDamage => _timeSinceHurt > _invincibilityTime;
	private bool CanRegenerate => _timeSinceHurt > _regenerationDelay;

	// Events
	[HideInInspector]
	public UnityEvent<float, float> OnInitiateHealth; // Parameters: health, maxHealth

	[HideInInspector]
	public UnityEvent<float, float> OnRegen; // Parameters: health, maxHealth

	[HideInInspector]
	public UnityEvent<float, float> OnHeal; // Parameters: health, maxHealth

	[HideInInspector]
	public UnityEvent<float, float> OnDamage; // Parameters: health, maxHealth

	[HideInInspector]
	public UnityEvent<float, float> OnRevive; // Parameters: health, maxHealth

	[HideInInspector]
	public UnityEvent OnDeath;

	// Getters
	public bool IsAlive => !_isDead;
	public bool IsDead => _isDead;
	public bool IsFullHealth => _maxHealth <= _health;
	public float GetHealth => _health;
	public float GetMaxHealth => _maxHealth;
	public float GetNormalizedHealth => _health / _maxHealth;

	private void Start()
	{
		_health = _maxHealth;
		_regeneration = _baseRegen;
		OnInitiateHealth?.Invoke(_health, _maxHealth);
	}

	private void Update()
	{
		if (!_isDead)
		{
			HandleRegeneration(Time.deltaTime);
		}

		_timeSinceHurt += Time.deltaTime;
	}

	private void HandleRegeneration(float deltaTime)
	{
		if (_toggleRegeneration && CanRegenerate && !IsFullHealth && !_isDead)
		{
			HealHealth(_regeneration * deltaTime);
			OnRegen?.Invoke(_health, _maxHealth);
		}
	}

	private void HealHealth(float healing)
	{
		_health += healing;
		_health = Mathf.Min(_health, _maxHealth);
	}

	/// <summary>
	/// Make the player take the given amount of damage and mark the player is dead when health less than 0
	/// </summary>
	/// <param name="damage">Damage to take</param>
	public void TakeDamage(float damage)
	{
		// Don't take damage if the player has died
		if (!_isDead && CanTakeDamage)
		{
			_health -= damage;
			_timeSinceHurt = 0f;

			// If the player is below a certain threshold of health, trigger death
			if (_health <= 0.01f)
			{
				OnDamage?.Invoke(_health, _maxHealth);
				OnDeath?.Invoke();
				_health = 0f;
				_isDead = true;
			}
			// Else, damage them
			else
			{
				OnDamage?.Invoke(_health, _maxHealth);
			}
		}
	}

	/// <summary>
	/// Revives the player and sets their health to the health amount
	/// </summary>
	/// <param name="healthNormalized">A value from 0-1 that lerps between 0 and max health, healing the player for that amount</param>
	public void Revive(float healthNormalized = 1f)
	{
		_isDead = false;
		HealHealth(healthNormalized * _maxHealth);
		OnRevive?.Invoke(_health, _maxHealth);
	}

	/// <summary>
	/// Changes the regneration rate
	/// </summary>
	/// <param name="regenRate">Amount of healing per second</param>
	public void SetRegeneration(float regenRate)
	{
		_regeneration = regenRate;
	}

	/// <summary>
	/// Sets the player's health to a value between 0 and _maxHealth
	/// </summary>
	/// <param name="healthNormalized">A value from 0-1 that lerps between 0 and max health, setting the player's health to that amount</param>
	public void SetHealth(float healthNormalized = 1f)
	{
		_health = Mathf.Clamp(_maxHealth * healthNormalized, 0, _maxHealth);
	}
}
