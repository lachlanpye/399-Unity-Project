using System.Collections;
using UnityEngine;

public class PentagramAttackLogic : MonoBehaviour
{
    public float waitBeforeLaser;

    private GameObject playerObject;
    private GameObject laserObject;

    private Animator laserAnimator;

    private bool playerInRange;

    private BossFightAudio laserAudio;

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

    public void BeginPentagramSequence()
    {
        StartCoroutine(PentagramSequenceCoroutine());
    }

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
            StartCoroutine(GameObject.FindGameObjectWithTag("GameController").GetComponent<WorldControl>().TakeBossDamage());
        }

        anim = "FadeOut";
        animator.SetTrigger(anim);
        yield return new WaitForSeconds(0.5823f);
        laserObject.SetActive(false);
        gameObject.SetActive(false);
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerObject = col.gameObject;
            playerInRange = true;
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
