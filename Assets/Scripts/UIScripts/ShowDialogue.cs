using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public IEnumerator BeginTextScrolling()
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

    public void HideDialogue()
    {
        ShowAllElements(false);
    }

    private void ShowAllElements(bool value)
    {
        bodyImage.enabled = value;
        titleImage.enabled = value;
        bodyText.gameObject.SetActive(value);
        titleText.gameObject.SetActive(value);
    }

    public IEnumerator PlayDialogueBlips (AudioClip clip, int numBlips)
    {
        for (int i = 0; i <= numBlips; i++)
        {
            AudioManager.publicInstance.PlayDialogue(clip);
            yield return new WaitForSeconds(clip.length);
        }


        yield return null;
    }
}
