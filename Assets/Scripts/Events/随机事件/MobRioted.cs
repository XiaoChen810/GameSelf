using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRioted : EventBase
{
    [Header("支付金额")]
    public float price;
    [Header("客流量减少程度")]
    public float amount;
    [Header("持续时间")]
    public int continuallyTime = 2;
    private int _continuallyTime = 0;
    private int _num = 0;

    public override void OnAccept()
    {
        Statistics.Instance.Gold -= price;

        base.OnAccept();
    }

    public override void OnCancel()
    {
        // 客流量减少
        GameManager.Instance.CreateCustomerTimeAdd(amount);
        _num += 1;
        _continuallyTime += continuallyTime;
        base.OnCancel();
    }

    public override void OnReset()
    {
        base.OnReset();
        if(_num > 0)
        {
            _continuallyTime -= 1;
            if (_continuallyTime - ((_num - 1) * continuallyTime) <= 0)
            {
                GameManager.Instance.CreateCustomerTimeAdd(-amount);
                _num -= 1;
            }
        }
    }
}
