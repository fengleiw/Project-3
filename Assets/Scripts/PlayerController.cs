using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;

    public static PlayerController instance;

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [SerializeField] private Transform groundCheck;

    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float jumpForce = 10f;

    [SerializeField] private LayerMask collisionMask;

    [SerializeField]
    private int maxJump = 2;
    private int jumpLeft;

    private float xAxis;
    Animator anim;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        jumpLeft = maxJump;


    }

    private void Update()
    {
        GetInput();
        Movement();
        Jump();
        Flip();
    }

    private void Movement()
    {
        //Move Horizontal
        var horizontalInput = Input.GetAxis("Horizontal");
        
       
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);

        anim.SetBool("isRunning", rb.velocity.x != 0 && IsGrounded());

        //Flip
        //if (horizontalInput != 0)
        //{
        //    transform.localScale = new Vector3(Mathf.Sign(-horizontalInput), 1, 1);
        //}
    }

    private void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }
    private void Flip()
    {
        if(xAxis > 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        } else if (xAxis < 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }

    private void Jump()
    {
        var jumpInput = Input.GetButtonDown("Jump");
        var jumpInputReleased = Input.GetButtonUp("Jump");
        //reset when grounded
        if(IsGrounded() && rb.velocity.y <= 0)
        {
            jumpLeft = maxJump;
        }
        if (jumpInput && jumpLeft > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            jumpLeft -= 1;
            
        }

        if (jumpInputReleased && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            
        }
        anim.SetBool("isJumping", !IsGrounded());
    }

    
    private bool IsGrounded()
    {
        if (Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckY, collisionMask)
            || Physics2D.Raycast(groundCheck.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, collisionMask)
            || Physics2D.Raycast(groundCheck.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, collisionMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
