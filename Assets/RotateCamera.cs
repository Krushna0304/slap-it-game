using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public Camera mainCamera;



    private void Start()
    {
        Invoke("ChangeCameraRotation", 5);
    }
    public void ChangeCameraRotation()
    {
        mainCamera.transform.Rotate(0, 0, 180);
    }

}
