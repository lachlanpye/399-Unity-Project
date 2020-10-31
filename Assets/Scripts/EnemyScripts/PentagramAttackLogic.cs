using System.Collections;
using UnityEngine;

// Component that controls how the pentagram operates.
public class PentagramAttackLogic : MonoBehaviour
{
    public float waitBeforeLaser;

    private GameObject playerObject;
    private GameObject laserObject;

    private Animator laserAnimator;

    private bool playerInRange;

    private BossFightAudio laserAudio;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        foreach (Transform t in transform)
        {
            if (t.gameObject.name == "Laser")
            {
                laserObject = t.gameObject;
            }
        }

        laserAnimator = laserObject.GetComponent<Animator>();
        laserObject.SetActive(false);
    }

    /// <summary>
    /// Lachlan Pye
    /// Helper function that begins the attack sequence of the pentagram.
    /// </summary>
    public void BeginPentagramSequence()
    {
        StartCoroutine(PentagramSequenceCoroutine());
    }

    /// <summary>
    /// Lachlan Pye
    /// Fades in the pentagram and waits an amount of time before playing the laser attack.
    /// If the player is in the trigger area, then damage the player. After this, fade out the 
    /// pentagram and hide it from view.
    /// </summary>
    private IEnumerator PentagramSequenceCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        string anim = "FadeIn";

        laserAudio = GameObject.Find("BossFightAudio").GetComponent<BossFightAudio>();
        laserAudio.PlayPentagram();

        animator.SetTrigger(anim);
        yield return new WaitForSeconds(0.333f);
        yield return new WaitForSeconds(waitBeforeLaser);

        laserObject.SetActive(true);
        laserAudio.PlayLaser();
        if (playerInRange)
        {
            GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldControl>().StartBossDamageCoroutine();
        }

        anim = "FadeOut";
        animator.SetTrigger(anim);
        yield return new WaitForSeconds(0.5823f);
        laserObject.SetActive(false);
        gameObject.SetActive(false);
        yield return null;
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player enters the trigger area, set the playerInRange bool to true.
    /// </summary>
    /// <param name="col">The collider of the gameobject that just entered the trigger area.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerObject = col.gameObject;
            playerInRange = true;
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player leaves the trigger area, set the playerInRange bool to false.
    /// </summary>
    /// <param name="col">The collider of the gameobject that just exited the trigger area.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
