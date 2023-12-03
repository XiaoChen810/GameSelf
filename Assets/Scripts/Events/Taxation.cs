using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taxation : EventBase
{
    public float taxation;
    public int 税收日;

    public override void OnAccept()
    {
        Statistics.Instance.Gold -= taxation;
        base.OnAccept();
    }

    public override void OnCancel()
    {
        _taxationNoPayNum++;
        base.OnCancel();
    }

    public override bool TriggerConditions()
    {
        if (isShow) return false;
        // 每21天一次税收
        if (Statistics.Instance.Date % 税收日 == 0 && Statistics.Instance.Date != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
