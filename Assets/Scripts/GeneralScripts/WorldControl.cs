using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;
using UnityEngine.UI;

using Enviroment;

public class WorldControl : MonoBehaviour
{
    [System.Serializable]
    public struct WarpLocation
    {
        public string pointName;
        public Vector2 newPlayerPosition;
        public Vector2 newCameraPosition;

        public Vector2 topLeftCameraBound;
        public Vector2 bottomRightCameraBound;

        public WarpLocation(string name, Vector2 playerPos, Vector2 cameraPos)
        {
            pointName = name;
            newPlayerPosition = playerPos;
            newCameraPosition = cameraPos;

            topLeftCameraBound = new Vector3();
            bottomRightCameraBound = new Vector3();
        }

        public WarpLocation(string name, Vector2 playerPos, Vector2 cameraPos, Vector2 topLeft, Vector2 bottomRight)
        {
            pointName = name;
            newPlayerPosition = playerPos;
            newCameraPosition = cameraPos;

            topLeftCameraBound = topLeft;
            bottomRightCameraBound = bottomRight;
        }
    }

    public GameObject mainCamera;
    public GameObject playerObject;
    public GameObject saveController;

    [Space]
    public GameObject UICanvas;
    public GameObject transitionPanel;

    [Space]
    public string warpPointsFileName;
    public float fadeTransitionTime;
    public float midTransitionPause;

    [HideInInspector]
    public List<WarpLocation> scenes;
    [HideInInspector]
    public bool paused;
    [HideInInspector]
    public int storyPosition;

    private ShowDialogue dialogueScript;
    private List<(string, string)> dialogueList;
    private bool dialogueActive;
    private bool nextLine;

    private HealthUI healthUI;
    private Image transitionPanelImage;
    private SaveAndLoadGame saveAndLoad;

    private GameObject pauseMenu;
    private GameObject saveMenu;
    private GameObject loadMenu;
    private GameObject settingsMenu;
    private bool pauseInputReset;

    private bool cameraFollowPlayer;
    private Vector3 topLeftCameraBound;
    private Vector3 bottomRightCameraBound;
    private Vector3 cameraPos;
    
    private PlayerBehaviour playerBehaviour;

    private string currentScene;

    void Start()
    {
        dialogueActive = false;
        dialogueList = new List<(string, string)>();
        if (UICanvas != null)
        {
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
                if (t.gameObject.name == "SaveMenu")
                {
                    saveMenu = t.gameObject;
                    saveMenu.SetActive(false);
                }
                if (t.gameObject.name == "LoadMenu")
                {
                    loadMenu = t.gameObject;
                    loadMenu.SetActive(false);
                }
                if (t.gameObject.name == "SettingsMenu")
                {
                    settingsMenu = t.gameObject;
                    settingsMenu.SetActive(false);
                }
            }
        }

        paused = false;
        cameraFollowPlayer = false;
        saveAndLoad = saveController.GetComponent<SaveAndLoadGame>();

        if (playerObject != null)
        {
            playerBehaviour = playerObject.GetComponent<PlayerBehaviour>();
        }

        if (warpPointsFileName != "")
        {
            XmlDocument doc = new XmlDocument();
            TextAsset textFile = Resources.Load<TextAsset>("Data/" + warpPointsFileName);
            doc.LoadXml(textFile.text);

            scenes = new List<WarpLocation>();
            float playerX = 0, playerY = 0, cameraX = 0, cameraY = 0, topLeftX = 0, topLeftY = 0, bottomRightX = 0, bottomRightY = 0;
            foreach (XmlNode node in doc.FirstChild.ChildNodes)
            {
                switch (node.Name) {
                    case "point":
                        string pointName = "";
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            if (bool.Parse(node.Attributes["cameraFollowPlayer"].Value) == true)
                            {
                                cameraFollowPlayer = true;
                            } else
                            {
                                cameraFollowPlayer = false;
                            }

                            XmlNode childNode = node.ChildNodes[i];
                            switch (childNode.Name)
                            {
                                case "name":
                                    pointName = childNode.InnerText;
                                    break;
                                case "playerX":
                                    playerX = float.Parse(childNode.InnerText);
                                    break;
                                case "playerY":
                                    playerY = float.Parse(childNode.InnerText);
                                    break;
                                case "cameraX":
                                    cameraX = float.Parse(childNode.InnerText);
                                    break;
                                case "cameraY":
                                    cameraY = float.Parse(childNode.InnerText);
                                    break;  
                                case "topLeftBoundX":
                                    topLeftX = float.Parse(childNode.InnerText);
                                    break;
                                case "topLeftBoundY":
                                    topLeftY = float.Parse(childNode.InnerText);
                                    break;
                                case "bottomRightBoundX":
                                    bottomRightX = float.Parse(childNode.InnerText);
                                    break;
                                case "bottomRightBoundY":
                                    bottomRightY = float.Parse(childNode.InnerText);
                                    break;
                                default:
                                    break;
                            }
                        }
                        scenes.Add(new WarpLocation(pointName, new Vector2(playerX, playerY), new Vector2(cameraX, cameraY), new Vector2(topLeftX, topLeftY), new Vector2(bottomRightX, bottomRightY)));
                        break;
                    case "default":
                        if (bool.Parse(node.Attributes["cameraFollowPlayer"].Value) == true)
                        {
                            cameraFollowPlayer = true;
                            for (int i = 0; i < node.ChildNodes.Count; i++)
                            {
                                XmlNode childNode = node.ChildNodes[i];
                                switch (childNode.Name)
                                {
                                    case "topLeftBoundX":
                                        topLeftX = float.Parse(childNode.InnerText);
                                        break;
                                    case "topLeftBoundY":
                                        topLeftY = float.Parse(childNode.InnerText);
                                        break;
                                    case "bottomRightBoundX":
                                        bottomRightX = float.Parse(childNode.InnerText);
                                        break;
                                    case "bottomRightBoundY":
                                        bottomRightY = float.Parse(childNode.InnerText);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            topLeftCameraBound = new Vector3(topLeftX, topLeftY, -10);
                            bottomRightCameraBound = new Vector3(bottomRightX, bottomRightY, -10);
                        }
                        else
                        {
                            cameraFollowPlayer = false;
                        }
                        break;
                }
            }
        }

        if (transitionPanel != null)
        {
            transitionPanelImage = transitionPanel.GetComponent<Image>();
        }

        StartCoroutine(EndFadeTransition());
    }

    public void UpdateCameraIfFollowing()
    {
        if (cameraFollowPlayer)
        {
            cameraPos = new Vector3(Mathf.Clamp(playerBehaviour.transform.position.x, topLeftCameraBound.x, bottomRightCameraBound.x), Mathf.Clamp(playerBehaviour.transform.position.y, bottomRightCameraBound.y, topLeftCameraBound.y), -10);
            mainCamera.transform.position = cameraPos;
        }
    }

    public void MoveScenes(string sceneName)
    {
        IEnumerator coroutineMoveScenes = CoroutineMoveScenes(sceneName);
        StartCoroutine(coroutineMoveScenes);
    }
    public IEnumerator CoroutineMoveScenes(string sceneName)
    {
        WarpLocation point = new WarpLocation();

        for (int i = 0; i < scenes.Count; i++)
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
            IEnumerator startFadeTransition = StartFadeTransition();
            yield return StartCoroutine(startFadeTransition);

            // Coroutine returns at mid point, so move the player and camera now
            playerObject.transform.position = new Vector3(point.newPlayerPosition.x, point.newPlayerPosition.y, 0);
            mainCamera.transform.position = new Vector3(point.newCameraPosition.x, point.newCameraPosition.y, -10);

            topLeftCameraBound = new Vector3(point.topLeftCameraBound.x, point.topLeftCameraBound.y, -10);
            bottomRightCameraBound = new Vector3(point.bottomRightCameraBound.x, point.bottomRightCameraBound.y, -10);

            IEnumerator endFadeTransition = EndFadeTransition();
            StartCoroutine(endFadeTransition);
        }

        yield return null;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        paused = true;
    }

    public void Resume()
    {
        if (pauseMenu != null) { pauseMenu.SetActive(false); }
        if (saveMenu != null) { saveMenu.SetActive(false); }
        if (loadMenu != null) { loadMenu.SetActive(false); }
        if (settingsMenu != null) { settingsMenu.SetActive(false); }
        paused = false;
    }

    public void Save(string savePoint)
    {
        saveAndLoad.currentSavingPoint = savePoint;
        saveMenu.SetActive(true);
        paused = true;
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

    public void SwitchToDayOrNight(string value)
    {
        DayOrNightObjects dayOrNightObjects = GameObject.Find("SceneObjects").GetComponent<DayOrNightObjects>();
        if (value == "day")
        {
            dayOrNightObjects.currentlyDay = true;
        } 
        else if (value == "night")
        {
            dayOrNightObjects.currentlyDay = false;
        }
    }

    public void CutsceneDialogueFunction(string xmlInput)
    {
        StartCoroutine(CutsceneDialogue(xmlInput, 1));
    }
    public IEnumerator CutsceneDialogue(string xmlInput, int mode)
    {
        dialogueList = new List<(string, string)>();
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

    public IEnumerator StartFadeTransition()
    {
        paused = true;
        for (int i = 0; i < fadeTransitionTime; i++)
        {
            transitionPanelImage.color = new Color(0, 0, 0, i / fadeTransitionTime);
            yield return new WaitForFixedUpdate();
        }

        transitionPanelImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(midTransitionPause);

        yield return null;
    }
    public IEnumerator EndFadeTransition()
    {
        for (int i = 0; i < fadeTransitionTime; i++)
        {
            transitionPanelImage.color = new Color(0, 0, 0, 1 - (i / fadeTransitionTime));
            yield return new WaitForFixedUpdate();
        }

        transitionPanelImage.color = new Color(0, 0, 0, 0);
        paused = false;

        yield return null;
    }
}
