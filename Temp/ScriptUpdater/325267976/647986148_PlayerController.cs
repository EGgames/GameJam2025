using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad base del jugador
    public float maxChargeForce = 15f; // Fuerza máxima de la super velocidad
    public float chargeRate = 10f; // Tasa de acumulación de potencia
    public float maxSpeed = 20f; // Velocidad máxima permitida durante la super velocidad

    private float currentCharge = 0f; // Potencia acumulada
    private bool isCharging = false; // Indicador de si se está cargando la potencia
    private Rigidbody2D rb; // Referencia al Rigidbody2D
    private Vector2 moveDirection; // Dirección de movimiento actual

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un Rigidbody2D en el objeto.");
        }

        rb.linearDamping = 1f; // Freno natural para desacelerar suavemente
    }

    void Update()
    {
        HandleMovementInput();
        HandleChargeAndRelease();
    }

    private void HandleMovementInput()
    {
        // Capturar la dirección del movimiento con las teclas
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        if (!isCharging)
        {
            // Movimiento base mientras no se está cargando
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

            // Reducir velocidad base para simular carga
            rb.linearVelocity = moveDirection * (moveSpeed * 0.5f); // La mitad de la velocidad base
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

                // Reiniciar la potencia
                currentCharge = 0f;
                isCharging = false;
            }
        }
    }
}
