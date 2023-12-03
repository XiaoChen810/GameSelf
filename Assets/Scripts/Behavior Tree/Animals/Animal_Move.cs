using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_Move : Action
{
    private Rigidbody2D rb;
    private float dir;
    public SharedVector2 sharedVector2;
    public float Speed;
    public float minRange;
    public float maxRange;

    public override void OnAwake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override TaskStatus OnUpdate()
    {
        dir = sharedVector2.Value.x - transform.position.x;
        if (Mathf.Abs(dir) > 0.1f)
        {
            return TaskStatus.Running;
        }
        else
        {
            rb.velocity = Vector2.zero;

            Vector2 newPos = sharedVector2.Value += new Vector2(Random.Range(-5f, 5f), 0);
            newPos.x = newPos.x > maxRange ? maxRange : newPos.x;
            newPos.x = newPos.x < minRange ? minRange : newPos.x;

            sharedVector2.Value = newPos;
            return TaskStatus.Success;
        }
    }

    public override void OnFixedUpdate()
    {
        dir = dir > 0 ? 1 : -1;
        rb.velocity = new Vector2(dir * Speed * Time.deltaTime, rb.velocity.y);
        transform.localScale = new Vector3(-dir, 1, 1);
    }
    
}
