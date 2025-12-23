using Febucci.TextAnimatorForUnity;
using PrimeTween;
using TMPro;
using UnityEngine;

public class VisualNovelUI : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private TypewriterComponent _typewriter;
  
  [Header("UI References")]
	[SerializeField] private CanvasGroup _canvasGroup;
	[SerializeField] private GameObject _leftNamePanel;
	[SerializeField] private GameObject _rightNamePanel;
	[SerializeField] private TextMeshProUGUI _leftNameText;
	[SerializeField] private TextMeshProUGUI _rightNameText;

  public enum SpeakerSide
  {
    Left, // Speaker is on the left
    Right, // Speaker is on the right
    None, // Narration, no one is speaking
  }

  private SpeakerSide _currentSide = SpeakerSide.None;

  private void OnEnable() {
		GameManager.Instance.DialogueEventsRef.OnStartDialogue += EnableStoryPanel;
		GameManager.Instance.DialogueEventsRef.OnDisplayDialogue += ChangeStoryText;
		GameManager.Instance.DialogueEventsRef.OnNameUpdate += ChangeNameText;
		GameManager.Instance.DialogueEventsRef.OnEndDialogue += DisableStoryPanel;
    GameManager.Instance.DialogueEventsRef.OnTypewriterSkip += SkipTypewriter;
    GameManager.Instance.OnGamePaused.AddListener(PauseTypewriter);
    GameManager.Instance.OnGameResume.AddListener(ResumeTypewriter);

		_typewriter.onTextShowed.AddListener(GameManager.Instance.DialogueEventsRef.TypewriterFinished);
	}

	private void OnDisable() {
		if (GameManager.Instance) {
			GameManager.Instance.DialogueEventsRef.OnStartDialogue -= EnableStoryPanel;
      GameManager.Instance.DialogueEventsRef.OnDisplayDialogue -= ChangeStoryText;
      GameManager.Instance.DialogueEventsRef.OnNameUpdate -= ChangeNameText;
      GameManager.Instance.DialogueEventsRef.OnEndDialogue -= DisableStoryPanel;
      GameManager.Instance.DialogueEventsRef.OnTypewriterSkip -= SkipTypewriter;
      GameManager.Instance.OnGamePaused.RemoveListener(PauseTypewriter);
      GameManager.Instance.OnGameResume.RemoveListener(ResumeTypewriter);
		}
	}

  private void ChangeNameText(string name, bool isLeft = true) {
    bool differentName = false;
    bool differentSpeakerSide = false;

    if (string.IsNullOrEmpty(name))
    {
      _currentSide = SpeakerSide.None;
    }
    else if (isLeft)
    {
      differentName = _leftNameText.text != name;
      differentSpeakerSide = _currentSide != SpeakerSide.Left;
      _currentSide = SpeakerSide.Left;
    }
    else
    {
      differentName = _rightNameText.text != name;
      differentSpeakerSide = _currentSide != SpeakerSide.Right;
      _currentSide = SpeakerSide.Right;
    }

    if (differentName || differentSpeakerSide || _currentSide == SpeakerSide.None)
    {
      UpdateNameBox(_currentSide, name);
    }
	}

  private void ChangeStoryText(string line) {
		_typewriter.ShowText(line);
	}
  
  private void EnableStoryPanel(string knotName) {
    float fadeDuration = 1f;
		Tween.Custom(_canvasGroup.alpha, 1, fadeDuration, newVal => _canvasGroup.alpha = newVal);
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = false;
	}

	private void DisableStoryPanel() {
    float fadeDuration = 1f;
		Tween.Custom(_canvasGroup.alpha, 0, fadeDuration, newVal => _canvasGroup.alpha = newVal);
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = false;
	}

  private void UpdateNameBox(SpeakerSide speakerSide = SpeakerSide.None, string name = "")
  {
    if (speakerSide == SpeakerSide.None)
    {
      _leftNamePanel.SetActive(false);
      _rightNamePanel.SetActive(false);
    }
    else if (speakerSide == SpeakerSide.Left)
    {
      _leftNamePanel.SetActive(true);
      _rightNamePanel.SetActive(false);
      _leftNameText.text = name;
    }
    else if (speakerSide == SpeakerSide.Right)
    {
      _leftNamePanel.SetActive(false);
      _rightNamePanel.SetActive(true);
      _rightNameText.text = name;
    }
  }

  private void SkipTypewriter() {
		_typewriter.SkipTypewriter();
	}

  private void PauseTypewriter()
  {
    _typewriter.SetTypewriterSpeed(0f);
  }

  private void ResumeTypewriter()
  {
    _typewriter.SetTypewriterSpeed(1f);
  }
}
