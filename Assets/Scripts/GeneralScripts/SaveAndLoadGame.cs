using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveAndLoadGame : MonoBehaviour
{
    public GameObject playerObj;
    public GameObject cameraObj;
    public GameObject gameController;

    public string sceneName;
    [Space]
    public string[] gameObjectsToDisableOnLoad;

    [HideInInspector]
    public string currentSavingPoint;

    [System.Serializable]
    public class GameData
    {
        public float playerPosX;
        public float playerPosY;
        public float playerHealth;

        public float cameraPosX;
        public float cameraPosY;

        public int storyPos;

        public string sceneName;
        public string[] gameObjectsToDisableOnLoad;

        public GameData(GameObject playerObj, GameObject cameraObj, GameObject gameController, string sceneName, string[] disableOnLoad)
        {
            playerPosX = playerObj.transform.position.x;
            playerPosY = playerObj.transform.position.y;
            playerHealth = playerObj.GetComponent<PlayerBehaviour>().currentHealth;

            cameraPosX = cameraObj.transform.position.x;
            cameraPosY = cameraObj.transform.position.y;

            gameObjectsToDisableOnLoad = disableOnLoad;

            this.sceneName = sceneName;
        }
    }

    void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    public GameData[] SlotsWithSaves()
    {
        GameData[] saveSlotHere = new GameData[8];

        for (int i = 1; i <= 8; i++)
        {
            string destination = Application.persistentDataPath + "/save" + i.ToString() + ".dat";
            FileStream file;
            if (File.Exists(destination))
            {
                file = File.OpenRead(destination);
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                GameData gameData = (GameData)binaryFormatter.Deserialize(file);
                file.Close();
                saveSlotHere[i-1] = gameData;
            } else
            {
                saveSlotHere[i-1] = null;
            }
        }

        return saveSlotHere;
    }

    public void SaveGame(string slotNum)
    {
        GameData gameData = new GameData(playerObj, cameraObj, gameController, sceneName, gameObjectsToDisableOnLoad);

        string destination = Application.persistentDataPath + "/save" + slotNum + ".dat";
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
        binaryFormatter.Serialize(file, gameData);
        file.Close();

        gameController.GetComponent<WorldControl>().Resume();
    }

    public void LoadGame(string slotNum)
    {
        string destination = Application.persistentDataPath + "/save" + slotNum + ".dat";
        FileStream file;

        if (File.Exists(destination))
        {
            file = File.OpenRead(destination);
        }
        else
        {
            Debug.LogError("File not found.");
            return;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        GameData gameData = (GameData)binaryFormatter.Deserialize(file);
        file.Close();

        IEnumerator fadeIntoLoad = FadeIntoLoad(gameData);
        StartCoroutine(fadeIntoLoad);
    }

    public IEnumerator FadeIntoLoad(GameData gameData)
    {
        WorldControl worldControl = gameController.GetComponent<WorldControl>();
        IEnumerator startFadeTransition = worldControl.StartFadeTransition();

        yield return StartCoroutine(startFadeTransition);

        DontDestroyOnLoad(gameObject);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameData.sceneName, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        foreach (string gameObjectName in gameData.gameObjectsToDisableOnLoad)
        {
            GameObject.Find(gameObjectName).SetActive(false);
        }

        worldControl = GameObject.Find("GameController").GetComponent<WorldControl>();
        worldControl.playerObject.transform.position = new Vector2(gameData.playerPosX, gameData.playerPosY);
        worldControl.mainCamera.transform.position = new Vector3(gameData.cameraPosX, gameData.cameraPosY, -10);

        IEnumerator endFadeTransition = worldControl.EndFadeTransition();
        yield return StartCoroutine(endFadeTransition);

        Destroy(gameObject);
        yield return null;
    }
}
