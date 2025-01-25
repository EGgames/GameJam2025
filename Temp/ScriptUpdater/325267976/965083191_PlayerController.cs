using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Parámetros de Movimiento")]
    public float moveSpeed = 5f;       // Velocidad base del jugador
    public float maxChargeForce = 15f; // Fuerza máxima de la "carga"
    public float chargeRate = 10f;     // Tasa de acumulación de la carga
    public float maxSpeed = 20f;       // Velocidad máxima

    private float currentCharge = 0f;  // Cantidad de carga acumulada
    private bool isCharging = false;   // Indicador de si se está cargando
    private Rigidbody2D rb;            // Referencia al Rigidbody2D
    private Vector2 moveDirection;     // Dirección de movimiento (flechas)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un Rigidbody2D en el objeto.");
        }

        // Ligero 'drag' para desacelerar suavemente al soltar las flechas
        rb.linearDamping = 1f;
    }

    void Update()
    {
        HandleMovementInput();
        HandleChargeAndRelease();
    }

    private void HandleMovementInput()
    {
        // Moverse con flechas en vez de GetAxis
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) moveX = 1f;
        if (Input.GetKey(KeyCode.UpArrow)) moveY = 1f;
        if (Input.GetKey(KeyCode.DownArrow)) moveY = -1f;

        // Normaliza para que las diagonales no aumenten la velocidad
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void FixedUpdate()
    {
        // Mientras NO estés cargando, tu velocidad base es según las flechas
        if (!isCharging)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
        // Si quisieras mantener la velocidad de las flechas AUN cargando, 
        // podrías quitar la condición y siempre asignar la velocidad base.
    }

    private void HandleChargeAndRelease()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Empieza o continúa la "carga" de energía mientras mantienes la barra
            isCharging = true;
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxChargeForce);

            // Importante: NO frenamos al cargar (se eliminó la línea que ponía la vel. a la mitad)
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Cuando sueltas la barra, se aplica el "impulso" en dirección al mouse
            if (isCharging)
            {
                // Calculamos la dirección hacia el mouse
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;

                // Sumamos la fuerza de la carga a la velocidad actual
                Vector2 boostedVelocity = rb.linearVelocity + directionToMouse * (moveSpeed + currentCharge);

                // Asignamos la velocidad resultante
                rb.linearVelocity = boostedVelocity;

                // Limitar la velocidad a maxSpeed
                if (rb.linearVelocity.magnitude > maxSpeed)
                {
                    rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                }

                // Reseteamos la carga
                currentCharge = 0f;
                isCharging = false;
            }
        }
    }
}
