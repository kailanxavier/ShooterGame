using UnityEditor;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private bool canInteract = true;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableMask;

    private void Update()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        canInteract = Physics.Raycast(ray, out RaycastHit hitInfo, interactDistance, interactableMask);

        if (canInteract)
        {
            // interact
            if (InputManager.Instance.Interact)
            {
                Interact(hitInfo);
            }
        }
    }

    private void Interact(RaycastHit hitInfo)
    {
        canInteract = false;
        InteractBase interactable = hitInfo.collider.gameObject.GetComponent<InteractBase>();
        if (interactable)
            interactable.BaseInteract();
        else
            Debug.Log("NULL");
    }
}
