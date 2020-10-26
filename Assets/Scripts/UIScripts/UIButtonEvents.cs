using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIButtonEvents : MonoBehaviour
{
    public GameObject gameController;
    public GameObject saveController;
    public GameObject cutsceneController;
    [Space]
    public GameObject pauseMenu;
    [Space]
    public GameObject saveMenu;
    [Space]
    public GameObject loadMenu;
    public GameObject loadSlotPanel;
    public Color normalSlotColor;
    public Color disabledSlotColor;
    [Space]
    public GameObject settingsMenu;
    public GameObject globalVolumeSlider;
    public GameObject bgmVolumeSlider;
    public GameObject fxVolumeSlider;

    private AudioManager audioManager;

    private WorldControl worldControl;
    private SaveAndLoadGame saveAndLoad;
    private CutsceneControl cutsceneControl;

    private bool hasPushedPause;
    private float[] volumeConfigs;

    private Dictionary<string, string> sceneToProperNameMap;
    
    [System.Serializable]
    public class Config
    {
        public float globalVolume;
        public float bgmVolume;
        public float fxVolume;

        public Config(float[] volumes)
        {
            globalVolume = volumes[0];
            bgmVolume = volumes[1];
            fxVolume = volumes[2];
        }
    }

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        worldControl = gameController.GetComponent<WorldControl>();
        saveAndLoad = saveController.GetComponent<SaveAndLoadGame>();
        cutsceneControl = cutsceneController.GetComponent<CutsceneControl>();

        hasPushedPause = false;
        volumeConfigs = new float[3];

        AudioManager.publicInstance.Instantiate();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        sceneToProperNameMap = new Dictionary<string, string>()
        {
            { "houseOutside" , "Outside House" },
            { "Town", "Town" },
            { "Forest", "Forest" },
            { "church", "Church" }
        };
    }

    void Update()
    {
        if (Input.GetAxis("Pause") > 0 && hasPushedPause == false && cutsceneControl.cutsceneActive == false)
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

        if (Input.GetAxis("Pause") == 0)
        {
            hasPushedPause = false;
        }
    }

    public void OpenLoadMenu()
    {
        pauseMenu.SetActive(false);
        loadMenu.SetActive(true);

        SaveAndLoadGame.GameData[] saveSlots = saveAndLoad.SlotsWithSaves();
        int i = 0;
        foreach (Transform t in loadSlotPanel.transform)
        {
            if (t.gameObject.tag == "SlotButton")
            {
                if (saveSlots[i] != null)
                {
                    t.gameObject.GetComponent<Image>().color = normalSlotColor;
                    t.gameObject.GetComponent<Button>().enabled = true;

                    t.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Slot " + (i + 1).ToString() + " - " + sceneToProperNameMap[saveSlots[i].sceneName];
                }
                else
                {
                    t.gameObject.GetComponent<Image>().color = disabledSlotColor;
                    t.gameObject.GetComponent<Button>().enabled = false;

                    t.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "Slot " + (i + 1).ToString();
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

    public void OpenSettingsMenu()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);

        string destination = Application.persistentDataPath + "/config.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Config config = (Config)binaryFormatter.Deserialize(file);

            file.Close();

            Debug.Log(destination);
            globalVolumeSlider.GetComponent<Slider>().value = config.globalVolume;
            bgmVolumeSlider.GetComponent<Slider>().value = config.bgmVolume;
            fxVolumeSlider.GetComponent<Slider>().value = config.fxVolume;
        }
    }
    public void SaveSettings()
    {
        // Write to config file
        Config config = new Config(volumeConfigs);
        string destination = Application.persistentDataPath + "/config.dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenWrite(destination);
        }
        else
        {
            file = File.Create(destination);
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(file, config);
        file.Close();
    }

    public void GlobalVolumeChange(System.Single volume)
    {
        audioManager.SetGlobalVolume(volume);
        volumeConfigs[0] = volume;
    }
    public void BGMVolumeChange(System.Single volume)
    {
        audioManager.SetBGMVolume(volume);
        volumeConfigs[1] = volume;
    }
    public void FXVolumeChange(System.Single volume)
    {
        audioManager.SetSFXVolume(volume);
        volumeConfigs[2] = volume;
    }

    public float[] GetAudioSettings()
    {
        string destination = Application.persistentDataPath + "/config.dat";
        float[] volumes = new float[3] { 0f, 0f, 0f };
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            Config config = (Config)binaryFormatter.Deserialize(file);
            file.Close();

            volumes[0] = config.globalVolume;
            volumes[1] = config.bgmVolume;
            volumes[2] = config.fxVolume;
        }
        else
        {
            volumes[0] = 1.0f;
            volumes[1] = 0.75f;
            volumes[2] = 1.0f;
        }

        return volumes;
    }

    public void Advance()
    {
        worldControl.GetNextLine();
    }

    public void Resume()
    {
        worldControl.Resume();
    }

    public void MainMenu()
    {
        if (pauseMenu != null) { pauseMenu.SetActive(false); }
        if (saveMenu != null) { saveMenu.SetActive(false); }
        if (loadMenu != null) { loadMenu.SetActive(false); }
        if (settingsMenu != null) { settingsMenu.SetActive(false); }
    }
    public void BackToMainPauseMenu()
    {
        if (saveMenu != null) { saveMenu.SetActive(false); }
        if (loadMenu != null) { loadMenu.SetActive(false); }
        if (settingsMenu != null) { settingsMenu.SetActive(false); }
        pauseMenu.SetActive(true);
    }

    public void NewGame()
    {
        StartCoroutine(StartNewGameFade());
    }
    private IEnumerator StartNewGameFade()
    {
        yield return StartCoroutine(worldControl.StartFadeTransition());
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void Exit()
    {
        StartCoroutine(StartExitFade());
    }
    private IEnumerator StartExitFade()
    {
        yield return StartCoroutine(worldControl.StartFadeTransition());
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    public void ForceExit()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
