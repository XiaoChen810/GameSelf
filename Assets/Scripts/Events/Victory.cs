using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : EventBase
{
    public float victoryPrice;

    public override void OnAccept()
    {
        base.OnAccept();
        EventManager.Instance.preEventDict.Add(name, this);
    }

    public override void OnCancel()
    {
        base.OnCancel();
        EventManager.Instance.preEventDict.Add(name, this);
    }

    public override bool TriggerConditions()
    {
        if(Statistics.Instance.Gold > victoryPrice)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
