using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Chat : Action
{
    public Craftsman craftsman;

    public override TaskStatus OnUpdate()
    {
        craftsman.StartChat(1.5f);
        Player.Instance.UseForcedFlip(-1, 2, Player.State.ToCraftsman);
        return TaskStatus.Success;
    }
}
