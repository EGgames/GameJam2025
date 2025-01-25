using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Parámetros de fuerza del impulso")]
    [Tooltip("Fuerza máxima que puede alcanzar el impulso con el mouse.")]
    public float maxForce = 10f;

    [Tooltip("Velocidad a la que se acumula la fuerza mientras se mantiene el mouse presionado.")]
    public float chargeRate = 5f;

    [Header("Parámetros de frenado (solo para el impulso)")]
    [Tooltip("Tiempo en segundos que tarda en frenar totalmente la parte del impulso.")]
    public float slowDownTime = 1f;

    [Header("Movimiento independiente con WASD")]
    [Tooltip("Velocidad de movimiento al usar WASD.")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb2D;                 // Referencia al Rigidbody2D
    private float currentForce = 0f;          // Fuerza acumulada del impulso
    private Vector2 impulseVelocity = Vector2.zero; // Velocidad que proviene del impulso

    private Coroutine slowDownCoroutine;      // Referencia a la corrutina de frenado

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
        // --- Carga de fuerza con el mouse ---
        if (Input.GetMouseButtonDown(0))
        {
            // Reiniciamos la fuerza acumulada
            currentForce = 0f;

            // Si había una corrutina de frenado en marcha, la detenemos
            if (slowDownCoroutine != null)
            {
                StopCoroutine(slowDownCoroutine);
                slowDownCoroutine = null;
            }
        }

        if (Input.GetMouseButton(0))
        {
            // Mientras mantengo el botón, sumo fuerza
            currentForce += chargeRate * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, 0f, maxForce);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Calculamos la dirección hacia el mouse
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPos - transform.position).normalized;

            // Convertimos esa fuerza a una velocidad "instantánea" (impulso)
            // impulseVelocity se sumará a wasdVelocity en FixedUpdate
            impulseVelocity = direction * currentForce;

            // Iniciamos la corrutina que frenará el impulso a lo largo de slowDownTime
            slowDownCoroutine = StartCoroutine(SlowDownImpulse());
        }
    }

    void FixedUpdate()
    {
        // Siempre calculamos la velocidad WASD (independiente del impulso)
        Vector2 wasdVelocity = GetWASDVelocity();

        // Sumamos la parte del impulso (que puede ir decayendo) con la parte del WASD
        rb2D.linearVelocity = wasdVelocity + impulseVelocity;
    }

    /// <summary>
    /// Corrutina que reduce gradualmente el impulso (impulseVelocity) a 0 en 'slowDownTime' segundos.
    /// </summary>
    private IEnumerator SlowDownImpulse()
    {
        Vector2 initialImpulse = impulseVelocity;   // Velocidad inicial del impulso
        float elapsed = 0f;

        while (elapsed < slowDownTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slowDownTime);

            // Hacemos Lerp del impulso desde el inicial hasta 0
            impulseVelocity = Vector2.Lerp(initialImpulse, Vector2.zero, t);

            // Esperamos al siguiente frame
            yield return null;
        }

        // Finalmente, aseguramos que el impulso sea 0
        impulseVelocity = Vector2.zero;
        slowDownCoroutine = null;
    }

    /// <summary>
    /// Calcula la velocidad deseada por WASD (movimiento independiente).
    /// </summary>
    private Vector2 GetWASDVelocity()
    {
        float x = Input.GetAxisRaw("Horizontal"); // -1, 0 o 1
        float y = Input.GetAxisRaw("Vertical");   // -1, 0 o 1

        // Normalizamos para no superar la magnitud 1 en diagonal
        Vector2 input = new Vector2(x, y).normalized;
        return input * moveSpeed;
    }
}