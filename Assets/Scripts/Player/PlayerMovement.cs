using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent (typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References: ")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundMask;
    private Rigidbody playerRb;

    [Header("Movement settings: ")]
    [SerializeField] private float acceleration = 60.0f;
    [SerializeField] private float airControl = 0.5f;
    [SerializeField] private float maxSpeed = 12.0f;
    [SerializeField] private float groundDrag = 6.0f;
    [SerializeField] private float groundCheckRadius = 0.35f;
    [SerializeField] private float groundCheckDistance = 0.4f;
    private Vector3 groundNormal = Vector3.up;

    [Header("Crouch settings: ")]
    [SerializeField] private float crouchSpeedMultiplier = 0.4f;
    [SerializeField] private float crouchAccelMultiplier = 0.5f;

    [Header("Jump settings: ")]
    [SerializeField] private float jumpForce = 35.0f;
    [SerializeField] private float jumpCooldown = 0.2f;
    private bool readyToJump = true;

    private bool jumpRequested;
    private Vector2 inputVector;
    private PlayerCrouchSlide crouch;

    private const float groundedRememberTime = 0.1f;
    private float groundedTimer;
    public bool IsGrounded
    {
        get { return groundedTimer > 0; }
    }

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        crouch = GetComponent<PlayerCrouchSlide>();
    }

    private void Update()
    {
        inputVector = InputManager.Instance.Move;
        jumpRequested = InputManager.Instance.Jump;

        bool rawGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);

        if (rawGrounded)
            groundedTimer = groundedRememberTime;
        else
            groundedTimer -= Time.deltaTime;

        if (Physics.Raycast(groundCheckPoint.position, Vector3.down, out RaycastHit hit, groundCheckDistance + 0.2f, groundMask))
            groundNormal = hit.normal;
        else
            groundNormal = Vector3.up;

        if (jumpRequested && IsGrounded && readyToJump)
            Jump();

        playerRb.linearDamping = IsGrounded ? groundDrag : 0f;
    }

    private void LateUpdate()
    {
        // rotate player to face cam dir
        Vector3 e = orientation.eulerAngles;
        playerRb.MoveRotation(Quaternion.Euler(0.0f, e.y, 0.0f));
    }

    private void FixedUpdate()
    {
        BaseMove();
        CounterMovement();
        ClampSpeed();
        TendToGround();
    }

    private void TendToGround()
    {
        if (!IsGrounded) return;

        playerRb.AddForce(Vector3.down * 120f, ForceMode.Acceleration);
    }

    private void BaseMove()
    {
        Vector3 y = Vector3.ProjectOnPlane(orientation.forward, groundNormal).normalized;
        Vector3 x = Vector3.ProjectOnPlane(orientation.right, groundNormal).normalized;

        Vector3 moveDir = y * inputVector.y + x * inputVector.x;
        if (moveDir.sqrMagnitude > 1.0f) moveDir.Normalize();

        float multiplier = IsGrounded ? 1f : airControl;

        if (crouch != null && crouch.IsCrouching && !crouch.IsSliding)
            multiplier *= crouchAccelMultiplier;

        playerRb.AddForce(acceleration * multiplier * moveDir, ForceMode.Acceleration);
    }

    private void Jump()
    {
        readyToJump = false;

        // reset vertical momentum
        playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        // prevent the annoying fucking super jump
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void CounterMovement()
    {
        if (!IsGrounded) return;

        Vector3 flatVel = new(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);

        if (flatVel.sqrMagnitude < 0.0001f) return;

        Vector3 fwd = transform.forward;
        Vector3 right = transform.right;

        float velFwd = Vector3.Dot(flatVel, fwd);
        float velRight = Vector3.Dot(flatVel, right);

        float inputY = inputVector.y;
        float inputX = inputVector.x;

        Vector3 counter = Vector3.zero;

        // forward axis
        if (Mathf.Abs(inputY) < 0.1f)
        {
            // no input
            counter += -fwd * velFwd * 10.0f;
        }
        else if (Mathf.Sign(inputY) != Mathf.Sign(velFwd))
        {
            // opposite input
            counter += -fwd * velFwd * 15.0f;
        }

        // right axis
        if (Mathf.Abs(inputX) < 0.1f)
        {
            // no input
            counter += -right * velRight * 10.0f;
        }
        else if (Mathf.Sign(inputX) != Mathf.Sign(velRight))
        {
            // opposite input
            counter += -right * velRight * 15.0f;
        }

        playerRb.AddForce(counter, ForceMode.Acceleration);
    }

    private void ClampSpeed()
    {
        Vector3 flatVel = new(playerRb.linearVelocity.x, 0f, playerRb.linearVelocity.z);
        float flatSpeed = flatVel.magnitude;

        float currentMaxSpeed = maxSpeed;

        if (crouch != null && crouch.IsCrouching && !crouch.IsSliding)
            currentMaxSpeed *= crouchSpeedMultiplier;

        if (flatSpeed > maxSpeed)
        {
            Vector3 limited = flatVel.normalized * currentMaxSpeed;
            Vector3 newVel = limited + Vector3.Project(playerRb.linearVelocity, groundNormal);
            playerRb.linearVelocity = newVel;
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public float Speed => playerRb.linearVelocity.magnitude;
}
