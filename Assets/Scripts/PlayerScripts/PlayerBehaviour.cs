﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [Tooltip("Number of pixels per second to move.")]
    public float moveSpeed;
    [Space]
    [Header("How far the distance is from the player in that direction where collisions are detected.")]
    public float upColliderDistance;
    public float leftColliderDistance;
    public float rightColliderDistance;
    public float downColliderDistance;
    [Space]

    [Tooltip("0 = stand front, 1-4 = walk front, 5 = stand back, 6-9 = walk back, 10 = stand right, 11-14 = walk right, 15 = stand left, 16-19 = walk left")]
    public Sprite[] playerSprites;
    [Tooltip("The delay between switching sprites while walking.")]
    public float walkingAnimSpeed;
    [Space]
    public GameObject gameController;
    public GameObject spotlight;

    [Space]
    public string currentArea;

    private SpriteRenderer spriteRenderer;
    private WorldControl worldControl;

    private float distance;
    private float timer;

    private int startAnim;
    private int currentAnim;
    private int endAnim;
    private string lastAxis;

    private RaycastHit2D upCast;
    private RaycastHit2D leftCast;
    private RaycastHit2D rightCast;
    private RaycastHit2D downCast;
    private int objectMask;

    private bool blockUp;
    private bool blockLeft;
    private bool blockRight;
    private bool blockDown;

    void Start()
    {
        timer = 0;
        startAnim = 0;
        currentAnim = 0;
        endAnim = 0;

        spriteRenderer = GetComponent<SpriteRenderer>();
        worldControl = gameController.GetComponent<WorldControl>();

        lastAxis = "down";
        objectMask = LayerMask.GetMask("Object");

        blockUp = false;
        blockLeft = false;
        blockRight = false;
        blockDown = false;
    }

    void Update()
    {
        distance = moveSpeed * 0.5f * Time.deltaTime;

        if (worldControl.DialogueActive() == false)
        {
            if (Input.GetAxis("Horizontal") > 0.5 && blockRight == false)
            {
                transform.Translate(new Vector3(distance, 0, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, -90);

                startAnim = 11;
                endAnim = 14;
                lastAxis = "right";
            }
            else if (Input.GetAxis("Horizontal") < -0.5 && blockLeft == false)
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
                }
                else if (lastAxis == "left")
                {
                    startAnim = 15;
                    endAnim = 15;
                }
                RoundPositionX();
            }

            if (Input.GetAxis("Vertical") > 0.5 && blockUp == false)
            {
                transform.Translate(new Vector3(0, distance, 0));
                spotlight.transform.eulerAngles = new Vector3(0, 0, 0);

                startAnim = 6;
                endAnim = 9;
                lastAxis = "up";
            }
            else if (Input.GetAxis("Vertical") < -0.5 && blockDown == false)
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

            spriteRenderer.sprite = playerSprites[currentAnim];
        }

        // Cast rays in all 4 directions for wall detection
        upCast = Physics2D.Raycast(transform.position - (Vector3.up * 16), Vector2.up, upColliderDistance, objectMask);
        leftCast = Physics2D.Raycast(transform.position - (Vector3.up * 16), Vector2.left, leftColliderDistance, objectMask);
        rightCast = Physics2D.Raycast(transform.position - (Vector3.up * 16), Vector2.right, rightColliderDistance, objectMask);
        downCast = Physics2D.Raycast(transform.position - (Vector3.up * 16), Vector2.down, downColliderDistance, objectMask);

        if (upCast.collider != null && upCast.collider.tag == "Wall")
        {
            blockUp = true;
        }
        else { blockUp = false; }

        if (leftCast.collider != null && leftCast.collider.tag == "Wall")
        {
            blockLeft = true;
        }
        else { blockLeft = false; }

        if (rightCast.collider != null && rightCast.collider.tag == "Wall")
        {
            blockRight = true;
        }
        else { blockRight = false; }

        if (downCast.collider != null && downCast.collider.tag == "Wall")
        {
            blockDown = true;
        }
        else { blockDown = false; }
    } 

    public void SetArea(string area)
    {
        currentArea = area;
    }

    private void RoundPositionX()
    {
        /*Vector3 pos = transform.position;
        pos.x = (float) (Mathf.Round(pos.x));
        transform.position = pos;*/
    }
    private void RoundPositionY()
    {
        /*Vector3 pos = transform.position;
        pos.y = (float)(Mathf.Round(pos.y));
        transform.position = pos;*/
    }
}