using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonEvents : MonoBehaviour
{
    public GameObject gameController;

    private WorldControl worldControl;

    void Start()
    {
        worldControl = gameController.GetComponent<WorldControl>();
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
        if (Application.isEditor == true)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
