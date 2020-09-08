using System.Collections;
using System.Xml;

using UnityEngine;

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
    public struct Cutscene
    {
        public string cutsceneName;
        [Tooltip("The file path to the script used for this cutscene.")]
        public string cutsceneScriptPath;
        public GameObject cameraObject;
        public Actor[] actors;
    }

    public Cutscene[] cutscenes;

    void Start()
    {
        StartCutscene("Intro");
    }
    
    public void StartCutscene(string cutsceneName)
    {
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
        int i;
        for (i = 0; i < cutscene.actors.Length; i++)
        {
            cutscene.actors[i].actor.SetActive(true);
        }
        float time;

        GameObject actor;
        string walkAnim;

        Vector3 startPosition;
        Vector3 endPosition;

        for (i = 0; i < nodes.Count; i++)
        {
            switch (nodes[i].Name)
            {
                case "cameraPan":
                    time = float.Parse(nodes[i].Attributes["time"].Value);
                    startPosition = cutscene.cameraObject.transform.position;
                    endPosition = new Vector3(float.Parse(nodes[i].Attributes["x"].Value), float.Parse(nodes[i].Attributes["y"].Value), -10);

                    IEnumerator cameraPan = CameraPan(cutscene, time, startPosition, endPosition);
                    if (bool.Parse(nodes[i].Attributes["yieldUntilDone"].Value) == true)
                    {
                        yield return StartCoroutine(cameraPan);
                    }
                    else
                    {
                        StartCoroutine(cameraPan);
                    }
                    break;

                case "actorWalk":
                    time = float.Parse(nodes[i].Attributes["time"].Value);

                    actor = null;
                    for (int j = 0; j < cutscene.actors.Length; j++)
                    {
                        if (cutscene.actors[j].actorAlias == nodes[i].InnerText)
                        {
                            actor = cutscene.actors[j].actor;
                            break;
                        }
                    }

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
                    Debug.Log("Dialogue");
                    break;
                case "actorAnimation":
                    Debug.Log("Actor Animation");
                    break;
                case "playSound":
                    Debug.Log("Play Sound");
                    break;
                case "debugLog":
                    Debug.Log(nodes[i].InnerText);
                    break;
            }
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
}
