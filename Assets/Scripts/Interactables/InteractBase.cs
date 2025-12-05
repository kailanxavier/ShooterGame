using UnityEngine;

public abstract class InteractBase : MonoBehaviour
{
    private bool alreadyInteracted = false;

    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {
        alreadyInteracted = true;
        if (!alreadyInteracted) return;
    }
}
