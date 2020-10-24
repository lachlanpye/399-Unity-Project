using UnityEngine;

public class TransitionPanelSizeFix : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 smallSize;
    private Vector2 largeSize;

    void Start()
    {
        rectTransform = transform as RectTransform;
        smallSize = new Vector2(961, 721);
        largeSize = new Vector2(481, 361);
    }

    void Update()
    {
        if (Screen.width >= 960 && Screen.height >= 720)
        {
            rectTransform.sizeDelta = smallSize;
        }
        else
        {
            rectTransform.sizeDelta = largeSize;
        }
    }
}
