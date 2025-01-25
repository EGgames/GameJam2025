using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class ProyectileEnemy : MonoBehaviour
{
    // The interval between this enemy's shots (in seconds)
    public float shootIntervalSecs;
    public GameObject proyectilePrefab;
    public GameObject target;

    private Collider2D collider;
    private Rigidbody2D rb;

    // How much time the enemy has to wait to shoot again (in seconds)
    private float shootCooldownSecs = 0;

    // Inertia settings
    public float moveSpeed = 5f; // How fast the enemy moves
    public float targetMaxDistance = 7f; // The target distance we want to maintain
    public float targetMinDistance = 1f; // The target distance we want to maintain
    public float smoothFactor = 0.1f; // Controls how smoothly the enemy moves

    private void Start()
    {
        shootCooldownSecs = Random.Range(0, shootIntervalSecs);
        collider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        // Ensure Rigidbody2D is set to Kinematic so we can control velocity directly
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Update()
    {
        TickShotInterval();
        TickAim();
        TickKeepDistanceToTarget();
    }

    private void TickKeepDistanceToTarget()
    {
        Vector2 targetPosition = target.transform.position;
        Vector2 currentPosition = transform.position;
        Vector2 direction = targetPosition - currentPosition;
        float currentDistance = direction.magnitude;

        // Normalize the direction vector to get the unit vector
        direction.Normalize();

        // Calculate the desired velocity
        Vector2 desiredVelocity = Vector2.zero;

        if (currentDistance > targetMaxDistance)
        {
            // Move closer to the target if we're too far away
            desiredVelocity = direction * moveSpeed;
        }
        else if (currentDistance < targetMinDistance)
        {
            // Move away from the target if we're too close
            desiredVelocity = -direction * moveSpeed;
        }

        // Apply smooth physics-based movement using velocity
        rb.velocity = Vector2.Lerp(rb.velocity, desiredVelocity, smoothFactor);
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
