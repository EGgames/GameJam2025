using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------ Music ------")]
   [SerializeField] AudioSource musicWave1;
   [SerializeField] AudioSource musicWave2;
   [SerializeField] AudioSource musicWave3;
   [SerializeField] AudioSource musicWave4;
   [SerializeField] AudioSource musicWave5;

    private void Start() 
    {
         musicWave1.Play();
    }

}

