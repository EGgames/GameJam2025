using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Array de prefabs de enemigos")]
    [Tooltip("Arrastra aquí distintos prefabs de enemigos en el Inspector.")]
    public GameObject[] enemyPrefabs;

    [Header("Parámetros de spawn")]
    [Tooltip("Intervalo de tiempo (segundos) entre cada spawn.")]
    public float spawnInterval = 2f;

    private float timer = 0f;

    // Puedes activar/desactivar el spawner
    public bool canSpawn = true;

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
    }

    private void SpawnEnemy()
    {
        // Elige un prefab aleatorio del array
        int index = Random.Range(0, enemyPrefabs.Length);

        // Instancia el enemigo en la posición del Spawner
        Instantiate(enemyPrefabs[index], transform.position, Quaternion.identity);
    }
}
