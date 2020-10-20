using UnityEngine;

public class CameraFix : MonoBehaviour
{
    [System.Serializable]
    public struct UIElement
    {
        public RectTransform elementTransform;
        public Vector2 smallPosition;
        public Vector2 largePosition; 
    }

    public GameObject fullUI;
    public UIElement[] uiElements;

    private bool currentlySmall;
    private Vector2 resolution;

    void Start()
    {
        if (fullUI == null)
        {
            GetComponent<CameraFix>().enabled = false;
        }

        resolution = new Vector2(Screen.width, Screen.height);
    }

    void Update()
    {
        if (resolution.x != Screen.width || resolution.y != Screen.height)
        {
            if (Screen.width >= 960 && Screen.height >= 720)
            {
                currentlySmall = false;
            }
            else
            {
                currentlySmall = true;
            }

            foreach (UIElement uiElement in uiElements)
            {
                uiElement.elementTransform.localPosition = (currentlySmall)
                                                            ? new Vector3(uiElement.smallPosition.x, uiElement.smallPosition.y, 0)
                                                            : new Vector3(uiElement.largePosition.x, uiElement.largePosition.y, 0);
            }

            resolution = new Vector2(Screen.width, Screen.height);
        }
    }
}
