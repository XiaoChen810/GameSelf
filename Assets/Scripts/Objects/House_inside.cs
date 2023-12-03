using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House_inside : MonoBehaviour
{
    public Vector2 doorPosition;
    public bool isSelected;

    private void OnMouseDown()
    {
        isSelected = true;
        if (Player.Instance.state != Player.State.ToDoor)
        {
            Player.Instance.state = Player.State.ToDoor;
            Player.OnReachThere onReachThere = new Player.OnReachThere(Player.Instance.LeaveIndoor);
            Player.Instance.ToThereAndCallBack(Player.State.ToDoor, doorPosition, onReachThere);
        }
    }
}
