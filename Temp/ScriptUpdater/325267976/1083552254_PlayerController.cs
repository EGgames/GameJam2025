using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Parámetros de fuerza del impulso (clic)")]
    [Tooltip("Fuerza máxima que puede alcanzar el impulso al soltar el clic.")]
    public float maxForce = 10f;

    [Tooltip("Velocidad a la que se acumula la fuerza mientras se mantiene el mouse presionado.")]
    public float chargeRate = 5f;

    [Header("Parámetros de frenado (solo afecta al impulso)")]
    [Tooltip("Tiempo en segundos que tarda en reducir la velocidad del impulso hasta 0.")]
    public float slowDownTime = 1f;

    [Header("Movimiento con WASD (independiente)")]
    [Tooltip("Velocidad de movimiento con teclas WASD.")]
    public float moveSpeed = 5f;

    [Header("Tamaño del collider (opcional)")]
    [Tooltip("Si usas un collider, puedes tomar extents automáticamente. O asignar manualmente el medio ancho y medio alto.")]
    public bool useColliderBounds = true;

    private Rigidbody2D rb2D;
    private float currentForce = 0f;                // Fuerza acumulada para el impulso
    private Vector2 impulseVelocity = Vector2.zero; // Velocidad extra proveniente del clic
    private Coroutine slowDownCoroutine;

    // Variables para el clamping
    private float halfWidth = 0.5f;   // Mitad del ancho del sprite/objeto
    private float halfHeight = 0.5f;  // Mitad de la altura del sprite/objeto

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        // Opcional: Tomar el tamaño de un Collider2D para el clamping más preciso
        if (useColliderBounds)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                // Extents = la mitad del tamaño total del collider
                halfWidth = col.bounds.extents.x;
                halfHeight = col.bounds.extents.y;
            }
        }
    }

    void Update()
    {
        // 1. Acumular fuerza al mantener pulsado el clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            currentForce = 0f; // Reiniciamos la fuerza
            if (slowDownCoroutine != null)
            {
                StopCoroutine(slowDownCoroutine);
                slowDownCoroutine = null;
            }
        }

        if (Input.GetMouseButton(0))
        {
            currentForce += chargeRate * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, 0f, maxForce);
        }

        // 2. Al soltar, calculamos la dirección y aplicamos el “impulso virtual”
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPos - transform.position).normalized;

            impulseVelocity = direction * currentForce;

            // Iniciamos la corrutina para frenar gradualmente
            slowDownCoroutine = StartCoroutine(SlowDownImpulse());
        }
    }

    void FixedUpdate()
    {
        // 3. Calcular la velocidad independiente de WASD
        Vector2 wasdVelocity = GetWASDVelocity();

        // 4. Sumar ambas velocidades y asignarlas
        rb2D.linearVelocity = wasdVelocity + impulseVelocity;
    }

    void LateUpdate()
    {
        // 5. Por último, después de aplicar la velocidad en FixedUpdate, forzamos el objeto a no salirse de la cámara
        ClampPositionToCamera();
    }

    /// <summary>
    /// Corrutina que va reduciendo impulseVelocity a 0 durante slowDownTime segundos.
    /// </summary>
    private IEnumerator SlowDownImpulse()
    {
        Vector2 initialImpulse = impulseVelocity;
        float elapsed = 0f;

        while (elapsed < slowDownTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slowDownTime);

            // Interpolamos desde la velocidad inicial hasta 0
            impulseVelocity = Vector2.Lerp(initialImpulse, Vector2.zero, t);

            yield return null;
        }

        // Forzamos a 0 al terminar
        impulseVelocity = Vector2.zero;
        slowDownCoroutine = null;
    }

    /// <summary>
    /// Retorna la velocidad deseada de WASD (movimiento continuo).
    /// </summary>
    private Vector2 GetWASDVelocity()
    {
        float x = Input.GetAxisRaw("Horizontal"); // -1, 0 o 1
        float y = Input.GetAxisRaw("Vertical");   // -1, 0 o 1

        // Normaliza para no superar magnitud 1 en diagonal
        Vector2 input = new Vector2(x, y).normalized;
        return input * moveSpeed;
    }

    /// <summary>
    /// Limita la posición del objeto dentro de los límites de la cámara ortográfica.
    /// </summary>
    private void ClampPositionToCamera()
    {
        Camera cam = Camera.main;

        if (cam.orthographic)
        {
            // Obtenemos la mitad de la altura de la cámara
            float camHalfHeight = cam.orthographicSize;
            // La mitad del ancho depende de la relación de aspecto
            float camHalfWidth = camHalfHeight * cam.aspect;

            Vector3 pos = transform.position;

            // Ajusta la posición para que no salga de la vista de la cámara
            pos.x = Mathf.Clamp(pos.x,
                cam.transform.position.x - camHalfWidth + halfWidth,
                cam.transform.position.x + camHalfWidth - halfWidth);

            pos.y = Mathf.Clamp(pos.y,
                cam.transform.position.y - camHalfHeight + halfHeight,
                cam.transform.position.y + camHalfHeight - halfHeight);

            transform.position = pos;
        }
        else
        {
            // Si no es ortográfico, el clamping es más complejo (depende de la distancia, FOV, etc.).
            // Aquí podrías implementar tu propia lógica o advertir al usuario.
            Debug.LogWarning("La cámara no es ortográfica; se necesita un método diferente para el clamping.");
        }
    }
}