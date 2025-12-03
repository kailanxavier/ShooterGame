using UnityEngine;

public class Chest : InteractBase
{
    protected override void Interact()
    {
        base.Interact();
        Debug.Log("Interacted with chest." + 
            "\n Got fucking nothing");
    }
}
