using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem inputSystem;
    public static InputManager Instance { get; private set; }

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPerformed;
    private bool attackPerformed;

    private bool interactPerformed;
    private bool interactedThisPress;

    private bool pausePerformed;
    private bool pausedThisPress;

    private bool crouchPerformed;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;

        inputSystem ??= new InputSystem();
    }

    private void Update()
    {
        if (!interactPerformed)
            interactedThisPress = false;

        if (!pausePerformed) 
            pausedThisPress = false;
    }

    private void OnEnable()
    {
        inputSystem.Enable();

        inputSystem.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputSystem.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputSystem.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputSystem.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputSystem.Player.Jump.performed += ctx => jumpPerformed = true;
        inputSystem.Player.Jump.canceled += ctx => jumpPerformed = false;

        inputSystem.Player.Attack.performed += ctx => attackPerformed = true;
        inputSystem.Player.Attack.canceled += ctx => attackPerformed = false;

        inputSystem.Player.Interact.performed += ctx => interactPerformed = true;
        inputSystem.Player.Interact.canceled += ctx => interactPerformed = false;

        inputSystem.Player.Crouch.performed += ctx => crouchPerformed = true;
        inputSystem.Player.Crouch.canceled += ctx => crouchPerformed = false;

        inputSystem.Player.PauseMenu.performed += ctx => pausePerformed = true;
        inputSystem.Player.PauseMenu.canceled += ctx => pausePerformed = false;

        inputSystem.UI.Cancel.performed += ctx => pausePerformed = true;
        inputSystem.UI.Cancel.canceled += ctx => pausePerformed = false;
    }

    private void OnDisable()
    {
        inputSystem.Disable();

        inputSystem.Player.Move.performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        inputSystem.Player.Move.canceled -= ctx => moveInput = Vector2.zero;

        inputSystem.Player.Look.performed -= ctx => lookInput = ctx.ReadValue<Vector2>();
        inputSystem.Player.Look.canceled -= ctx => lookInput = Vector2.zero;

        inputSystem.Player.Jump.performed -= ctx => jumpPerformed = true;
        inputSystem.Player.Jump.canceled -= ctx => jumpPerformed = false;

        inputSystem.Player.Attack.performed -= ctx => attackPerformed = true;
        inputSystem.Player.Attack.canceled -= ctx => attackPerformed = false;

        inputSystem.Player.Interact.performed -= ctx => interactPerformed = true;
        inputSystem.Player.Interact.canceled -= ctx => interactPerformed = false;

        inputSystem.Player.Crouch.performed -= ctx => crouchPerformed = true;
        inputSystem.Player.Crouch.canceled -= ctx => crouchPerformed = false;

        inputSystem.Player.PauseMenu.performed -= ctx => pausePerformed = true;
        inputSystem.Player.PauseMenu.canceled -= ctx => pausePerformed = false;

        inputSystem.UI.Cancel.performed -= ctx => pausePerformed = true;
        inputSystem.UI.Cancel.canceled -= ctx => pausePerformed = false;
    }

    public void EnableInput()
    {
        inputSystem.Player.Enable();
        inputSystem.UI.Disable();
    }

    public void DisableInput()
    {
        inputSystem.Player.Disable();
        inputSystem.UI.Enable();
    }

    public Vector2 Move => moveInput;
    public Vector2 Look => lookInput;
    public bool Jump => jumpPerformed;
    public bool Attack => attackPerformed;
    public bool Crouch => crouchPerformed;

    public bool PauseMenu
    {
        get
        {
            if (pausePerformed && !pausedThisPress)
            {
                pausedThisPress = true;
                return true;
            }
            return false;
        }
    }

    public bool Interact
    {
        get
        {
            if (interactPerformed && !interactedThisPress)
            {
                interactedThisPress = true;
                return true;
            }
            return false;
        }
    }

}
