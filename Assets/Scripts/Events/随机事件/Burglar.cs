using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burglar : EventBase
{
    public float stealGold;
    private float probabilityThreshold;     //  概率门槛
    [SerializeField] private float increment;   // 增量

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
        // 如果今天没触发，则增加概率，触发了，则归回原位
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
