using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<Animator>().SetTrigger("isActivate");
    }

}
