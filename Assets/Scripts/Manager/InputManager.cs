using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : PersistentSingleton<InputManager>
{
	// References
	public InputActionAsset InputActions;

	// Actions
	private InputAction _movementAction;
	private InputAction _continueStoryAction;
	private InputAction _escapeAction;

	// Events
	[HideInInspector]
	public UnityEvent<Vector2> OnMovement;

	[HideInInspector]
	public UnityEvent OnContinueStoryPerformed;

	[HideInInspector]
	public UnityEvent OnEscapePerformed;

	private const string PLAYER_ACTION_MAP = "Player";
	private const string UI_ACTION_MAP = "UI";

	/** Start Methods **/

	protected override void Awake()
	{
		base.Awake();
		EnablePlayerInput();
		EnableUIInput();
		SetupInputActions();
	}

	private void OnEnable()
	{
		EnablePlayerInput();
	}

	private void OnDisable()
	{
		DisablePlayerInput();
	}

	private void SetupInputActions()
	{
		_movementAction = InputSystem.actions.FindAction("Movement");
		_continueStoryAction = InputSystem.actions.FindAction("ContinueStory");
		_escapeAction = InputSystem.actions.FindAction("Escape");
	}

	/** Update Methods **/

	private void Update()
	{
		UpdateInputs();
	}

	private void UpdateInputs()
	{
		UpdateMovementVector(_movementAction, ref OnMovement);

		AddEventToAction(_continueStoryAction, ref OnContinueStoryPerformed);
		AddEventToAction(_escapeAction, ref OnEscapePerformed);
	}

	/// <summary>
	/// Updates a Vector3 variable depending on a movement input action
	/// </summary>
	/// <param name="movementInput">Vector3 representing the movement</param>
	/// <param name="inputAction">Input action was pressed</param>
	private void UpdateMovementVector(InputAction inputAction, ref UnityEvent<Vector2> unityEvent)
	{
		Vector3 readVector = inputAction.ReadValue<Vector3>();
		unityEvent?.Invoke(new Vector2(readVector.x, readVector.z));
	}

	/// <summary>
	/// Checks every update if the input was pressed and calls the unity event
	/// </summary>
	/// <param name="inputAction">Input action was pressed</param>
	/// <param name="unityEvent">Unity Event To Trigger</param>
	private void AddEventToAction(InputAction inputAction, ref UnityEvent unityEvent)
	{
		if (inputAction.WasPressedThisFrame())
		{
			unityEvent?.Invoke();
		}
	}

	/// <summary>
	/// Checks every update if the input was held down and calls the unity event
	/// </summary>
	/// <param name="inputAction">Input action was pressed</param>
	/// <param name="unityEvent">Unity Event To Trigger</param>
	private void AddEventToActionHold(InputAction inputAction, ref UnityEvent unityEvent)
	{
		if (inputAction.IsPressed())
		{
			unityEvent?.Invoke();
		}
	}

	/// <summary>
	/// Checks every update if the input was released and calls the unity event
	/// </summary>
	/// <param name="inputAction">Input action was pressed</param>
	/// <param name="unityEvent">Unity Event To Trigger</param>
	private void AddEventToActionRelease(InputAction inputAction, ref UnityEvent unityEvent)
	{
		if (inputAction.WasReleasedThisFrame())
		{
			unityEvent?.Invoke();
		}
	}

	/// <summary>
	/// Enable Player Input
	/// </summary>
	public void EnablePlayerInput()
	{
		InputActions.FindActionMap(PLAYER_ACTION_MAP).Enable();
	}

	/// <summary>
	/// Disable Player Input
	/// </summary>
	public void DisablePlayerInput()
	{
		InputActions.FindActionMap(PLAYER_ACTION_MAP).Disable();
	}

	/// <summary>
	/// Enable UI Input
	/// </summary>
	public void EnableUIInput()
	{
		InputActions.FindActionMap(UI_ACTION_MAP).Enable();
	}

	/// <summary>
	/// Disable UI Input
	/// </summary>
	public void DisableUIInput()
	{
		InputActions.FindActionMap(UI_ACTION_MAP).Disable();
	}
}
