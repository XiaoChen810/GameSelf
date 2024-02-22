using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Customer")]
    // public GameObject CustomerPrefab;
    public List<GameObject> CustomerPrefabsList;
    [SerializeField]private float CreateCustomerTime;    // 每隔一段时间来一个顾客
    public Vector3 CreatePosition;      // 生成点位置
    public bool OpenCreateCustomer;     // 开关

    [Header("门前排队位置队列")]
    public Vector3 DoorPosition;
    public float Spacing;
    public Queue<Vector3> DoorQueue = new Queue<Vector3>();
    public List<Vector3> DoorList = new List<Vector3>();

    [Header("地图边界")]
    public Vector3 MapEdgePosition;

    [Header("事件")]
    public GameObject EventPanel;
    public Text HeaderText;
    public Text DescriptionText;
    public Button AcceptBtn;
    public Button CancelBtn;

    [Header("菜单")]
    public GameObject MenuPanel;

    [Header("难度")]
    public int minPrice;
    public int maxPrice;
    public int minMaterialNum;
    public int maxMaterialNum;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < 10; i++)
        {
            DoorQueue.Enqueue(new Vector3(DoorPosition.x + i * Spacing,0,0));   
        }

        StartCoroutine(CreateCustomerCo());
        StartCoroutine(ChectQueueCo(2,DoorQueue));

        Init();
    }

    private void Init()
    {
        MenuPanel.SetActive(false);
        EventPanel.SetActive(false);
    }

    private void Start()
    {
        Statistics.Instance.OnDayDark += DateDark;
        Statistics.Instance.OnGoldUpdate += Instance_OnGoldUpdate;
        Statistics.Instance.OnMoning += Instance_OnMoning;
        Statistics.Instance.OnAfternoon += Instance_OnAfternoon;
        Statistics.Instance.OnNoon += Instance_OnNoon;
        Statistics.Instance.OnNight += Instance_OnNight;
    }

    #region TimePeriod

    private void Instance_OnMoning(float arg1, float arg2)
    {
        SetND(minPrice - 1, maxPrice - 1, minMaterialNum, maxMaterialNum);
    }

    private void Instance_OnAfternoon(float arg1, float arg2)
    {
        SetND(minPrice + 1, maxPrice + 1, minMaterialNum, maxMaterialNum);
    }

    private void Instance_OnNoon(float arg1, float arg2)
    {
        SetND(minPrice + 3, maxPrice + 3, minMaterialNum, maxMaterialNum);
    }
    private void Instance_OnNight(float arg1, float arg2)
    {
        SetND(minPrice - 3, maxPrice - 3, minMaterialNum, maxMaterialNum);
    }

    private void DateDark()
    {
        
    }

    #endregion

    private void OnDestroy()
    {
        Statistics.Instance.OnDayDark -= DateDark;
        Statistics.Instance.OnGoldUpdate -= Instance_OnGoldUpdate;
        Statistics.Instance.OnMoning -= Instance_OnMoning;
        Statistics.Instance.OnAfternoon -= Instance_OnAfternoon;
        Statistics.Instance.OnNoon -= Instance_OnNoon;
        Statistics.Instance.OnNight -= Instance_OnNight;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            MenuPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    #region Customers

    IEnumerator CreateCustomerCo()
    {
        while (true)
        {
            if (!OpenCreateCustomer)
            {
                yield return new WaitForEndOfFrame();
                continue;
            }
            float t = UnityEngine.Random.Range(CreateCustomerTime - 2, CreateCustomerTime + 3);
            yield return new WaitForSeconds(t);
            Instantiate(SelectCustomer(), CreatePosition, Quaternion.identity);
        }
    }

    private GameObject SelectCustomer()
    {
        // 获取各方势力的好感度
        float a = Statistics.Instance.LoveResidents;
        float b = Statistics.Instance.LoveMonster;
        float c = Statistics.Instance.LoveAdventurer;

        // 根据好感度提高生成的概率
        List<float> loves = new List<float>();
        loves.Add(a);
        loves.Add(b);
        loves.Add(c);

        // 获取随机数
        float total = a + b + c;
        float range = UnityEngine.Random.Range(0, total);

        // 返回对应的预制体
        total = 0;
        int index = 0;
        foreach (float love in loves)
        {
            total += love;

            if(range < total)
            {
                return CustomerPrefabsList[index];
            }

            index++;
        }

        return CustomerPrefabsList[0];
    }


    /// <summary>
    /// 队列相关
    /// </summary>
    /// <param name="time"></param>
    /// <param name="queue"></param>
    /// <returns></returns>
    IEnumerator ChectQueueCo(float time, Queue<Vector3> queue)
    {
        while (true)
        {
            // 每隔一段时间检查队列是否有重复的值
            yield return new WaitForSeconds(time);

            HashSet<Vector3> uniqueValues = new HashSet<Vector3>();
            Queue<Vector3> nonDuplicateQueue = new Queue<Vector3>();

            while (queue.Count > 0)
            {
                Vector3 currentValue = queue.Dequeue();

                if (uniqueValues.Add(currentValue))
                {
                    nonDuplicateQueue.Enqueue(currentValue);
                }
                else
                {
                    Debug.Log("Duplicate value found and removed: " + currentValue);
                }
            }

            // 将非重复的值重新放回队列
            while (nonDuplicateQueue.Count > 0)
            {
                queue.Enqueue(nonDuplicateQueue.Dequeue());
            }
        }


    }

    public Vector3 GetDoorPosition(Vector3 oldPosition)
    {
        // 如果提供的位置已经在等待列表中
        if (DoorList.Contains(oldPosition))
        {
            // 如果队列不为空，找到队列中最近的位置
            if (DoorQueue.Count > 0)
            {
                Vector3 closestPosition = FindClosestPosition();
                // 如果有更近的位置
                if (closestPosition.x < oldPosition.x)
                {
                    for(int i = 0; i < DoorQueue.Count; i++)
                    {
                        Vector3 t = DoorQueue.Dequeue();
                        if(t != closestPosition)
                        {
                            DoorQueue.Enqueue(t);
                        }
                        else
                        {
                            DoorQueue.Enqueue(oldPosition);
                            DoorList.RemoveAll(item => item == oldPosition);
                            DoorList.Add(closestPosition);

                            return closestPosition;
                        }
                    }
                }
                else
                {
                    // 没有则返回原来的位置
                    return oldPosition;
                }
            }
        }
        if (!DoorQueue.Contains(oldPosition))
        {
            if(DoorQueue.Count > 0)
            {
                // 出队一个位置
                Vector3 newPosition = DoorQueue.Dequeue();
                // Debug.Log("出队一个位置" + newPosition);
                // 添加到等待列表中
                DoorList.Add(newPosition);
                // 返回出队的位置
                return newPosition;
            }
        }
        return oldPosition;
    }

    private Vector3 FindClosestPosition()
    {
        Vector3 closestPosition = DoorQueue.Peek();

        foreach (Vector3 position in DoorQueue)
        {
            if (position.x < closestPosition.x)
            {
                closestPosition = position;
            }
        }

        return closestPosition;
    }

    public void LeaveDoor(Vector3 providedPosition)
    {
        // Debug.Log("入队一个位置" + providedPosition);
        // 入队提供的位置
        DoorQueue.Enqueue(providedPosition);
        // 删除等待队列中提供的位置
        DoorList.RemoveAll(item => item == providedPosition);
    }


    #endregion

    #region Event

    public bool TryShowEvent()
    {
        if (EventPanel.activeSelf == false)
        {
            EventPanel.SetActive(true);
            Time.timeScale = 0;
            return true;
        }
        else
        {
            Debug.Log("已经有事件在打开");
            return false;
        }
    }

    public bool ShowEventButNoAccept()
    {
        if (EventPanel.activeSelf == false)
        {
            EventPanel.SetActive(true);
            AcceptBtn.gameObject.SetActive(false);
            Time.timeScale = 0;
            return true;
        }
        else
        {
            Debug.Log("已经有事件在打开");
            return false;
        }
    }

    public bool ShowEventButNoCancel()
    {
        if (EventPanel.activeSelf == false)
        {
            EventPanel.SetActive(true);
            CancelBtn.gameObject.SetActive(false);
            Time.timeScale = 0;
            return true;
        }
        else
        {
            Debug.Log("已经有事件在打开");
            return false;
        }
    }

    public void CloseEvent()
    {
        EventPanel.SetActive(false);
        AcceptBtn.gameObject.SetActive(true);
        CancelBtn.gameObject.SetActive(true);
        Time.timeScale = 1;
    }

    #endregion

    #region Menu

    public void OnContinue()
    {
        MenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnExit()
    {
        Application.Quit();
        Time.timeScale = 1f;
    }

    public void OnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Statistics.Instance.Restart();
        Time.timeScale = 1f;
    }

    #endregion

    #region Level 

    private void Instance_OnGoldUpdate(float gold)
    {
        // 随玩家金币数量变化,基准值2.5
        if (gold < 30)
        {
            SetND(5, 10, 1, 2);
        }
        if (gold >= 30 && gold < 60)
        {
            SetND(9, 15, 3, 5);
        }
        if (gold >= 60 && gold < 100)
        {
            SetND(12, 18, 5, 7);
        }
        if (gold >= 100 && gold < 200)
        {
            SetND(10, 20, 7, 10);
        }
        if (gold >= 200 && gold < 1000)
        {
            SetND(12, 40, 5, 15);
        }
    }

    private void SetND(int a, int b, int c, int d)
    {
        minPrice = a;
        maxPrice = b;
        minMaterialNum = c;
        maxMaterialNum = d;
    }

    public void CreateCustomerTimeAdd(float addNum)
    {
        Debug.Log($"CreateCustomerTime Add {addNum}");
        CreateCustomerTime += addNum;
    }

    #endregion
}
