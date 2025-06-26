using System.Collections;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;

    [Header("References")]
    [SerializeField] private LayerMask terrainLayerMask;
    [SerializeField] private Transform leftWallRaycastPoint;
    [SerializeField] private Transform rightWallRaycastPoint;

    [Header("Run")]
    [SerializeField] public float runSpeed;
    [SerializeField] public float runAccel;

    [Header("Jump")]
    [SerializeField] public float baseGravityScale;
    [SerializeField] public float jumpSpeed;

    [Header("Crouch")]
    [SerializeField] public float crouchSpeed;
    [SerializeField] public float crouchAccel;

    [Header("Slide")]
    [SerializeField] public float slideSpeed;

    [Header("Dodge")]
    [SerializeField] public AnimationCurve dodgeSpeedCurve;
    [SerializeField] public float dodgeStartSpeed;
    [SerializeField] public float dodgeEndSpeed;
    [SerializeField] public float dodgeDuration;


    [Header("Dive")]
    [SerializeField] public float diveSpeed;
    [SerializeField] public float diveUpVelocity;

    [Header("Roll")]
    [SerializeField] public float rollSpeed;
    [SerializeField] public float rollDuration;

    [Header("Wall")]
    [SerializeField] public float wallSlideSpeed;
    [SerializeField] public float wallSlideAccel;
    [SerializeField] public float wallJumpSpeed;
    [SerializeField] public float wallJumpAngle;

    [Header("Attacks")]
    [SerializeField] public float attackCombo1Duration;
    [SerializeField] public float attackCombo1EndLag;
    [SerializeField] public float attackCombo2Duration;
    [SerializeField] public float attackCombo2EndLag;
    [SerializeField] public float attackCrouchDuration;

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

    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public InputButton jumpInput = new InputButton("Jump", 0.2f);
    [HideInInspector] public InputButton dodgeInput = new InputButton("Roll", 0.2f);
    [HideInInspector] public InputButton attackInput = new InputButton("Attack", 0.2f);

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

        SetUpStateMachine();
    }

    private void SetUpStateMachine()
    {
        stateMachine = new StateMachine<StateKey, Knight>(debugMode);


        BaseState<StateKey, Knight> idleState = new PlayerStates.Idle(this);
        stateMachine.AddState(StateKey.Idle, idleState);

        BaseState<StateKey, Knight> runState = new PlayerStates.Run(this);
        stateMachine.AddState(StateKey.Run, runState);

        BaseState<StateKey, Knight> crouchState = new PlayerStates.Crouch(this);
        stateMachine.AddState(StateKey.Crouch, crouchState);

        BaseState<StateKey, Knight> slideState = new PlayerStates.Slide(this);
        stateMachine.AddState(StateKey.Slide, slideState);

        BaseState<StateKey, Knight> dodgeState = new PlayerStates.Dodge(this);
        stateMachine.AddState(StateKey.Dodge, dodgeState);

        BaseState<StateKey, Knight> rollState = new PlayerStates.Roll(this);
        stateMachine.AddState(StateKey.Roll, rollState);


        BaseState<StateKey, Knight> aerialState = new PlayerStates.Aerial(this);
        stateMachine.AddState(StateKey.Aerial, aerialState);

        BaseState<StateKey, Knight> airDiveState = new PlayerStates.AirDive(this);
        stateMachine.AddState(StateKey.AirDive, airDiveState);


        BaseState<StateKey, Knight> wallSlideState = new PlayerStates.WallSlide(this);
        stateMachine.AddState(StateKey.WallSlide, wallSlideState);


        BaseState<StateKey, Knight> attackComboState = new PlayerStates.AttackCombo(this);
        stateMachine.AddState(StateKey.AttackCombo, attackComboState);

        BaseState<StateKey, Knight> attackCrouchState = new PlayerStates.AttackCrouch(this);
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

    public void MoveTowardsX(float targetVelocityX, float acceleration)
    {
        // Faster acceleration if trying to turn in opposite direction or stopping
        float accelFactor = Mathf.Sign(targetVelocityX) == Mathf.Sign(rbody.linearVelocityX) ? 1.0f : 2.0f;
        rbody.linearVelocityX = Mathf.MoveTowards(rbody.linearVelocityX, targetVelocityX, accelFactor * acceleration * Time.deltaTime);
    }

    public void MoveTowardsY(float targetVelocityY, float acceleration)
    {
        // Faster acceleration if trying to turn in opposite direction or stopping
        float accelFactor = Mathf.Sign(targetVelocityY) == Mathf.Sign(rbody.linearVelocityY) ? 1.0f : 2.0f;
        rbody.linearVelocityY = Mathf.MoveTowards(rbody.linearVelocityY, targetVelocityY, accelFactor * acceleration * Time.deltaTime);
    }

    public void Jump()
    {
        rbody.linearVelocityY = jumpSpeed;
        animator.SetTrigger("jump");
        FlipSpriteToFaceInputDirection();
        timeSinceLastJump = 0.0f;
    }

    public void WallJump()
    {
        Vector2 velocityVector;
        velocityVector.x = wallJumpSpeed * Mathf.Cos(Mathf.Deg2Rad * wallJumpAngle);
        velocityVector.x *= IsWallToRight() ? -1 : 1;
        velocityVector.y = wallJumpSpeed * Mathf.Sin(Mathf.Deg2Rad * wallJumpAngle);
        rbody.linearVelocity = velocityVector;
        FlipSprite(IsWallToRight());
        timeSinceLastJump = 0.0f;
    }

    public Animator GetAnimator() => animator;

    public bool IsFacingLeft() => spriteRenderer.flipX;
    public bool IsFacingRight() => !spriteRenderer.flipX;
    public float GetFacingDirection() => spriteRenderer.flipX ? -1.0f : 1.0f;
    public bool WantsToTurn() => (horizontalInput < 0.0f && IsFacingRight())
                || (horizontalInput > 0.0f && IsFacingLeft());
    public void FlipSprite(bool facingLeft) => spriteRenderer.flipX = facingLeft;
    public void FlipSpriteToFaceInputDirection()
    {
        if (horizontalInput == 0.0f) return;
        spriteRenderer.flipX = horizontalInput < 0.0f;
    }

    public Vector2 GetVelocity() => rbody.linearVelocity;
    public float GetVelocityX() => rbody.linearVelocityX;
    public void SetVelocityX(float newVelocityX) => rbody.linearVelocityX = newVelocityX;
    public float GetVelocityY() => rbody.linearVelocityY;
    public void SetVelocityY(float newVelocityY) => rbody.linearVelocityY = newVelocityY;
    public void SetGravityScale(float newGravityScale) => rbody.gravityScale = newGravityScale;

    public bool IsGrounded() => groundRaycastHit && rbody.linearVelocityY <= 0.5f;
    public bool IsFalling() => !IsGrounded() && rbody.linearVelocityY < 0.0f;
    public bool IsWallToLeft() => leftWallRaycastHit;
    public bool IsWallToRight() => rightWallRaycastHit;
    public bool IsInputtingTowardsWall()
    {
        if (IsWallToLeft()) { return horizontalInput < 0.0f; }
        else if (IsWallToRight()) { return horizontalInput > 0.0f; }
        else { return false; }
    }
    public void SnapToWall()
    {
        RaycastHit2D hit = IsWallToLeft() ? leftWallRaycastHit : rightWallRaycastHit;
        Vector2 wallOffset = transform.position - (IsWallToLeft() ? leftWallRaycastPoint.position : rightWallRaycastPoint.position);
        rbody.MovePosition(hit.point + wallOffset);
    }

    public float GetTimeSinceLastJump() => timeSinceLastJump;
    

    public string GetCurrentStateString() => stateMachine.GetCurrentStateString();
}
