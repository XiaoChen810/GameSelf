using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightManager : MonoBehaviour
{
    [Header("全局光照")]
    public Light2D globalLight;

    [Header("不同时期，光照强度")]
    public float moningE; 
    public float noonE; 
    public float afternoonE;
    public float nightfallE;
    public float nightE;

    [Header("不同时间，光照颜色")]
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
        // 清晨到正午
        StartCoroutine(LightCo(moningE, noonE, arg1, arg2, moningC,noonC));
    }

    private void Instance_OnNoon(float arg1, float arg2)
    {
        // 正午到下午
        StartCoroutine(LightCo(noonE, afternoonE, arg1, arg2, noonC, afternoonC));
    }
    private void Instance_OnAfternoon(float arg1, float arg2)
    {
        // 下午到傍晚
        StartCoroutine(LightCo(afternoonE, nightfallE, arg1, arg2, afternoonC, nightfallC));
    }

    private void Instance_OnNight(float arg1, float arg2)
    {
        // 傍晚到晚上
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
