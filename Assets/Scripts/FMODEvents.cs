using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class FMODEvents : Singleton<FMODEvents>
{
	[field: SerializeField]
	[field: FoldoutGroup("Music", expanded: true)]
	public EventReference IceBossa_Bgm { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("Music", expanded: true)]
	public EventReference BloodMoon_Bgm { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("Music", expanded: true)]
	public EventReference Roam_Bgm { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("Ambience", expanded: true)]
	public EventReference Ambience_Amb { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference Bite_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference ButtonClick_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference Cast_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference Catch_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference LineBreak_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference Struggle_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference Buy_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference Sell_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference Discard_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference InventoryOpen_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("SFX", expanded: true)]
	public EventReference InventoryClose_Sfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("Loop SFX", expanded: true)]
	public EventReference Reel_LoopSfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("Loop SFX", expanded: true)]
	public EventReference Struggle_LoopSfx { get; private set; }

	[field: SerializeField]
	[field: FoldoutGroup("Loop SFX", expanded: true)]
	public EventReference MovingShip_LoopSfx { get; private set; }
}
