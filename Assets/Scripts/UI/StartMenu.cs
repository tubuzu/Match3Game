// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject levelSelectPanel;
    public GameObject gameStartPanel;

    private void Start()
    {
        if (GameData.instance.firstSplashLaunch)
        {
            gameStartPanel.SetActive(true);
            levelSelectPanel.SetActive(false);
        }
        else
        {
            gameStartPanel.SetActive(false);
            levelSelectPanel.SetActive(true);
        }
    }

    public void Play()
    {
        gameStartPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        GameData.instance.firstSplashLaunch = false;
        SoundManager.instance.PlayRandomSFX();
    }

    public void Home()
    {
        gameStartPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        SoundManager.instance.PlayRandomSFX();
    }
}
