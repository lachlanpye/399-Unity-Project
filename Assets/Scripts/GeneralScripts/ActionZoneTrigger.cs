using UnityEngine;
using UnityEngine.Events;

// Component that triggers an event when the player walks into the trigger area.
public class ActionZoneTrigger : MonoBehaviour
{
    [Header("Setting up a scene move:", order = 0)]
    [Space(-10, order = 1)]
    [Header("1) Use 'GameController' as the GameObject.", order = 2)]
    [Space(-10, order = 3)]
    [Header("2) Select WorldControl > MoveScenes as the function.", order = 4)]
    [Space(-10, order = 5)]
    [Header("3) Enter the name of the scene. You can add scenes by selecting the", order = 6)]
    [Space(-10, order = 7)]
    [Header("'GameController' object and looking at the WorldControl script.", order = 8)]
    [Space(order = 9)]
    public UnityEvent triggerEvent;

    /// <summary>
    /// Lachlan Pye
    /// Initialize null event if it has not been set via the Inspector.
    /// </summary>
    void Awake()
    {
        if (triggerEvent == null)
        {
            triggerEvent = new UnityEvent();
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player enters the trigger area, trigger the event.
    /// </summary>
    /// <param name="col">The collider of the object that just entered the trigger.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            triggerEvent.Invoke();
        }
    }
}
