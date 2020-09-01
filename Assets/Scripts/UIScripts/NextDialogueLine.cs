using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextDialogueLine : MonoBehaviour
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
}
