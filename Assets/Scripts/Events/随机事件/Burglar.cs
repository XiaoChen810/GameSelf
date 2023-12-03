using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burglar : EventBase
{
    public float stealGold;
    private float probabilityThreshold;     //  �����ż�
    [SerializeField] private float increment;   // ����

    protected override void Awake()
    {
        base.Awake();
        probabilityThreshold = _probabilisticEvent.Probability;
    }

    public override void OnAccept()
    {
        Statistics.Instance.Gold -= stealGold;

        base.OnAccept();
    }

    public override void OnShow()
    {
        if (GameManager.Instance.EventPanel.activeSelf)
        {
            isShow = false;
            return;
        }
        GameManager.Instance.HeaderText.text = _headerName;
        GameManager.Instance.DescriptionText.text = _description;
        GameManager.Instance.AcceptBtn.onClick.RemoveAllListeners();
        GameManager.Instance.AcceptBtn.onClick.AddListener(OnAccept);
        GameManager.Instance.CancelBtn.onClick.RemoveAllListeners();
        GameManager.Instance.CancelBtn.onClick.AddListener(OnCancel);

        isShow = GameManager.Instance.ShowEventButNoCancel();
    }

    public override void OnReset()
    {
        // �������û�����������Ӹ��ʣ������ˣ�����ԭλ
        if(isShow)
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
