using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoboComeBeg : EventBase
{
    public float begGold;

    private float probabilityThreshold;     //  �����ż�
    [SerializeField] private float increment;   // ����

    protected override void Awake()
    {
        base.Awake();
        probabilityThreshold = _probabilisticEvent.Probability;
    }

    public override void OnAccept()
    {
        // ��Ǯ
        Statistics.Instance.Gold -= begGold;

        // �úø�,���������
        GameManager.Instance.CreateCustomerTimeAdd(-3);

        base.OnAccept();
    }

    public override void OnCancel()
    {
        // Ӱ��
        GameManager.Instance.CreateCustomerTimeAdd(2);

        base.OnCancel();
    }

    public override void OnReset()
    {
        // �������û�����������Ӹ��ʣ������ˣ�����ԭλ
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
