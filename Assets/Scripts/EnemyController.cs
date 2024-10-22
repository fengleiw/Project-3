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
    protected float recoilTimer;
    protected Rigidbody2D rb;

    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.instance;
    }

    
    protected virtual void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            } else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public virtual void EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damage;
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
            isRecoiling = true;
        }
    }

    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.instance.pState.invincible)
        {
            Attack();
            PlayerController.instance.HitStopTime(0, 5, 0.5f);
        }
    }

    protected virtual void Attack()
    {
        PlayerController.instance.TakeDamage(damage);
    }
    
}