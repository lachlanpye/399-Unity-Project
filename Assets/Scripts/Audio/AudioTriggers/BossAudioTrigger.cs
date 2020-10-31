using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Janine Aunzo
/// Contains trigger functions for boss animation events.
/// Attach this script to all objects that use boss animations.
/// </summary>

public class BossAudioTrigger : MonoBehaviour
{
    private BossFightAudio bossAudio;
    // Start is called before the first frame update
    void Start()
    {
        bossAudio = GameObject.Find("BossFightAudio").GetComponent<BossFightAudio>();
    }

    /// <summary>
    /// Janine Aunzo
    /// Calls the method in BossFightAudio script.
    /// </summary>
    private void PlaySwipeSound()
    {
        bossAudio.PlaySwipe();
    }
}
