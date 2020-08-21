using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_room : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other){
        Debug.Log("Player detected.");
    }
}
