using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : EnemyController
{
    

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, new Vector2(PlayerController.instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }
    public override void EnemyHit(float _damage, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damage, _hitDirection, _hitForce);
    }
}
