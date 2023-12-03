using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrestedOfTaxation : EventBase
{
    public override bool TriggerConditions()
    {
        if(_taxationNoPayNum >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
