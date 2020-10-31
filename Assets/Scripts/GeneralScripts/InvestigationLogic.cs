using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Component that calls a function when all objects have been checked at least once.
public class InvestigationLogic : MonoBehaviour
{
    public UnityEvent onAllObjectsInvestigated;
    public List<GameObject> investigatableObjects;

    private List<InspectObjectScript> inspectObjectScripts;
    private WorldControl worldControl;
    private bool allInvestigated;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        inspectObjectScripts = new List<InspectObjectScript>();
        foreach (GameObject obj in investigatableObjects)
        {
            inspectObjectScripts.Add(obj.GetComponent<InspectObjectScript>());
        }
        allInvestigated = false;

        if (onAllObjectsInvestigated == null)
        {
            onAllObjectsInvestigated = new UnityEvent();
        }

        worldControl = GameObject.Find("GameController").GetComponent<WorldControl>();
    }

    /// <summary>
    /// Lachlan Pye
    /// Checks if every object has been investigated. If true, then it triggers an event
    /// and destroys itself.
    /// </summary>
    void Update()
    {
        allInvestigated = true;
        foreach (InspectObjectScript script in inspectObjectScripts)
        {
            if (script.hasInvestigated == false)
            {
                allInvestigated = false;
            }
        }

        if (allInvestigated == true && worldControl.paused == false)
        {
            onAllObjectsInvestigated.Invoke();
            Destroy(gameObject);
        }
    }
}
