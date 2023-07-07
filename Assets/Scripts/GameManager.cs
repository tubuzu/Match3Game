using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause,
}
public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Level")]
    public World world;
    public int level;
    private Level levelInfo;

    [Header("Game state")]
    public GameState currentState = GameState.move;

    [Header("UI")]
    public GameObject movesLabel;
    public GameObject timeLabel;
    public Text counter;

    [Header("Win Condition")]
    public EndGameRequirements endGameRequirements;
    public int currentCounterValue;
    private float timer;

    private void Awake()
    {
        GameManager.instance = this;
        this.level = PlayerPrefs.GetInt("Current Level", 0);
        if (world != null)
        {
            if (world.levels[level] != null)
            {
                this.levelInfo = world.levels[level];
            }
        }
    }

    private void Start()
    {
        SetupGame();
        UIManager.instance.ShowUIGameIntro(true);
    }

    public Level GetLevelInfo()
    {
        return levelInfo;
    }

    private void SetupGame()
    {
        this.endGameRequirements = this.levelInfo.endGameRequirements;
        currentCounterValue = endGameRequirements.counterValue;
        if (endGameRequirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timer = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = currentCounterValue.ToString();

        // reset collected items
        for (int i = 0; i < levelInfo.levelGoals.Length; i++)
        {
            levelInfo.levelGoals[i].numberCollected = 0;
        }
    }

    public void DescreaseCounterValue()
    {
        if (currentState == GameState.pause || currentState == GameState.lose || currentState == GameState.wait) return;

        if (currentCounterValue >= 1)
        {
            currentCounterValue--;
            counter.text = currentCounterValue.ToString();
        }
        if (currentCounterValue <= 0)
        {
            currentState = GameState.lose;
            StartCoroutine(LoseGame());
        }
    }

    public void WinGame()
    {
        UIManager.instance.ShowUIWinGame(true);
        currentState = GameState.win;
    }

    // public void LoseGame()
    // {
    //     // Debug.Log("lose");
    //     UIManager.instance.ShowUILoseGame(true);
    //     currentState = GameState.lose;
    // }

    public IEnumerator LoseGame()
    {
        yield return new WaitUntil(() => currentState != GameState.wait);
        currentState = GameState.lose;
        UIManager.instance.ShowUILoseGame(true);
    }

    private void FixedUpdate()
    {
        if (endGameRequirements.gameType == GameType.Time)
        {
            timer -= Time.fixedDeltaTime;
            if (timer <= 0)
            {
                timer = 1;
                DescreaseCounterValue();
            }
        }
    }

    public IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);
        currentState = GameState.move;
    }

    public void PauseGame(bool pause)
    {
        if (pause) this.currentState = GameState.pause;
        else this.currentState = GameState.move;
    }

    public bool IsGamePaused()
    {
        return currentState == GameState.win || currentState == GameState.lose || currentState == GameState.pause;
    }
}
