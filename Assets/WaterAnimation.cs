using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAnimation : MonoBehaviour
{
    public float speed;
    private float x, y;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
        x += speed * Time.deltaTime;
        y += speed * Time.deltaTime;
        GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(x, y);
    }
}
