using UnityEngine;

public class MeleeEnemy : Enemy
{
    [Tooltip("Radio de distancia a mantener con otros enemigos.")]
    public float avoidanceRadius = 2f;
    
    [Tooltip("Si está en true, el enemigo mata al jugador en una colisión si este no lo destruye antes.")]
    public bool garras = false;

    [Tooltip("Referencia al objeto con el collider de ataque")]
    public GameObject _attackColliderObj;
    

    [Header("Alternancia de color y garras")]
    [Tooltip("Cada cuántos segundos alterna entre garras activas/rojas y garras inactivas/color original.")]
    public float intervaloCambiarGarras = 2f;
    
    private Color _colorOriginal;

    protected override void Start()
    {
        base.Start();
        if (_spriteRenderer != null)
            _colorOriginal = _spriteRenderer.color;

        // Iniciar la rutina que alterna color y garras cada X segundos
        StartCoroutine(ToggleGarrasColorRoutine());
    }

    protected override void MoverEnemigo()
    {
        if (!_player) return;

        // Calculamos la dirección hacia el jugador
        var attractionDirection = (_player.position - transform.position).normalized;

        var repulsionDirection = Vector3.zero;

        // Detectamos si hay otros enemigos cerca
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, avoidanceRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy") && collider.gameObject != gameObject)
            {
                // Si hay un enemigo cerca, alejarse de él
                repulsionDirection += (transform.position - collider.transform.position).normalized /
                                      Vector3.Distance(collider.transform.position, transform.position);
            }
        }
        
        // Sumamos la dirección de repulsión a la dirección de atracción
        Vector3 moveDirection = (attractionDirection + repulsionDirection).normalized;

        // Calculamos la nueva posición
        Vector2 newPosition = _rb.position + (Vector2)moveDirection * (moveSpeed * Time.deltaTime);

        // Movemos al enemigo usando Rigidbody2D.MovePosition
        _rb.MovePosition(newPosition);
    }

    private System.Collections.IEnumerator ToggleGarrasColorRoutine()
    {
        while (true)
        {
            // Esperamos X segundos antes de cambiar de estado
            yield return new WaitForSeconds(intervaloCambiarGarras);

            // Alternamos el estado de garras
            garras = !garras;
            _attackColliderObj.SetActive(garras);
            // if (_spriteRenderer)
            // {
            //     _spriteRenderer.color = garras ? Color.red : _colorOriginal;
            // }
            _animator.SetBool("IsAttacking", garras);

            // Reproducir sonido de ataque
            if (garras)
            {
                _audioSource.clip = attackSounds[Random.Range(0, attackSounds.Length)];
                _audioSource.Play();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificamos si el objeto colisionado es el jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            // Obtener el script del jugador
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                if (!playerController.isDashing)
                {
                    // Aplicar daño al jugador
                    playerController.TakeDamage(1); // Ajusta la cantidad de daño según sea necesario
                }

                if (playerController.isPoweredUp)
                {
                    // Si el jugador está en modo powered up, matar al enemigo
                    Die();
                }
            }
        }
    }
}