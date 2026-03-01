using System.Collections;
using UnityEngine;
using StateMachine;

public partial class Knight : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;

    [Header("References")]
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private Transform leftWallRaycastPoint;
    [SerializeField] private Transform rightWallRaycastPoint;

    [Header("Run")]
    [SerializeField] private float runSpeed;
    [SerializeField] private float runAccel;

    [Header("Jump")]
    [SerializeField] private float baseGravityScale;
    [SerializeField] private float jumpSpeed;

    [Header("Crouch")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchAccel;

    [Header("Slide")]
    [SerializeField] private float slideSpeed;

    [Header("Dodge")]
    [SerializeField] private AnimationCurve dodgeSpeedCurve;
    [SerializeField] private float dodgeStartSpeed;
    [SerializeField] private float dodgeEndSpeed;
    [SerializeField] private float dodgeDuration;


    [Header("Dive")]
    [SerializeField] private float diveSpeed;
    [SerializeField] private float diveUpVelocity;

    [Header("Roll")]
    [SerializeField] private float rollSpeed;
    [SerializeField] private float rollDuration;

    [Header("Wall")]
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallSlideAccel;
    [SerializeField] private float wallJumpSpeed;
    [SerializeField] private float wallJumpAngle;

    [Header("Attacks")]
    [SerializeField] private float attackCombo1Duration;
    [SerializeField] private float attackCombo1EndLag;
    [SerializeField] private float attackCombo2Duration;
    [SerializeField] private float attackCombo2EndLag;
    [SerializeField] private float attackCrouchDuration;

    private Rigidbody2D rbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private StateMachine<StateKey, Knight> stateMachine;
    public enum StateKey
    {
        Idle, Run, Crouch, Slide, Dodge, Roll,

        Aerial, AirDive,

        WallSlide, LedgeHang, LedgeVault,

        AttackCombo, AttackCrouch,

        Hurt, Die
    }

    private float horizontalInput;
    private float verticalInput;
    private readonly InputButton jumpInput = new("Jump", 0.2f);
    private readonly InputButton dodgeInput = new("Roll", 0.2f);
    private readonly InputButton attackInput = new("Attack", 0.2f);

    private RaycastHit2D groundRaycastHit;
    private RaycastHit2D leftWallRaycastHit;
    private RaycastHit2D rightWallRaycastHit;

    private float timeSinceLastJump = 9999.9f;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.gravityScale = baseGravityScale;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();

        InitializeStateMachine();
    }

    private void InitializeStateMachine()
    {
        stateMachine = new StateMachine<StateKey, Knight>(debugMode);


        var idleState = new Idle(this);
        stateMachine.AddState(StateKey.Idle, idleState);

        var runState = new Run(this);
        stateMachine.AddState(StateKey.Run, runState);

        var crouchState = new Crouch(this);
        stateMachine.AddState(StateKey.Crouch, crouchState);

        var slideState = new Slide(this);
        stateMachine.AddState(StateKey.Slide, slideState);

        var dodgeState = new Dodge(this);
        stateMachine.AddState(StateKey.Dodge, dodgeState);

        var rollState = new Roll(this);
        stateMachine.AddState(StateKey.Roll, rollState);


        var aerialState = new Aerial(this);
        stateMachine.AddState(StateKey.Aerial, aerialState);

        var airDiveState = new AirDive(this);
        stateMachine.AddState(StateKey.AirDive, airDiveState);


        var wallSlideState = new WallSlide(this);
        stateMachine.AddState(StateKey.WallSlide, wallSlideState);


        var attackComboState = new AttackCombo(this);
        stateMachine.AddState(StateKey.AttackCombo, attackComboState);

        var attackCrouchState = new AttackCrouch(this);
        stateMachine.AddState(StateKey.AttackCrouch, attackCrouchState);


        stateMachine.Begin(StateKey.Idle);
    }

    private void Update()
    {
        GatherInput();

        groundRaycastHit = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, terrainLayerMask);
        leftWallRaycastHit = Physics2D.Raycast(leftWallRaycastPoint.position, Vector2.left, 0.04f, terrainLayerMask);
        rightWallRaycastHit = Physics2D.Raycast(rightWallRaycastPoint.position, Vector2.right, 0.04f, terrainLayerMask);

        timeSinceLastJump += Time.deltaTime;

        stateMachine.Update();

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void GatherInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        jumpInput.Update();
        dodgeInput.Update();
        attackInput.Update();
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("speed x", Mathf.Abs(rbody.linearVelocityX));
        animator.SetFloat("speed y", Mathf.Abs(rbody.linearVelocityY));
        animator.SetBool("is falling", IsFalling());
        animator.SetBool("is grounded", IsGrounded());
    }

    private void MoveTowardsX(float targetVelocityX, float acceleration)
    {
        // Faster acceleration if trying to turn in opposite direction or stopping
        float accelFactor = Mathf.Approximately(Mathf.Sign(targetVelocityX), Mathf.Sign(rbody.linearVelocityX)) ? 1.0f : 2.0f;
        rbody.linearVelocityX = Mathf.MoveTowards(rbody.linearVelocityX, targetVelocityX, accelFactor * acceleration * Time.deltaTime);
    }

    private void MoveTowardsY(float targetVelocityY, float acceleration)
    {
        // Faster acceleration if trying to turn in opposite direction or stopping
        float accelFactor = Mathf.Approximately(Mathf.Sign(targetVelocityY), Mathf.Sign(rbody.linearVelocityY)) ? 1.0f : 2.0f;
        rbody.linearVelocityY = Mathf.MoveTowards(rbody.linearVelocityY, targetVelocityY, accelFactor * acceleration * Time.deltaTime);
    }

    private void Jump()
    {
        rbody.linearVelocityY = jumpSpeed;
        animator.SetTrigger("jump");
        FlipSpriteToFaceInputDirection();
        timeSinceLastJump = 0.0f;
    }

    private void WallJump()
    {
        Vector2 velocityVector;
        velocityVector.x = wallJumpSpeed * Mathf.Cos(Mathf.Deg2Rad * wallJumpAngle);
        velocityVector.x *= IsWallToRight() ? -1 : 1;
        velocityVector.y = wallJumpSpeed * Mathf.Sin(Mathf.Deg2Rad * wallJumpAngle);
        rbody.linearVelocity = velocityVector;
        FlipSprite(IsWallToRight());
        timeSinceLastJump = 0.0f;
    }

    private bool IsFacingLeft() => spriteRenderer.flipX;
    private bool IsFacingRight() => !spriteRenderer.flipX;
    private float GetFacingDirection() => spriteRenderer.flipX ? -1.0f : 1.0f;
    private bool WantsToTurn() => (horizontalInput < 0.0f && IsFacingRight())
                || (horizontalInput > 0.0f && IsFacingLeft());
    private void FlipSprite(bool facingLeft) => spriteRenderer.flipX = facingLeft;
    private void FlipSpriteToFaceInputDirection()
    {
        if (horizontalInput == 0.0f) return;
        spriteRenderer.flipX = horizontalInput < 0.0f;
    }

    private bool IsGrounded() => groundRaycastHit && rbody.linearVelocityY <= 0.5f;
    private bool IsFalling() => !IsGrounded() && rbody.linearVelocityY < 0.0f;
    private bool IsWallToLeft() => leftWallRaycastHit;
    private bool IsWallToRight() => rightWallRaycastHit;
    private bool IsInputtingTowardsWall()
    {
        if (IsWallToLeft()) { return horizontalInput < 0.0f; }
        if (IsWallToRight()) { return horizontalInput > 0.0f; }
        return false;
    }
    private void SnapToWall()
    {
        RaycastHit2D hit = IsWallToLeft() ? leftWallRaycastHit : rightWallRaycastHit;
        Vector2 wallOffset = transform.position - (IsWallToLeft() ? leftWallRaycastPoint.position : rightWallRaycastPoint.position);
        rbody.MovePosition(hit.point + wallOffset);
    }
    
    public string GetCurrentStateString() => stateMachine.GetCurrentStateString();
}
