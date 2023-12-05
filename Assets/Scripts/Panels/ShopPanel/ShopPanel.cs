using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [Header("��Ʒ")]
    public Goods goods1;
    public Goods goods2;
    public Goods goods3;
    public Goods goods4;

    public void ClickCloseButton()
    {
        gameObject.SetActive(false);
        goods1.gameObject.SetActive(!goods1.isBuy); 
        goods2.gameObject.SetActive(!goods2.isBuy);
        goods3.gameObject.SetActive(!goods3.isBuy);
        goods4.gameObject.SetActive(!goods4.isBuy);
    }
}
