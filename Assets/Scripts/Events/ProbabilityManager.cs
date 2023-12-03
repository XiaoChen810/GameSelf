using System.Collections.Generic;
using UnityEngine;

public class ProbabilityManager : MonoBehaviour 
{
    private static ProbabilityManager instance;
    public static ProbabilityManager Instance
    {
        get
        {
            return instance;
        }
    }

    // �����¼��б����ڴ��ֻ�ܴ���һ���¼�ʱ
    [SerializeField] private List<ProbabilisticEvent> probabilisticEvents = new List<ProbabilisticEvent>();
    private float totalProbability;

    // ������ʸ������������жϿ���ͬʱ�������¼�
    [SerializeField] private float probabilisticRandonFloat;

    private void Awake()
    {
        instance = this;
    }


    public void ADDEvent(ProbabilisticEvent probabilisticEvent)
    {
        probabilisticEvents.Add(probabilisticEvent);
    }

    public bool ListEvent(ProbabilisticEvent e)
    {
        if(probabilisticEvents.Count == 0)
        {
            return false;
        }

        totalProbability = 0f;
        foreach(var  probabilisticEvent in probabilisticEvents)
        {
            totalProbability += probabilisticEvent.Probability;
        }

        float p = Random.Range(0, totalProbability);

        totalProbability = 0f;
        foreach (var probabilisticEvent in probabilisticEvents)
        {
            totalProbability += probabilisticEvent.Probability;
            if(p <= totalProbability && probabilisticEvent.Name == e.Name)
            {
                return true;
            }
        }

        return false;
    }

    public float GetANewProbabilityParameter()
    {
        probabilisticRandonFloat = Random.Range(0, 100f);
        return probabilisticRandonFloat;
    }

}

[System.Serializable]
public class ProbabilisticEvent
{
    [HideInInspector] public string Name;

    public bool IsProbabilistic;

    [Header("����")]
    [Range(0, 100)] public float Probability;
}
