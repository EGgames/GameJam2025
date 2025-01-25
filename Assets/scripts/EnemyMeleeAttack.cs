using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verificamos que el objeto que choca tenga la etiqueta "Player"
        if (!other.gameObject.CompareTag("Player")) return;
        
        // Accedemos al script de control del jugador
        PlayerController playerController = other.gameObject.GetOrAddComponent<PlayerController>();

        // Si el jugador está en modo dash, no toma daño
        if (playerController.isDashing) return;
        
        playerController.TakeDamage();
        Debug.Log("¡El enemigo tiene garras y daña al jugador!");
    }
}