using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildHouse : EventBase
{
    public override bool TriggerConditions()
    {
        if (EventManager.Instance.preEventDict.Count > 0 && EventManager.Instance.preEventDict.ContainsKey(_preEventName))
        {
            if (EventManager.Instance.preEventDict[_preEventName].isShow)
            {
                if (Statistics.Instance.Wood >= 100)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
