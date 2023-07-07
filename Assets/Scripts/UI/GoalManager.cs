using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;
    public bool completed;
    public Sprite goalSprite;
    public DotTag tag;
}

public enum GoalCompletionType
{
    getANumber,
    getAll,
}

public class GoalManager : MonoBehaviour
{
    public static GoalManager instance;

    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();

    public GameObject goalPrefab;
    public Transform goalIntroParent;
    public Transform goalGameParent;

    public float goalIntroScale = 1.2f;
    public float goalGameScale = 0.8f;

    private void Awake()
    {
        GoalManager.instance = this;
    }

    private void Start()
    {
        SetupGoals();
    }

    private void SetupGoals()
    {
        Level info = GameManager.instance.GetLevelInfo();
        if (info != null)
        {
            this.levelGoals = info.levelGoals;
        }
        for (int i = 0; i < levelGoals.Length; i++)
        {
            GameObject goal = Instantiate(goalPrefab, goalIntroParent.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroParent);
            goal.transform.localScale = Vector3.one;
            goal.transform.localScale = Vector3.one * goalIntroScale;

            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            if (GetGoalCompletionType(levelGoals[i].tag) == GoalCompletionType.getANumber)
                panel.thisString = "0/" + levelGoals[i].numberNeeded;
            else panel.thisText.transform.parent.gameObject.SetActive(false);

            goal = Instantiate(goalPrefab, goalGameParent.position, Quaternion.identity);
            goal.transform.SetParent(goalGameParent);
            goal.transform.localScale = Vector3.one;
            goal.transform.localScale = Vector3.one * goalGameScale;

            panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            if (GetGoalCompletionType(levelGoals[i].tag) == GoalCompletionType.getANumber)
                panel.thisString = "0/" + levelGoals[i].numberNeeded;
            else panel.thisText.transform.parent.gameObject.SetActive(false);

            levelGoals[i].completed = false;

            currentGoals.Add(panel);
        }
    }

    public void CheckGetAllGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (GetGoalCompletionType(levelGoals[i].tag) == GoalCompletionType.getAll)
            {
                switch (levelGoals[i].tag)
                {
                    case DotTag.BreakableDot:
                        if (Board.instance.GetListTiles(Board.instance.breakableTiles).Count == 0) levelGoals[i].completed = true;
                        break;
                    case DotTag.RockDot:
                        if (Board.instance.GetListTiles(Board.instance.rockTiles).Count == 0) levelGoals[i].completed = true;
                        break;
                    case DotTag.SlimeDot:
                        if (Board.instance.GetListTiles(Board.instance.slimeTiles).Count == 0) levelGoals[i].completed = true;
                        break;
                    case DotTag.LockDot:
                        if (Board.instance.GetListTiles(Board.instance.lockTiles).Count == 0) levelGoals[i].completed = true;
                        break;
                }
            }
        }
        CheckAllCompleted();
    }

    public void CheckGetANumberGoal(string goalToCompare)
    {
        DotTag tag;
        System.Enum.TryParse(goalToCompare, out tag);
        if (GetGoalCompletionType(tag) == GoalCompletionType.getANumber)
        {
            for (int i = 0; i < levelGoals.Length; i++)
            {
                if (goalToCompare != levelGoals[i].tag.ToString()) continue;
                levelGoals[i].numberCollected++;
                if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
                {
                    levelGoals[i].completed = true;
                    currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
                }
                else
                {
                    currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;
                }
                break;
            }
        }
    }

    public void CheckAllCompleted()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (!levelGoals[i].completed) return;
        }
        GameManager.instance.WinGame();
    }

    public GoalCompletionType GetGoalCompletionType(DotTag tag)
    {
        switch (tag)
        {
            case DotTag.BlueDot:
            case DotTag.GreenDot:
            case DotTag.GrayDot:
            case DotTag.OrangeDot:
            case DotTag.LightGreenDot:
            case DotTag.RedDot:
                return GoalCompletionType.getANumber;
            case DotTag.BreakableDot:
            case DotTag.RockDot:
            case DotTag.SlimeDot:
            case DotTag.LockDot:
                return GoalCompletionType.getAll;
            default:
                return GoalCompletionType.getANumber;
        }
    }
}
