using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopPanel : MonoBehaviour
{
    [Header("…Ã∆∑")]
    public Goods goods1;
    public Goods goods2;
    public Goods goods3;
    public Goods goods4;

    public void ClickCloseButton()
    {
        gameObject.SetActive(false);
        goods1.gameObject.SetActive(true); 
        goods2.gameObject.SetActive(true);
        goods3.gameObject.SetActive(true);
        goods4.gameObject.SetActive(true);
    }
}
