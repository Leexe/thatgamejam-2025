using System.Collections.Generic;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class VisualNovelDictionary : SerializedScriptableObject
{
  public readonly Dictionary<string, EventReference> MusicMap;
  public readonly Dictionary<string, EventReference> SfxMap;
  public readonly Dictionary<string, Sprite> CharacterSpriteMap;
}