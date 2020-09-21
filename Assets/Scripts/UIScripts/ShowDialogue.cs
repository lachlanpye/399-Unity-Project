using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowDialogue : MonoBehaviour
{
    public GameObject gameController;
    [Space]
    public float audioDelay;
    public float scrollDelay;
    [HideInInspector]
    public bool scrolling;

    private GameObject forwardButton;
    private Image panelImage;
    private TextMeshProUGUI dialogueBox;
    private WorldControl worldControl;

    private Coroutine showTextCoroutine;
    private string fullText;

    void Awake()
    {
        forwardButton = GameObject.Find("ForwardButton");
        worldControl = gameController.GetComponent<WorldControl>();

        panelImage = GetComponent<Image>();
        dialogueBox = GetComponentInChildren<TextMeshProUGUI>();

        showTextCoroutine = null;
        ShowAllElements(false);
        scrolling = false;
    }

    public void ShowDialogueLine((string, string) line)
    {
        if (showTextCoroutine != null)
        {
            StopCoroutine(showTextCoroutine);

            dialogueBox.text = fullText;
            scrolling = false;
            showTextCoroutine = null;
        }
        else
        {
            if (line.Item2 != null)
            {
                ShowAllElements(true);
                scrolling = true;

                fullText = line.Item2;
                showTextCoroutine = StartCoroutine(BeginTextScrolling());

                float totalScrollTime = scrollDelay * line.Item2.Length;
                float temp = totalScrollTime / audioDelay;
                int numOfAudioBlips = (int)temp;
            }
            else
            {
                scrolling = false;
                worldControl.GetNextLine();
            }
        }
    }

    public IEnumerator BeginTextScrolling()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            dialogueBox.text = fullText.Substring(0, i);

            yield return new WaitForSeconds(scrollDelay);
        }

        dialogueBox.text = fullText;
        scrolling = false;
        showTextCoroutine = null;

        yield return null;
    }

    public void HideDialogue()
    {
        ShowAllElements(false);
    }

    private void ShowAllElements(bool value)
    {
        panelImage.enabled = value;
        dialogueBox.gameObject.SetActive(value);
        forwardButton.SetActive(value);
    }
}
