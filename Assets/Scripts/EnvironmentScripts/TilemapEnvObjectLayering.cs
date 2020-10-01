using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapEnvObjectLayering : MonoBehaviour
{
    public GameObject tilemap;
    private TilemapRenderer tilemapRenderer;

    void Start()
    {
        tilemapRenderer = tilemap.GetComponent<TilemapRenderer>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            tilemapRenderer.sortingLayerName = "FrontObject";
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            tilemapRenderer.sortingLayerName = "BehindObject";
        }
    }
}
