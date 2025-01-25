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

    [Tooltip("Tiempo de invulnerabilidad después de recibir daño.")]
    public float damageCooldown = 1f;

    [Header("Parámetros de fuerza del impulso (barra espaciadora)")]
    [Tooltip("Fuerza máxima que puede alcanzar el impulso al soltar la barra espaciadora.")]
    public float maxForce = 10f;

    [Tooltip("Velocidad a la que se acumula la fuerza mientras se mantiene la barra espaciadora presionada.")]
    public float chargeRate = 5f;

    [Tooltip("Tiempo en segundos que tarda en reducir la velocidad del impulso hasta 0.")]
    public float slowDownTime = 1f;

    [Header("Sistema de combustible")]
    [Tooltip("Capacidad máxima de combustible.")]
    public float maxFuel = 100f;

    [Tooltip("Tasa de consumo de combustible por segundo mientras mantenemos la barra espaciadora presionada.")]
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

    // --- Disparo de Proyectiles ---
    [Header("Sistema de Disparo")]
    [Tooltip("Prefab del proyectil a disparar.")]
    public GameObject projectilePrefab;

    [Tooltip("Punto desde donde se disparan los proyectiles.")]
    public Transform firePoint;

    [Tooltip("Velocidad del proyectil.")]
    public float projectileSpeed = 20f;

    [Tooltip("Tiempo entre disparos para limitar la tasa de disparo.")]
    public float fireRate = 0.5f;

    private float nextFireTime = 0f;

    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;

    // --- Impulso con la barra espaciadora ---
    private float currentForce = 0f;
    private Vector2 impulseVelocity = Vector2.zero;
    private Coroutine slowDownCoroutine;
    public bool isDashing;

    // --- Sistema de combustible ---
    private float currentFuel;

    // --- Movimiento flotante con WASD ---
    private Vector2 wasdVelocity = Vector2.zero;

    // --- Clamping en la cámara ---
    private float halfWidth = 0.5f;
    private float halfHeight = 0.5f;

    private float currentDamageCooldown = 0f;

    // Variable para guardar cuándo se inicia la barra espaciadora
    private float pressStartTime;

    // Variable para almacenar la última dirección de movimiento
    private Vector2 lastMoveDirection = Vector2.up; // Dirección predeterminada

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        // Manejar el disparo de proyectiles
        HandleShooting();
    }

    void FixedUpdate()
    {
        // ========== Movimiento flotante con WASD ==========

        // 1. Leer la entrada (Horizontal y Vertical)
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(x, y).normalized;

        if (input.magnitude > 0.01f)
        {
            // Guardar la última dirección de movimiento
            lastMoveDirection = input;

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

            if (speed > 0f)
            {
                wasdVelocity = wasdVelocity.normalized * speed;
            }
            else
            {
                wasdVelocity = Vector2.zero;
            }
        }

        // 4. Sumar la velocidad de WASD + el impulso (barra espaciadora)
        rb2D.linearVelocity = wasdVelocity + impulseVelocity;
    }

    private void HandleShooting()
    {
        // Verificar si se ha presionado el botón izquierdo del mouse y si el tiempo actual es mayor que nextFireTime
        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            ShootProjectile();
            nextFireTime = Time.time + fireRate;
        }
    }


    private void ShootProjectile()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Prefab del proyectil o FirePoint no están asignados en el PlayerController.");
            return;
        }

        // Instanciar el proyectil en la posición del firePoint
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Obtener la posición del mouse en el mundo
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - firePoint.position).normalized;

        // Asignar la dirección y velocidad al proyectil
        ProjectilePlayer projectileScript = projectile.GetComponent<ProjectilePlayer>();
        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction, projectileSpeed);
        }
        else
        {
            Debug.LogWarning("El prefab del proyectil no tiene el script 'Projectile' asignado.");
        }
    }


    private void ChargeImpulse()
    {
        // Si no hay suficiente combustible, no permitir cargar el impulso
        if (currentFuel <= 0f) return;

        // ========== Impulso con la barra espaciadora ==========
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Registramos el momento en que se inició la barra espaciadora
            pressStartTime = Time.time;
            currentForce = 0f;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            // Acumulamos fuerza mientras se mantiene la barra espaciadora
            currentForce += chargeRate * Time.deltaTime;

            // Fuerza máxima depende del combustible disponible
            var finalMaxForce = Mathf.Lerp(0f, maxForce, currentFuel / maxFuel);
            currentForce = Mathf.Clamp(currentForce, 0f, finalMaxForce);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Definir la dirección del impulso basado en la última dirección de movimiento
            Vector2 direction = lastMoveDirection;

            // Si el jugador no se está moviendo, define una dirección por defecto
            if (direction == Vector2.zero)
            {
                direction = Vector2.up; // Por ejemplo, hacia arriba
            }

            // Aplicamos una "velocidad" de impulso
            impulseVelocity = direction * currentForce;

            // Calcular la duración de la barra espaciadora presionada
            float pressDuration = Time.time - pressStartTime;
            // El consumo de combustible es proporcional al tiempo presionado, con un tope máximo de fuelConsumptionRate
            float fuelToConsume = Mathf.Min(pressDuration * fuelConsumptionRate, fuelConsumptionRate);
            // Consumir combustible
            currentFuel = Mathf.Max(0f, currentFuel - fuelToConsume);

            // Activamos el dash
            isDashing = true;

            // Iniciamos la corrutina que frena el impulso poco a poco
            slowDownCoroutine = StartCoroutine(SlowDownImpulse());
        }
    }

    private void RechargeFuel()
    {
        // Recargar combustible si no estamos cargando el impulso y ya no hay velocidad residual de impulso
        if (!Input.GetKey(KeyCode.Space) && impulseVelocity == Vector2.zero)
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
        isDashing = false;
    }

    private IEnumerator BeginDamageCooldown()
    {
        currentDamageCooldown = damageCooldown;
        // Le damos un efecto visual de invulnerabilidad bajando la opacidad
        spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        while (currentDamageCooldown > 0f)
        {
            currentDamageCooldown = Mathf.Max(0, currentDamageCooldown - Time.deltaTime);
            yield return null;
        }
        spriteRenderer.color = Color.white;
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
        if (currentDamageCooldown > 0f) return;
        health -= damage;
        GameManager.Instance.UpdateLives(health);
        StartCoroutine(BeginDamageCooldown());
        if (health <= 0)
        {
            KillPlayer();
        }
    }

    public Vector2 GetCurrentVelocity()
    {
        // Devuelve la suma actual que usa el Player
        return wasdVelocity + impulseVelocity;
    }

    // Este método lo llamará Gellyfish cuando quiera cambiar la velocidad
    public void ApplyExternalVelocity(Vector2 external)
    {
        // Aquí decides cómo lo integras. Por ejemplo:
        impulseVelocity = external;
        // O lo sumas, o lo mezclas con la velocidad que ya traías, etc.

        // Iniciamos la corrutina de frenado
        if (slowDownCoroutine != null)
        {
            StopCoroutine(slowDownCoroutine);
        }
        slowDownCoroutine = StartCoroutine(SlowDownImpulse());
    }
}
