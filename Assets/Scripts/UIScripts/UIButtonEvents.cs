using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIButtonEvents : MonoBehaviour
{
    public GameObject gameController;
    public GameObject saveController;
    [Space]
    public GameObject pauseMenu;
    [Space]
    public GameObject saveMenu;
    [Space]
    public GameObject loadMenu;
    public GameObject loadSlotPanel;

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

    public void OpenLoadMenu()
    {
        pauseMenu.SetActive(false);
        loadMenu.SetActive(true);

        bool[] saveSlots = saveAndLoad.SlotsWithSaves();
        int i = 0;
        foreach (Transform t in loadSlotPanel.transform)
        {
            if (t.gameObject.tag == "SlotButton")
            {
                if (saveSlots[i] == true)
                {
                    t.gameObject.GetComponent<Button>().enabled = true;
                }
                else
                {
                    t.gameObject.GetComponent<Button>().enabled = false;
                }

                i++;
            }
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
