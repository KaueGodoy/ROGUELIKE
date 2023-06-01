using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public CharacterStats characterStats;

    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private PlayerControls playerControls;
    [SerializeField] private InputActionReference playerInputAction;

    private PlayerDamage playerDamage;

    [Header("Ground")]
    [SerializeField] private LayerMask jumpableGround;

    #region BaseStats

    [Header("Base Stats")]
    public float baseHealth = 22000f;
    public float baseAttack = 10;
    public float baseAttackPercent = 0;
    public float baseAttackFlat = 0;
    public float baseDamageBonus = 0;
    public float baseCritRate = 5;
    public float baseCritDamage = 50;
    public float baseDefense = 15;
    public float baseAttackSpeed = 5;

    #endregion

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        playerDamage = GetComponent<PlayerDamage>();

        //audioManager = GetComponent<AudioManager>(); // doesnt work because component is not applied to this game object
        characterStats = new CharacterStats(baseHealth, baseAttack, baseAttackPercent, baseAttackFlat, baseDamageBonus, baseCritRate, baseCritDamage, baseDefense, baseAttackSpeed);
        Debug.Log("Player init");

        playerControls = new PlayerControls();

    }
    void Start()
    {
        currentHealth = maxHealth;
        isAlive = true;

        UIEventHandler.HealthChanged(this.currentHealth, this.maxHealth);
        UpdateUI();

        // handling input through events
        //playerControls.Player.Jump.performed += _ => Jump2();
        //ReadInput();
    }

    private void ReadInput()
    {
        // list of player inputs with events being subscribed and calling the functions to perform the action using the start method
        playerControls.Player.Jump.performed += context => Jump3(context.ReadValue<float>());
    }

    private void Jump2()
    {
        Debug.Log("Handling input with events calling the methods instead of having the methods in the update function");
    }

    private void Jump3(float jumpForce)
    {
        Debug.Log("Using the context to pass a value ");
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerInputAction.action.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerInputAction.action.Disable();
    }

    void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            if (isDashing) return;

            if (isAlive)
            {
                ProcessInput();
                Attack();
                Ult();
                UpdateUI();
            }
        }
    }

    private void FixedUpdate()
    {

        if (!PauseMenu.GameIsPaused)
        {
            if (isDashing) return;

            if (isAlive)
            {
                Move();
                Jump();
                BetterJump();
                DashTrigger();
                Flip();
            }

            UpdateTimers();
            UpdateAnimationState();
        }

    }

    #region Input

    private void ProcessInput()
    {
        // horizontal movement
        //moveX = Input.GetAxisRaw("Horizontal");

        // reset jump counter
        if (IsGrounded() && !playerControls.Player.Jump.triggered)
        {
            jumpCounter = 0f;
            IsJumpingMidAir = false;
            maxJump = defaultMaxJump;
        }

        // grounded 
        if (IsGrounded())
        {
            hangTimeCounter = hangTime;
        }
        else
        {
            hangTimeCounter -= Time.deltaTime;
        }

        // jump
        if (playerControls.Player.Jump.triggered)
        {
            jumpBufferCounter = jumpBufferLength;

            if (jumpBufferCounter > 0f && (hangTimeCounter > 0f || jumpCounter < maxJump))
            {
                jumpRequest = true;
                jumpCounter++;

            }
        }
        else
        {
            if (jumpBufferCounter > -2f)
            {
                jumpBufferCounter -= Time.deltaTime;

            }
        }

        // attack
        //if (playerControls.Player.Attack.triggered)
        //{
        //    basicAttackRequest = true;
        //}

        // skill
        if (playerControls.Player.Skill.triggered)
        {
            attackRequest = true;
        }

        // ult
        if (playerControls.Player.Ult.triggered)
        {
            ultAttackRequest = true;
        }

        // dash
        if (playerControls.Player.Dash.triggered && canDash)
        {
            dashRequest = true;
        }

        // damage test DELETE
        if (Input.GetKeyDown(KeyCode.U))
        {
            TakeDamage(damageAmount);
        }

        // heal test DELETE
        if (Input.GetKeyDown(KeyCode.I))
        {
            Heal(healAmount);
        }
    }

    #endregion

    #region Timers
    private void UpdateTimers()
    {
        // hit
        if (isHit)
        {
            hitTimer += Time.deltaTime;
        }

        if (hitTimer > hitCooldown)
        {
            isHit = false;
            hitTimer = 0f;
        }

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
        if (attackString)
        {
            timeSinceAttack += Time.deltaTime;
        }
        if (timeSinceAttack > attackStringReset)
        {
            attackString = false;
            currentAttack = 0;
        }

        // ult
        if (ultAttackAnimation)
        {
            ultAttackTimer += Time.deltaTime;
        }
        if (ultAttackTimer > ultAttackDelay)
        {
            ultAttackAnimation = false;
            ultAttackTimer = 0f;
        }

    }
    #endregion

    #region Health

    [Header("Health")]
    public float currentHealth = 0;
    public float maxHealth = 3;

    [Header("Damage and Heal")]
    public float damageAmount = 1f;
    public float healAmount = 1f;

    private bool isAlive;
    private readonly float deathAnimationTime = 0.8f;

    [Space]
    public float hitCooldown = 0.3f;
    private float hitTimer = 0.0f;
    private bool isHit = false;

    public void TakeDamage(float damageAmount)
    {
        Debug.Log("Player takes: " + damageAmount + " damage");

        FindObjectOfType<AudioManager>().PlaySound("Hit");
        currentHealth -= Mathf.FloorToInt(damageAmount);
        isHit = true;

        UIEventHandler.HealthChanged(this.currentHealth, this.maxHealth);

        if (currentHealth <= 0)
        {
            UpdateUI();
            Die();
            UIEventHandler.HealthChanged(0, this.maxHealth);
            Invoke("RestartLevel", deathAnimationTime);
        }
    }

    private void Heal(float healAmount)
    {
        FindObjectOfType<AudioManager>().PlaySound("Hit");
        currentHealth += Mathf.FloorToInt(healAmount);

        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    #endregion


    #region Movement

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float moveX;

    private Vector2 moveH;
    private Vector2 direction;

    public bool isFacingRight = true;

    private void MoveOld()
    {
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);
    }

    private void Move()
    {
        moveH = playerControls.Player.Move.ReadValue<Vector2>();

        direction = new Vector2(moveH.x * moveSpeed, rb.velocity.y);

        if (direction != Vector2.zero)
        {
            rb.velocity = direction;
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    #endregion

    #region Jump

    [Header("Jump")]
    public float jumpForce = 5f;
    public float tapJumpMultiplier = 1f;
    public float holdJumpMultiplier = 1f;
    public float fallMultiplier = 2.5f;
    [Space]
    public float jumpCounter = 0f;
    public float maxJump = 2f;
    public float defaultMaxJump = 2f;
    [Space]
    // coyote jump
    public float hangTime = 0.2f;
    public float hangTimeCounter;
    [Space]
    // jump buffer
    public float jumpBufferLength = 0.2f;
    public float jumpBufferCounter;
    [Space]
    public bool IsJumpingMidAir = false;

    private bool jumpRequest = false;

    private void Jump()
    {
        if (jumpRequest)
        {
            FindObjectOfType<AudioManager>().PlaySound("Jump");

            if (IsJumpingMidAir)
            {
                // changing velocity to jump (could also do += Vector2.up * jumpForce;)
                rb.velocity = Vector2.up * jumpForce;
                IsJumpingMidAir = false;
            }
            else
            {
                // adding force to jump (less prone to bug)
                //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                rb.velocity = Vector2.up * jumpForce;

            }

            hangTimeCounter = 0f;
            jumpBufferCounter = 0f;
            jumpRequest = false;
        }
    }

    private void BetterJump()
    {
        // changing gravity directly
        if (rb.velocity.y < 0f)
        {
            rb.gravityScale = fallMultiplier;
        }

        playerInputAction.action.canceled += context =>
        {
            if (rb.velocity.y > 0)
            {
                rb.gravityScale = tapJumpMultiplier;
            }

        };
        playerInputAction.action.performed += context =>
        {
            rb.gravityScale = holdJumpMultiplier;
        };

        /* OLD
          
         
        // changing gravity directly
        if (rb.velocity.y < 0f)
        {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        --- velocity instead of gravity

        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        */
    }
    public void JumpPlayerInputComponent(InputAction.CallbackContext context)
    {
        /*
        if (rb.velocity.y < 0f)
        {
            rb.gravityScale = fallMultiplier;
        }
        if (context.performed && IsGrounded())
        {
            rb.gravityScale = 1f;
            rb.velocity = Vector2.up * jumpForce;
        }
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            rb.gravityScale = tapJumpMultiplier;
        }*/
    }

    #endregion Jump

    #region Attack

    [Header("Attack/Skill")]
    public Transform firePoint;
    public Transform spawnPoint;
    public GameObject pfProjectile;

    private float attackTimer = 0.0f;

    public float attackDelay = 0.4f;
    public float timeSinceAttack = 0.0f;
    public float attackStringReset = 0.8f;

    public int currentAttack = 0;

    public bool attackRequest = false;
    public bool attackString = false;

    private bool attackAnimation = false;
    private bool isAttacking = false;

    public PlayerCooldowns skillCooldowns;

    private void Attack()
    {
        if (attackRequest && skillCooldowns.offCooldown)
        {
            attackRequest = false;
            attackAnimation = true;
            attackString = true;

            if (!isAttacking)
            {
                isAttacking = true;

                UpdateAttackString();

                Invoke("InstantiateAttack", attackDelay - 0.1f);

                Invoke("AttackComplete", attackDelay);

                ResetAttackString();
            }
        }
    }

    public void UpdateAttackString()
    {
        currentAttack++;
        //Debug.Log("Attacking");

        // loops attack string
        if (currentAttack > 3)
        {
            currentAttack = 1;
        }
    }

    private void InstantiateAttack()
    {
        Instantiate(pfProjectile, firePoint.position, firePoint.rotation);
        FindObjectOfType<AudioManager>().PlaySound("Attack");
    }
    public void ResetAttackString()
    {
        timeSinceAttack = 0.0f;
    }

    private void AttackComplete()
    {
        isAttacking = false;
    }

    #endregion

    #region Ult

    [Header("Ult")]
    public Transform UltSpawnPoint;
    public GameObject pfUltProjectile;

    public float ultAttackDelay = 0.4f;
    private float ultAttackTimer = 0.0f;
    public float ultTimeSinceAttack = 0.0f;

    public bool ultAttackRequest = false;
    private bool ultAttackAnimation = false;
    private bool isUlting = false;

    private void Ult()
    {
        if (skillCooldowns.ultOffCooldown)
        {
            if (ultAttackRequest)
            {
                ultAttackRequest = false;
                ultAttackAnimation = true;

                if (!isUlting)
                {
                    isUlting = true;

                    Invoke("InstantiateUlt", ultAttackDelay - 0.1f);
                    Invoke("UltComplete", ultAttackDelay);
                }
            }
        }
        else
        {
            ultAttackRequest = false; // Reset the ult attack request if the ult is on cooldown
        }
    }

    private void InstantiateUlt()
    {
        Instantiate(pfUltProjectile, UltSpawnPoint.position, UltSpawnPoint.rotation);
        FindObjectOfType<AudioManager>().PlaySound("Attack");
    }

    private void UltComplete()
    {
        isUlting = false;
    }


    #endregion

    #region Dash

    [Header("Dash")]
    public float dashSpeed = 5f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    private bool canDash = true;
    private bool dashRequest = false;
    private bool isDashing;

    [SerializeField] private TrailRenderer tr;

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        IsJumpingMidAir = false;

        // NEEDS IMPROVEMENT
        if (!IsGrounded() && jumpCounter != 0)
        {
            IsJumpingMidAir = true;
            jumpCounter--;
        }

        FindObjectOfType<AudioManager>().PlaySound("PlayerDash");

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0f);
        tr.emitting = true;


        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void DashTrigger()
    {
        if (dashRequest)
        {
            StartCoroutine(Dash());
            dashRequest = false;
        }
    }
    #endregion

    private void Flip()
    {
        if (isFacingRight && moveH.x < 0f || !isFacingRight && moveH.x > 0f)
        {
            // flipping the player using scale

            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;

            firePoint.Rotate(firePoint.rotation.x, 180f, firePoint.rotation.z);
            spawnPoint.Rotate(firePoint.rotation.x, 180f, firePoint.rotation.z);
            UltSpawnPoint.Rotate(firePoint.rotation.x, 180f, firePoint.rotation.z);
        }

    }

    #region Level

    private void RestartLevel()
    {
        isAlive = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Die()
    {
        FindObjectOfType<AudioManager>().PlaySound("GameOver");
        isAlive = false;
        rb.bodyType = RigidbodyType2D.Static;
    }
    #endregion 


    #region Animation

    private Animator animator;
    private string currentAnimation;

    private const string PLAYER_DEATH = "derildo_death";
    private const string PLAYER_HIT = "derildo_hit";
    private const string PLAYER_ATTACK_STRING_01 = "derildo_attack1";
    private const string PLAYER_ATTACK_STRING_02 = "derildo_attack2";
    private const string PLAYER_ATTACK_STRING_03 = "derildo_attack3";
    private const string PLAYER_JUMP = "derildo_jump";
    private const string PLAYER_FALL = "derildo_fall";
    private const string PLAYER_WALK = "derildo_walk";
    private const string PLAYER_IDLE = "derildo_idle";

    public void ChangeAnimationState(string newAnimation)
    {
        if (currentAnimation == newAnimation) return;

        animator.Play(newAnimation);
        currentAnimation = newAnimation;
    }

    private void UpdateAnimationState()
    {
        // death
        if (!isAlive)
        {
            ChangeAnimationState(PLAYER_DEATH);
        }
        // hit
        else if (isHit)
        {
            ChangeAnimationState(PLAYER_HIT);
        }
        // attack
        else if (attackAnimation)
        {
            if (currentAttack == 1)
            {
                ChangeAnimationState(PLAYER_ATTACK_STRING_01);
                Debug.Log("Attack string number: " + currentAttack);

            }
            else if (currentAttack == 2)
            {
                ChangeAnimationState(PLAYER_ATTACK_STRING_02);
                Debug.Log("Attack string number: " + currentAttack);

            }
            else if (currentAttack == 3)
            {
                ChangeAnimationState(PLAYER_ATTACK_STRING_03);
                Debug.Log("Attack string number: " + currentAttack);

            }
        }
        else if (ultAttackAnimation)
        {
            ChangeAnimationState(PLAYER_ATTACK_STRING_02);
        }
        // jump
        else if (rb.velocity.y > .1f && !IsGrounded())
        {
            ChangeAnimationState(PLAYER_JUMP);
        }
        // fall
        else if (rb.velocity.y < .1f && !IsGrounded())
        {
            ChangeAnimationState(PLAYER_FALL);
        }
        // move
        else if (moveH.x > 0 || moveH.x < 0)
        {
            ChangeAnimationState(PLAYER_WALK);
        }
        // idle
        else
        {
            ChangeAnimationState(PLAYER_IDLE);
        }

    }
    #endregion

    #region UI
    [Header("UI Elements")]

    [SerializeField] private PlayerHealthBar healthBar;
    [SerializeField] private PlayerCooldowns cooldowns;

    public void UpdateUI()
    {
        healthBar.UpdateHealthBar(maxHealth, currentHealth);
        //cooldowns.UpdateCooldowns(attackTimer);
    }

    #endregion

    private bool IsGrounded()
    {
        float extraHeightText = .1f;

        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size - new Vector3(0.1f, 0f, 0f), 0f, Vector2.down, extraHeightText, jumpableGround);

        // draw gizmos
        /*

        Color rayColor;

        if(raycastHit.collider != null)
        {
            rayColor = Color.green;
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider.bounds.center + new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, 0), Vector2.down * (boxCollider.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(boxCollider.bounds.center - new Vector3(boxCollider.bounds.extents.x, boxCollider.bounds.extents.y + extraHeightText), Vector2.right * (boxCollider.bounds.extents.x * 2f), rayColor);
        Debug.Log(raycastHit.collider);

         */

        return raycastHit.collider != null;

    }

}
