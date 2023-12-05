using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

[System.Serializable]
public class Order
{
    [Header("�۸�")]
    public float price;
    [Header("�������")]
    public float materialNum;
    [Header("ͼ��")]
    public Sprite icon;
    [Header("��ѡ��")]
    public bool isSelect;
    [Header("�ѽ���")]
    public bool isAccept;
    [Header("�����")]
    public bool isFinish;
    [Header("�ѿ�ʼ")]
    public bool isStart;
    [Header("�ѳ�ʱ")]
    public bool isOutTime;
    [Header("����")]
    public float progress = 1;
    [Header("����")]
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
        ����, ����, ð����
    }
    [Header("��Ӫ")]
    public Faction myFaction;

    [Header("����")]
    public Order m_order;
    public GameObject MyOrderPanel;
    public Image orderContentImage;
    public List<Sprite> orderContentSprites;

    [Header("�ƶ��ٶ�")]
    public float speed;
    public Vector2 targetPos;
    private Vector3 lastTransfrom;

    [Header("��ť����")]
    public float shakeDuration = 0.2f;
    public float shakeAmount = 0.1f;

    [Header("����")]
    public GameObject chatBubble;
    public Image chatImage;
    public List<Sprite> chatImages;

    [Header("����")]
    public Slider PatienceSlider;
    public Image PatienceSliderFillImage;
    public float patienceMax = 20;
    public float patience;
    public Color green;
    public Color red;
    
    [Header("��ѡ��")]
    public bool isSelected;
    private Transform QueuingPoint;
    public SpriteRenderer OutlineRenderer;
    public HighLight highLight;

    [Header("OK")]
    public GameObject OKImage;

    public Coroutine chatCoroutine;

    private void Start()
    {
        // �������
        rb = GetComponentInChildren<Rigidbody2D>();
        coll = GetComponentInChildren<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();
        canvas = GetComponentInChildren<Canvas>();

        // UI��ϵ
        PriceText.text = m_order.price.ToString();
        MaterialNumText.text = m_order.materialNum.ToString();
        AcceptBtn.onClick.AddListener(AcceptTask);
        CancelBtn.onClick.AddListener(CancelTask);
        canvas.worldCamera = Camera.main;

        // λ�������ʼ��
        targetPos = transform.position;
        lastTransfrom = transform.position;
        QueuingPoint = GameObject.Find("�Ŷӵ�").GetComponent<Transform>();

        // UI��ʼ��
        chatBubble.SetActive(false);
        MyOrderPanel.SetActive(false);

        // ��������ֵЭ��
        patience = patienceMax;
        // StartCoroutine(PatientCo());
        
        // �����¼�
        Statistics.Instance.OnDayDark += DateDark;
    }

    private void OnEnable()
    {
        // ��ʼ����������
        if(myFaction == Faction.����)
        {
            m_order.price = Random.Range(GameManager.Instance.minPrice, GameManager.Instance.maxPrice + 1);
            m_order.materialNum = Random.Range(GameManager.Instance.minMaterialNum, GameManager.Instance.maxMaterialNum + 1);
            m_order.icon = orderContentSprites[Random.Range(0, orderContentSprites.Count)];
            orderContentImage.sprite = m_order.icon;

        }
        else if(myFaction == Faction.����)
        {
            // ����٣�����Ҳ��
            m_order.price = Random.Range(GameManager.Instance.minPrice - 5, GameManager.Instance.maxPrice + 1 - 5);
            m_order.materialNum = Random.Range(GameManager.Instance.minMaterialNum - 2, GameManager.Instance.maxMaterialNum + 1 - 2);
            m_order.icon = orderContentSprites[Random.Range(0, orderContentSprites.Count)];
            orderContentImage.sprite = m_order.icon;
        }
        else if(myFaction == Faction.ð����)
        {
            // ���࣬����Ҳ��
            m_order.price = Random.Range(GameManager.Instance.minPrice + 5, GameManager.Instance.maxPrice + 1 + 5);
            m_order.materialNum = Random.Range(GameManager.Instance.minMaterialNum + 2, GameManager.Instance.maxMaterialNum + 1 + 2);
            m_order.icon = orderContentSprites[Random.Range(0, orderContentSprites.Count)];
            orderContentImage.sprite = m_order.icon;
        }
        // У��
        m_order.price = m_order.price <= 0 ? 1 : m_order.price;
        m_order.materialNum = m_order.materialNum <= 0 ? 1 : m_order.materialNum;
    }

    private void DateDark()
    {
        if(!m_order.isAccept)
        {
            GameManager.Instance.LeaveDoor(targetPos);      // ���뿪ǰ��λ�û���ȥ
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

        // UI����
        //// �������ݵķ���
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
        // �ڷǱ���״̬�����ȴ���ҹ���ʱ��״̬������ֵ���
        // �ٶ�����ɺ󣬲������
        if (!anim.GetBool("Running") && !m_order.isFinish)
        {
            patience -= Time.deltaTime;
            // ����������
            PatienceSlider.value = patience / patienceMax;
            // ����������Ҳ����
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
        // �����
        if (isSelected)
        {
            // ������δ��ѡ��ʱ,��ͣ����ʱ����ʾ����
            if (!m_order.isSelect && !anim.GetBool("Running"))
            {
                MyOrderPanel.SetActive(true);
            }

            // �����û����ѡ��״̬
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
            // �ƶ���ʱ�򲻻�˵��
            EndChat();
        }
        else
        {
            //Debug.Log("����Ŀ���λ��: " + distance + "�ҵ�λ��" + transform.position.x + "����λ��" + target.x);
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
            // �����ظ����
            return;
        }
        else
        {
            isClickAccept = true;
            // ����������������֧�����ҿ�����Ӷ���ʱ
            if (Player.Instance.CanAddOrder(m_order))
            {
                // Player������Ӧ�������ƶ�����,��Ӷ�����
                Player.OnReachThere onReachThere = new Player.OnReachThere(AcceptOrder);
                Player.Instance.ToThereAndCallBack(Player.State.ToCustomer, QueuingPoint.position, onReachThere);

            }
            else
            {
                // Debug.Log("�޷���������");
                // ����
                StartCoroutine(ShakeButton(AcceptBtn));
            }
        }
    }
    private void AcceptOrder()
    {
        // ���ж�һ�οɷ���ܶ���
        if (Player.Instance.CanAddOrder(m_order))
        {
            // ������ʾ�رգ�����ӵ������б�
            MyOrderPanel.SetActive(false);
            OKImage.SetActive(false);
            Player.Instance.AddOrder(m_order);

            // ����������
            patience = patienceMax;

            // ���Ӻøж�
            Statistics.Instance.Love(myFaction, 1);
        }
        else
        {
            Debug.LogWarning("�����˿��ܵĴ���");
            // ��������Խ��ܶ�������������µ��ȷ�������ж�
            isClickAccept = false;         
        }
    }
    public void CancelTask()
    {
        // Debug.Log("�ܾ�����");
        MyOrderPanel.SetActive(false);
        m_order.isSelect = true;
        m_order.isAccept = false;
        // ���ٺøж�
        Statistics.Instance.Love(myFaction, -1);
    }

    IEnumerator ShakeButton(Button myButton)
    {
        isClickAccept = true;
        Vector3 originalPosition = myButton.transform.position;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // ���������ƫ����������ʵ�ֶ���Ч��
            float x = originalPosition.x + Random.Range(-shakeAmount, shakeAmount);
            float y = originalPosition.y + Random.Range(-shakeAmount, shakeAmount);

            // ����ťλ������Ϊ���ƫ�Ƶ�λ��
            myButton.transform.position = new Vector3(x, y, originalPosition.z);

            // �ȴ�һ֡
            yield return null;

            // �����Ѿ���ȥ��ʱ��
            elapsed += Time.deltaTime;
        }

        // �ָ���ť��ԭʼλ��
        myButton.transform.position = originalPosition;
        isClickAccept = false;
    }

    #endregion

    IEnumerator PatientCo()
    {
        while (true)
        {
            yield return null;
            // �ڷǱ���״̬�����ȴ���ҹ���ʱ��״̬������ֵ���
            // �ٶ�����ɺ󣬲������
            if (!anim.GetBool("Running")&& !m_order.isFinish)
            {
                patience -= Time.deltaTime;
                // ����������
                PatienceSlider.value = patience / patienceMax;
                // ����������Ҳ����
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
        //// ʱ��ʱ˵����Ч��
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
