using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class ProyectileEnemy : MonoBehaviour
{
    // The interval between this enemy's shots (in seconds)
    public float shootIntervalSecs;
    public GameObject proyectilePrefab;
    public GameObject target;

    private Collider2D collider;

    // How much time the enemy has to wait to shoot again (in seconds)
    private float shootCooldownSecs = 0;

    private void Start()
    {
        shootCooldownSecs = Random.Range(0, shootIntervalSecs);
        collider = GetComponent<Collider2D>();
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Proyectile"))
        {
            Debug.Log($"'{gameObject.name}' has impacted with a proyectile and got destroyed.");
            Destroy(gameObject);
        }
    }

    public void Shoot()
    {
        Debug.Log($"'{gameObject.name}' has shot!");
        
        // Create a proyectile
        GameObject proyectileObject = Instantiate(proyectilePrefab, transform.position, transform.rotation);
        Collider2D proyectileCollider = proyectileObject.GetComponent<Collider2D>();

        //ignore the collision between the proyectile and this enemy for a bit to prevent self attacking.
        Physics2D.IgnoreCollision(proyectileCollider, collider, true);
        StartCoroutine(ReenableCollision(proyectileCollider, collider));
    }

    private IEnumerator ReenableCollision(Collider2D proyectileCollider, Collider2D enemyCollider)
    {
        yield return new WaitForSeconds(1); // Adjust delay as necessary
        if (proyectileCollider != null && enemyCollider != null)
        {
            Physics2D.IgnoreCollision(proyectileCollider, enemyCollider, false);
        }
    }
}
