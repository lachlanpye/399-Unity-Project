using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAudio : MonoBehaviour
{

    [SerializeField] private AudioClip gameOverBGM;

    // Start is called before the first frame update
    public void playGameOver()
    {
        AudioManager.publicInstance.FadeInBGM(gameOverBGM);
    }
}
