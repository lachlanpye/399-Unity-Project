using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonEvents : MonoBehaviour
{
    public GameObject gameController;

    private WorldControl worldControl;
    private bool hasPushedPause;

    void Start()
    {
        hasPushedPause = false;
        worldControl = gameController.GetComponent<WorldControl>();
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
