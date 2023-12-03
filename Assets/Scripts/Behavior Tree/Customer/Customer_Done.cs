using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Done : Action
{
    public Customer c;
    public override void OnAwake()
    {
        c = GetComponent<Customer>();
    }
    public override TaskStatus OnUpdate()
    {
        c.targetPos = GameManager.Instance.MapEdgePosition;
        if (Mathf.Abs(transform.position.x - c.targetPos.x) < 0.1)
        {
            return TaskStatus.Success;
        }
        return TaskStatus.Running;
    }
}
