using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using System.Linq;

public class FindMatches : MonoBehaviour
{
    public static FindMatches instance;

    private void Awake()
    {
        FindMatches.instance = this;
    }

    public void FindAllMatches()
    {
        // Debug.Log("FindAllMatchesCo");
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return null;

        Dot[,] allDots = Board.instance.allDots;
        int width = Board.instance.width;
        int height = Board.instance.height;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Dot curDot = allDots[i, j];
                if (curDot == null)
                    continue;

                CheckHorizontalMatches(curDot, i, j);
                CheckVerticalMatches(curDot, i, j);
            }
        }

        MarkBombsAndExplode(allDots, width, height);
    }

    private void CheckHorizontalMatches(Dot curDot, int col, int row)
    {
        if (col > 0 && col < Board.instance.width - 1)
        {
            Dot leftDot = Board.instance.allDots[col - 1, row];
            Dot rightDot = Board.instance.allDots[col + 1, row];
            if (leftDot != null && rightDot != null)
            {
                if (leftDot.gameObject.tag == curDot.gameObject.tag && rightDot.gameObject.tag == curDot.gameObject.tag)
                {
                    leftDot.isMatched = true;
                    curDot.isMatched = true;
                    rightDot.isMatched = true;
                }
            }
        }
    }

    private void CheckVerticalMatches(Dot curDot, int col, int row)
    {
        if (row > 0 && row < Board.instance.height - 1)
        {
            Dot upDot = Board.instance.allDots[col, row + 1];
            Dot downDot = Board.instance.allDots[col, row - 1];
            if (upDot != null && downDot != null)
            {
                if (upDot.gameObject.tag == curDot.gameObject.tag && downDot.gameObject.tag == curDot.gameObject.tag)
                {
                    upDot.isMatched = true;
                    curDot.isMatched = true;
                    downDot.isMatched = true;
                }
            }
        }
    }

    private void MarkBombsAndExplode(Dot[,] allDots, int width, int height)
    {
        bool[] hasColBomb = new bool[width];
        bool[] hasRowBomb = new bool[height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Dot dot = allDots[i, j];
                if (dot == null || !dot.isMatched)
                    continue;

                if (dot.isRowBomb && !hasRowBomb[j])
                {
                    hasRowBomb[j] = true;
                    GetRowPieces(i, j);
                }
                else if (dot.isColumnBomb && !hasColBomb[i])
                {
                    hasColBomb[i] = true;
                    GetColumnPieces(i, j);
                }
                else if (dot.isAdjacentBomb)
                {
                    GetAdjacentPieces(i, j);
                }
            }
        }
    }

    //Doing
    private List<List<Dot>> GetMatchesGroups()
    {
        List<List<Dot>> dotGroups = new List<List<Dot>>();

        Dot[,] allDots = Board.instance.allDots;
        int width = Board.instance.width;
        int height = Board.instance.height;
        bool[,] visited = new bool[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (visited[i, j] || Board.instance.allDots[i, j] == null || !allDots[i, j].isMatched)
                    continue;

                List<Dot> dotGroup = new List<Dot>();
                TraverseNeighbors(i, j, dotGroup, visited, allDots);

                if (dotGroup.Count > 2)
                {
                    dotGroups.Add(dotGroup);
                }
            }
        }

        return dotGroups;
    }

    private void TraverseNeighbors(int col, int row, List<Dot> dotGroup, bool[,] visited, Dot[,] allDots)
    {
        int[] dy = { 0, 0, -1, 1 };
        int[] dx = { -1, 1, 0, 0 };

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(new Vector2Int(col, row));
        // visited[col, row] = true;

        while (queue.Count > 0)
        {
            Vector2Int position = queue.Dequeue();
            if (visited[position.x, position.y]) continue;

            if (Board.instance.CompareTag(position.x, position.y, position.x + 1, position.y) && Board.instance.CompareTag(position.x, position.y, position.x - 1, position.y))
            {
                if (!visited[position.x, position.y])
                {
                    dotGroup.Add(allDots[position.x, position.y]);
                    visited[position.x, position.y] = true;
                }
                queue.Enqueue(new Vector2Int(position.x + 1, position.y));
                queue.Enqueue(new Vector2Int(position.x - 1, position.y));
            }
            if (Board.instance.CompareTag(position.x, position.y, position.x - 1, position.y) && Board.instance.CompareTag(position.x, position.y, position.x - 2, position.y))
            {
                if (!visited[position.x, position.y])
                {
                    dotGroup.Add(allDots[position.x, position.y]);
                    visited[position.x, position.y] = true;
                }
                queue.Enqueue(new Vector2Int(position.x - 1, position.y));
                // queue.Enqueue(new Vector2Int(position.x - 2, position.y));
            }
            if (Board.instance.CompareTag(position.x, position.y, position.x + 1, position.y) && Board.instance.CompareTag(position.x, position.y, position.x + 2, position.y))
            {
                if (!visited[position.x, position.y])
                {
                    dotGroup.Add(allDots[position.x, position.y]);
                    visited[position.x, position.y] = true;
                }
                queue.Enqueue(new Vector2Int(position.x + 1, position.y));
                // queue.Enqueue(new Vector2Int(position.x + 2, position.y));
            }
            if (Board.instance.CompareTag(position.x, position.y, position.x, position.y + 1) && Board.instance.CompareTag(position.x, position.y, position.x, position.y - 1))
            {
                if (!visited[position.x, position.y])
                {
                    dotGroup.Add(allDots[position.x, position.y]);
                    visited[position.x, position.y] = true;
                }
                queue.Enqueue(new Vector2Int(position.x, position.y + 1));
                queue.Enqueue(new Vector2Int(position.x, position.y - 1));
            }
            if (Board.instance.CompareTag(position.x, position.y, position.x, position.y - 1) && Board.instance.CompareTag(position.x, position.y, position.x, position.y - 2))
            {
                if (!visited[position.x, position.y])
                {
                    dotGroup.Add(allDots[position.x, position.y]);
                    visited[position.x, position.y] = true;
                }
                queue.Enqueue(new Vector2Int(position.x, position.y - 1));
                // queue.Enqueue(new Vector2Int(position.x, position.y - 2));
            }
            if (Board.instance.CompareTag(position.x, position.y, position.x, position.y + 1) && Board.instance.CompareTag(position.x, position.y, position.x, position.y + 2))
            {
                if (!visited[position.x, position.y])
                {
                    dotGroup.Add(allDots[position.x, position.y]);
                    visited[position.x, position.y] = true;
                }
                queue.Enqueue(new Vector2Int(position.x, position.y + 1));
                // queue.Enqueue(new Vector2Int(position.x, position.y + 2));
            }
        }
    }

    private bool IsValidNeighbor(int col, int row, Dot[,] allDots, bool[,] visited, int prevCol, int prevRow)
    {
        int width = allDots.GetLength(0);
        int height = allDots.GetLength(1);

        if (row < 0 || row >= height || col < 0 || col >= width)
            return false;

        if (visited[col, row])
            return false;

        if (allDots[col, row].gameObject.tag != allDots[prevCol, prevRow].gameObject.tag)
            return false;

        return true;
    }

    private BombType GetBombType(List<Dot> group)
    {
        int maxMatchColumn = 0;
        int maxMatchRow = 0;
        // string msg = "";

        for (int i = 0; i < group.Count; i++)
        {
            // msg += "(" + group[i].column.ToString() + ", " + group[i].row.ToString() + ") ";
            int matchColumn = 0;
            int matchRow = 0;
            for (int j = 0; j < group.Count; j++)
            {
                if (j == i) continue;
                if (group[i].column == group[j].column) matchColumn++;
                else if (group[i].row == group[j].row) matchRow++;
            }
            maxMatchColumn = Mathf.Max(matchColumn, maxMatchColumn);
            maxMatchRow = Mathf.Max(matchRow, maxMatchRow);
        }
        // Debug.Log(msg);
        // Debug.Log("group.Count: " + group.Count.ToString() + ", row: " + (maxMatchRow + 1).ToString() + ", column: " + (maxMatchColumn + 1).ToString());
        if (maxMatchRow > 3 || maxMatchColumn > 3) return BombType.color;
        else if (maxMatchRow > 1 && maxMatchColumn > 1) return BombType.package;
        else if (maxMatchColumn > 2) return BombType.column;
        else if (maxMatchRow > 2) return BombType.row;
        else return BombType.NONE;
    }

    private Vector2Int GetBombPosition(List<Dot> group)
    {
        int col = 0, row = 0;
        Dot currentDot = Board.instance.currentDot;
        Dot otherDot = Board.instance.currentDot.otherDot;
        bool contain = false;

        for (int i = 0; i < group.Count; i++)
        {
            if ((group[i].column == currentDot.column && group[i].row == currentDot.row) || (group[i].column == otherDot.column && group[i].row == otherDot.row))
            {
                col = group[i].column;
                row = group[i].row;
                contain = true;
                break;
            }
        }

        if (!contain)
        {
            int randInt = Random.Range(0, group.Count);
            col = group[randInt].column;
            row = group[randInt].row;
        }

        return new Vector2Int(col, row);
    }

    public void CheckBomb()
    {
        List<List<Dot>> matchGroups = GetMatchesGroups();
        // Debug.Log("Lần này có " + matchGroups.Count.ToString() + " bom");

        foreach (List<Dot> group in matchGroups)
        {
            BombType type = GetBombType(group);
            // Debug.Log("BombType" + type.ToString());
            if (type == BombType.NONE) continue;
            // Debug.Log(type.ToString());
            Vector2Int bombPosition = GetBombPosition(group);
            // Debug.Log("bomb position: " + bombPosition[0].ToString() + "," + bombPosition[1].ToString());
            Dot bombDot = Board.instance.allDots[bombPosition[0], bombPosition[1]];
            if (bombDot != null) bombDot.MakeBombByType(type);
        }
        // Debug.Log("-----------");
    }
    //Doing

    public void MatchPiecesOfColor(string color)
    {
        // Debug.Log("MatchPiecesOfColor");
        for (int i = 0; i < Board.instance.width; i++)
        {
            for (int j = 0; j < Board.instance.height; j++)
            {
                if (Board.instance.allDots[i, j] == null || Board.instance.allDots[i, j].isMatched) continue;
                if (Board.instance.allDots[i, j].tag == color)
                {
                    Board.instance.allDots[i, j].isMatched = true;
                }
            }
        }
    }

    private void GetAdjacentPieces(int column, int row)
    {
        for (int i = Mathf.Max(column - 1, 0); i <= Mathf.Min(column + 1, Board.instance.width - 1); i++)
        {
            for (int j = Mathf.Max(row - 1, 0); j <= Mathf.Min(row + 1, Board.instance.height - 1); j++)
            {
                if (Board.instance.allDots[i, j] == null || Board.instance.allDots[i, j].isMatched) continue;
                Board.instance.allDots[i, j].isMatched = true;
            }
        }
    }

    private void GetColumnPieces(int column, int row)
    {
        for (int i = 0; i < Board.instance.height; i++)
        {
            if (Board.instance.allDots[column, i] == null || Board.instance.allDots[column, i].isMatched) continue;
            if (Board.instance.allDots[column, row].isColorBomb)
            {
                MatchPiecesOfColor(Board.instance.allDots[column, row].gameObject.tag);
            }
            Board.instance.allDots[column, i].isMatched = true;
        }
    }

    private void GetRowPieces(int column, int row)
    {
        // Debug.Log("Get row pieces");
        for (int i = 0; i < Board.instance.width; i++)
        {
            if (Board.instance.allDots[i, row] == null || Board.instance.allDots[i, row].isMatched) continue;
            if (Board.instance.allDots[column, row].isColorBomb)
            {
                MatchPiecesOfColor(Board.instance.allDots[column, row].gameObject.tag);
            }
            Board.instance.allDots[i, row].isMatched = true;
        }
    }

    private T GetSafeValue<T>(T[,] array, int x, int y)
    {
        if (x >= 0 && x < array.GetLength(0) && y >= 0 && y < array.GetLength(1))
        {
            return array[x, y];
        }
        else
        {
            return default(T);
        }
    }
}
