using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdealMotion : MonoBehaviour
{
    public float min;
    public float max;
    public float speed;
    private float value;

    private void Start()
    {
        value = speed;
    }

    void Update()
    {
       
        if(transform.position.z >= max)
        {
            value = speed;
        }

        if(transform.position.z <= min)
        {
            value = -speed;
        }
        transform.Translate(0,0, value * Time.deltaTime);
    }
}
