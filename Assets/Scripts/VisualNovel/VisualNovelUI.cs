using System.Collections.Generic;
using Febucci.TextAnimatorForUnity;
using PrimeTween;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the visual novel UI presentation layer.
/// Subscribes to DialogueEvents for decoupled communication with DialogueController.
/// </summary>
public class VisualNovelUI : MonoBehaviour
{
	[Header("References")]
	[SerializeField]
	private TypewriterComponent _typewriter;

	[Header("Character References")]
	[SerializeField]
	private VisualNovelDictionary _visualNovelDictionary;

	[SerializeField]
	private GameObject _characterPrefab;

	[SerializeField]
	private Transform _leftPosition;

	[SerializeField]
	private Transform _centerPosition;

	[SerializeField]
	private Transform _rightPosition;

	private readonly Dictionary<string, VNCharacter> _activeCharacters = new Dictionary<string, VNCharacter>();

	[Header("UI References")]
	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private GameObject _namePanel;

	[SerializeField]
	private TextMeshProUGUI _nameText;

	[Header("Data")]
	[SerializeField]
	private float _defaultFadeOutDuration = 1f;

	private void OnEnable()
	{
		GameManager.Instance.DialogueEventsRef.OnStartDialogue += EnableStoryPanel;
		GameManager.Instance.DialogueEventsRef.OnDisplayDialogue += ChangeStoryText;
		GameManager.Instance.DialogueEventsRef.OnCharacterUpdate += UpdateCharacter;
		GameManager.Instance.DialogueEventsRef.OnCharacterRemove += RemoveCharacter;
		GameManager.Instance.DialogueEventsRef.OnNameUpdate += ChangeNameText;
		GameManager.Instance.DialogueEventsRef.OnEndDialogue += DisableStoryPanel;
		GameManager.Instance.DialogueEventsRef.OnEndDialogue += RemoveAllCharacters;
		GameManager.Instance.DialogueEventsRef.OnTypewriterSkip += SkipTypewriter;
		GameManager.Instance.OnGamePaused.AddListener(PauseTypewriter);
		GameManager.Instance.OnGameResume.AddListener(ResumeTypewriter);

		_typewriter.onTextShowed.AddListener(GameManager.Instance.DialogueEventsRef.TypewriterFinished);
	}

	private void OnDisable()
	{
		if (GameManager.Instance)
		{
			GameManager.Instance.DialogueEventsRef.OnStartDialogue -= EnableStoryPanel;
			GameManager.Instance.DialogueEventsRef.OnDisplayDialogue -= ChangeStoryText;
			GameManager.Instance.DialogueEventsRef.OnCharacterUpdate -= UpdateCharacter;
			GameManager.Instance.DialogueEventsRef.OnCharacterRemove -= RemoveCharacter;
			GameManager.Instance.DialogueEventsRef.OnNameUpdate -= ChangeNameText;
			GameManager.Instance.DialogueEventsRef.OnEndDialogue -= DisableStoryPanel;
			GameManager.Instance.DialogueEventsRef.OnEndDialogue += RemoveAllCharacters;
			GameManager.Instance.DialogueEventsRef.OnTypewriterSkip -= SkipTypewriter;
			GameManager.Instance.OnGamePaused.RemoveListener(PauseTypewriter);
			GameManager.Instance.OnGameResume.RemoveListener(ResumeTypewriter);
		}
	}

	private void Start()
	{
		RemoveAllCharacters(0.01f);
	}

	#region UI Updates

	/// <summary>
	/// Updates the speaker name display.
	/// Shows the name panel if a name is provided, hides it otherwise.
	/// </summary>
	/// <param name="name">Speaker name to display, or empty/null to hide.</param>
	private void ChangeNameText(string name)
	{
		if (string.IsNullOrEmpty(name))
		{
			_namePanel.SetActive(false);
		}
		else
		{
			_namePanel.SetActive(true);
			_nameText.text = name;
		}
	}

	/// <summary>
	/// Displays a line of dialogue using the typewriter effect.
	/// </summary>
	/// <param name="line">The dialogue line to display.</param>
	private void ChangeStoryText(string line)
	{
		_typewriter.ShowText(line);
	}

	/// <summary>
	/// Fades in the story panel when dialogue begins.
	/// </summary>
	/// <param name="knotName">Name of the Ink knot being started (unused for UI).</param>
	private void EnableStoryPanel(string knotName)
	{
		float fadeDuration = 1f;
		Tween.Custom(_canvasGroup.alpha, 1, fadeDuration, newVal => _canvasGroup.alpha = newVal);
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = false;
	}

	/// <summary>
	/// Fades out the story panel when dialogue ends.
	/// </summary>
	private void DisableStoryPanel()
	{
		float fadeDuration = 1f;
		Tween.Custom(_canvasGroup.alpha, 0, fadeDuration, newVal => _canvasGroup.alpha = newVal);
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = false;
	}

	#endregion

	#region Typewriter

	/// <summary>
	/// Skips the current typewriter animation to show full text immediately.
	/// </summary>
	private void SkipTypewriter()
	{
		_typewriter.SkipTypewriter();
	}

	/// <summary>
	/// Pauses the typewriter animation. Called when game is paused.
	/// </summary>
	private void PauseTypewriter()
	{
		_typewriter.SetTypewriterSpeed(0f);
	}

	/// <summary>
	/// Resumes the typewriter animation. Called when game is unpaused.
	/// </summary>
	private void ResumeTypewriter()
	{
		_typewriter.SetTypewriterSpeed(1f);
	}

	#endregion

	#region Character Display

	/// <summary>
	/// Updates a character's state, including spawning, moving, or changing sprite.
	/// </summary>
	/// <param name="name">Name of the character.</param>
	/// <param name="position">Target screen position (left, right, center).</param>
	/// <param name="spriteKey">Key for the new sprite, or null/empty to keep current.</param>
	/// <param name="fadeDuration">Duration for fade transitions.</param>
	private void UpdateCharacter(string name, CharacterPosition position, string spriteKey, float fadeDuration)
	{
		Transform targetParent = GetPositionTransform(position);

		if (!_activeCharacters.TryGetValue(name, out VNCharacter character))
		{
			// Spawn new character
			character = Instantiate(_characterPrefab, targetParent).GetComponent<VNCharacter>();
			if (character == null)
			{
				Debug.LogWarning("[VisualNovelUI] Character Prefab Doesn't Have VNCharacter");
				return;
			}
			character.ChangeObjectName(name);
			_activeCharacters[name] = character;

			// Initialize alpha to 0 for fade-in
			character.SetTransparency(0f);
			character.FadeIn(fadeDuration);
		}
		else
		{
			// Move existing character if parent changed
			if (character.transform.parent != targetParent)
			{
				character.transform.SetParent(targetParent);
			}
		}

		// Handle flipping based on position
		if (position == CharacterPosition.Right)
		{
			character.transform.localScale = new Vector3(
				character.transform.localScale.x * -1,
				character.transform.localScale.y,
				character.transform.localScale.z
			);
		}

		// Update Sprite if key provided
		if (!string.IsNullOrEmpty(spriteKey))
		{
			if (_visualNovelDictionary.CharacterSpriteMap.TryGetValue(spriteKey, out Sprite newSprite))
			{
				character.SwitchSprite(newSprite);
			}
			else
			{
				Debug.LogWarning($"[VisualNovelUI] Character sprite not found for key: {spriteKey}");
			}
		}
	}

	/// <summary>
	/// Removes a specific character from the screen.
	/// </summary>
	/// <param name="name">Name of the character to remove.</param>
	/// <param name="fadeDuration">Duration of fade out.</param>
	private void RemoveCharacter(string name, float fadeDuration)
	{
		if (_activeCharacters.TryGetValue(name, out VNCharacter character))
		{
			_activeCharacters.Remove(name);
			character.FadeOutAndDestroy(fadeDuration);
		}
	}

	/// <summary>
	/// Cleans up all character sprites from the screen
	/// </summary>
	/// <param name="fadeDuration">How long it takes to fade out and delete all characters</param>
	private void RemoveAllCharacters(float fadeDuration = 1f)
	{
		var activeTransforms = new HashSet<Transform>();

		foreach (VNCharacter character in _activeCharacters.Values)
		{
			if (character != null)
			{
				activeTransforms.Add(character.transform);
				character.FadeOutAndDestroy(fadeDuration);
			}
		}
		_activeCharacters.Clear();

		// Helper to destroy any objects not tracked by the system
		void DestroyOrphans(Transform parent)
		{
			if (parent == null)
			{
				return;
			}
			for (int i = 0; i < parent.childCount; i++)
			{
				Transform child = parent.GetChild(i);
				if (!activeTransforms.Contains(child))
				{
					Destroy(child.gameObject);
				}
			}
		}

		DestroyOrphans(_leftPosition);
		DestroyOrphans(_centerPosition);
		DestroyOrphans(_rightPosition);
	}

	/// <summary>
	/// Cleans up all character sprites from the screen using the default fade duration.
	/// </summary>
	private void RemoveAllCharacters()
	{
		RemoveAllCharacters(_defaultFadeOutDuration);
	}

	#endregion

	#region Helper

	/// <summary>
	/// Helper to get the Transform corresponding to a CharacterPosition enum.
	/// </summary>
	/// <param name="position">The position enum value.</param>
	/// <returns>The transform for that position.</returns>
	private Transform GetPositionTransform(CharacterPosition position)
	{
		return position switch
		{
			CharacterPosition.Left => _leftPosition,
			CharacterPosition.Right => _rightPosition,
			_ => _centerPosition,
		};
	}

	#endregion
}
