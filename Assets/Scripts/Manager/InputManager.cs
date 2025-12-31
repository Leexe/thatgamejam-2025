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

	private InputActionMap _playerActionMap;
	private InputActionMap _uiActionMap;

	// Events
	[HideInInspector]
	public UnityEvent<Vector2> OnMovement;

	[HideInInspector]
	public UnityEvent OnContinueStoryPerformed;

	[HideInInspector]
	public UnityEvent OnEscapePerformed;

	private const string PlayerActionMap = "Player";
	private const string UIActionMap = "UI";

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

	private void OnDestroy()
	{
		if (_movementAction != null)
		{
			_movementAction.performed -= OnMovementPerformed;
			_movementAction.canceled -= OnMovementCanceled;
		}
	}

	/// <summary>
	/// Initializes the input actions by finding them in the InputSystem.
	/// </summary>
	private void SetupInputActions()
	{
		_playerActionMap = InputActions.FindActionMap(PlayerActionMap);
		_uiActionMap = InputActions.FindActionMap(UIActionMap);

		_movementAction = InputActions.FindAction("Movement");
		_movementAction.performed += OnMovementPerformed;
		_movementAction.canceled += OnMovementCanceled;

		_continueStoryAction = InputActions.FindAction("ContinueStory");
		_escapeAction = InputActions.FindAction("Escape");
	}

	/** Update Methods **/

	private void Update()
	{
		UpdateInputs();
	}

	private void UpdateInputs()
	{
		AddEventToAction(_continueStoryAction, ref OnContinueStoryPerformed);
		AddEventToAction(_escapeAction, ref OnEscapePerformed);
	}

	/// <summary>
	/// Callback fired when movement input is performed.
	/// </summary>
	private void OnMovementPerformed(InputAction.CallbackContext context)
	{
		Vector3 readVector = context.ReadValue<Vector3>();
		OnMovement?.Invoke(new Vector2(readVector.x, readVector.z));
	}

	/// <summary>
	/// Callback fired when movement input is canceled.
	/// </summary>
	private void OnMovementCanceled(InputAction.CallbackContext context)
	{
		OnMovement?.Invoke(Vector2.zero);
	}

	/// <summary>
	/// Checks if the input action was pressed this frame and invokes the UnityEvent.
	/// </summary>
	/// <param name="inputAction">The input action to check.</param>
	/// <param name="unityEvent">The UnityEvent to trigger.</param>
	private void AddEventToAction(InputAction inputAction, ref UnityEvent unityEvent)
	{
		if (inputAction.WasPressedThisFrame())
		{
			unityEvent?.Invoke();
		}
	}

	/// <summary>
	/// Checks if the input action is currently held down and invokes the UnityEvent.
	/// </summary>
	/// <param name="inputAction">The input action to check.</param>
	/// <param name="unityEvent">The UnityEvent to trigger.</param>
	private void AddEventToActionHold(InputAction inputAction, ref UnityEvent unityEvent)
	{
		if (inputAction.IsPressed())
		{
			unityEvent?.Invoke();
		}
	}

	/// <summary>
	/// Checks if the input action was released this frame and invokes the UnityEvent.
	/// </summary>
	/// <param name="inputAction">The input action to check.</param>
	/// <param name="unityEvent">The UnityEvent to trigger.</param>
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
		_playerActionMap?.Enable();
	}

	/// <summary>
	/// Disable Player Input
	/// </summary>
	public void DisablePlayerInput()
	{
		_playerActionMap?.Disable();
	}

	/// <summary>
	/// Enable UI Input
	/// </summary>
	public void EnableUIInput()
	{
		_uiActionMap?.Enable();
	}

	/// <summary>
	/// Disable UI Input
	/// </summary>
	public void DisableUIInput()
	{
		_uiActionMap?.Disable();
	}
}
