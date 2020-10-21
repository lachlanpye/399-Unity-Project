using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSwipeRadius : MonoBehaviour
{
    private BossBehaviour bossBehaviour;
    public bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        bossBehaviour = GetComponentInParent<BossBehaviour>();
        playerInRange = false;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player" && bossBehaviour.BossIsStunned() == false)
        {
            playerInRange = true;
            bossBehaviour.SwipeAttack(GetComponent<BossSwipeRadius>());
        }

        if (col.gameObject.tag == "Player" && bossBehaviour.BossIsStunned() == true)
        {
            bossBehaviour.AttackIndicatorActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            bossBehaviour.AttackIndicatorActive(false);
            playerInRange = false;
        }
    }
    
}
