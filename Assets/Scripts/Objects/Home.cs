using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    public Vector2 doorPosition;
    public bool isSelected;

    private void OnMouseDown()
    {
        isSelected = true;
        if (Player.Instance.state != Player.State.ToHouse)
        {
            Player.Instance.state = Player.State.ToHouse;
            Player.OnReachThere onReachThere = new Player.OnReachThere(Player.Instance.EnterIndoor);
            Player.Instance.ToThereAndCallBack(Player.State.ToHouse, doorPosition,onReachThere);
        }       
    }


}
