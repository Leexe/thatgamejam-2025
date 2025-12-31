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
	public int MaxHealth { get; protected set; }

	public Vector2 Position { get; set; }
	public HitBoxData HitBoxes { get; protected set; }

	/// <summary>
	/// Sets up starting position and state of player. Called at start of fight
	/// </summary>
	public abstract void Init(Vector2 position, Vector2 opponentPosition);

	/// <summary>
	/// Cleans stuff up.
	/// </summary>
	public abstract void Destroy();

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
	/// TODO: Called by FightManager when this fighter is hit by a grab
	/// </summary>
	public abstract AttackResult OnHitByGrab(AttackInfo attack);

	/// <summary>
	/// Called by FightManager when the attack made contact.
	/// </summary>
	public abstract void OnAttackLanded();

	/// <summary>
	/// Called by FightManager when fighters should update visuals.
	/// </summary>
	public abstract void UpdateVisuals();

	public virtual void DrawGizmos()
	{
		// position
		Gizmos.color = Color.white;
		GizmoUtils.DrawCrossHair(Position);

		// collision box
		Gizmos.color = Color.limeGreen;
		GizmoUtils.DrawRect(RectUtils.TranslateRect(HitBoxes.CollisionBox, Position));

		// hitboxes
		Gizmos.color = Color.blue;
		foreach (Rect hurtBox in HitBoxes.HurtBoxes)
		{
			GizmoUtils.DrawRect(RectUtils.TranslateRect(hurtBox, Position));
		}
		Gizmos.color = Color.red;
		if (HitBoxes.Attack.HasValue)
		{
			GizmoUtils.DrawRect(RectUtils.TranslateRect(HitBoxes.Attack.Value.Bounds, Position));
		}
	}
}
