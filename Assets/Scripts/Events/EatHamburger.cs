using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatHamburger : EventBase
{
    public int price;

    public override void OnAccept()
    {
        Statistics.Instance.Gold -= price;
        Statistics.Instance.Satiety = Statistics.Instance.SatietyMax;
        base.OnAccept();
    }

    public override void OnCancel()
    {
        base .OnCancel();
    }

    public override bool TriggerConditions()
    {
        if (isShow) return false;
        // ¶Ç×Ó¶öµÄÊ±ºò
        if (Statistics.Instance.Satiety <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
