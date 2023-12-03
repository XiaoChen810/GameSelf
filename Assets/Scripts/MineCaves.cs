using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MineCaves : MonoBehaviour
{
    private BoxCollider2D coll;
    private Slider workSlider;
    private Canvas canvas;

    [Header("�����ٶȣ�Խ��Խ��")]
    [Range(0f, 1f)] public float WorkSpeed;
    public bool canWork = true;
    public bool isWorking;
    public Transform workPos;

    [Header("���һ�ι����õ�����")]
    public float workResults = 2;

    [Header("���һ�ι������ĵ�����")]
    public float workEnergy = 10;

    [Header("��ѡ��")]
    public bool isSelected;

    [Header("ͼ��")]
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
