using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineCaves : MonoBehaviour
{
    private BoxCollider2D coll;
    private Slider workSlider;
    private Canvas canvas;

    [Header("工作速度，越大越慢")]
    [Range(0f, 1f)] public float WorkSpeed;
    public bool canWork = true;
    public bool isWorking;
    public Transform workPos;

    [Header("完成一次工作得到的铁")]
    public float workResults = 2;

    [Header("完成一次工作消耗的体力")]
    public float workEnergy = 10;

    [Header("被选中")]
    public bool isSelected;

    [Header("图像")]
    public List<SpriteRenderer> srList;

    private void Awake()
    {
        coll = GetComponent<BoxCollider2D>();
        workSlider = GetComponentInChildren<Slider>();
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;

    }

    private void Update()
    {
        if (isSelected)
        {

            float distance = Mathf.Abs(Player.Instance.transform.position.x - workPos.transform.position.x);

            if (distance < 0.1f)
            {
                isWorking = true;
                foreach (SpriteRenderer r in srList)
                {
                    r.sortingOrder = 100;
                }
            }

            if (Player.Instance.state != Player.State.ToMineCaves)
            {
                isWorking = false;
                isSelected = false;
                foreach (SpriteRenderer r in srList)
                {
                    r.sortingOrder = 0;
                }
            }

            if (!canWork)
            {
                isWorking = false;
                isSelected = false;
                foreach (SpriteRenderer r in srList)
                {
                    r.sortingOrder = 0;
                }
                Player.Instance.state = Player.State.None;
            }

            if (isWorking)
            {
                if (workSlider.value <= 0)
                {
                    workSlider.value = 1;
                    Statistics.Instance.Iron += workResults;
                    Statistics.Instance.Energy -= workEnergy;
                }
                workSlider.value -= WorkSpeed * Time.deltaTime;
            }
        }
    }

    private void OnMouseDown()
    {
        if(canWork)
        {
            isSelected = true;
            if (Player.Instance.state != Player.State.ToMineCaves)
            {
                Player.Instance.state = Player.State.ToMineCaves;
                Player.Instance.m_targetPos = workPos.position;
            }
        }

    }

}
