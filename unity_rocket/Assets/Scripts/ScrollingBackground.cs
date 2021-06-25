using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{

    public float speed = 0.1f;
    void Update()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material material = renderer.material;
        Vector2 offset = material.mainTextureOffset;

        offset.y += Time.deltaTime * speed;
        material.mainTextureOffset = offset;
    }

    public void SetSpeed(float x){
        speed = Mathf.Max(0.1f, Mathf.Min(2f, speed * x));
    }
}
