using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;


    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private Animator anim;
    private Canvas canvas;
    private AudioSource audioSource;

    [Header("委托订单列表")]
    private List<Order> m_ordersList = new List<Order>();
    public int OrderMaxNum = 5;

    [Header("移动速度")]
    public float Speed;
    private float m_currentSpeed;
    [HideInInspector] public Vector2 m_targetPos;
    private Vector3 m_lastTransfrom;
    private float m_forcedDir;

    public enum State
    {
        None,
        Walk,
        ToCraftsman,
        ToMineCaves,
        ToCustomer,
        ToHouse,
        ToBed,
        ToDoor,
        SelectCustomer
    }
    [Header("状态")]
    public State state;

    [Header("表情")]
    public GameObject chatBubble;
    public Image chatImage;

    [Header("初始位置")]
    public Vector2 InitPosition;
    public Vector2 IndoorPosition;

    [Header("右键移动范围")]
    public float moveMinRange = -40;
    public float moveMaxRange = 2;

    // 是否在室内
    [HideInInspector] public bool isIndoor;
    // 是否开启强制方向转换
    private bool m_forcedDirOpen;   

    public delegate void OnReachThere();

    private void Awake()
    {
        Instance = this;

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;

        m_currentSpeed = Speed;
        m_targetPos = transform.position;
        m_lastTransfrom = transform.position;
        m_ordersList.Clear();
        state = State.None;
        chatBubble.SetActive(false);
    }

    private void Start()
    {
        Statistics.Instance.OnDayDark += DateDark;
    }

    private void DateDark()
    {
        // 位置变回初始位置
        transform.position = InitPosition;
        // 状态改成空
        state = State.None;
        // 重置目标位置
        m_targetPos = transform.position;
        m_lastTransfrom = transform.position;
        // 重置在室外
        isIndoor = false;
        // 强制面向右边
        UseForcedFlip(1,0.1f,State.None);
    }

    private void OnDestroy()
    {
        Statistics.Instance.OnDayDark -= DateDark;
    }

    private void Update()
    {
        RightClickMove();
        HandleTouchInput();

        // 饱食度不足时
        if (Statistics.Instance.Satiety <= 0)
        {
            m_currentSpeed = Speed / 2;
            anim.speed = 0.5f;
            audioSource.pitch = 0.5f;
        }
        else
        {
            m_currentSpeed = Speed;
            anim.speed = 1f;
            audioSource.pitch = 1f;
        }
    }



    private void FixedUpdate()
    {
        MoveToTarget(m_targetPos);

        if (m_forcedDirOpen)
        {
            Flip(m_forcedDir);
        }
        else
        {
            Flip();
        }
    }

    #region 手机

    private Vector2 touchStartPos; // 记录触摸开始位置
    private float lastTapTime; // 上一次点击的时间
    private float doubleTapTimeThreshold = 0.3f; // 双击时间间隔的阈值

    void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;

                    // 计算两次点击之间的时间间隔
                    float timeSinceLastTap = Time.time - lastTapTime;
                    lastTapTime = Time.time;

                    // 如果时间间隔小于阈值，认为是双击
                    if (timeSinceLastTap < doubleTapTimeThreshold)
                    {
                        state = State.Walk;
                        Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);
                        if (touchWorldPos.x > moveMinRange && touchWorldPos.x < moveMaxRange)
                            m_targetPos = touchWorldPos;
                    }
                    break;
            }
        }
    }

    #endregion

    #region Public

    /// <summary>
    /// 改变状态，和移动目标点，并调用一个协程，在状态未改变的情况下到达目标点，执行回调函数
    /// </summary>
    /// <param name="targetState"></param>      目标状态
    /// <param name="targetPostion"></param>    目标点
    /// <param name="callback"></param>         回调函数
    public void ToThereAndCallBack(State targetState, Vector2 targetPostion, OnReachThere callback)
    {
        state = targetState;
        m_targetPos = targetPostion;
        StartCoroutine(ToThereAndCallBackCo(targetState, targetPostion, callback));
    }

    IEnumerator ToThereAndCallBackCo(State targetState, Vector2 targetPostion, OnReachThere callback)
    {
        while (true)
        {
            yield return null;
            if(state == targetState)
            {
                float distance = Mathf.Abs(transform.position.x - targetPostion.x);
                if(distance < 0.1)
                {
                    callback();
                    yield break;
                }
            }
            else
            {
                yield break;
            }
        }
    }

    /// <summary>
    /// 强制转向一段时间，当状态发生改变时取消
    /// </summary>
    /// <param name="dir"></param>      方向,1为正。
    /// <param name="time"></param>     持续时间
    /// <param name="state"></param>    目标状态
    public void UseForcedFlip(float dir, float time, State state)
    {
        StartCoroutine(FlipCo(dir, time, state));
    }

    IEnumerator FlipCo(float dir, float time, State stateInF)
    {
        m_forcedDirOpen = true;
        m_forcedDir = dir;
        while (time > 0)
        {
            yield return null;
            time -= Time.deltaTime;
            if (state != stateInF)
            {
                m_forcedDirOpen = false;
                yield break;
            }
        }
        m_forcedDirOpen = false;
    }

    /// <summary>
    /// 有关订单类的公共函数
    /// CanAddOrder()    AddOrder(Order order)   RemoveOrder(Order order)    GetOrder()  FinishOrder(Order order)
    /// </summary>
    /// <returns></returns>
    #region Order

    public bool CanAddOrder(Order order)
    {
        if(Statistics.Instance.Iron - order.materialNum < 0)
        {
            return false;
        }
        return m_ordersList.Count < OrderMaxNum ? true : false;
    }

    public void AddOrder(Order order)
    {
        // 订单状态改变
        order.isSelect = true;
        order.isAccept = true;

        // 玩家数据改变
        Statistics.Instance.Iron -= order.materialNum;

        m_ordersList.Add(order);
        OrdersGroup.Instance.Add(order);
    }

    public void RemoveOrder(Order order)
    {
        m_ordersList.Remove(order);
    }

    public Order GetOrder()
    {
        if (m_ordersList.Count > 0)
        {
            foreach (var o in m_ordersList)
            {
                if (o.isFinish) continue;
                else
                {
                    return o;
                }
            }
        }
        return null;
    }

    public void FinishOrder(Order order)
    {
        order.isFinish = true;
        Statistics.Instance.Gold += order.price;
        m_ordersList.Remove(order);
    }

    #endregion

    /// <summary>
    /// 家
    /// </summary>

    public void EnterIndoor()
    {
        Statistics.Instance.anim.SetTrigger("SceneLoaded");
        transform.position = IndoorPosition;
        moveMinRange = -13.5f;
        moveMaxRange = -5.5f;
        isIndoor = true;
    }

    public void LeaveIndoor()
    {
        Statistics.Instance.anim.SetTrigger("SceneLoaded");
        transform.position = InitPosition;
        moveMinRange = -40f;
        moveMaxRange = 2f;
        isIndoor = false;
    }

    [HideInInspector] public bool isSleep;
    public void GoToSleep()
    {
        UseForcedFlip(1, 0.1f, State.ToBed);
        anim.SetBool("Sleeping", true);
        isSleep = true;
        StartCoroutine(WakeCo());
    }

    IEnumerator WakeCo()
    {
        while (true)
        {
            yield return null;
            if(state != State.ToBed)
            {
                anim.SetBool("Sleeping", false);
                isSleep = false;
                yield break;
            }
        }
    }


    #endregion

    #region Move

    private void RightClickMove()
    {
        // 右键点地板移动
        if (Input.GetMouseButtonDown(1))
        {
            state = State.Walk;
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mousePosition.x > moveMinRange && mousePosition.x < moveMaxRange)
                m_targetPos = mousePosition;
        }
    }

    private void Flip()
    {
        float dir = m_lastTransfrom.x - transform.position.x;
        if ( dir < 0 && Mathf.Abs(dir) > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
            m_lastTransfrom = transform.position;
        }
        if (dir > 0 && Mathf.Abs(dir) > 0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            m_lastTransfrom = transform.position;
        }

    }

    private void Flip(float dir)
    {
        m_lastTransfrom = transform.position;
        transform.localScale = new Vector3(dir, 1, 1);
    }



    private void MoveToTarget(Vector2 target)
    {
        if (Mathf.Abs(transform.position.x - target.x) > 0.1f)
        {
            // 移动
            anim.SetBool("Running", true);
            Vector2 dir = new Vector2(target.x - transform.position.x, 0);
            dir.Normalize();
            rb.velocity = new Vector2(dir.x * m_currentSpeed * Time.deltaTime, rb.velocity.y);

            // 检查音效是否正在播放
            if (!audioSource.isPlaying)
            {
                // 切换音效
                AudioClip run = Resources.Load<AudioClip>("Audio/人物在草地上跑");
                audioSource.clip = run;

                // 播放音效
                audioSource.Play();
            }

        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("Running", false);
            // 停止音效
            audioSource.Stop();
        }
    }

    #endregion

    #region Chat

    private void Chat()
    {
        if(!chatBubble.activeSelf)
        {
            chatBubble.SetActive(true);
            anim.SetBool("Chating", true);
        }
        else
        {
            chatBubble.SetActive(false);
            anim.SetBool("Chating", false);
        }

    }

    #endregion
}
