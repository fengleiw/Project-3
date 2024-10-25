using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ZombieBasic : EnemyController
{
    float timer;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float chargeDuration;
    [SerializeField] private LayerMask whatIsGround;


    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.ZB_Idle);

        rb.gravityScale = 12f;
    }

    protected override void UpdateEnemyState()
    {


        if (health <= 0)
        {
            Death(0.05f);
        }
        Vector3 _ledgeCheckStartPoint = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.ZB_Idle:
                

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStartPoint, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStartPoint, _wallCheckDir, ledgeCheckX * 10); //doesnt work if ledgeCheckX < 0.45 ?

                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.ZB_Suprised);
                    
                }

                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                    
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                    
                }
                break;
            case EnemyStates.ZB_Suprised:
                rb.velocity = new Vector2(0, jumpForce);
                ChangeState(EnemyStates.ZB_Charge);

                break;
            case EnemyStates.ZB_Charge:
                timer += Time.deltaTime;
                //UnityEngine.Debug.Log(timer);
                if (timer < chargeDuration)
                {
                    
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        
                        if (transform.localScale.x > 0)
                        {
                            rb.velocity = new Vector2(speed * chargeSpeedMultiplier, rb.velocity.y);
                            
                        }
                        else
                        {
                            rb.velocity = new Vector2(-speed * chargeSpeedMultiplier, rb.velocity.y);
                            
                        }
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.ZB_Idle);
                }
                break;
        }
    }

    protected override void ChangeCurrentAnimation()
    {
        if (currentEnemyState == EnemyStates.ZB_Idle)
        {
            anim.speed = 1;
        }
        if (currentEnemyState == EnemyStates.ZB_Charge)
        {
            anim.speed = chargeSpeedMultiplier;
        }
    }
}

