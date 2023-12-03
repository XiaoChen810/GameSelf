using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Sleep : EventBase
{
    public override bool TriggerConditions()
    {
        // ����������ʱ��������һ��
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
        // �ָ�����
        Statistics.Instance.Energy = Statistics.Instance.EnergyMax;
        // ǿ�н�����һ��
        Statistics.Instance.ToNextDate();
        base.OnAccept();
    }
}
