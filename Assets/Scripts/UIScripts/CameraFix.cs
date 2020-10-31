using UnityEngine;

// Component applied to the camera to fix UI elements positioning, as the Pixel Perfect Camera component does not automatically re-position UI elements.
public class CameraFix : MonoBehaviour
{
    /// <summary>
    /// Lachlan Pye
    /// Allows the editor to set a position for specific UI elements when the game window is large or small
    /// as the Unity editor does not provide this functionality by default.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// Start function checks whether the game is being run on Mac or Windows and sets the window limits accordingly
    /// as the limits are different on Windows compared to Mac.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// Each frame, check whether the window has become larger or smaller than the window limits.
    /// If this has happened, then resize the UI elements so they are in their proper positions.
    /// </summary>
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
