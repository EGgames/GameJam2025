using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento básica
    public float maxChargeForce = 15f; // Fuerza máxima acumulada
    public float chargeRate = 10f; // Tasa de acumulación de fuerza

    private float currentCharge = 0f; // Fuerza acumulada actual
    private bool isCharging = false; // Indicador de si se está acumulando fuerza
    private Vector2 chargeDirection = Vector2.zero; // Dirección actual de la carga
    private Rigidbody2D rb; // Referencia al Rigidbody2D

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("No se encontró un Rigidbody2D en el objeto.");
        }
    }

    void Update()
    {
        HandleMovement();
        HandleChargeAndRelease();
    }

    private void HandleMovement()
    {
        // Movimiento con las flechas
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveX, moveY);

        // Actualizar dirección de carga solo si hay movimiento
        if (movement != Vector2.zero && !isCharging)
        {
            chargeDirection = movement.normalized; // Actualizar la dirección
        }

        // Aplicar movimiento básico si no se está cargando
        if (!isCharging)
        {
            rb.linearVelocity = movement * moveSpeed;
        }
    }

    private void HandleChargeAndRelease()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Acumular fuerza mientras se mantiene presionada la barra espaciadora
            isCharging = true;
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxChargeForce);

            // Durante la carga, el jugador queda inmóvil
            rb.linearVelocity = Vector2.zero;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Liberar la fuerza acumulada al soltar la barra espaciadora
            if (isCharging && chargeDirection != Vector2.zero)
            {
                Vector2 releaseForce = chargeDirection * currentCharge;
                rb.AddForce(releaseForce, ForceMode2D.Impulse);
            }

            // Reiniciar la carga
            currentCharge = 0f;
            isCharging = false;
        }
    }
}
