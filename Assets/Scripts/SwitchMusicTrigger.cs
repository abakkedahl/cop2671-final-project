using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchMusicTrigger : MonoBehaviour
{
    public AudioClip newTrack;
    public Canvas canvasToCheck; // Assign the Canvas object in the Inspector

    private AudioManager theAM;
    private bool hasSwitched = false; // To ensure the music switches only once

    void Start()
    {
        theAM = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (!canvasToCheck.gameObject.activeInHierarchy && !hasSwitched)
        {
            if (newTrack != null)
            {
                Debug.Log("Switching music to: " + newTrack.name);
                theAM.ChangeBGM(newTrack);
                hasSwitched = true; // Prevent further switching
            }
        }
    }
}
