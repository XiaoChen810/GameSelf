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

    [Header("����Ǹ����¼�")]
    [SerializeField] protected ProbabilisticEvent _probabilisticEvent;
    protected float todyProbabilityParameter;

    [Header("�Ƿ������ֱ�����")]
    [SerializeField] protected bool isHasProtective;
    [SerializeField] protected int deadline = 3;

    [Header("�Ƿ�ֻ����һ��")]
    [SerializeField] protected bool isOnlyOneTrigger;

    [Header("ǰ���¼�����")]
    [SerializeField] protected string _preEventName;

    public bool isShow = false;

    protected AudioSource audioSource;
    protected AudioClip BtnClip;

    // Ƿ˰����
    protected float _taxationNoPayNum = 0;


    public string HeaderName { get { return _headerName; } }

    public string Description { get { return _description; } }

    /// <summary>
    /// ������ȷ�ϰ�ť��ȡ����ť
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
    /// ��չʾ����ʱ�Ĵ���
    /// GameManager.Instance.ShowEvent()�����᷵���Ƿ�ɹ���ֵ����isShow
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
    /// �жϴ����߼�
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

        Debug.LogWarning("�����˴��󣬵����¼��жϷ���False");
        return false;
    }

    /// <summary>
    /// ������ʱ
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
        BtnClip = Resources.Load<AudioClip>("Audio/�����ť��Ч");

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
        // ��������
        if (isHasProtective)
        {
            if(Statistics.Instance.Date < deadline)
            {
                return;
            }
        }

        // ��ʱ���жϣ�����ʱ���ж�
        if (!isShow && !Statistics.Instance.isUpdating)
        {
            if (TriggerConditions())
            {
                isShow = true;      // Ԥ����
                OnShow();
            }
        }

    }
}
