using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : MonoBehaviour
{
    public Vector2 doorPosition;
    public bool isSelected;

    private void OnMouseDown()
    {
        isSelected = true;
        if (Player.Instance.state != Player.State.ToBed)
        {
            Player.Instance.state = Player.State.ToBed;
            Player.OnReachThere onReachThere = new Player.OnReachThere(Player.Instance.GoToSleep);
            Player.Instance.ToThereAndCallBack(Player.State.ToBed, doorPosition, onReachThere);
        }
    }
}
