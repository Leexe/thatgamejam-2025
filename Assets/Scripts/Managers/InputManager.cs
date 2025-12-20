using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : PersistantSingleton<InputManager>
{
	// References
	public InputActionAsset InputActions;

	// Actions
	private InputAction _movementAction;
	private InputAction _interactAction;
	private InputAction _specialAbilityAction;
	private InputAction _inventoryAction;
	private InputAction _mapAction;
	private InputAction _escapeAction;
	private InputAction _anchorAction;

	// Events
	[HideInInspector]
	public UnityEvent<Vector2> OnMovement;

	[HideInInspector]
	public UnityEvent OnInteractPerformed;

	[HideInInspector]
	public UnityEvent OnSpecialAbilityPerformed;

	[HideInInspector]
	public UnityEvent OnEscapePerformed;

	[HideInInspector]
	public UnityEvent OnMapPerformed;

	[HideInInspector]
	public UnityEvent OnInventoryPerformed;

	[HideInInspector]
	public UnityEvent OnAnchorPerformed;

	[HideInInspector]
	public UnityEvent OnAnchorHeld;

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
		_interactAction = InputSystem.actions.FindAction("Interact");
		_specialAbilityAction = InputSystem.actions.FindAction("Special Ability");
		_escapeAction = InputSystem.actions.FindAction("Escape");
		_mapAction = InputSystem.actions.FindAction("Map");
		_inventoryAction = InputSystem.actions.FindAction("Inventory");
		_anchorAction = InputSystem.actions.FindAction("Anchor");
	}

	/** Update Methods **/

	private void Update()
	{
		UpdateInputs();
	}

	private void UpdateInputs()
	{
		UpdateMovementVector(_movementAction, ref OnMovement);

		AddEventToAction(_interactAction, ref OnInteractPerformed);
		AddEventToAction(_specialAbilityAction, ref OnSpecialAbilityPerformed);
		AddEventToAction(_mapAction, ref OnMapPerformed);
		AddEventToAction(_inventoryAction, ref OnInventoryPerformed);
		AddEventToAction(_escapeAction, ref OnEscapePerformed);
		AddEventToAction(_anchorAction, ref OnAnchorPerformed);

		AddEventToActionHold(_anchorAction, ref OnAnchorHeld);
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
