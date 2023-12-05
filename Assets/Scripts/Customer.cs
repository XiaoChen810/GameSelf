using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class Order
{
    [Header("价格")]
    public float price;
    [Header("所需材料")]
    public float materialNum;
    [Header("图标")]
    public Sprite icon;
    [Header("已选择")]
    public bool isSelect;
    [Header("已接受")]
    public bool isAccept;
    [Header("已完成")]
    public bool isFinish;
    [Header("已开始")]
    public bool isStart;
    [Header("已超时")]
    public bool isOutTime;
    [Header("进度")]
    public float progress = 1;
    [Header("耐心")]
    public float patience = 1;
}


public class Customer : MonoBehaviour
{
    [SerializeField] private Transform body;
    private Rigidbody2D rb;
    private CapsuleCollider2D coll;
    private Animator anim;
    private Canvas canvas;

    [Header("UI")]
    public Text PriceText;
    public Text MaterialNumText;
    public Button AcceptBtn;
    public Button CancelBtn;

    public enum Faction
    {
        人类, 怪物, 冒险者
    }
    [Header("阵营")]
    public Faction myFaction;

    [Header("订单")]
    public Order m_order;
    public GameObject MyOrderPanel;
    public Image orderContentImage;
    public List<Sprite> orderContentSprites;

    [Header("移动速度")]
    public float speed;
    public Vector2 targetPos;
    private Vector3 lastTransfrom;

    [Header("按钮抖动")]
    public float shakeDuration = 0.2f;
    public float shakeAmount = 0.1f;

    [Header("表情")]
    public GameObject chatBubble;
    public Image chatImage;
    public List<Sprite> chatImages;

    [Header("耐心")]
    public Slider PatienceSlider;
    public Image PatienceSliderFillImage;
    public float patienceMax = 20;
    public float patience;
    public Color green;
    public Color red;
    
    [Header("被选中")]
    public bool isSelected;
    private Transform QueuingPoint;
    public SpriteRenderer OutlineRenderer;
    public HighLight highLight;

    [Header("OK")]
    public GameObject OKImage;

    public Coroutine chatCoroutine;

    private void Start()
    {
        // 组件调用
        rb = GetComponentInChildren<Rigidbody2D>();
        coll = GetComponentInChildren<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();
        canvas = GetComponentInChildren<Canvas>();

        // UI联系
        PriceText.text = m_order.price.ToString();
        MaterialNumText.text = m_order.materialNum.ToString();
        AcceptBtn.onClick.AddListener(AcceptTask);
        CancelBtn.onClick.AddListener(CancelTask);
        canvas.worldCamera = Camera.main;

        // 位置坐标初始化
        targetPos = transform.position;
        lastTransfrom = transform.position;
        QueuingPoint = GameObject.Find("排队点").GetComponent<Transform>();

        // UI初始化
        chatBubble.SetActive(false);
        MyOrderPanel.SetActive(false);

        // 开启耐心值协程
        patience = patienceMax;
        // StartCoroutine(PatientCo());
        
        // 订阅事件
        Statistics.Instance.OnDayDark += DateDark;
    }

    private void OnEnable()
    {
        // 初始化订单参数
        if(myFaction == Faction.人类)
        {
            m_order.price = Random.Range(GameManager.Instance.minPrice, GameManager.Instance.maxPrice + 1);
            m_order.materialNum = Random.Range(GameManager.Instance.minMaterialNum, GameManager.Instance.maxMaterialNum + 1);
            m_order.icon = orderContentSprites[Random.Range(0, orderContentSprites.Count)];
            orderContentImage.sprite = m_order.icon;

        }
        else if(myFaction == Faction.怪物)
        {
            // 金额少，材料也少
            m_order.price = Random.Range(GameManager.Instance.minPrice - 5, GameManager.Instance.maxPrice + 1 - 5);
            m_order.materialNum = Random.Range(GameManager.Instance.minMaterialNum - 2, GameManager.Instance.maxMaterialNum + 1 - 2);
            m_order.icon = orderContentSprites[Random.Range(0, orderContentSprites.Count)];
            orderContentImage.sprite = m_order.icon;
        }
        else if(myFaction == Faction.冒险者)
        {
            // 金额多，材料也多
            m_order.price = Random.Range(GameManager.Instance.minPrice + 5, GameManager.Instance.maxPrice + 1 + 5);
            m_order.materialNum = Random.Range(GameManager.Instance.minMaterialNum + 2, GameManager.Instance.maxMaterialNum + 1 + 2);
            m_order.icon = orderContentSprites[Random.Range(0, orderContentSprites.Count)];
            orderContentImage.sprite = m_order.icon;
        }
        // 校验
        m_order.price = m_order.price <= 0 ? 1 : m_order.price;
        m_order.materialNum = m_order.materialNum <= 0 ? 1 : m_order.materialNum;
    }

    private void DateDark()
    {
        if(!m_order.isAccept)
        {
            GameManager.Instance.LeaveDoor(targetPos);      // 在离开前把位置还回去
            Destroy(gameObject, 1f);
        }
    }

    private void OnDestroy()
    {
        Statistics.Instance.OnDayDark -= DateDark;
    }

    #region Update

    private void Update()
    {
        Selected();

        // UI更新
        //// 聊天气泡的方向
        //chatBubble.transform.localScale = new Vector3(
        //    Mathf.Abs(chatBubble.transform.localScale.x) * body.localScale.x,
        //    chatBubble.transform.localScale.y,
        //    chatBubble.transform.localScale.z
        //    );

        if (isClickAccept && Player.Instance.state != Player.State.ToCustomer)
        {
            isClickAccept = false;
        }

        PatienceUpdate();
    }

    private void PatienceUpdate()
    {
        // 在非奔跑状态，即等待玩家过来时的状态，耐心值会减
        // 再订单完成后，不会减少
        if (!anim.GetBool("Running") && !m_order.isFinish)
        {
            patience -= Time.deltaTime;
            // 耐心条更新
            PatienceSlider.value = patience / patienceMax;
            // 订单的耐心也更新
            m_order.patience = patience / patienceMax;
        }

        PatienceSliderFillImage.color = patience / patienceMax <= 0.3f ? red : green;

        if (patience < 0)
        {
            MyOrderPanel.SetActive(false);
        }
    }

    private void Selected()
    {
        // 当点击
        if (isSelected)
        {
            // 当订单未被选择时,当停下来时，显示订单
            if (!m_order.isSelect && !anim.GetBool("Running"))
            {
                MyOrderPanel.SetActive(true);
            }

            // 当玩家没有在选择状态
            if (Player.Instance.state != Player.State.SelectCustomer)
            {
                isSelected = false;
                highLight.EndHighLight();
                MyOrderPanel.SetActive(false);
                // Debug.Log(MyOrderPanel.activeSelf);
            }
        }
    }

    #endregion

    #region FixedUpdate

    private void FixedUpdate()
    {
        MoveToTarget(targetPos);

        Flip();
    }

    private void Flip()
    {
        float dir = lastTransfrom.x - body.transform.position.x;
        if (dir < 0 && Mathf.Abs(dir) > 0.1f)
        {
            body.transform.localScale = new Vector3(1, 1, 1);
            lastTransfrom = body.transform.position;
        }
        if (dir > 0 && Mathf.Abs(dir) > 0.1f)
        {
            body.transform.localScale = new Vector3(-1, 1, 1);
            lastTransfrom = body.transform.position;
        }
    }

    private void MoveToTarget(Vector2 target)
    {
        float distance = Mathf.Abs(transform.position.x - target.x);
        if (distance > 0.1f)
        {
            anim.SetBool("Running", true);
            Vector2 dir = new Vector2(target.x - transform.position.x, 0);
            dir.Normalize();
            rb.velocity = new Vector2(dir.x * speed * Time.deltaTime, rb.velocity.y);
            // 移动的时候不会说话
            EndChat();
        }
        else
        {
            //Debug.Log("距离目标点位置: " + distance + "我的位置" + transform.position.x + "他的位置" + target.x);
            anim.SetBool("Running", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    #endregion

    private void OnMouseDown()
    {
        isSelected = true;
        highLight.StartHighLight();
        Player.Instance.state = Player.State.SelectCustomer;
    }

    #region Button

    private bool isClickAccept;
    public void AcceptTask()
    {
        if (isClickAccept)
        {
            // 不可重复点击
            return;
        }
        else
        {
            isClickAccept = true;
            // 当订单材料数可以支付，且可以添加订单时
            if (Player.Instance.CanAddOrder(m_order))
            {
                // Player调用相应函数，移动到点,添加订单。
                Player.OnReachThere onReachThere = new Player.OnReachThere(AcceptOrder);
                Player.Instance.ToThereAndCallBack(Player.State.ToCustomer, QueuingPoint.position, onReachThere);

            }
            else
            {
                // Debug.Log("无法接受任务");
                // 抖动
                StartCoroutine(ShakeButton(AcceptBtn));
            }
        }
    }
    private void AcceptOrder()
    {
        // 再判断一次可否接受订单
        if (Player.Instance.CanAddOrder(m_order))
        {
            // 订单显示关闭，并添加到订单列表
            MyOrderPanel.SetActive(false);
            OKImage.SetActive(false);
            Player.Instance.AddOrder(m_order);

            // 耐心条回满
            patience = patienceMax;

            // 增加好感度
            Statistics.Instance.Love(myFaction, 1);
        }
        else
        {
            Debug.LogWarning("出现了可能的错误");
            // 如果不可以接受订单，则可以重新点击确定键来判断
            isClickAccept = false;         
        }
    }
    public void CancelTask()
    {
        // Debug.Log("拒绝任务");
        MyOrderPanel.SetActive(false);
        m_order.isSelect = true;
        m_order.isAccept = false;
        // 减少好感度
        Statistics.Instance.Love(myFaction, -1);
    }

    IEnumerator ShakeButton(Button myButton)
    {
        isClickAccept = true;
        Vector3 originalPosition = myButton.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // 生成随机的偏移量，用于实现抖动效果
            float x = originalPosition.x + Random.Range(-shakeAmount, shakeAmount);
            float y = originalPosition.y + Random.Range(-shakeAmount, shakeAmount);

            // 将按钮位置设置为随机偏移的位置
            myButton.transform.position = new Vector3(x, y, originalPosition.z);

            // 等待一帧
            yield return null;

            // 更新已经过去的时间
            elapsed += Time.deltaTime;
        }

        // 恢复按钮到原始位置
        myButton.transform.position = originalPosition;
        isClickAccept = false;
    }

    #endregion

    IEnumerator PatientCo()
    {
        while (true)
        {
            yield return null;
            // 在非奔跑状态，即等待玩家过来时的状态，耐心值会减
            // 再订单完成后，不会减少
            if (!anim.GetBool("Running")&& !m_order.isFinish)
            {
                patience -= Time.deltaTime;
                // 耐心条更新
                PatienceSlider.value = patience / patienceMax;
                // 订单的耐心也更新
                m_order.patience = patience / patienceMax;
            }

            PatienceSliderFillImage.color = patience / patienceMax <= 0.3f ? red : green;

            if (patience < 0)
            {
                MyOrderPanel.SetActive(false);
                yield break;
            }

        }
    }

    #region Chat

    public void OnWait()
    {
        StartCoroutine(ChatCo());
    }

    IEnumerator ChatCo()
    {
        yield break;
        //// 时不时说话的效果
        //while (true)
        //{
        //    yield return null;
        //    float t = Random.Range(5, 10);
        //    yield return new WaitForSeconds(t);
        //    StartChat();
        //    t = Random.Range(1, 3);
        //    yield return new WaitForSeconds(t);
        //    EndChat();
        //}
    }

    private void StartChat()
    {
        if (!chatBubble.activeSelf)
        {
            chatImage.sprite = chatImages[Random.Range(0, chatImages.Count)];
            chatBubble.SetActive(true);
            anim.SetBool("Chating", true);
        }
    }

    private void EndChat()
    {
        chatBubble.SetActive(false);
        // anim.SetBool("Chating", false);
    }

    #endregion
}
