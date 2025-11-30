using UnityEngine;

public class ProceduralPOV : MonoBehaviour
{
    [Header("IK Targets")]
    public Transform leftHandTarget;
    public Transform rightHandTarget;

    private Vector3 leftBasePos;
    private Vector3 rightBasePos;

    private Vector3 leftSmoothedOffset;
    private Vector3 rightSmoothedOffset;

    private Vector3 leftVel;
    private Vector3 rightVel;

    private Rigidbody rb;

    [Header("Motion Influence")]
    public float swayStrength = 0.04f;
    public float bobStrength = 0.025f;
    public float inertiaStrength = 0.06f;
    public float jumpLift = 0.12f;

    [Header("Spring Smoothing")]
    public float smoothTime = 0.12f;

    private float bobTimer;

    void Start()
    {
        leftBasePos = leftHandTarget.localPosition;
        rightBasePos = rightHandTarget.localPosition;

        rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        Vector3 vel = rb.linearVelocity;
        float speed = new Vector2(vel.x, vel.z).magnitude;

        bobTimer += Time.deltaTime * speed * 10f;
        Vector3 inertia = vel * inertiaStrength;

        float jump = Mathf.Clamp(vel.y, -2f, 2f) * jumpLift;
        float bob = Mathf.Sin(bobTimer) * bobStrength * speed;
        float sway = Mathf.Sin(bobTimer * 0.5f) * swayStrength * speed;

        Vector3 leftTarget =
            new Vector3(sway, jump + bob, 0) + inertia;

        Vector3 rightTarget =
            new Vector3(-sway, jump - bob, 0) + inertia;

        leftSmoothedOffset = Vector3.SmoothDamp(leftSmoothedOffset, leftTarget, ref leftVel, smoothTime);
        rightSmoothedOffset = Vector3.SmoothDamp(rightSmoothedOffset, rightTarget, ref rightVel, smoothTime);

        leftHandTarget.localPosition = leftBasePos + leftSmoothedOffset;
        rightHandTarget.localPosition = rightBasePos + rightSmoothedOffset;
    }
}
