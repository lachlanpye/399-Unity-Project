using UnityEngine;

// Component that controls what happens when the player gets close to the boss.
public class BossSwipeRadius : MonoBehaviour
{
    private BossBehaviour bossBehaviour;
    public bool playerInRange;
    private PlayerAudioTrigger playerAudioTrigger;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        bossBehaviour = GetComponentInParent<BossBehaviour>();
        playerInRange = false;
        
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player enters the trigger area and the boss is stunned, then show the attack indicator.
    /// If the boss is not stunned, then play the boss' swipe attack.
    /// </summary>
    /// <param name="col">The collider of the game object that just entered the trigger area.</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if (playerAudioTrigger == null)
        {
            playerAudioTrigger = GameObject.Find("Player").GetComponent<PlayerAudioTrigger>();
        }

        if (col.gameObject.tag == "Player" && bossBehaviour.BossIsStunned() == false)
        {
            playerInRange = true;
            bossBehaviour.SwipeAttack(GetComponent<BossSwipeRadius>());
        }

        if (col.gameObject.tag == "Player" && bossBehaviour.BossIsStunned() == true)
        {
            bossBehaviour.AttackIndicatorActive(true);
            playerAudioTrigger.hitBoss = true;
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player leaves the trigger area, then hide the attack indicator.
    /// </summary>
    /// <param name="col">The collider of the game object that just exited the trigger area.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            bossBehaviour.AttackIndicatorActive(false);
            playerInRange = false;
            playerAudioTrigger.hitBoss = false;
        }
    }
    
}
