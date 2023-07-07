using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : MonoBehaviour
{
    public GameObject pausePanel;
    public bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
        isPaused = !isPaused;
        GameManager.instance.PauseGame(isPaused);
        if (isPaused)
        {
            pausePanel.SetActive(true);
        }
        else pausePanel.SetActive(false);
    }
}
