using System.Collections;
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
    private int frequencyOfNodes;

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
        frequencyOfNodes = Mathf.RoundToInt(Vector3.Magnitude(distance) / nodeFrequency);

        // Orthogonal direction vector between enemy and player
        orthogonalVector = Vector3.Normalize(Vector3.Cross(distance, new Vector3(0, 0, -90)));
    
        float positionOnLine = 1.0f / (float) frequencyOfNodes;
        nextPosition = transform.position 
                        + (distance * positionOnLine)
                        + (orthogonalVector * sineWaveAmplitude * Mathf.Sin((Time.time * sineWaveFrequency) + 2*Mathf.PI*(positionOnLine)));

        transform.Translate((nextPosition - transform.position).normalized * moveSpeed * 16 * Time.deltaTime, Space.World);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(nextPosition, 8);
    }
}
