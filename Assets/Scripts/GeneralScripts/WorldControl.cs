using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldControl : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject playerObject;

    [System.Serializable]
    public struct SceneLocation
    {
        public string sceneName;
        public Vector2 newPlayerPosition;
        public Vector2 newCameraPosition;
    }
    public SceneLocation[] scenes;

    private string currentScene;

    public void MoveScenes(string sceneName)
    {
        SceneLocation scene = new SceneLocation();

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].sceneName == sceneName)
            {
                scene = scenes[i];
            }
        }

        if (scene.sceneName == null)
        {
            Debug.LogWarning("ERROR: Scene name not assigned in ActionZone object.");
        }
        else
        {
            // Add some kind of scene transition here
            playerObject.transform.position = new Vector3(scene.newPlayerPosition.x, scene.newPlayerPosition.y, 0);
            mainCamera.transform.position = new Vector3(scene.newCameraPosition.x, scene.newCameraPosition.y, -10);
        }
    }
}
