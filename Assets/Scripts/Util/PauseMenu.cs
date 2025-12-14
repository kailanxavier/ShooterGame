using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [SerializeField] private GameObject soundtrack;
    [SerializeField] private bool soundtrackOn = true;

    private void Start()
    {
        UpdateMasterVolume();
        UpdateMusicVolume();
        UpdateSoundFXVolume();
        soundtrack.SetActive(soundtrackOn);

        // hide menu after updates
        gameObject.SetActive(false);
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    public void ToggleSoundtrack()
    {
        soundtrackOn = !soundtrackOn;

        if (soundtrackOn)
            soundtrack.SetActive(true);
        else
            soundtrack.SetActive(false);
    }

    public void UpdateMasterVolume()
    {
        SoundManager.Instance.UpdateMasterVolume(masterVolumeSlider.value);
    }

    public void UpdateSoundFXVolume()
    {
        SoundManager.Instance.UpdateSoundFXVolume(sfxVolumeSlider.value);
    }

    public void UpdateMusicVolume()
    {
        SoundManager.Instance.UpdateMusicVolume(musicVolumeSlider.value);
    }
}
