using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class HealthController : MonoBehaviour, IDamageable
{
	#region Settings
	[Header("Max Health")]
	[Tooltip("How much health the player has")]
	[SerializeField]
	private float _maxHealth = 100f;

	[Header("Debug")]
	[SerializeField]
	private bool _godMode;

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

	#endregion

	#region State

	private float _timeSinceHurt;
	private float _regeneration;
	private float _health;
	private bool _isDead;
	private bool CanTakeDamage => _timeSinceHurt > _invincibilityTime && !_godMode;
	private bool CanRegenerate => _timeSinceHurt > _regenerationDelay;

	#endregion

	#region Events

	// Events

	/// <summary>
	/// Event triggered when health is initialized. Parameters: currentHealth, maxHealth
	/// </summary>
	[HideInInspector]
	public UnityEvent<float, float> OnInitiateHealth;

	/// <summary>
	/// Event triggered when health is regenerated. Parameters: delta (amount regenerated), currentHealth, maxHealth
	/// </summary>
	[HideInInspector]
	public UnityEvent<float, float, float> OnRegen;

	/// <summary>
	/// Event triggered when entity is healed. Parameters: delta (amount healed), currentHealth, maxHealth
	/// </summary>
	[HideInInspector]
	public UnityEvent<float, float, float> OnHeal;

	/// <summary>
	/// Event triggered when entity takes damage. Parameters: delta (damage taken), currentHealth, maxHealth
	/// </summary>
	[HideInInspector]
	public UnityEvent<float, float, float> OnDamage;

	/// <summary>
	/// Event triggered when entity is revived. Parameters: currentHealth, maxHealth
	/// </summary>
	[HideInInspector]
	public UnityEvent<float, float> OnRevive;

	/// <summary>
	/// Event triggered when entity dies.
	/// </summary>
	[HideInInspector]
	public UnityEvent OnDeath;

	/// <summary>
	/// Event triggered when health changes. Parameters: delta (change amount), currentHealth, maxHealth
	/// </summary>
	[HideInInspector]
	public UnityEvent<float, float, float> OnHealthChanged;

	#endregion

	#region Properties

	// Getters
	public bool IsAlive => !_isDead;
	public bool IsDead => _isDead;
	public bool IsFullHealth => _health >= _maxHealth;
	public float Health => _health;
	public float MaxHealth => _maxHealth;
	public float GetNormalizedHealth => _maxHealth > 0 ? _health / _maxHealth : 0;

	#endregion

	#region Initialization

	private void Start()
	{
		InitializeHealth();
	}

	private void InitializeHealth()
	{
		_health = _maxHealth;
		_regeneration = _baseRegen;
		_isDead = false;
		OnInitiateHealth?.Invoke(_health, _maxHealth);
		OnHealthChanged?.Invoke(0, _health, _maxHealth);
	}

	#endregion

	#region Logic

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
			float amount = _regeneration * deltaTime;
			Heal(amount);
			OnRegen?.Invoke(amount, _health, _maxHealth);
		}
	}

	#endregion

	#region Public API

	/// <summary>
	/// Heals the entity by the specified amount
	/// </summary>
	/// <param name="amount">Amount to heal.</param>
	[Button("Heal")]
	public void Heal(float amount)
	{
		if (_isDead)
		{
			return;
		}

		float previousHealth = _health;
		_health += amount;
		_health = Mathf.Min(_health, _maxHealth);

		float diff = _health - previousHealth;
		if (diff > 0)
		{
			OnHeal?.Invoke(diff, _health, _maxHealth);
			OnHealthChanged?.Invoke(diff, _health, _maxHealth);
		}
	}

	/// <summary>
	/// Make the player take the given amount of damage and mark the player as dead when health is less than or equal to 0
	/// </summary>
	/// <param name="damage">Damage to take</param>
	[Button("Take Damage")]
	public void TakeDamage(float damage)
	{
		if (_godMode)
		{
			return;
		}

		// Don't take damage if the player has died
		if (!_isDead && CanTakeDamage)
		{
			_health -= damage;
			_timeSinceHurt = 0f;

			OnDamage?.Invoke(damage, _health, _maxHealth);
			OnHealthChanged?.Invoke(-damage, _health, _maxHealth);

			// If the player is below a certain threshold of health, trigger death
			if (_health <= 0.01f)
			{
				Kill();
			}
		}
	}

	[Button("Kill")]
	public void Kill()
	{
		if (_isDead)
		{
			return;
		}

		float previousHealth = _health;
		_health = 0f;
		_isDead = true;

		OnDeath?.Invoke();
		OnHealthChanged?.Invoke(_health - previousHealth, _health, _maxHealth);
	}

	/// <summary>
	/// Revives the player and sets their health to the normalized health amount
	/// </summary>
	/// <param name="healthNormalized">A value from 0-1 that lerps between 0 and max health, setting the player's health to that amount.</param>
	[Button("Revive")]
	public void Revive(float healthNormalized = 1f)
	{
		_isDead = false;
		float healAmount = healthNormalized * _maxHealth;
		_health = Mathf.Clamp(healAmount, 0, _maxHealth);

		OnRevive?.Invoke(_health, _maxHealth);
		OnHealthChanged?.Invoke(healAmount, _health, _maxHealth); // Approximation of change
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
	/// Sets the player's health based on the normalized value
	/// </summary>
	/// <param name="healthNormalized">A value from 0-1 that lerps between 0 and max health, setting the player's health to that amount</param>
	public void SetHealth(float healthNormalized = 1f)
	{
		float previousHealth = _health;
		_health = Mathf.Clamp(_maxHealth * healthNormalized, 0, _maxHealth);
		float diff = _health - previousHealth;

		OnHealthChanged?.Invoke(diff, _health, _maxHealth);
	}

	/// <summary>
	/// Sets the maximum health of the entity
	/// </summary>
	/// <param name="newMax">The new value for maximum health.</param>
	/// <param name="healToFull">If true, sets current health to current max health.</param>
	[Button("Set Max Health")]
	public void SetMaxHealth(float newMax, bool healToFull = false)
	{
		_maxHealth = newMax;
		if (healToFull)
		{
			_health = _maxHealth;
		}
		else
		{
			// Clamp current health if it exceeds new max
			if (_health > _maxHealth)
			{
				_health = _maxHealth;
			}
		}

		OnHealthChanged?.Invoke(0, _health, _maxHealth);
		OnInitiateHealth?.Invoke(_health, _maxHealth);
	}

	#endregion
}
