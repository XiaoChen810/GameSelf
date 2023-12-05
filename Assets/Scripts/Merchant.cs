using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Merchant : MonoBehaviour
{
    private Transform body;
    private Rigidbody2D rb;
    private PolygonCollider2D coll;
    private Animator anim;

    [Header("�ƶ��ٶ�")]
    public float speed;
    public Vector2 targetPos;
    private Vector3 lastTransfrom;

    [Header("�̵����")]
    public ShopPanel shopPanel;

    [Header("����ͼ��")]
    public List<Sprite> goodsSpriteList = new List<Sprite>();
    [SerializedDictionary("����", "����ͼ��")]
    public SerializedDictionary<Sprite, string> goodsDict = new SerializedDictionary<Sprite, string>();

    [Header("���������С����")]
    public int goodsMinCount;
    public int goodsMaxCount;

    [Header("����������ͼ۸�")]
    public int goodsMinPrice;
    public int goodsMaxPrice;

    [Header("�����ۿ�")]
    [Range(0.1f, 1f)] public float goodsDiscount = 1;

    public enum State
    {
        ����,
        ������
    }
    [Header("״̬")]
    public State state;

    private void Awake()
    {
        body = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();


        shopPanel.gameObject.SetActive(false);
        state = State.������;

        ShopUpdate();
    }

    private void OnEnable()
    {
        Statistics.Instance.OnDateUpdate += ShopUpdate;
    }

    private void ShopUpdate()
    {
        // �ȹر��̵�ҳ��
        shopPanel.gameObject.SetActive(false);
        // ���»�����Ϣ       
        GoodsUpdate(shopPanel.goods1);
        GoodsUpdate(shopPanel.goods2);
        GoodsUpdate(shopPanel.goods3);
        GoodsUpdate(shopPanel.goods4);

        // Debug.Log("������Ϣ�������");
    }

    private void GoodsUpdate(Goods goods)
    {
        // ����µ�ͼƬ���������۸�
        goods.Image.sprite = goodsSpriteList[Random.Range(0, goodsSpriteList.Count)];
        goods.num = Random.Range(goodsMinCount, goodsMaxCount);
        goods.price = (int)(Random.Range(goodsMinPrice, goodsMaxPrice) * goodsDiscount);

        // �����µ�ͼƬ���������۸�
        goods.Name = goodsDict[goods.Image.sprite];
        goods.Number.text = goods.num.ToString();
        goods.Price.text = goods.price.ToString();

        goods.isBuy = false;
        goods.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        Statistics.Instance.OnDateUpdate -= ShopUpdate;
    }

    private void OnMouseDown()
    {
        if(state == State.����)
        {
            shopPanel.gameObject.SetActive(true);
        }
    }

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
        if (Mathf.Abs(transform.position.x - target.x) > 0.1f)
        {
            anim.SetBool("Runing", true);
            Vector2 dir = new Vector2(target.x - transform.position.x, 0);
            dir.Normalize();
            rb.velocity = new Vector2(dir.x * speed * Time.deltaTime, rb.velocity.y);

        }
        else
        {
            anim.SetBool("Runing", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    #endregion

}
