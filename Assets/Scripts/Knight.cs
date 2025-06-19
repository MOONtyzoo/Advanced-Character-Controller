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

        if (isGrounded && jumpInput > 0)
        {
            rbody.linearVelocityY = jumpVelocity;
        }

        spriteRenderer.flipX = rbody.linearVelocityX == 0.0f ? spriteRenderer.flipX : rbody.linearVelocityX > 0.0f ? false : true;
        if (isGrounded)
        {
            if (rbody.linearVelocityX == 0.0f)
            {
                animator.Play("Idle");
            }
            else
            {
                animator.Play("Run");
            }
        }
        else
        {
            if (rbody.linearVelocityY > 0.0f)
            {
                animator.Play("Jump");
            }
            else
            {
                animator.Play("Fall");
            }
        }
    }
}
