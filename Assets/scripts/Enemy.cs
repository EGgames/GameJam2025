using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Atributos del enemigo")]
    [Tooltip("Si está en true, el enemigo mata al jugador en una colisión si esta no lo destruye antes.")]
    public bool garras = false;

    [Header("Parámetros de destrucción por choque")]
    [Tooltip("Fuerza mínima de colisión para destruir al enemigo.")]
    public float fuerzaDestruccion = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos que el objeto que choca tenga la etiqueta "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            //Accedo al PlayerController que tiene asignado el jugador de manera privada 
            PlayerController playerController = collision.gameObject.GetOrAddComponent<PlayerController>();
            // La magnitud de la velocidad relativa en el impacto
            float fuerzaChoque = collision.relativeVelocity.magnitude;

            // 1. Si el choque es lo bastante fuerte, destruimos al enemigo
            if (fuerzaChoque >= fuerzaDestruccion)
            {
                // Se destruye el enemigo
                Destroy(gameObject);
            }
            else
            {
                // 2. Si el choque NO es suficiente para destruirlo
                //    y este enemigo tiene garras, matamos al jugador
                if (garras)
                {
                    // Lógica de "muerte" del jugador.                    
                    playerController.KillPlayer();
                    Debug.Log("¡El enemigo tiene garras y mata al jugador!");
                }
            }
        }
    }
}
