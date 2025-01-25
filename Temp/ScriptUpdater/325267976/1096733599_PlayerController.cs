using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool life;
    public int lifeInteger;

    public float moveSpeed = 5f; // Velocidad de movimiento con las flechas
    public float acceleration = 20f; // Aceleración al mantener presionada la barra espaciadora
    public float maxSpeed = 10f; // Velocidad máxima al acelerar

    private bool isAccelerating = false; // Indicador de si se está acelerando
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
        HandleAcceleration();
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

    private void HandleAcceleration()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            // Acelerar en la dirección actual
            isAccelerating = true;
            Vector2 accelerationForce = currentDirection * acceleration * Time.deltaTime;

            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(accelerationForce, ForceMode2D.Force);
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            // Detener la aceleración al soltar la barra espaciadora
            isAccelerating = false;
        }
    }
}