using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Pool settings: ")]
    [SerializeField] private int poolSize = 16;
    [SerializeField] private GameObject audioSourcePrefab;

    [Header("Default settings: ")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    public float minPitchVariation = 0.95f;
    public float maxPitchVariation = 1.05f;

    private Queue<AudioSource> audioPool = new Queue<AudioSource>();
    [SerializeField] private AudioMixerGroup mixerGroup;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // craete pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject sourceObj = new GameObject("PooledAudioSource_" + i);
            sourceObj.transform.SetParent(transform);
            AudioSource src = sourceObj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            audioPool.Enqueue(src);
        }
    }

    public void PlayWeaponSound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource src = GetAvailableAudioSource();
        src.transform.position = position;
        src.volume = volume * masterVolume;
        src.pitch = Random.Range(minPitchVariation, maxPitchVariation);
        src.spatialBlend = 1f;
        src.minDistance = 2f;
        src.maxDistance = 25f;
        src.outputAudioMixerGroup = mixerGroup;

        src.clip = clip;
        src.Play();

        StartCoroutine(ReturnToPoolAfterPlay(src, clip.length / src.pitch));
    }

    private AudioSource GetAvailableAudioSource()
    {
        AudioSource src = audioPool.Count > 0 ? audioPool.Dequeue() : new GameObject("ExtraAudioSource").AddComponent<AudioSource>();
        src.gameObject.SetActive(true);
        return src;
    }

    private IEnumerator ReturnToPoolAfterPlay(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);
        src.Stop();
        src.clip = null;
        src.gameObject.SetActive(false);
        audioPool.Enqueue(src);
    }
}