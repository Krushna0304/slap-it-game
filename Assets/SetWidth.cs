using UnityEngine;

public class SetWidth : MonoBehaviour
{
    private RectTransform rt;


    void Start()
    {
      rt = GetComponent<RectTransform>();

      float w = Screen.width;
      float h = Screen.height;

      float requiredWidth = 1920 * w / h;
      rt.sizeDelta = new Vector2(requiredWidth, rt.sizeDelta.y);

    }

}
