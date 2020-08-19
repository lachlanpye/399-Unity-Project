using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaZoneTrigger : MonoBehaviour
{
    public string areaName;

    private PlayerBehaviour playerBehaviour;

    void Start()
    {
        playerBehaviour = GameObject.Find("Player").GetComponent<PlayerBehaviour>();

        WorldControl worldControl = GameObject.Find("GameController").GetComponent<WorldControl>();
        bool foundScene = false;
        for (int i = 0; i < worldControl.scenes.Length; i++)
        {
            if (worldControl.scenes[i].sceneName == areaName)
            {
                foundScene = true;
                break;
            }
        }
        
        if (foundScene == false)
        {
            Debug.LogError("AreaZone " + areaName + " has a name that does not match with any active scenes. Please add this scene to the GameController or change the name of the AreaZone.");
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerBehaviour.SetArea(areaName);
            Debug.Log(areaName);
        }
    }
}
