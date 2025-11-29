using UnityEngine;

public class ProceduralPOV : MonoBehaviour
{
    [Header("References: ")]
    [SerializeField] private Transform leftHandBone;
    [SerializeField] private Transform rightHandBone;
    private Rigidbody playerRb;

    [Header("Jump reaction: ")]
    [SerializeField] private float jumpLiftAmount = 0.1f;
    [SerializeField] private float damping = 6.0f;

    [Header("Sway settings: ")]
    [SerializeField] private float swayAmount = 0.05f;
    [SerializeField] private float swaySpeed = 4.0f;

    [Header("Bob settings: ")]
    [SerializeField] private float bobAmount = 0.03f;
    [SerializeField] private float bobFreq = 8.0f;

    private Vector3 leftHandPos;
    private Vector3 rightHandPos;

    private void Start()
    {
        leftHandPos = leftHandBone.localPosition;
        rightHandPos = rightHandBone.localPosition;

        playerRb = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        Vector3 vel = playerRb.linearVelocity;
        float speed = new Vector2(vel.x, vel.y).magnitude;

        float time = Time.time;

        // jump
        float verticalVel = vel.y;
        float jumpOffset = Mathf.Lerp(0f, jumpLiftAmount, Mathf.Clamp01(verticalVel / 10.0f));
        jumpOffset = Mathf.Lerp(jumpOffset, 0.0f, Time.deltaTime * damping);

        // movement
        float sway = Mathf.Sin(time * swaySpeed) * speed * swayAmount;

        // bob
        float bob = Mathf.Sin(time * bobFreq) * speed * bobAmount;

        // apply offsets
        Vector3 leftOffset = new Vector3(sway, jumpOffset + bob, 0);
        Vector3 rightOffset = new Vector3(-sway, jumpOffset - bob, 0);

        leftHandBone.localPosition = leftHandPos + leftOffset;
        rightHandBone.localPosition = rightHandPos + rightOffset;
    }
}
