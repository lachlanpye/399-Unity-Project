using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Tooltip("Number of pixels per second to move.")]
    public float moveSpeed;
    [Space]

    [Tooltip("0 = stand front, 1-4 = walk front, 5 = stand back, 6-9 = walk back, 10 = stand right, 11-14 = walk right, 15 = stand left, 16-19 = walk left")]
    public Sprite[] playerSprites;
    [Tooltip("The delay between switching sprites while walking.")]
    public float walkingAnimSpeed;
    [Space]
    public GameObject spotlight;

    private SpriteRenderer renderer;

    private float distance;
    private float timer;

    private int startAnim;
    private int currentAnim;
    private int endAnim;

    private string lastAxis;

    void Start()
    {
        timer = 0;
        startAnim = 0;
        currentAnim = 0;
        endAnim = 0;

        renderer = GetComponent<SpriteRenderer>();
        lastAxis = "down";
    }

    // Update is called once per frame
    void Update()
    {
        distance = moveSpeed * 16 * Time.deltaTime;

        if (Input.GetAxis("Horizontal") > 0.5)
        {
            transform.Translate(new Vector3(distance, 0, 0));
            spotlight.transform.eulerAngles = new Vector3(0, 0, -90);

            startAnim = 11;
            endAnim = 14;
            lastAxis = "right";
        }
        else if (Input.GetAxis("Horizontal") < -0.5)
        {
            transform.Translate(new Vector3(-distance, 0, 0));
            spotlight.transform.eulerAngles = new Vector3(0, 0, 90);

            startAnim = 16;
            endAnim = 19;
            lastAxis = "left";
        }
        else
        {
            // Set standing pos to lastAxis
            if (lastAxis == "right")
            {
                startAnim = 10;
                endAnim = 10;
            } else if (lastAxis == "left")
            {
                startAnim = 15;
                endAnim = 15;
            }
            RoundPositionX();
        }

        if (Input.GetAxis("Vertical") > 0.5)
        {
            transform.Translate(new Vector3(0, distance, 0));
            spotlight.transform.eulerAngles = new Vector3(0, 0, 0);

            startAnim = 6;
            endAnim = 9;
            lastAxis = "up";
        }
        else if (Input.GetAxis("Vertical") < -0.5)
        {
            transform.Translate(new Vector3(0, -distance, 0));
            spotlight.transform.eulerAngles = new Vector3(0, 0, 180);

            startAnim = 1;
            endAnim = 4;
            lastAxis = "down";
        }
        else
        {
            // Set standing pos to lastAxis
            if (lastAxis == "up")
            {
                startAnim = 6;
                endAnim = 6;
            }
            else if (lastAxis == "down")
            {
                startAnim = 0;
                endAnim = 0;
            }
            RoundPositionY();
        }

        if (currentAnim <= startAnim)
        {
            currentAnim = startAnim;
        }
        if (currentAnim > endAnim)
        {
            currentAnim = endAnim;
        }

        timer += Time.deltaTime;
        if (timer >= walkingAnimSpeed)
        {
            timer = 0;
            currentAnim += 1;
            if (currentAnim > endAnim)
            {
                currentAnim = startAnim;
            }
        }

        renderer.sprite = playerSprites[currentAnim];
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
