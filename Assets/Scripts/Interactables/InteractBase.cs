using UnityEngine;

public abstract class InteractBase : MonoBehaviour
{
    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {
        Debug.Log("Interacted!");
    }
}
