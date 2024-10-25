using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;
    [SerializeField] protected float damage;
    [SerializeField] protected GameObject blood;
    protected float recoilTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    [HideInInspector][SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;

    protected Animator anim;


    protected enum EnemyStates
    {
        //Crawler
        Crawler_Idle,
        Crawler_Flip,
        Crawler_Run,

        //Buzzer
        Buzzer_Idle,
        Buzzer_Chase,
        Buzzer_Stunned,
        Buzzer_Death,

        //Zomebie basic
        ZB_Idle,
        ZB_Suprised,
        ZB_Charge
    }
    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    protected EnemyStates currentEnemyState;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = PlayerController.instance;

    }


    protected virtual void Update()
    {


        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        else
        {
            UpdateEnemyState();
        }
    }


    protected virtual void UpdateEnemyState()
    {

    }
    protected virtual void ChangeCurrentAnimation() { }

    public virtual void EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damage;
        if (!isRecoiling)
        {
            GameObject _blood = Instantiate(blood, transform.position, Quaternion.identity);
            Destroy(_blood, 5.5f);
            rb.velocity = (-_hitForce * recoilFactor * _hitDirection);
        }
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }

    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.instance.pState.invincible && health > 0)
        {
            Attack();
            PlayerController.instance.HitStopTime(0, 5, 0.5f);
        }
    }

    protected virtual void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }

    protected virtual void Attack()
    {
        PlayerController.instance.TakeDamage(damage);
    }

}