using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("��������")]
    public AudioClip dayClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dayClip = Resources.Load<AudioClip>("Audio/�����������");

        audioSource.clip = dayClip;
        audioSource.Play();
    }

}
