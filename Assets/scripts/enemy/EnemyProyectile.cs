using UnityEngine;

public class EnemyProyectile : MonoBehaviour
{
    public float despawnInSecs;
    public float maxVelocity;
    public float accelerationPerSec;
    private float velocity = 0;

    private void Start()
    {
        Destroy(gameObject, despawnInSecs);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verificar si el objeto tiene el tag "Player"
        if (!collision.gameObject.CompareTag("Player")) return;
        
        // Obtener el script PlayerController del objeto colisionado
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player == null) return;
            
        // Si el jugador está en modo dash, no toma daño
        if (player.isDashing) return;
            
        // Aplicar daño al jugador
        player.TakeDamage(1);
        Destroy(gameObject); // Destruir proyectil

        // Opcional: Instanciar un efecto visual al impactar
        /*
         if (hitEffect != null)
         {
             Instantiate(hitEffect, transform.position, Quaternion.identity);
         }
         */
    }
    private void Update()
    {
        // Increment velocity based on acceleration and clamp to maxVelocity
        if (velocity < maxVelocity)
        {
            velocity += accelerationPerSec * Time.deltaTime;
            velocity = Mathf.Min(velocity, maxVelocity);
        }

        // Use the object's up direction for movement in 2D space
        Vector2 movement = (Vector2)transform.up * (velocity * Time.deltaTime);

        // Move the object in world space
        transform.Translate(movement, Space.World);
    }
}
