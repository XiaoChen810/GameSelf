using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantRequest : EventBase
{
    [Header("特殊订单")]
    public Order order;
    [Header("折扣")]
    public float discount = 0.9f;
    [Header("期限/秒")]
    public float patienceMax;
    private float patience;


    public override void OnAccept()
    {
        InitOrder();
        patience = patienceMax;
        Player.Instance.AddOrder(order);
        StartCoroutine(OrderListen());
        base.OnAccept();
    }

    private void InitOrder()
    {
        order.progress = 1;
        order.patience = 1;
        order.isAccept = false;
        order.isFinish = false;
        order.isSelect = false;
        order.isStart = false;
        order.isOutTime = false;
    }

    public override bool TriggerConditions()
    {
        if(!Player.Instance.CanAddOrder(order))
        {
            return false;
        }
        return base.TriggerConditions();
    }

    IEnumerator OrderListen()
    {
        while(true)
        {
            yield return null;
            if(order.isOutTime)
            {
                // 订单超时的逻辑
                yield break;
            }
            if(order.isFinish)
            {
                // 完成订单的逻辑
                Merchant merchant = GameObject.Find("Merchant").GetComponent<Merchant>();
                merchant.goodsDiscount *= discount;
                yield break;
            }
            patience -= Time.deltaTime;
            order.patience = patience / patienceMax;
        }
    }
}
