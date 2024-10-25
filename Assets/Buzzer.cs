using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buzzer : EnemyController
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;
    float timer;
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Buzzer_Idle);
    }
    protected override void Update()
    {
        base.Update();
        
    }
    protected override void UpdateEnemyState() //fixed
    {
        
        float _dist = Vector2.Distance(transform.position, PlayerController.instance.transform.position); 
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Buzzer_Idle:
                rb.velocity = new Vector2(0, 0);
                if(_dist < chaseDistance) 
                {
                    ChangeState(EnemyStates.Buzzer_Chase); 
                    
                }

                break;

            case EnemyStates.Buzzer_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.instance.transform.position, Time.deltaTime * speed)); 

                FlipBuzzer();
                if(_dist > chaseDistance)
                {
                    ChangeState(EnemyStates.Buzzer_Idle);
                }
                break;

            case EnemyStates.Buzzer_Stunned:
                timer += Time.deltaTime;

                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Buzzer_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Buzzer_Death:
                Death(Random.Range(5,10));
                break;
        }
    }

    void FlipBuzzer()
    {

        sr.flipX =  PlayerController.instance.transform.position.x > transform.position.x;
    }

    public override void EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damage, _hitDirection, _hitForce);

        if(health > 0)
        {
            ChangeState(EnemyStates.Buzzer_Stunned);
        } else
        {
            ChangeState(EnemyStates.Buzzer_Death);
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12;
        base.Death(_destroyTime);
    }

    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("idle", GetCurrentEnemyState == EnemyStates.Buzzer_Idle);
        anim.SetBool("chase", GetCurrentEnemyState == EnemyStates.Buzzer_Chase);
        anim.SetBool("stun", GetCurrentEnemyState == EnemyStates.Buzzer_Stunned);
        if(GetCurrentEnemyState == EnemyStates.Buzzer_Death)
        {
            anim.SetTrigger("death");
        }
    }
}
