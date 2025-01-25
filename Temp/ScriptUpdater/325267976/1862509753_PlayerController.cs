using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Parámetros de fuerza")]
    [Tooltip("Fuerza máxima que puede alcanzar el impulso.")]
    public float maxForce = 10f;

    [Tooltip("Velocidad a la que se acumula la fuerza mientras se mantiene el mouse presionado.")]
    public float chargeRate = 5f;

    [Header("Parámetros de frenado")]
    [Tooltip("Tiempo en segundos que tarda en frenar completamente.")]
    public float slowDownTime = 1f;

    private Rigidbody2D rb2D;        // Referencia al Rigidbody2D
    private float currentForce = 0f; // Fuerza acumulada
    private Coroutine slowDownCoroutine; // Corrutina para frenar

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (rb2D == null)
        {
            Debug.LogError("Este script requiere un componente Rigidbody2D en el mismo GameObject.");
        }
    }

    void Update()
    {
        // Cuando se presiona el botón izquierdo por primera vez
        if (Input.GetMouseButtonDown(0))
        {
            currentForce = 0f;
            // Detenemos la corrutina de frenado si todavía estaba en proceso,
            // para que no interfiera con la próxima "disparada".
            if (slowDownCoroutine != null)
            {
                StopCoroutine(slowDownCoroutine);
                slowDownCoroutine = null;
            }
        }

        // Mientras se mantenga presionado, acumulamos fuerza
        if (Input.GetMouseButton(0))
        {
            currentForce += chargeRate * Time.deltaTime;
            currentForce = Mathf.Clamp(currentForce, 0f, maxForce);
        }

        // Al soltar el botón
        if (Input.GetMouseButtonUp(0))
        {
            // Obtenemos la posición del mouse en coordenadas de mundo
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Calculamos la dirección 2D normalizada
            Vector2 direction = (mouseWorldPosition - transform.position).normalized;

            // (Opcional) Reiniciamos la velocidad existente antes de lanzar
            rb2D.linearVelocity = Vector2.zero;

            // Aplicamos el impulso
            rb2D.AddForce(direction * currentForce, ForceMode2D.Impulse);

            // Iniciamos la corrutina de frenado
            slowDownCoroutine = StartCoroutine(SlowDown());
        }
    }

    /// <summary>
    /// Corrutina que reduce la velocidad de rb2D a 0 en 'slowDownTime' segundos.
    /// </summary>
    private IEnumerator SlowDown()
    {
        Vector2 initialVelocity = rb2D.linearVelocity;  // Velocidad justo después de impulsar
        float elapsedTime = 0f;

        while (elapsedTime < slowDownTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / slowDownTime);
            // Lerp de la velocidad inicial hasta 0
            rb2D.linearVelocity = Vector2.Lerp(initialVelocity, Vector2.zero, t);
            yield return null;
        }

        // Aseguramos que quede totalmente parado
        rb2D.linearVelocity = Vector2.zero;
    }
}