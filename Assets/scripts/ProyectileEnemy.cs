using UnityEngine;
using System.Collections.Generic;

public class ProyectileEnemy : MonoBehaviour
{
    // The interval between this enemy's shots (in seconds)
    public float shootIntervalSecs;
    public GameObject proyectilePrefab;
    public GameObject target;

    // How much time the enemy has to wait to shoot again (in seconds)
    private float shootCooldownSecs = 0;

    private void Start()
    {
        shootCooldownSecs = Random.Range(0, shootIntervalSecs);
    }

    private void Update()
    {
        TickShotInterval();
        TickAim();
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

    private void TickAim()
    {
        Vector2 targetPosition = target.transform.position;
        Vector2 direction = targetPosition - (Vector2)transform.position;
        //-90 is used to use transform.up as the front of the enemy
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) -90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void Shoot()
    {
        Debug.Log($"'{gameObject.name}' has shot!");
        Instantiate(proyectilePrefab, transform.position, transform.rotation);
    }
}
