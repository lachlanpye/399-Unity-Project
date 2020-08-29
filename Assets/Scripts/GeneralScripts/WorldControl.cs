﻿using System.Collections;
using System.Collections.Generic;
using System.Xml;

using UnityEngine;
using UnityEngine.UI;

public class WorldControl : MonoBehaviour
{
    [System.Serializable]
    public struct WarpLocation
    {
        public string pointName;
        public Vector2 newPlayerPosition;
        public Vector2 newCameraPosition;
    }

    public GameObject mainCamera;
    public GameObject playerObject;

    [Space]
    public GameObject UICanvas;

    [Space]
    public WarpLocation[] scenes;

    private ShowDialogue dialogueScript;
    private List<(string, string)> dialogueList;
    private bool dialogueActive;
    private int pointer;

    private HealthUI healthUI;
    private GameObject pauseMenu;
    [HideInInspector]
    public bool paused;
    private bool pauseInputReset;

    private PlayerBehaviour playerBehaviour;

    private string currentScene;

    void Start()
    {
        dialogueActive = false;
        foreach (Transform t in UICanvas.transform)
        {
            if (t.gameObject.name == "DialoguePanel")
            {
                dialogueScript = t.gameObject.GetComponent<ShowDialogue>();
            }
            if (t.gameObject.name == "HealthPanel")
            {
                healthUI = t.gameObject.GetComponent<HealthUI>();
            }
            if (t.gameObject.name == "PauseMenu")
            {
                pauseMenu = t.gameObject;
                pauseMenu.SetActive(false);
                paused = false;
            }
        }

        dialogueList = new List<(string, string)>();

        playerBehaviour = playerObject.GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        if (Input.GetAxis("Pause") > 0 && pauseInputReset == true)
        {
            if (paused == false)
            {
                pauseInputReset = false;
                Pause();
            }
            else
            {
                pauseInputReset = false;
                Resume();
            }
        }
        if (Input.GetAxis("Pause") == 0)
        {
            pauseInputReset = true;
        }
    }

    public void MoveScenes(string sceneName)
    {
        WarpLocation point = new WarpLocation();

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].pointName == sceneName)
            {
                point = scenes[i];
            }
        }

        if (point.pointName == null)
        {
            Debug.LogWarning("ERROR: Warp point name not assigned in ActionZone object.");
        }
        else
        {
            // Add some kind of scene transition here
            playerObject.transform.position = new Vector3(point.newPlayerPosition.x, point.newPlayerPosition.y, 0);
            mainCamera.transform.position = new Vector3(point.newCameraPosition.x, point.newCameraPosition.y, -10);
        }
    }
    public void Pause()
    {
        pauseMenu.SetActive(true);
        paused = true;
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        paused = false;
    }

    public void TakeDamage()
    {
        healthUI.HalfHealth();
    }

    public bool DialogueActive()
    {
        return dialogueActive;
    }
    public void DialogueScene(string fileName)
    {
        TextAsset textFile = Resources.Load<TextAsset>("Dialogue/" + fileName);

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textFile.text);

        // Create an array of speaker / dialogue pairs
        dialogueList = new List<(string, string)>();
        foreach (XmlNode node in doc.GetElementsByTagName("line"))
        {
            string speaker = node.Attributes["speaker"].Value;
            string line = node.InnerText;

            dialogueList.Add((speaker, line));
        }

        pointer = 0;
        dialogueActive = true;
        GetNextLine();
    }
    public void GetNextLine()
    {
        if (pointer < dialogueList.Count)
        {
            dialogueScript.ShowDialogueLine(dialogueList[pointer]);
            pointer++;
        } else
        {
            dialogueActive = false;
            dialogueScript.HideDialogue();
        }
    }
}
