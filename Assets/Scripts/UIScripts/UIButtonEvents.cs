using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public Color normalSlotColor;
    public Color disabledSlotColor;

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
                    t.gameObject.GetComponent<Image>().color = normalSlotColor;
                    t.gameObject.GetComponent<Button>().enabled = true;
                }
                else
                {
                    t.gameObject.GetComponent<Image>().color = disabledSlotColor;
                    t.gameObject.GetComponent<Button>().enabled = false;
                }

                i++;
            }
        }       
    }
    public void Load(string slotNum)
    {
        saveAndLoad.LoadGame(slotNum);
    }

    public void Save(string slotNum)
    {
        saveAndLoad.SaveGame(slotNum);
    }

    public void Advance()
    {
        worldControl.GetNextLine();
    }

    public void Resume()
    {
        worldControl.Resume();
    }

    public void NewGame()
    {
        StartCoroutine(StartNewGameFade());
    }
    private IEnumerator StartNewGameFade()
    {
        yield return StartCoroutine(worldControl.StartFadeTransition());
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Exit()
    {
        StartCoroutine(StartExitFade());
    }
    private IEnumerator StartExitFade()
    {
        yield return StartCoroutine(worldControl.StartFadeTransition());
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
