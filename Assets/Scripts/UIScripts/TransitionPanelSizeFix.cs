using UnityEngine;

public class TransitionPanelSizeFix : MonoBehaviour
{
    private RectTransform rectTransform;

    private Vector2 smallSize;
    private Vector2 largeSize;

    private int widthLimit;
    private int heightLimit;

    void Start()
    {
        rectTransform = transform as RectTransform;

        if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        {
            smallSize = new Vector2(1441, 1081);
            largeSize = new Vector2(481, 361);

            widthLimit = 1440;
            heightLimit = 1080;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            smallSize = new Vector2(961, 721);
            largeSize = new Vector2(481, 361);

            widthLimit = 960;
            heightLimit = 720;
        }
    }

    void Update()
    {
        if (Screen.width >= widthLimit && Screen.height >= heightLimit)
        {
            rectTransform.sizeDelta = smallSize;
        }
        else
        {
            rectTransform.sizeDelta = largeSize;
        }
    }
}
