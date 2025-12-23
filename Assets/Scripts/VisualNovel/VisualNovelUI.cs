using System.Collections.Generic;
using Febucci.TextAnimatorForUnity;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
	private Image _characterPrefab;

	[SerializeField]
	private Transform _leftPosition;

	[SerializeField]
	private Transform _centerPosition;

	[SerializeField]
	private Transform _rightPosition;

	private readonly Dictionary<string, Image> _activeCharacters = new Dictionary<string, Image>();

	[Header("UI References")]
	[SerializeField]
	private CanvasGroup _canvasGroup;

	[SerializeField]
	private GameObject _namePanel;

	[SerializeField]
	private TextMeshProUGUI _nameText;

	/// <summary>
	/// Subscribes to dialogue and game events when enabled.
	/// </summary>
	private void OnEnable()
	{
		GameManager.Instance.DialogueEventsRef.OnStartDialogue += EnableStoryPanel;
		GameManager.Instance.DialogueEventsRef.OnDisplayDialogue += ChangeStoryText;
		GameManager.Instance.DialogueEventsRef.OnCharacterUpdate += UpdateCharacter;
		GameManager.Instance.DialogueEventsRef.OnCharacterRemove += RemoveCharacter;
		GameManager.Instance.DialogueEventsRef.OnNameUpdate += ChangeNameText;
		GameManager.Instance.DialogueEventsRef.OnEndDialogue += DisableStoryPanel;
		GameManager.Instance.DialogueEventsRef.OnTypewriterSkip += SkipTypewriter;
		GameManager.Instance.OnGamePaused.AddListener(PauseTypewriter);
		GameManager.Instance.OnGameResume.AddListener(ResumeTypewriter);

		_typewriter.onTextShowed.AddListener(GameManager.Instance.DialogueEventsRef.TypewriterFinished);
	}

	/// <summary>
	/// Unsubscribes from dialogue and game events when disabled.
	/// </summary>
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
			GameManager.Instance.DialogueEventsRef.OnTypewriterSkip -= SkipTypewriter;
			GameManager.Instance.OnGamePaused.RemoveListener(PauseTypewriter);
			GameManager.Instance.OnGameResume.RemoveListener(ResumeTypewriter);
		}
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

	private void UpdateCharacter(string name, CharacterPosition position, string spriteKey, float fadeDuration)
	{
		Transform targetParent = GetPositionTransform(position);

		if (!_activeCharacters.TryGetValue(name, out Image characterImage))
		{
			// Spawn new character
			characterImage = Instantiate(_characterPrefab, targetParent);
			characterImage.name = name;
			_activeCharacters[name] = characterImage;

			// Initialize alpha to 0 for fade-in
			Color color = characterImage.color;
			color.a = 0;
			characterImage.color = color;
			Tween.Alpha(characterImage, 1f, fadeDuration);
		}
		else
		{
			// Move existing character if parent changed
			if (characterImage.transform.parent != targetParent)
			{
				characterImage.transform.SetParent(targetParent);
			}
		}

		// Update Sprite if key provided
		if (!string.IsNullOrEmpty(spriteKey))
		{
			if (_visualNovelDictionary.CharacterSpriteMap.TryGetValue(spriteKey, out Sprite newSprite))
			{
				characterImage.sprite = newSprite;
				characterImage.preserveAspect = true;
			}
			else
			{
				Debug.LogWarning($"[VisualNovelUI] Character sprite not found for key: {spriteKey}");
			}
		}
	}

	private void RemoveCharacter(string name, float fadeDuration)
	{
		if (_activeCharacters.TryGetValue(name, out Image characterImage))
		{
			_activeCharacters.Remove(name);
			Tween.Alpha(characterImage, 0f, fadeDuration).OnComplete(() => Destroy(characterImage.gameObject));
		}
	}

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
