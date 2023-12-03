using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("°×ÌìÒôÀÖ")]
    public AudioClip dayClip;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dayClip = Resources.Load<AudioClip>("Audio/°×ÌìÄñ½ĞÉùÒô");

        audioSource.clip = dayClip;
        audioSource.Play();
    }

}
