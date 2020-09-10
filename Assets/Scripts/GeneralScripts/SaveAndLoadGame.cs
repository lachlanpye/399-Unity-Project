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

        public GameData(GameObject playerObj, GameObject cameraObj, GameObject gameController, string sceneName)
        {
            playerPosX = playerObj.transform.position.x;
            playerPosY = playerObj.transform.position.y;
            playerHealth = playerObj.GetComponent<PlayerBehaviour>().currentHealth;

            cameraPosX = cameraObj.transform.position.x;
            cameraPosY = cameraObj.transform.position.y;

            this.sceneName = sceneName;
            storyPos = gameController.GetComponent<WorldControl>().storyPosition;
        }
    }

    void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    public bool[] SlotsWithSaves()
    {
        bool[] saveSlotHere = new bool[8];

        for (int i = 0; i < 8; i++)
        {
            string destination = Application.persistentDataPath + "/slot" + i.ToString() + ".dat";
            if (File.Exists(destination))
            {
                saveSlotHere[i] = true;
            } else
            {
                saveSlotHere[i] = false;
            }
        }

        return saveSlotHere;
    }

    public void SaveGame()
    {
        GameData gameData = new GameData(playerObj, cameraObj, gameController, sceneName);

        string destination = Application.persistentDataPath + "/slot0.dat";
        Debug.Log(destination);
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
    }

    public void LoadGame()
    {
        string destination = Application.persistentDataPath + "/save0.dat";
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

        worldControl = GameObject.Find("GameController").GetComponent<WorldControl>();
        worldControl.playerObject.transform.position = new Vector2(gameData.playerPosX, gameData.playerPosY);
        worldControl.mainCamera.transform.position = new Vector3(gameData.cameraPosX, gameData.cameraPosY, -10);

        IEnumerator endFadeTransition = worldControl.EndFadeTransition();
        yield return StartCoroutine(endFadeTransition);

        Destroy(gameObject);
        yield return null;
    }
}
