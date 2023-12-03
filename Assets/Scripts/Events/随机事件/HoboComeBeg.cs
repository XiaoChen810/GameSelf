using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoboComeBeg : EventBase
{
    public float begGold;

    private float probabilityThreshold;     //  概率门槛
    [SerializeField] private float increment;   // 增量

    protected override void Awake()
    {
        base.Awake();
        probabilityThreshold = _probabilisticEvent.Probability;
    }

    public override void OnAccept()
    {
        // 花钱
        Statistics.Instance.Gold -= begGold;

        // 得好感,更多的人来
        GameManager.Instance.CreateCustomerTimeAdd(-3);

        base.OnAccept();
    }

    public override void OnCancel()
    {
        // 影响
        GameManager.Instance.CreateCustomerTimeAdd(2);

        base.OnCancel();
    }

    public override void OnReset()
    {
        // 如果今天没触发，则增加概率，触发了，则归回原位
        if (isShow)
        {
            _probabilisticEvent.Probability = probabilityThreshold;
            isShow = false;
        }
        else
        {
            _probabilisticEvent.Probability += increment;
        }

        todyProbabilityParameter = ProbabilityManager.Instance.GetANewProbabilityParameter();
    }
}
