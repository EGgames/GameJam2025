using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Header("Array de prefabs de enemigos")]
    [Tooltip("Arrastra aquí distintos prefabs de enemigos en el Inspector.")]
    public GameObject[] enemyPrefabs;

    [Header("Parámetros de spawn")]
    [Tooltip("Intervalo de tiempo (segundos) entre cada spawn.")]
    public float spawnInterval = 2f;
    
    [Tooltip("Cantidad de enemigos a spawnear antes de desactivar el spawner.")]
    public int numberOfSpawns = 3;

    private float timer = 0f;
    private int spawnCount = 0;

    // Puedes activar/desactivar el spawner
    public bool canSpawn = true;

    private void Awake()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs de enemigos asignados en el Spawner.");
            canSpawn = false;
        }
        
        if (numberOfSpawns <= 0)
        {
            Debug.LogError("El número de spawns debe ser mayor a 0.");
            canSpawn = false;
        }

        if (canSpawn)
        {
            SpawnEnemy();
            GameManager.Instance.enemiesLeft += numberOfSpawns;
        }
    }

    void Update()
    {
        if (!canSpawn || enemyPrefabs == null || enemyPrefabs.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
        
        if (spawnCount >= numberOfSpawns)
        {
            canSpawn = false;
        }
    }

    private void SpawnEnemy()
    {
        // Elige un prefab aleatorio del array
        int index = Random.Range(0, enemyPrefabs.Length);

        // Instancia el enemigo en la posición del Spawner
        Instantiate(enemyPrefabs[index], transform.position, Quaternion.identity);
        
        spawnCount++;
    }
    
    // Dibujar gizmos para visualizar el área de spawn
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
        // Texto con la cantidad de enemigos restantes
        Handles.Label(transform.position + Vector3.up, $"{numberOfSpawns - spawnCount}");
    }
}
