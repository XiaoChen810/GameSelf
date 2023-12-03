using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Craftsman : MonoBehaviour
{
    private Rigidbody2D rb;
    private PolygonCollider2D coll;
    private Animator anim;
    private Slider workSlider;
    private Canvas canvas;

    [Range(0,1)] public float WorkSpeed;
    public Order curOrder = new Order();
    public bool isWorking;

    [Header("����")]
    public GameObject chatBubble;
    public Image chatImage;

    [Header("��ѡ��")]
    public bool isSelected;
    [HideInInspector] public HighLight highLight;

    [Header("��Ч")]
    public AudioSource workAudioSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();
        workSlider = GetComponentInChildren<Slider>();
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        highLight = GetComponent<HighLight>();  
    }

    private void Update()
    {
        if(isWorking)
        {
            curOrder = Player.Instance.GetOrder();
            highLight.EndHighLight();
            if(curOrder != null)
            {
                if(!curOrder.isStart)
                {
                    curOrder.isStart = true;
                    workSlider.value = 1;
                }
                anim.SetBool("Working", true);
            }
            else
            {
                isWorking = false;
                anim.SetBool("Working", false);
            }
        }
    }

    /// <summary>
    /// ������ʱ,��Ϊ��ѡ����һ῿����Ȼ��ͨ��Ȼ��ʼ����
    /// </summary>
    private void OnMouseDown()
    {
        // ��Ӱ��Action_Selected
        if (!isWorking)
        {
            isSelected = true;
            highLight.StartHighLight();
        }
    }

    #region old
    //IEnumerator TalkWork(Vector2 chatPos)
    //{

    //    while (true)
    //    {
    //        yield return null;

    //        if (HasPlayerStateChanged())
    //        {
    //            Debug.LogWarning("�ı���Ŀ��");
    //            yield break;
    //        }

    //        float distance = Mathf.Abs(Player.Instance.transform.position.x - chatPos.x);

    //        // ���ӽ���ͨ����ͨ�����
    //        if (distance < 0.1f)
    //        {
    //            StartChat();

    //            float chatTime = 5f;
    //            while (chatTime > 0f)
    //            {
    //                yield return null;

    //                if (HasPlayerStateChanged())
    //                {
    //                    Debug.LogWarning("��ͨʱ�ı���Ŀ��");
    //                    EndChat();
    //                    yield break;
    //                }

    //                chatTime -= Time.deltaTime;
    //            }

    //            isWorking = true;
    //            Player.Instance.state = Player.State.None;
    //            EndChat();

    //            yield break;
    //        }
    //    }
    //}

    //bool HasPlayerStateChanged()
    //{
    //    return Player.Instance.state != Player.State.TalkingForCraftsman;
    //}

    #endregion

    public void Work()
    {
        if (curOrder.progress - WorkSpeed <= 0)
        {
            // ��һ���û�����ɶ���
            curOrder.progress = 0;
            workSlider.value = 0;
            Player.Instance.FinishOrder(curOrder);

            // ��ȡ�µĶ���
            curOrder = Player.Instance.GetOrder();
        }
        else
        {
            // ������ɶ�����
            curOrder.progress -= WorkSpeed;

            // ������Ч
            workAudioSource.PlayOneShot(workAudioSource.clip);

            // UIֵ�ı�
            workSlider.value = curOrder.progress;
        }
    }

    public void WorkOutTime(Order order)
    {
        Debug.Log("������ʱ");
        if(curOrder == order)
        {
            Debug.Log("��ȡ�µĶ���");
            // ��ȡ�µĶ���
            curOrder = Player.Instance.GetOrder();
        }
    }

    #region Chat

    private bool isInChat;  // ��ֹ��ε��
    public void StartChat(float time)
    {
        if (!isInChat && !isWorking)
        {
            // Debug.Log("StartChat");
            StartCoroutine(ChatCo(time));
        }
    }

    IEnumerator ChatCo(float time)
    {
        isInChat = true;
        anim.SetBool("Chating", true);
        yield return new WaitForSeconds(time);
        anim.SetBool("Chating", false);
        isInChat = false;
    }

    public void EndChat()
    {
        anim.SetBool("Chating", false);
    }
    #endregion
}
