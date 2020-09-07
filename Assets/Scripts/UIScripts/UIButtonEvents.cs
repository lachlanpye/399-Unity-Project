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

    void Update()
    {
        if (Input.GetAxis("Pause") > 0)
        {
            worldControl.Pause();
        }
    }

    public void Advance()
    {
        worldControl.GetNextLine();
    }
}
