using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource[] sfx;
    public AudioSource[] backgroundMusic;

    [Range(0, 1)]
    public float maxBgMusic;
    [Range(0, 1)]
    public float maxSFX;

    private void Awake()
    {
        SoundManager.instance = this;
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            TurnOnMusic(true);
            int soundToPlay = Random.Range(0, backgroundMusic.Length);
            backgroundMusic[soundToPlay].Play();
        }
    }

    public void PlayRandomSFX()
    {
        if (PlayerPrefs.GetInt("Sound", 1) == 1)
        {
            //Choose a random number
            int soundToPlay = Random.Range(0, sfx.Length);
            //play that clip
            sfx[soundToPlay].Play();
        }
    }

    public void TurnOnMusic(bool isOn)
    {
        if (isOn)
        {
            foreach (AudioSource aus in sfx)
            {
                aus.volume = 1 * maxSFX;
            }
            foreach (AudioSource aus in backgroundMusic)
            {
                aus.volume = 1 * maxBgMusic;
            }
        }
        else
        {
            foreach (AudioSource aus in sfx)
            {
                aus.volume = 0;
            }
            foreach (AudioSource aus in backgroundMusic)
            {
                aus.volume = 0;
            }
        }
    }
}
