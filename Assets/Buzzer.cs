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
        switch (currentEnemyState)
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
}
