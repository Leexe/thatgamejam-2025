using UnityEngine;

[RequireComponent(typeof(Hitbox))]
public class AttackData : MonoBehaviour
{
	[Header("References")]
	public Hitbox Hitbox;

	[Header("Parameters")]
	[Tooltip("Visual Direction of the attack. Useful for orienting VFX.")]
	public Vector2 VisualDirection = Vector2.zero;
	public Direction Direction = Direction.None;
	public AttackType AttackType = AttackType.Mid;
	public bool IsGrab = false;
	public int Damage = 100;
}
