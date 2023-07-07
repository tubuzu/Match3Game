using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelConfirmPanel : MonoBehaviour
{
    public int level;
    private int starsActive;
    private int highScore;

    public Image[] stars;
    public Text highScoreText;
    public Text starText;

    public string gameSceneName;

    public void Setup(int level)
    {
        this.level = level;

        if (GameData.instance != null)
        {
            starsActive = GameData.instance.saveData.stars[level - 1];
            highScore = GameData.instance.saveData.highScores[level - 1];
        }

        highScoreText.text = "" + highScore;
        starText.text = "" + starsActive + "/3";
        for (int i = 0; i < 3; i++)
        {
            if (i < starsActive)
                stars[i].enabled = true;
            else
                stars[i].enabled = false;
        }
    }

    public void Play()
    {
        GameData.instance.Save();
        PlayerPrefs.SetInt("Current Level", level - 1);
        SoundManager.instance.PlayRandomSFX();
        SceneManager.LoadScene(gameSceneName);
    }

    public void Close()
    {
        SoundManager.instance.PlayRandomSFX();
        gameObject.SetActive(false);
    }
}
