using UnityEngine;
using UnityEngine.InputSystem;

public class HandlePlayerInput : MonoBehaviour
{
    private PlayerControls _playerControls;
    [SerializeField] private InputActionReference _playerInputAction;

    private PlayerMovement _playerMovement;


    [SerializeField] private Vector2 _moveH;
    public Vector2 MoveH { get { return _moveH; } set { _moveH = value; } }

    private void Awake()
    {
        _playerControls = new PlayerControls();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        _playerControls.Enable();
        _playerInputAction.action.Enable();
    }

    private void OnDisable()
    {
        _playerControls.Disable();
        _playerInputAction.action.Disable();
    }

    private void Update()
    {
        //ProcessInput();
        _moveH = _playerControls.Player.Move.ReadValue<Vector2>();

    }

    private void FixedUpdate()
    {
        _playerMovement.MoveNew(_moveH);
    }

    #region Input

    //public void ProcessInput()
    //{
    //    // if press button > 


    //    // reset jump counter
    //    if (IsGrounded() && !_playerControls.Player.Jump.triggered)
    //    {
    //        jumpCounter = 0f;
    //        IsJumpingMidAir = false;
    //        maxJump = defaultMaxJump;
    //    }

    //    // grounded 
    //    if (IsGrounded())
    //    {
    //        hangTimeCounter = hangTime;
    //    }
    //    else
    //    {
    //        hangTimeCounter -= Time.deltaTime;
    //    }

    //    // jump
    //    if (_playerControls.Player.Jump.triggered)
    //    {
    //        jumpBufferCounter = jumpBufferLength;

    //        if (jumpBufferCounter > 0f && (hangTimeCounter > 0f || jumpCounter < maxJump))
    //        {
    //            jumpRequest = true;
    //            jumpCounter++;
    //        }
    //    }
    //    else
    //    {
    //        if (jumpBufferCounter > -2f)
    //        {
    //            jumpBufferCounter -= Time.deltaTime;

    //        }
    //    }

    //    // attack
    //    if (_playerControls.Player.Attack.triggered)
    //    {
    //        attackRequest = true;
    //    }

    //    // skill
    //    if (_playerControls.Player.Skill.triggered)
    //    {
    //        _playerSkill.skillAttackRequest = true;
    //    }

    //    // ult
    //    if (_playerControls.Player.Ult.triggered)
    //    {
    //        _playerUlt.ultAttackRequest = true;
    //    }

    //    // dash
    //    if (_playerControls.Player.Dash.triggered && canDash)
    //    {
    //        dashRequest = true;
    //    }

    //    // damage test DELETE
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        TakeDamage(damageAmount);
    //    }

    //    // heal test DELETE
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        Heal(healAmount);
    //    }
    //}

    #endregion
}
