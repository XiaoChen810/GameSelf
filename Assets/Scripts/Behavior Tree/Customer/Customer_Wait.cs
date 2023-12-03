using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Wait : Action
{
    public Customer c;
    public override void OnAwake()
    {
        c = GetComponent<Customer>();
    }


    public override TaskStatus OnUpdate()
    {
        if (c.m_order.isFinish)
        {
            return TaskStatus.Success;
        }
        if(c.patience <=  0)
        {
            return TaskStatus.Failure;
        }
        return TaskStatus.Running;
    }
}
