using Unity.VisualScripting;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private AudioSource footsteps;

    private PlayerMovement playerMovement;
    private bool isPlaying = false;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {

        if (playerMovement.Speed > 0.01f && playerMovement.IsGrounded)
        {
            if (isPlaying) return;
            isPlaying = true;
            footsteps.Play();
        }
        else
        {
            isPlaying = false;
            footsteps.Stop();
        }
    }
}
