using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool life;
    public int lifeInteger;

    public float moveSpeed = 5f; // Velocidad de movimiento con las flechas
    public float maxChargeForce = 15f; // Fuerza máxima acumulada
    public float chargeRate = 10f; // Tasa de acumulación de fuerza

    private float currentCharge = 0f; // Fuerza acumulada actual
    private bool isCharging = false; // Indicador de si se está acumulando fuerza
    private Vector2 currentDirection = Vector2.zero; // Dirección actual del movimiento
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

        if (movement != Vector2.zero)
        {
            currentDirection = movement.normalized; // Actualizar la dirección actual
        }

        // Movimiento base
        rb.linearVelocity = movement * moveSpeed;
    }

    private void HandleChargeAndRelease()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Acumular fuerza mientras se mantiene presionada la barra espaciadora
            isCharging = true;
            currentCharge += chargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxChargeForce);
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Liberar la fuerza acumulada al soltar la barra espaciadora
            if (isCharging)
            {
                Vector2 releaseForce = currentDirection * currentCharge;
                rb.AddForce(releaseForce, ForceMode2D.Impulse);
                currentCharge = 0f; // Reiniciar la carga
                isCharging = false;
            }
        }
    }
}
