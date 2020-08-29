using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InspectObjectScript : MonoBehaviour
{
    [Header("When the player interacts with the object, ", order = 0)]
    [Space(-10, order = 1)]
    [Header("the function set here will be called.", order = 2)]
    [Space(-10, order = 3)]
    [Header("Use WorldControl > DialogueScene to set up a dialogue", order = 4)]
    [Space(-10, order = 5)]
    [Header("trigger, with the text file name as the parameter.", order = 6)]
    [Space(order = 7)]
    public UnityEvent triggerEvent;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;

        if (triggerEvent == null)
        {
            triggerEvent = new UnityEvent();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spriteRenderer.enabled = true;
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            if (Input.GetAxis("Interact") > 0)
            {
                triggerEvent.Invoke();
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            spriteRenderer.enabled = false;
        }
    }
}
