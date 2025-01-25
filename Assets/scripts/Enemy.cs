using UnityEngine;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour
{
    [Header("Atributos del enemigo")]
    [Tooltip("Si está en true, el enemigo mata al jugador en una colisión si esta no lo destruye antes.")]
    public bool garras = false;

    [Header("Parámetros de destrucción por choque")]
    [Tooltip("Fuerza mínima de colisión para destruir al enemigo.")]
    public float fuerzaDestruccion = 5f;

    [Header("Movimiento / Persecución")]
    [Tooltip("Velocidad base del enemigo")]
    public float velocidad = -5f;

    [Header("Alternancia de color y garras")]
    [Tooltip("Cada cuántos segundos alterna entre garras activas/rojas y garras inactivas/color original.")]
    public float intervaloCambiarGarras = 2f;

    // Referencias internas
    private Transform _player;          // Para perseguir al jugador
    private Vector2 velocidadActual;   // Velocidad (x, y) actual del enemigo
    private SpriteRenderer _spriteRenderer;
    private Color _colorOriginal;
    private Rigidbody2D _rb;

    private void Start()
    {
        // 1. Velocidad inicial en X
        velocidadActual = new Vector2(0f, 0f);

        // 2. Buscar al jugador (por etiqueta "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        // 3. Guardar referencia al SpriteRenderer y su color original
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
            _colorOriginal = _spriteRenderer.color;
        
        // 4. Guardar referencia al Rigidbody2D
        _rb = GetComponent<Rigidbody2D>();

        // 5. Iniciar la rutina que alterna color y garras cada X segundos
        StartCoroutine(ToggleGarrasColorRoutine());
    }

    private void Update()
    {
        MoverEnemigo();
        // MantenerDentroDeCamaraConRebote(); // No es necesario ya que la cámara sigue al jugador
    }

    /// <summary>
    /// Mueve al enemigo: rebote horizontal y persecución vertical suave.
    /// </summary>
    private void MoverEnemigo()
    {
        if (!_player) return;
        
        // Calculamos la dirección hacia el jugador
        Vector2 direction = (_player.transform.position - transform.position).normalized;

        // Calculamos la nueva posición
        Vector2 newPosition = _rb.position + direction * (velocidad * Time.fixedDeltaTime);

        // Movemos al enemigo usando Rigidbody2D.MovePosition
        _rb.MovePosition(newPosition);
    }

    /// <summary>
    /// Evita que el enemigo salga de los límites de la cámara rebotando en los bordes.
    /// </summary>
    private void MantenerDentroDeCamaraConRebote()
    {
        // Convertimos la posición del enemigo a coordenadas de viewport (0..1 dentro de la cámara).
        Vector3 posViewport = Camera.main.WorldToViewportPoint(transform.position);

        // Rebotar en X si toca el borde izquierdo (x < 0) o derecho (x > 1)
        if (posViewport.x <= 0f || posViewport.x >= 1f)
        {
            velocidadActual.x *= -1f;
            posViewport.x = Mathf.Clamp(posViewport.x, 0f, 1f);
        }

        // Rebotar en Y si toca el borde inferior (y < 0) o superior (y > 1)
        if (posViewport.y <= 0f || posViewport.y >= 1f)
        {
            velocidadActual.y *= -1f;
            posViewport.y = Mathf.Clamp(posViewport.y, 0f, 1f);
        }

        // Actualizamos la posición en coordenadas de mundo
        transform.position = Camera.main.ViewportToWorldPoint(posViewport);
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

            // Si estaba en false, activamos garras y color rojo
            // Si estaba en true, desactivamos garras y ponemos color original
            garras = !garras;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = garras ? Color.red : _colorOriginal;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos que el objeto que choca tenga la etiqueta "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Accedemos al script de control del jugador
            PlayerController playerController = collision.gameObject.GetOrAddComponent<PlayerController>();

            // Magnitud de la velocidad relativa en el impacto
            float fuerzaChoque = collision.relativeVelocity.magnitude;

            // Si el choque es suficientemente fuerte, destruimos al enemigo
            if (fuerzaChoque >= fuerzaDestruccion)
            {
                GameManager.Instance.ScoreCount(); //Contamos el score aqui
                Destroy(gameObject);
            }
            else
            {
                // Si la fuerza es menor y este enemigo tiene garras, daña al jugador
                if (garras)
                {
                    playerController.TakeDamage();
                    Debug.Log("¡El enemigo tiene garras y daña al jugador!");
                }
            }
        }
    }
}
