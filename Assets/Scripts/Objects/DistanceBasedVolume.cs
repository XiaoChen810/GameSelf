using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBasedVolume : MonoBehaviour
{
    public Transform target;  // Ŀ������
    public float minDistance = 2f;  // �����Ŀ��֮�����С����
    public float maxDistance = 20f;  // �����Ŀ��֮���������
    public float minVolume = 0.1f;  // ��С����
    public float maxVolume = 1f;  // �������

    private Camera mainCamera;
    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // ���������Ŀ��֮��ľ���
        float distance = Mathf.Abs(mainCamera.transform.position.x - target.position.x);

        // ������ӳ�䵽������Χ
        float normalizedDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        normalizedDistance = Mathf.Clamp01((maxDistance - distance) / (maxDistance - minDistance));

        float targetVolume = Mathf.Lerp(minVolume, maxVolume, normalizedDistance);

        // ��������
        audioSource.volume = targetVolume;

    }
}
