using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChaseAttack : MonoBehaviour
{
    private Transform target;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            // Get the Player component from the player object
            player = playerObject.GetComponent<Player>();
            target = playerObject.transform;

            if (player == null)
            {
                Debug.LogError("Player component not found on the player object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    private void Update()
    {
        UpdateCooldowns();

        float distance = Vector2.Distance(transform.position, target.position);
        direction = (target.position - transform.position).normalized;
        FlipSprite();

        if (distance < attackRange)
        {
            Attack();

        }
        else if (distance < chaseRange)
        {
            Chase();
        }
        else
        {
            Idle();
        }

    }

    private void UpdateCooldowns()
    {
        // attack
        if (attackAnimation)
        {
            attackTimer += Time.deltaTime;
        }
        if (attackTimer > attackDelay)
        {
            attackAnimation = false;
            attackTimer = 0f;
        }
    }

    #region Movement

    /// <summary>
    /// move the character left and right
    /// chase the player 
    /// flip the sprite
    /// </summary>

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseRange = 10f;
    public bool isChasing = false;
    public bool isFacingRight;


    private void Move()
    {
        rb.velocity = direction * moveSpeed;
    }

    private void Chase()
    {
        isChasing = true;
        Move();

        if (IsInvoking("PerformAttack"))
        {
            CancelInvoke("PerformAttack");
        }
    }

    private void FlipSprite()
    {
        if (direction.x > 0f && isFacingRight || direction.x < 0f && !isFacingRight)
        {
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

    #endregion


    #region Attack

    /// <summary>
    /// attack the player when within range
    /// </summary>

    [Header("Attack")]
    public float attackRange = 5f;
    public float damage = 300f;

    public bool attackAnimation = false;

    public float attackTimer = 0.0f;
    public float attackDelay = 0.4f;
    public float timeSinceAttack = 0.0f;

    private Player player;

    private void Attack()
    {
        isChasing = false;
        direction = Vector2.zero;
        attackAnimation = true;

        if (!IsInvoking("PerformAttack"))
        {
            InvokeRepeating("PerformAttack", .5f, .5f);
        }
    }

    public void PerformAttack()
    {
        player.TakeDamage(damage);
        DamagePopup.Create(player.transform.position + Vector3.right + Vector3.up, (int)damage);
    }

    #endregion

    /// <summary>
    /// do nothing
    /// play idle animation
    /// </summary>

    private void Idle()
    {
        isChasing = false;
        direction = Vector2.zero;

        if (IsInvoking("PerformAttack"))
        {
            CancelInvoke("PerformAttack");
        }
    }
}
