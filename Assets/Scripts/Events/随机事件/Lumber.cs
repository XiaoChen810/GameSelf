using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumber : EventBase
{
    public float need;
    public float reward; 
    

    public override bool TriggerConditions()
    {      
        if(Statistics.Instance.Gold < 200 && Statistics.Instance.Iron >= need)
        {
            return false;
        }
        return base.TriggerConditions();
    }

    public override void OnAccept()
    {
        Statistics.Instance.Wood += reward;
        Statistics.Instance.Iron -= need;
        base.OnAccept();
    }
}
