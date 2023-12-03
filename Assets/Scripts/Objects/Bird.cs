using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private Transform m_trans;
    [Header("目标点")]
    public Transform targetPos0;
    public Transform targetPos1;
    public Transform targetPos2;

    [Header("贝塞尔曲线的控制点")]
    public Transform controlPoint1;
    public Transform controlPoint2;

    private Vector2 t0, t1, t2, c1, c2;

    public float duration = 2f; // 移动的总时间
    public float clockTime = 0;
    public float waitTime = 3;
    public float resetTime = 5;

    private Animator m_anim;

    public enum State
    {
        ready,
        come,
        wait,
        nextWait,
        leave
    }

    public State state;

    private void Start()
    {
        m_trans = transform;
        m_anim = GetComponent<Animator>();
        state = State.ready;

        t0 = targetPos0.position;
        t1 = targetPos1.position;
        t2 = targetPos2.position;
        c1 = controlPoint1.position; 
        c2 = controlPoint2.position;

        m_trans.position = t0;
    }

    private void Update()
    {
        switch (state)
        {
            case State.ready:
                {
                    m_anim.SetBool("Flying", true);
                    FlyToTarget(t0, c1, t1);
                    state = State.come;
                    break;
                }
            case State.come:
                {
                    if (clockTime < duration)
                    {
                        clockTime += Time.deltaTime;
                    }
                    else
                    {
                        m_anim.SetBool("Flying", false);
                        m_anim.SetTrigger("Wait");
                        clockTime = 0;

                        state = State.wait;
                    }
                    break;
                }
            case State.wait:
                {
                    if (clockTime < waitTime)
                    {
                        clockTime += Time.deltaTime;
                    }
                    else
                    {
                        if (WaitChoose(50))
                        {
                            clockTime = 0;
                            state = State.nextWait;
                        }
                        else
                        {
                            m_anim.SetBool("Flying", true);
                            FlyToTarget(t1, c2, t2);
                            clockTime = 0;
                            state = State.leave;
                        }

                    }
                    break;
                }
            case State.nextWait:
                {
                    m_anim.SetBool("Flying", false);
                    m_anim.SetTrigger("Wait");
                    state = State.wait;
                    break;
                }
            case State.leave:
                {
                    if (clockTime < duration + resetTime)
                    {
                        clockTime += Time.deltaTime;
                    }
                    else
                    {
                        m_trans.position = targetPos0.position;
                        state = State.ready;
                        clockTime = 0;
                    }
                    break;
                }
        }
    }

    private bool WaitChoose(int gailv)
    {
        int r = Random.Range(0, 100);
        if(gailv > r)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void FlyToTarget(Vector2 target0, Vector2 control, Vector2 target)
    {
        StartCoroutine(MoveCo(target0, control, target));
    }

    IEnumerator MoveCo(Vector2 target0, Vector2 control, Vector2 target)
    {
        float time = 0;
        while (true)
        {
            yield return null;
            if(time < duration)
            {
                time += Time.deltaTime;

                float t = time / duration;
                //Vector2 currentPosition = Bezier(me.position, control.position, target.position, t);
                Vector2 currentPosition = CalculateBezierPoint(t,target0,control,target);

                m_trans.position = currentPosition;
            }
            else
            {
                break;
            }
        }
    }

    // 贝塞尔曲线公式计算插值点
    Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

}
