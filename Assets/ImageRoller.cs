using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageRoller : MonoBehaviour
{
    private Renderer spriteRenderer;
    public float rollSpeed;
    private Vector2 offset;
    void Start()
    {
        spriteRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        offset = new Vector2(0,Time.time * rollSpeed);
        spriteRenderer.material.mainTextureOffset = offset;
    }
}
