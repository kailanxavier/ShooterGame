using UnityEngine;

public class DynamicFOV : MonoBehaviour
{
    public static DynamicFOV Instance { get; private set; }

    [Header("Base FOV Settings")]
    [SerializeField] private float defaultFOV = 75f;

    [Header("Speed-Based FOV")]
    [SerializeField] private float maxSpeedFOVBoost = 15f;
    [SerializeField] private float speedForMaxFOV = 12f;  

    private Camera cam;

    private float targetFOV;
    private float currentVelocity;

    [SerializeField] private float smoothTime = 0.15f;

    private float externalFOVAdd = 0f;
    private float externalVelocity = 0f;

    private PlayerMovement player;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        cam = Camera.main;
        player = FindFirstObjectByType<PlayerMovement>();

        targetFOV = defaultFOV;
    }

    private void Update()
    {
        float speedFOV = CalculateSpeedFOV();
        float combinedTarget = defaultFOV + speedFOV + externalFOVAdd;

        cam.fieldOfView = Mathf.SmoothDamp(
            cam.fieldOfView,
            combinedTarget,
            ref currentVelocity,
            smoothTime
        );
    }

    private float CalculateSpeedFOV()
    {
        if (player == null) return 0f;

        float speed = player.Speed;
        float t = Mathf.Clamp01(speed / speedForMaxFOV);

        return Mathf.Lerp(0f, maxSpeedFOVBoost, t);
    }

    public void SetExternalFOV(float value, float smooth = -1f)
    {
        externalFOVAdd = value;
        if (smooth > 0) smoothTime = smooth;
    }

    public void ResetExternalFOV(float smooth = -1f)
    {
        externalFOVAdd = 0f;
        if (smooth > 0) smoothTime = smooth;
    }
}
