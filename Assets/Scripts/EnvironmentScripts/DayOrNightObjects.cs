using System.Collections;

using UnityEngine;
using UnityEngine.Tilemaps;

namespace Enviroment
{
    // Component that controls the colors of a list of game objects and which game objects are active or inactive.
    public class DayOrNightObjects : MonoBehaviour
    {
        public GameObject gameController;
        public GameObject dayObjects;
        public GameObject nightObjects;

        public bool currentlyDay;

        /// <summary>
        /// Lachlan Pye
        /// Struct that controls the day and night color of an object within the scene.
        /// </summary>
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

        private WorldControl worldControl;

        /// <summary>
        /// Lachlan Pye
        /// Get the renderers for every game object that needs to be re-colored.
        /// </summary>
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

            worldControl = gameController.GetComponent<WorldControl>();
        }

        /// <summary>
        /// Lachlan Pye
        /// Switch to day or night colors depending on the time of day.
        /// </summary>
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

        /// <summary>
        /// Lachlan Pye
        /// Enable daytime objects and change the colors of each game object to their day color.
        /// </summary>
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

        /// <summary>
        /// Lachlan Pye
        /// Enable nighttime objects and change the colors of each game object to their night color.
        /// </summary>
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

        /// <summary>
        /// Lachlan Pye
        /// Helper function that starts the DelayedColorUpdate coroutine.
        /// </summary>
        /// <param name="newLightingColors">The list of new lighting colors to be set.</param>
        public void UpdateColorValues(LightingColor[] newLightingColors)
        {
            IEnumerator delayedUpdateColors = DelayedUpdateColors(newLightingColors);
            StartCoroutine(delayedUpdateColors);
        }

        /// <summary>
        /// Lachlan Pye
        /// Overwrites the colors of the game objects that are already stored in the LightingColors list.
        /// </summary>
        /// <param name="newLightingColors">The list of new lighting colors to be set.</param>
        public void UpdateColorValuesImmediate(LightingColor[] newLightingColors)
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

        /// <summary>
        /// Lachlan Pye
        /// After waiting for the length of a fade out transition, update color values.
        /// </summary>
        /// <param name="newLightingColors">The list of new lighting colors to be set.</param>
        public IEnumerator DelayedUpdateColors(LightingColor[] newLightingColors)
        {
            for (int i = 0; i < worldControl.fadeTransitionTime; i++)
            {
                yield return new WaitForFixedUpdate();
            }

            UpdateColorValuesImmediate(newLightingColors);

            yield return null;
        }
    }
}
