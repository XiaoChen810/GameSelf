using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Statistics : MonoBehaviour
{
    public static Statistics Instance;


    public event Action OnDateUpdate;
    public event Action<float,float> OnMoning;
    public event Action<float, float> OnNoon;
    public event Action<float, float> OnAfternoon;
    public event Action<float, float> OnNight;
    public event Action OnDayDark;

    public event Action<float> OnGoldUpdate;

    public Animator anim;

    [Header("基本数据面板")]
    [SerializeField] private StatisticsPanel statisticsPanel;
    [Header("金币")]
    [SerializeField] private float _gold;
    public float Gold
    {
        get
        {
            return _gold;
        }
        set
        {
            if(value > _gold)
            {
                // 先加载好数据之后，才加载audioSource，才会触发
                if (audioSource != null)
                    audioSource.PlayOneShot(GoldClip);
            }
            _gold = value;
            statisticsPanel.Gold.text = _gold.ToString();
            OnGoldUpdate?.Invoke(_gold);
        }

    }

    [Header("木头")]
    [SerializeField] private float _wood;
    public float Wood
    {
        get
        {
            return _wood;
        }
        set
        {
            _wood = value;
            statisticsPanel.Wood.text = _wood.ToString();
        }
    }

    [Header("铁")]
    [SerializeField] private float _iron;
    public float Iron
    {
        get
        {
            return _iron;
        }
        set
        {
            _iron = value;
            statisticsPanel.Iron.text = _iron.ToString();
        }
    }

    [Header("金砖")]
    [SerializeField] private float _brics;
    public float Brics
    {
        get
        {
            return _brics;
        }
        set
        {
            _brics  = value;
            statisticsPanel.Brics.text = _brics.ToString();
        }
    }

    [Header("饱食度")]
    [SerializeField] private float _satiety;
    public float Satiety
    {
        get
        {
            return _satiety;
        }
        set
        {
            _satiety = value;
            statisticsPanel.Satiety.value = _satiety / SatietyMax;
        }
    }
    public float SatietyMax;

    [Header("精力")]
    [SerializeField] private float _energy;
    public float Energy
    {
        get { return _energy; }
        set
        {
            _energy = value;
            statisticsPanel.Energy.value = _energy / EnergyMax;
        }
    }
    public float EnergyMax;

    [Header("天数")]
    [SerializeField] private float _date;
    public float Date
    {
        get
        {
            return _date;
        }
        set
        {
            _date = value;
            statisticsPanel.Date.text = $"Date: {Date}";
        }
    }
    private float LastDate;
    [Min(10)]public float TimeScale = 60;
    // 6~14~19~24~6
    // 0~8~13~18~24
    public float Second;
    private float Moning;
    private float Noon;
    private float AfterNoon;
    private float Nightfall;
    private float Night;
    [HideInInspector] public bool isUpdating;
    [SerializeField] private float ToNextDateDelayTime = 5;
    [SerializeField] private bool isClickToNextDate;
    public enum DatePeriod
    {
        None, Moning, Noon, AfterNoon, Nightfall, Night
    }
    [SerializeField] DatePeriod datePeriod = DatePeriod.None;

    // 音效
    private AudioSource audioSource;
    private AudioClip GoldClip;
    private AudioClip NextDayClip;

    [Header("各派系好感度")]
    [Range(0, 100f)] public float LoveResidents = 50f;
    [Range(0, 100f)] public float LoveMonster = 50f;
    [Range(0, 100f)] public float LoveAdventurer = 50f;

    private void Awake()
    {
        Instance = this;

        LoadData();

        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        LastDate = Date;

        Moning = 0;
        Noon = TimeScale / 24 * 8;
        AfterNoon = TimeScale / 24 * 13;
        Nightfall = TimeScale / 24 * 18;
        Night = TimeScale;

        Second = Moning;

        GoldClip = Resources.Load<AudioClip>("Audio/收获金币音效");
        NextDayClip = Resources.Load<AudioClip>("Audio/下一天音效");
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    private void Update()
    {
        DateUpdate();
        SatietyUpdate();
        EnergyUpdate();
    }

    public void Love(Customer.Faction faction,float add)
    {
        switch (faction)
        {
            case Customer.Faction.人类:
                {
                    LoveResidents += add;
                    break;
                }
            case Customer.Faction.怪物:
                {
                    LoveMonster += add;
                    break;
                }
            case Customer.Faction.冒险者:
                {
                    LoveAdventurer += add;
                    break;
                }
        }
    }



    #region Save And Load

    private void SaveData()
    {
        PlayerPrefs.SetFloat("Gold", Gold);
        PlayerPrefs.SetFloat("Wooden", Wood);
        PlayerPrefs.SetFloat("Iron", Iron);
        PlayerPrefs.SetFloat("Brics", Brics);
        PlayerPrefs.SetFloat("Satiety", Satiety);
        PlayerPrefs.SetFloat("Energy",Energy);
        PlayerPrefs.SetFloat("Date", Date);
    }

    private void LoadData()
    {
        Gold = PlayerPrefs.GetFloat("Gold",0);
        Wood = PlayerPrefs.GetFloat("Wooden",0);
        Iron = PlayerPrefs.GetFloat("Iron",10);
        Brics = PlayerPrefs.GetFloat("Brics", 0);
        Satiety = PlayerPrefs.GetFloat("Satiety",SatietyMax);
        Energy = PlayerPrefs.GetFloat("Energy",EnergyMax);
        Date = PlayerPrefs.GetFloat("Date", 0);
    }

#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.quitting += SaveDataOnEditorQuit;
    }

    private void OnDisable()
    {
        EditorApplication.quitting -= SaveDataOnEditorQuit;
    }

    private void SaveDataOnEditorQuit()
    {
        SaveData();
    }
#endif

    #endregion

    #region Date

    //IEnumerator DateCo()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        if (!isUpdating)
    //        {
    //            Second += Time.deltaTime;

    //            // 新的一天，清晨到正午
    //            if (Second >= Moning && Second < Noon && datePeriod != DatePeriod.Moning)
    //            {
    //                datePeriod = DatePeriod.Moning;
    //                OnMoning?.Invoke(Moning, Noon);
    //            }
    //            // 正午到下午
    //            if (Second >= Noon && Second < AfterNoon && datePeriod != DatePeriod.Noon)
    //            {
    //                datePeriod = DatePeriod.Noon;
    //                OnNoon?.Invoke(Noon, AfterNoon);
    //            }
    //            // 下午到傍晚
    //            if (Second >= AfterNoon && Second < Nightfall && datePeriod != DatePeriod.AfterNoon)
    //            {
    //                datePeriod = DatePeriod.AfterNoon;
    //                OnAfternoon?.Invoke(AfterNoon, Nightfall);
    //            }
    //            // 傍晚到晚上
    //            if (Second >= Nightfall && Second < Night && datePeriod != DatePeriod.Nightfall)
    //            {
    //                datePeriod = DatePeriod.Nightfall;
    //                OnNight?.Invoke(Nightfall, Night);
    //            }
    //            if (Second >= Night && datePeriod != DatePeriod.Night)
    //            {
    //                datePeriod = DatePeriod.Night;
    //                StartCoroutine(DateUpdateMoningCo());
    //            }

    //            // 保险丝
    //            if (LastDate != Date)
    //            {
    //                Debug.LogWarning("LastDate != Date");
    //                LastDate = Date;
    //                anim.SetTrigger("Fade");
    //                // 通知人物天黑了要重置了
    //                OnDayDark?.Invoke();

    //                yield return new WaitForSecondsRealtime(3f);

    //                // 解锁
    //                isUpdating = false;
    //                // 通知各种事件日期更新了
    //                OnDateUpdate?.Invoke();
    //            }
    //        }
    //    }
    //}

    private void DateUpdate()
    {
        if (!isUpdating)
        {
            Second += Time.deltaTime;

            // 新的一天，清晨到正午
            if (Second >= Moning && Second < Noon && datePeriod != DatePeriod.Moning)
            {
                datePeriod = DatePeriod.Moning;
                OnMoning?.Invoke(Moning, Noon);
            }
            // 正午到下午
            if (Second >= Noon && Second < AfterNoon && datePeriod != DatePeriod.Noon)
            {
                datePeriod = DatePeriod.Noon;
                OnNoon?.Invoke(Noon, AfterNoon);
            }
            // 下午到傍晚
            if (Second >= AfterNoon && Second < Nightfall && datePeriod != DatePeriod.AfterNoon)
            {
                datePeriod = DatePeriod.AfterNoon;
                OnAfternoon?.Invoke(AfterNoon, Nightfall);
            }
            // 傍晚到晚上
            if (Second >= Nightfall && Second < Night && datePeriod != DatePeriod.Nightfall)
            {
                datePeriod = DatePeriod.Nightfall;
                OnNight?.Invoke(Nightfall, Night);
            }
            if (Second >= Night && datePeriod != DatePeriod.Night)
            {
                datePeriod = DatePeriod.Night;
                StartCoroutine(DateUpdateMoningCo());
            }

            // 保险丝
            if (LastDate != Date)
            {
                Debug.LogWarning("LastDate != Date");
            }
        }
    }

    IEnumerator DateUpdateMoningCo()
    {
        audioSource.PlayOneShot(NextDayClip);

        // 上锁
        isUpdating = true;
        Second = Moning;
        Date++;

        LastDate = Date;
        anim.SetTrigger("Fade");
        yield return new WaitForSecondsRealtime(1.001f);
        // 通知人物天黑了要重置了
        OnDayDark?.Invoke();

        yield return new WaitForSecondsRealtime(2.001f);

        // 解锁
        isUpdating = false;
        // 通知各种事件日期更新了
        OnDateUpdate?.Invoke();
    }


    public void ToNextDate()
    {
        if (!isUpdating && !isClickToNextDate)
        {
            StartCoroutine(ToNextDateDelayCo());
            StartCoroutine(DateUpdateMoningCo());
            OnMoning?.Invoke(Moning, Noon);
        }
    }

    IEnumerator ToNextDateDelayCo()
    {
        isClickToNextDate = true;
        yield return new WaitForSeconds(ToNextDateDelayTime);
        isClickToNextDate = false;
    }

    #endregion

    #region 饱食度和精力

    private void SatietyUpdate()
    {
        if (Satiety > 0)
        {
            Satiety -= Time.deltaTime;
        }
        else
        {
            Satiety = 0;
        }
    }

    private void EnergyUpdate()
    {
        if(Energy < EnergyMax)
        {
            if (Player.Instance.isSleep)
            {
                Energy += Time.deltaTime;
                Time.timeScale = 2;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        else
        {
            Energy = EnergyMax;
            Time.timeScale = 1;
        }
    }

    #endregion


    public void Restart()
    {
        Gold = 0;

        Wood = 0;

        Iron = 10;

        Brics = 0;

        Satiety = SatietyMax;

        Energy = EnergyMax;

        LastDate = Date = Second = 0;

        datePeriod = DatePeriod.None;

        SaveData();
    }

    public void Cost(float goldNum)
    {
        Gold -= goldNum;
    }
}
