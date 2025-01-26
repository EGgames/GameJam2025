using UnityEngine;

public class ProjectilePlayer : MonoBehaviour
{
    private Vector2 direction;
    private float speed;

    [Tooltip("Tiempo en segundos antes de que el proyectil sea destruido automáticamente.")]
    public float lifeTime = 5f;

    [Tooltip("Capa de objetos que el proyectil puede impactar.")]
    public LayerMask collisionLayers;

    //[Tooltip("Efecto visual al colisionar (opcional).")]
    //public GameObject hitEffect;

    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;
    
    private bool isPoweredUp;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Programar la destrucción del proyectil después de 'lifeTime' segundos
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Mover el proyectil en la dirección asignada
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    /// <summary>
    /// Configura la dirección y velocidad del proyectil.
    /// </summary>
    /// <param name="dir">Dirección del movimiento.</param>
    /// <param name="spd">Velocidad del proyectil.</param>
    public void SetDirection(Vector2 dir, float spd)
    {
        direction = dir.normalized;
        speed = spd;
    }
    
    /// <summary>
    /// Obtiene la dirección del proyectil.
    /// </summary>
    /// <returns>Dirección del proyectil.</returns>
    public Vector2 GetDirection()
    {
        return direction;
    }

    /// <summary>
    /// Obtiene la velocidad del proyectil.
    /// </summary>
    /// <returns>Velocidad del proyectil.</returns>
    public float GetSpeed()
    {
        return speed;
    }
    
    public void BouncePowerUp()
    {
        isPoweredUp = true;
        // Cambiamos el color del sprite
        spriteRenderer.color = Color.magenta;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ignorar colisiones con ciertos objetos basados en etiquetas
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Projectile") || collision.gameObject.CompareTag("Jellyfish"))
            return;

        // Verificar si el objeto tiene el tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Obtener el script Enemy del objeto colisionado
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                //Suma un punto
                GameManager.Instance.ScoreCount();
                // Aplicar daño al enemigo
                enemy.TakeDamage(1); // En este caso, 1 daño es suficiente para destruir al enemigo
                Destroy(gameObject); //Destruimos el proyectil
            }
            else
            {
                Debug.LogWarning($"El objeto {collision.gameObject.name} no tiene el script Enemy.");
            }
        }

        // Opcional: Instanciar un efecto visual al impactar
        /*
         if (hitEffect != null)
         {
             Instantiate(hitEffect, transform.position, Quaternion.identity);
         }
         */

    }
}

