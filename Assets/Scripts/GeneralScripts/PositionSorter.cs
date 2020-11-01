using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Written by Tyler
public class PositionSorter : MonoBehaviour
{
    public int sortingOrderBase = 5000;
    public int offset = 0;
    public bool runOnlyOnce = false;

    private bool isRepeating = false;
    private Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        myRenderer = gameObject.GetComponent<Renderer>();

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!isRepeating)
        {
            if (runOnlyOnce)
            {
                myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
                Destroy(this);
            } else
            {
                InvokeRepeating("UpdateSorting", 0f, 0.1f);
                isRepeating = true;
            }
        }
        
    }
    void UpdateSorting()
    {
        myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
    }
}
