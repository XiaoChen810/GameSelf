using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HighLight : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Material OutlineMaterial;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Shader shader = Shader.Find("Shader Graphs/Outline");
        OutlineMaterial = new Material(shader);
        spriteRenderer.sharedMaterial = OutlineMaterial;
    }

    public void StartHighLight()
    {

        OutlineMaterial.color = new Color(1f, 0.4f, 0f, 1f);
    }

    public void EndHighLight()
    {

        OutlineMaterial.color = Color.black;
    }
}
