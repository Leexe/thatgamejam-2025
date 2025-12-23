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
	[SerializeField] private TypewriterComponent _typewriter;

	[Header("UI References")]
	[SerializeField] private CanvasGroup _canvasGroup;
	[SerializeField] private GameObject _namePanel;
	[SerializeField] private TextMeshProUGUI _nameText;

	/// <summary>
	/// Subscribes to dialogue and game events when enabled.
	/// </summary>
	private void OnEnable()
	{
		GameManager.Instance.DialogueEventsRef.OnStartDialogue += EnableStoryPanel;
		GameManager.Instance.DialogueEventsRef.OnDisplayDialogue += ChangeStoryText;
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

	#region Typewriter Control

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
}
