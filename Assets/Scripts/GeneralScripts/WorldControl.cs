using System.Collections;
using System.Collections.Generic;

using System.Xml;
using UnityEngine;

public class WorldControl : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject playerObject;

    [System.Serializable]
    public struct WarpLocation
    {
        public string pointName;
        public Vector2 newPlayerPosition;
        public Vector2 newCameraPosition;
    }
    public WarpLocation[] scenes;

    private string currentScene;

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

    public void DialogueScene(string fileName)
    {
        TextAsset textFile = Resources.Load<TextAsset>("Dialogue/" + fileName);

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textFile.text);

        // Create an array of speaker / dialogue pairs
        XmlNodeList dialogueLines = doc.GetElementsByTagName("line");
        List<(string, string)> tupleList = new List<(string, string)>();

        foreach (XmlNode node in dialogueLines)
        {
            Debug.Log(node.InnerText);
        }
    }
}
