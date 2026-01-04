using System.Diagnostics.CodeAnalysis;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "Odin.OdinUnknownGroupingPath")]
public class FMODEvents : MonoSingleton<FMODEvents>
{
	#region Music

	[field: SerializeField]
	[field: FoldoutGroup("Music", true)]
	public EventReference Title_Bgm { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("Music", true)]
	public EventReference FightingGame_Bgm { get; private set; }

	#endregion

	#region Ambience

	// [field: SerializeField]
	// [field: FoldoutGroup("Ambience", expanded: true)]
	// public EventReference Ambience_Amb { get; private set; }

	#endregion

	#region SFX

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference Jump_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference KickLand_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference Kick_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference PunchLand_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference Punch_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference Death_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference Block_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference Dash_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference TitleDap_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference TitleReveal_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference TitleJingle_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference GameStart_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference ButtonHover_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX")]
	public EventReference ButtonClick_Sfx { get; private set; }

	#endregion

	#region Looping SFX

	// Loop SFX

	// [field: SerializeField]
	// [field: FoldoutGroup("Loop SFX", expanded: true)]
	// public EventReference Reel_LoopSFX { get; private set; }

	#endregion
}
