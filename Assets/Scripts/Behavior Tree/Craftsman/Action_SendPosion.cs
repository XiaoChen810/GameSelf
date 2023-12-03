using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace Core.Tree
{
    public class Action_SendPosion : Action
    {
        public Vector2 facePos;

        public override TaskStatus OnUpdate()
        {
            
            if (Player.Instance.state != Player.State.ToCraftsman)
            {
                Vector2 chatPos = new Vector2(transform.position.x + facePos.x, transform.position.y + facePos.y);
                Player.Instance.state = Player.State.ToCraftsman;
                Player.Instance.m_targetPos = chatPos;
                return TaskStatus.Success;
            }
            return TaskStatus.Running;
        }
    }
}