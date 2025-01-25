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
        
        [Tooltip("Lista de oleadas con sus tiempos de aparición.")]
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
                yield return new WaitForSeconds(wave.spawnTime);
                SpawnWave(wave);
            }
        }

        private void SpawnWave(EnemyWave wave)
        {
            foreach (var spawner in wave.spawners)
            {
                spawner.gameObject.SetActive(true);
            }
        }
    }
}