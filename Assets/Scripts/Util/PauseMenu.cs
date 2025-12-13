using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        UpdateMasterVolume();
        UpdateMusicVolume();
        UpdateSoundFXVolume();

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
