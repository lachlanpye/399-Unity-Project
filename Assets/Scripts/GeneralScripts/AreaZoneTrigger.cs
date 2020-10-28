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
        for (int i = 0; i < worldControl.scenes.Count; i++)
        {
            if (worldControl.scenes[i].pointName == areaName)
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
}
