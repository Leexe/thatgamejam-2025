using System;
using System.Collections.Generic;
using Ink.Runtime;

public class DialogueEvents {
	/** Visual Events **/
	public Action<string, float> OnCharacterUpdate;

	public void UpdateCharacter(string characterKey, float fadeDuration) 
  {
		OnCharacterUpdate?.Invoke(characterKey, fadeDuration);
	}

	public Action<string, bool> OnNameUpdate;

	public void UpdateName(string name, bool isLeft = true) 
  {
		OnNameUpdate?.Invoke(name, isLeft);
	}

	/** Story Events **/

	public Action<string> OnStartDialogue;

	public void StartDialogue(string knotName) 
  {
		OnStartDialogue?.Invoke(knotName);
	}

	public Action OnEndDialogue;

	public void EndStory() 
  {
		OnEndDialogue?.Invoke();
	}

	public Action<string> OnDisplayDialogue;

	public void DisplayDialogue(string dialogue) {
		OnDisplayDialogue?.Invoke(dialogue);
	}

	public Action OnTypewriterSkip;

	public void SkipTypewriter() {
		OnTypewriterSkip?.Invoke();
	}

	public Action OnTypewriterFinish;

	public void TypewriterFinished() {
		OnTypewriterFinish?.Invoke();
	}

	/** Choice Events **/

	public Action<List<Choice>> OnDisplayChoices;

	public void DisplayChoices(List<Choice> choiceList) {
		OnDisplayChoices?.Invoke(choiceList);
	}

	public Action<int> OnChoiceSelect;

	public void UpdateChoiceSelected(int choiceIndex) {
		OnChoiceSelect?.Invoke(choiceIndex);
	}

	public Action OnDisableChoices;

	public void DisableChoices() {
		OnDisableChoices?.Invoke();
	}

	/** Music Events **/

	public Action<string> OnPlayMusic;

	public void PlayMusic(string musicKey) {
		OnPlayMusic?.Invoke(musicKey);
	}

	public Action<string> OnPlaySFX;

	public void PlaySFX(string sfxKey) {
		OnPlaySFX?.Invoke(sfxKey);
	}

	public Action<string> OnPlayAmbience;

	public void PlayAmbience(string ambienceKey) {
		OnPlayAmbience?.Invoke(ambienceKey);
	}
}

