using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectObjectScript : MonoBehaviour
{
    [Tooltip("Enter the name of the dialogue file this object is using.")]
    public string dialogueFile;

    private SpriteRenderer spriteRenderer;
    private GameObject gameController;
    private bool hasInteracted;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        gameController = GameObject.Find("GameController");
        hasInteracted = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && hasInteracted == false)
        {
            spriteRenderer.enabled = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (Input.GetAxis("Interact") > 0 && hasInteracted == false)
            {
                IEnumerator dialogueScene = gameController.GetComponent<WorldControl>().CutsceneDialogue(dialogueFile, 1);
                StartCoroutine(dialogueScene);
                hasInteracted = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spriteRenderer.enabled = false;
            hasInteracted = false;
        }
    }
}
