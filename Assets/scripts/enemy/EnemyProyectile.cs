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
        // Verificar si el objeto colisionado está en las capas especificadas

        // Verificar si el objeto tiene el tag "Enemy"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtener el script Enemy del objeto colisionado
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                //Suma un punto
                player.TakeDamage();
                // Aplicar daño al enemigo
                player.TakeDamage(1); // En este caso, 1 daño es suficiente para destruir al enemigo
            }
            else
            {
                Debug.LogWarning($"El objeto {collision.gameObject.name} no tiene el script PlayerController.");
            }
        }

        // Ignorar colisiones con el propio jugador
        if (collision.gameObject.CompareTag("Enemy"))
            return;

        // Opcional: Instanciar un efecto visual al impactar
        /*
         if (hitEffect != null)
         {
             Instantiate(hitEffect, transform.position, Quaternion.identity);
         }
         */

        // Destruir el proyectil tras la colisión
        Destroy(gameObject);
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
        Vector2 movement = (Vector2)transform.up * velocity * Time.deltaTime;

        // Move the object in world space
        transform.Translate(movement, Space.World);
    }
}
