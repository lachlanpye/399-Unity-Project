using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;


public class TvStaticTrigger : MonoBehaviour
{
    public UnityEvent tvStaticEvent;

    void Awake()
    {
        if (tvStaticEvent == null)
        {
            tvStaticEvent = new UnityEvent();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            tvStaticEvent.Invoke();
        }
    }
}

