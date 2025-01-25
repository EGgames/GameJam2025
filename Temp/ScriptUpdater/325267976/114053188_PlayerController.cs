using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool life;
    public int lifeInteger;

    public float moveSpeed = 5f; // Velocidad de movimiento con las flechas
    public float maxChargeTime = 2f; // Tiempo máximo para cargar fuerza
    public float maxForce = 20f; // Fuerza máxima al soltar la barra espaciadora

    private float chargeTime = 0f; // Tiempo de carga actual
    private bool isCharging = false; // Indicador de si se está cargando la fuerza
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
        HandleChargeAndLaunch();
    }

    private void HandleMovement()
    {
        // Movimiento con las flechas
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveX, moveY);
        rb.linearVelocity = movement * moveSpeed;
    }

    private void HandleChargeAndLaunch()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Cargar fuerza
            isCharging = true;
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);
        }

        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            // Lanzar al soltar la barra espaciadora
            isCharging = false;
            float chargePercentage = chargeTime / maxChargeTime;
            float force = chargePercentage * maxForce;

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 direction = (mousePosition - rb.position).normalized;

            rb.AddForce(direction * force, ForceMode2D.Impulse);

            chargeTime = 0f; // Reiniciar el tiempo de carga
        }
    }
}

