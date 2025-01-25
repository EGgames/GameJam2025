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
            }
        }
    }
}
