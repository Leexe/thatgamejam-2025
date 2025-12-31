using UnityEngine;

/// <summary>
/// A scriptable object that contains data SPECIFIC to the <c>BasicFighter</c> fighter class.
/// </summary>
[CreateAssetMenu(fileName = "BasicFighterSpritesSO", menuName = "FighterSprites/BasicFighter")]
public class BasicFighterSpritesSO : ScriptableObject
{
	[Header("Parameters")]
	[Tooltip("X Offset of the sprite relative to the Fighter's <c>Position</c> attribute.")]
	public float XOffset = 0f;

	[Tooltip("Y Offset of the sprite relative to the Fighter's <c>Position</c> attribute.")]
	public float YOffset = 0f;

	[Header("Sprites")]
	public Sprite[] CrouchBlock;

	public Sprite[] CrouchHurt;

	public Sprite[] CrouchIdle;

	public Sprite[] CrouchKick;

	public Sprite[] CrouchPunch;

	public Sprite[] CrouchTransition;

	public Sprite[] Grab;

	public Sprite[] Jump;

	public Sprite[] JumpHurt;

	public Sprite[] JumpKick;

	public Sprite[] JumpPunch;

	public Sprite[] KnockDownGetUp;

	public Sprite[] StandBlock;

	public Sprite[] StandHurt;

	public Sprite[] StandIdle;

	public Sprite[] StandKick;

	public Sprite[] StandPunch;

	public Sprite[] Walk;
}
