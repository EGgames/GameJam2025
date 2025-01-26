using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------ Music ------")]
   [SerializeField] AudioSource musicWave1;
   [SerializeField] AudioSource musicWave2;
   [SerializeField] AudioSource musicWave3;
   [SerializeField] AudioSource musicWave4;
   [SerializeField] AudioSource musicWave5;

   [Header("------ SFX ------")]

   // player
   [SerializeField] AudioSource plyr_bubba_shot;
   [SerializeField] AudioSource plyr_dash;
   [SerializeField] AudioSource plyr_damage_1;
   [SerializeField] AudioSource plyr_death;
   [SerializeField] AudioSource catship_melee_1;
   [SerializeField] AudioSource catship_melee_2;
   [SerializeField] AudioSource catship_range;
   [SerializeField] AudioSource catship_death_1;
   [SerializeField] AudioSource catship_death_2;
   [SerializeField] AudioSource catship_death_3;
   [SerializeField] AudioSource jelly_bounce;
   

   

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

