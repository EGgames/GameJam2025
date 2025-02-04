using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    
    [Header("Elementos de UI")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void Start()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 1);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 1);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume", 1);
    }

    public void Resume()
    {
        GameManager.Instance.TogglePause();
    }

    public void Restart()
    {
        GameManager.Instance.RestartGame();
    }
    
    public void Exit()
    {
        Application.Quit();
    }
    
    public void SetMasterVolume()
    {
        var volume = masterVolumeSlider.value;
        audioMixer.SetFloat("master", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }
    
    public void SetMusicVolume()
    {
        var volume = musicVolumeSlider.value;
        audioMixer.SetFloat("music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    
    public void SetSFXVolume()
    {
        var volume = sfxVolumeSlider.value;
        audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }
}
