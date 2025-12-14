using UnityEditor;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private bool canInteract = true;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private GameObject interactUI;

    [SerializeField] private GameObject paywallCanvas;

    private void Update()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        canInteract = Physics.Raycast(ray, out RaycastHit hitInfo, interactDistance, interactableMask | obstacleMask);
        interactUI.SetActive(canInteract);

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
        if (interactable == null)
        {
            interactable = hitInfo.collider.gameObject.GetComponentInParent<InteractBase>();
        }

        if (interactable != null)
        {
            interactable.BaseInteract();
        }

        else
        {
            paywallCanvas.SetActive(true);

            InputManager.Instance.DisableInput();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
