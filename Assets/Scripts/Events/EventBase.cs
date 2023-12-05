using AYellowpaper.SerializedCollections;
using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[System.Serializable]
public abstract class EventBase : MonoBehaviour,IEvent
{
    [SerializeField][Multiline(5)] protected string _headerName, _description;

    [Header("如果是概率事件")]
    [SerializeField] protected ProbabilisticEvent _probabilisticEvent;
    protected float todyProbabilityParameter;

    [Header("是否有新手保护期")]
    [SerializeField] protected bool isHasProtective;
    [SerializeField] protected int deadline = 3;

    [Header("是否只触发一次")]
    [SerializeField] protected bool isOnlyOneTrigger;

    [Header("前置事件名称")]
    [SerializeField] protected string _preEventName;

    public bool isShow = false;

    protected AudioSource audioSource;
    protected AudioClip BtnClip;

    // 欠税次数
    protected float _taxationNoPayNum = 0;


    public string HeaderName { get { return _headerName; } }

    public string Description { get { return _description; } }

    /// <summary>
    /// 当按下确认按钮和取消按钮
    /// </summary>
    public virtual void OnAccept()
    {
        audioSource.PlayOneShot(BtnClip);
        GameManager.Instance.CloseEvent();
    }

    public virtual void OnCancel()
    {
        audioSource.PlayOneShot(BtnClip);
        GameManager.Instance.CloseEvent();
    }

    /// <summary>
    /// 在展示出来时的代码
    /// GameManager.Instance.ShowEvent()函数会返回是否成功的值传给isShow
    /// </summary>
    public virtual void OnShow()
    {
        isShow = GameManager.Instance.TryShowEvent();
        if (isShow)
        {
            GameManager.Instance.HeaderText.text = _headerName;
            GameManager.Instance.DescriptionText.text = _description;
            GameManager.Instance.AcceptBtn.onClick.RemoveAllListeners();
            GameManager.Instance.AcceptBtn.onClick.AddListener(OnAccept);
            GameManager.Instance.CancelBtn.onClick.RemoveAllListeners();
            GameManager.Instance.CancelBtn.onClick.AddListener(OnCancel);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// 判断触发逻辑
    /// </summary>
    /// <returns></returns>
    public virtual bool TriggerConditions()
    {
        if(_probabilisticEvent.IsProbabilistic)
        {
            if (_probabilisticEvent.Probability > todyProbabilityParameter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        Debug.LogWarning("触发了错误，导致事件判断返回False");
        return false;
    }

    /// <summary>
    /// 当重置时
    /// </summary>
    public virtual void OnReset()
    {
        if(!isOnlyOneTrigger)
        {
            isShow = false;
            todyProbabilityParameter = ProbabilityManager.Instance.GetANewProbabilityParameter();
        }
    }



    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        BtnClip = Resources.Load<AudioClip>("Audio/点击按钮音效");

        _probabilisticEvent.Name = this.gameObject.name;
        todyProbabilityParameter = 100f;
    }

    protected void OnEnable()
    {
        Statistics.Instance.OnDateUpdate += OnReset;
    }

    protected void OnDestroy()
    {
        Statistics.Instance.OnDateUpdate -= OnReset;
    }

    protected void Update()
    {
        // 保护期限
        if (isHasProtective)
        {
            if(Statistics.Instance.Date < deadline)
            {
                return;
            }
        }

        // 打开时不判断，更新时不判断
        if (!isShow && !Statistics.Instance.isUpdating)
        {
            if (TriggerConditions())
            {
                isShow = true;      // 预防针
                OnShow();
            }
        }

    }
}
