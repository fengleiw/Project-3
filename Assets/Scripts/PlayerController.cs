using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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

    [Header("Attacking Setting")]
    bool attack = false;
    float timeBetweenAttack, timeSinceAttack;
    [SerializeField] Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float damage;
    [SerializeField] GameObject slashEffect;

    [Header("Recoil Setting")]
    [SerializeField] int recoilXStep = 5;
    [SerializeField] int recoilYStep = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepXRecoilded, stepYRecoilded;
    [Space(5)]

    [Header("Health Setting")]
    public int health;
    public int maxHealth;
    public delegate void OnHealthChangeDelegate();
    [HideInInspector] public OnHealthChangeDelegate onHealthChangeCallback;
    [SerializeField] float hitFlashSpeed;
    [Space(5)]

    private SpriteRenderer sr;

    private float xAxis, yAxis;
    Animator anim;
    public PlayerStateList pState;
    private Rigidbody2D rb;

    public static PlayerController instance;

    bool restoreTime;
    float restoreTimeSpeed;

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
        sr = GetComponent<SpriteRenderer>();
        jumpLeft = maxJump;
        gravity = rb.gravityScale;
        Health = maxHealth;
    }

    private void Update()
    {
        GetInput();
        if (pState.dashing) return;
        Movement();
        Jump();
        Flip();
        StartDash();
        Attack();
        RestoreTimeScale();
        FlashWhileInvincible();
        Debug.Log(Time.timeScale);
    }

    private void FixedUpdate()
    {
        if(pState.dashing) return;
        Recoil();
    }
    private void Movement()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        anim.SetBool("isRunning", rb.velocity.x != 0 && IsGrounded());
    }


    private void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if(Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            } else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;
        if(_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        } else
        {
            restoreTime = true;
        }
    }

    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ? 
            Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }

    private void StartDash()
    {
        //Got some problems with canDash variable (fixed)
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }
        if (IsGrounded())
        {
            dashed = false;
        }
    }
    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("takeDamage");

        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                if(onHealthChangeCallback != null)
                {
                    onHealthChangeCallback.Invoke();
                }
            }
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;

        anim.SetTrigger("Dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(-transform.localScale.x * dashSpeed, 0);
        /*if (IsGrounded())*/
        Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
    }
    private void Flip()
    {
        if (xAxis > 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookingLeft = false;
        }
        else if (xAxis < 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookingLeft = true;
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


    private void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;
            anim.SetTrigger("Attacking");

            if (yAxis == 0 || yAxis < 0 && IsGrounded())
            {
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, recoilXSpeed);
                Instantiate(slashEffect, SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, UpAttackTransform);
            }
            else if (yAxis < 0 && !IsGrounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 90, DownAttackTransform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }
    private void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        if (objectsToHit.Length > 0)
        {
            _recoilDir = true;
        }
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<EnemyController>() != null)
            {
                objectsToHit[i].GetComponent<EnemyController>().EnemyHit(damage, (transform.position - objectsToHit[i].transform.position).normalized, recoilStrength);
            }
        }
    }

    //Need to change this below code for up and down attack animation, they have other animation with match with them.
    //Just for testing
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform);
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle);
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    private void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookingLeft)
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {

                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            jumpLeft = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }

        //Stop recoil
        if (pState.recoilingX && stepXRecoilded < recoilXStep)
        {
            stepXRecoilded++;
        }
        else
        {
            StopRecoilX();
        }
        if (pState.recoilingY && stepYRecoilded < recoilYStep)
        {
            stepYRecoilded++;
        }
        else
        {
            StopRecoilY();
        }
        if (IsGrounded())
        {
            StopRecoilY();
        }

    }

    private void StopRecoilX()
    {
        stepXRecoilded = 0;
        pState.recoilingX = false;
    }
    private void StopRecoilY()
    {
        stepYRecoilded = 0;
        pState.recoilingY = false;
    }
    public void TakeDamage(float _damage)
    {
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
    }
}