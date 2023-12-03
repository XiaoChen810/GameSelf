using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Sleep : EventBase
{
    public override bool TriggerConditions()
    {
        // 当体力不足时，进入下一天
        if (Statistics.Instance.Energy <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
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

    public override void OnAccept()
    {
        // 恢复体力
        Statistics.Instance.Energy = Statistics.Instance.EnergyMax;
        // 强行进入下一天
        Statistics.Instance.ToNextDate();
        base.OnAccept();
    }
}
