using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{

    [Header("Horizontal Setting")]
    [SerializeField] private float speed = 2f;
    [Space(5)]

    [Header("Vertical Setting")]
    [SerializeField] private int maxJump = 2;
    private int jumpLeft;
    [SerializeField] private float jumpForce = 10f;
    [Space(5)]

    [Header("Ground Check Setting")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask collisionMask;
    [Space(5)]

    [Header("Dash Setting")]
    private bool canDash = true;
    private bool dashed;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;

    [SerializeField] GameObject dashEffect;
    private float gravity;
    [Space(5)]

    private float xAxis;
    Animator anim;
    PlayerStateList pState;
    private Rigidbody2D rb;

    public static PlayerController instance;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
        jumpLeft = maxJump;
        gravity = rb.gravityScale;
    }

    private void Update()
    {
        GetInput();
        if (pState.dashing) return;
        Movement();
        Jump();
        Flip();
        StartDash();
    }

    private void Movement()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        anim.SetBool("isRunning", rb.velocity.x != 0 && IsGrounded());
    }

    private void StartDash()
    {
        //Got some problems with canDash variable (fixed)
        if(Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (IsGrounded()) {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;

        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(-transform.localScale.x * dashSpeed, 0);
        /*if (IsGrounded())*/ Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
    }
    private void Flip()
    {
        if (xAxis > 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (xAxis < 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
    }

    private void Jump()
    {
        var jumpInput = Input.GetButtonDown("Jump");
        var jumpInputReleased = Input.GetButtonUp("Jump");

        //reset when grounded
        if (IsGrounded() && rb.velocity.y <= 0)
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