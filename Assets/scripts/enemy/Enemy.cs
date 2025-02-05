using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class Enemy : MonoBehaviour
{
    [Header("Atributos del Enemigo")]
    [Tooltip("Vida del enemigo.")]
    public int health = 1;
    [Tooltip("Velocidad base del enemigo")]
    public float moveSpeed = 5f;

    [Header("Configuraciones del objeto de enemigo")]
    [Tooltip("Efecto visual al morir (opcional).")]
    public GameObject deathEffect;
    [Tooltip("Referencia al SpriteRenderer del enemigo.")]
    public SpriteRenderer _spriteRenderer;
    [Tooltip("Referencia al Objeto de collider (body attack) del enemigo.")]
    public GameObject _bodyAttackColliderObj;
    
    [Header("Sonidos del enemigo")]
    [Tooltip("Sonidos de ataque del enemigo.")]
    public AudioClip[] attackSounds;
    [Tooltip("Sonidos de muerte del enemigo.")]
    public AudioClip[] deathSounds;

    protected Transform _player;
    protected Rigidbody2D _rb;
    protected Animator _animator;
    protected AudioSource _audioSource;
    protected Collider2D _collider;

    protected bool IsDead;

    protected virtual void Start()
    {
        // Buscar al jugador (por etiqueta "Player")
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
        
        // Guardar referencia al Animator
        _animator = GetComponent<Animator>();

        // Guardar referencia al Rigidbody2D
        _rb = GetComponent<Rigidbody2D>();
        
        // Guardar referencia al Collider
        _collider = GetComponent<Collider2D>();
        
        // Guardar referencia al AudioSource
        _audioSource = GetComponent<AudioSource>();
    }

    protected virtual void FixedUpdate()
    {
        if (IsDead) return;
        MoverEnemigo();
    }

    protected abstract void MoverEnemigo();

    public virtual void TakeDamage(int damage)
    {
        if (IsDead) return;
        health -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} daño. Salud restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        IsDead = true;
        
        // Desactivar el collider de ataque
        if (_bodyAttackColliderObj)
        {
            _bodyAttackColliderObj.SetActive(false);
        }
        
        // Desactivar el collider
        _collider.enabled = false;
        
        // Opcional: Instanciar un efecto visual al morir
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        
        // Reproducir sonido de muerte
        _audioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)];
        _audioSource.Play();
        
        // Cuando termina el sonido de muerte, destruir el objeto
        Destroy(gameObject, _audioSource.clip.length);
        
        GameManager.Instance.ScoreCount(); // Contar el score aquí
    }
}