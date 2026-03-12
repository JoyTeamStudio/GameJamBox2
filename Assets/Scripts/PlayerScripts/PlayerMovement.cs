using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private Rigidbody2D rb;
    private PlayerHealth health;
    //private TrailRenderer trail;
    //private Animator animator;

    public float speed;
    public float jump;
    public float dash;
    private float horizontal;

    public Transform[] groundCheck;
    public LayerMask groundLayer;

    private bool isDashing;
    private bool canDash;
    private bool isFacingRight;
    public bool grounded;
    public bool canMove;

    public int jumps;

    public bool hasDoubleJump;
    public bool hasDash;

    public RoomTransition.Direction currentDirection;
    public bool isTransitioning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<PlayerHealth>();
        //trail = GetComponent<TrailRenderer>();
        //animator = GetComponent<Animator>();

        isFacingRight = true;
        canDash = true;
        canMove = true;
    }

    private void Update()
    {
        //animator.SetBool("isDashing", isDashing);
        if (isDashing) return;

        if (canMove)
        {
            horizontal = Input.GetAxisRaw("Horizontal");

            if (grounded)
                jumps = 1;

            if (Input.GetButtonDown("Jump") && (grounded || jumps > 0))
            {
                if(grounded || hasDoubleJump)
                {
                    jumps--;
                    /*if (!grounded)
                    {
                        for (int i = 0; i < groundCheck.Length; i++)
                        {
                            groundCheck[i].GetComponent<ParticleSystem>().Play();
                        }
                    }*/
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jump);
                }
            }

            if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            {
                Jump(rb.linearVelocity.y * 0.5f);
            }

            if (Input.GetButtonDown("Dash") && canDash && hasDash)
            {
                StartCoroutine(Dash());
            }

            //animator.SetBool("isFalling", rb.linearVelocity.y < 0);
        }

        if(isTransitioning)
        {
            if (currentDirection == RoomTransition.Direction.Right || currentDirection == RoomTransition.Direction.Left)
                transform.Translate(speed * Time.deltaTime * Vector3.right);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isDashing) return;

        if(canMove)
        {
            grounded = IsGrounded();
            rb.linearVelocity = new Vector2(horizontal * speed, rb.linearVelocity.y);
            //animator.SetBool("isWalking", horizontal != 0);
            Flip();
        }
    }

    public void Jump(float jumpValue)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpValue);
    }

    private void Flip()
    {
        if(isFacingRight && horizontal < 0 || !isFacingRight && horizontal > 0)
        {
            transform.eulerAngles += new Vector3(0, 180, 0);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Clamp(transform.eulerAngles.y, -180, 180), transform.eulerAngles.z);
            isFacingRight = !isFacingRight;
        }
    }

    public IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float multi = 1;

        if (!isFacingRight)
            multi = -multi;

        rb.linearVelocity = new Vector2(dash * multi, 0);
        //trail.emitting = true;

        yield return new WaitForSeconds(0.2f);
        //trail.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(0.4f);
        canDash = true;
    }

    public bool IsGrounded()
    {
        bool grounded = false;

        for(int i = 0; i < groundCheck.Length; i++)
        {
            grounded = grounded || Physics2D.OverlapCircle(groundCheck[i].position, 0.1f, groundLayer);
            if(grounded) break;
        }

        return grounded;
    }

    public IEnumerator Heal()
    {
        float gravity = rb.gravityScale;
        playerAttack.canAttack = false;
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0;
        yield return new WaitForSeconds(0.85f);
        health.Heal();
        rb.gravityScale = gravity;
        canMove = true;
        health.isHealing = false;
        playerAttack.canAttack = true;
    }

    public void Transition()
    {
        rb.linearVelocity = Vector2.zero;
        isTransitioning = true;

        if (currentDirection == RoomTransition.Direction.Up)
            rb.AddForce(new Vector2(4, 22), ForceMode2D.Impulse);

        StartCoroutine(StopTransition());
    }

    public IEnumerator StopTransition()
    {
        yield return new WaitForSeconds(0.65f);
        EndTransition();
    }

    public void EndTransition()
    {
        canMove = true;
        isTransitioning = false;
        rb.gravityScale = 3;
    }

    public void StartTransition(RoomTransition.Direction direction)
    {
        currentDirection = direction;
        isDashing = false;
        canMove = false;
        isTransitioning = true;
        canDash = true;
    }

    public void StopMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        GetComponent<PlayerAttack>().canAttack = false;
    }

    public void StartMovement()
    {
        canMove = true;
        GetComponent<PlayerAttack>().canAttack = true;
    }
}
