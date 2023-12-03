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

    [Header("表情")]
    public GameObject chatBubble;
    public Image chatImage;

    [Header("被选中")]
    public bool isSelected;
    [HideInInspector] public HighLight highLight;

    [Header("音效")]
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
    /// 当按下时,设为被选择，玩家会靠近，然后沟通，然后开始工作
    /// </summary>
    private void OnMouseDown()
    {
        // 仅影响Action_Selected
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
    //            Debug.LogWarning("改变了目标");
    //            yield break;
    //        }

    //        float distance = Mathf.Abs(Player.Instance.transform.position.x - chatPos.x);

    //        // 当接近后沟通，沟通完后工作
    //        if (distance < 0.1f)
    //        {
    //            StartChat();

    //            float chatTime = 5f;
    //            while (chatTime > 0f)
    //            {
    //                yield return null;

    //                if (HasPlayerStateChanged())
    //                {
    //                    Debug.LogWarning("沟通时改变了目标");
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
            // 这一次敲击后完成订单
            curOrder.progress = 0;
            workSlider.value = 0;
            Player.Instance.FinishOrder(curOrder);

            // 获取新的订单
            curOrder = Player.Instance.GetOrder();
        }
        else
        {
            // 订单完成度增加
            curOrder.progress -= WorkSpeed;

            // 播放音效
            workAudioSource.PlayOneShot(workAudioSource.clip);

            // UI值改变
            workSlider.value = curOrder.progress;
        }
    }

    public void WorkOutTime(Order order)
    {
        Debug.Log("订单超时");
        if(curOrder == order)
        {
            Debug.Log("获取新的订单");
            // 获取新的订单
            curOrder = Player.Instance.GetOrder();
        }
    }

    #region Chat

    private bool isInChat;  // 防止多次点击
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
