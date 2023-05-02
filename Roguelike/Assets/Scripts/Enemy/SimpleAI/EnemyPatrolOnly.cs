using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyPatrolJump;

public class EnemyPatrolOnly : MonoBehaviour
{
    public Player player;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool isFacingRight;
    public bool isPatrolling;

    [Header("Attack")]
    public float attackRange = 5f;
    public float damage = 300f;

    [Header("Knockback")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.5f;

    public bool isKnockback = false;
    public float knockbackTimer = 0f;

    [Header("Patrol")]
    public Transform[] patrolPoints;
    private int patrolIndex = 0;


    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        FlipSprite();
        Patrol();
    }


    private void FixedUpdate()
    {
        CheckTimers();
    }

    private void CheckTimers()
    {
        if (isKnockback)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockback = false;
            }
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return; // no patrol points defined

        direction = (patrolPoints[patrolIndex].position - transform.position).normalized;
        Move();

        if (Vector2.Distance(transform.position, patrolPoints[patrolIndex].position) < 0.1f)
        {
            // switch to the next patrol point
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        CancelInvoke("PerformAttack");
    }

    private void Move()
    {
        rb.velocity = direction * moveSpeed;
    }

    private void Attack()
    {
        if (!IsInvoking("PerformAttack"))
        {
            InvokeRepeating("PerformAttack", .5f, 1.5f);
        }
    }

    public void PerformAttack()
    {
        player.TakeDamage(damage);
        DamagePopup.Create(player.transform.position + Vector3.right + Vector3.up, (int)damage);
    }

    private void ApplyKnockback()
    {
        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
        Vector2 direction = player.gameObject.transform.position - transform.position;
        playerRigidbody.velocity = direction.normalized * knockbackForce;
        isKnockback = true;
        knockbackTimer = knockbackDuration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // rigid body velocity
        if (collision.gameObject.CompareTag("Player"))
        {
            PerformAttack();
            ApplyKnockback();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // rigid body impulse (based on mass)
        if (player != null)
        {
            player.TakeDamage(damage);
            DamagePopup.Create(player.transform.position + Vector3.right + Vector3.up, (int)damage);

            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            Vector3 direction = collision.gameObject.transform.position - transform.position;
            playerRb.AddForce(direction.normalized * knockbackForce, ForceMode2D.Impulse);
        }
    }

    private void FlipSprite()
    {
        if (isFacingRight && direction.x < 0f || !isFacingRight && direction.x > 0f)
        {
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = isFacingRight;
        }

    }
}