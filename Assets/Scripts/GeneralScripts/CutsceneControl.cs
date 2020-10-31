using System.Collections;
using System.Xml;

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

// Component that allows cutscenes to be created and played in a scene.
public class CutsceneControl : MonoBehaviour
{
    /// <summary>
    /// Lachlan Pye
    /// Stores a gameobject that can be controlled by a cutscene script.
    /// </summary>
    [System.Serializable]
    public struct Actor
    {
        [Tooltip("The name used to refer to this actor in the cutscene's script.")]
        public string actorAlias;
        public GameObject actor;
    }

    /// <summary>
    /// Lachlan Pye
    /// A series of parameters to set when a cutscene starts, such as whether the object is active or inactive when the
    /// cutscene starts and their starting / ending location.
    /// </summary>
    [System.Serializable]
    public struct ObjectPrep
    {
        public GameObject gameObject;
        public bool VisibleOnStart;
        public Vector2 startPos;
        public bool ignoreStartCoords;
        [Space]
        public bool VisibleOnFinish;
        public Vector2 endPos;
        public bool ignoreEndCoords;
    }

    /// <summary>
    /// Lachlan Pye
    /// Holds the path to a cutscene file, as well as a list of objects to be set before the cutscene
    /// starts and a list of actors that can be controlled by the cutscene script.
    /// </summary>
    [System.Serializable]
    public struct Cutscene
    {
        public string cutsceneName;
        [Tooltip("The file path to the script used for this cutscene.")]
        public string cutsceneScriptPath;
        public GameObject cameraObject;
        [Space]
        [Tooltip("Show or hide game objects at the start or end of the scene.")]
        public ObjectPrep[] objectPrep;
        [Space]
        [Tooltip("Game objects that will perform actions in the scene.")]
        public Actor[] actors;
    }

    public GameObject gameController;
    public Cutscene[] cutscenes;

    private WorldControl worldControl;
    private GameObject healthUIObject;

    [HideInInspector]
    public bool cutsceneActive;

    private bool healthUIActive;

    private AudioListener cameraListener;
    private GameObject player;
    private float fadeTime;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        worldControl = gameController.GetComponent<WorldControl>();

        healthUIObject = worldControl.GetHealthUI();
        healthUIActive = healthUIObject.activeInHierarchy;
        cameraListener = Camera.main.GetComponent<AudioListener>();
    }
    
    /// <summary>
    /// Lachlan Pye
    /// Starts a cutscene with the specified name. Hides the health UI object, finds the xml file holding
    /// the script and then starts the RunCutsceneScript coroutine to actually play the script.
    /// </summary>
    /// <param name="cutsceneName">The name of the cutscene to be played.</param>
    public void StartCutscene(string cutsceneName)
    {
        if (healthUIObject.activeInHierarchy == true)
        {
            healthUIActive = true;
            healthUIObject.SetActive(false);
        }
        else
        {
            healthUIActive = false;
        }
        Cutscene cutscene = new Cutscene();
        for (int i = 0; i < cutscenes.Length; i++)
        {
            if (cutscenes[i].cutsceneName == cutsceneName)
            {
                cutscene = cutscenes[i];
                break;
            }
        }

        TextAsset textFile = Resources.Load<TextAsset>("Cutscenes/" + cutscene.cutsceneScriptPath);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textFile.text);

        IEnumerator enumerator = RunCutsceneScript(doc.GetElementsByTagName("*"), cutscene);
        StartCoroutine(enumerator);
    }

    /// <summary>
    /// Lachlan Pye
    /// Runs a series of commands stored in xml nodes.
    /// </summary>
    /// <param name="nodes">A list of xml nodes containing script commands to be run, which makes up the whole cutscene.</param>
    /// <param name="cutscene">The Cutscene object that is currently playing.</param>
    private IEnumerator RunCutsceneScript(XmlNodeList nodes, Cutscene cutscene)
    {
        worldControl.paused = true;
        cutsceneActive = true;
        for (int i = 0; i < cutscene.actors.Length; i++)
        {
            cutscene.actors[i].actor.SetActive(true);
        }
        for (int i = 0; i < cutscene.objectPrep.Length; i++)
        {
            cutscene.objectPrep[i].gameObject.SetActive(cutscene.objectPrep[i].VisibleOnStart);
            if (cutscene.objectPrep[i].ignoreStartCoords == false)
            {
                cutscene.objectPrep[i].gameObject.transform.position = cutscene.objectPrep[i].startPos;
            }
        }
        float time;

        float shakeMagnitude;
        float dampingSpeed;

        float intensity;

        string walkAnim;
        string animTrigger;
        string audio;

        bool returnToIdle;
        bool setActive;
        
        GameObject actor;

        Vector3 startPosition;
        Vector3 endPosition; 

        for (int i = 0; i < nodes.Count; i++)
        {
            switch (nodes[i].Name)
            {
                // Lachlan Pye
                // Moves the camera to a set position over a period of time.
                case "cameraPan":
                    time = float.Parse(nodes[i].Attributes["time"].Value);
                    startPosition = cutscene.cameraObject.transform.position;
                    endPosition = new Vector3(float.Parse(nodes[i].Attributes["x"].Value), float.Parse(nodes[i].Attributes["y"].Value), -10);

                    IEnumerator cameraPan = CameraPan(cutscene, time, startPosition, endPosition);
                    if (nodes[i].Attributes["yieldUntilDone"] != null)
                    {
                        if (bool.Parse(nodes[i].Attributes["yieldUntilDone"].Value) == true)
                        {
                            yield return StartCoroutine(cameraPan);
                        }
                        else
                        {
                            StartCoroutine(cameraPan);
                        }
                    }
                    else
                    {
                        StartCoroutine(cameraPan);
                    }
                    break;

                // Lachlan Pye
                // Shakes the camera by randomly moving the camera for a period of time.
                case "cameraShake":
                    time = float.Parse(nodes[i].Attributes["time"].Value);
                    shakeMagnitude = float.Parse(nodes[i].Attributes["shakeMagnitude"].Value);
                    dampingSpeed = float.Parse(nodes[i].Attributes["dampingSpeed"].Value);

                    IEnumerator cameraShake = CameraShake(cutscene, time, shakeMagnitude, dampingSpeed);
                    if (nodes[i].Attributes["yieldUntilDone"] != null)
                    {
                        if (bool.Parse(nodes[i].Attributes["yieldUntilDone"].Value) == true)
                        {
                            yield return StartCoroutine(cameraShake);
                        }
                        else
                        {
                            StartCoroutine(cameraShake);
                        }
                    }
                    else
                    {
                        StartCoroutine(cameraShake);
                    }

                    break;

                // Lachlan Pye
                // Moves an actor to a location over a period of time while playing a certain animation.
                case "actorWalk":
                    time = float.Parse(nodes[i].Attributes["time"].Value);

                    actor = FindActor(cutscene, nodes[i].InnerText);

                    startPosition = actor.transform.position;
                    endPosition = new Vector3(float.Parse(nodes[i].Attributes["x"].Value), float.Parse(nodes[i].Attributes["y"].Value), 0);

                    walkAnim = nodes[i].Attributes["walkAnim"].Value;

                    IEnumerator actorWalk = ActorWalk(cutscene, actor, time, endPosition, walkAnim);
                    yield return StartCoroutine(actorWalk);
                    break;

                // Lachlan Pye
                // Wait for a period of time before resuming playback of the cutscene.
                case "wait":
                    yield return new WaitForSeconds(float.Parse(nodes[i].InnerText));
                    break;

                // Lachlan Pye
                // Play a script of dialogue before continuing.
                case "dialogue":
                    IEnumerator cutsceneDialogue = worldControl.CutsceneDialogue(nodes[i].OuterXml, 0);    
                    yield return StartCoroutine(cutsceneDialogue);
                    break;

                // Lachlan Pye
                // Play an animation tied to an actor.
                case "actorAnimation":
                    actor = FindActor(cutscene, nodes[i].InnerText);

                    returnToIdle = bool.Parse(nodes[i].Attributes["returnToIdle"].Value);
                    animTrigger = nodes[i].Attributes["animTriggerName"].Value;

                    IEnumerator actorAnimation = ActorAnimation(cutscene, actor, animTrigger, returnToIdle);
                    if (nodes[i].Attributes["yieldUntilDone"] != null)
                    {
                        if (bool.Parse(nodes[i].Attributes["yieldUntilDone"].Value) == true)
                        {
                            yield return StartCoroutine(actorAnimation);
                        }
                        else
                        {
                            StartCoroutine(actorAnimation);
                        }
                    }
                    else
                    {
                        StartCoroutine(actorAnimation);
                    }
                    break;

                // Lachlan Pye
                // Set an actor to be active or inactive.
                case "actorActive":
                    actor = FindActor(cutscene, nodes[i].InnerText);

                    setActive = bool.Parse(nodes[i].Attributes["setActive"].Value);

                    IEnumerator actorActive = ActorActive(cutscene, actor, setActive);
                    yield return StartCoroutine(actorActive);
                    break;

                case "playSound":
                    audio = nodes[i].InnerText;

                    AudioManager.publicInstance.PlaySFX(Resources.Load<AudioClip>("Audio/" + audio));
                    break;

                case "playMusic":
                    audio = nodes[i].InnerText;

                    AudioManager.publicInstance.PlayBGM(Resources.Load<AudioClip>("Audio/" + audio));
                    break;

                case "stopMusic":
                    AudioManager.publicInstance.StopBGM();
                    break;

                case "fadeOutMusic":
                    if (nodes[i].Attributes != null)
                    {
                        fadeTime = float.Parse(nodes[i].Attributes["fadeTime"].Value);
                        AudioManager.publicInstance.FadeOutBGM(fadeTime);
                    } 
                    else
                    {
                        AudioManager.publicInstance.FadeOutBGM();
                    }
                    
                    break;

                case "fadeInMusic":
                    audio = nodes[i].InnerText;

                    AudioManager.publicInstance.FadeInBGM(Resources.Load<AudioClip>("Audio/" + audio));
                    break;

                case "fadeOutSFX":
                    AudioManager.publicInstance.FadeOutSFXLoop();
                    break;

                case "fadeInSFX":
                    audio = nodes[i].InnerText;

                    AudioManager.publicInstance.FadeInSFXLoop(Resources.Load<AudioClip>("Audio/" + audio));
                    break;

                case "enableCameraListener":
                    cameraListener.enabled = true;
                    break;

                case "disableCameraListener":
                    cameraListener.enabled = false;
                    break;

                // Lachlan Pye
                // Fade the screen out by gradually covering it with a black panel.
                case "fadeOut":
                    IEnumerator fadeOut = worldControl.StartFadeTransition();
                    yield return StartCoroutine(fadeOut);
                    break;

                // Lachlan Pye
                // Fade the screen in by gradually removing the black panel.
                case "fadeIn":
                    IEnumerator fadeIn = worldControl.EndFadeTransition();
                    yield return StartCoroutine(fadeIn);
                    break;

                // Lachlan Pye
                // Set the lighting intensity of an actor with a Light component.
                case "setLighting":
                    intensity = float.Parse(nodes[i].Attributes["intensity"].Value);

                    actor = FindActor(cutscene, nodes[i].InnerText);

                    IEnumerator setLighting = SetLighting(cutscene, actor, intensity);
                    StartCoroutine(setLighting);
                    break;

                // Lachlan Pye
                // Orders the boss to begin attacking the player. Only used in the StartBossFight cutscene.
                case "startBossFight":
                    actor = FindActor(cutscene, nodes[i].InnerText);
                    actor.GetComponent<BossBehaviour>().BeginFirstPhase();
                    break;

                // Lachlan Pye
                // Orders the boss to begin the second phase of the boss fight. Only used in the MidBossFight cutscene.
                case "startSecondPhase":
                    actor = FindActor(cutscene, nodes[i].InnerText);
                    actor.GetComponent<BossBehaviour>().BeginSecondPhase();
                    break;

                // Lachlan Pye
                // Switches objects to their day or night modes.
                case "switchToDayOrNight":
                    worldControl.SwitchToDayOrNight(nodes[i].InnerText);
                    break;

                // Lachlan Pye
                // Allow the camera to follow the player once the cutscene ends.
                case "cameraFollowPlayer":
                    worldControl.cameraFollowPlayer = bool.Parse(nodes[i].Attributes["enabled"].Value);
                    break;
                
                // Lachlan Pye
                // Reduces the time over which the flash effect plays.
                case "slowFlashEffect":
                    worldControl.SlowFlashEffect();
                    break;

                // Lachlan Pye
                // Activate the lucas flash ability.
                case "lucasFlashEffect":
                    StartCoroutine(worldControl.LucasFlashEffect());
                    break;

                // Lachlan Pye
                // Similar to "fadeOut", but over a longer time.
                case "slowFadeOut":
                    worldControl.SlowFadeOut();
                    break;

                // Lachlan Pye
                // Logs text to the console, for debugging purposes.
                case "debugLog":
                    Debug.Log(nodes[i].InnerText);
                    break;

            }
        }

        for (int i = 0; i < cutscene.objectPrep.Length; i++)
        {
            cutscene.objectPrep[i].gameObject.SetActive(cutscene.objectPrep[i].VisibleOnFinish);
            if (cutscene.objectPrep[i].ignoreEndCoords == false)
            {
                cutscene.objectPrep[i].gameObject.transform.position = cutscene.objectPrep[i].endPos;
            }
        }

        worldControl.paused = false;
        cutsceneActive = false;

        if (healthUIActive == true)
        {
            healthUIObject.SetActive(true);
        }

        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Pans the camera to a position over a period of time.
    /// </summary>
    /// <param name="cutscene">The cutscene object that is being run.</param>
    /// <param name="time">The time over which to pan the camera.</param>
    /// <param name="startPos">The starting position of the camera.</param>
    /// <param name="endPos">The ending position of the camera.</param>
    private IEnumerator CameraPan(Cutscene cutscene, float time, Vector3 startPos, Vector3 endPos)
    {
        for (float i = 0; i <= 1; i += (1 / time) * Time.deltaTime)
        {
            cutscene.cameraObject.transform.position = Vector3.Lerp(startPos, endPos, i);
            yield return new WaitForFixedUpdate();
        }
        cutscene.cameraObject.transform.position = endPos;
        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Shakes the camera over a period of time.
    /// </summary>
    /// <param name="cutscene">The cutscene object that is being run.</param>
    /// <param name="time">The time over which to shake the camera.</param>
    /// <param name="shakeMagnitude">The amplitude of each camera shake.</param>
    /// <param name="dampingSpeed">The speed at which the camera shakes.</param>
    private IEnumerator CameraShake(Cutscene cutscene, float time, float shakeMagnitude, float dampingSpeed)
    {
        Vector3 pos = cutscene.cameraObject.transform.position;

        float i = 0;
        while (i <= 1)
        {
            Vector3 randomCircle = Random.insideUnitSphere;
            randomCircle.z = -10;

            cutscene.cameraObject.transform.position = pos + randomCircle * shakeMagnitude;
            yield return new WaitForSeconds(Time.deltaTime * dampingSpeed);

            i += (1 / time) * Time.deltaTime;
        }

        cutscene.cameraObject.transform.position = pos;

        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Moves the actor to a certain position over a period of time while playing an animation.
    /// </summary>
    /// <param name="cutscene">The cutscene object that is being run.</param>
    /// <param name="actor">The actor that is moving.</param>
    /// <param name="time">The time over which the actor moves.</param>
    /// <param name="endPos">The ending position of the actor.</param>
    /// <param name="walkAnim">The animation that should play as the actor walks.</param>
    private IEnumerator ActorWalk(Cutscene cutscene, GameObject actor, float time, Vector3 endPos, string walkAnim)
    {
        Vector3 startPos = actor.transform.position;

        Animator animator = actor.GetComponent<Animator>();
        animator.SetTrigger(walkAnim);
        for (float i = 0; i <= 1; i += (1 / time) * Time.deltaTime)
        {
            actor.transform.position = Vector3.Lerp(startPos, endPos, i);
            yield return new WaitForFixedUpdate();
        }

        actor.transform.position = endPos;
        animator.SetTrigger("Idle");

        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Plays an animation.
    /// </summary>
    /// <param name="cutscene">The cutscene object that is being run.</param>
    /// <param name="actor">The actor that will have the animation played.</param>
    /// <param name="animTrigger">The name of the trigger that is connected to the animation.</param>
    /// <param name="returnToIdle">Whether the actor will return to an idle state or continue playing the animation.</param>
    private IEnumerator ActorAnimation(Cutscene cutscene, GameObject actor, string animTrigger, bool returnToIdle)
    {
        Animator animator = actor.GetComponent<Animator>();
        animator.SetTrigger(animTrigger);

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (returnToIdle)
        {
            animator.SetTrigger("Idle");
        }

        yield return null;
    }
    /// <summary>
    /// Lachlan Pye
    /// Sets an actor to be active or inactive.
    /// </summary>
    /// <param name="cutscene">The cutscene object that is being run.</param>
    /// <param name="actor">The actor to be set.</param>
    /// <param name="setActive">Whether the actor should be active or inactive.</param>
    private IEnumerator ActorActive(Cutscene cutscene, GameObject actor, bool setActive)
    {
        actor.SetActive(setActive);
        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Sets the lighting of a lighting object.
    /// </summary>
    /// <param name="cutscene">The cutscene object that is being run.</param>
    /// <param name="actor">The actor to have their lighting changed.</param>
    /// <param name="intensity">The level at which to set the lighting.</param>
    private IEnumerator SetLighting(Cutscene cutscene, GameObject actor, float intensity)
    {
        actor.GetComponent<Light2D>().intensity = intensity;

        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// Using the actor alias that is used in the script, search the list of actors in the Cutscene object for the
    /// game object that corresponds to the actor alias. If no such actor exists, log an error.
    /// </summary>
    /// <param name="cutscene">The cutscene object that is being run.</param>
    /// <param name="actorAlias">The alias of the actor that is being searched for.</param>
    /// <returns>A gameobject that corresponds to the actor alias.</returns>
    private GameObject FindActor(Cutscene cutscene, string actorAlias)
    {
        GameObject actor = null;
        for (int i = 0; i < cutscene.actors.Length; i++)
        {
            if (cutscene.actors[i].actorAlias == actorAlias)
            {
                actor = cutscene.actors[i].actor;
                break;
            }
        }

        if (actor == null)
        {
            Debug.LogError("Actor alias not found in this cutscene. Please check that the cutscene has the correct actorAlias in the Inspector for the CutsceneController.");
        }

        return actor;
    }
}
