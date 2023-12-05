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

    [Header("移动速度")]
    public float speed;
    public Vector2 targetPos;
    private Vector3 lastTransfrom;

    [Header("商店面板")]
    public ShopPanel shopPanel;

    [Header("货物图标")]
    public List<Sprite> goodsSpriteList = new List<Sprite>();
    [SerializedDictionary("名字", "货物图标")]
    public SerializedDictionary<Sprite, string> goodsDict = new SerializedDictionary<Sprite, string>();

    [Header("货物最大最小数量")]
    public int goodsMinCount;
    public int goodsMaxCount;

    [Header("货物的最高最低价格")]
    public int goodsMinPrice;
    public int goodsMaxPrice;

    [Header("货物折扣")]
    [Range(0.1f, 1f)] public float goodsDiscount = 1;

    public enum State
    {
        工作,
        不工作
    }
    [Header("状态")]
    public State state;

    private void Awake()
    {
        body = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<PolygonCollider2D>();
        anim = GetComponent<Animator>();


        shopPanel.gameObject.SetActive(false);
        state = State.不工作;

        ShopUpdate();
    }

    private void OnEnable()
    {
        Statistics.Instance.OnDateUpdate += ShopUpdate;
    }

    private void ShopUpdate()
    {
        // 先关闭商店页面
        shopPanel.gameObject.SetActive(false);
        // 更新货物信息       
        GoodsUpdate(shopPanel.goods1);
        GoodsUpdate(shopPanel.goods2);
        GoodsUpdate(shopPanel.goods3);
        GoodsUpdate(shopPanel.goods4);

        // Debug.Log("货物信息更新完成");
    }

    private void GoodsUpdate(Goods goods)
    {
        // 随机新的图片，数量，价格
        goods.Image.sprite = goodsSpriteList[Random.Range(0, goodsSpriteList.Count)];
        goods.num = Random.Range(goodsMinCount, goodsMaxCount);
        goods.price = (int)(Random.Range(goodsMinPrice, goodsMaxPrice) * goodsDiscount);

        // 更新新的图片，数量，价格
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
        if(state == State.工作)
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
