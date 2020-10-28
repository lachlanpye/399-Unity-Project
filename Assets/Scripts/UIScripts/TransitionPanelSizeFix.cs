using UnityEngine;

// Component used to control the size of a game object depending on how large or small the game window is.
public class TransitionPanelSizeFix : MonoBehaviour
{
    private RectTransform rectTransform;

    private Vector2 smallSize;
    private Vector2 largeSize;

    private int widthLimit;
    private int heightLimit;

    /// <summary>
    /// Lachlan Pye
    /// Check whether the game is played on a Mac or Windows computer, and initalize variables accordingly.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// If the game window is larger than the limit, then resize this game object.
    /// </summary>
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
