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
    [Tooltip("Velocidad horizontal base del enemigo. Si es negativa, empezará moviéndose a la izquierda.")]
    public float velocidadHorizontal = -5f;

    [Tooltip("Velocidad máxima para ajustar posición vertical hacia el jugador.")]
    public float velocidadVerticalMax = 2f;

    [Tooltip("Factor de suavizado (cuanto mayor, más lentamente se ajusta la velocidad vertical).")]
    public float suavizadoVertical = 2f;

    [Header("Alternancia de color y garras")]
    [Tooltip("Cada cuántos segundos alterna entre garras activas/rojas y garras inactivas/color original.")]
    public float intervaloCambiarGarras = 2f;

    // Referencias internas
    private Transform player;          // Para perseguir al jugador
    private Vector2 velocidadActual;   // Velocidad (x, y) actual del enemigo
    private SpriteRenderer spriteRenderer;
    private Color colorOriginal;

    private void Start()
    {
        // 1. Velocidad inicial en X
        velocidadActual = new Vector2(velocidadHorizontal, 0f);

        // 2. Buscar al jugador (por etiqueta "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // 3. Guardar referencia al SpriteRenderer y su color original
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            colorOriginal = spriteRenderer.color;

        // 4. Iniciar la rutina que alterna color y garras cada X segundos
        StartCoroutine(ToggleGarrasColorRoutine());
    }

    private void Update()
    {
        MoverEnemigo();
        MantenerDentroDeCamaraConRebote();
    }

    /// <summary>
    /// Mueve al enemigo: rebote horizontal y persecución vertical suave.
    /// </summary>
    private void MoverEnemigo()
    {
        // Perseguir al jugador en Y
        if (player != null)
        {
            // Calcular la diferencia en Y respecto al jugador
            float diferenciaY = player.position.y - transform.position.y;

            // Limitamos la velocidad vertical para que no sea excesivamente agresivo
            float velocidadDeseadaY = Mathf.Clamp(diferenciaY, -velocidadVerticalMax, velocidadVerticalMax);

            // Suavizamos la transición entre la velocidadActual.y y la deseada
            velocidadActual.y = Mathf.Lerp(velocidadActual.y, velocidadDeseadaY, Time.deltaTime * suavizadoVertical);
        }

        // Aplicamos la traslación (x, y) con la velocidad actual
        transform.Translate(velocidadActual * Time.deltaTime);
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
            if (spriteRenderer != null)
            {
                spriteRenderer.color = garras ? Color.red : colorOriginal;
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
                Destroy(gameObject);
            }
            else
            {
                // Si la fuerza es menor y este enemigo tiene garras, mata al jugador
                if (garras)
                {
                    playerController.KillPlayer();
                    Debug.Log("¡El enemigo tiene garras y mata al jugador!");
                }
            }
        }
    }
}
