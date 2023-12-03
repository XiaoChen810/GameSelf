using BehaviorDesigner.Runtime.Tasks;

using UnityEngine;

public class Action_Work : Action
{
    public Craftsman craftsman;
    public override TaskStatus OnUpdate()
    {
        craftsman.isWorking = true;
        Player.Instance.state = Player.State.None;
        if (craftsman.curOrder != null)
        {
            return TaskStatus.Running;
        }
        else
        {
            // 当前没有任务就结束
            return TaskStatus.Success;
        }
    }
}
