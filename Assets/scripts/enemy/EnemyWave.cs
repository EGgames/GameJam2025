using UnityEngine.Audio;

[System.Serializable]
public class EnemyWave
{
    public Spawner[] spawners;
    public AudioMixerSnapshot musicSnapshot;

    public EnemyWave(float spawnTime, Spawner[] spawners, AudioMixerSnapshot musicSnapshot)
    {
        this.spawners = spawners;
        this.musicSnapshot = musicSnapshot;
    }
}