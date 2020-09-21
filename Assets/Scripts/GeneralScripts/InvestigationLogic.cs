using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvestigationLogic : MonoBehaviour
{
    public UnityEvent onAllObjectsInvestigated;
    public List<GameObject> investigatableObjects;

    private List<InspectObjectScript> inspectObjectScripts;
    private WorldControl worldControl;
    private bool allInvestigated;

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
