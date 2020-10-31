using UnityEngine;
using UnityEngine.Tilemaps;

// Component that changes the sorting layer for tilemaps.
public class TilemapEnvObjectLayering : MonoBehaviour
{
    public GameObject tilemap;
    private TilemapRenderer tilemapRenderer;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables.
    /// </summary>
    void Start()
    {
        tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player is in the trigger area, then sort the tilemap to be in front of the player.
    /// </summary>
    /// <param name="col">The collider of the game object that entered the trigger area.</param>
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            tilemapRenderer.sortingLayerName = "FrontObject";
        }
    }

    /// <summary>
    /// Lachlan Pye
    /// If the player exits the trigger area, then sort the tilemap to be behind the player.
    /// </summary>
    /// <param name="col">The collider of the game object that entered the trigger area.</param>
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            tilemapRenderer.sortingLayerName = "BehindObject";
        }
    }
}
