using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobRioted : EventBase
{
    [Header("֧�����")]
    public float price;
    [Header("���������ٳ̶�")]
    public float amount;
    [Header("����ʱ��")]
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
        // ����������
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
