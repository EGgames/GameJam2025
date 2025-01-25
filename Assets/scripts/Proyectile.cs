using UnityEngine;

public class Proyectile : MonoBehaviour
{
    public float maxVelocity;
    public float accelerationPerSec;
    private float velocity = 0;

    private void Update()
    {
        // Increment velocity based on acceleration and clamp to maxVelocity
        if (velocity < maxVelocity)
        {
            velocity += accelerationPerSec * Time.deltaTime;
            velocity = Mathf.Min(velocity, maxVelocity);
        }

        // Use the object's up direction for movement in 2D space
        Vector2 movement = (Vector2)transform.up * velocity * Time.deltaTime;

        // Move the object in world space
        transform.Translate(movement, Space.World);
    }
}
