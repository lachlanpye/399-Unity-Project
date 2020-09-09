using UnityEngine;
using UnityEngine.Events;

namespace Enviroment
{
    public class ZoneChangeObjectColor : MonoBehaviour
    {
        public DayOrNightObjects.LightingColor[] newLightingColors;

        [System.Serializable]
        public class ChangeColorEvent : UnityEvent<DayOrNightObjects.LightingColor[]> { };
        [SerializeField]
        [Header("Setting up object color changes:", order = 0)]
        [Space(-10, order = 1)]
        [Header("1) Use 'SceneObjects' as the GameObject.", order = 2)]
        [Space(-10, order = 3)]
        [Header("2) Select DayOrNightObjects > MoveScenes as the function.", order = 4)]
        [Space(-10, order = 5)]
        [Header("3) Set up the newLightingColors array as you want the objects to appear.", order = 6)]
        [Space(-10, order = 7)]
        [Header("Upon entering this trigger, the colors will be updated.", order = 8)]
        [Space(order = 9)]
        public ChangeColorEvent changeColorEvent;

        void Awake()
        {
            if (changeColorEvent == null)
            {
                changeColorEvent = new ChangeColorEvent();
            }
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.tag == "Player")
            {
                changeColorEvent.Invoke(newLightingColors);
            }
        }
    }
}
