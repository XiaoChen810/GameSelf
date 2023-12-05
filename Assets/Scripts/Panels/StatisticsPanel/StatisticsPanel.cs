using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : MonoBehaviour
{
    [Header("���")]
    public Text Gold;
    [Header("ľͷ")]
    public Text Wood;
    [Header("��")]
    public Text Iron;
    [Header("��ש")]
    public Text Brics;
    [Header("��ʳ��")]
    public Slider Satiety;
    [Header("����")]
    public Slider Energy;
    [Header("����")]
    public Text Date;

    public Button ToNextDateBtn;

    private void Start()
    {
        ToNextDateBtn.onClick.RemoveAllListeners();
        ToNextDateBtn.onClick.AddListener(Statistics.Instance.ToNextDate);
    }

    private void Update()
    {
        //Statistics statistics = Statistics.Instance;
        //Gold.text = statistics.Gold.ToString();
        //Wood.text = statistics.Wood.ToString();
        //Iron.text = statistics.Iron.ToString();
        //Brics.text = statistics.Brics.ToString();
        //Satiety.value = statistics.Satiety / statistics.SatietyMax;
        //Energy.value = statistics.Energy / statistics.EnergyMax;
        //Date.text = $"Date: {statistics.Date.ToString()}";
    }
}
