[System.Serializable]
public class EnemyWave
{
    public float spawnTime;
    public Spawner[] spawners;

    public EnemyWave(float spawnTime, Spawner[] spawners)
    {
        this.spawnTime = spawnTime;
        this.spawners = spawners;
    }
}