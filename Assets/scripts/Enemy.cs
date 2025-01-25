using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Atributos del Enemigo")]
    [Tooltip("Vida del enemigo.")]
    public int health = 1; // Establece a 1 para que muera con un solo disparo

    [Tooltip("Si está en true, el enemigo mata al jugador en una colisión si este no lo destruye antes.")]
    public bool garras = false;

    [Tooltip("Referencia al objeto con el collider de ataque")]
    public GameObject _attackColliderObj;

    [Header("Movimiento / Persecución")]
    [Tooltip("Velocidad base del enemigo")]
    public float velocidad = 5f;

    [Header("Alternancia de color y garras")]
    [Tooltip("Cada cuántos segundos alterna entre garras activas/rojas y garras inactivas/color original.")]
    public float intervaloCambiarGarras = 2f;

    [Tooltip("Efecto visual al morir (opcional).")]
    public GameObject deathEffect;

    // Referencias internas
    private Transform _player;          // Para perseguir al jugador
    private SpriteRenderer _spriteRenderer;
    private Color _colorOriginal;
    private Rigidbody2D _rb;

    private void Start()
    {
        // Buscar al jugador (por etiqueta "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        // Guardar referencia al SpriteRenderer y su color original
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
            _colorOriginal = _spriteRenderer.color;

        // Guardar referencia al Rigidbody2D
        _rb = GetComponent<Rigidbody2D>();

        // Iniciar la rutina que alterna color y garras cada X segundos
        StartCoroutine(ToggleGarrasColorRoutine());
    }

    private void Update()
    {
        MoverEnemigo();
    }

    /// <summary>
    /// Mueve al enemigo hacia el jugador.
    /// </summary>
    private void MoverEnemigo()
    {
        if (!_player) return;

        // Calculamos la dirección hacia el jugador
        Vector2 direction = (_player.position - transform.position).normalized;

        // Calculamos la nueva posición
        Vector2 newPosition = _rb.position + direction * (velocidad * Time.deltaTime);

        // Movemos al enemigo usando Rigidbody2D.MovePosition
        _rb.MovePosition(newPosition);
    }

    /// <summary>
    /// Rutina que alterna cada 'intervaloCambiarGarras' segundos:
    /// - Garras activas (color rojo)
    /// - Garras inactivas (color original)
    /// </summary>
    private System.Collections.IEnumerator ToggleGarrasColorRoutine()
    {
        while (true)
        {
            // Esperamos X segundos antes de cambiar de estado
            yield return new WaitForSeconds(intervaloCambiarGarras);

            // Alternamos el estado de garras
            garras = !garras;
            _attackColliderObj.SetActive(garras);
            if (_spriteRenderer)
            {
                _spriteRenderer.color = garras ? Color.red : _colorOriginal;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos si el objeto colisionado es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtener el script del jugador
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (!playerController.isDashing)
                {
                    // Aplicar daño al jugador
                    playerController.TakeDamage(1); // Ajusta la cantidad de daño según sea necesario
                }
                if (playerController.isPoweredUp)
                {
                    // Si el jugador está en modo powered up, matar al enemigo
                    Die();
                }
            }
        }
    }

    /// <summary>
    /// Método para aplicar daño al enemigo.
    /// </summary>
    /// <param name="damage">Cantidad de daño a aplicar.</param>
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} daño. Salud restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Método que se llama cuando el enemigo muere.
    /// </summary>
    private void Die()
    {
        // Opcional: Instanciar un efecto visual al morir
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Destruir el enemigo
        Destroy(gameObject);
        GameManager.Instance.ScoreCount(); // Contar el score aquí
    }
}
