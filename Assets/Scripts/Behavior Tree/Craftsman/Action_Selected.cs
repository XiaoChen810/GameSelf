using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Action_Selected : Action
{
    public Craftsman c;

    public override TaskStatus OnUpdate()
    {
        if (c.isSelected)
        {
            return TaskStatus.Success;
        }
        else
        {
            return TaskStatus.Running;
        }
    }
}
