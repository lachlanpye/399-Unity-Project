using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Component that allows an object to be interacted with to trigger an event.
public class InspectObjectScript : MonoBehaviour
{
    public UnityEvent interactEvent;
    public bool hasInvestigated;

    private SpriteRenderer spriteRenderer;
    private GameObject gameController;
    private bool hasInteracted;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        gameController = GameObject.Find("GameController");
        hasInteracted = false;
        hasInvestigated = false;
    }

    /// <summary>
    /// Lachlan Pye
    /// Helper function that passes the name of a dialogue file to be displayed.
    /// </summary>
    /// <param name="fileName">The filename of the dialogue file to be displayed.</param>
    public void StartDialogue(string fileName)
    {
        IEnumerator dialogueScene = gameController.GetComponent<WorldControl>().CutsceneDialogue(fileName, 1);
        StartCoroutine(dialogueScene);
    }

    /// <summary>
    /// Lachlan Pye
    /// Shows the "interactable object" icon if the player enters the trigger range.
    /// </summary>
    /// <param name="col">The collider of the game object that entered the trigger area.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && hasInteracted == false)
        {
            spriteRenderer.enabled = true;
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player uses the Interact key, then call the event associated with this object.
    /// </summary>
    /// <param name="col">The collider of the game object that entered the trigger area.</param>
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

    /// <summary>
    /// Lachlan Pye
    /// Hides the "interactable object" icon if the player leaves the trigger range.
    /// </summary>
    /// <param name="col">The collider of the game object that entered the trigger area.</param>
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
