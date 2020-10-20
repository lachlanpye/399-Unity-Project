using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudioTrigger : MonoBehaviour
{
    //Trigger functions for animation events


    private BossFightAudio bossAudio;
    // Start is called before the first frame update
    void Start()
    {
        bossAudio = GameObject.Find("BossFightAudio").GetComponent<BossFightAudio>();
    }

    private void PlaySwipeSound()
    {
        // Function triggered by animation event
        bossAudio.PlaySwipe();
    }

    private void PlayStunSound()
    {
        bossAudio.PlayStun();
    }
}
