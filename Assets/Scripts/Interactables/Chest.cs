using UnityEngine;

public class Chest : InteractBase
{
    [SerializeField] private AnimationClip openClip;
    [SerializeField] private Collider lidCollider;
    private Animator chestAnimator;
    private bool interacted = false;
    private bool secondInteracted = false;

    private void Awake()
    {
        chestAnimator = GetComponentInChildren<Animator>();
    }

    protected override void Interact()
    {
        if (secondInteracted) return;

        base.Interact();
        chestAnimator.Play(openClip.name);
        lidCollider.enabled = true;

        if (!interacted)
        {
            interacted = true;
            return;
        }

        // pickup interaction goes here
        secondInteracted = true;
    }
}
