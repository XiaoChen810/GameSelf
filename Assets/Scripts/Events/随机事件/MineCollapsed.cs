using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineCollapsed : EventBase
{
    [Header("矿洞")]
    public MineCaves mineCaves;
    [Header("维修时间")]
    public int fixedTime = 2;
    private int _fixedTime;

    protected override void Awake()
    {
        base.Awake();
        mineCaves = GameObject.Find("矿洞").GetComponent<MineCaves>();
    }

    public override bool TriggerConditions()
    {
        // 在玩家挖矿时可能触发
        if(!mineCaves.isWorking)
        {
            return false;
        }

        return base.TriggerConditions();
    }

    public override void OnAccept()
    {
        mineCaves.canWork = false;
        _fixedTime = fixedTime;
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
        base.OnReset();
        _fixedTime -= 1;
        if(_fixedTime <= 0)
        {
            mineCaves.canWork = true;
        }
    }

}
