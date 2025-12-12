using UnityEngine;

public class EnemyAudioFootsteps : MonoBehaviour
{
    private AudioSource footsteps;
    private bool isPlaying = false;

    private EnemyAI enemy;

    private void Awake()
    {
        footsteps = GetComponent<AudioSource>();
        enemy = GetComponentInParent<EnemyAI>();
    }

    private void Update()
    {

        if (enemy.MoveSpeed > 0.01f)
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
