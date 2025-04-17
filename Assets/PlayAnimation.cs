using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
    public void OnEnable()
    {
        GetComponent<Animation>().Play();
    }
}
