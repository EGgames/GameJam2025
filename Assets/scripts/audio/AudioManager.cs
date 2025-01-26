using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------ Music ------")]
   [SerializeField] AudioSource musicWave1;
   [SerializeField] AudioSource musicWave2;
   [SerializeField] AudioSource musicWave3;
   [SerializeField] AudioSource musicWave4;
   [SerializeField] AudioSource musicWave5;

   [SerializeField] 

    private void Start() 
    {
         
         //     Play the music al unísono.

         musicWave1.Play();
         musicWave2.Play();
         musicWave3.Play();
         musicWave4.Play();
         musicWave5.Play();


    }



}

