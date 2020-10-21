using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFix : MonoBehaviour
{
    [System.Serializable]
    public struct UIElement
    {
        public string elementName;
        public Vector2 smallPosition;
        public Vector2 largePosition;

        [HideInInspector]
        public RectTransform elementTransform;
    }

    public GameObject fullUI;
    public UIElement[] uiElements;

    private bool sizeChange;
    private bool currentlySmall;

    void Start()
    {
        if (fullUI == null)
        {
            GetComponent<CameraFix>().enabled = false;
        }
        else
        {
            for (int i = 0; i < uiElements.Length; i++)
            {
                uiElements[i].elementTransform = GameObject.Find(uiElements[i].elementName).GetComponent<RectTransform>();
            }
        }

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
                uiElement.elementTransform.localPosition = (currentlySmall) 
                                                            ? new Vector3(uiElement.smallPosition.x, uiElement.smallPosition.y, 0) 
                                                            : new Vector3(uiElement.largePosition.x, uiElement.largePosition.y, 0);
            }
        }
    }
}
