using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class AttackData : MonoBehaviour
{
	[Header("References")]
	public Hitbox Hitbox;

	[Tooltip("Visual location of the attack. Useful for orienting VFX.")]
	public Transform VisualPoint;

	[Header("Parameters")]
	[Tooltip("Visual direction of the attack. Useful for orienting VFX.")]
	public Vector2 VisualDirection = Vector2.zero;
	public Direction Direction = Direction.None;
	public AttackType AttackType = AttackType.Mid;
	public bool IsGrab = false;
	public bool IsHeavy = false;
	public int Damage = 100;
}
