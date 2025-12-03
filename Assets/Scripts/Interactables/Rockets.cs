using UnityEngine;

public class Rockets : InteractBase
{
    protected override void Interact()
    {
        base.Interact();
        Debug.Log("Interacted with the rockets!" + 
            "\nKABOOM");
    }
}
