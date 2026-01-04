using UnityEngine;

/// <summary>
/// The base class that all fighters in the game must inherit from.
///
/// <para>
/// All fighters must...
/// </para>
/// <list type="bullet">
/// <item>Report Health, Position, and Hitboxes (hurtbox, grab box, attack boxes)</item>
/// <item>Update Position and Hitboxes when instructed to do so each frame</item>
/// <item>Respond to attacks and grabs, returning if they hit or were blocked</item>
/// <item>Report and update visuals (what sprite is displayed)</item>
/// </list>
/// <para>
/// Beyond that, there is a lot of flexibility in how things are implemented -
/// most classes will use some state machine but that is not a requirement.
/// </para>
/// </summary>
public abstract class Fighter : MonoBehaviour
{
	public int Health { get; protected set; }

	public int MaxHealth = 1000;

	public abstract HitBoxData ReadHitBoxes();

	/// <summary>
	/// Sets up starting position and state of player. Called at start of fight
	/// </summary>
	public abstract void Init(Vector2 position, Vector2 opponentPosition);

	/// <summary>
	/// Cleans stuff up.
	/// </summary>
	public virtual void Destroy() { }

	/// <summary>
	/// Applies state changes and movement according to input
	/// </summary>
	public abstract void Tick(in InputInfo input, in Vector2 opponentPosition);

	/// <summary>
	/// Called by FightManager when this fighter is hit by an attack
	/// </summary>
	/// <param name="attack">The attack that hit</param>
	/// <returns>Whether the attack landed, was blocked, or neither</returns>
	public abstract AttackResult OnHitByAttack(AttackInfo attack);

	/// <summary>
	/// Called by FightManager when the attack made contact.
	/// </summary>
	public abstract void OnAttackLanded(AttackResult result);

	/// <summary>
	/// Called by FightManager after performing hitbox separation.
	/// </summary>
	public virtual void OnPostSeparate() { }

	public virtual void DrawGizmos()
	{
		HitBoxData hitboxes = ReadHitBoxes();

		// position
		Gizmos.color = Color.pink;
		GizmoUtils.DrawCrossHair(transform.position);

		// collision box
		Gizmos.color = Color.limeGreen;
		GizmoUtils.DrawRect(hitboxes.CollisionBox);

		// hitboxes
		Gizmos.color = Color.blue;
		foreach (Rect hurtBox in hitboxes.HurtBoxes)
		{
			GizmoUtils.DrawRect(hurtBox);
		}
		Gizmos.color = Color.red;
		if (hitboxes.Attack.HasValue)
		{
			GizmoUtils.DrawRect(hitboxes.Attack.Value.Bounds);
		}
	}
}
