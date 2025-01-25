using System;
using UnityEngine;

public class GellyFishBounce : MonoBehaviour
{
    [Header("Parámetros de Rebote")]
    [SerializeField] private float bounceForce = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Calcula la normal y la velocidad reflejada
                Vector2 normal = collision.contacts[0].normal;
                Vector2 reflectedVelocity = Vector2.Reflect(playerController.GetCurrentVelocity(), normal);

                // Ajusta la magnitud al gusto (bounceForce)
                reflectedVelocity = reflectedVelocity.normalized * bounceForce;

                // Notificas al PlayerController para que modifique su 'impulseVelocity'
                playerController.ApplyExternalVelocity(reflectedVelocity);
                
                // Le decimos al PlayerController que active el power-up de rebote
                playerController.BouncePowerUp();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            // Hacer que el proyectil rebote
            var projectile = other.gameObject.GetComponent<ProjectilePlayer>();
            if (projectile != null)
            {
                // Calcula la normal y la velocidad reflejada
                Vector2 normal = transform.position - projectile.transform.position;
                Vector2 reflectedVelocity = Vector2.Reflect(projectile.GetDirection(), normal);

                // Ajusta la magnitud al gusto (bounceForce)
                reflectedVelocity = reflectedVelocity.normalized * bounceForce;

                // Notificas al proyectil para que modifique su dirección
                projectile.SetDirection(reflectedVelocity, projectile.GetSpeed());
                
                projectile.BouncePowerUp();
            }
        }
    }
}
