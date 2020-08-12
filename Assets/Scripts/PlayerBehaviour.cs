using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Tooltip("Number of pixels per second to move.")]
    public float moveSpeed;

    private float distance;

    // Update is called once per frame
    void Update()
    {
        distance = moveSpeed * 16 * Time.deltaTime;

        if (Input.GetAxis("Horizontal") > 0.5)
        {
            transform.Translate(new Vector3(distance, 0, 0));
        }
        else if (Input.GetAxis("Horizontal") < -0.5)
        {
            transform.Translate(new Vector3(-distance, 0, 0));
        }
        else
        {
            RoundPositionX();
        }

        if (Input.GetAxis("Vertical") > 0.5)
        {
            transform.Translate(new Vector3(0, distance, 0));
        }
        else if (Input.GetAxis("Vertical") < -0.5)
        {
            transform.Translate(new Vector3(0, -distance, 0));
        }
        else
        {
            RoundPositionY();
        }
    } 

    private void RoundPositionX()
    {
        Vector3 pos = transform.position;
        pos.x = (float) (Mathf.Round(pos.x));
        transform.position = pos;
    }
    private void RoundPositionY()
    {
        Vector3 pos = transform.position;
        pos.y = (float)(Mathf.Round(pos.y));
        transform.position = pos;
    }
}
