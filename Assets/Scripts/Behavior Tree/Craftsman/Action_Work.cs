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
            // ��ǰû������ͽ���
            return TaskStatus.Success;
        }
    }
}
