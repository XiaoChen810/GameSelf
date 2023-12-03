using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insolvency : EventBase
{
    [Header("ÆÆ²úÏß")]
    [Range(-500, -100)] public float BankruptLine;

    public override bool TriggerConditions()
    {
        float property = Statistics.Instance.Gold + (Statistics.Instance.Brics * 50);
        if (property < BankruptLine)
        {
            Debug.Log(property + "\t" + BankruptLine + "\t" + (property < BankruptLine));
            return true;
        }
        else
        {
            return false;
        }
    }
}
