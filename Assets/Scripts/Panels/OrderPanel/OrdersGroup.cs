using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OrdersGroup : MonoBehaviour
{
    public static OrdersGroup Instance;
    public GameObject orderPanelPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        foreach (Transform c in transform)
        {
            OrderPanel o = c.GetComponent<OrderPanel>();
            if (o.order.isFinish)
            {
                Destroy(c.gameObject);
            }
        }
    }

    public void Add(Order order)
    {
        // 获取预制件的OrderPanel组件
        GameObject o = Instantiate(orderPanelPrefab,transform);
        OrderPanel orderPanel = o.GetComponent<OrderPanel>();

        orderPanel.order = order;
        orderPanel.Price.text = order.price.ToString();
        orderPanel.Met.text = order.materialNum.ToString();
        orderPanel.image.sprite = order.icon; 
    }
}
