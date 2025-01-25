using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]
    public float moveSpeed = 5f;       // Velocidad base del jugador
    public float maxChargeForce = 15f; // Fuerza máxima de la super velocidad
    public float chargeRate = 10f;     // Tasa de acumulación de potencia
    public float maxSpeed = 20f;       // Velocidad máxima permitida durante la super velocidad

    private float currentCharge = 0f;  // Potencia acumulada
    private bool isCharging = false;   // Indicador de si se está cargando la potencia
    private Rigidbody2D rb;            // Referencia al Rigidbody2D
    private Vector2 moveDirection;     // Dirección de movimiento actual

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un Rigidbody2D en el objeto.");
        }

        // Pequeño 'drag' para desacelerar suavemente cuando no hay entrada
        rb.linearDamping = 1f;
    }

    void Update()
    {
        HandleMovementInput();
        HandleChargeAndRelease();
    }

    private void HandleMovementInput()
    {
        // Aquí forzamos el uso de flechas en lugar de Input.GetAxis()
        float moveX = 0f;
        float moveY = 0f;

        // Flechas horizontales
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveX = 1f;
        }

        // Flechas verticales
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveY = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveY = -1f;
        }

        // Normalizamos para que las diagonales no sumen más velocidad
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Si no estamos cargando, aplicamos la velocidad base normal
        if (!isCharging)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private void HandleChargeAndRelease()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Empieza la carga de energía
            isCharging = true;
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxChargeForce);

            // Reducir velocidad base mientras se carga
            rb.linearVelocity = moveDirection * (moveSpeed * 0.5f);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Al soltar la barra espaciadora, se libera la "carga"
            if (isCharging && moveDirection != Vector2.zero)
            {
                // Calculamos la fuerza adicional y la aplicamos
                Vector2 boostedForce = moveDirection * (moveSpeed + currentCharge);
                rb.linearVelocity = boostedForce;

                // Limitar la velocidad máxima
                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }
            }

            // Reiniciamos valores
            currentCharge = 0f;
            isCharging = false;
        }
    }
}
