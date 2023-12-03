using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceBasedVolume : MonoBehaviour
{
    public Transform target;  // 目标物体
    public float minDistance = 2f;  // 相机和目标之间的最小距离
    public float maxDistance = 20f;  // 相机和目标之间的最大距离
    public float minVolume = 0.1f;  // 最小音量
    public float maxVolume = 1f;  // 最大音量

    private Camera mainCamera;
    private AudioSource audioSource;

    void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 计算相机和目标之间的距离
        float distance = Mathf.Abs(mainCamera.transform.position.x - target.position.x);

        // 将距离映射到音量范围
        float normalizedDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        normalizedDistance = Mathf.Clamp01((maxDistance - distance) / (maxDistance - minDistance));

        float targetVolume = Mathf.Lerp(minVolume, maxVolume, normalizedDistance);

        // 设置音量
        audioSource.volume = targetVolume;

    }
}
