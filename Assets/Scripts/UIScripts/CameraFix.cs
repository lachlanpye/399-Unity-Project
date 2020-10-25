using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFix : MonoBehaviour
{
    [System.Serializable]
    public struct UIElement
    {
        public GameObject elementTransform;
        public Vector2 smallPosition;
        public Vector2 largePosition;
    }

    public UIElement[] uiElements;

    private int widthLimit;
    private int heightLimit;

    private bool sizeChange;
    private bool currentlySmall;

    void Start()
    {
        sizeChange = true;

        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            widthLimit = 1440;
            heightLimit = 1080;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            widthLimit = 960;
            heightLimit = 720;
        }
    }

    void Update()
    {    
        if (Screen.width >= widthLimit && Screen.height >= heightLimit)
        {
            if (currentlySmall == true)
            {
                sizeChange = true;
            }
            currentlySmall = false;
        }
        else
        {
            if (currentlySmall == false)
            {
                sizeChange = true;
            }
            currentlySmall = true;
        }

        if (sizeChange == true)
        {
            sizeChange = false;
            foreach (UIElement uiElement in uiElements)
            {
                uiElement.elementTransform.GetComponent<RectTransform>().localPosition = (currentlySmall) 
                    ? new Vector3(uiElement.smallPosition.x, uiElement.smallPosition.y, 0) 
                    : new Vector3(uiElement.largePosition.x, uiElement.largePosition.y, 0);
            }
        }
    }
}
