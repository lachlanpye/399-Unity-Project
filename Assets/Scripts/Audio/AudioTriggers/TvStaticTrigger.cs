using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

/// <summary>
/// Janine Aunzo
/// Calls TV static audio method specified in inspector.
/// Attach this script to zones that lead to and leave the living room area.
/// </summary>

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

    /// <summary>
    /// Janine Aunzo
    /// Triggers event that either plays or stops TV static audio.
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            tvStaticEvent.Invoke();
        }
    }
}

