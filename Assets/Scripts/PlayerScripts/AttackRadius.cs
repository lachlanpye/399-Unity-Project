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

    //void OnTriggerStay2D(Collider2D col)
    //{
    //    if (col.gameObject.tag == "Enemy")
    //    {
    //        EnemyBehaviour enemyBehaviour = col.gameObject.GetComponent<EnemyBehaviour>();
    //        if (enemyBehaviour.currentState == EnemyBehaviour.State.Stunned)
    //        {
    //            playerBehaviour.AbleToAttack(col.gameObject);
    //        }
    //        else
    //        {
    //            playerBehaviour.NotAbleToAttack(col.gameObject);
    //        }
    //    }
    //}

private void OnTriggerStay2D(Collider2D collision)
{
    if (collision.gameObject.tag == "Enemy")
    {
        EnemyBehaviour enemyBehaviour = collision.gameObject.GetComponent<EnemyBehaviour>();
        if (enemyBehaviour.currentState == EnemyBehaviour.State.Stunned)
        {
            playerBehaviour.AbleToAttack(collision.gameObject);
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
