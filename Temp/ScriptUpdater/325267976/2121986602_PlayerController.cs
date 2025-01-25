using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Velocidad de movimiento con las flechas
    public float maxChargeForce = 15f; // Fuerza máxima acumulada
    public float chargeRate = 10f; // Tasa de acumulación de fuerza

    private float currentCharge = 0f; // Fuerza acumulada actual
    private bool isCharging = false; // Indicador de si se está acumulando fuerza
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
        // Movimiento con las flechas (opcional, puedes eliminarlo si no necesitas movimiento base)
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveX, moveY);

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
                // Dirección hacia el mouse
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = (mousePosition - rb.position).normalized;

                // Aplicar la fuerza acumulada en la dirección del mouse
                Vector2 releaseForce = direction * currentCharge;
                rb.AddForce(releaseForce, ForceMode2D.Impulse);

                // Reiniciar la carga
                currentCharge = 0f;
                isCharging = false;
            }
        }
    }
}
