using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InspectObjectScript : MonoBehaviour
{
    public UnityEvent interactEvent;
    public bool hasInvestigated;

    private SpriteRenderer spriteRenderer;
    private GameObject gameController;
    private bool hasInteracted;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        gameController = GameObject.Find("GameController");
        hasInteracted = false;
        hasInvestigated = false;
    }

    public void StartDialogue(string fileName)
    {
        IEnumerator dialogueScene = gameController.GetComponent<WorldControl>().CutsceneDialogue(fileName, 1);
        StartCoroutine(dialogueScene);
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
                interactEvent.Invoke();
                hasInteracted = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spriteRenderer.enabled = false;
            if (hasInteracted == true)
            {
                hasInvestigated = true;
            }

            hasInteracted = false;
        }
    }
}
