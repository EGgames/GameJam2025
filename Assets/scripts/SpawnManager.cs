using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnManager : MonoBehaviour
    {
        //Inicializamos instancia Singleton
        public static SpawnManager Instance;
        
        [Tooltip("Duración de las oleadas en segundos.")]
        public int waveDuration = 32;
        
        [Tooltip("Lista de oleadas.")]
        public List<EnemyWave> waves;
        
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
            // Iniciar la rutina de spawn
            StartCoroutine(SpawnWaves());
        }

        private IEnumerator SpawnWaves()
        {
            foreach (var wave in waves)
            {
                if (wave != waves[0])
                {
                    yield return new WaitForSeconds(waveDuration);
                }
                SpawnWave(wave);
                PlayWaveMusic(wave);
            }
        }

        private void SpawnWave(EnemyWave wave)
        {
            foreach (var spawner in wave.spawners)
            {
                spawner.gameObject.SetActive(true);
            }
        }
        
        private void PlayWaveMusic(EnemyWave wave)
        {
            // Reproducir música de la oleada
            if (wave.musicSnapshot)
            {
                wave.musicSnapshot.TransitionTo(0);
            }
        }
    }
}