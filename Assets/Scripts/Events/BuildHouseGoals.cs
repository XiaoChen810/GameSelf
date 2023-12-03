using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHouseGoals : EventBase
{
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
        if(EventManager.Instance.preEventDict.Count > 0 && EventManager.Instance.preEventDict.ContainsKey(_preEventName))
        {
            if (EventManager.Instance.preEventDict[_preEventName].isShow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
