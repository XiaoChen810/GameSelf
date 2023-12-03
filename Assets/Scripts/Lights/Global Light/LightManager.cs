using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [Header("ȫ�ֹ���")]
    public Light2D globalLight;

    [Header("��ͬʱ�ڣ�����ǿ��")]
    public float moningE; 
    public float noonE; 
    public float afternoonE;
    public float nightfallE;
    public float nightE;

    [Header("��ͬʱ�䣬������ɫ")]
    public Color moningC;
    public Color noonC;
    public Color afternoonC;
    public Color nightfallC;
    public Color nightC;

    private void OnEnable()
    {
        Statistics.Instance.OnMoning += Instance_OnMoning;
        Statistics.Instance.OnNoon += Instance_OnNoon;
        Statistics.Instance.OnAfternoon += Instance_OnAfternoon;
        Statistics.Instance.OnNight += Instance_OnNight;
        Statistics.Instance.OnDayDark += Instance_OnDayDark;
    }

    private void Instance_OnMoning(float arg1, float arg2)
    {
        // �峿������
        StartCoroutine(LightCo(moningE, noonE, arg1, arg2, moningC,noonC));
    }

    private void Instance_OnNoon(float arg1, float arg2)
    {
        // ���絽����
        StartCoroutine(LightCo(noonE, afternoonE, arg1, arg2, noonC, afternoonC));
    }
    private void Instance_OnAfternoon(float arg1, float arg2)
    {
        // ���絽����
        StartCoroutine(LightCo(afternoonE, nightfallE, arg1, arg2, afternoonC, nightfallC));
    }

    private void Instance_OnNight(float arg1, float arg2)
    {
        // ��������
        StartCoroutine(LightCo(nightfallE, nightE, arg1, arg2,nightfallC, nightC));
    }

    private void Instance_OnDayDark()
    {
        globalLight.color = moningC;
    }

    private void OnDestroy()
    {
        Statistics.Instance.OnMoning += Instance_OnMoning;
        Statistics.Instance.OnNoon += Instance_OnNoon;
        Statistics.Instance.OnAfternoon += Instance_OnAfternoon;
        Statistics.Instance.OnNight += Instance_OnNight;
    }

    private IEnumerator LightCo(float minIntensity, float maxIntensity, float start, float end, Color color1,Color color2)
    {
        globalLight.color = color1;
        while (Statistics.Instance.Second >= start && Statistics.Instance.Second < end)
        {
            yield return null;
            float t = Mathf.Clamp01((Statistics.Instance.Second - start) / (end - start));
            if (t >= 0.6f)
            {
                globalLight.color = color2;
            }
            globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }

    }
}
