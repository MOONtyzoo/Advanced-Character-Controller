using System.Collections;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField] private float runSpeed;
    [SerializeField] private float runAccel;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private LayerMask terrainLayerMask;

    private Rigidbody2D rbody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isGrounded = false;
    private bool isFalling = false;
    private bool isTurning = false;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float jumpInput = Input.GetAxis("Jump");

        // Faster acceleration if trying to turn in opposite direction or stopping
        float accelFactor = Mathf.Sign(horizontalInput) == Mathf.Sign(rbody.linearVelocityX) ? 1.0f : 2.0f;
        rbody.linearVelocityX = Mathf.MoveTowards(rbody.linearVelocityX, horizontalInput * runSpeed, accelFactor * runAccel * Time.deltaTime);

        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, terrainLayerMask);
        isFalling = !isGrounded && rbody.linearVelocityY < 0.0f;

        if (isGrounded && jumpInput > 0.0f)
        {
            rbody.linearVelocityY = jumpVelocity;
            animator.SetTrigger("jump");
        }

        animator.SetFloat("run speed", Mathf.Abs(rbody.linearVelocityX));
        animator.SetBool("is falling", isFalling);
        animator.SetBool("is grounded", isGrounded);

        if (isGrounded && horizontalInput != 0.0f && ((horizontalInput < 0.0f) != spriteRenderer.flipX))
        {
            if (Mathf.Abs(rbody.linearVelocityX) > 2.0f)
            {
                isTurning = true;
            }
            else
            {
                if (!isTurning) spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
        else
        {
            isTurning = false;
        }

        animator.SetBool("is turning", isTurning);
    }
}
