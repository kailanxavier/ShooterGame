using Unity.VisualScripting;
using UnityEngine;

public class PlayerCrouchSlide : MonoBehaviour
{
    private CapsuleCollider capsule;
    private PlayerMovement player;
    private Rigidbody playerRb;

    [Header("Crouch settings: ")]
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.2f;
    [SerializeField] private float transitionSpeed = 8.0f;

    [Header("Slide settings: ")]
    [SerializeField] private float slideSpeed = 12.0f;
    [SerializeField] private float slideDecay = 6.0f;
    [SerializeField] private float minSlideSpeed = 6.0f;

    [Header("Camera settings: ")]
    [SerializeField] private float slideFOVBoost = 10.0f;

    private bool isCrouching = false;
    private bool isSliding = false;
    private float targetHeight;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        playerRb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();

        targetHeight = standingHeight;
    }

    private void Update()
    {
        bool crouchInput = InputManager.Instance.Crouch;

        if (crouchInput && !isCrouching)
            StartCrouch();

        if (!crouchInput && isCrouching && !isSliding)
            StopCrouch();

        float h = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * transitionSpeed);
        capsule.height = h;

        if (isSliding)
        {
            playerRb.AddForce(-playerRb.linearVelocity.normalized * slideDecay, ForceMode.Acceleration);

            if (playerRb.linearVelocity.magnitude < minSlideSpeed || !player.IsGrounded)
                StopSlide();
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;
        targetHeight = crouchHeight;

        if (player.Speed > minSlideSpeed && player.IsGrounded)
            StartSlide();
    }

    private void StopCrouch()
    {
        isCrouching = false;
        targetHeight = standingHeight;
    }

    private void StartSlide()
    {
        if (isSliding) return;

        // only slide if moving forward
        float forwardDir = Vector3.Dot(playerRb.linearVelocity, transform.forward);
        bool movingFwd = forwardDir > 0.1f;
        if (!movingFwd) return;

        isSliding = true;

        Vector3 fwd = transform.forward;
        playerRb.AddForce(fwd * slideSpeed, ForceMode.Impulse);

        DynamicFOV.Instance.SetExternalFOV(slideFOVBoost);
    }

    private void StopSlide()
    {
        if (!isSliding) return;
        isSliding = false;

        DynamicFOV.Instance.ResetExternalFOV();

        if (!InputManager.Instance.Crouch)
            StopCrouch();
    }

    public bool IsCrouching => isCrouching;
    public bool IsSliding => isSliding;
}
