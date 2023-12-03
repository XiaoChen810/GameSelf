using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatHamburger : EventBase
{
    public int price;

    public override void OnAccept()
    {
        Statistics.Instance.Gold -= price;
        Statistics.Instance.Satiety = Statistics.Instance.SatietyMax;
        base.OnAccept();
    }

    public override void OnCancel()
    {
        base .OnCancel();
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
        if (Statistics.Instance.Gold >= price)
        {
            isShow = GameManager.Instance.ShowEvent();
        }
        else
        {
            isShow = GameManager.Instance.ShowEventButNoAccept();
        }


    }

    public override bool TriggerConditions()
    {
        if (isShow) return false;
        // ¶Ç×Ó¶öµÄÊ±ºò
        if (Statistics.Instance.Satiety <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
