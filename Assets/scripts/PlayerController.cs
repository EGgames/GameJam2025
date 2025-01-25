using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [FormerlySerializedAs("lives")]
    [Header("Vida del jugador")]
    [Tooltip("Cantidad de vida que contrndra el jugador en la partida")]
    public int health;

    [Header("Parámetros de fuerza del impulso (clic)")]
    [Tooltip("Fuerza máxima que puede alcanzar el impulso al soltar el clic.")]
    public float maxForce = 10f;

    [Tooltip("Velocidad a la que se acumula la fuerza mientras se mantiene el mouse presionado.")]
    public float chargeRate = 5f;

    [Tooltip("Tiempo en segundos que tarda en reducir la velocidad del impulso hasta 0.")]
    public float slowDownTime = 1f;
    
    [Header("Sistema de combustible")]
    [Tooltip("Capacidad máxima de combustible.")]
    public float maxFuel = 100f;

    [Tooltip("Tasa de consumo de combustible mientras se carga el impulso.")]
    public float fuelConsumptionRate = 10f;

    [Tooltip("Cantidad de combustible que se recarga por segundo.")]
    public float fuelRechargeRate = 5f;

    [Header("Movimiento flotante con WASD (independiente)")]
    [Tooltip("Velocidad máxima al moverse con WASD.")]
    public float moveSpeed = 5f;

    [Tooltip("Aceleración al pulsar WASD (cuanto más alto, más rápido alcanza moveSpeed).")]
    public float wasdAcceleration = 10f;

    [Tooltip("Fricción (desaceleración) cuando se sueltan las teclas WASD.")]
    public float wasdFriction = 5f;

    [Header("Clamping en la cámara")]
    [Tooltip("Si usas un collider, puedes tomar extents automáticamente. O asignar manualmente el medio ancho y medio alto.")]
    public bool useColliderBounds = true;

    private Rigidbody2D rb2D;

    // --- Impulso con el mouse ---
    private float currentForce = 0f;
    private Vector2 impulseVelocity = Vector2.zero;
    private Coroutine slowDownCoroutine;
    
    // --- Sistema de combustible ---
    private float currentFuel;
        
    // --- Movimiento flotante con WASD ---
    private Vector2 wasdVelocity = Vector2.zero;

    // --- Clamping en la cámara ---
    private float halfWidth = 0.5f;
    private float halfHeight = 0.5f;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        currentFuel = maxFuel;

        // Si queremos tomar automáticamente el tamaño del Collider2D
        if (useColliderBounds)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null)
            {
                halfWidth = col.bounds.extents.x;
                halfHeight = col.bounds.extents.y;
            }
        }
    }

    void Update()
    {
        ChargeImpulse();
        RechargeFuel();
        
        // Actualizar la UI del combustible
        GameManager.Instance.UpdateFuelAmount(currentFuel);
    }

    void FixedUpdate()
    {
        // ========== Movimiento flotante con WASD ==========

        // 1. Leer la entrada (Horizontal y Vertical)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(x, y).normalized;

        // 2. Si hay input, aceleramos hacia la velocidad deseada
        if (input.magnitude > 0.01f)
        {
            // Añadimos aceleración en la dirección del input
            wasdVelocity += input * (wasdAcceleration * Time.fixedDeltaTime);

            // Limitamos a la velocidad máxima "moveSpeed"
            if (wasdVelocity.magnitude > moveSpeed)
            {
                wasdVelocity = wasdVelocity.normalized * moveSpeed;
            }
        }
        else
        {
            // 3. Si NO hay input, aplicamos fricción (desaceleración suave)
            float decrease = wasdFriction * Time.fixedDeltaTime;
            float speed = wasdVelocity.magnitude;

            // Reducimos la magnitud de la velocidad poco a poco
            speed = Mathf.Max(0f, speed - decrease);

            // Mantener la dirección, pero con menor magnitud
            wasdVelocity = wasdVelocity.normalized * speed;
        }

        // 4. Sumar la velocidad de WASD + el impulso (clic)
        rb2D.linearVelocity = wasdVelocity + impulseVelocity;
    }

    void LateUpdate()
    {
        // Evitar que se salga de la cámara ortográfica
        // ClampPositionToCamera(); // No es necesario ya que la cámara sigue al jugador
    }

    private void ChargeImpulse()
    {
        // Si no hay suficiente combustible, no permitir cargar el impulso
        if (currentFuel <= 0f) return;

        float fuelConsumed;
        
        // ========== Impulso con el mouse ==========
        if (Input.GetMouseButtonDown(0))
        {
            currentForce = 0f;
        }

        if (Input.GetMouseButton(0))
        {
            // Acumulamos fuerza mientras se mantiene el botón
            currentForce += chargeRate * Time.deltaTime;
            
            // Fuerza máxima depende del combustible disponible
            // si combustible = maxFuel, entonces maxForce
            // si combustible = 0, entonces 0
            var finalMaxForce = Mathf.Lerp(0f, maxForce, currentFuel / maxFuel);
            currentForce = Mathf.Clamp(currentForce, 0f, finalMaxForce);
        }

        if (Input.GetMouseButtonUp(0))
        {
            // Al soltar, aplicamos una "velocidad" de impulso
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPos - transform.position).normalized;
            impulseVelocity = direction * currentForce;
            
            // Consumir combustible
            currentFuel = Mathf.Max(0f, currentFuel - fuelConsumptionRate);

            // Iniciamos la corrutina que frena el impulso poco a poco
            slowDownCoroutine = StartCoroutine(SlowDownImpulse());
        }
    }
    
    private void RechargeFuel()
    {
        // Recargar combustible si no estamos cargando el impulso y ya no hay velocidad residual de impulso
        if (!Input.GetMouseButton(0) && impulseVelocity == Vector2.zero)
        {
            currentFuel = Mathf.Min(maxFuel, currentFuel + fuelRechargeRate * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Corrutina para frenar gradualmente el impulso (impulseVelocity) en slowDownTime segundos.
    /// </summary>
    private IEnumerator SlowDownImpulse()
    {
        Vector2 startImpulse = impulseVelocity;
        float elapsed = 0f;

        while (elapsed < slowDownTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slowDownTime);

            // Reducir progresivamente de la velocidad inicial hasta 0
            impulseVelocity = Vector2.Lerp(startImpulse, Vector2.zero, t);

            yield return null;
        }

        impulseVelocity = Vector2.zero;
        slowDownCoroutine = null;
    }

    /// <summary>
    /// Mantiene la posición dentro de los límites de la cámara ortográfica.
    /// </summary>
    private void ClampPositionToCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        if (cam.orthographic)
        {
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = camHalfHeight * cam.aspect;

            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(
                pos.x,
                cam.transform.position.x - camHalfWidth + halfWidth,
                cam.transform.position.x + camHalfWidth - halfWidth
            );
            pos.y = Mathf.Clamp(
                pos.y,
                cam.transform.position.y - camHalfHeight + halfHeight,
                cam.transform.position.y + camHalfHeight - halfHeight
            );

            transform.position = pos;
        }
        else
        {
            Debug.LogWarning("La cámara no es ortográfica; se requiere otra lógica para el clamping en perspectiva.");
        }
    }
    /// <summary>
    /// Se muere
    /// </summary>
    public void KillPlayer()
    {
        Debug.Log("El jugador ha muerto");
        GameManager.Instance.GameOver();
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Toma daño
    /// </summary>
    public void TakeDamage(int damage = 1)
    {
        health -= damage;
        GameManager.Instance.UpdateLives(health);
        if (health <= 0)
        {
            KillPlayer();
        }
    }
}