using UnityEngine;
using System.Collections.Generic;

public class ProyectileEnemy : MonoBehaviour
{
    // The interval between this enemy's shots (in seconds)
    public float shootIntervalSecs;
    public GameObject proyectilePrefab;

    // How much time the enemy has to wait to shoot again (in seconds)
    private float shootCooldownSecs = 0;

    private void Start()
    {
        shootCooldownSecs = Random.Range(0, shootIntervalSecs);
    }

    private void Update()
    {
        TickShotInterval();
    }

    private void TickShotInterval()
    {
        shootCooldownSecs -= Time.deltaTime;
        if(shootCooldownSecs <= 0)
        {
            Shoot();
            shootCooldownSecs = shootIntervalSecs;
        }
    }

    public void Shoot()
    {
        Debug.Log($"'{gameObject.name}' has shot!");
        Instantiate(proyectilePrefab, transform.position, transform.rotation);
    }
}
