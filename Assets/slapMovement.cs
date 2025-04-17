using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class slapMovement : MonoBehaviour
{
    // Start is called before the first frame update

    
    private bool flag ;
    private Transform targetTransform;
    private Transform handTransform;

    public Transform leftHandTransform;
    public Transform rightHandTransform;

    public Transform idealLeftTargetTransform;
    public Transform idealRightTargetTransform;

    private void Start()
    {
        flag = true;
    }
    public void OnSlapButtonClicked(Button button)
    {
        if (!flag)
        {
            Debug.Log("Bhai Chitting koi krta hai");
            return;
        }

        flag = false;
        GetComponent<IdealMotion>().enabled = false;
        if(button.name == "Left")
        {
            handTransform = leftHandTransform;
            targetTransform = idealLeftTargetTransform;
        }
        else
        {
            handTransform = rightHandTransform;
            targetTransform = idealRightTargetTransform;
        }
        StartCoroutine(SlapMotion());
    }

    IEnumerator SlapMotion()
    {
        Vector3 initialPos = handTransform.position;
        Quaternion initialRos = handTransform.rotation;
        
        float count = 0;
        while (count <= 10)
        {
           
            handTransform.rotation = Quaternion.Slerp(initialRos, targetTransform.rotation, (float)(count / (float)10));
            handTransform.position = Vector3.Lerp(initialPos,targetTransform.position,(float)(count/(float)10));
            yield return new WaitForSeconds(.02f);
            count++;
        }

        count--;
        while (count >= 0)
        {
            handTransform.position = Vector3.Lerp(initialPos, targetTransform.position, (float)(count / (float)10));
            handTransform.rotation = Quaternion.Slerp(initialRos, targetTransform.rotation, (float)(count / (float)10));
            yield return new WaitForSeconds(.04f);
            count--;
        }

        GetComponent<IdealMotion>().enabled = true;
        flag = true;
    }
    
}
