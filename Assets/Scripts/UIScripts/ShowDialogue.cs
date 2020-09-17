using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowDialogue : MonoBehaviour
{
    private GameObject forwardButton;

    private Image panelImage;
    private TextMeshProUGUI dialogueBox;

    void Awake()
    {
        Debug.Log("here");
        forwardButton = GameObject.Find("ForwardButton");

        panelImage = GetComponent<Image>();
        dialogueBox = GetComponentInChildren<TextMeshProUGUI>();

        ShowAllElements(false);
    }

    public void ShowDialogueLine((string, string) line)
    {
        ShowAllElements(true);
        dialogueBox.text = line.Item2;
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
