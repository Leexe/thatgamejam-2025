using UnityEngine;
using Ink.Runtime;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class DialogueController : MonoBehaviour {
	[Header("References")]
	[SerializeField] 
	private TextAsset _inkJson;

	private DialogueEvents _dialogueEvents;

	[Header("Transition Times")]
	[SerializeField] 
	private float _characterFadeDuration = 0.5f;

	private Story _story;
	private bool _storyPlaying = false;
	private bool _typewriterPlaying = false;
	private int _choiceIndex = -1;
	private bool _needToDisplayChoices = false;
	private bool ChoicesAvailable => _story.currentChoices.Count > 0;

	private void Awake() 
	{
		_story = new Story(_inkJson.text);
	}

	private void OnEnable() {
		_dialogueEvents = GameManager.Instance.DialogueEventsRef;
		_dialogueEvents.OnStartDialogue += StartStory;
		_dialogueEvents.OnChoiceSelect += ContinueStory;
		_dialogueEvents.OnTypewriterFinish += SetTypewriterInactive;
		_dialogueEvents.OnTypewriterFinish += DisplayChoices;
		InputManager.Instance.OnContinueStoryPerformed.AddListener(ContinueStory);
	}

	private void OnDisable() 
	{
		if (GameManager.Instance) {
			_dialogueEvents.OnStartDialogue -= StartStory;
			_dialogueEvents.OnChoiceSelect -= ContinueStory;
			_dialogueEvents.OnTypewriterFinish -= SetTypewriterInactive;
			_dialogueEvents.OnTypewriterFinish -= DisplayChoices;
		}
		if (InputManager.Instance)
		{
			InputManager.Instance.OnContinueStoryPerformed.RemoveListener(ContinueStory);
		}
	}

	private void Start()
	{
		RestartStory_Debug();
	}

	[Button("Debug")]
	private void RestartStory_Debug()
	{
		StartStory("Beginning");
	}

	private void StartStory(string knotName) {
		if (_storyPlaying) {
			return;
		}

		_storyPlaying = true;

		if (knotName == "") {
			Debug.LogError("KnotName is Empty");
		}
		else {
			_story.ChoosePathString(knotName);
		}

		ContinueStory();
	}

	private void ContinueStory() {
		// If the background is transition do not continue story
		if (!_storyPlaying || GameManager.Instance.GamePaused) {
			return;
		}

		// If the typewriter is playing, then skip the dialogue and break from the function
		if (_typewriterPlaying) {
			_dialogueEvents.SkipTypewriter();
			return;
		}

		// If a choice was selected, proceed down the path chosen
		if (ChoicesAvailable && _choiceIndex != -1) {
			_story.ChooseChoiceIndex(_choiceIndex);
			_choiceIndex = -1;
		}

		if (_story.canContinue) {
			string story = _story.Continue();
			_typewriterPlaying = true;

			// Parse Tags
			if (_story.currentTags.Count > 0) {
				ParseTags(_story.currentTags);
			}

			// If the background is changing, wait until it finishes before continuing
      DisplayDialogue();
		}
		else if (!ChoicesAvailable) {
			ExitStory();
		}
	}

	private void DisplayDialogue() {
		string storyLine = _story.currentText;
		// If the next line is blank, try to find a line that isn't blank and display it
		while (IsLineBlank(storyLine) && _story.canContinue) {
			storyLine = _story.Continue();

			while (IsLineBlank(storyLine) && _story.canContinue) {
				storyLine = _story.Continue();
			}

			if (IsLineBlank(storyLine) && !_story.canContinue) {
				ExitStory();
			}
		}

		// Display a choice if possible
		if (ChoicesAvailable) {
			_needToDisplayChoices = true;
		}

		_dialogueEvents.DisplayDialogue(storyLine);
	}

	private void ParseTags(List<string> tags) {
		foreach (string tag in tags) {
			string identifier = tag.Substring(0, 2).ToLower();
			string context = tag.Substring(3);

			if (identifier == "ch") {
				_dialogueEvents.UpdateCharacter(context, _characterFadeDuration);
			}
			else if (identifier == "nm") {
				if (context[0] == 'r')
				{
					context = tag.Substring(5);
					_dialogueEvents.UpdateName(context, false);
				}
				else if (context[0] == 'l')
				{
					context = tag.Substring(5);
					_dialogueEvents.UpdateName(context, true);
				}
				else
				{
					_dialogueEvents.UpdateName("");
				}
			}
			else if (identifier == "sx") {
				_dialogueEvents.PlaySFX(context);
			}
			else if (identifier == "ms") {
				_dialogueEvents.PlayMusic(context);
			}
			else if (identifier == "ab") {
				_dialogueEvents.PlayAmbience(context);
			}
		}
	}

	private void DisplayChoices() {
		if (_needToDisplayChoices) {
			_dialogueEvents.DisplayChoices(_story.currentChoices);
		}
	}

	private void ContinueStory(int choiceIndex) {
		_choiceIndex = choiceIndex;
		ContinueStory();
	}

	private void ExitStory() {
		_storyPlaying = false;
		_typewriterPlaying = false;

		GameManager.Instance.DialogueEventsRef.EndStory();
	}

	private void SetTypewriterInactive() {
		_typewriterPlaying = false;
	}

	private bool IsLineBlank(string line) {
		return line.Trim().Equals("") || line.Trim().Equals("\n");
	}
}
