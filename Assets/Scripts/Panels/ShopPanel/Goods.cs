using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Goods : MonoBehaviour
{
    public string Name;
    public Image Image;
    public Text Number;
    public Text Price;
    public Button button;
    public int num,price;

    private void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickBuy);
    }


    private void OnClickBuy()
    {
        switch (Name)
        {
            case "ľͷ":
                {
                    Statistics.Instance.Wood += num;
                    break;
                }
            case "���":
                {
                    Statistics.Instance.Brics += num;
                    break;
                }
            case "����":
                {
                    Statistics.Instance.Iron += num;
                    break;
                }
        }

        Statistics.Instance.Cost(price);
        gameObject.SetActive(false);
    }
}




