using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]
    public float moveSpeed = 5f;       // Velocidad base del jugador
    public float maxChargeForce = 15f; // Fuerza máxima de la super velocidad
    public float chargeRate = 10f;     // Tasa de acumulación de potencia
    public float maxSpeed = 20f;       // Velocidad máxima permitida durante la super velocidad

    public float currentCharge = 0f;  // Potencia acumulada
    public bool isCharging = false;   // Indicador de si se está cargando la potencia
    private Rigidbody2D rb;            // Referencia al Rigidbody2D
    private Vector2 moveDirection;     // Dirección de movimiento actual

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un Rigidbody2D en el objeto.");
        }

        // Freno natural para desacelerar suavemente
        rb.linearDamping = 1f;
    }

    void Update()
    {
        HandleMovementInput();
        HandleChargeAndRelease();
    }

    private void HandleMovementInput()
    {
        // Teclas de flecha para movimiento
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) moveX = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) moveY = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) moveY = -1f;

        // Se normaliza para que la velocidad diagonal no sea mayor
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Movimiento base mientras no se está cargando
        if (!isCharging)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    private void HandleChargeAndRelease()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Acumular potencia mientras se mantiene presionada la barra espaciadora
            isCharging = true;
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxChargeForce);

            // -----------------------------
            // ESTA LÍNEA ESTABA FRENANDO:
            // rb.velocity = moveDirection * (moveSpeed * 0.5f);
            // -----------------------------
            // Si no quieres reducir velocidad, simplemente la comentas o la eliminas.
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Liberar la super velocidad al soltar la barra espaciadora
            if (isCharging && moveDirection != Vector2.zero)
            {
                Vector2 boostedForce = moveDirection * (moveSpeed + currentCharge);
                rb.linearVelocity = boostedForce;

                // Limitar la velocidad máxima
                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }
            }

            // Reiniciar la potencia
            currentCharge = 0f;
            isCharging = false;
        }
    }
}
