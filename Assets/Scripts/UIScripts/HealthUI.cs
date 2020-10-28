using UnityEngine;
using UnityEngine.UI;

// Component used to change the current look of the health UI gameobject.
public class HealthUI : MonoBehaviour
{
    public Sprite[] healthSprites;

    private Image image;

    /// <summary>
    /// Lachlan Pye
    /// Initialize variables and set initial health sprite.
    /// </summary>
    void Start()
    {
        image = GetComponent<Image>();
        SetHealth(0);
    }

    /// <summary>
    /// Lachlan Pye
    /// Change the health sprite depending on the currentHealth parameter.
    /// </summary>
    /// <param name="currentHealth">The health of the player.</param>
    public void SetHealth(int currentHealth)
    {
        image.sprite = healthSprites[currentHealth];
    }
}
