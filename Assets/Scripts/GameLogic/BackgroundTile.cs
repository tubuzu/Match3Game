// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public int hitPoints;
    private int curPoints;
    private SpriteRenderer sprite;

    public int column;
    public int row;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        curPoints = hitPoints;
    }

    public void TakeDamage(int damage)
    {
        curPoints -= damage;
        MakeLighter();
        if (curPoints <= 0)
        {
            if (GoalManager.instance != null)
            {
                GoalManager.instance.CheckGetANumberGoal(this.gameObject.tag);
            }
            Destroy(this.gameObject);
        }
    }

    private void MakeLighter()
    {
        Color color = sprite.color;
        float newAlpha = color.a * ((float)curPoints / hitPoints);
        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }

    public void SetPosition(int column, int row)
    {
        this.column = column;
        this.row = row;
    }
}
