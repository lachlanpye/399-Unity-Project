using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayOrNightObjects : MonoBehaviour
{
    public GameObject dayObjects;
    public GameObject nightObjects;

    public bool currentlyDay;

    void Update()
    {
        if (currentlyDay == true)
        {
            ChangeToDay();
        }
        if (currentlyDay == false)
        {
            ChangeToNight();
        }
    }

    public void ChangeToDay()
    {
        dayObjects.SetActive(true);
        nightObjects.SetActive(false);
    }

    public void ChangeToNight()
    {
        dayObjects.SetActive(false);
        nightObjects.SetActive(true);
    }
}
