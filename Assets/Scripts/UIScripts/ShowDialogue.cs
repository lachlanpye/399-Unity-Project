using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Component used to create the character scrolling effect and blip noise effect in the dialogue UI panel.
public class ShowDialogue : MonoBehaviour
{
    public GameObject gameController;
    public GameObject dialogueBody;
    public GameObject dialogueTitle;

    [Space]
    public float audioDelay;
    public float scrollDelay;
    [HideInInspector]
    public bool scrolling;

    private Image bodyImage;
    private Image titleImage;
    private TextMeshProUGUI bodyText;
    private TextMeshProUGUI titleText;
    private WorldControl worldControl;

    private Coroutine showTextCoroutine;
    private Coroutine playBlipsCoroutine;
    private string fullText;

    private Dictionary<string, string> properNames;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Awake()
    {
        worldControl = gameController.GetComponent<WorldControl>();

        bodyImage = dialogueBody.GetComponent<Image>();
        titleImage = dialogueTitle.GetComponent<Image>();
        bodyText = dialogueBody.GetComponentInChildren<TextMeshProUGUI>();
        titleText = dialogueTitle.GetComponentInChildren<TextMeshProUGUI>();

        showTextCoroutine = null;
        ShowAllElements(false);
        scrolling = false;

        properNames = new Dictionary<string, string>()
        {
            { "father", "Ray" },
            { "son", "Lucas" },
            { "molly", "Molly" },
            { "alfred", "Alfred" },
            { "player", "" },
            { "cat", "Cat" },
            { "francine", "Francine" }
        };
    }

    /// <summary>
    /// Lachlan Pye
    /// Called when the next line of text needs to be shown scrolling and playing audio blips in the dialogue UI box.
    /// If there is already text scrolling through, then complete the line currently being shown.
    /// Else, begin scrolling the text and playing audio blips.
    /// </summary>
    /// <param name="line">A string tuple containing the next dialogue line and dialogue speaker to be shown.</param>
    public void ShowDialogueLine((string, string) line)
    {
        if (showTextCoroutine != null)
        {
            StopCoroutine(showTextCoroutine);
            StopCoroutine(playBlipsCoroutine);

            bodyText.text = fullText;
            scrolling = false;
            showTextCoroutine = null;
        }
        else
        {
            if (line.Item2 != null)
            {
                ShowAllElements(true);
                scrolling = true;

                titleText.text = properNames[line.Item1];
                fullText = line.Item2;
                showTextCoroutine = StartCoroutine(BeginTextScrolling());

                AudioClip speaker = Resources.Load<AudioClip>("Audio/" + line.Item1);
                audioDelay = speaker.length;

                float totalScrollTime = scrollDelay * line.Item2.Length;
                float temp = totalScrollTime / audioDelay;
                int numOfAudioBlips = (int) Mathf.Ceil(temp);

                playBlipsCoroutine = StartCoroutine(PlayDialogueBlips(speaker, numOfAudioBlips));
            }
            else
            {
                scrolling = false;
                worldControl.GetNextLine();
            }
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// Scroll the current line of text by adding one character, waiting a short time, and then adding the next until
    /// the full line is completed.
    /// </summary>
    private IEnumerator BeginTextScrolling()
    {
        for (int i = 0; i < fullText.Length; i++)
        {
            bodyText.text = fullText.Substring(0, i);

            yield return new WaitForSeconds(scrollDelay);
        }

        bodyText.text = fullText;
        scrolling = false;
        showTextCoroutine = null;

        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Hide the dialogue box once the dialogue script has been completed.
    /// </summary>
    public void HideDialogue()
    {
        ShowAllElements(false);
    }


    /// <summary>
    /// Lachlan Pye
    /// Set the dialogue UI box to be shown or hidden.
    /// </summary>
    /// <param name="value">Whether the UI elements should be shown or not.</param>
    private void ShowAllElements(bool value)
    {
        bodyImage.enabled = value;
        titleImage.enabled = value;
        bodyText.gameObject.SetActive(value);
        titleText.gameObject.SetActive(value);
    }

    /// <summary>
    /// Janine Aunzo
    /// Play dialogue blips.
    /// </summary>
    /// <param name="clip">Audio clip of the current speaker.</param>
    /// <param name="numBlips">Number of dialogue blips to play.</param>
    private IEnumerator PlayDialogueBlips (AudioClip clip, int numBlips)
    {
        for (int i = 0; i <= numBlips; i++)
        {
            AudioManager.publicInstance.PlayDialogue(clip);
            yield return new WaitForSeconds(clip.length);
        }

        yield return null;
    }
}
