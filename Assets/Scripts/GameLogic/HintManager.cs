using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public static HintManager instance;

    public float hintDelay;
    private float hintDelaySeconds;
    public GameObject hintParticle;
    public GameObject currentHint;

    public List<Dot> possibleMoves = new List<Dot>();

    private void Awake()
    {
        HintManager.instance = this;
    }

    private void Start()
    {
        hintDelaySeconds = hintDelay;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.IsGamePaused()) return;
        if (hintDelaySeconds > 0)
            hintDelaySeconds -= Time.fixedDeltaTime;
        if (hintDelaySeconds <= 0 && currentHint == null && GameManager.instance.currentState == GameState.move)
        {
            MarkHint();
            hintDelaySeconds = hintDelay;

            // #if UNITY_EDITOR
            //             UnityEditor.EditorApplication.isPaused = true;
            // #endif
        }
    }

    private List<Dot> FindAllMatches()
    {
        possibleMoves = new List<Dot>();
        for (int i = 0; i < Board.instance.width; i++)
        {
            for (int j = 0; j < Board.instance.height; j++)
            {
                if (Board.instance.allDots[i, j] == null) continue;
                if (Board.instance.IsMarkerTile(TileKind.Lock, i, j)) continue;

                if (i < Board.instance.width - 1)
                {
                    if (Board.instance.SwitchAndCheck(i, j, Vector2.right))
                    {
                        possibleMoves.Add(Board.instance.allDots[i, j]);
                    }
                }
                if (j < Board.instance.height - 1)
                {
                    if (Board.instance.SwitchAndCheck(i, j, Vector2.up))
                    {
                        possibleMoves.Add(Board.instance.allDots[i, j]);
                    }
                }
            }
        }
        return possibleMoves;
    }

    private Dot PickOneRandomly()
    {
        List<Dot> possibleMoves = new List<Dot>();
        possibleMoves = FindAllMatches();
        if (possibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUse];
        }
        return null;
    }

    private void MarkHint()
    {
        Dot move = PickOneRandomly();
        if (move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    public void DestroyHint()
    {
        if (currentHint == null) return;
        Destroy(currentHint);
        currentHint = null;
        hintDelaySeconds = hintDelay;
    }
}
