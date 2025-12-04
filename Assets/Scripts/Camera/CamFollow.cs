using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform desiredPos;

    private void LateUpdate()
    {
        transform.SetPositionAndRotation(desiredPos.position, desiredPos.rotation);
    }
}
