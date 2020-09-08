using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;

public class WorldControl : MonoBehaviour
{
    [System.Serializable]
    public struct WarpLocation
    {
        public string pointName;
        public Vector2 newPlayerPosition;
        public Vector2 newCameraPosition;
    }

    public GameObject mainCamera;
    public GameObject playerObject;

    [Space]
    public GameObject UICanvas;

    [Space]
    public WarpLocation[] scenes;

    private ShowDialogue dialogueScript;
    private List<(string, string)> dialogueList;
    private bool dialogueActive;
    private bool nextLine;

    private HealthUI healthUI;
    private GameObject pauseMenu;
    [HideInInspector]
    public bool paused;
    private bool pauseInputReset;

    private PlayerBehaviour playerBehaviour;

    private string currentScene;

    void Start()
    {
        dialogueActive = false;
        foreach (Transform t in UICanvas.transform)
        {
            if (t.gameObject.name == "DialoguePanel")
            {
                dialogueScript = t.gameObject.GetComponent<ShowDialogue>();
            }
            if (t.gameObject.name == "HealthPanel")
            {
                healthUI = t.gameObject.GetComponent<HealthUI>();
            }
            if (t.gameObject.name == "PauseMenu")
            {
                pauseMenu = t.gameObject;
                pauseMenu.SetActive(false);
                paused = false;
            }
        }

        dialogueList = new List<(string, string)>();

        playerBehaviour = playerObject.GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        if (Input.GetAxis("Pause") > 0 && pauseInputReset == true)
        {
            if (paused == false)
            {
                pauseInputReset = false;
                Pause();
            }
            else
            {
                pauseInputReset = false;
                Resume();
            }
        }
        if (Input.GetAxis("Pause") == 0)
        {
            pauseInputReset = true;
        }
    }

    public void MoveScenes(string sceneName)
    {
        WarpLocation point = new WarpLocation();

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].pointName == sceneName)
            {
                point = scenes[i];
            }
        }

        if (point.pointName == null)
        {
            Debug.LogWarning("ERROR: Warp point name not assigned in ActionZone object.");
        }
        else
        {
            // Add some kind of scene transition here
            playerObject.transform.position = new Vector3(point.newPlayerPosition.x, point.newPlayerPosition.y, 0);
            mainCamera.transform.position = new Vector3(point.newCameraPosition.x, point.newCameraPosition.y, -10);
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        paused = true;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        paused = false;
    }

    public void TakeDamage()
    {
        healthUI.HalfHealth();
    }

    public bool DialogueActive()
    {
        return dialogueActive;
    }

    public void GetNextLine()
    {
        nextLine = true;
    }

    public IEnumerator CutsceneDialogue(string xmlInput, int mode)
    {
        XmlDocument doc = new XmlDocument();
        if (mode == 0)
        {
            doc.LoadXml(xmlInput);
        }
        else if (mode == 1)
        {
            TextAsset textFile = Resources.Load<TextAsset>("Dialogue/" + xmlInput);
            doc.LoadXml(textFile.text);
        }

        foreach (XmlNode node in doc.GetElementsByTagName("line"))
        {
            string speaker = node.Attributes["speaker"].Value;
            string line = node.InnerText;

            dialogueList.Add((speaker, line));
        }
        dialogueList.Add((null, null));

        dialogueActive = true;
        nextLine = true;
        for (int i = 0; i < dialogueList.Count; i++)
        {
            while (nextLine == false)
            {
                yield return null;
            }

            dialogueScript.ShowDialogueLine(dialogueList[i]);
            nextLine = false;
        }

        dialogueActive = false;
        dialogueScript.HideDialogue();

        yield return null;
    }
}
