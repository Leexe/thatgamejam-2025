using System;
using System.Collections.Generic;
using Ink.Runtime;

/// <summary>
/// Screen positions for character sprites.
/// </summary>
public enum CharacterPosition
{
	Left,
	Center,
	Right,
}

/// <summary>
/// Central event hub for dialogue-related communication between systems.
/// </summary>
public class DialogueEvents
{
	#region Visual Events

	/// <summary>
	/// Event fired when character sprite should change.
	/// </summary>
	/// <summary>
	/// Event fired when character sprite should change or move.
	/// </summary>
	public Action<string, CharacterPosition, string, float> OnCharacterUpdate;

	/// <summary>
	/// Updates the displayed character sprite.
	/// </summary>
	/// <param name="name">Name of the character.</param>
	/// <param name="position">Screen position (Left, Center, Right).</param>
	/// <param name="spriteKey">Key identifying the sprite. If null/empty, only moves the character.</param>
	/// <param name="fadeDuration">Duration of the fade transition in seconds.</param>
	public void UpdateCharacter(string name, CharacterPosition position, string spriteKey, float fadeDuration)
	{
		OnCharacterUpdate?.Invoke(name, position, spriteKey, fadeDuration);
	}

	/// <summary>
	/// Event fired when a character should be removed.
	/// </summary>
	public Action<string, float> OnCharacterRemove;

	/// <summary>
	/// Removes a character from the screen.
	/// </summary>
	/// <param name="name">Name of the character to remove.</param>
	/// <param name="fadeDuration">Duration of the fade out.</param>
	public void RemoveCharacter(string name, float fadeDuration)
	{
		OnCharacterRemove?.Invoke(name, fadeDuration);
	}

	/// <summary>
	/// Event fired when all characters should be removed.
	/// </summary>
	public Action OnAllCharacterRemove;

	/// <summary>
	/// Removes all characters from the screen.
	/// </summary>
	/// <param name="fadeDuration">Duration of the fade out.</param>
	public void RemoveAllCharacters(float fadeDuration)
	{
		OnAllCharacterRemove?.Invoke();
	}

	/// <summary>
	/// Event fired when speaker name should change.
	/// </summary>
	public Action<string> OnNameUpdate;

	/// <summary>
	/// Updates the displayed speaker name.
	/// </summary>
	/// <param name="name">Name to display, or empty string to hide.</param>
	public void UpdateName(string name)
	{
		OnNameUpdate?.Invoke(name);
	}

	/// <summary>
	/// Event fired when background should change.
	/// </summary>
	public Action<string> OnBackgroundUpdate;

	/// <summary>
	/// Updates the displayed background.
	/// </summary>
	/// <param name="backgroundKey">Key identifying the background to display.</param>
	public void UpdateBackground(string backgroundKey)
	{
		OnBackgroundUpdate?.Invoke(backgroundKey);
	}

	#endregion

	#region Story Events

	/// <summary>
	/// Event fired when dialogue should start at a specific knot.
	/// </summary>
	public Action<string> OnStartDialogue;

	/// <summary>
	/// Starts the dialogue from a specified Ink knot.
	/// </summary>
	/// <param name="knotName">Name of the Ink knot to start from.</param>
	public void StartDialogue(string knotName)
	{
		OnStartDialogue?.Invoke(knotName);
	}

	/// <summary>
	/// Event fired when dialogue ends.
	/// </summary>
	public Action OnEndDialogue;

	/// <summary>
	/// Signals that the story has ended.
	/// </summary>
	public void EndStory()
	{
		OnEndDialogue?.Invoke();
	}

	/// <summary>
	/// Event fired when a new dialogue line should be displayed.
	/// </summary>
	public Action<string> OnDisplayDialogue;

	/// <summary>
	/// Displays a line of dialogue text.
	/// </summary>
	/// <param name="dialogue">The dialogue text to display.</param>
	public void DisplayDialogue(string dialogue)
	{
		OnDisplayDialogue?.Invoke(dialogue);
	}

	#endregion

	#region Typewriter Events

	/// <summary>
	/// Event fired when typewriter animation should be skipped.
	/// </summary>
	public Action OnTypewriterSkip;

	/// <summary>
	/// Skips the current typewriter animation to show full text immediately.
	/// </summary>
	public void SkipTypewriter()
	{
		OnTypewriterSkip?.Invoke();
	}

	/// <summary>
	/// Event fired when typewriter animation completes.
	/// </summary>
	public Action OnTypewriterFinish;

	/// <summary>
	/// Signals that the typewriter animation has finished.
	/// </summary>
	public void TypewriterFinished()
	{
		OnTypewriterFinish?.Invoke();
	}

	#endregion

	#region Choice Events

	/// <summary>
	/// Event fired when choices should be displayed.
	/// </summary>
	public Action<List<Choice>> OnDisplayChoices;

	/// <summary>
	/// Displays a list of choices for the player to select.
	/// </summary>
	/// <param name="choiceList">List of Ink Choice objects to display.</param>
	public void DisplayChoices(List<Choice> choiceList)
	{
		OnDisplayChoices?.Invoke(choiceList);
	}

	/// <summary>
	/// Event fired when a choice is selected.
	/// </summary>
	public Action<int> OnChoiceSelect;

	/// <summary>
	/// Signals that a choice has been selected.
	/// </summary>
	/// <param name="choiceIndex">Index of the selected choice.</param>
	public void UpdateChoiceSelected(int choiceIndex)
	{
		OnChoiceSelect?.Invoke(choiceIndex);
	}

	/// <summary>
	/// Event fired when choices should be hidden.
	/// </summary>
	public Action OnDisableChoices;

	/// <summary>
	/// Hides all currently displayed choices.
	/// </summary>
	public void DisableChoices()
	{
		OnDisableChoices?.Invoke();
	}

	#endregion

	#region Audio Events

	/// <summary>
	/// Event fired when music should change.
	/// </summary>
	public Action<string> OnPlayMusic;

	/// <summary>
	/// Plays or switches to a music track.
	/// </summary>
	/// <param name="musicKey">Key identifying the music track.</param>
	public void PlayMusic(string musicKey)
	{
		OnPlayMusic?.Invoke(musicKey);
	}

	/// <summary>
	/// Event fired when a sound effect should play.
	/// </summary>
	public Action<string> OnPlaySFX;

	/// <summary>
	/// Plays a one-shot sound effect.
	/// </summary>
	/// <param name="sfxKey">Key identifying the sound effect.</param>
	public void PlaySFX(string sfxKey)
	{
		OnPlaySFX?.Invoke(sfxKey);
	}

	/// <summary>
	/// Event fired when ambience should change.
	/// </summary>
	public Action<string> OnPlayAmbience;

	/// <summary>
	/// Plays or switches to an ambient sound.
	/// </summary>
	/// <param name="ambienceKey">Key identifying the ambience track.</param>
	public void PlayAmbience(string ambienceKey)
	{
		OnPlayAmbience?.Invoke(ambienceKey);
	}

	#endregion
}
