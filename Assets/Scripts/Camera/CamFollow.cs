using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField] private Transform desiredPos;

    private void Update()
    {
        transform.SetPositionAndRotation(desiredPos.position, desiredPos.rotation);
    }
}
