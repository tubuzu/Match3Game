// using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Animator fadePanelAnim;
    public Animator gameIntroAnim;
    public Animator winPanelAnim;
    public Animator losePanelAnim;
    public Animator pausePanelAnim;

    public GameObject fadePanel;
    public GameObject gameIntroPanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    public Text winCoinText;
    public Text winStarText;
    public Text loseCoinText;
    public Text loseStarText;

    public Image musicButtonImage;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    public Text musicStatusText;
    public bool isPaused = false;

    public string splashSceneName;

    private void Awake()
    {
        UIManager.instance = this;
        this.fadePanelAnim = fadePanel.GetComponent<Animator>();
        this.gameIntroAnim = gameIntroPanel.GetComponent<Animator>();
        this.winPanelAnim = winPanel.GetComponent<Animator>();
        this.losePanelAnim = losePanel.GetComponent<Animator>();
        this.pausePanelAnim = pausePanel.GetComponent<Animator>();
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Sound") && PlayerPrefs.GetInt("Sound") == 0)
        {
            SoundManager.instance.TurnOnMusic(false);
            musicButtonImage.sprite = musicOffSprite;
            musicStatusText.text = "Music: Off";
        }
        else
        {
            musicButtonImage.sprite = musicOnSprite;
            musicStatusText.text = "Music: On";
        }
    }



    public void SoundButtonOnclick()
    {
        if (PlayerPrefs.HasKey("Sound") && PlayerPrefs.GetInt("Sound") == 0)
        {
            musicButtonImage.sprite = musicOnSprite;
            PlayerPrefs.SetInt("Sound", 1);
            SoundManager.instance.TurnOnMusic(true);
            musicStatusText.text = "Music: On";
        }
        else
        {
            PlayerPrefs.SetInt("Sound", 0);
            SoundManager.instance.TurnOnMusic(false);
            musicButtonImage.sprite = musicOffSprite;
            musicStatusText.text = "Music: Off";
        }
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(splashSceneName);
    }

    public void StartGame()
    {
        ShowUIGameIntro(false);

        StartCoroutine(GameManager.instance.GameStart());
    }

    public void ShowUIGameIntro(bool show)
    {
        if (show)
        {
            fadePanel.SetActive(true);
            gameIntroPanel.SetActive(true);
        }
        else
        {
            fadePanelAnim.SetTrigger("Out");
            gameIntroAnim.SetTrigger("Out");
            Invoke(nameof(OnFadeOut), .5f);
        }
    }

    public void ShowUIWinGame(bool show)
    {
        if (show)
        {
            fadePanel.SetActive(true);
            winPanel.SetActive(true);
            winCoinText.text = ScoreManager.instance.score.ToString();
            winStarText.text = ScoreManager.instance.starAchieved.ToString() + "/3";
        }
        else
        {
            fadePanelAnim.SetTrigger("Out");
            winPanelAnim.SetTrigger("Out");
            Invoke(nameof(OnFadeOut), .5f);
        }
    }

    public void ShowUILoseGame(bool show)
    {
        if (show)
        {
            fadePanel.SetActive(true);
            losePanel.SetActive(true);
            loseCoinText.text = ScoreManager.instance.score.ToString();
            loseStarText.text = ScoreManager.instance.starAchieved.ToString() + "/3";
        }
        else
        {
            fadePanelAnim.SetTrigger("Out");
            losePanelAnim.SetTrigger("Out");
            Invoke(nameof(OnFadeOut), .5f);
        }
    }

    public void ShowUIPauseGame(bool show)
    {
        isPaused = show;
        GameManager.instance.PauseGame(isPaused);
        if (show)
        {
            fadePanel.SetActive(true);
            pausePanel.SetActive(true);
        }
        else
        {
            fadePanelAnim.SetTrigger("Out");
            pausePanelAnim.SetTrigger("Out");
            Invoke(nameof(OnFadeOut), .5f);
        }
    }

    public void WinOK()
    {
        if (GameData.instance != null)
        {
            GameData.instance.saveData.isActive[GameManager.instance.level + 1] = true;
            GameData.instance.Save();
        }
        SceneManager.LoadScene(splashSceneName);
    }

    public void LoseOK()
    {
        SceneManager.LoadScene(splashSceneName);
    }

    public void OnFadeOut()
    {
        fadePanel.SetActive(false);
        gameIntroPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    // public void OnPausePanelOut()
    // {
    //     pausePanel.SetActive(false);
    // }
}
