using System;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpPower = 20f;
    [SerializeField] private State playersState;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;


    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D BoxCollider;
    private float wallJumpCoolDown;
    private float gravity = 5f;
    private float horizontalInput;

    enum State
    {
        Idle,
        Run,
        Jump,
        Grounded
    }

    private void Awake()
    {
        playersState = State.Idle;
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        BoxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        PlayerMove();
        PlayerJump();


        animator.SetBool("IsRunning", playersState == State.Run);
        animator.SetBool("IsGrounded", isGrounded());

    }

    void PlayerJump()
    {
        if (wallJumpCoolDown > 0.2f)
        {
//            horizontalInput = Input.GetAxis("Horizontal");
            float x = horizontalInput * speed;

            body.velocity = new Vector2(x, body.velocity.y);

            if (OnWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector3.zero;
            }
            else
            {
                body.gravityScale = gravity;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else
        {
            wallJumpCoolDown += Time.deltaTime;
        }
    }

    void Jump()
    {
        if (isGrounded())
        {
            playersState = State.Jump; 
            float y = jumpPower;
            body.velocity = new Vector2(body.velocity.x, y);
            animator.SetTrigger("Jump");
        }
        else if(OnWall() && !isGrounded())
        {
            if (horizontalInput != 0)
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 0); //1 for right, -1 for left
                transform.localScale = new Vector3(-Mathf.Sign(transform.localScale.x) ,transform.localScale.y, transform.localScale.z);
            }
            else
            {
                body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 6); //1 for right, -1 for left

            }
            wallJumpCoolDown = 0f;
        }
    }

    void PlayerMove()
    {
   //     horizontalInput = Input.GetAxis("Horizontal");
        float x = horizontalInput * speed;
        
        body.velocity = new Vector2(x, body.velocity.y);

        //flip player when moving
        if (horizontalInput > 0.01f)
        {
            playersState = State.Run;
            transform.localScale = Vector3.one; // if player scale is different than 1,1,1, you need to create another vector
        }
        else if(horizontalInput < -0.01f)
        {
            playersState = State.Run;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if(horizontalInput == 0)
        {
            playersState = State.Idle;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(BoxCollider.bounds.center, BoxCollider.bounds.size,0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool OnWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(BoxCollider.bounds.center, BoxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0),
            0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool canAttack()
    {
        return horizontalInput == 0 && isGrounded() && !OnWall();
    }
}
