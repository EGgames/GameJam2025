using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Inicializamos instancia Singleton
    public static AudioManager Instance;
    
    [SerializeField] private AudioSource sfxSource;
    
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
            DontDestroyOnLoad(gameObject); // Mantener este objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //     Play the music al unísono.
        musicWave1.Play();
        musicWave2.Play();
        musicWave3.Play();
        musicWave4.Play();
        musicWave5.Play();
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
    
    public void StopMusic()
    {
        musicWave1.Stop();
        musicWave2.Stop();
        musicWave3.Stop();
        musicWave4.Stop();
        musicWave5.Stop();
    }
}