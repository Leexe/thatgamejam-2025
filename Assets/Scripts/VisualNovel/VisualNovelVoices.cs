using Febucci.TextAnimatorForUnity;
using Sirenix.OdinInspector;
using UnityEngine;

public class VisualNovelVoices : MonoBehaviour
{
	[FoldoutGroup("References")]
	[SerializeField]
	private TypewriterComponent _typewriter;

	[FoldoutGroup("References")]
	[SerializeField]
	private VisualNovelDictionarySO _vnDictionary;

	// Private Variables
	private bool _charactersTalking = false;
	private VoiceSO _currentCharacterVoice;

	private void OnEnable()
	{
		_typewriter.onCharacterVisible.AddListener(PlayVoice);
		GameManager.Instance.DialogueEventsRef.OnStartDialogue += ResetVoiceState;
		GameManager.Instance.DialogueEventsRef.OnDialogueVoice += SetSpeakingCharacter;
	}

	private void OnDisable()
	{
		_typewriter.onCharacterVisible.RemoveListener(PlayVoice);
		if (GameManager.Instance)
		{
			GameManager.Instance.DialogueEventsRef.OnStartDialogue -= ResetVoiceState;
			GameManager.Instance.DialogueEventsRef.OnDialogueVoice -= SetSpeakingCharacter;
		}
	}

	/// <summary>
	/// Resets the voice state at the start of each new line.
	/// Called before tags are processed.
	/// </summary>
	private void ResetVoiceState()
	{
		if (_currentCharacterVoice)
		{
			_currentCharacterVoice.ResetCount();
		}
		_charactersTalking = false;
		_currentCharacterVoice = null;
	}

	/// <summary>
	/// Called when a #d_characterName tag is encountered.
	/// Enables voice playback for this line.
	/// </summary>
	private void SetSpeakingCharacter(string characterName)
	{
		_charactersTalking = true;
		if (_vnDictionary.TryGetVoice(characterName, out VoiceSO voice))
		{
			_currentCharacterVoice = voice;
		}
	}

	/// <summary>
	/// Plays a voice sample for the current character.
	/// </summary>
	private void PlayVoice(Febucci.TextAnimatorCore.Text.CharacterData characterData)
	{
		if (_charactersTalking && _currentCharacterVoice != null)
		{
			_currentCharacterVoice.PlayVoice(characterData.info.character);
		}
	}
}
