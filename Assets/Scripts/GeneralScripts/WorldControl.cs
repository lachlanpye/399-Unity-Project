using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;

using Enviroment;
using Forest;

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

        public bool cameraFollowPlayer;

        public WarpLocation(string name, Vector2 playerPos, Vector2 cameraPos, bool cameraFollowPlayer)
        {
            pointName = name;
            newPlayerPosition = playerPos;
            newCameraPosition = cameraPos;

            topLeftCameraBound = new Vector3();
            bottomRightCameraBound = new Vector3();

            this.cameraFollowPlayer = cameraFollowPlayer;
        }

        public WarpLocation(string name, Vector2 playerPos, Vector2 cameraPos, Vector2 topLeft, Vector2 bottomRight, bool cameraFollowPlayer)
        {
            pointName = name;
            newPlayerPosition = playerPos;
            newCameraPosition = cameraPos;

            topLeftCameraBound = topLeft;
            bottomRightCameraBound = bottomRight;

            this.cameraFollowPlayer = cameraFollowPlayer;
        }
    }

    public GameObject mainCamera;
    public GameObject playerObject;
    public GameObject saveController;

    [Space]
    public GameObject UICanvas;
    public GameObject globalLight;
    public GameObject transitionPanel;
    public GameObject[] gameOverElements;

    [Space]
    public string warpPointsFileName;
    public float fadeTransitionTime;
    public float midTransitionPause;
    [Space]
    public float startFlashEffectTime;
    public float endFlashEffectTime;

    [HideInInspector]
    public List<WarpLocation> scenes;
    [HideInInspector]
    public bool paused;
    [HideInInspector]
    public bool cameraFollowPlayer;

    private ShowDialogue dialogueScript;
    private List<(string, string)> dialogueList;
    private bool dialogueActive;
    private bool nextLine;

    private PlayerBehaviour playerBehaviour;
    private HealthUI healthUI;
    private Image transitionPanelImage;
    private SaveAndLoadGame saveAndLoad;
    private ForestSegmentLogic forestSegmentLogic;

    private GameObject pauseMenu;
    private GameObject saveMenu;
    private GameObject loadMenu;
    private GameObject settingsMenu;
    private bool pauseInputReset;

    private Vector3 topLeftCameraBound;
    private Vector3 bottomRightCameraBound;
    private Vector3 cameraPos;

    private string currentScene;

    void Awake()
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
            bool segmentCameraFollowPlayer = false;
            foreach (XmlNode node in doc.FirstChild.ChildNodes)
            {
                switch (node.Name)
                {
                    case "point":
                        string pointName = "";
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            if (bool.Parse(node.Attributes["cameraFollowPlayer"].Value) == true)
                            {
                                segmentCameraFollowPlayer = true;
                            }
                            else
                            {
                                segmentCameraFollowPlayer = false;
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
                        scenes.Add(new WarpLocation(pointName, new Vector2(playerX, playerY), new Vector2(cameraX, cameraY), new Vector2(topLeftX, topLeftY), new Vector2(bottomRightX, bottomRightY), segmentCameraFollowPlayer));
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

                    case "segment":
                        forestSegmentLogic = GetComponent<ForestSegmentLogic>();
                        forestSegmentLogic.PopulateArrays(node);
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

            UpdateBounds(new Vector3(point.newPlayerPosition.x, point.newPlayerPosition.y, 0),
                            new Vector3(point.newCameraPosition.x, point.newCameraPosition.y, -10),
                            new Vector3(point.topLeftCameraBound.x, point.topLeftCameraBound.y, -10),
                            new Vector3(point.bottomRightCameraBound.x, point.bottomRightCameraBound.y, -10));

            IEnumerator endFadeTransition = EndFadeTransition();
            StartCoroutine(endFadeTransition);
        }

        yield return null;
    }
    public void UpdateBounds(Vector3 newPlayerPos, Vector3 newCameraPos, Vector3 newTopLeftCameraBound, Vector3 newBottomRightCameraBound)
    {
        playerObject.transform.position = newPlayerPos;
        mainCamera.transform.position = newCameraPos;

        topLeftCameraBound = newTopLeftCameraBound;
        bottomRightCameraBound = newBottomRightCameraBound;
    }
    public void UpdateBounds2(Vector3 newTopLeftCameraBound, Vector3 newBottomRightCameraBound)
    {
        topLeftCameraBound = newTopLeftCameraBound;
        bottomRightCameraBound = newBottomRightCameraBound;
    }

    public IEnumerator CoroutineMoveSegments(ForestSegmentLogic.ForestSegment segment, GameObject enemyPrefab, GameObject enemyParent, string exitSide)
    {
        IEnumerator startFadeTransition = StartFadeTransition();
        yield return StartCoroutine(startFadeTransition);

        foreach (Transform t in enemyParent.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Vector2 enemySpawnPos in segment.enemySpawns)
        {
            GameObject enemy = Instantiate(enemyPrefab, enemyParent.transform, true);
            enemy.transform.position = new Vector3(enemySpawnPos.x, enemySpawnPos.y, 0);
        }

        switch (exitSide)
        {
            case "top":
                UpdateBounds(new Vector3(segment.bottomInPoint.x, segment.bottomInPoint.y, 114.9941f),
                                new Vector3(Mathf.Clamp(segment.bottomInPoint.x, segment.topLeftCameraBound.x, segment.bottomRightCameraBound.x), Mathf.Clamp(segment.bottomInPoint.y, segment.bottomRightCameraBound.y, segment.topLeftCameraBound.y), -10),
                                new Vector3(segment.topLeftCameraBound.x, segment.topLeftCameraBound.y, -10),
                                new Vector3(segment.bottomRightCameraBound.x, segment.bottomRightCameraBound.y, -10));
                break;

            case "bottom":
                UpdateBounds(new Vector3(segment.topInPoint.x, segment.topInPoint.y, 114.9941f),
                                new Vector3(Mathf.Clamp(segment.topInPoint.x, segment.topLeftCameraBound.x, segment.bottomRightCameraBound.x), Mathf.Clamp(segment.topInPoint.y, segment.bottomRightCameraBound.y, segment.topLeftCameraBound.y), -10),
                                new Vector3(segment.topLeftCameraBound.x, segment.topLeftCameraBound.y, -10),
                                new Vector3(segment.bottomRightCameraBound.x, segment.bottomRightCameraBound.y, -10));
                break;

            case "left":
                UpdateBounds(new Vector3(segment.rightInPoint.x, segment.rightInPoint.y, 114.9941f),
                                new Vector3(Mathf.Clamp(segment.rightInPoint.x, segment.topLeftCameraBound.x, segment.bottomRightCameraBound.x), Mathf.Clamp(segment.rightInPoint.y, segment.bottomRightCameraBound.y, segment.topLeftCameraBound.y), -10),
                                new Vector3(segment.topLeftCameraBound.x, segment.topLeftCameraBound.y, -10),
                                new Vector3(segment.bottomRightCameraBound.x, segment.bottomRightCameraBound.y, -10));
                break;

            case "right":
                UpdateBounds(new Vector3(segment.leftInPoint.x, segment.leftInPoint.y, 114.9941f),
                                new Vector3(Mathf.Clamp(segment.leftInPoint.x, segment.topLeftCameraBound.x, segment.bottomRightCameraBound.x), Mathf.Clamp(segment.leftInPoint.y, segment.bottomRightCameraBound.y, segment.topLeftCameraBound.y), -10),
                                new Vector3(segment.topLeftCameraBound.x, segment.topLeftCameraBound.y, -10),
                                new Vector3(segment.bottomRightCameraBound.x, segment.bottomRightCameraBound.y, -10));
                break;

        }

        IEnumerator endFadeTransition = EndFadeTransition();
        StartCoroutine(endFadeTransition);

        yield return null;
    }

    public void MoveLocations(int sceneId)
    {
        StartCoroutine(CoroutineMoveLocations(sceneId));
    }
    public IEnumerator CoroutineMoveLocations(int sceneId)
    {
        IEnumerator startFadeTransition = StartFadeTransition();
        yield return StartCoroutine(startFadeTransition);

        SceneManager.LoadScene(sceneId, LoadSceneMode.Single);

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

    public IEnumerator TakeBipedalDamage(GameObject enemy)
    {
        playerBehaviour.health++;
        playerBehaviour.canMove = false;
        enemy.GetComponent<SpriteRenderer>().enabled = false;

        if (playerBehaviour.health < 3)
        {
            Debug.Log("attack!");
            healthUI.SetHealth(playerBehaviour.health);
            StartCoroutine(playerBehaviour.PlayBipedalHurtAnimation());
            yield return new WaitForSeconds(0.667f * 2);
            playerBehaviour.canMove = false;
            yield return new WaitForSeconds(0.667f * 2);
            enemy.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (playerBehaviour.health == 3)
        {
            healthUI.SetHealth(playerBehaviour.health);
            StartCoroutine(playerBehaviour.PlayBipedalKillAnimation());
            yield return new WaitForSeconds(3);

            yield return StartCoroutine(StartFadeTransition());
            yield return new WaitForSeconds(1);
            foreach (GameObject ele in gameOverElements)
            {
                StartCoroutine(FadeInObject(ele, 20));
            }
        }

        playerBehaviour.canMove = true;
        yield return null;
    }
    public IEnumerator TakeBossDamage()
    {
        playerBehaviour.health++;
        playerBehaviour.canMove = false;

        if (playerBehaviour.health < 3)
        {
            healthUI.SetHealth(playerBehaviour.health);
            StartCoroutine(playerBehaviour.PlayBossHurtAnimation());
            yield return new WaitForSeconds(0.5f);
            playerBehaviour.canMove = false;
        }
        else if (playerBehaviour.health == 3)
        {
            Debug.Log("here");
            healthUI.SetHealth(playerBehaviour.health);
            StartCoroutine(playerBehaviour.PlayBossKillAnimation());
            yield return new WaitForSeconds(3);

            yield return StartCoroutine(StartFadeTransition());
            yield return new WaitForSeconds(1);
            foreach (GameObject ele in gameOverElements)
            {
                StartCoroutine(FadeInObject(ele, 20));
            }
        }

        playerBehaviour.canMove = true;
        yield return null;
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
    public void SetLightIntensity(float intensity)
    {
        globalLight.GetComponent<Light2D>().intensity = intensity;
    }

    public void SlowFlashEffect()
    {
        startFlashEffectTime = 60;
        endFlashEffectTime = 90;
    }
    public void SlowFadeOut()
    {
        fadeTransitionTime = fadeTransitionTime * 3;
    }

    public IEnumerator LucasFlashEffect()
    {
        for (int i = 0; i < startFlashEffectTime; i++)
        {
            transitionPanelImage.color = new Color(255, 255, 255, i / startFlashEffectTime);
            yield return new WaitForFixedUpdate();
        }

        transitionPanelImage.color = new Color(255, 255, 255, 1);

        for (int i = 0; i < endFlashEffectTime; i++)
        {
            transitionPanelImage.color = new Color(255, 255, 255, 1 - (i / endFlashEffectTime));
            yield return new WaitForFixedUpdate();
        }

        transitionPanelImage.color = new Color(255, 255, 255, 0);

        yield return null;
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
            if (dialogueScript.scrolling == true)
            {
                i--;
            }

            nextLine = false;
            dialogueScript.ShowDialogueLine(dialogueList[i]);

            while (nextLine == false)
            {
                yield return null;
            }
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

    public IEnumerator FadeInObject(GameObject gameObject, float fadeInTime)
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        for (int i = 0; i < fadeInTime; i++)
        {
            gameObject.GetComponent<Image>().color = new Color(255, 255, 255, i / fadeInTime);
            yield return new WaitForFixedUpdate();
        }
        gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
    }
}
