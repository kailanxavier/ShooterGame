using UnityEngine;

public class Chest : InteractBase
{
    [SerializeField] private AnimationClip openClip;
    [SerializeField] private Collider lidCollider;
    [SerializeField] private AudioClip chestOpen;
    [SerializeField, Range(0.0f, 1.0f)] private float chestOpenVolume = 1.0f;
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
            SoundManager.Instance.PlaySound(chestOpen, transform.position, chestOpenVolume);
            return;
        }

        // pickup interaction goes here
        secondInteracted = true;
    }
}
