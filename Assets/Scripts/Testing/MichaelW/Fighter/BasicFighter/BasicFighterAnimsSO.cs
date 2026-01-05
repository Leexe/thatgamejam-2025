using UnityEngine;

/// <summary>
/// A scriptable object that contains animations SPECIFIC to the <c>BasicFighter</c> fighter class.
/// </summary>
[CreateAssetMenu(fileName = "BasicFighterAnims", menuName = "FighterAnims/BasicFighter")]
public class BasicFighterAnimsSO : ScriptableObject
{
	[Header("Animations")]
	public AnimationClip CrouchBlock;
	public AnimationClip CrouchHurt;
	public AnimationClip CrouchIdle;
	public AnimationClip CrouchKick;
	public AnimationClip CrouchPunch;
	public AnimationClip CrouchTransition;
	public AnimationClip DashBack;
	public AnimationClip DashForward;
	public AnimationClip Die;
	public AnimationClip Grab;
	public AnimationClip Grabbed;
	public AnimationClip GrabSuccess;
	public AnimationClip Jump;
	public AnimationClip JumpHurt;
	public AnimationClip JumpKick;
	public AnimationClip JumpPunch;
	public AnimationClip KnockDownGetUp;
	public AnimationClip StandBlock;
	public AnimationClip StandHurt;
	public AnimationClip StandIdle;
	public AnimationClip StandKick;
	public AnimationClip StandPunch;
	public AnimationClip Walk;
}
