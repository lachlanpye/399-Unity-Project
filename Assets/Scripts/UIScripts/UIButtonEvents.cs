using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonEvents : MonoBehaviour
{
    public GameObject gameController;
    public GameObject saveController;

    private WorldControl worldControl;
    private SaveAndLoadGame saveAndLoad;
    private bool hasPushedPause;

    void Start()
    {
        hasPushedPause = false;
        worldControl = gameController.GetComponent<WorldControl>();
        saveAndLoad = saveController.GetComponent<SaveAndLoadGame>();
    }

    void Update()
    {
        if (Input.GetAxis("Pause") > 0 && hasPushedPause == false)
        {
            if (worldControl.paused == false)
            {
                worldControl.Pause();
            }
            else
            {
                worldControl.Resume();
            }
            hasPushedPause = true;
        }
    }

    public void Load()
    {
        saveAndLoad.LoadGame();
    }

    public void Advance()
    {
        worldControl.GetNextLine();
    }

    public void Resume()
    {
        worldControl.Resume();
    }

    public void Exit()
    {
        Debug.Log("Exit");
    }
}
