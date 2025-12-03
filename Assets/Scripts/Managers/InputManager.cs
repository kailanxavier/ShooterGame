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
    }

    public Vector2 Move => moveInput;
    public Vector2 Look => lookInput;
    public bool Jump => jumpPerformed;
    public bool Attack => attackPerformed;
    public bool Interact => interactPerformed;
}
