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

    private bool sizeChange;
    private bool currentlySmall;

    void Start()
    {
        sizeChange = true;
    }

    void Update()
    {    
        if (Screen.width >= 960 && Screen.height >= 720)
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
