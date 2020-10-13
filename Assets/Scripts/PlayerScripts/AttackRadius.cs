using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRadius : MonoBehaviour
{
    private PlayerBehaviour playerBehaviour;

    void Start()
    {
        playerBehaviour = GetComponentInParent<PlayerBehaviour>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            EnemyBehaviourV2 enemyBehaviour = col.gameObject.GetComponent<EnemyBehaviourV2>();
            if (enemyBehaviour.currentState == EnemyBehaviourV2.State.Stunned)
            {
                playerBehaviour.AbleToAttack(col.gameObject);
            }
            else
            {
                playerBehaviour.NotAbleToAttack(col.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            EnemyBehaviourV2 enemyBehaviour = collision.gameObject.GetComponent<EnemyBehaviourV2>();
            if (enemyBehaviour.currentState == EnemyBehaviourV2.State.MoveIn || enemyBehaviour.currentState == EnemyBehaviourV2.State.Attack)
            {
                return;
                //player takes damage and plays enemy attack animation
                //must play this as a player animation as I have placed player in centre of each frame 
                //so that we will hopefully avoid any weird jumps when transitioning from current player animation to being attacked animation (enemy won't appear to jump as it isn't visible before this animation)

                //can potentially get rid of MoveIn comparison, it was just incase the enemy hadn't moved to attack state quick enough since this is only called once on enter
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            playerBehaviour.NotAbleToAttack(col.gameObject);
        }
    }
}
