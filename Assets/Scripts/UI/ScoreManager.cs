using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;
    public Text scoreText;
    public int score;
    public Image scoreBar;
    public int[] scoreGoals;
    public int starAchieved;

    private void Awake()
    {
        ScoreManager.instance = this;
    }

    private void Start()
    {
        Setup();
        UpdateBar();
    }

    private void Setup()
    {
        Level info = GameManager.instance.GetLevelInfo();
        if (info != null)
        {
            this.scoreGoals = info.scoreGoals;
        }
        starAchieved = 0;
    }

    private void FixedUpdate()
    {
        scoreText.text = "" + score;
    }

    public void IncreaseScore(int amountToIncrease)
    {
        score += amountToIncrease;
        GameData.instance.UpdateHighestScore(score);
        if (starAchieved < scoreGoals.Length && score >= scoreGoals[starAchieved])
        {
            starAchieved++;
            GameData.instance.UpdateLevelStar(starAchieved);
        }
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (Board.instance != null && scoreBar != null)
        {

            int length = this.scoreGoals.Length;

            scoreBar.fillAmount = (float)score / (float)this.scoreGoals[length - 1];
        }
    }
}
