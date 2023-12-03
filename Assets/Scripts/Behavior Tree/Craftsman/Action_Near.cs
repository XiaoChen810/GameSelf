using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Action_Near : Action
{
    public Craftsman craftsman;
    public Vector2 facePos;

    public override void OnAwake()
    {
        craftsman = GetComponent<Craftsman>();  
    }

    public override TaskStatus OnUpdate()
    {
        Vector2 chatPos = new Vector2(transform.position.x + facePos.x, transform.position.y + facePos.y);
        float distance = Mathf.Abs(Player.Instance.transform.position.x - chatPos.x);

        if (distance < 0.1f)
        {
            return TaskStatus.Success;
        }
        if(Player.Instance.state != Player.State.ToCraftsman)
        {
            craftsman.isSelected = false;
            craftsman.highLight.EndHighLight();
            return TaskStatus.Failure;
        }
        return TaskStatus.Running;
    }
}
