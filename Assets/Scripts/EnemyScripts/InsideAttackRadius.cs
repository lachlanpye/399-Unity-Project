using UnityEngine;

// Component that tells the enemy object when the player is close.
public class InsideAttackRadius : MonoBehaviour
{
    private EnemyBehaviour behaviour;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        behaviour = transform.parent.gameObject.GetComponent<EnemyBehaviour>();
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player is inside the trigger range, then tell the enemy behaviour
    /// component the player is close.
    /// </summary>
    /// <param name="col">The collider of the game object that is in the trigger area.</param>
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            behaviour.PlayerEntersRange();
        }
    }
}
