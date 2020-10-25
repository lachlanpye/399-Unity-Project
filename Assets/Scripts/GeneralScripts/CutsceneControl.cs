using System.Collections;
using System.Xml;

using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CutsceneControl : MonoBehaviour
{
    [System.Serializable]
    public struct Actor
    {
        [Tooltip("The name used to refer to this actor in the cutscene's script.")]
        public string actorAlias;
        public GameObject actor;
    }

    [System.Serializable]
    public struct ObjectPrep
    {
        public GameObject gameObject;
        public bool VisibleOnStart;
        public Vector2 startPos;
        [Space]
        public bool VisibleOnFinish;
        public Vector2 endPos;
    }

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
    public GameObject healthUI;
    public Cutscene[] cutscenes;

    private WorldControl worldControl;

    [HideInInspector]
    public bool cutsceneActive;

    private bool healthUIActive;

    private AudioListener cameraListener;
    private GameObject player;

    void Start()
    {
        worldControl = gameController.GetComponent<WorldControl>();
        healthUIActive = healthUI.activeInHierarchy;
        cameraListener = Camera.main.GetComponent<AudioListener>();
    }
    
    public void StartCutscene(string cutsceneName)
    {
        if (healthUI.activeInHierarchy == true)
        {
            healthUIActive = true;
            healthUI.SetActive(false);
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
            cutscene.objectPrep[i].gameObject.transform.position = cutscene.objectPrep[i].startPos;
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

                case "actorWalk":
                    time = float.Parse(nodes[i].Attributes["time"].Value);

                    actor = FindActor(cutscene, nodes[i].InnerText);

                    startPosition = actor.transform.position;
                    endPosition = new Vector3(float.Parse(nodes[i].Attributes["x"].Value), float.Parse(nodes[i].Attributes["y"].Value), 0);

                    walkAnim = nodes[i].Attributes["walkAnim"].Value;

                    IEnumerator actorWalk = ActorWalk(cutscene, actor, time, endPosition, walkAnim);
                    yield return StartCoroutine(actorWalk);
                    break;

                case "wait":
                    yield return new WaitForSeconds(float.Parse(nodes[i].InnerText));
                    break;

                case "dialogue":
                    IEnumerator cutsceneDialogue = worldControl.CutsceneDialogue(nodes[i].OuterXml, 0);    
                    yield return StartCoroutine(cutsceneDialogue);
                    break;

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

                case "actorActive":
                    actor = FindActor(cutscene, nodes[i].InnerText);

                    setActive = bool.Parse(nodes[i].Attributes["setActive"].Value);

                    IEnumerator actorActive = ActorActive(cutscene, actor, setActive);
                    StartCoroutine(actorActive);
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
                    AudioManager.publicInstance.FadeOutBGM();
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

                case "fadeOut":
                    IEnumerator fadeOut = worldControl.StartFadeTransition();
                    yield return StartCoroutine(fadeOut);
                    break;

                case "fadeIn":
                    IEnumerator fadeIn = worldControl.EndFadeTransition();
                    yield return StartCoroutine(fadeIn);
                    break;

                case "setLighting":
                    intensity = float.Parse(nodes[i].Attributes["intensity"].Value);

                    actor = FindActor(cutscene, nodes[i].InnerText);

                    IEnumerator setLighting = SetLighting(cutscene, actor, intensity);
                    StartCoroutine(setLighting);
                    break;

                case "startBossFight":
                    actor = FindActor(cutscene, nodes[i].InnerText);
                    actor.GetComponent<BossBehaviour>().BeginFirstPhase();
                    break;

                case "startSecondPhase":
                    actor = FindActor(cutscene, nodes[i].InnerText);
                    actor.GetComponent<BossBehaviour>().BeginSecondPhase();
                    break;

                case "switchToDayOrNight":
                    worldControl.SwitchToDayOrNight(nodes[i].InnerText);
                    break;

                case "cameraFollowPlayer":
                    worldControl.cameraFollowPlayer = bool.Parse(nodes[i].Attributes["enabled"].Value);
                    break;

                case "slowFlashEffect":
                    worldControl.SlowFlashEffect();
                    break;

                case "lucasFlashEffect":
                    StartCoroutine(worldControl.LucasFlashEffect());
                    break;

                case "slowFadeOut":
                    worldControl.SlowFadeOut();
                    break;

                case "debugLog":
                    Debug.Log(nodes[i].InnerText);
                    break;

            }
        }

        for (int i = 0; i < cutscene.objectPrep.Length; i++)
        {
            cutscene.objectPrep[i].gameObject.SetActive(cutscene.objectPrep[i].VisibleOnFinish);
            cutscene.objectPrep[i].gameObject.transform.position = cutscene.objectPrep[i].endPos;
        }

        worldControl.paused = false;
        cutsceneActive = false;

        if (healthUIActive == true)
        {
            healthUI.SetActive(true);
        }

        yield return null;
    }

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
    private IEnumerator ActorActive(Cutscene cutscene, GameObject actor, bool setActive)
    {
        actor.SetActive(setActive);
        yield return null;
    }

    private IEnumerator SetLighting(Cutscene cutscene, GameObject actor, float intensity)
    {
        actor.GetComponent<Light2D>().intensity = intensity;

        yield return null;
    }

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
