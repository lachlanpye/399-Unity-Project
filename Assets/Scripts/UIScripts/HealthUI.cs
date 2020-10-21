using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Sprite[] healthSprites;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        SetHealth(0);
    }

    public void SetHealth(int currentHealth)
    {
        image.sprite = healthSprites[currentHealth];
    }
}
