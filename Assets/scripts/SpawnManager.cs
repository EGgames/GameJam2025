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

        private void FixedUpdate()
        {
            // Seguir al jugador
            if (!GameManager.Instance.player) return;
            transform.position = new Vector3(GameManager.Instance.player.transform.position.x, GameManager.Instance.player.transform.position.y, transform.position.z);
        }

        private IEnumerator SpawnWaves()
        {
            foreach (var wave in waves)
            {
                SpawnWave(wave);
                PlayWaveMusic(wave);
                GameManager.Instance.currentWave++;
                
                // Si no es la última oleada, esperar X segundos antes de detenerla y pasar a la siguiente
                if (wave == waves[^1]) continue;
                yield return new WaitForSeconds(waveDuration);
                StopWave(wave);
            }
        }

        private void SpawnWave(EnemyWave wave)
        {
            foreach (var spawner in wave.spawners)
            {
                spawner.gameObject.SetActive(true);
            }
        }
        
        private void StopWave(EnemyWave wave)
        {
            foreach (var spawner in wave.spawners)
            {
                spawner.gameObject.SetActive(false);
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