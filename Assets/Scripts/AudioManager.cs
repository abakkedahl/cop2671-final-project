using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource BackGroundMusic;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeBGM(AudioClip music)
    {
        BackGroundMusic.Stop();
        BackGroundMusic.clip = music;
        BackGroundMusic.Play();
    }
}
