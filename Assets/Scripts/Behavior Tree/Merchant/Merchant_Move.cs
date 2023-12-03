using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Merchant_Move : Action
{
    Merchant merchant;
    public Vector2 targetPos;

    public override void OnAwake()
    {
        merchant = GetComponent<Merchant>();
    }


    public override TaskStatus OnUpdate()
    {
        float distance = Mathf.Abs(targetPos.x - merchant.transform.position.x);
        if(distance < 0.1f)
        {
            return TaskStatus.Success;
        }
        else
        {
            merchant.targetPos = targetPos;
            return TaskStatus.Running;
        }
    }
}
