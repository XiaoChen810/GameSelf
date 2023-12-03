using BehaviorDesigner.Runtime.Tasks.Unity.UnityAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventurerLove60 : EventBase
{
    [Header("½±Àø")]
    public float reward = 50;

    public override bool TriggerConditions()
    {
        if(Statistics.Instance.LoveAdventurer >= 60)
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
        Statistics.Instance.Gold += reward;

        base.OnAccept();
    }
}
