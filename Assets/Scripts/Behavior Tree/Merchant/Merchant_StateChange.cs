using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;


public class Merchant_StateChange : Action
{
    Merchant merchant;
    public Merchant.State newState;

    public override void OnAwake()
    {
        merchant = GetComponent<Merchant>();
    }

    public override TaskStatus OnUpdate()
    {
        merchant.state = newState;
        return TaskStatus.Success;
    }
}

