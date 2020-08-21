using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will teleport the player, the camera to desired room. Will also change base layer colour.

public class change_room : MonoBehaviour
{
    public GameObject Player;
    public GameObject Camera;

    public void OnTriggerEnter2D(Collider2D other){
        Debug.Log("Player detected.");
        Player.transform.position = new Vector3(-12,-1.17f,0);
        Camera.transform.position = new Vector3(-17.51f,-1,-10);
    }
}
