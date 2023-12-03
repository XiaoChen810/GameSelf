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
        // �����Ƿ���ܣ�ֻҪ����ֵΪ0����ֱ���˳�����Done״̬
        if (c.patience <= 0)
        {
            return TaskStatus.Failure;
        }

        // �߹�ȥ
        c.targetPos = GameManager.Instance.GetDoorPosition(c.targetPos);
        // �ж��Ƿ�����
        if (c.m_order.isSelect)
        {

            // �ж��Ƿ����
            if (c.m_order.isAccept)
            {
                // ����Ӧ������Wait״̬��������OnWait
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
