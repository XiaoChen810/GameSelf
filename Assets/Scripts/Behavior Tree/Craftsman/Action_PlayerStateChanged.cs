using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class Action_PlayerStateChanged : Action
{
    public Craftsman craftsman;

    public override void OnAwake()
    {
        craftsman = GetComponent<Craftsman>();
    }

    public override TaskStatus OnUpdate()
    {
        if(craftsman.isWorking)
        {
            if (Player.Instance.state != Player.State.ToCraftsman)
            {
                // 当已经开始工作时，角色的目标可以变动,仅取消选中
                craftsman.isSelected = false;
                craftsman.highLight.EndHighLight();
            }
            return TaskStatus.Success;
        }
        else
        {
            if (Player.Instance.state != Player.State.ToCraftsman)
            {
                // 当未开始工作时，角色的目标不可以变动，返回Failure
                craftsman.isSelected = false;
                craftsman.highLight.EndHighLight();
                craftsman.EndChat();
                return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Running;
            }
        }
    }
}