﻿using UnityEngine;
using UnityEngine.Serialization;

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

    protected Transform _player;
    protected Rigidbody2D _rb;
    protected Animator _animator;

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
    }

    protected virtual void FixedUpdate()
    {
        MoverEnemigo();
    }

    protected abstract void MoverEnemigo();

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} daño. Salud restante: {health}");

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        // Opcional: Instanciar un efecto visual al morir
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Destruir el enemigo
        Destroy(gameObject);
        GameManager.Instance.ScoreCount(); // Contar el score aquí
    }
}