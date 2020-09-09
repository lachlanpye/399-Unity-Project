using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enviroment
{
    public class DayOrNightObjects : MonoBehaviour
    {
        public GameObject dayObjects;
        public GameObject nightObjects;

        public bool currentlyDay;

        [System.Serializable]
        public struct LightingColor
        {
            public GameObject sceneObject;
            public Color dayColor;
            public Color nightColor;

            [HideInInspector]
            public Component renderer;
        }
        [Space]
        public LightingColor[] dayOrNightLightingColors;

        void Start()
        {
            for (int i = 0; i < dayOrNightLightingColors.Length; i++)
            {
                if (dayOrNightLightingColors[i].sceneObject.GetComponent<SpriteRenderer>() != null)
                {
                    dayOrNightLightingColors[i].renderer = dayOrNightLightingColors[i].sceneObject.GetComponent<SpriteRenderer>();
                }
                else if (dayOrNightLightingColors[i].sceneObject.GetComponent<Tilemap>() != null)
                {
                    dayOrNightLightingColors[i].renderer = dayOrNightLightingColors[i].sceneObject.GetComponent<Tilemap>();
                }
                else
                {
                    dayOrNightLightingColors[i].renderer = null;
                }
            }
        }

        void Update()
        {
            if (currentlyDay == true)
            {
                ChangeToDay();
            }
            if (currentlyDay == false)
            {
                ChangeToNight();
            }
        }

        public void ChangeToDay()
        {
            dayObjects.SetActive(true);
            nightObjects.SetActive(false);

            for (int i = 0; i < dayOrNightLightingColors.Length; i++)
            {
                if (dayOrNightLightingColors[i].renderer != null)
                {
                    if (dayOrNightLightingColors[i].renderer is SpriteRenderer)
                    {
                        SpriteRenderer spriteRenderer = (SpriteRenderer)dayOrNightLightingColors[i].renderer;
                        spriteRenderer.color = dayOrNightLightingColors[i].dayColor;
                    }
                    else if (dayOrNightLightingColors[i].renderer is Tilemap)
                    {
                        Tilemap tilemapRenderer = (Tilemap)dayOrNightLightingColors[i].renderer;
                        tilemapRenderer.color = dayOrNightLightingColors[i].dayColor;
                    }
                }
            }
        }

        public void ChangeToNight()
        {
            dayObjects.SetActive(false);
            nightObjects.SetActive(true);

            for (int i = 0; i < dayOrNightLightingColors.Length; i++)
            {
                if (dayOrNightLightingColors[i].renderer != null)
                {
                    if (dayOrNightLightingColors[i].renderer is SpriteRenderer)
                    {
                        SpriteRenderer spriteRenderer = (SpriteRenderer)dayOrNightLightingColors[i].renderer;
                        spriteRenderer.color = dayOrNightLightingColors[i].nightColor;
                    }
                    else if (dayOrNightLightingColors[i].renderer is Tilemap)
                    {
                        Tilemap tilemapRenderer = (Tilemap)dayOrNightLightingColors[i].renderer;
                        tilemapRenderer.color = dayOrNightLightingColors[i].nightColor;
                    }
                }
            }
        }

        public void UpdateColorValues(LightingColor[] newLightingColors)
        {
            for (int i = 0; i < newLightingColors.Length; i++)
            {
                if (newLightingColors[i].sceneObject.GetComponent<SpriteRenderer>() != null)
                {
                    newLightingColors[i].renderer = newLightingColors[i].sceneObject.GetComponent<SpriteRenderer>();
                }
                else if (newLightingColors[i].sceneObject.GetComponent<Tilemap>() != null)
                {
                    newLightingColors[i].renderer = newLightingColors[i].sceneObject.GetComponent<Tilemap>();
                }
                else
                {
                    newLightingColors[i].renderer = null;
                }
            }
            for (int i = 0; i < newLightingColors.Length; i++)
            {
                if (newLightingColors[i].renderer != null)
                {
                    for (int j = 0; j < dayOrNightLightingColors.Length; j++)
                    {
                        if (GameObject.ReferenceEquals(newLightingColors[i].sceneObject, dayOrNightLightingColors[j].sceneObject))
                        {
                            dayOrNightLightingColors[j].dayColor = newLightingColors[i].dayColor;
                            dayOrNightLightingColors[j].nightColor = newLightingColors[i].nightColor;
                        }
                    }
                }
            }

            if (currentlyDay)
            {
                ChangeToDay();
            }
            else
            {
                ChangeToNight();
            }
        }
    }
}
