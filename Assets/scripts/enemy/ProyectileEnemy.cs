using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class ProyectileEnemy : Enemy
{
    // The interval between this enemy's shots (in seconds)
    public float shootIntervalSecs;
    public GameObject proyectilePrefab;

    // How much time the enemy has to wait to shoot again (in seconds)
    private float shootCooldownSecs = 0;

    public float targetMaxDistance = 7f; // The target distance we want to maintain
    public float targetMinDistance = 1f; // The target distance we want to maintain

    public Transform firePoint;
    
    private float currentDistance;
    
    private SpriteRenderer spriteRenderer;

    protected override void Start()
    {
        base.Start();

        // fields setup
        shootCooldownSecs = Random.Range(0, shootIntervalSecs);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (!_player) return;
        TickShotInterval();
        TickAim();
    }

    protected override void MoverEnemigo()
    {
        Vector2 targetPosition = _player.position;
        Vector2 currentPosition = transform.position;
        Vector2 direction = targetPosition - currentPosition;
        currentDistance = direction.magnitude;

        // Normalize the direction vector to get the unit vector
        direction.Normalize();

        // Calculate the desired position
        Vector2 desiredPosition = currentPosition;

        if (currentDistance > targetMaxDistance)
        {
            // Move closer to the target if we're too far away
            desiredPosition += direction * (moveSpeed * Time.deltaTime);
        }
        else if (currentDistance < targetMinDistance)
        {
            // Move away from the target if we're too close
            desiredPosition -= direction * (moveSpeed * Time.deltaTime);
        }

        // Move the enemy using Rigidbody2D.MovePosition
        _rb.MovePosition(desiredPosition);
    }

    private void TickShotInterval()
    {
        shootCooldownSecs -= Time.deltaTime;
        if (shootCooldownSecs <= 0 && currentDistance < targetMaxDistance)
        {
            Shoot();
            shootCooldownSecs = shootIntervalSecs;
        }
    }

    private void TickAim()
    {
        // Flip the sprite horizontally if the player is to the right
        spriteRenderer.flipX = _player.position.x > transform.position.x;
    }

    public void Shoot()
    {
        Debug.Log($"'{gameObject.name}' has shot!");
        // Instantiate projectile prefab and rotate it so that it's up direction is the same as the player's direction
        
    }
}