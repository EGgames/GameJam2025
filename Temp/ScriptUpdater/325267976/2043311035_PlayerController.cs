using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Parámetros de fuerza del impulso")]
    [Tooltip("Fuerza máxima que puede alcanzar el impulso.")]
    public float maxForce = 10f;

    [Tooltip("Velocidad a la que se acumula la fuerza mientras se mantiene el mouse presionado.")]
    public float chargeRate = 5f;

    [Header("Parámetros de frenado")]
    [Tooltip("Tiempo en segundos que tarda en frenar completamente después del impulso.")]
    public float slowDownTime = 1f;

    [Header("Movimiento con WASD")]
    [Tooltip("Velocidad de movimiento con teclas WASD.")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb2D;            // Referencia al Rigidbody2D
    private float currentForce = 0f;     // Fuerza acumulada
    private Coroutine slowDownCoroutine; // Corrutina de frenado, por si queremos cancelarla si se impulsa de nuevo

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogError("Este script requiere un componente Rigidbody2D en el mismo GameObject.");
        }
    }

    void Update()
    {
        // --- Manejo de Impulso con el Mouse ---
        if (Input.GetMouseButtonDown(0))
        {
            // Reiniciamos la fuerza acumulada
            currentForce = 0f;

            // Si estaba frenando de un impulso anterior, lo detenemos
            if (slowDownCoroutine != null)
            {
                StopCoroutine(slowDownCoroutine);
                slowDownCoroutine = null;
            }
        }

        // Mientras se mantenga presionado, acumulamos fuerza
        if (Input.GetMouseButton(0))
        {
            currentForce += chargeRate * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, 0f, maxForce);
        }

        // Al soltar el botón, aplicamos la fuerza y comenzamos a frenar
        if (Input.GetMouseButtonUp(0))
        {
            // Obtenemos la posición del mouse en coordenadas de mundo
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculamos la dirección 2D normalizada
            Vector2 direction = (mouseWorldPos - transform.position).normalized;

            // (Opcional) Reiniciamos la velocidad existente antes de lanzar, para no mezclar velocidades previas
            rb2D.linearVelocity = Vector2.zero;

            // Aplicamos el impulso
            rb2D.AddForce(direction * currentForce, ForceMode2D.Impulse);

            // Iniciamos la corrutina de frenado
            slowDownCoroutine = StartCoroutine(SlowDown());
        }

        // --- Movimiento con WASD (lo controlamos en Update o FixedUpdate; aquí se hace un approach simple) ---
        HandleWASDMovement();
    }

    /// <summary>
    /// Corrutina que reduce la velocidad del último impulso a 0 en 'slowDownTime' segundos,
    /// pero *suma* la velocidad generada por WASD cada frame.
    /// </summary>
    private IEnumerator SlowDown()
    {
        Vector2 impulseVelocity = rb2D.linearVelocity; // Velocidad justo después de impulsar
        float elapsedTime = 0f;

        while (elapsedTime < slowDownTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / slowDownTime);

            // Velocidad proveniente del impulso (lerp de la velocidad inicial a 0)
            Vector2 impulseLerp = Vector2.Lerp(impulseVelocity, Vector2.zero, t);

            // Sumamos la velocidad de WASD (en este caso, la calculamos en cada frame aquí mismo)
            Vector2 wasdVelocity = GetWASDVelocity();

            // La "mezcla" final de velocidades
            rb2D.linearVelocity = impulseLerp + wasdVelocity;

            yield return null;
        }

        // Al acabar el frenado, forzamos la velocidad a 0. 
        // OPCIONAL: Si no quieres cortar en seco, quita o modifica esta línea.
        rb2D.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// Aplica movimiento con WASD. En este caso, ajustamos directamente la velocidad
    /// del rigidbody en vez de usar AddForce, para mayor control.
    /// </summary>
    private void HandleWASDMovement()
    {
        // Solo aplicamos en Update; si quieres un control más estable, puedes mover esta lógica a FixedUpdate.
        Vector2 wasdVelocity = GetWASDVelocity();

        // Si no estamos en el SlowDown, dejamos que la física maneje la velocidad.
        // Si SÍ estamos en el SlowDown, la corrutina sumará este valor cada frame.
        // Por eso, aquí solo aplicamos wasdVelocity si NO hay corrutina corriendo.
        if (slowDownCoroutine == null)
        {
            rb2D.linearVelocity = wasdVelocity;
        }
    }

    /// <summary>
    /// Calcula la velocidad base de WASD sin aplicarla aún.
    /// </summary>
    private Vector2 GetWASDVelocity()
    {
        float x = Input.GetAxisRaw("Horizontal"); // -1, 0 o 1
        float y = Input.GetAxisRaw("Vertical");   // -1, 0 o 1
        Vector2 input = new Vector2(x, y).normalized; // Normalizamos para no superar 1 en diagonal

        return input * moveSpeed;
    }
}