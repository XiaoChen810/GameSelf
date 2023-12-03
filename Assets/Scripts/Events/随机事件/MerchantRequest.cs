using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantRequest : EventBase
{
    [Header("���ⶩ��")]
    public Order order;
    [Header("�ۿ�")]
    public float discount = 0.9f;
    [Header("����/��")]
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
                // ������ʱ���߼�
                yield break;
            }
            if(order.isFinish)
            {
                // ��ɶ������߼�
                Merchant merchant = GameObject.Find("Merchant").GetComponent<Merchant>();
                merchant.goodsDiscount *= discount;
                yield break;
            }
            patience -= Time.deltaTime;
            order.patience = patience / patienceMax;
        }
    }
}
