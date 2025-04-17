using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustToSafeArea : MonoBehaviour
{
    Rect safeArea;
    public RectTransform container;
    private Rect previousSafeArea;
    private void Start()
    {
        container = GetComponent<RectTransform>();
        safeArea = Screen.safeArea;

        if (Screen.safeArea != previousSafeArea)
        {
            UpdateAnchors();
        }
    }

    private void Update()
    {
        if (Screen.safeArea != previousSafeArea)
        {
            UpdateAnchors();
        }
    }

    void UpdateAnchors()
    {

        Vector2 anchorMin = Screen.safeArea.position;

        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x/=Screen.width;
        anchorMax.x/=Screen.width;
        anchorMin.y/=Screen.height;
        anchorMax.y/=Screen.height;

        container.anchorMin = anchorMin;
        container.anchorMax = anchorMax;

        previousSafeArea  = Screen.safeArea;
    }
}
