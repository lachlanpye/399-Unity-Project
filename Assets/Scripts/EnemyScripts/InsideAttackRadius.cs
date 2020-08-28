using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsideAttackRadius : MonoBehaviour
{
    private EnemyBehaviour behaviour;

    void Start()
    {
        behaviour = transform.parent.gameObject.GetComponent<EnemyBehaviour>();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            behaviour.PlayerEntersRange();
        }
    }
}
