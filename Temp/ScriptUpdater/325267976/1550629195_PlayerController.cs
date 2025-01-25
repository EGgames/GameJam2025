using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Parámetros de fuerza del impulso")]
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

    private Rigidbody2D rb2D;
    private float currentForce = 0f;             // Fuerza acumulada para el impulso
    private Vector2 impulseVelocity = Vector2.zero; // Velocidad extra proveniente del clic

    private Coroutine slowDownCoroutine;          // Referencia a la corrutina de frenado (por si queremos cancelarla)

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogError("Este script requiere un Rigidbody2D en el GameObject.");
        }
    }

    void Update()
    {
        // 1. Acumular fuerza al mantener pulsado el clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            // Reiniciamos la fuerza acumulada
            currentForce = 0f;

            // Si estaba frenando un impulso anterior, lo detenemos
            if (slowDownCoroutine != null)
            {
                StopCoroutine(slowDownCoroutine);
                slowDownCoroutine = null;
            }
        }

        if (Input.GetMouseButton(0))
        {
            // Aumentamos la fuerza mientras se mantenga el botón
            currentForce += chargeRate * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, 0f, maxForce);
        }

        // 2. Al soltar, calculamos la dirección y aplicamos un "impulso virtual"
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPos - transform.position).normalized;

            // Asignamos la velocidad de impulso (no usamos AddForce para poder separarla de WASD)
            impulseVelocity = direction * currentForce;

            // Iniciamos la corrutina que irá reduciendo la impulseVelocity a 0
            slowDownCoroutine = StartCoroutine(SlowDownImpulse());
        }
    }

    void FixedUpdate()
    {
        // 3. Calcular la velocidad independiente de WASD cada FixedUpdate
        Vector2 wasdVelocity = GetWASDVelocity();

        // 4. Sumar ambas velocidades y asignarlas al Rigidbody2D
        rb2D.linearVelocity = wasdVelocity + impulseVelocity;
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

            // Hacemos "lerp" de la velocidad inicial del impulso hasta 0
            impulseVelocity = Vector2.Lerp(initialImpulse, Vector2.zero, t);

            // Esperamos hasta el siguiente frame
            yield return null;
        }

        // Aseguramos que la parte de impulso quede en 0 al finalizar
        impulseVelocity = Vector2.zero;
        slowDownCoroutine = null;
    }

    /// <summary>
    /// Devuelve la velocidad deseada de WASD (movimiento continuo).
    /// </summary>
    private Vector2 GetWASDVelocity()
    {
        float x = Input.GetAxisRaw("Horizontal"); // -1, 0 o 1
        float y = Input.GetAxisRaw("Vertical");   // -1, 0 o 1

        // Normaliza para no superar magnitud 1 en diagonal
        Vector2 input = new Vector2(x, y).normalized;

        // Multiplicamos por la velocidad configurada
        return input * moveSpeed;
    }
}