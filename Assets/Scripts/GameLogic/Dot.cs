using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public enum BombType
{
    NONE,
    row,
    column,
    color,
    package,
}

public enum DotTag
{
    GreenDot,
    OrangeDot,
    PirpleDot,
    RedDot,
    BlueDot,
    YellowDot,
    LightGreenDot,
    GrayDot,
    ColorDot,
    BreakableDot,
    RockDot,
    SlimeDot,
    LockDot,
}

public class Dot : MonoBehaviour
{
    [Header("Interact")]
    private Vector2 firstTouchPosition = Vector2.zero;
    private Vector2 finalTouchPosition = Vector2.zero;
    private Vector2 tempPosition;

    [Header("Reference Object")]
    public Dot otherDot;
    public bool isMatched = false;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0;
    public float swipeResist = .5f;

    [Header("Dot Properties")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public Color effectColor;

    [Header("Powerup Stuff")]
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isColorBomb;
    public bool isAdjacentBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentMarker;

    [Header("Animation")]
    private Animator anim;
    private float shineDelay;

    private float stepDelay = 0.5f;
    private bool isMoving = false;

    private void Start()
    {
        this.anim = GetComponent<Animator>();
        this.shineDelay = Random.Range(3f, 6f);

        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;

        InvokeRepeating(nameof(Shine), shineDelay, shineDelay);
    }

    public void Initialize(int targetX, int targetY)
    {
        this.targetX = targetX;
        this.targetY = targetY;
        column = this.targetX;
        row = this.targetY;
        previousColumn = column;
        previousRow = row;
    }

    public void Shine()
    {
        anim.SetTrigger("Shine");
    }

    public void Pop()
    {
        anim.SetTrigger("Pop");
        // anim.SetBool("Pop", true);
    }

    public void Squish(bool horizontal)
    {
        if (horizontal)
            anim.SetTrigger("SquishHorizontal");
        else anim.SetTrigger("SquishVertical");
    }

    public void Touch()
    {
        anim.SetTrigger("Touch");
    }

    // //testing and debug only
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            // isColumnBomb = true;
            // GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            // arrow.transform.parent = this.transform;

            // isRowBomb = true;
            // GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            // arrow.transform.parent = this.transform;

            // isColorBomb = true;
            // GameObject bomb = Instantiate(colorBomb, transform.position, Quaternion.identity);
            // bomb.transform.parent = this.transform;

            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.IsGamePaused()) return;
        targetX = column;
        targetY = row;
        tempPosition = new Vector2(targetX, targetY);
        if (Mathf.Abs(targetX - transform.localPosition.x) > .1 || Mathf.Abs(targetY - transform.localPosition.y) > .1)
        {
            bool wasMoving = this.isMoving;
            this.isMoving = true;

            transform.localPosition = Vector2.Lerp(transform.localPosition, tempPosition, .2f);

            if (Board.instance.allDots[targetX, targetY] != this)
            {
                Board.instance.allDots[targetX, targetY] = this;
            }
            if (!Board.instance.needToCheckMatches && !wasMoving)
                Board.instance.needToCheckMatches = true;
        }
        else
        {
            this.isMoving = false;
            transform.localPosition = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if (otherDot == null) yield break;

        if (isColorBomb)
        {
            FindMatches.instance.MatchPiecesOfColor(otherDot.gameObject.tag);
            isMatched = true;
        }
        else if (otherDot.isColorBomb)
        {
            FindMatches.instance.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.isMatched = true;
        }

        yield return new WaitForSeconds(this.stepDelay);

        if (!isMatched && !otherDot.isMatched)
        {
            otherDot.column = column;
            otherDot.row = row;
            column = previousColumn;
            row = previousRow;
            yield return new WaitForSeconds(this.stepDelay);
            Board.instance.currentDot = null;
            GameManager.instance.currentState = GameState.move;
        }
        else
        {
            if (GameManager.instance.endGameRequirements.gameType == GameType.Moves) GameManager.instance.DescreaseCounterValue();
            Board.instance.DestroyMatches();
        }
    }

    private void OnMouseDown()
    {
        if (HintManager.instance != null)
        {
            HintManager.instance.DestroyHint();
        }
        if (GameManager.instance.currentState == GameState.move)
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        if (GameManager.instance.currentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            GameManager.instance.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

            Board.instance.currentDot = this;

            MovePieces();
        }
        else
        {
            Touch();
            GameManager.instance.currentState = GameState.move;
        }
    }

    private void MovePiecesActual(Vector2 direction)
    {
        otherDot = Board.instance.allDots[column + (int)direction.x, row + (int)direction.y];

        if (otherDot == null)
        {
            GameManager.instance.currentState = GameState.move;
            return;
        }

        if (Board.instance.IsMarkerTile(TileKind.Lock, column, row) || Board.instance.IsMarkerTile(TileKind.Lock, otherDot.column, otherDot.row))
        {
            GameManager.instance.currentState = GameState.move;
            return;
        }

        if (direction == Vector2.up || direction == Vector2.down) Squish(false);
        else Squish(true);

        otherDot.column += -1 * (int)direction.x;
        otherDot.row += -1 * (int)direction.y;
        previousColumn = column;
        previousRow = row;
        column += (int)direction.x;
        row += (int)direction.y;

        StartCoroutine(CheckMoveCo());
    }

    private void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < Board.instance.width - 1)
        {
            //right
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < Board.instance.height - 1)
        {
            //up
            MovePiecesActual(Vector2.up);
        }
        else if (swipeAngle > 135 || swipeAngle <= -135 && column > 0)
        {
            //left
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle > -135 && swipeAngle <= -45 && row > 0)
        {
            //down
            MovePiecesActual(Vector2.down);
        }
        else
        {
            GameManager.instance.currentState = GameState.move;
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColorBomb()
    {
        isColorBomb = true;
        GameObject bomb = Instantiate(colorBomb, transform.position, Quaternion.identity);
        bomb.transform.parent = this.transform;
        this.gameObject.tag = DotTag.ColorDot.ToString();
    }

    public void MakeAdjacentBomb()
    {
        isAdjacentBomb = true;
        GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }

    public void MakeBombByType(BombType type)
    {
        if (!this.isMatched) return;
        if (this.IsBomb())
        {
            foreach (Transform t in this.transform)
            {
                if (t.gameObject.tag == "Bomb")
                {
                    Destroy(t.gameObject);
                }
            }
        }

        this.isMatched = false;
        switch (type)
        {
            case BombType.row:
                MakeRowBomb();
                break;
            case BombType.column:
                MakeColumnBomb();
                break;
            case BombType.package:
                MakeAdjacentBomb();
                break;
            case BombType.color:
                MakeColorBomb();
                break;
            default:
                break;
        }
    }

    public bool IsBomb()
    {
        return isRowBomb || isColumnBomb || isAdjacentBomb || isColorBomb;
    }

}
