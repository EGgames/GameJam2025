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
    [Tooltip("Velocidad máxima al moverse con WASD.")]
    public float moveSpeed = 5f;

    [Tooltip("Aceleración al pulsar WASD (cuanto más alto, más rápido llega a la velocidad objetivo).")]
    public float wasdAcceleration = 15f;

    [Tooltip("Desaceleración cuando se sueltan las teclas WASD (cuanto más alto, más rápido se detiene).")]
    public float wasdDeceleration = 15f;

    [Header("Tamaño del collider (clamping)")]
    [Tooltip("Si usas un collider, puedes tomar extents automáticamente. O asignar manualmente el medio ancho y medio alto.")]
    public bool useColliderBounds = true;

    private Rigidbody2D rb2D;

    // --- Impulso con el mouse ---
    private float currentForce = 0f;                // Fuerza acumulada para el impulso al soltar el clic
    private Vector2 impulseVelocity = Vector2.zero; // Velocidad extra proveniente del clic
    private Coroutine slowDownCoroutine;            // Para frenar el impulso gradualmente

    // --- Movimiento con WASD (con inercia) ---
    private Vector2 wasdVelocity = Vector2.zero;    // Velocidad “suave” de WASD

    // --- Clamping en la cámara ---
    private float halfWidth = 0.5f;   // Mitad del ancho del objeto
    private float halfHeight = 0.5f;  // Mitad de la altura del objeto

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        // Si tenemos un Collider2D y queremos medir automáticamente su tamaño
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
        // ========== IMPULSO CON EL MOUSE ==========
        // 1. Acumular fuerza al mantener pulsado el clic izquierdo
        if (Input.GetMouseButtonDown(0))
        {
            currentForce = 0f; // Reinicia la fuerza
            // Si había una corrutina frenando un impulso anterior, la detenemos
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

        // 2. Al soltar, creamos la "impulseVelocity"
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mouseWorldPos - transform.position).normalized;
            impulseVelocity = direction * currentForce;

            // Iniciamos la corrutina que reduce el impulso a 0 gradualmente
            slowDownCoroutine = StartCoroutine(SlowDownImpulse());
        }
    }

    void FixedUpdate()
    {
        // ========== WASD CON INERCIA ==========

        // 1. Leemos la entrada del jugador (-1, 0 o 1). Normalizamos para diagonales.
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");
        Vector2 inputDir = new Vector2(xInput, yInput).normalized;

        // 2. Calculamos la velocidad deseada de WASD (sin inercia instantánea)
        Vector2 desiredWASDVelocity = inputDir * moveSpeed;

        // 3. Aceleramos o desaceleramos wasdVelocity hacia esa velocidad deseada
        float delta = Time.fixedDeltaTime; // Usamos fixedDeltaTime en FixedUpdate
        if (inputDir.magnitude > 0.01f)
        {
            // El jugador está pulsando alguna tecla. Aceleramos hacia desiredWASDVelocity
            wasdVelocity = Vector2.MoveTowards(
                wasdVelocity,
                desiredWASDVelocity,
                wasdAcceleration * delta
            );
        }
        else
        {
            // El jugador soltó WASD, desaceleramos hacia 0
            wasdVelocity = Vector2.MoveTowards(
                wasdVelocity,
                Vector2.zero,
                wasdDeceleration * delta
            );
        }

        // 4. Asignamos la velocidad total al Rigidbody2D:
        //    la suma de la parte "WASD" (con inercia) y la parte del "impulso" de clic.
        rb2D.linearVelocity = wasdVelocity + impulseVelocity;
    }

    void LateUpdate()
    {
        // ========== CLAMPING PARA NO SALIR DE LA CÁMARA ==========
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

            // Hacemos un Lerp desde la velocidad inicial hasta 0
            impulseVelocity = Vector2.Lerp(initialImpulse, Vector2.zero, t);

            yield return null;
        }

        // Aseguramos que la parte de impulso sea 0 al terminar
        impulseVelocity = Vector2.zero;
        slowDownCoroutine = null;
    }

    /// <summary>
    /// Limita la posición del objeto dentro de los límites de la cámara ortográfica.
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
            Debug.LogWarning("La cámara no es ortográfica. Se requiere otra lógica para clamping en perspectiva.");
        }
    }
}