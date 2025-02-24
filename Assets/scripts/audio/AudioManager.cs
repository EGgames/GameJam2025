using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    //Inicializamos instancia Singleton
    public static AudioManager Instance;
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource pitchVariationSource;
    
    [Header("------ Music ------")] 
    [SerializeField] private AudioSource musicWave1;
    [SerializeField] private AudioSource musicWave2;
    [SerializeField] private AudioSource musicWave3;
    [SerializeField] private AudioSource musicWave4;
    [SerializeField] private AudioSource musicWave5;

    [Header("------ SFX ------")] 
    public AudioClip playerBubbaShot;
    public AudioClip playerDash;
    public AudioClip[] playerDamage;
    public AudioClip playerDeath;
    public AudioClip jellyBounce;

    private void Awake()
    {
        // Configura el Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadVolume();
        PlayMusic();
    }

    public void PlaySFX(AudioClip clip)
    {
        // Reproducir un solo clip de sonido
        sfxSource.PlayOneShot(clip);
    }

    public void PlayRandomSFX(AudioClip[] clips)
    {
        // Reproducir un clip de sonido aleatorio
        sfxSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
    }
    
    public void PlaySFXWithPitchVariation(AudioClip clip, float minPitch = 0.85f, float maxPitch = 1.2f)
    {
        pitchVariationSource.pitch = Random.Range(minPitch, maxPitch);
        pitchVariationSource.PlayOneShot(clip);
    }

    public void PlayMusic()
    {
        // Play the music al unísono.
        musicWave1.Play();
        musicWave2.Play();
        musicWave3.Play();
        musicWave4.Play();
        musicWave5.Play();
    }

    public void PauseMusic()
    {
        musicWave1.Pause();
        musicWave2.Pause();
        musicWave3.Pause();
        musicWave4.Pause();
        musicWave5.Pause();
    }
    
    public void StopMusic()
    {
        musicWave1.Stop();
        musicWave2.Stop();
        musicWave3.Stop();
        musicWave4.Stop();
        musicWave5.Stop();
    }
    
    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            var masterVolume = PlayerPrefs.GetFloat("masterVolume");
            audioMixer.SetFloat("master", Mathf.Log10(masterVolume) * 20);
        }
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            var musicVolume = PlayerPrefs.GetFloat("musicVolume");
            audioMixer.SetFloat("music", Mathf.Log10(musicVolume) * 20);
        }
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            var sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
            audioMixer.SetFloat("sfx", Mathf.Log10(sfxVolume) * 20);
        }
    }
}