using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class ProyectileEnemy : MonoBehaviour
{
    // The interval between this enemy's shots (in seconds)
    public float shootIntervalSecs;
    public GameObject proyectilePrefab;
    private GameObject target;

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
        //components
        collider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        //references
        target = GameObject.FindGameObjectWithTag("Player");

        //fields setup
        shootCooldownSecs = Random.Range(0, shootIntervalSecs);
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
        // Instead of setting the velocity directly, we add a force to the Rigidbody2D
        Vector2 velocityDifference = desiredVelocity - rb.linearVelocity;
        Vector2 forceToApply = velocityDifference * smoothFactor;

        // Apply the force to the Rigidbody2D to move it dynamically
        rb.AddForce(forceToApply);
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
        float angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void Shoot()
    {
        Debug.Log($"'{gameObject.name}' has shot!");
        Instantiate(proyectilePrefab, transform.position, transform.rotation);
    }
}
