using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectObjectScript : MonoBehaviour
{
    [Tooltip("Enter the name of the dialogue file this object is using.")]
    public string dialogueFile;

    private SpriteRenderer spriteRenderer;
    private GameObject gameController;
    // Tag

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        gameController = GameObject.Find("GameController");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spriteRenderer.enabled = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (Input.GetAxis("Interact") > 0)
            {
                gameController.GetComponent<WorldControl>().DialogueScene(dialogueFile);
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spriteRenderer.enabled = false;
        }
    }
}
