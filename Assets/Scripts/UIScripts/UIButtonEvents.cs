using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Component that controls UI button presses.
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
    private bool hasPushedEnter;
    private float[] volumeConfigs;

    private Dictionary<string, string> sceneToProperNameMap;

    /// <summary>
    /// Lachlan Pye
    /// Class that can be serialized to a file to save the player's sound settings between sessions.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        worldControl = gameController.GetComponent<WorldControl>();
        saveAndLoad = saveController.GetComponent<SaveAndLoadGame>();
        cutsceneControl = cutsceneController.GetComponent<CutsceneControl>();

        hasPushedPause = false;
        hasPushedEnter = false;
        volumeConfigs = new float[3];

        //Janine - initialise to default volumes
        volumeConfigs[0] = 1f;
        volumeConfigs[1] = 0.75f;
        volumeConfigs[2] = 1f;

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

    /// <summary>
    /// Lachlan Pye
    /// Update function that provides various functions.
    /// </summary>
    void Update()
    {
        // Lachlan Pye
        // If the player uses the Pause key, then pause the game if it is not paused, and unpause the game if it is paused.
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

        // Lachlan Pye
        // If there is dialogue playing, check if the player is pushing the Submit key or the mouse down key, and go to the next dialogue line if they are doing so.
        if ((Input.GetAxis("Submit") > 0 || Input.GetMouseButtonDown(0) == true) && worldControl.dialogueActive == true && hasPushedEnter == false)
        {
            hasPushedEnter = true;
            Advance();
        }

        if (Input.GetAxis("Submit") == 0)
        {
            hasPushedEnter = false;
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// Called when the Load UI button is pressed in the Pause menu.
    /// This function displays the Load menu, and also enables / disables the slot UI buttons depending on whether the
    /// slot has a corresponding save attached to it.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// Called from the Load UI menu, this loads the game from a certain slot.
    /// </summary>
    /// <param name="slotNum">The number of the slot that the game should be loaded from.</param>
    public void Load(string slotNum)
    {
        saveAndLoad.LoadGame(slotNum);
    }
    /// <summary>
    /// Lachlan Pye
    /// Called from the Save UI menu, this saves the game to a certain slot.
    /// </summary>
    /// <param name="slotNum">The number of the slot that the game should be saved to.</param>
    public void Save(string slotNum)
    {
        saveAndLoad.SaveGame(slotNum);
    }

    /// <summary>
    /// Lachlan Pye
    /// Displays the Settings UI menu, and loads the player's current settings from the player's config file.
    /// </summary>
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

    /// <summary>
    /// Lachlan Pye
    /// Saves the current sound settings to the player's config file.
    /// </summary>
    public void SaveSettings()
    {
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

    /// <summary>
    /// Lachlan Pye
    /// When the global volume setting is changed, automatically update the game global volume.
    /// </summary>
    /// <param name="volume">The value to which the volume should be set to.</param>
    public void GlobalVolumeChange(System.Single volume)
    {
        audioManager.SetGlobalVolume(volume);
        volumeConfigs[0] = volume;
    }
    /// <summary>
    /// Lachlan Pye
    /// When the background music volume setting is changed, automatically update the game background music volume.
    /// </summary>
    /// <param name="volume">The value to which the volume should be set to.</param>
    public void BGMVolumeChange(System.Single volume)
    {
        audioManager.SetBGMVolume(volume);
        volumeConfigs[1] = volume;
    }
    /// <summary>
    /// Lachlan Pye
    /// When the sound effects setting is changed, automatically update the game sound effects volume.
    /// </summary>
    /// <param name="volume">The value to which the volume should be set to.</param>
    public void FXVolumeChange(System.Single volume)
    {
        audioManager.SetSFXVolume(volume);
        volumeConfigs[2] = volume;
    }

    /// <summary>
    /// Lachlan Pye
    /// Gets the audio settings from the config file and returns the current volume levels.
    /// </summary>
    /// <returns>Array of floats representing the audio levels.</returns>
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

    /// <summary>
    /// Lachlan Pye
    /// Calls the GetNextLine function.
    /// </summary>
    public void Advance()
    {
        worldControl.GetNextLine();
    }

    /// <summary>
    /// Lachlan Pye
    /// Calls the Resume function.
    /// </summary>
    public void Resume()
    {
        worldControl.Resume();
    }

    /// <summary>
    /// Lachlan Pye
    /// Hides all pause menu elements.
    /// </summary>
    public void MainMenu()
    {
        if (pauseMenu != null) { pauseMenu.SetActive(false); }
        if (saveMenu != null) { saveMenu.SetActive(false); }
        if (loadMenu != null) { loadMenu.SetActive(false); }
        if (settingsMenu != null) { settingsMenu.SetActive(false); }
    }
    /// <summary>
    /// Lachlan Pye
    /// Hides all pause menu elements besides the main pause menu.
    /// </summary>
    public void BackToMainPauseMenu()
    {
        if (saveMenu != null) { saveMenu.SetActive(false); }
        if (loadMenu != null) { loadMenu.SetActive(false); }
        if (settingsMenu != null) { settingsMenu.SetActive(false); }
        pauseMenu.SetActive(true);
    }

    /// <summary>
    /// Lachlan Pye
    /// Starts the StartNewGameFade coroutine to load a new game from the Main Menu.
    /// </summary>
    public void NewGame()
    {
        StartCoroutine(StartNewGameFade());
    }
    /// <summary>
    /// Lachlan Pye
    /// Wait for the transition screen to fade in and then load the first scene in the game to start a new game.
    /// </summary>
    private IEnumerator StartNewGameFade()
    {
        yield return StartCoroutine(worldControl.StartFadeTransition());
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    /// <summary>
    /// Lachlan Pye
    /// Starts the StartExitFade coroutine to return to the Main Menu from the game.
    /// </summary>
    public void Exit()
    {
        StartCoroutine(StartExitFade());
    }
    /// <summary>
    /// Lachlan Pye
    /// Wait for the transition screen to fade in and then load the Main Menu scene.
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartExitFade()
    {
        yield return StartCoroutine(worldControl.StartFadeTransition());
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    /// <summary>
    /// Lachlan Pye
    /// Forces the game to immediately exit without waiting for the fade transition.
    /// </summary>
    public void ForceExit()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    /// <summary>
    /// Lachlan Pye
    /// Exit the game window from the Main Menu.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
