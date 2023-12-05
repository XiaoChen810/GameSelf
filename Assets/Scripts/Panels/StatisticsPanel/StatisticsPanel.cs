using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsPanel : MonoBehaviour
{
    [Header("金币")]
    public Text Gold;
    [Header("木头")]
    public Text Wood;
    [Header("铁")]
    public Text Iron;
    [Header("金砖")]
    public Text Brics;
    [Header("饱食度")]
    public Slider Satiety;
    [Header("精力")]
    public Slider Energy;
    [Header("天数")]
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
