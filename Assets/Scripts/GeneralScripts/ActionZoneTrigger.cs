using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionZoneTrigger : MonoBehaviour
{
    public UnityEvent triggerEvent;

    void Awake()
    {
        if (triggerEvent == null)
        {
            triggerEvent = new UnityEvent();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            triggerEvent.Invoke();
        }
    }
}
