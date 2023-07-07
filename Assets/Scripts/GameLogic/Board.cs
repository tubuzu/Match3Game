using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileKind
{
    Breakable,
    Blank,
    Lock,
    Rock,
    Slime,
    Normal,
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}
public class Board : MonoBehaviour
{
    public static Board instance;

    [Header("Board")]
    public int width;
    public int height;
    public Vector2 offset;
    public TileType[] boardLayout;

    [Header("Prefabs")]
    public GameObject[] dots;
    public GameObject destoryEffect;
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject lockTilePrefab;
    public GameObject rockTilePrefab;
    public GameObject slimeTilePrefab;

    [Header("Tiles")]
    public Dot[,] allDots;
    public Dot currentDot;
    private bool[,] blankSpaces;
    public BackgroundTile[,] breakableTiles;
    public BackgroundTile[,] lockTiles;
    public BackgroundTile[,] rockTiles;
    public BackgroundTile[,] slimeTiles;

    [Header("Scoring")]
    public int basePieceValue = 20;
    private int streakValue = 1;

    private float refillDelay = .5f;
    public bool needToCheckMatches = false;
    private bool canMakeSlime = true;

    private void Awake()
    {
        Board.instance = this;
    }

    private void Start()
    {
        Level info = GameManager.instance.GetLevelInfo();
        if (info != null)
        {
            this.width = info.width;
            this.height = info.height;
            this.dots = info.dots;
            this.boardLayout = info.boardLayout;
        }

        breakableTiles = new BackgroundTile[width, height];
        lockTiles = new BackgroundTile[width, height];
        slimeTiles = new BackgroundTile[width, height];
        rockTiles = new BackgroundTile[width, height];
        blankSpaces = new bool[width, height];
        allDots = new Dot[width, height];

        Setup();

        GameManager.instance.currentState = GameState.pause;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.IsGamePaused()) return;
        if (needToCheckMatches)
        {
            FindMatches.instance.FindAllMatches();
            needToCheckMatches = false;
        }
    }

    public void GenerateTiles()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if (boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
            else if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                GenerateMarkerTile(TileKind.Breakable, i, breakableTilePrefab, breakableTiles);
            }
            else if (boardLayout[i].tileKind == TileKind.Lock)
            {
                GenerateMarkerTile(TileKind.Lock, i, lockTilePrefab, lockTiles);
            }
            else if (boardLayout[i].tileKind == TileKind.Rock)
            {
                GenerateMarkerTile(TileKind.Rock, i, rockTilePrefab, rockTiles);
            }
            else if (boardLayout[i].tileKind == TileKind.Slime)
            {
                GenerateMarkerTile(TileKind.Slime, i, slimeTilePrefab, slimeTiles);
            }
        }
    }

    public void GenerateMarkerTile(TileKind kind, int layoutIndex, GameObject prefab, BackgroundTile[,] tiles)
    {
        Vector2 tempPosition = new Vector2(boardLayout[layoutIndex].x + offset.x, boardLayout[layoutIndex].y + offset.y);
        GameObject tile = Instantiate(prefab, tempPosition, Quaternion.identity);
        tile.transform.parent = transform;
        tiles[boardLayout[layoutIndex].x, boardLayout[layoutIndex].y] = tile.GetComponent<BackgroundTile>();
        tiles[boardLayout[layoutIndex].x, boardLayout[layoutIndex].y].SetPosition(boardLayout[layoutIndex].x, boardLayout[layoutIndex].y);
    }

    private void Setup()
    {
        GenerateTiles();
        // test
        //         int[,] testBoard = new int[,]
        // {
        //     { 1, 0 },
        //     { 1, 0 },
        //     { 0, 1 },
        //     { 1, 0 },
        // };
        // // {
        // //     { 0, 0, 0, 0, 0, 0, 0 },
        // //     { 1, 0, 0, 0, 0, 0, 0 },
        // //     { 1, 0, 0, 0, 1, 1, 1 },
        // //     { 1, 0, 0, 0, 1, 0, 0 },
        // //     { 1, 1, 1, 1, 1, 0, 0 },
        // //     { 0, 0, 0, 0, 0, 0, 0 },
        // //     { 1, 1, 1, 0, 1, 1, 1 },
        // //     { 0, 0, 1, 0, 1, 0, 0 },
        // //     { 0, 0, 1, 1, 1, 0, 0 },
        // // };

        //         int rowCount = testBoard.GetLength(0);
        //         int colCount = testBoard.GetLength(1);

        //         int[,] reversedBoard = new int[colCount, rowCount];

        //         for (int i = 0; i < rowCount; i++)
        //         {
        //             for (int j = 0; j < colCount; j++)
        //             {
        //                 reversedBoard[j, i] = testBoard[i, j];
        //             }
        //         }
        // test
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (blankSpaces[i, j]) continue;

                Vector2 tempPosition = new Vector2(i + offset.x, j + offset.y);

                GameObject backgorundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                backgorundTile.transform.parent = this.transform;
                backgorundTile.name = "(" + i + ", " + j + ")";

                if (rockTiles[i, j] || slimeTiles[i, j]) continue;

                int dotToUse = Random.Range(0, dots.Length);
                // test
                // int dotToUse = 0;
                // if (reversedBoard[i, j] == 1) dotToUse = 1;
                // test
                int maxIterations = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "(" + i + ", " + j + ")";

                allDots[i, j] = dot.GetComponent<Dot>();
                allDots[i, j].Initialize(i, j);
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && allDots[column - 1, row] != null && allDots[column - 2, row] != null)
        {
            if (allDots[column - 1, row].gameObject.tag == piece.tag && allDots[column - 2, row].gameObject.tag == piece.tag)
                return true;
        }
        if (row > 1 && allDots[column, row - 1] != null && allDots[column, row - 2] != null)
        {
            if (allDots[column, row - 1].gameObject.tag == piece.tag && allDots[column, row - 2].gameObject.tag == piece.tag)
                return true;
        }

        return false;
    }

    public void DestroyMatches()
    {
        FindMatches.instance.CheckBomb();

        // #if UNITY_EDITOR
        //         UnityEditor.EditorApplication.isPaused = true;
        // #endif

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }

        StartCoroutine(DecreaseRow());
    }

    private bool TileTakeDamage(BackgroundTile[,] tiles, int column, int row)
    {
        if (tiles[column, row] != null)
        {
            tiles[column, row].TakeDamage(1);
            if (tiles[column, row].hitPoints <= 0)
            {
                tiles[column, row] = null;
            }
            return true;
        }
        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row] == null) return;
        if (!allDots[column, row].isMatched) return;

        TileTakeDamage(breakableTiles, column, row);
        TileTakeDamage(lockTiles, column, row);
        DamageSurroundMarkerTiles(rockTiles, column, row);
        if (DamageSurroundMarkerTiles(slimeTiles, column, row)) canMakeSlime = false;

        if (GoalManager.instance != null)
        {
            GoalManager.instance.CheckGetANumberGoal(allDots[column, row].tag.ToString());
        }

        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayRandomSFX();
        }

        GameObject particle = Instantiate(destoryEffect, allDots[column, row].transform.position, Quaternion.identity);

        Color color = allDots[column, row].effectColor;
        ParticleSystem[] particleSystems = particle.transform.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null)
            {
                var mainModule = ps.main;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(color);
            }
        }

        Destroy(particle, .5f);
        allDots[column, row].Pop();
        Destroy(allDots[column, row].gameObject);

        ScoreManager.instance.IncreaseScore(basePieceValue * streakValue);

        allDots[column, row] = null;
    }

    private bool DamageSurroundMarkerTiles(BackgroundTile[,] tiles, int column, int row)
    {
        bool result = false;
        if (column > 0 && TileTakeDamage(tiles, column - 1, row))
        {
            result = true;
        }
        if (column < width - 1 && TileTakeDamage(tiles, column + 1, row))
        {
            result = true;
        }
        if (row > 0 && TileTakeDamage(tiles, column, row - 1))
        {
            result = true;
        }
        if (row < height - 1 && TileTakeDamage(tiles, column, row + 1))
        {
            result = true;
        }
        return result;
    }

    private void CheckToMakeSlime()
    {
        int maxSlimeSpawn = 3;
        int slimeSpawned = 0;
        List<BackgroundTile> slimeTilesList = GetListTiles(slimeTiles);
        int count = slimeTilesList.Count;
        while (canMakeSlime && slimeSpawned < maxSlimeSpawn && slimeSpawned < count)
        {
            if (MakeNewSlime(slimeTilesList))
                slimeSpawned++;
        }
    }

    private Vector2 CheckForAdjacent(int column, int row)
    {
        List<Vector2> directions = new List<Vector2>();

        if (column < width - 1 && allDots[column + 1, row] && !slimeTiles[column + 1, row])
        {
            directions.Add(Vector2.right);
        }
        if (column > 0 && allDots[column - 1, row] && !slimeTiles[column - 1, row])
        {
            directions.Add(Vector2.left);
        }
        if (row < height - 1 && allDots[column, row + 1] && !slimeTiles[column, row + 1])
        {
            directions.Add(Vector2.up);
        }
        if (row > 0 && allDots[column, row - 1] && !slimeTiles[column, row - 1])
        {
            directions.Add(Vector2.down);
        }

        if (directions.Count > 0)
        {
            int randomIndex = Random.Range(0, directions.Count);
            return directions[randomIndex];
        }

        return Vector2.zero;
    }

    private bool MakeNewSlime(List<BackgroundTile> slimeTilesList)
    {
        int randInt = Random.Range(0, slimeTilesList.Count);
        Vector2 adjacent = CheckForAdjacent(slimeTilesList[randInt].column, slimeTilesList[randInt].row);
        if (adjacent != Vector2.zero)
        {
            Vector2Int tempPosition = new Vector2Int(slimeTilesList[randInt].column + (int)adjacent.x, slimeTilesList[randInt].row + (int)adjacent.y);
            Destroy(allDots[tempPosition.x, tempPosition.y].gameObject);
            GameObject tile = Instantiate(slimeTilePrefab, tempPosition + offset, Quaternion.identity);
            slimeTiles[tempPosition.x, tempPosition.y] = tile.GetComponent<BackgroundTile>();
            slimeTiles[tempPosition.x, tempPosition.y].SetPosition(tempPosition.x, tempPosition.y);
            slimeTilesList.RemoveAt(randInt);
            return true;
        }
        return false;
    }

    public List<BackgroundTile> GetListTiles(BackgroundTile[,] tiles)
    {
        List<BackgroundTile> result = new List<BackgroundTile>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] != null) result.Add(tiles[i, j]);
            }
        }
        return result;
    }

    private IEnumerator DecreaseRow()
    {
        yield return new WaitForSeconds(refillDelay);
        for (int i = 0; i < width; i++)
        {
            Queue emptyQueue = new Queue();
            for (int j = 0; j < height; j++)
            {
                if (blankSpaces[i, j] || rockTiles[i, j] || slimeTiles[i, j]) continue;

                if (allDots[i, j] == null) emptyQueue.Enqueue(j);
                else if (allDots[i, j] != null && emptyQueue.Count > 0)
                {
                    allDots[i, j].row = (int)emptyQueue.Dequeue();
                    allDots[i, j] = null;
                    emptyQueue.Enqueue(j);
                }
            }
        }

        yield return new WaitForSeconds(refillDelay);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null || blankSpaces[i, j] || rockTiles[i, j] || slimeTiles[i, j]) continue;

                Vector2 tempPosition = new Vector2(i + offset.x, 15 + offset.y);

                int dotToUse = Random.Range(0, dots.Length);
                int maxIterations = 0;
                while (MatchesAt(i, j, dots[dotToUse].gameObject) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }

                GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                piece.transform.parent = this.transform;
                allDots[i, j] = piece.GetComponent<Dot>();
                allDots[i, j].Initialize(i, j);
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].isMatched)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(refillDelay);

        if (MatchesOnBoard())
        {
            // Debug.Log("matchesOnBoard");
            streakValue++;
            yield return new WaitForSeconds(refillDelay);
            DestroyMatches();
        }
        else
        {
            streakValue = 1;
            currentDot = null;
            CheckToMakeSlime();

            yield return new WaitForSeconds(refillDelay);
            canMakeSlime = true;

            if (IsDeadLocked())
            {
                ShuffleBoard();
                // Debug.Log("Deadlocked");
            }

            if (GameManager.instance.currentState != GameState.pause)
            {
                GameManager.instance.currentState = GameState.move;
                GoalManager.instance.CheckGetAllGoals();
            }
        }
    }

    public bool ValidPosition(int col, int row)
    {
        bool validPos = col >= 0 && col < width && row >= 0 && row < height;
        return validPos && allDots[col, row] != null;
    }

    public bool CompareTag(int col1, int row1, int col2, int row2)
    {
        if (!ValidPosition(col1, row1) || !ValidPosition(col2, row2)) return false;
        return allDots[col1, row1].gameObject.tag == allDots[col2, row2].gameObject.tag;
    }

    public bool CheckMatchesAtDot(int col, int row)
    {
        if (CompareTag(col, row, col + 1, row) && CompareTag(col, row, col - 1, row)) return true;
        else if (CompareTag(col, row, col - 1, row) && CompareTag(col, row, col - 2, row)) return true;
        else if (CompareTag(col, row, col + 1, row) && CompareTag(col, row, col + 2, row)) return true;
        else if (CompareTag(col, row, col, row + 1) && CompareTag(col, row, col, row - 1)) return true;
        else if (CompareTag(col, row, col, row - 1) && CompareTag(col, row, col, row - 2)) return true;
        else if (CompareTag(col, row, col, row + 1) && CompareTag(col, row, col, row + 2)) return true;
        return false;
    }

    private void SwitchDot(int col1, int row1, int col2, int row2)
    {
        Dot temp = allDots[col1, row1];
        allDots[col1, row1] = allDots[col2, row2];
        allDots[col2, row2] = temp;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        Vector2Int switchPos = new Vector2Int(column + (int)direction.x, row + (int)direction.y);
        if (lockTiles[switchPos.x, switchPos.y] || rockTiles[switchPos.x, switchPos.y] || slimeTiles[switchPos.x, switchPos.y]) return false;

        if (allDots[column, row].gameObject.tag == allDots[switchPos.x, switchPos.y].gameObject.tag) return false;
        SwitchDot(column, row, switchPos.x, switchPos.y);
        bool result = CheckMatchesAtDot(column, row) || CheckMatchesAtDot(switchPos.x, switchPos.y);
        SwitchDot(column, row, switchPos.x, switchPos.y);
        return result;
    }

    private bool IsDeadLocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null) continue;

                if (i < width - 1)
                {
                    if (SwitchAndCheck(i, j, Vector2.right))
                        return false;
                }
                if (j < height - 1)
                {
                    if (SwitchAndCheck(i, j, Vector2.up))
                        return false;
                }
            }
        }
        return true;
    }

    public void ShuffleBoard()
    {
        List<Dot> newBoard = new List<Dot>();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null) continue;
                newBoard.Add(allDots[i, j]);
            }
        }
        System.Array.Clear(allDots, 0, allDots.Length);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (blankSpaces[i, j] || rockTiles[i, j] || slimeTiles[i, j]) continue;
                int pieceToUse = Random.Range(0, newBoard.Count);

                int maxIterations = 0;
                while (MatchesAt(i, j, newBoard[pieceToUse].gameObject) && maxIterations < 100)
                {
                    pieceToUse = Random.Range(0, newBoard.Count);
                    maxIterations++;
                }

                Dot piece = newBoard[pieceToUse];
                piece.column = i;
                piece.row = j;
                allDots[i, j] = piece;
                newBoard.Remove(piece);
            }
        }
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }
    }

    public bool IsMarkerTile(TileKind kind, int column, int row)
    {
        if (kind == TileKind.Lock)
            return lockTiles[column, row] != null;
        if (kind == TileKind.Breakable)
            return breakableTiles[column, row] != null;
        if (kind == TileKind.Rock)
            return rockTiles[column, row] != null;
        if (kind == TileKind.Slime)
            return slimeTiles[column, row] != null;
        return false;
    }
}
