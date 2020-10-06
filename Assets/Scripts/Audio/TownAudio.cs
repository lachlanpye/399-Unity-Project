using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownAudio : MonoBehaviour
{
    #pragma warning disable 0649
    [SerializeField] private AudioClip townBGM;
    #pragma warning restore 0649

    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2f);
        AudioManager.publicInstance.PlayBGM(townBGM);
    }
}
