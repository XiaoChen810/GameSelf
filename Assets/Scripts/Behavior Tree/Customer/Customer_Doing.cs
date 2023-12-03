using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Customer_Doing : Action
{
    public Customer c;

    public override void OnAwake()
    {
        c = GetComponent<Customer>();
    }
    public override TaskStatus OnUpdate()
    {
        // 无论是否接受，只要耐心值为0，则直接退出进入Done状态
        if (c.patience <= 0)
        {
            return TaskStatus.Failure;
        }

        // 走过去
        c.targetPos = GameManager.Instance.GetDoorPosition(c.targetPos);
        // 判断是否点击了
        if (c.m_order.isSelect)
        {

            // 判断是否接受
            if (c.m_order.isAccept)
            {
                // 被答应，进入Wait状态，并开启OnWait
                c.OnWait();
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        }
        return TaskStatus.Running;
    }
}
