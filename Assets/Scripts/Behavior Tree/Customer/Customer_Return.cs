using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer_Return : Action
{
    public Customer c;
    public override void OnAwake()
    {
        c = GetComponent<Customer>();
    }

    public override TaskStatus OnUpdate()
    {
        // ���뿪ǰ��λ�û���ȥ
        GameManager.Instance.LeaveDoor(c.targetPos);
        if (c.chatCoroutine != null) c.StopCoroutine(c.chatCoroutine);
        return TaskStatus.Success;


    }
}
