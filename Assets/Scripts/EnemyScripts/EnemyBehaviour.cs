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

    [Space]
    public GameObject playerObject;
    public GameObject gameController;
    [Space]
    [Tooltip("This must have the same name as one of the scenes in 'GameController|WorldControl'.")]
    public string enemyArea;

    private SpriteRenderer spriteRenderer;
    private WorldControl worldControl;

    private Vector3 orthogonalVector;
    private Vector3 nextPosition;
    private int intervalOfNodes;

    private PlayerBehaviour playerBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        nextPosition = new Vector3();

        spriteRenderer = GetComponent<SpriteRenderer>();
        playerBehaviour = playerObject.GetComponent<PlayerBehaviour>();
        worldControl = gameController.GetComponent<WorldControl>();

        UpdateOpacity(0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerBehaviour.currentArea == enemyArea && !worldControl.DialogueActive())
        {
            Vector3 distance = playerObject.GetComponent<Transform>().position - transform.position;
            intervalOfNodes = Mathf.RoundToInt(Vector3.Magnitude(distance) / nodeFrequency);

            // Orthogonal direction vector between enemy and player
            orthogonalVector = Vector3.Normalize(Vector3.Cross(distance, new Vector3(0, 0, -90)));

            nextPosition = transform.position
                            + (distance / intervalOfNodes)
                            + (orthogonalVector * sineWaveAmplitude * Mathf.Sin(Time.time * sineWaveFrequency));

            transform.Translate((nextPosition - transform.position).normalized * moveSpeed * 16 * Time.deltaTime, Space.World);
        }
    }

    public void PlayerEntersRange()
    {
        Debug.Log("player in range");
    }

    public void UpdateOpacity(float value)
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, value);
    }
}
