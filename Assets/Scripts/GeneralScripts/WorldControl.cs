using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Experimental.Rendering.Universal;

using Enviroment;
using Forest;

// Component that controls various world settings.
public class WorldControl : MonoBehaviour
{
    /// <summary>
    /// Lachlan Pye
    /// Struct that stores a warp location, used to move the player and camera between areas of the scene.
    /// </summary>
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
    [Space]
    public GameOverAudio gameOverAudio;
    public GameObject gameOverUI;

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
    public bool dialogueActive;
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

    private const int SLOW_START_FADE_TIME = 60;
    private const int SLOW_END_FADE_TIME = 120;

    private string currentScene;
    public bool playerIsInvulnerable;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// Also loads the data file for this scene, used to store warp points.
    /// If there is a ForestSegment component attached, it will also load the segments stored in the file.
    /// Finally, it fades out the transition panel and shows the scene.
    /// </summary>
    void Awake()
    {
        dialogueActive = false;
        dialogueList = new List<(string, string)>();
        playerIsInvulnerable = false;

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
            if (playerBehaviour.flashlightActiveTime != 0)
            {
                healthUI.gameObject.SetActive(true);
            }
            else
            {
                healthUI.gameObject.SetActive(false);
            }
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

    /// <summary>
    /// Lachlan Pye
    /// If the camera is set to follow the player, then update the camera such that it is at the player's position, clamped between the top left and bottom right vector bounds.
    /// </summary>
    public void UpdateCameraIfFollowing()
    {
        if (cameraFollowPlayer)
        {
            cameraPos = new Vector3(Mathf.Clamp(playerBehaviour.transform.position.x, topLeftCameraBound.x, bottomRightCameraBound.x), Mathf.Clamp(playerBehaviour.transform.position.y, bottomRightCameraBound.y, topLeftCameraBound.y), -10);
            mainCamera.transform.position = cameraPos;
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// Start the CoroutineMoveScenes coroutine, used to move the player between areas of the scene.
    /// </summary>
    /// <param name="sceneName">The name of the warp point the player should be moved to.</param>
    public void MoveScenes(string sceneName)
    {
        IEnumerator coroutineMoveScenes = CoroutineMoveScenes(sceneName);
        StartCoroutine(coroutineMoveScenes);
    }
    /// <summary>
    /// Lachlan Pye
    /// Searches for the warp point specified by the sceneName parameter. It then fades out, moves the player and camera, and then fades back in at the new location.
    /// </summary>
    /// <param name="sceneName">The name of the warp point the player should be moved to.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Lachlan Pye
    /// Update the player's position, the camera's position, the top left camera bound and the bottom right camera bound.
    /// </summary>
    /// <param name="newPlayerPos">The player's new position.</param>
    /// <param name="newCameraPos">The camera's new position.</param>
    /// <param name="newTopLeftCameraBound">The new top left camera bound.</param>
    /// <param name="newBottomRightCameraBound">The new bottom right camera bound.</param>
    public void UpdateBounds(Vector3 newPlayerPos, Vector3 newCameraPos, Vector3 newTopLeftCameraBound, Vector3 newBottomRightCameraBound)
    {
        playerObject.transform.position = newPlayerPos;
        mainCamera.transform.position = newCameraPos;

        topLeftCameraBound = newTopLeftCameraBound;
        bottomRightCameraBound = newBottomRightCameraBound;
    }
    /// <summary>
    /// Lachlan Pye
    /// Update the top left camera bound and bottom right camera bound only.
    /// </summary>
    /// <param name="newTopLeftCameraBound">The new top left camera bound.</param>
    /// <param name="newBottomRightCameraBound">The new bottom right camera bound.</param>
    public void UpdateBoundsOnly(Vector3 newTopLeftCameraBound, Vector3 newBottomRightCameraBound)
    {
        topLeftCameraBound = newTopLeftCameraBound;
        bottomRightCameraBound = newBottomRightCameraBound;
    }

    /// <summary>
    /// Lachlan Pye
    /// Used exclusively in the Forest scene. Fade out the scene, destroy all enemies currently active, and then respawn them in the new segment.
    /// Depending on the exit side, move the player to the opposite side of the new segment. Then, fade the scene back in.
    /// </summary>
    /// <param name="segment">The segment to be used to derive the new segment information from.</param>
    /// <param name="enemyPrefab">The enemy gameobject prefab.</param>
    /// <param name="enemyParent">The gameobject under which the enemies will be spawned under.</param>
    /// <param name="exitSide">The side from which the player exited the last segment from.</param>
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
            enemy.transform.position = new Vector3(enemySpawnPos.x, enemySpawnPos.y, 115);
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

    /// <summary>
    /// Lachlan Pye
    /// Start the CoroutineMoveLocations coroutine.
    /// </summary>
    /// <param name="sceneId">The sceneId to be passed to the coroutine.</param>
    public void MoveLocations(int sceneId)
    {
        StartCoroutine(CoroutineMoveLocations(sceneId));
    }
    /// <summary>
    /// Fades out the scene and loads the next scene.
    /// </summary>
    /// <param name="sceneId">The id of the scene to be loaded.</param>
    public IEnumerator CoroutineMoveLocations(int sceneId)
    {
        IEnumerator startFadeTransition = StartFadeTransition();
        yield return StartCoroutine(startFadeTransition);

        SceneManager.LoadScene(sceneId, LoadSceneMode.Single);

    }

    /// <summary>
    /// Lachlan Pye
    /// Stops the game and shows the pause menu.
    /// </summary>
    public void Pause()
    {
        pauseMenu.SetActive(true);
        paused = true;
    }

    /// <summary>
    /// Lachlan Pye
    /// Starts the game again and hides all pause menu UI elements.
    /// </summary>
    public void Resume()
    {
        if (pauseMenu != null) { pauseMenu.SetActive(false); }
        if (saveMenu != null) { saveMenu.SetActive(false); }
        if (loadMenu != null) { loadMenu.SetActive(false); }
        if (settingsMenu != null) { settingsMenu.SetActive(false); }
        paused = false;
    }

    /// <summary>
    /// Lachlan Pye
    /// Pauses the game and opens the save menu UI.
    /// </summary>
    /// <param name="savePoint"></param>
    public void Save(string savePoint)
    {
        saveAndLoad.currentSavingPoint = savePoint;
        saveMenu.SetActive(true);
        paused = true;
    }

    /// <summary>
    /// Lachlan Pye
    /// Starts the TakeBipedalDamage coroutine.
    /// </summary>
    /// <param name="enemy">The enemy which is dealing damage to the player.</param>
    public void StartTakeBipdealDamageCoroutine(GameObject enemy)
    {
        StartCoroutine(TakeBipedalDamage(enemy));
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player is not invulnerable, then stop the player. If the player has enough health to survive, then play the Hurt animation
    /// and resume the game after a moment. If the player does not have enough health to survive, then play the Kill animation and show the Game Over UI screen.
    /// </summary>
    /// <param name="enemy">The enemy that is attacking the player.</param>
    private IEnumerator TakeBipedalDamage(GameObject enemy)
    {
        if (!playerIsInvulnerable)
        {
            playerBehaviour.health++;
            playerBehaviour.canMove = false;
            enemy.GetComponent<SpriteRenderer>().enabled = false;

            if (playerBehaviour.health < 3)
            {
                Debug.Log("attack!");
                playerIsInvulnerable = true;
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

                gameOverAudio.playGameOver();

                yield return StartCoroutine(StartFadeTransition());
                yield return new WaitForSeconds(1);
                gameOverUI.SetActive(true);
                foreach (Transform t in gameOverUI.transform)
                {
                    StartCoroutine(FadeInObject(t.gameObject, 20));
                }
            }

            playerBehaviour.canMove = true;
            yield return new WaitForSeconds(playerBehaviour.invulnerabilityTime);
            playerIsInvulnerable = false;
        }
        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Starts the TakeBossDamage coroutine.
    /// </summary>
    public void StartBossDamageCoroutine()
    {
        StartCoroutine(TakeBossDamage());
    }
    /// <summary>
    /// Lachlan Pye
    /// Stop the player. If the player has enough health to survive the hit, then play the Hurt animation. If the 
    /// player does not have enough health to survive the hit, then play the Kill animation and show the Game Over UI screen.
    /// </summary>
    public IEnumerator TakeBossDamage()
    {
        playerBehaviour.health++;
        playerBehaviour.canMove = false;

        if (playerBehaviour.health < 3)
        {
            healthUI.SetHealth(playerBehaviour.health);
            StartCoroutine(playerBehaviour.PlayBossHurtAnimation());
            yield return new WaitForSeconds(0.5f);

            playerBehaviour.canMove = true;
        }
        else if (playerBehaviour.health == 3)
        {
            healthUI.SetHealth(playerBehaviour.health);
            StartCoroutine(playerBehaviour.PlayBossKillAnimation());
            yield return new WaitForSeconds(3.0f);

            gameOverAudio.playGameOver();

            yield return StartCoroutine(StartFadeTransition());
            yield return new WaitForSeconds(1.0f);
            gameOverUI.SetActive(true);
            foreach (Transform t in gameOverUI.transform)
            {
                StartCoroutine(FadeInObject(t.gameObject, 20));
            }
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// Returns whether there is dialogue currently playing.
    /// </summary>
    /// <returns>Whether there is dialogue currently playing.</returns>
    public bool DialogueActive()
    {
        return dialogueActive;
    }

    /// <summary>
    /// Lachlan Pye
    /// Get the next line of the dialogue.
    /// </summary>
    public void GetNextLine()
    {
        nextLine = true;
    }

    /// <summary>
    /// Lachlan Pye
    /// Used in the houseInterior scene. Switches a number of game objects to being in their day / night time coloring.
    /// </summary>
    /// <param name="value">Whether the gameobjects should be in their day or night state.</param>
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
    /// <summary>
    /// Lachlan Pye
    /// Set the light intensity of the global light object.
    /// </summary>
    /// <param name="intensity">The intensity of the global light object.</param>
    public void SetLightIntensity(float intensity)
    {
        globalLight.GetComponent<Light2D>().intensity = intensity;
    }

    /// <summary>
    /// Lachlan Pye
    /// Set the flash effect to take place over a longer period of time.
    /// </summary>
    public void SlowFlashEffect()
    {
        startFlashEffectTime = SLOW_START_FADE_TIME;
        endFlashEffectTime = SLOW_END_FADE_TIME;
    }
    /// <summary>
    /// Lachlan Pye
    /// Set the transition effect to take place over a longer period of time.
    /// </summary>
    public void SlowFadeOut()
    {
        fadeTransitionTime = fadeTransitionTime * 3;
    }

    /// <summary>
    /// Lachlan Pye
    /// Start the flash effect by gradually increasing the alpha of the image and then gradually decreasing it.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// Start the CutsceneDialogue coroutine.
    /// </summary>
    /// <param name="xmlInput">The name of the xml file that holds the script for the cutscene.</param>
    public void CutsceneDialogueFunction(string xmlInput)
    {
        StartCoroutine(CutsceneDialogue(xmlInput, 1));
    }
    /// <summary>
    /// Lachlan Pye
    /// Either loads an xml file, or parses an xml string for a dialogue script. It will then show the dialogue box and display the first line in the 
    /// dialogue script, and then wait until the player chooses to continue. Once all lines of dialogue have been shown, the dialogue box is hidden.
    /// </summary>
    /// <param name="xmlInput">The name of the xml file, or the xml string itself, containing the dialogue script.</param>
    /// <param name="mode">If this is set to 0, the function will treat xmlInput as a file name. If this is set to 1, the function will treat xmlInput as an xml string.</param>
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

    /// <summary>
    /// Lachlan Pye
    /// Portray the fade transition effect by gradually increasing the alpha of the transition panel.
    /// </summary>
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
    /// <summary>
    /// Lachlan Pye
    /// Portray the fade transition effect by gradually decreasing the alpha of the transition panel. Ideally should be called after StartFadeTransition has been called.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// Similar to StartFadeTransition, fades in a game object over a period of time by gradually increasing the alpha of the object's sprite.
    /// </summary>
    /// <param name="gameObject">The gameobject to be faded in.</param>
    /// <param name="fadeInTime">The time over which to fade in the object, in frames.</param>
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
    
    /// <summary>
    /// Lachlan Pye
    /// Returns the HealthUI game object.
    /// </summary>
    /// <returns>The HealthUI game object.</returns>
    public GameObject GetHealthUI()
    {
        return healthUI.gameObject;
    }
}
