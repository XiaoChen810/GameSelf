using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderPanel : MonoBehaviour
{
    public Order order;

    public Text Price;
    public Text Met;
    public Image image;
    public Slider Progress;
    public Slider Patience;

    private void Update()
    {
        Progress.value = order.progress;
        Patience.value = order.patience;

        // ¶©µ¥³¬Ê±
        if(Patience.value <= 0)
        {
            order.isOutTime = true;
            Player.Instance.RemoveOrder(order);
            Craftsman craftsman = GameObject.Find("Craftsman").GetComponent<Craftsman>();
            craftsman.WorkOutTime(order);
            Destroy(gameObject);
        }
    }

}
