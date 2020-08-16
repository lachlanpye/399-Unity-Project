﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    public float moveSpeed;
    [Tooltip("The amount of waypoint nodes created for each segment of the path.")]
    public int nodeFrequency;
    [Space]
    public float sineWaveFrequency;
    public int sineWaveAmplitude;

    private GameObject player;
    private Vector3 orthogonalVector;
    private Vector3 nextPosition;
    private int intervalOfNodes;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        nextPosition = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 distance = player.GetComponent<Transform>().position - transform.position;
        intervalOfNodes = Mathf.RoundToInt(Vector3.Magnitude(distance) / nodeFrequency);

        // Orthogonal direction vector between enemy and player
        orthogonalVector = Vector3.Normalize(Vector3.Cross(distance, new Vector3(0, 0, -90)));

        nextPosition = transform.position
                        + (distance / intervalOfNodes)
                        + (orthogonalVector * sineWaveAmplitude * Mathf.Sin(Time.time * sineWaveFrequency));

        transform.Translate((nextPosition - transform.position).normalized * moveSpeed * 16 * Time.deltaTime, Space.World);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere((nextPosition - transform.position).normalized * moveSpeed * Time.deltaTime, 8);
    }
}
